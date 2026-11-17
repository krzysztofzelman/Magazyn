# Magazyn

Projekt nauki C#: API + strona Blazor na **wspólnej bazie** `data/magazyn.db`.
Konsola jest teraz **osobnym projektem** — `D:\Dane\Projekty\Nauka C#\MagazynConsole\` — i ma własną bazę.

## Układ folderów

```
Magazyn\
├─ Magazyn.slnx          rozwiązanie z trzema projektami (start w Visual Studio)
├─ data\magazyn.db       baza SQLite — wspólna dla API i strony
├─ docs\                 notatki z nauki (dni 3–6, mapa projektu, drzewo plików)
└─ src\
   ├─ MagazynApi\        API, port 5109: api/Batch, api/przedmioty
   ├─ MagazynWeb\        strona Blazor, port 5008: / (artykuły), /partie
   └─ MagazynShared\     wspólne modele: Batch, Item, DispatchRequest
```

Zależności: `MagazynApi` i `MagazynWeb` korzystają z `MagazynShared`.

## Jak uruchomić

W Visual Studio: otwórz `Magazyn.slnx` i wciśnij **Ctrl+F5** (startuje API + stronę).
Strona: `http://localhost:5008`.

Z terminala (dwa okna):

```
cd src\MagazynApi
dotnet run
```

```
cd src\MagazynWeb
dotnet run
```

## Konsola

Osobny projekt: `D:\Dane\Projekty\Nauka C#\MagazynConsole\` (rozwiązanie `MagazynConsole.slnx`).

```
cd ..\MagazynConsole
dotnet run
```

Konsola czyta **własną** bazę `MagazynConsole\data\magazyn.db` — dane z konsoli nie pojawiają się na stronie `/partie` i odwrotnie.

## Notatki z nauki

Pliki `nauka-dzien-*.md`, `mapa-projektu.md`, `drzewo-plikow.md` leżą w `docs\`.

**`docs\wdrozenie-vps.md`** — mapa wdrożenia na serwer (stan 2026-11-17): cztery rzeczy,
które blokują kontenery, co już czeka na VPS (domena, cert, porty), kolejność dwóch sesji
i pułapki (WebSockety Blazora, wolumen na bazę, `*.db` w `.gitignore`).
