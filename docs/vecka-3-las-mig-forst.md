# ⚠️ Vecka 3 – läs mig först

## Det här är en sårbar övningsbranch

Branchen `exercise/vecka-3-holes` är **medvetet osäker**. Vi har byggt in säkerhetshål på
riktigt, så att du ska kunna hitta och laga dem själv. Det är hela poängen med veckan:
*Tänk som en angripare.*

> **Kör bara lokalt. Deploya aldrig den här koden. Lägg aldrig in riktig data.**
>
> Testa bara mot din egen lokala app. Att leta sårbarheter i system du inte äger är olagligt.
> Här övar vi etiskt: du angriper din egen kod.

## Vad som är nytt den här veckan

Tre nya team vill använda appen. Nu delar fyra team på samma app och samma databas:

| Team | `X-Team` | 
|---|---|
| Sälj | `salj` |
| Ekonomi | `ekonomi` |
| Support | `support` |
| Utveckling | `utveckling` |

Det finns ingen inloggning ännu (det kommer vecka 4). I stället säger du vilket team du
tillhör genom att skicka en **HTTP-header**: `X-Team: salj`. Appen litar på headern den här
veckan – det är helt okej för övningen. Hålen ligger någon annanstans.

Kravet från kunden är enkelt och hårt:

> **Inget team får se eller ändra ett annat teams uppgifter.**

## Din uppgift

1. Starta appen lokalt (se nedan) och lär dig hur den fungerar som en vanlig användare.
2. Läs `docs/arendet-vecka-3.md` – där berättar Sara vad hon är orolig för.
3. Sätt på dig angriparhatten och **hitta hålen**. De finns där. Bevisa varje hål.
4. Laga dem test-först (det gör vi tillsammans på lektionerna onsdag).

## Starta lokalt

**Backend** (http://localhost:5069)

```bash
cd backend
dotnet run --project Starter.Api
```

Databasen `starter.db` skapas och fylls med de fyra teamens uppgifter första gången.
Vill du börja om från rena data: ta bort `backend/Starter.Api/starter.db` och starta igen.

**Frontend** (http://localhost:5173)

```bash
cd frontend
npm install
npm run dev
```

I frontend finns en liten team-väljare högst upp. Den styr vilken `X-Team`-header som skickas.

## Hur du anropar API:t

Se `docs/arendet-vecka-3.md` för färdiga `curl`-exempel, och `requests.http` som du kan köra
direkt i VS Code / Rider (REST Client / HTTP-klienten).
