using StoryBuilder.Core.Models;
using StoryBuilder.Core.Services;

namespace StoryBuilder.Quickstart;

/// <summary>
/// Builds a provisional, source-attributed StoryBuilder workspace from raWWar's living Empire history.
/// The workspace deliberately contains no invented in-world dates or accepted historical canon.
/// </summary>
public static class RaWWarEmpireWorkspace
{
    public const string SourceUri = "https://github.com/TrentBest/raWWar/blob/development/docs/EMPIRE_HISTORY_AND_FACTION_ORIGINS.md";

    public static StoryBuilderSession Create()
    {
        var session = new StoryBuilderSession("raWWar — Galactic Empire History");
        session.Workspace.Experience = new ExperienceDefinition
        {
            Name = "raWWar Galactic Empire historical foundation",
            Purpose = "Author a source-grounded, causally coherent history for the Galactic Empire and its thirteen major Houses.",
            Audience = "raWWar creators, collaborators, and future campaign authors",
            Constraints =
            [
                "Names, dates, exact rulers, and specific historical events remain refinable.",
                "House descriptions are provisional summaries, not totalizing faction definitions.",
                "Distinguish source statements from connective interpretation.",
                "No proposed record becomes accepted canon without explicit review."
            ]
        };

        var source = session.AddSource(new SourceRecord
        {
            Title = "raWWar — Empire History & Faction Origins",
            SourceType = "Living design document",
            LocationOrUri = SourceUri,
            Perspective = "Authorial design model describing the Empire's consolidation, political fractures, and the origins of thirteen major Houses.",
            ReliabilityNote = "Primary project source for this workspace. The source explicitly leaves names, dates, exact rulers, and specific historical events refinable; claims here remain Proposed."
        });

        var houses = new[]
        {
            ("House Veyr", "House", "A major House associated in the source with stability and preservation of order."),
            ("House Cael", "House", "A major House associated in the source with Imperial administration."),
            ("House Oris", "House", "A major House associated in the source with industrial power and production."),
            ("House Sere", "House", "A major House associated in the source with independent knowledge and research autonomy."),
            ("House Avar", "House", "A major House associated in the source with cultural preservation and resistance to homogenization."),
            ("House Talan", "House", "A major House associated in the source with merchant independence and alternative trade networks."),
            ("House Merid", "House", "A major House associated in the source with political consent and representation."),
            ("House Rhen", "House", "A major House associated in the source with frontier self-reliance."),
            ("House Solenne", "House", "A major House associated in the source with civilian welfare and protection."),
            ("House Kharos", "House", "A major House associated in the source with professional military competence."),
            ("House Nyr", "House", "A major House associated in the source with local autonomy and self-government."),
            ("House Voss", "House", "A major House associated in the source with revolutionary opposition to the existing order."),
            ("House Draven", "House", "A major House associated in the source with uncompromising resistance to the Empire in its present form.")
        };

        var organizations = houses.ToDictionary(
            house => house.Item1,
            house => session.CreateOrganization(house.Item1, house.Item2, house.Item3));

        foreach (var house in houses)
        {
            var organization = organizations[house.Item1];
            session.AddHistoricalClaim(new HistoricalClaim
            {
                Statement = $"{organization.Name} is described by the source as a major House associated with {house.Item3[house.Item3.IndexOf("associated in the source with ", StringComparison.Ordinal)..].TrimEnd('.')}.",
                Attribution = "raWWar design source",
                Status = CanonStatus.Proposed,
                SourceIds = [source.Id],
                RelatedEntityIds = [organization.Id]
            });
        }

        var historicalClaims = new[]
        {
            "Human civilization expanded across many star systems and developed diverse local governments and institutions before the Empire emerged.",
            "The Empire's early consolidation offered shared trade, navigation, arbitration, defense, and infrastructure that individual worlds struggled to provide alone.",
            "The Imperial Compact exchanged portions of local sovereignty for Imperial guarantees while leaving worlds local law, culture, leadership, property, military traditions, and economic institutions.",
            "Expansion created a Distance Crisis: communication, travel, reinforcement, inspection, governance, and repair became slower and less reliable across the growing Empire.",
            "The Imperial court responded to the Distance Crisis with centralizing reforms that expanded administrative, military, industrial, navigational, and resource oversight.",
            "The source frames the thirteen Houses as different historical responses to pressures created by the Empire and its expansion, rather than factions invented solely to fill a game roster.",
            "The current Empress inherited the galaxy's existing divisions and uses rivalries, favors, debts, contracts, appointments, protection, intelligence, and selective punishment or mercy to preserve Imperial authority.",
            "The Houses remain divided in part because they disagree about what should replace the Empress and the current Imperial order.",
            "The source presents each House as a historical philosophy under pressure, with strengths that can become weaknesses; the Houses are not simple morality categories.",
            "Faction origins are intended to connect historical pressures to institutions, collective memory, values, dispositions, doctrine, capabilities, and behavior, while preserving tendencies rather than hard-coded destinies."
        };

        foreach (var statement in historicalClaims)
        {
            session.AddHistoricalClaim(new HistoricalClaim
            {
                Statement = statement,
                Attribution = "raWWar design source",
                Status = CanonStatus.Proposed,
                SourceIds = [source.Id]
            });
        }

        var timeline = session.CreateTimeline(
            "Galactic Empire — provisional historical structure",
            "A source-grounded outline of historical pressures and political transformations. Planning points are structural anchors, not dated events or accepted canon.");

        var outline = new[]
        {
            ("Pre-Imperial Expansion", "Establish the diverse worlds, local governments, trade leagues, protectorates, and settlements that predate Imperial consolidation."),
            ("Imperial Consolidation", "Explore the shared problems the early Empire solved and the legitimacy it initially earned."),
            ("The Imperial Compact", "Clarify the negotiated division between local sovereignty and Imperial responsibilities."),
            ("The Distance Crisis", "Develop the consequences of scale for communication, travel, governance, logistics, and defense."),
            ("The First Imperial Reforms", "Trace centralizing responses and the institutional classes they empowered."),
            ("The Industrial Acceleration", "Explore industrial capacity, strategic dependence, and the rise of Oris."),
            ("The Knowledge Accords", "Explore research autonomy, access, and the rise of Sere."),
            ("The Preservation Wars", "Explore cultural continuity and resistance to homogenization."),
            ("The Merchant Revolt", "Explore trade regulation, merchant networks, and the rise of Talan."),
            ("The Civic Crisis", "Explore consent, representation, and the legitimacy of Imperial authority."),
            ("The Frontier Compact", "Explore distance, survival, and frontier self-reliance."),
            ("The Civilian Reckoning", "Explore extraction, inequality, civilian protection, and welfare."),
            ("The Professional Armies", "Explore military specialization and the relationship between command competence and political authority."),
            ("The Secession Crisis", "Explore local autonomy and resistance to centralized government."),
            ("The Revolution", "Explore movements that conclude reform is insufficient."),
            ("The Unbound", "Explore uncompromising resistance and the historical memory attributed to Draven."),
            ("The Empress", "Explore inherited rivalries and the political mechanisms used to preserve Imperial power.")
        };

        foreach (var (label, intent) in outline)
            session.CreatePlanningPoint(timeline.Id, label, intent: intent);

        session.AddContinuityIssue(new ContinuityIssue
        {
            Description = "The source does not establish absolute dates, reign lengths, or a complete ordering for every House origin. Resolve chronology only when supported by additional raWWar material or an explicit author decision.",
            Status = CanonStatus.Proposed
        });
        session.AddContinuityIssue(new ContinuityIssue
        {
            Description = "The thirteen House summaries are deliberately high-level. Expand internal factions, regional variation, dissenting members, and changing doctrine before treating any disposition as universal.",
            Status = CanonStatus.Proposed
        });
        session.AddContinuityIssue(new ContinuityIssue
        {
            Description = "Separate direct source statements from connective historical interpretation as the universe expands; retain source attribution and author review for every new claim.",
            Status = CanonStatus.Proposed
        });

        return session;
    }
}
