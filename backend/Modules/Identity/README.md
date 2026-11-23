# Modul Identity

Modul Identity je platformski modul zadužen za registraciju korisnika, prijavu i izdavanje JWT-a (JSON Web Token). Sastoji se od dva projekta. Projekat `Identity` sadrži sam modul, a projekat `Identity.Tests` njegove integracione testove. Studenti ovaj modul koriste kroz njegove krajnje tačke, ali ga ne menjaju i ne referenciraju iz svojih modula.

## Problem koji rešava

Skoro svaka krajnja tačka sistema mora da zna ko je prijavljeni korisnik. Podatak o korisniku je pri tome osetljiv, jer uz njega idu lozinka i uloge. Kada bi svaki modul sam vodio evidenciju korisnika, isti osetljivi podaci bi se ponavljali na više mesta i isti bezbednosni problemi bi se rešavali više puta.

Zato o korisničkim nalozima brine jedan centralni modul. On proverava identitet korisnika pri prijavi i izdaje potpisan token u kome se nalaze identifikator korisnika, adresa elektronske pošte i uloge. Ostali moduli nikada ne pristupaju korisničkim nalozima. Prijavljenog korisnika poznaju samo preko vrednosti `UserId` koju kontroler čita iz tokena i prosleđuje aplikacionom sloju kao običan parametar.

Posao oko tokena podeljen je na dva mesta. Modul Identity izdaje token, a projekat `Host.Api` konfiguriše proveru tokena za sve pristigle zahteve. Zahvaljujući toj podeli, funkcionalni moduli o tokenima ne znaju ništa. Njima je dovoljan atribut `[Authorize]` na kontroleru i vrednost `UserId` iz zahteva.

## Odstupanje od strukture modula

Funkcionalni moduli imaju pet projekata, po jedan za svaki sloj. Modul Identity ima jedan projekat.

Podela na pet projekata postoji da bi štitila domenska pravila i granice između modula. Modul Identity nema domenska pravila koja bi štitio, jer čitavu logiku naloga, lozinki i uloga preuzima gotova biblioteka ASP.NET Core Identity. Nema ni potrebe za granicom prema ostatku sistema, jer ga nijedan drugi modul ne referencira. Pet projekata bi u ovoj situaciji bila nepotrebna apstrakcija. Ovaj modul ujedno prikazuje snagu modular monolit arhitekture, koja dozvoljava da pratimo različite arhitekture na nivou pojedinačnih modula.

## Elementi projekta Identity

Projekat je podeljen na tri direktorijuma koji odgovaraju slojevima funkcionalnog modula.

### Api

Sadrži klasu `IdentityController` sa dve krajnje tačke. Krajnja tačka `POST /api/identity/register` pravi novi nalog, dodeljuje mu ulogu `tourist` i vraća token. Kroz aplikaciju se registruju samo turisti; vodiči se unose u bazu. Krajnja tačka `POST /api/identity/login` proverava adresu elektronske pošte i lozinku i vraća token. Obe krajnje tačke su javne, jer im pristupaju korisnici koji još nemaju token. Kontroler direktno koristi klasu `UserManager` iz biblioteke ASP.NET Core Identity, koja vrši ulogu aplikacionog servisa.

Sadrži i klasu koja definiše metodu `AddIdentityControllers` kojom `Host.Api` uključuje kontrolere ovog projekta, po istom obrascu kao kod funkcionalnih modula.

### Core

Sadrži klase koje opisuju korisnika i token. Klasa `ApplicationUser` predstavlja korisnika i nasleđuje gotovu klasu `IdentityUser`, bez dodatnih polja. Klasa `JwtSettings` sadrži podešavanja tokena koja se čitaju iz konfiguracije, poput ključa za potpisivanje i roka važenja. Klasa `JwtTokenFactory` od korisnika i njegovih uloga pravi potpisan token. Token nosi identifikator korisnika u polju `sub`, adresu elektronske pošte u polju `email` i po jedno polje za svaku ulogu.

Ovde su i DTO klase koje krajnje tačke primaju i vraćaju. Klase `RegisterDto` i `LoginDto` opisuju zahteve, a klasa `AccessTokenDto` odgovor sa tokenom.

### Infrastructure

Sadrži pristup bazi i uključivanje modula u aplikaciju. Klasa `IdentityModuleDbContext` nasleđuje gotov kontekst biblioteke ASP.NET Core Identity i smešta njegove tabele u šemu `identity`, po pravilu da svaki modul ima svoju šemu. Direktorijum `Migrations` sadrži migracije te šeme.

Metoda `AddIdentityModule` registruje sve delove modula i jedino je mesto koje `Host.Api` poziva pored metode `AddIdentityControllers`. Klasa `IdentityModuleInitializer` se izvršava pri pokretanju aplikacije. Ona primenjuje migracije i upisuje uloge `guide` (vodič) i `tourist` (turista) ako već ne postoje, pa aplikacija posle pokretanja uvek ima spremnu šemu i uloge. U razvojnom okruženju, ako baza nema nijednog korisnika, upisuje i demo naloge sa lozinkom `Password1!`: `guide1@explorer.dev` i `guide2@explorer.dev` (vodiči), kao i `tourist1@explorer.dev` do `tourist3@explorer.dev` (turisti). Ovako se u bazu unose vodiči, koji se ne registruju kroz aplikaciju.

## Projekat Identity.Tests

Projekat sadrži integracione testove koji šalju prave HTTP zahteve krajnjim tačkama registracije i prijave i proveravaju odgovore, uključujući sadržaj vraćenog tokena. Ovo je jedini projekat koji te dve krajnje tačke testira, jer su jedino ovde one predmet testa. Testovi ostalih modula prijavljenog korisnika dobijaju direktno od test infrastrukture, bez prolaska kroz modul Identity.

Početni podaci ovog projekta imaju jednu posebnost. U funkcionalnim modulima početni podaci nastaju pozivom domenskih konstruktora. Korisnički nalog se u pravom radu pravi kroz klasu `UserManager`, koja usput računa heš lozinke i normalizovane kolone. Mehanizam za vraćanje baze na početno stanje upisuje redove direktno u bazu i taj korak zaobilazi, pa klasa `UserSeed` te vrednosti popunjava sama. Identifikator korisnika dolazi iz klase `WellKnownUsers`, tako da testovi svih modula govore o istim korisnicima.
