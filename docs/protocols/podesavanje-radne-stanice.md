# Podešavanje radne stanice

## Kada se primenjuje

Jednom, pre prvog rada na projektu. Protokol se završava proverom koja potvrđuje da je radna stanica ispravno podešena — ne prijavljujte da ste spremni dok poslednji korak ne prođe.

## 1. Alati

Instalirajte sledeće, redom kojim su navedeni:

1. **Git** — [git-scm.com](https://git-scm.com/). Podesite ime i adresu koje će stajati uz vaše commit-ove:
   ```
   git config --global user.name "Ime Prezime"
   git config --global user.email "adresa@uns.ac.rs"
   ```
2. **.NET 10 SDK** — [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0). Instalirajte SDK, ne samo runtime. Provera: `dotnet --version` ispisuje verziju koja počinje sa `10.`.
3. **Razvojno okruženje** — Visual Studio 2026 (Community je dovoljan) sa paketom „ASP.NET and web development", ili Rider. Za rad na klijentskoj aplikaciji dovoljan je VS Code.
4. **PostgreSQL 17** — [postgresql.org](https://www.postgresql.org/download/). Tokom instalacije:
   - lozinka korisnika `postgres` mora biti `admin`,
   - port ostaje podrazumevani `5432`.

   Ove vrednosti očekuje `appsettings.Development.json` i ne menjaju se lokalno. Provera: pgAdmin (instaliran uz PostgreSQL) uspešno se povezuje na lokalni server.
5. **Node.js 22 LTS** — [nodejs.org](https://nodejs.org/). Provera: `node --version` ispisuje verziju koja počinje sa `v22.`.
6. **Angular CLI** — `npm install -g @angular/cli`. Provera: `ng --version`.

## 2. Projekat

### 2.1. Sopstveni repozitorijum

1. Otvorite repozitorijum početnog projekta (link je u obaveštenju na Canvasu) i preuzmite ga kao .zip arhivu: na GitHub-u dugme **Code → Download ZIP**, a na GitLab-u **Code → Download source code → zip**. Ne klonirajte ga, jer razvijate u svom repozitorijumu, a ne u repozitorijumu početnog projekta.
2. Raspakujte arhivu u direktorijum u kom držite projekte (na Windows-u: desni klik na arhivu → **Extract All**). Iz same arhive se ne može raditi: git, `dotnet` i `npm` rade samo nad raspakovanim datotekama. Posle raspakivanja arhiva vam više nije potrebna.
3. Pronađite **koren projekta**: to je folder u kom se nalaze `README.md`, `backend` i `frontend`. Windows pri raspakivanju često napravi još jedan folder istog naziva oko njega, pa je koren projekta tada unutrašnji folder (npr. `...\ftn-psw-start-individual-main\ftn-psw-start-individual-main`). Sve komande u ovom protokolu pokrećete iz korena projekta ili iz direktorijuma koje navode.
4. Na Windows-u otvorite PowerShell, pređite u koren projekta (`cd <putanja do korena projekta>`) i skinite oznaku „preuzeto sa interneta“ sa svih datoteka. Bez ovog koraka `dotnet` odbija neke datoteke kao blokirane.
   ```
   Get-ChildItem -Recurse | Unblock-File
   ```
5. Na GitLab-u napravite nov, prazan projekat sa vidljivošću **Private** (bez README datoteke, da bi bio zaista prazan). U podešavanjima projekta (Manage → Members) dodajte nalog `psw-ftn`.
6. Iz korena projekta postavite ga u svoj repozitorijum:
   ```
   git init -b main
   git add .
   git commit -m "Početni projekat"
   git remote add origin <adresa vašeg GitLab projekta>
   git push -u origin main
   ```
   Grana mora da se zove `main`, jer je veštine za agenta očekuju pod tim imenom. Proverite na GitLab-u da su sve datoteke stigle, uključujući `.gitignore` u korenu: bez nje bi se u repozitorijum upisivali i direktorijumi `node_modules`, `bin` i `obj`.

### 2.2. Priprema za prevođenje

Iz korena projekta:

```
dotnet tool restore
cd backend
dotnet build
cd ../frontend
npm install
```

`dotnet tool restore` instalira lokalne alate projekta (trenutno `dotnet-ef`, potreban za migracije) prema spisku u `.config/`. Verzija alata je zadata u projektu.

`dotnet build` mora proći bez grešaka. Upozorenja se u ovom projektu tretiraju kao greške.

`npm install` preuzima biblioteke klijentske aplikacije u direktorijum `frontend/node_modules/`, koji se ne commit-uje. Komandu ponavljate i kada se u projekat doda nova biblioteka.

## 3. Prvo pokretanje

Ako na računaru već imate bazu `explorer` (npr. od timskog projekta), obrišite je u pgAdmin-u, zajedno sa testnim bazama čiji naziv počinje sa `explorer-test`. Ovaj projekat koristi baze istog naziva, ali sa drugačijim modelom.

Iz direktorijuma `backend`:

```
dotnet run --project Host.Api
```

Pri prvom pokretanju aplikacija sama kreira potrebne strukture u bazi `explorer` (o tome detaljnije u protokolu [Migracije: lokalni rad](migracije-lokalni-rad.md)). Zatim u pregledaču otvorite `http://localhost:5000/scalar`: prikazuje se Scalar, interaktivni pregled svih endpoint-a. Kroz njega pozovite `POST /api/identity/register` sa proizvoljnom e-adresom i lozinkom koja ima bar šest znakova, veliko i malo slovo, cifru i specijalni znak (npr. `Lozinka1!`). Primer koji Scalar sam popuni (`"string"`) ne zadovoljava ta pravila i vraća odgovor 400. Odgovor sa JWT tokenom potvrđuje da aplikacija i baza rade zajedno.

Ne zaustavljajući serversku aplikaciju, u drugom terminalu iz direktorijuma `frontend` pokrenite:

```
npm start
```

U pregledaču otvorite `http://localhost:4200` i prijavite se nalogom koji ste upravo registrovali, ili demo nalogom vodiča `guide1@explorer.dev` sa lozinkom `Password1!` (spisak svih demo naloga je u README.md). Uspešna prijava potvrđuje da klijentska aplikacija stiže do servera — razvojni server zahteve ka `/api` prosleđuje na `localhost:5000` prema datoteci `proxy.conf.json`.

## 4. Završna provera

Zaustavite obe aplikacije, pa iz direktorijuma `backend` pokrenite:

```
dotnet test
```

Testovi kreiraju sopstvene testne baze i ne diraju bazu `explorer`. Zatim iz direktorijuma `frontend` pokrenite provere klijenta:

```
npm run lint
npm run build
```

Kada svi testovi i obe provere klijenta prođu, radna stanica je spremna.

## Česti problemi

- **`dotnet` prijavljuje da je datoteka blokirana („came from another computer and might be blocked“)** — preskočen je korak 4 iz odeljka 2.1. Iz korena projekta, u PowerShell-u, pokrenite `Get-ChildItem -Recurse | Unblock-File` i ponovite komandu.
- **`dotnet ef` ne postoji kao komanda** — niste pokrenuli `dotnet tool restore`, ili komandu pokrećete izvan repozitorijuma (alat je lokalan za projekat).
- **Aplikacija pada pri pokretanju uz grešku o konekciji** — PostgreSQL servis nije pokrenut, ili lozinka korisnika `postgres` nije `admin`. Lozinku možete promeniti kroz pgAdmin; ne menjajte `appsettings.Development.json`.
- **Prijava demo nalogom javlja pogrešnu e-adresu ili lozinku** — demo nalozi se upisuju samo u praznu bazu, a vaša baza `explorer` je ostala od ranije. Zaustavite server, obrišite bazu i ponovo ga pokrenite.
- **Testovi padaju uz grešku o konekciji, a aplikacija radi** — vidite protokol [Testna baza podataka](testna-baza.md).
- **Klijentska aplikacija se učitava, ali prijava i registracija ne rade** — serverska aplikacija nije pokrenuta. Klijent zahteve šalje serveru na `localhost:5000`, pa oba moraju raditi istovremeno.
- **`ng` ili `npm` ne postoji kao komanda** — Node.js nije instaliran ili terminal je otvoren pre instalacije; otvorite nov terminal.
