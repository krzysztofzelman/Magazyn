# Magazyn

Projekt nauki C#: magazyn towaru (artykuły, partie, PZ/WZ) w trzech warstwach na **jednym pliku bazy** `data/magazyn.db`.

## Układ folderów

```
Magazyn\
├─ Magazyn.slnx          rozwiązanie z czterema projektami (start w Visual Studio)
├─ data\magazyn.db       baza SQLite — wspólna dla wszystkich warstw
└─ src\
   ├─ Magazyn\           konsola: opcje 1–7 (CRUD artykułów/partii, PZ, WZ FIFO)
   ├─ MagazynApi\        API, port 5109: api/Batch, api/przedmioty
   ├─ MagazynWeb\        strona Blazor, port 5008: / (artykuły), /partie
   └─ MagazynShared\     wspólne modele: Batch, Item, DispatchRequest
```

Zależności: `MagazynApi` i `MagazynWeb` korzystają z `MagazynShared`. Konsola jest samodzielna (ma własny `Item.cs`).

## Jak uruchomić

W Visual Studio: otwórz `Magazyn.slnx` i wciśnij **Ctrl+F5** (startuje API + stronę).

Z terminala (dwa okna):

```
cd src\MagazynApi
dotnet run
```

```
cd src\MagazynWeb
dotnet run
```

Konsola:

```
cd src\Magazyn
dotnet run
```

## Notatki z nauki

Pliki `nauka-dzien-*.md`, `mapa-projektu.md`, `drzewo-plikow.md` leżą w folderach projektów, których dotyczą (`src\Magazyn\`, `src\MagazynApi\`).
