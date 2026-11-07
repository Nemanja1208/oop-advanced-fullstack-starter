# Mätning: håller vi kraven?

Uppmätt 2026-11-07 på en utvecklardator (ThinkPad, Windows 11, .NET 10, Release-bygge, SQLite).
Siffrorna gäller den här datorn. Det viktiga är att du mäter på samma sätt varje gång, så att du ser om det
blir bättre eller sämre.

## 1. Sortering (BenchmarkDotNet)
Krav: sortera 1 000 uppgifter på under 1 ms.

```bash
cd backend
dotnet run -c Release --project Starter.Benchmarks -- --filter "*" --job short
```

| Antal | Medel | Minne per anrop |
|---|---|---|
| 100 | 5,7 µs | 5 kB |
| 1 000 | 82 µs | 39 kB |
| 10 000 | 1 862 µs | 382 kB |

✅ 1 000 uppgifter: 0,08 ms, alltså ungefär tolv gånger snabbare än kravet.
Observera: tio gånger fler uppgifter ger drygt tjugo gånger längre tid. Sortering växer snabbare än linjärt.

## 2. Svarstid under last (autocannon)
Krav: `GET /api/todos` p99 under 200 ms med 1 000 uppgifter och 20 samtidiga användare, 0 % fel.

```bash
# Terminal 1: starta API:t i Release mot en tom databas
cd backend
dotnet run -c Release --project Starter.Api

# Terminal 2: fyll på 1 000 uppgifter och kör lasttestet i 10 sekunder
node perf/seed.mjs 1000
npx autocannon -c 20 -d 10 http://localhost:5069/api/todos
```

| Mått | Värde |
|---|---|
| p50 | 6 ms |
| p97,5 | 20 ms |
| p99 | 27 ms |
| Max | 65 ms |
| Anrop per sekund | ca 2 500 |
| Fel | 0 |

✅ p99 27 ms mot kravet 200 ms.
⚠ Ett fynd: varje svar är ungefär 170 kB, eftersom hela listan skickas varje gång. Med 100 000 uppgifter
håller det inte. Det är ett bra skäl att införa sidindelning (paginering) innan någon klagar.

## 3. Resultat: blir kunden hjälpt?
Krav: andelen uppgifter klara i tid ska gå från ca 60 % till 85 % på fyra veckor.

```bash
curl http://localhost:5069/api/todos/stats
# {"completedOnTime":0,"completedLate":0,"overdueNow":0,"onTimeRate":null}
```

`onTimeRate` är `null` tills något med ett datum har blivit klart. "Ingen data än" och "0 %" är olika svar,
och det ska man inte blanda ihop när man rapporterar till en kund.
Det här talet kan inga tester bevisa. Det får vi veta först när Sara har använt appen i några veckor.
Därför mäter vi det i produktion, och följer upp med henne.
