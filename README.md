# EatAndRate

Éttermek és étteremláncok értékelésére szolgáló alkalmazás. A felhasználók regisztrálnak,
majd az éttermeket három szempont szerint (étel, kiszolgálás, hangulat) 1–5 pontra értékelhetik,
szöveges megjegyzéssel.

## Felépítés

| Projekt | Leírás |
|---|---|
| `API` | ASP.NET Core Web API (.NET 10), Entity Framework Core + MySQL, JWT-alapú hitelesítés, BCrypt jelszó-hash |
| `Frontend` | WPF asztali kliens (.NET 10, MVVM), világos és sötét témával |
| `EatAndRate.Shared` | Az API és a kliens közös DTO-i, validációs attribútumokkal |

Az adatbázis (MySQL 8.4) és az API Dockerben fut, a WPF kliens a gépen.

## Funkciók (kliens)

- **Regisztráció / bejelentkezés** – felhasználónévvel vagy e-mail címmel lehet belépni
- **Áttekintés (Dashboard)** – heti értékelésszám, kedvenc étteremlánc, értékelt láncok száma,
  a közösség legutóbbi értékelései (kattintásra részletek ablak), a legjobbra értékelt lánc és annak top 3 étterme
- **Éttermek** – étteremláncok listája logóval, a láncon belüli éttermek, értékelés írása
- **Értékeléseim** – saját értékelések listája rendezési lehetőségekkel (dátum, étterem, lánc, pontszám),
  részletek, módosítás és törlés
- **Beállítások** – váltás világos és sötét téma között

## Indítás Dockerrel

Szükséges: [Docker Desktop](https://www.docker.com/products/docker-desktop/)

```
git clone https://github.com/kovacsdavid0/Beadando.git
cd Beadando/EatAndRate
docker compose up -d --build
```

Az API a `http://localhost:8080` címen érhető el.
OpenAPI leírás: `http://localhost:8080/openapi/v1.json`

Az adatbázis és a táblák automatikusan létrejönnek az első indításkor (migráció + kezdeti adatok).

> **Fontos:** ha az `API` vagy az `EatAndRate.Shared` kódja megváltozik, az API image-et újra kell
> építeni (`docker compose up -d --build`), különben a konténer a régi kódot futtatja.

Ezután a kliens a Visual Studióból indítható (`Frontend` projekt, F5).
A kliens alapértelmezetten a `http://localhost:8080` címen keresi az API-t
(`Frontend/Services/ApiClient.cs`, `BaseUrl`).

### Teszt fiókok

Az első indításkor létrejövő felhasználók:

| Felhasználónév | Jelszó |
|---|---|
| `alice` | `Password123!` |
| `bob` | `Password123!` |

## Leállítás

```
docker compose down
```

Adatbázis törlésével együtt (tiszta újrakezdés):

```
docker compose down -v
```

## Adatbázis

### Létrehozási paraméterek

Az adatbázist a `docker-compose.yml` `db` szolgáltatása hozza létre az alábbi paraméterekkel:

| Paraméter | Érték |
|---|---|
| Szerver | MySQL 8.4 (`mysql:8.4` image) |
| Konténer neve | `eatandrate-db` |
| Adatbázis neve | `eatandrate` |
| Felhasználó / jelszó | `eatandrate` / `eatandrate` |
| Root jelszó | `rootpass` |
| Port (gépről) | `localhost:3307` (a konténeren belül `3306`) |
| Karakterkódolás | `utf8mb4` |
| Adattárolás | `db-data` nevesített Docker volume |

Kapcsolati sztring:

- gépről (Visual Studióból futó API): `Server=localhost;Port=3307;Database=eatandrate;User=eatandrate;Password=eatandrate;`
- Dockerben (API konténer): `Server=db;Port=3306;Database=eatandrate;User=eatandrate;Password=eatandrate;`

Docker nélkül, saját MySQL szerveren ugyanez kézzel így hozható létre:

```sql
CREATE DATABASE eatandrate CHARACTER SET utf8mb4;
CREATE USER 'eatandrate'@'%' IDENTIFIED BY 'eatandrate';
GRANT ALL PRIVILEGES ON eatandrate.* TO 'eatandrate'@'%';
FLUSH PRIVILEGES;
```

A táblákat nem kell kézzel létrehozni: az API induláskor lefuttatja az EF Core migrációt
(`API/Migrations/20261029132020_InitialCreate.cs`), majd feltölti a kezdeti adatokat (`API/Data/DbSeeder.cs`).

### Táblák

**Users**

| Oszlop | Típus | Megkötés |
|---|---|---|
| `Id` | int | PK, auto increment |
| `Username` | varchar(50) | kötelező, egyedi |
| `Email` | varchar(100) | kötelező, egyedi |
| `PasswordHash` | varchar(255) | kötelező (BCrypt) |
| `CreatedAt` | datetime(6) | kötelező (UTC) |

**Franchises** (étteremláncok)

| Oszlop | Típus | Megkötés |
|---|---|---|
| `Id` | int | PK, auto increment |
| `Name` | varchar(100) | kötelező, egyedi |

**Restaurants**

| Oszlop | Típus | Megkötés |
|---|---|---|
| `Id` | int | PK, auto increment |
| `Name` | varchar(100) | kötelező |
| `Address` | varchar(200) | opcionális |
| `City` | varchar(100) | opcionális |
| `FranchiseId` | int | FK → `Franchises.Id`, opcionális, lánc törlésekor `NULL` lesz |

**Reviews**

| Oszlop | Típus | Megkötés |
|---|---|---|
| `Id` | int | PK, auto increment |
| `FoodRating` | int | kötelező, 1–5 (CHECK) |
| `ServiceRating` | int | kötelező, 1–5 (CHECK) |
| `AtmosphereRating` | int | kötelező, 1–5 (CHECK) |
| `Comment` | varchar(1000) | opcionális |
| `CreatedAt` | datetime(6) | kötelező (UTC) |
| `UserId` | int | FK → `Users.Id`, felhasználó törlésekor kaszkád törlés |
| `RestaurantId` | int | FK → `Restaurants.Id`, étterem törlésekor kaszkád törlés |

A (`UserId`, `RestaurantId`) páros egyedi: egy felhasználó egy éttermet csak egyszer értékelhet.

## Fejlesztés (API Visual Studióból)

1. Csak az adatbázis indítása: `docker compose up -d db`
2. Az API indítása Visual Studióból (F5) – a `http` profil a `http://localhost:5029` címen fut
3. A kliensben a `Frontend/Services/ApiClient.cs` fájlban a `BaseUrl`-t át kell állítani
   `http://localhost:5029/`-ra, különben a kliens továbbra is a Dockeres API-hoz (8080) kapcsolódik

Az adatbázis a gépről a `localhost:3307` címen érhető el
(user: `eatandrate`, jelszó: `eatandrate`).

## API végpontok

Az `[Auth]` jelölésű végpontokhoz `Authorization: Bearer <token>` fejléc kell
(a token a regisztráció / bejelentkezés válaszában érkezik).

**Hitelesítés**

| Metódus | Útvonal | |
|---|---|---|
| POST | `/api/auth/register` | Regisztráció |
| POST | `/api/auth/login` | Bejelentkezés (felhasználónév vagy e-mail) |

**Étteremláncok**

| Metódus | Útvonal | |
|---|---|---|
| GET | `/api/franchises` | Összes lánc (étterem- és értékelésszám, átlag) |
| GET | `/api/franchises/{id}` | Egy lánc |
| POST / PUT / DELETE | `/api/franchises[/{id}]` | Létrehozás / módosítás / törlés `[Auth]` |

**Éttermek**

| Metódus | Útvonal | |
|---|---|---|
| GET | `/api/restaurants?city=&franchiseId=` | Éttermek, szűrhetően |
| GET | `/api/restaurants/{id}` | Étterem részletei |
| GET | `/api/restaurants/search/name?keyword=` | Keresés név szerint |
| GET | `/api/restaurants/search/franchise?keyword=` | Keresés lánc szerint |
| GET | `/api/restaurants/search/city?keyword=` | Keresés város szerint |
| POST / PUT / DELETE | `/api/restaurants[/{id}]` | Létrehozás / módosítás / törlés `[Auth]` |

**Értékelések**

| Metódus | Útvonal | |
|---|---|---|
| GET | `/api/restaurants/{restaurantId}/reviews` | Egy étterem értékelései |
| POST | `/api/restaurants/{restaurantId}/reviews` | Új értékelés (éttermenként egy) `[Auth]` |
| GET | `/api/reviews/mine` | Saját értékelések `[Auth]` |
| GET | `/api/reviews/recent?count=` | Legutóbbi értékelések (1–50, alapból 10) |
| GET | `/api/reviews/{id}` | Egy értékelés |
| PUT / DELETE | `/api/reviews/{id}` | Saját értékelés módosítása / törlése `[Auth]` |
