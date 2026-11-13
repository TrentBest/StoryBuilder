using System.Security.Cryptography;
using System.Text;
using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;
using StoryBuilder.Persistence;
using TheSingularityWorkshop.FSM_Serialization;
using Xunit;

namespace StoryBuilder.Core.Tests;

public sealed class InWorldPublicationEditionTests
{
    [Fact]
    public void VesperReachEditionsPreserveIndependentAccountsCorrectionLineageAndKnowledgeBoundaries()
    {
        var session = new StoryBuilderSession("Vesper Reach publication proof");
        var timeline = session.CreateTimeline("Station chronology");
        var signalSource = session.AddSource(new SourceRecord
        {
            Title = "Signal instrument record",
            SourceType = "Instrument record",
            Perspective = "Station analysis"
        });
        var officialSource = session.AddSource(new SourceRecord
        {
            Title = "Official lounge bulletin",
            SourceType = "Public bulletin",
            Perspective = "Station administration"
        });
        var independentSource = session.AddSource(new SourceRecord
        {
            Title = "Independent courier account",
            SourceType = "Witness account",
            Perspective = "Independent courier"
        });

        var signal = session.CreateTimelineEvent(
            timeline.Id,
            "An anomalous signal is detected.",
            "Station instruments detect an anomalous signal beyond the settled frontier.",
            "Station shift 41");
        signal = signal with { SourceIds = [signalSource.Id], Status = CanonStatus.Accepted };
        session.Workspace.ReplaceEvent(signal);

        var initialClaim = new HistoricalClaim
        {
            Statement = "The signal appears to originate beyond the settled frontier.",
            Attribution = "Station analysis",
            Confidence = 0.7,
            Status = CanonStatus.Accepted,
            SourceIds = [signalSource.Id],
            RelatedEntityIds = [signal.Id]
        };
        var independentClaim = new HistoricalClaim
        {
            Statement = "The courier account challenges the preliminary direction estimate.",
            Attribution = "Independent courier account",
            Confidence = 0.6,
            Status = CanonStatus.Proposed,
            SourceIds = [independentSource.Id],
            RelatedEntityIds = [signal.Id]
        };
        var correctedClaim = new HistoricalClaim
        {
            Statement = "The available measurements establish detection but not a reliable direction.",
            Attribution = "Station analysis correction",
            Confidence = 0.9,
            Status = CanonStatus.Accepted,
            SourceIds = [signalSource.Id, officialSource.Id],
            RelatedEntityIds = [signal.Id]
        };
        session.AddHistoricalClaim(initialClaim);
        session.AddHistoricalClaim(independentClaim);
        session.AddHistoricalClaim(correctedClaim);

        var officialOutlet = new PublicationOutlet
        {
            Name = "Vesper Reach Station Bulletin",
            ArtifactType = "Public bulletin",
            Description = "Official station-issued notices."
        };
        var courierOutlet = new PublicationOutlet
        {
            Name = "Independent Courier Ledger",
            ArtifactType = "Witness dispatch",
            Description = "Independently authored courier accounts."
        };
        session.Workspace.AddPublicationOutlet(officialOutlet);
        session.Workspace.AddPublicationOutlet(courierOutlet);

        var initial = CreateEdition(
            officialOutlet.Id, 1, "Preliminary signal bulletin",
            "Station instruments report an anomalous signal beyond the settled frontier. Initial direction estimate is provisional.",
            "Station shift 41, 00:14",
            [initialClaim.Id], [officialSource.Id, signalSource.Id], [signal.Id]);
        var independentEventIds = new List<Guid> { signal.Id };
        var independent = CreateEdition(
            courierOutlet.Id, 1, "Courier's independent account",
            "The courier disputes the bulletin's preliminary direction estimate.",
            "Station shift 41, 00:27",
            [independentClaim.Id], [independentSource.Id], independentEventIds);
        var correction = CreateEdition(
            officialOutlet.Id, 2, "Correction: signal direction remains unconfirmed",
            "Correction: detection is confirmed, but the available measurements do not establish a reliable direction.",
            "Station shift 41, 00:43",
            [correctedClaim.Id], [signalSource.Id, officialSource.Id], [signal.Id],
            initial.Id, "Narrows the preliminary direction claim; the original bulletin remains preserved.");

        session.Workspace.AddInWorldPublicationEdition(initial);
        session.Workspace.AddInWorldPublicationEdition(independent);
        session.Workspace.AddInWorldPublicationEdition(correction);
        independentEventIds.Clear();

        var ilyan = session.CreateCharacter("Ilyan Voss", "Signal analyst");
        var mara = session.CreateCharacter("Mara Quill", "Courier");
        session.UpdateCharacter(ilyan);
        session.UpdateCharacter(mara);
        var projections = new ObserverKnowledgeProjectionService();
        Assert.Empty(projections.Project(session.Workspace, ilyan.Id, signal.Id).Events);
        Assert.Empty(projections.Project(session.Workspace, mara.Id, signal.Id).Events);
        Assert.Equal(0, session.Workspace.ObserverKnowledge.Count);

        Assert.Empty(StoryWorkspaceValidator.Validate(session.Workspace));
        var originalContent = initial.Content;
        var originalHash = initial.ContentSha256;
        var originalCreatedAt = initial.CreatedAtUtc;

        using var output = new MemoryBinaryStream();
        StoryWorkspaceBinaryDocument.FromSession(session).Pack(output);
        using var input = new MemoryBinaryStream(output.ToArray());
        var restored = new StoryWorkspaceBinaryDocument();
        restored.Unpack(input);

        var restoredInitial = Assert.Single(restored.Workspace.InWorldPublicationEditions, item => item.Id == initial.Id);
        var restoredIndependent = Assert.Single(restored.Workspace.InWorldPublicationEditions, item => item.Id == independent.Id);
        var restoredCorrection = Assert.Single(restored.Workspace.InWorldPublicationEditions, item => item.Id == correction.Id);
        Assert.Equal(originalContent, restoredInitial.Content);
        Assert.Equal(originalHash, restoredInitial.ContentSha256);
        Assert.Equal(originalCreatedAt, restoredInitial.CreatedAtUtc);
        Assert.Equal(initial.Id, restoredCorrection.SupersedesEditionId);
        Assert.Equal(1, restoredInitial.EditionNumber);
        Assert.Equal(2, restoredCorrection.EditionNumber);
        Assert.Equal("Station shift 41, 00:14", restoredInitial.InWorldPublishedAt);
        Assert.Contains(signal.Id, restoredIndependent.EventIds);
        Assert.Single(restoredIndependent.EventIds);
        Assert.Contains(independentClaim.Id, restoredIndependent.ClaimIds);
        Assert.Equal(2, restored.Workspace.PublicationOutlets.Count);
        Assert.Empty(restored.Workspace.ObserverKnowledge);
        Assert.Empty(StoryWorkspaceValidator.Validate(restored.Workspace));
    }

    [Fact]
    public void GenericArchivalCatalogArtifactUsesTheSamePublicationContract()
    {
        var workspace = new StoryWorkspace { Name = "Museum archive" };
        var outlet = new PublicationOutlet
        {
            Name = "North Gallery Archive",
            ArtifactType = "Archival catalog",
            Description = "Catalogs issued records for a public collection."
        };
        workspace.AddPublicationOutlet(outlet);
        var edition = CreateEdition(
            outlet.Id, 1, "Catalog entry: restored field journal",
            "A field journal was restored and cataloged with its provenance record.",
            "Museum year 18, spring catalog",
            [], [], []);
        workspace.AddInWorldPublicationEdition(edition);

        Assert.Empty(StoryWorkspaceValidator.Validate(workspace));
        Assert.Equal("Archival catalog", Assert.Single(workspace.PublicationOutlets).ArtifactType);
    }

    [Fact]
    public void PublicationEditionRejectsMissingAndCrossOutletLineage()
    {
        var workspace = new StoryWorkspace();
        var firstOutlet = new PublicationOutlet { Name = "First outlet" };
        var secondOutlet = new PublicationOutlet { Name = "Second outlet" };
        workspace.AddPublicationOutlet(firstOutlet);
        workspace.AddPublicationOutlet(secondOutlet);
        var original = CreateEdition(firstOutlet.Id, 1, "Original", "Original content", "Year 1", [], [], []);
        workspace.AddInWorldPublicationEdition(original);

        var crossOutletCorrection = CreateEdition(
            secondOutlet.Id, 2, "Invalid correction", "Changed content", "Year 2", [], [], [], original.Id);
        Assert.Throws<InvalidOperationException>(() => workspace.AddInWorldPublicationEdition(crossOutletCorrection));

        var missingOutletEdition = CreateEdition(Guid.NewGuid(), 1, "Missing outlet", "Content", "Year 1", [], [], []);
        Assert.Throws<KeyNotFoundException>(() => workspace.AddInWorldPublicationEdition(missingOutletEdition));
    }

    private static InWorldPublicationEdition CreateEdition(
        Guid outletId,
        int number,
        string title,
        string content,
        string inWorldPublishedAt,
        IReadOnlyList<Guid> claimIds,
        IReadOnlyList<Guid> sourceIds,
        IReadOnlyList<Guid> eventIds,
        Guid? supersedesEditionId = null,
        string? correctionNotes = null) => new()
    {
        OutletId = outletId,
        EditionNumber = number,
        Title = title,
        Content = content,
        ContentFormat = "text/plain",
        ContentSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))),
        InWorldPublishedAt = inWorldPublishedAt,
        IntendedAudience = "Station residents",
        AccessScope = "Publicly issued within the fictional setting",
        ClaimIds = claimIds,
        SourceIds = sourceIds,
        EventIds = eventIds,
        SupersedesEditionId = supersedesEditionId,
        CorrectionNotes = correctionNotes
    };
}
