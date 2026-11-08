namespace StoryBuilder.Core.Models;

/// <summary>
/// A reusable page layout shared by authoring tools and reading/rendering clients.
/// Geometry is normalized to the page bounds (0..1); content remains linked to canonical records.
/// </summary>
public sealed record PageComposition
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ManuscriptSectionId { get; init; }
    public Guid? VisualAssetId { get; init; }
    public string Name { get; init; } = "Untitled page";
    public PageCompositionOrientation Orientation { get; init; } = PageCompositionOrientation.Portrait;
    public PageCompositionFit ImageFit { get; init; } = PageCompositionFit.Contain;
    public PageCompositionRegion TextRegion { get; init; } = new() { X = 0.08, Y = 0.72, Width = 0.84, Height = 0.2 };
    public PageCompositionRegion ImageRegion { get; init; } = new() { X = 0.05, Y = 0.05, Width = 0.9, Height = 0.62 };
    public string? FocalPoint { get; init; }
    public int LayerOrder { get; init; }
    /// <summary>Optional normalized region that readers must discover to advance this page interactively.</summary>
    public PageCompositionRegion? DiscoveryRegion { get; init; }
    public string? DiscoveryPrompt { get; init; }
    /// <summary>Seconds before the discovery target begins its gentle breathing hint.</summary>
    public int DiscoveryHintDelaySeconds { get; init; } = 12;
}

public sealed record PageCompositionRegion
{
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; } = 1;
    public double Height { get; init; } = 1;
}

public enum PageCompositionOrientation { Portrait, Landscape, Square }
public enum PageCompositionFit { Contain, Cover, Fill }
