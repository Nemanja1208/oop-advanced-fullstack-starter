# Kravspecifikation: Deadlines & prioritet

**Kund:** Sara, teamledare (8 personer) · **Version:** 1.0 · **Status:** godkänd av Sara

## 1. Problemet, med kundens egna ord
> "Vi missar deadlines hela tiden, och ingen märker det förrän kunden ringer och är arg."

Efter intervjun förstår vi problemet så här: uppgifter har inget datum i appen, så ingen ser vad som
håller på att bli sent. Den snabbfix som finns (Kims `/overdue`) visar fel, så teamet har slutat lita på den.

## 2. Mål, och hur vi vet att vi har nått det
| Mål | Mätvärde | Idag | Mål om 4 veckor | Hur vi mäter |
|---|---|---|---|---|
| Färre missade deadlines | Andel uppgifter klara i tid | ca 60 % (Saras uppskattning) | 85 % | `GET /api/todos/stats` → `onTimeRate` |
| Ingen ska bli överraskad | Försenade uppgifter som ingen har sett | okänt | 0 | Försenade ligger alltid överst i listan |
| Folk ska lita på listan | Felaktigt försenade uppgifter | flera per dag | 0 | Testerna för AK-2.1 och AK-2.2 |

## 3. Frågor vi ställde, och svaren
| Fråga | Svar från Sara |
|---|---|
| Vad händer idag när en deadline missas? | Kunden ringer. Då är det redan för sent. |
| När räknas något som försenat? Vid midnatt, eller vid arbetsdagens slut? | När dagen är slut. Har jag sagt fredag så får det bli klart fredag kväll. |
| Vilken tid gäller? Servern står i Irland. | Svensk tid, alltid. |
| Måste alla uppgifter ha ett datum? | Nej, mycket är "när du hinner". |
| Får man sätta ett datum som redan har passerat? | Nej, det är alltid ett misstag. |
| Hur vill du se vad som är bråttom? | Försenat högst upp, sedan det som är närmast. |
| Är vissa uppgifter viktigare än andra? | Ja, kunduppgifter. Låg, normal och hög räcker. |
| Hur vet du om det har blivit bättre om en månad? | Om fler saker blir klara i tid. Det vill jag kunna se själv. |
| Hur många uppgifter har ni? Hur många använder appen samtidigt? | Ett par hundra öppna. Alla åtta, ofta samtidigt på morgonen. |
| Vill ni ha mejl när något blir sent? | Gärna, men inte nu. Listan först. |

## 4. User stories och acceptanskriterier
**US-1** Som teammedlem vill jag kunna sätta ett förfallodatum på en uppgift, så att alla vet när den ska vara klar.
- **AK-1.1** Givet en ny uppgift med förfallodatum idag eller senare, när jag sparar, så sparas datumet.
- **AK-1.2** Givet ett förfallodatum som redan har passerat, när jag sparar, så nekas uppgiften med ett tydligt fel (400).
- **AK-1.3** Givet en ny uppgift utan förfallodatum, när jag sparar, så sparas den utan datum.

**US-2** Som teammedlem vill jag se vilka uppgifter som är försenade, så att jag kan agera innan kunden ringer.
- **AK-2.1** Givet en uppgift med förfallodatum idag, när klockan är 23:59 svensk tid, så är den *inte* försenad. Från 00:00 dagen efter är den det.
- **AK-2.2** Givet en uppgift som är klar, så är den aldrig försenad, oavsett datum.
- **AK-2.3** När jag hämtar `GET /api/todos/overdue`, så får jag bara försenade uppgifter, och hur många dagar sena de är.

**US-3** Som teammedlem vill jag att listan visar det mest brådskande först, så att jag vet vad jag ska göra härnäst.
- **AK-3.1** Försenade först, sedan efter förfallodatum (närmast först), sedan efter prioritet (hög först). Uppgifter utan datum kommer efter dem med datum, och klara uppgifter allra sist.

**US-4** Som teammedlem vill jag kunna markera hur viktig en uppgift är, så att kunduppgifter inte drunknar.
- **AK-4.1** Givet en ny uppgift utan prioritet, så får den prioritet Normal.
- **AK-4.2** Givet en okänd prioritet, så nekas uppgiften med ett tydligt fel (400).

**US-5** Som teamledare vill jag se hur många uppgifter som blev klara i tid, så att jag vet om det blir bättre.
- **AK-5.1** När en uppgift markeras som klar, så sparas när den blev klar.
- **AK-5.2** När jag hämtar `GET /api/todos/stats`, så får jag antal klara i tid, klara sent, försenade just nu och andelen klara i tid.

## 5. Use case: UC-1 Planera dagen
**Aktör:** teammedlem · **Mål:** veta vad som ska göras först idag · **Förutsättning:** appen är igång

**Huvudflöde (happy path)**
1. Teammedlemmen öppnar appen på morgonen.
2. Systemet visar listan: försenade uppgifter överst och tydligt markerade, sedan dagens och kommande.
3. Teammedlemmen väljer den översta uppgiften och gör den.
4. Teammedlemmen markerar den som klar.
5. Systemet sparar när den blev klar och flyttar ner den i listan.

**Alternativa flöden**
- **A1 Inget är försenat** (steg 2): listan börjar med dagens uppgifter. Ingen varning visas.
- **A2 Uppgiften blir klar för sent** (steg 5): den räknas som "klar sent" i statistiken, inte "i tid".
- **A3 Ingen uppgift har datum** (steg 2): listan visas som förut, nyast först.

**Undantagsflöden**
- **E1 API:t svarar inte** (steg 2): appen visar "Kunde inte hämta uppgifterna" i stället för en tom lista.
- **E2 Uppgiften har tagits bort av någon annan** (steg 4): systemet svarar 404 och appen säger det.

## 6. Icke-funktionella krav
| Krav | Gräns | Hur vi mäter |
|---|---|---|
| Svarstid, lista | `GET /api/todos` p99 under 200 ms med 1 000 uppgifter och 20 samtidiga användare | Lasttest med autocannon (`perf/`) |
| Sortering | Under 1 ms för 1 000 uppgifter | BenchmarkDotNet (`Starter.Benchmarks`) |
| Felfrekvens | 0 % fel under lasttestet | autocannon: `errors` och `non2xx` |
| Tid | Alla datumregler i svensk tid (Europe/Stockholm) | Testerna för AK-2.1 |

## 7. Utanför scope (just nu)
- Mejl eller notiser när något blir sent (Sara: "inte nu").
- Återkommande uppgifter.
- Vem som äger en uppgift.

## 8. Spårbarhet: krav → test
Varje acceptanskriterium har minst ett test som är märkt med kravets id:

```bash
dotnet test --filter "Krav=AK-2.1"
```
