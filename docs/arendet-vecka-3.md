# Ärendet vecka 3: "Nu är vi fyra team i samma app"

## Mejlet från Sara

> Hej igen!
>
> Det gick ju bra med deadlines, tack för det. Nu har något hänt: tre andra team på företaget
> har sett vår app och vill också använda den. Ekonomi, Support och Utveckling. Så nu är vi fyra
> team som delar på samma app.
>
> En sak gör mig nervös. Ekonomi lägger in riktigt känsliga saker – lönekörningar, bonusunderlag,
> sånt som verkligen inte får läcka. Och Sälj ska absolut inte kunna peta i Supports uppgifter.
>
> Jag fick inte riktigt något svar när jag frågade utvecklaren hur det är löst. Hen sa "vi skickar
> med vilket team man är i" och bytte ämne. Det lät inte superlugnt.
>
> Kan ni titta på det? Kravet från min sida är enkelt: **inget team får se eller ändra ett annat
> teams uppgifter.** Punkt.
>
> /Sara, teamledare

## Hur det är löst (så här långt)

Det finns ingen inloggning ännu – den kommer senare. Just nu säger du vilket team du tillhör
genom att skicka headern `X-Team` med varje anrop, t.ex. `X-Team: ekonomi`. Appen litar på den.

De fyra teamen och deras header-värden:

| Team | `X-Team` |
|---|---|
| Sälj | `salj` |
| Ekonomi | `ekonomi` |
| Support | `support` |
| Utveckling | `utveckling` |

## Ditt uppdrag

Sätt på dig angriparhatten. Du spelar en nyfiken (eller elak) medlem i **Sälj** som vill komma
åt det du inte får. Kravet "inget team får se eller ändra ett annat teams uppgifter" ska hålla –
men gör det verkligen det?

> I den här appen finns **sex hål**. Några handlar om just det Sara är rädd för: att ett team
> kommer åt ett annat teams data. Andra är bredare säkerhetsmissar som en angripare älskar.
> Vi listar dem *inte* här – din uppgift är att hitta dem, ett i taget, och bevisa varje med ett
> anrop. På onsdag lagar vi dem test-först tillsammans.

Tips på var man brukar börja leta: vad händer om du byter `X-Team`? Vad händer om du skickar med
fält som du "inte borde" kunna sätta? Vad händer om du ber om en enskild uppgift i stället för
listan? Vad svarar servern när något går fel? Och vad händer om du anropar väldigt många gånger?

## curl-exempel (helt vanliga anrop att utgå från)

Lista dina egna uppgifter (som Sälj):

```bash
curl -H "X-Team: salj" http://localhost:5069/api/todos
```

Skapa en uppgift:

```bash
curl -H "X-Team: salj" -H "Content-Type: application/json" \
  -d '{"title":"Ring kunden","priority":"High"}' \
  http://localhost:5069/api/todos
```

Hämta en enskild uppgift (byt ut 101 mot ett id):

```bash
curl -H "X-Team: salj" http://localhost:5069/api/todos/101
```

Sök bland uppgifter:

```bash
curl -H "X-Team: salj" "http://localhost:5069/api/todos/search?q=offert"
```

Uppdatera eller ta bort:

```bash
curl -X PUT -H "X-Team: salj" -H "Content-Type: application/json" \
  -d '{"title":"Ny titel","priority":"Normal"}' \
  http://localhost:5069/api/todos/101

curl -X DELETE -H "X-Team: salj" http://localhost:5069/api/todos/101
```

Prova att byta `salj` mot `ekonomi`, `support` eller `utveckling` och se vad du får se.
Och fundera sedan på: vad *borde* du få se – och stämmer det?
