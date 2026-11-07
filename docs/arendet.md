# Ärendet: "Vi missar deadlines hela tiden"

## Mejlet från kunden

> Hej!
>
> Vi är ett team på åtta personer som använder er uppgiftsapp varje dag. Problemet är att vi missar
> deadlines hela tiden, och ingen märker det förrän kunden ringer och är arg.
>
> Kan ni fixa det?
>
> /Sara, teamledare

Det är allt. Ingen specifikation, inga skisser, inga siffror. Precis som i verkligheten.

## Vad som redan finns

En konsult (Kim) hann börja innan hen bytte uppdrag. Kim lade till ett förfallodatum (`DueDate`) på
uppgifter och en endpoint, `GET /api/todos/overdue`, som visar försenade uppgifter.
Det finns inga tester för något av det.

## Buggrapport från Sara, två dagar efter demon

> Appen säger att saker är försenade samma dag som de ska vara klara. Klockan nio på morgonen står det
> att "Skicka offert" är en dag sen, fast den ska vara klar idag. Folk har slutat lita på listan.

## Ditt uppdrag

1. Ta reda på vad Sara egentligen behöver (måndag).
2. Gör Kims kod testbar, och bevisa buggen med ett test innan du rättar den (onsdag).
3. Bygg resten test-först, och visa med siffror att det fungerar (onsdag till fredag).
