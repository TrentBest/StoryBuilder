using StoryBuilder.FSM_COS;
using StoryBuilder.Persistence;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class StoryBuilderLocalExperienceSessionTests
{
    [Fact]
    public async Task Open_restores_and_dispose_persists_the_local_experience()
    {
        var root = Path.Combine(Path.GetTempPath(), "StoryBuilder", Guid.NewGuid().ToString("N"));
        var repository = new LocalStoryWorkspaceRepository(root);
        var host = new StoryBuilderExperienceHost();

        try
        {
            Guid workspaceId;
            await using (var opened = await StoryBuilderLocalExperienceSession.OpenAsync(
                host, repository, "city", "Tacoma"))
            {
                Assert.Equal("city: Tacoma", opened.Hosted.Experience.Session.Workspace.Name);
                Assert.Contains(opened.Hosted.Assembly.Bundles, bundle =>
                    bundle.Id == opened.Hosted.Experience.Id);
                workspaceId = opened.Hosted.Experience.Session.Workspace.Id;
            }

            await using var reopened = await StoryBuilderLocalExperienceSession.OpenAsync(
                host, repository, "city", "Tacoma");

            Assert.Equal(workspaceId, reopened.Hosted.Experience.Session.Workspace.Id);
            Assert.Equal("city: Tacoma", reopened.Hosted.Experience.Session.Workspace.Name);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }
}
