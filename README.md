# EatAndRate

**Szex!**


## Indítás Dockerrel

Szükséges: [Docker Desktop](https://www.docker.com/products/docker-desktop/)

```
git clone <https://github.com/kovacsdavid0/Beadando.git>
cd EatAndRate
docker compose up -d --build
```

Az API a `http://localhost:8080` címen érhető el.
OpenAPI leírás: `http://localhost:8080/openapi/v1.json`

Az adatbázis és a táblák automatikusan létrejönnek az első indításkor.

## Leállítás

```
docker compose down
```

Adatbázis törlésével együtt (tiszta újrakezdés):

```
docker compose down -v
```

## Fejlesztés

1. Csak az adatbázis indítása: `docker compose up -d db`
2. Az API indítása Visual Studióból (F5)

Az adatbázis a gépről a `localhost:3307` címen érhető el
(user: `eatandrate`, jelszó: `eatandrate`).