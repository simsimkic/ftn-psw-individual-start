# Explorer: početni projekat za individualni rad

Ovo je početni projekat za individualni projekat iz predmeta Projektovanje softvera (projekat za ocene 6 i 7). Aplikacija je modularni monolit (ASP.NET Core, Angular, PostgreSQL), čija je arhitektura već postavljena. Vaš zadatak je da u nju ugradite funkcionalnosti iz specifikacije projekta. Gradivo koje predmet pokriva i koje primenjujete u projektu je u [bazi znanja](docs/knowledge-base).

## Šta već postoji

- **Arhitektura.** Serverska aplikacija je podeljena na module, a svaki modul na slojeve (Api, Application, Domain, Infrastructure). Pravila zavisnosti između slojeva proveravaju arhitektonski testovi u projektu `Host.Tests`, pa narušeno pravilo obara `dotnet test`. Detalji su u [backend/README.md](backend/README.md).
- **Registracija, prijava i uloge.** Modul `Identity` registruje korisnike, prijavljuje ih i izdaje JWT token. Postoje dve uloge: `tourist` (turista) i `guide` (vodič). Kroz aplikaciju se registruju samo turisti, a vodiči se unose u bazu (vidite [Prijava](#prijava)).
- **Primer jedne celine kroz sve slojeve.** Modul `Social` sadrži blogove sa komentarima: agregat sa entitetom, komande i upite, repozitorijum agregata i repozitorijum za čitanje, kontrolere, jedinične i integracione testove i Angular stranice. Vodič piše i objavljuje blog, a svaki prijavljeni korisnik ga čita i komentariše. Kada niste sigurni kako nešto da napravite, pogledajte kako je to urađeno u ovom modulu.
- **Prazan modul `Exploration`**, u kome pravite sve funkcionalnosti iz specifikacije (vidite [Gde radite](#gde-radite)).
- **Mape.** Biblioteka Leaflet je već dodata u klijentsku aplikaciju, zajedno sa svojim stilovima.
- **Baza znanja** (`docs/knowledge-base`) sa lekcijama o svemu što projekat koristi i **protokoli** (`docs/protocols`) za podešavanje i rad sa bazom.

## Pokretanje

Potrebni alati i njihova podešavanja (.NET 10 SDK, PostgreSQL 17 sa lozinkom `admin` za korisnika `postgres`, Node.js 22, Angular CLI) opisani su u protokolu [Podešavanje radne stanice](docs/protocols/podesavanje-radne-stanice.md).

> **Ako na računaru već imate bazu `explorer`** (npr. od timskog projekta), obrišite je u pgAdmin-u pre prvog pokretanja, zajedno sa testnim bazama čiji naziv počinje sa `explorer-test`. Ovaj projekat koristi baze istog naziva, ali sa drugačijim modelom, pa bi aplikacija na staroj bazi pala pri pokretanju ili ne bi upisala demo naloge.

Početni projekat ne klonirate, već ga sa njegovog repozitorijuma preuzimate kao .zip arhivu, raspakujete i postavljate u svoj privatni GitLab repozitorijum, u koji dodajete nalog `psw-ftn`. Koraci su u odeljku „Sopstveni repozitorijum“ protokola za podešavanje.

Pre prvog pokretanja, iz korena repozitorijuma:

```
dotnet tool restore
cd backend
dotnet build
cd ../frontend
npm install
```

> **Na Windows-u**, posle raspakivanja arhive `dotnet` može da prijavi da je neka datoteka blokirana („came from another computer and might be blocked“). Windows tako označava datoteke preuzete sa interneta. Iz korena projekta, u PowerShell-u, skinite tu oznaku sa svih datoteka, pa ponovite komandu koja nije uspela:
>
> ```
> Get-ChildItem -Recurse | Unblock-File
> ```

**Server**, iz direktorijuma `backend`:

```
dotnet run --project Host.Api
```

Pri prvom pokretanju aplikacija sama pravi bazu `explorer` i upisuje demo naloge. Na `http://localhost:5000/scalar` je pregled svih endpoint-a servera.

**Klijent**, u drugom terminalu dok server radi, iz direktorijuma `frontend`:

```
npm start
```

Aplikacija je na `http://localhost:4200`. Server i klijent moraju raditi istovremeno.

**Provera**, kada su obe aplikacije zaustavljene: `dotnet test` iz direktorijuma `backend`, odnosno `npm run lint` i `npm run build` iz direktorijuma `frontend`.

## Prijava

U razvojnom okruženju aplikacija upisuje demo naloge. Prijavljujete se e-adresom, a lozinka je za sve naloge `Password1!`.

| Uloga | E-adresa |
|---|---|
| vodič | `guide1@explorer.dev`, `guide2@explorer.dev` |
| turista | `tourist1@explorer.dev`, `tourist2@explorer.dev`, `tourist3@explorer.dev` |

Demo nalozi se upisuju samo kada baza nema nijednog korisnika. Ako prijava demo nalogom vraća grešku o pogrešnoj e-adresi ili lozinci, baza je ostala od ranije: zaustavite server, obrišite bazu `explorer` i ponovo ga pokrenite.

Nov nalog možete registrovati i kroz aplikaciju, ali on uvek dobija ulogu turiste. Lozinka mora imati bar šest znakova, veliko i malo slovo, cifru i specijalni znak.

## Gde radite

Sve funkcionalnosti iz specifikacije pravite u modulu `Exploration`, koji je prazan:

- server: `backend/Modules/Exploration/`
- klijent: `frontend/src/app/modules/exploration/`

Modul `Social` je primer i ne menjate ga. Ne menjate ni platformski deo projekta (`Host.Api`, `Host.Tests`, `Shared`, modul `Identity`, `.csproj` datoteke i podešavanja prevođenja). Ako vam se čini da je izmena tog dela neophodna, prvo se javite asistentu.

## Protokoli

| Protokol | Kada se primenjuje |
|---|---|
| [Podešavanje radne stanice](docs/protocols/podesavanje-radne-stanice.md) | Jednom, pre prvog rada na projektu. |
| [Migracije: lokalni rad](docs/protocols/migracije-lokalni-rad.md) | Svaki put kada menjate model podataka. |
| [Testna baza podataka](docs/protocols/testna-baza.md) | Kada integracioni testovi padaju iz razloga koji ne liči na vaš kod. |

## Veštine za rad sa agentom

Uz projekat dolaze veštine za agenta Claude Code (u `.claude/skills/`, a za Codex u `.agents/skills/`). Veštine u isto vreme razvijaju funkcionalnosti i vas podučavaju, gde je glavni cilj da razvijete svoje sposobnosti.

Jednu funkcionalnost vodite od zahteva do commit-a, ovim redom:

| Veština | Šta joj dajete | Šta je rezultat |
|---|---|---|
| `/psw-zahtevi` | Tekst zahteva | Po jednu specifikaciju za svaku funkcionalnost, sa namerom, ponašanjem i pravilima |
| `/psw-dizajn` | Jednu specifikaciju | Istu specifikaciju dopunjenu dizajnom: korisničko iskustvo, domenski model, tok poruka i dizajn testova |
| `/psw-implementacija` | Specifikaciju sa odobrenim dizajnom | Kod na novoj grani i lokalni commit |

Tokom implementacije agent piše kod u pozadini, a vi predviđate kako će taj kod izgledati i zatim predviđanje poredite sa napisanim kodom. Spajanje grane u `main` i slanje na repozitorijum radite sami.

Ostale veštine pozivate po potrebi:

| Veština | Kada je pozivate |
|---|---|
| `/psw-pitaj` | Kada vam deo koda nije jasan. Agent objašnjava šta kod radi i zašto je tako napisan, pojmovima iz baze znanja. |
| `/psw-ucenje` | Kada se spremate za odbranu projekta. Agent postavlja pitanja o kodu koji ste sami napisali. Uz poziv možete navesti deo gradiva koji želite posebno da obradite, na primer `/psw-ucenje klijent`. |

Objašnjenje agenta koje izlazi iz okvira baze znanja počinje oznakom `Van baze znanja:`.
