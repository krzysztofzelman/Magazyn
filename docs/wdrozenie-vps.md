# Magazyn — wdrożenie na VPS (mapa)

Stan sprawdzony **2026-11-17**: repo `Magazyn`, commit `b7ae6b0`, drzewo czyste.
Cel: `https://magazyn.kzelman.pl` — strona `MagazynWeb` + API `MagazynApi` w dwóch kontenerach, baza `magazyn.db` w wolumenie.

---

## 1. Co blokuje wdrożenie (4 rzeczy)

### 1.1 Adresy API wpisane na sztywno — 12 linii

`http://localhost:5109/...` stoi w kodzie stron:

| plik | linie |
|---|---|
| `src\MagazynWeb\Components\Pages\Home.razor` | 104, 152, 159, 165, 168, 175, 176 |
| `src\MagazynWeb\Components\Pages\Batches.razor` | 133, 146, 154, 186, 201 |

`src\MagazynWeb\Program.cs` ma `builder.Services.AddHttpClient();` — bez `BaseAddress`.

**Dlaczego to blokuje:** w kontenerze `localhost` znaczy „ten sam kontener". Strona
w kontenerze `magazyn-web` pod `localhost:5109` nie znajdzie API z kontenera `magazyn-api`.

**Naprawa — ten sam wzorzec co w Shopie, Dzień 20:**

1. `src\MagazynWeb\appsettings.json` → `"ApiUrl": "http://localhost:5109"`
2. `Program.cs` →
   ```csharp
   var apiUrl = builder.Configuration["ApiUrl"] ?? "http://localhost:5109";
   builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
   ```
3. w `.razor` zdjąć prefiks z 12 linii, zostaje sam koniec adresu:
   `"api/przedmioty"`, `$"api/przedmioty/{item.Id}"`, `"api/Batch"`, `$"api/Batch/{editedId}"`, `"api/Batch/wydanie"`.
4. w `docker-compose.yml` dla usługi strony: `ApiUrl: http://magazyn-api:8080`
   (nazwa usługi z compose = nazwa hosta w sieci Dockera).

To jest **jednocześnie** punkt „DI HttpClient" z kolejki Magazynu — jedna robota, dwa cele.

### 1.2 Connection string wskazuje ścieżkę Windows

`src\MagazynApi\appsettings.json`:

```json
"MagazynDb": "Data Source=D:\\Dane\\Projekty\\Nauka C#\\Magazyn\\data\\magazyn.db"
```

Na Linuksie nie ma `D:\`. **Naprawa:** `Data Source=/app/data/magazyn.db` + wolumen w compose.

### 1.3 Brak Dockera — 0 plików

Sprawdzone globem w całym `Magazyn\`: **nie ma** `Dockerfile`, `docker-compose.yml`, `.dockerignore`.

Potrzebne: 2 × `Dockerfile` (`MagazynApi`, `MagazynWeb`), 1 × `docker-compose.yml`, `.dockerignore`
i nazwany wolumen na bazę. To boilerplate — wzór leży w `Shop\`. Nie ma czego uczyć się na pamięć.

### 1.4 Baza nie pojedzie z gitem

`.gitignore` ma `*.db`, `*.db-shm`, `*.db-wal` ⇒ `git clone` na serwerze **nie przyniesie**
`data\magazyn.db` (16 384 B). Trzeba wgrać plik albo założyć pustą bazę:

```
scp -i "%USERPROFILE%\.ssh\vps_magazyn" -P 2022 "D:\Dane\Projekty\Nauka C#\Magazyn\data\magazyn.db" root@31.3.218.196:/root/magazyn/data/
```

(`scp` bierze `-P`, `ssh` bierze `-p`.)

---

## 2. Co już czeka na serwerze (sprawdzone przez SSH, 2026-11-17)

- **DNS:** `magazyn.kzelman.pl` → A `31.3.218.196` ✅
- **nginx:** `/etc/nginx/sites-available/magazyn.kzelman.pl` — blok 443 z `return 404`
  + blok 80 → 301 na https (w tym samym pliku `api.kzelman.pl` → `localhost:3001`)
- **cert:** `/etc/letsencrypt/live/kzelman.pl/fullchain.pem`, SAN: `api.kzelman.pl`,
  `kzelman.pl`, **`magazyn.kzelman.pl`**, `www.kzelman.pl`
  ⇒ cert już pokrywa domenę, **żadnego nowego certbota nie trzeba**
- **porty:** 5008 i 5109 wolne; na serwerze stoją tylko `shop-shop-web-1` (127.0.0.1:5107)
  i `shop-shop-api-1` (127.0.0.1:5143)

### Wpis nginx — wzór z `sites-available/shop.kzelman.pl`

Zamiast `return 404` w bloku 443:

```nginx
location / {
    proxy_pass http://127.0.0.1:5108;
    proxy_http_version 1.1;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
}
```

Ostatnie dwie linie są **obowiązkowe** dla Blazor Server — circuit leci po WebSockecie.
Potem: `nginx -t && systemctl reload nginx`.

API **nie musi wychodzić na zewnątrz** — strona gada z nim po nazwie usługi w sieci Dockera,
więc wystawiamy tylko port strony.

---

## 3. Czego NIE trzeba robić przed wdrożeniem (szlif, nie warunek)

- `min="1"` na polu ilości — `Batches.razor`, linia 83 (atrybutu brak)
- blokada podwójnego kliknięcia w `Dispatch` — flagi `isDispatching` w kodzie **nie ma**,
  trzeba napisać od zera (flaga + `return` na początku + `disabled` na przycisku)
- `??` na `List<Batch>?`
- „Przedmiot" → „Artykuł" na Home

---

## 4. Kolejność — dwie sesje

**Sesja A — kod, potem Docker:**

1. `ApiUrl` w `appsettings.json` + `BaseAddress` w `Program.cs`
2. 12 linii w `.razor` bez prefiksu → test lokalnie `Ctrl+F5`, `http://localhost:5008`
3. connection string → `/app/data/magazyn.db`
4. 2 × `Dockerfile` + `docker-compose.yml` + `.dockerignore` → lokalnie
   `docker compose up -d --build` → test na hoście
5. commit + push → na serwerze `git clone` do `/root/magazyn`, `scp` bazy,
   `docker compose up -d --build`

**Sesja B — nginx:**

6. `proxy_pass` zamiast `return 404` + WebSockety
7. `nginx -t` → `systemctl reload nginx`
8. test: `https://magazyn.kzelman.pl` → tabela artykułów, `/partie` → partie

**Kolejność ma znaczenie:** najpierw kod, potem kontenery. Odwrotnie debugujesz dwie rzeczy
naraz — czy kontener wstał, i czy strona znajduje API.

---

## 5. Pułapki

- **Blazor Server bez WebSocketów** — strona wstanie, HTML się wyświetli, ale przyciski będą
  „martwe" (circuit nie połączy się). Objaw: klikasz i nic.
- **Brak wolumenu na bazę** — dane znikają przy każdym `docker compose up --build`.
- **`data\` po `git clone`** — będzie tam tylko `.gitkeep`, bazy nie ma (patrz 1.4).
- **Porty na hoście** wiązać do `127.0.0.1`, nie `0.0.0.0` (tak jak Shop) — inaczej port
  omija `ufw`.
- **Dwa logowania** — SSH na serwer (klucz `vps_magazyn`, port 2022) to co innego niż
  wejście do aplikacji; Magazyn nie ma logowania, więc nic tu nie trzeba pamiętać.

---

## 6. Kiedy wracamy

Po zamknięciu `foreach` vs `while` w `ShoppingList` (jego własny kod, nie Magazyn).
Pierwsze 5 minut sesji A: `Ctrl+F5` na `Magazyn.slnx` i sprawdzenie, czy `http://localhost:5008`
pokazuje artykuły, a `/partie` — partie z `data\magazyn.db`.
