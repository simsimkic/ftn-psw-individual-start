using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Social.Domain.Blogs;

namespace Social.Infrastructure.Persistence;

internal sealed class SocialModuleInitializer : IHostedService
{
    private static readonly Guid FirstGuide = Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid SecondGuide = Guid.Parse("30000000-0000-0000-0000-000000000002");
    private static readonly Guid FirstTourist = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid SecondTourist = Guid.Parse("20000000-0000-0000-0000-000000000002");
    private static readonly Guid ThirdTourist = Guid.Parse("20000000-0000-0000-0000-000000000003");

    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;

    public SocialModuleInitializer(IServiceProvider serviceProvider, IHostEnvironment environment)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<SocialDbContext>();
        await dbContext.Database.MigrateAsync();

        if (_environment.IsDevelopment() && !await dbContext.Blogs.AnyAsync())
        {
            dbContext.Blogs.AddRange(CreateDemoBlogs());
            await dbContext.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static List<Blog> CreateDemoBlogs()
    {
        var example = new Blog(FirstGuide, "Modul Social je primer",
            "Blog je primer jedne celine urađene kroz sve slojeve: agregat sa entitetom komentara, komande i upiti, repozitorijumi, kontroleri, testovi i stranice na klijentu. Svoje funkcionalnosti pravite u modulu Exploration, po istom obrascu.",
            []);
        example.Publish();
        example.AddComment(FirstTourist, "Savet: pre kodiranja pročitajte kako je blog napravljen, od agregata do stranice.");
        example.AddComment(SecondTourist, "Primer prati strukturu, ali ne i domen vašeg projekta. Domen modelujete sami.");
        example.AddComment(ThirdTourist, "Pratite strukturu primera, čak i kad deluje jednostavno.");

        var server = new Blog(FirstGuide, "Kako je organizovan server",
            "Server je modularni monolit na ASP.NET Core platformi. Svaki modul ima svoju šemu u bazi i svoje slojeve, a arhitektonski testovi proveravaju da slojevi poštuju pravila zavisnosti.",
            []);
        server.Publish();
        server.AddComment(FirstTourist, "Savet: poslovna pravila idu u agregat, a ne u servis ili kontroler.");
        server.AddComment(SecondTourist, "Pokrenite sve testove pre svakog commit-a. Srećno sa migracijama!");
        server.AddComment(ThirdTourist, "Kad kompajler odbije referencu između projekata, to je namerno.");

        var client = new Blog(SecondGuide, "Kako je organizovan klijent",
            "Klijent je Angular aplikacija čiji moduli prate module servera. Tipovi za komunikaciju sa serverom se pišu ručno, kao verni odraz DTO-ova sa servera.",
            []);
        client.Publish();
        client.AddComment(FirstTourist, "Savet: upite pišite kao httpResource u stranici, a komande u servisu grupe.");
        client.AddComment(SecondTourist, "Frontend nije strašan koliko izgleda. Srećno!");
        client.AddComment(ThirdTourist, "Kad menjate DTO na serveru, u istoj izmeni ažurirajte i njegov tip na klijentu.");

        var knowledgeBase = new Blog(SecondGuide, "Baza znanja kursa",
            "U direktorijumu docs/knowledge-base se nalaze lekcije koje objašnjavaju svaki koncept koji projekat koristi, od arhitekture modula do migracija baze.",
            []);
        knowledgeBase.Publish();
        knowledgeBase.AddComment(FirstTourist, "Savet: krenite od INDEX.md i pratite preduslove svake lekcije.");
        knowledgeBase.AddComment(SecondTourist, "Primeri u lekcijama su pojednostavljeni i namerno se razlikuju od projekta.");
        knowledgeBase.AddComment(ThirdTourist, "Pročitajte lekciju pre nego što pitate. Srećno sa učenjem!");

        return [example, server, client, knowledgeBase];
    }
}
