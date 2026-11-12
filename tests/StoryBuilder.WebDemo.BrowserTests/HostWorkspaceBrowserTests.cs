using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Playwright;
using Xunit;

namespace StoryBuilder.WebDemo.BrowserTests;

public sealed class HostWorkspaceBrowserTests
{
    [Fact]
    public async Task ManuscriptSurvivesBrowserReloadAndWebHostRestart()
    {
        var repositoryRoot = FindRepositoryRoot();
        var appDll = Path.Combine(repositoryRoot, "samples", "StoryBuilder.WebDemo", "bin", "Release", "net8.0", "StoryBuilder.WebDemo.dll");
        Assert.True(File.Exists(appDll), $"WebDemo build output was not found: {appDll}");

        var workspacePath = Path.Combine(Path.GetTempPath(), $"storybuilder-browser-{Guid.NewGuid():N}");
        Directory.CreateDirectory(workspacePath);
        var port = GetFreePort();
        var address = $"http://127.0.0.1:{port}";
        Process? host = null;

        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            page.SetDefaultTimeout(10_000);

            host = StartHost(appDll, repositoryRoot, workspacePath, address);
            await NavigateWhenReadyAsync(page, address);

            const string manuscriptTitle = "Browser persistence proof";
            const string sectionTitle = "A stable chapter";
            const string manuscriptText = "This text must survive a real browser reload and a host process restart.";

            await page.GetByLabel("New book title").FillAsync(manuscriptTitle);
            await page.GetByRole(AriaRole.Button, new() { Name = "Create book" }).ClickAsync();
            await page.GetByLabel("Section title").FillAsync(sectionTitle);
            await page.GetByRole(AriaRole.Button, new() { Name = "Add section" }).ClickAsync();
            await page.GetByLabel("Actual manuscript text").FillAsync(manuscriptText);
            await page.GetByRole(AriaRole.Button, new() { Name = "Save and refresh product preview" }).ClickAsync();

            await ExpectWorkspaceContentAsync(page, manuscriptTitle, sectionTitle, manuscriptText);

            await page.ReloadAsync(new PageReloadOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
            await ExpectWorkspaceContentAsync(page, manuscriptTitle, sectionTitle, manuscriptText);

            StopHost(host);
            host = StartHost(appDll, repositoryRoot, workspacePath, address);
            await NavigateWhenReadyAsync(page, address);
            await ExpectWorkspaceContentAsync(page, manuscriptTitle, sectionTitle, manuscriptText);
        }
        finally
        {
            if (host is not null)
                StopHost(host);
            try { Directory.Delete(workspacePath, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    private static async Task ExpectWorkspaceContentAsync(IPage page, string title, string section, string body)
    {
        await page.GetByRole(AriaRole.Heading, new() { Name = title }).WaitForAsync();
        await page.GetByRole(AriaRole.Heading, new() { Name = section }).WaitForAsync();
        await page.GetByText(body, exact: true).WaitForAsync();
    }

    private static async Task NavigateWhenReadyAsync(IPage page, string address)
    {
        Exception? lastError = null;
        for (var attempt = 0; attempt < 30; attempt++)
        {
            try
            {
                await page.GotoAsync(address, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 2_000
                });
                await page.GetByText("Host workspace autosave is ready. Your words remain yours to revise.")
                    .Or(page.GetByText("Saved on host"))
                    .WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });
                return;
            }
            catch (PlaywrightException ex)
            {
                lastError = ex;
                await Task.Delay(500);
            }
        }

        throw new InvalidOperationException($"WebDemo did not become ready at {address}.", lastError);
    }

    private static Process StartHost(string appDll, string repositoryRoot, string workspacePath, string address)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false
        };
        start.ArgumentList.Add(appDll);
        start.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        start.Environment["ASPNETCORE_URLS"] = address;
        start.Environment["StoryBuilder__Workspaces__StoragePath"] = workspacePath;

        var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start the StoryBuilder WebDemo host.");
        return process;
    }

    private static void StopHost(Process process)
    {
        if (process.HasExited)
        {
            process.Dispose();
            return;
        }

        process.Kill(entireProcessTree: true);
        if (!process.WaitForExit(10_000))
            throw new TimeoutException("StoryBuilder WebDemo host did not stop.");
        process.Dispose();
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string FindRepositoryRoot()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, "samples", "StoryBuilder.WebDemo")))
                    return directory.FullName;
                directory = directory.Parent;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the StoryBuilder repository root.");
    }
}
