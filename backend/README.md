# Struktura serverske aplikacije (backend)

Serverska aplikacija je modularni monolit izgrađen na platformi ASP.NET Core. Ceo sistem se prevodi u jednu aplikaciju i koristi jednu PostgreSQL bazu podataka, pri čemu svaki modul ima sopstvenu šemu u toj bazi.

Deo koda je postavljen unapred i čini osnovu projekta (platformski deo), a deo pišete vi. Uz svaku stavku je navedeno da li je menjate. Platformski deo ne menjate: ako vam se čini da je izmena neophodna, prvo se javite asistentu.

## Explorer.slnx

Predstavlja rešenje (engl. *solution*) koje okuplja sve projekte serverske aplikacije.

Datoteka je deo platforme. Menja se samo kada se u rešenje dodaje nov projekat ili kada se postojeći uklanja, što u individualnom projektu nije potrebno.

## Directory.Build.props

Sadrži podešavanja prevođenja (engl. *build*) koja važe za sve projekte u rešenju. U njoj je definisana ciljna verzija platforme (.NET 10), uključena je provera referenci koje mogu biti `null` i podešeno je da se upozorenja prevodioca tretiraju kao greške. Zahvaljujući ovoj datoteci, pojedinačni projekti ne moraju da ponavljaju ista podešavanja.

Datoteka je deo platforme i ne menja se.

## Directory.Packages.props

Sadrži spisak svih spoljnih biblioteka (NuGet paketa) koje rešenje koristi, zajedno sa njihovim verzijama. Verzije se određuju isključivo na ovom mestu, pa pojedinačni projekti navode samo ime paketa, bez verzije. Time se sprečava da različiti moduli koriste različite verzije iste biblioteke.

Datoteka je deo platforme. Menja se samo kada se u sistem uvodi nova biblioteka, a uvođenje nove biblioteke je odluka koju prethodno dogovarate sa asistentom.

## Host.Api

Sadrži projekat koji predstavlja ulaznu tačku i kompozicioni koren cele aplikacije. To je jedini projekat koji se pokreće. U njemu se rešava registracija svih modula i osnovna konfiguracija procesa obrade HTTP odgovora i zahteva (provera JWT tokena, middleware koji izuzetke pretvara u HTTP odgovore). Tu su i konfiguracione datoteke `appsettings.json` i `appsettings.Development.json`, u kojima su podaci za povezivanje sa bazom i ključ za potpisivanje tokena.

Projekat je deo platforme i ne menja se, jer su svi moduli već registrovani.

## Host.Tests

Sadrži projekat sa arhitektonskim testovima. Ti testovi proveravaju da li projekti poštuju pravila zavisnosti (npr. da domenski sloj ne zavisi od tehnoloških detalja i da jedan modul ne pristupa unutrašnjosti drugog modula). Testovi se izvršavaju i lokalno i u sistemu kontinualne integracije, pa narušavanje pravila obara izgradnju.

Projekat je deo platforme i ne menja se. Ako ovi testovi prijave grešku, ispravlja se kod, a ne test. Vrste arhitektonskih testova i pravila koja čuvaju objašnjava lekcija `docs/knowledge-base/server/3-modularni-monolit/3-arhitektonski-testovi.md`.

## Shared

Sadrži zajednički kod koji koriste svi moduli. Podeljen je na četiri projekta:
- `Shared.Api` sadrži pomoćni kod za kontrolerski sloj (npr. metodu `GetUserId`, kojom kontroler čita identifikator prijavljenog korisnika iz tokena).
- `Shared.Domain` sadrži klase `Entity` i `AggregateRoot`, koje nasleđuju domenske klase (vrednosni objekat se modeluje kroz C# record). Izuzecima `DomainException` i `NotFoundException` domen prijavljuje narušavanje pravila, odnosno nepostojanje traženog podatka. Klasa `PageResult` je oblik rezultata upita koji vraća stranicu liste.
- `Shared.Infrastructure` sadrži pomoćni kod za sloj infrastrukture (npr. metoda `AddModuleDbContext`, koja registruje bazu podataka modula tako da modul dobije sopstvenu šemu i sopstvenu tabelu istorije migracija).
- `Shared.Tests` sadrži pomoćni kod za integracione testove: klasu `ExplorerApiFactory`, koja svakom test projektu obezbeđuje sopstvenu testnu bazu (obara je i ponovo kreira jednom po pokretanju testova), metodu `Reseed` za vraćanje podataka na početno stanje pre svakog testa, izdavanje test tokena (`CreateClientFor`), metodu `CreateContext`, kroz koju provera čita bazu, i klasu `WellKnownUsers` sa fiksnim identifikatorima korisnika koje testovi koriste.

Sva četiri projekta su deo platforme. Ne menjate ih: kod koji pišete ostaje u modulu `Exploration`.

## Modules

Ovaj direktorijum sadrži module aplikacije. Ovde se odvija najveći deo vašeg rada, i to isključivo u modulu `Exploration`.

### Identity

Modul `Identity` je izuzetak od strukture ostalih modula. To je platformski projekat, zadužen za registraciju i prijavu korisnika i za izdavanje JWT tokena. Ostali moduli ga ne referenciraju, već korisnika pamte samo preko njegovog identifikatora. Uz njega postoji i projekat `Identity.Tests`. Njegovi testovi koriste prave krajnje tačke za registraciju i prijavu, pa nisu uzor za testove funkcionalnih modula. Uzor je test projekat modula `Social`.

### Funkcionalni moduli

Funkcionalni moduli su `Exploration` i `Social`. Modul `Exploration` je prazan i u njemu pravite sve funkcionalnosti iz specifikacije projekta. Modul `Social` sadrži gotov primer (blogovi sa komentarima) koji prikazuje jednu celinu kroz sve slojeve, od agregata do testova. Koristite ga kao uzor i ne menjajte ga. Oba modula imaju istovetnu strukturu od šest projekata:
- `<Ime>.Api` sadrži kontrolere koji primaju HTTP zahtev, pozivaju jednu metodu aplikacionog sloja i vraćaju HTTP odgovor.
- `<Ime>.Application` sadrži aplikacione servise i upitne klase koji opisuju kako se koordinišu domenski objekti i tehničke sposobnosti da se ispuni slučaj korišćenja. Tu su i DTO klase, čije instance prihvataju i vraćaju aplikacioni servisi, profili za mapiranje DTO u domenski objekat i obratno, kao i interfejsi od infrastrukturnih servisa.
- `<Ime>.Contracts` sadrži interfejs koji modul nudi drugim modulima i po potrebi DTO klase čije instance se razmenjuju između modula. To je jedini deo modula koji drugi moduli smeju da referenciraju. Navedene interfejse implementiraju aplikacioni servisi.
- `<Ime>.Domain` sadrži domenske objekte (koren agregata, entiteti, vrednosni objekti) i domenske servise. Ovde se implementiraju domenski koncepti i pravila.
- `<Ime>.Infrastructure` sadrži implementacije infrastrukturnih servisa, poput repozitorijuma, konektorskih klasa i tehničkih stručnjačkih klasa koje koriste biblioteke. U njemu su `DbContext`, EF konfiguracije, migracije, implementacije repozitorijuma i upita.
- `<Ime>.Tests` sadrži testove modula, podeljene na direktorijume `Unit` i `Integration`. Jedinični testovi proveravaju pravila domena, a integracioni testovi šalju prave HTTP zahteve serverskoj aplikaciji koja radi nad testnom bazom podataka. Pravila za pisanje testova navedena su u odeljku Testing dokumenta `backend/AGENTS.md`.

U modulu `Exploration` ovih šest projekata menjate pri razvoju svake nove funkcionalnosti. Projekat `Exploration.Contracts` ostaje prazan, jer u individualnom projektu nema razmene podataka između modula.
