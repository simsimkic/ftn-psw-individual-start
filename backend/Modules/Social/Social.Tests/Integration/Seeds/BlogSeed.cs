using Shared.Tests;
using Social.Domain.Blogs;

namespace Social.Tests.Integration.Seeds;

internal static class BlogSeed
{
    public static readonly Blog FortressImpressions = new(WellKnownUsers.Guide, "Utisci sa tvrđave", "Obilazak Petrovaradinske tvrđave ostavio je snažan utisak: podzemne galerije, Sahat kula i pogled na Dunav.", ["https://example.com/tvrdjava.jpg"]);
    public static readonly Blog CityNotes = new(WellKnownUsers.Guide, "Beleške iz grada", "Kratke beleške sa šetnje centrom grada.", []);
    public static readonly Blog SecondGuidesDiary = new(WellKnownUsers.SecondGuide, "Tuđi dnevnik", "Beleške drugog autora, vidljive samo njemu.", []);
    public static readonly Blog PublishedRiverbank;
    public static readonly Blog CommentedMuseumVisit;

    static BlogSeed()
    {
        PublishedRiverbank = new(WellKnownUsers.Guide, "Zalasci na keju", "Fotografije i utisci sa večernje šetnje novosadskim kejom.", []);
        PublishedRiverbank.Publish();

        CommentedMuseumVisit = new(WellKnownUsers.Guide, "Poseta muzeju", "Utisci sa stalne postavke Muzeja Vojvodine.", []);
        CommentedMuseumVisit.Publish();
        CommentedMuseumVisit.AddComment(WellKnownUsers.Tourist, "Odlična preporuka!");
    }

    public static Blog[] All => [FortressImpressions, CityNotes, SecondGuidesDiary, PublishedRiverbank, CommentedMuseumVisit];
}
