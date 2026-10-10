# Sviluppo e verifica

## Toolchain

Compila su Windows con il SDK indicato dal `global.json` del branch. `main` usa .NET 10 stabile; `experiment/net11-modernization` richiede il relativo SDK .NET 11 preview. Mantieni separati i due target: non portare il preview nella release stabile senza una migrazione e una verifica dedicate.

## Verifica locale

Dalla radice del repository:

```powershell
dotnet --info
git status --short --branch
dotnet restore Project/WinHubX.sln
dotnet build Project/WinHubX.sln --configuration Release --no-restore -warnaserror
dotnet run --project Project/WinHubX.Tests/WinHubX.Tests.csproj --configuration Release --no-build --no-restore
dotnet list Project/WinHubX/WinHubX.csproj package --vulnerable --include-transitive
git diff --check
```

Il progetto usa Microsoft Testing Platform; per eseguire i test usare `dotnet run` come nella pipeline, non affidarsi a `dotnet test` se il runner MTP segnala un errore di handshake.

La pipeline GitHub Actions esegue restore, build Release con warning-as-error, test, audit NuGet, publish Windows, smoke test dell'avvio e creazione del manifest SHA-256. CodeQL viene eseguito separatamente. Un esito locale non sostituisce la conferma dei workflow sul commit pubblicato.

## Modifiche e pull request

- Mantieni una modifica focalizzata e aggiungi test per i comportamenti modificati.
- Non sopprimere warning o analizzatori; risolvi la causa o documenta il limite supportato.
- Non committare output di build, IDE, profiler, dump, impostazioni personali o log.
- Prima del push controlla `git diff --check`, build e test del branch corretto; usa commit firmati secondo la configurazione del repository.
- Non includere credenziali, token, certificati privati o dati personali in issue, log e artefatti.

Per i dettagli della distribuzione consulta [Build e pubblicazione](../RELEASE.md); per le convenzioni di contributo consulta [`CONTRIBUTING.md`](../../CONTRIBUTING.md).
