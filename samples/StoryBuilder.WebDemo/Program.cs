using Microsoft.Extensions.FileProviders;
using StoryBuilder.MicroBundle;
using StoryBuilder.Core.Services;
using StoryBuilder.FSM;
using StoryBuilder.FSM_COS;\nusing StoryBuilder.Persistence;
using StoryBuilder.WebDemo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<StoryBuilderExperienceHost>();
builder.Services.AddScoped<StoryBuilderHostedExperience>(services =>
    services.GetRequiredService<StoryBuilderExperienceHost>().Create("story", "StoryBuilder Web Demo"));
builder.Services.AddScoped<StoryLifecycleRegistry>();
builder.Services.AddScoped<NarratorAgent>(services => new NarratorAgent(services.GetRequiredService<StoryBuilderSession>()));
builder.Services.AddScoped<StoryBuilderPersistenceCoordinator>();
builder.Services.AddScoped<StoryBuilderSession>(services =>
    services.GetRequiredService<StoryBuilderHostedExperience>().Experience.Session);

var workspacePath = Path.GetFullPath(\n    builder.Configuration["StoryBuilder:Workspaces:StoragePath"]\n    ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "workspaces"));\nDirectory.CreateDirectory(workspacePath);\nbuilder.Services.AddSingleton(new LocalStoryWorkspaceRepository(workspacePath));\n\nvar visualAssetPath = Path.GetFullPath(
    builder.Configuration["StoryBuilder:VisualAssets:StoragePath"]
    ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "visual-assets"));
Directory.CreateDirectory(visualAssetPath);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(visualAssetPath),
    RequestPath = "/visual-assets"
});
app.UseAntiforgery();
app.MapRazorComponents<StoryBuilder.WebDemo.Components.App>()
    .AddInteractiveServerRenderMode();
app.Run();

public partial class Program { }
