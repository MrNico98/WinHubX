# Contribuire a WinHubX

Grazie per il contributo. Per modifiche importanti apri prima una issue descrivendo problema, motivazione e impatto.

## Workflow

1. Crea un fork e un branch descrittivo (`fix/nome`, `feat/nome`, `docs/nome`).
2. Mantieni le modifiche focalizzate e non includere `bin/`, `obj/`, `.vs/`, `publish/`, log o dump diagnostici.
3. Aggiorna documentazione, localizzazione e test quando il comportamento cambia.
4. Apri una pull request verso `main` con contesto, rischi e verifica eseguita.

## Verifica locale

```powershell
dotnet restore Project/WinHubX.sln
dotnet build Project/WinHubX.sln --configuration Release --warnaserror
dotnet run --project Project/WinHubX.Tests/WinHubX.Tests.csproj --configuration Release
git diff --check
```

La suite xUnit usa Microsoft.Testing.Platform; avviarla con `dotnet run` come sopra (anche in CI), dopo il restore/build della soluzione.

Non sopprimere warning per far passare la CI: correggi la causa o documenta una motivazione tecnica verificabile. I pacchetti NuGet devono provenire da fonti affidabili.

## Modifiche di sistema

Le operazioni su registro, servizi, attività pianificate, Defender o rete devono dichiarare cosa cambiano, richiedere elevazione solo quando necessaria, offrire un annullamento quando possibile, evitare riavvii automatici e usare percorsi/URL allowlistati.

Non aggiungere bypass di licenze, credenziali, chiavi private, certificati o codice destinato a eludere controlli di sicurezza.

## Pull request

Descrivi problema, soluzione, test, breaking change e impatto sui sistemi supportati. Le immagini sono utili per modifiche UI.

## Firma delle release

La firma Authenticode viene gestita fuori dal repository dal responsabile della pubblicazione. Non committare certificati, password, token o workflow con segreti incorporati.
