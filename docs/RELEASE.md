# Build e pubblicazione

Questa guida descrive i profili riproducibili verificati per WinHubX. La release stabile usa il branch `main` con SDK .NET 10; `experiment/net11-modernization` è un branch sperimentale e richiede l'SDK .NET 11 preview indicato dal suo `global.json`. Le versioni esatte sono vincolate dai rispettivi `global.json`: non sostituirle con un SDK arbitrario durante una build di release.

## Prerequisiti

- Windows x64;
- Git e .NET SDK della versione indicata dal `global.json` del branch;
- per eseguire una build framework-dependent, il corrispondente .NET Desktop Runtime;
- rete per il restore iniziale dei pacchetti NuGet.

Controllare prima dell'inizio:

```powershell
dotnet --info
git status --short --branch
```

## Verifica locale

Eseguire dalla radice del repository:

```powershell
dotnet restore Project/WinHubX.sln
dotnet build Project/WinHubX.sln --configuration Release --no-restore --warnaserror
dotnet run --project Project/WinHubX.Tests/WinHubX.Tests.csproj --configuration Release --no-build --no-restore
dotnet list Project/WinHubX.sln package --vulnerable --include-transitive
git diff --check
```

La pipeline Windows esegue anche lo smoke test del publish e CodeQL. Una release non va pubblicata se uno di questi controlli fallisce.
L'artefatto CI contiene `SHA256SUMS.json`, con hash SHA-256 e dimensione di ogni file pubblicato, incluso l'eseguibile. Per i binari il manifest registra anche lo stato Authenticode: se firmato, la pipeline richiede `Valid` e registra soggetto e thumbprint; se non firmato, riporta esplicitamente `NotSigned`. Ogni firma presente ma non valida interrompe la pipeline. Per validare un file scaricato, confrontare il digest ricalcolato localmente con la voce `path` corrispondente; il manifest da solo non sostituisce una firma Authenticode o una fonte di pubblicazione autenticata.

L'updater verifica l'hash del download e della copia staged nella stessa directory dell'app, sostituisce l'eseguibile con backup, avvia la nuova versione e attende la finestra principale. Se il processo non parte o non diventa pronto entro 20 secondi, viene terminato e il backup viene ripristinato. I test coprono successo, hash errato e rollback.

## Profili supportati

### Singolo eseguibile, runtime installato

```powershell
dotnet publish Project/WinHubX/WinHubX.csproj --configuration Release --runtime win-x64 --self-contained false --warnaserror
```

Distribuire l'intera directory `Project/WinHubX/bin/Release/<TFM>/win-x64/publish/`, non solo l'eseguibile: le dipendenze native possono restare file separati. È necessario il Desktop Runtime della stessa major.

### Singolo eseguibile self-contained

```powershell
dotnet publish Project/WinHubX/WinHubX.csproj --configuration Release --runtime win-x64 --self-contained true --warnaserror
```

Questo profilo include il runtime e abilita la compressione del bundle. L'output è specifico per Windows x64 e può richiedere spazio temporaneo all'avvio per estrarre librerie native.

## Profili non supportati

- **Trimming:** non abilitarlo. L'SDK .NET rifiuta la pubblicazione WinForms con `NETSDK1175`; il framework dipende da componenti COM incorporati che il linker non può rimuovere in sicurezza.
- **Native AOT:** non dichiarato supportato né verificato per questa applicazione WinForms e il relativo insieme di dipendenze.
- **ReadyToRun:** non è un profilo di release finché non viene misurato su Windows con confronto di dimensione, startup e memoria rispetto al profilo normale.

Non aggirare questi limiti con `NoWarn`, proprietà di compatibilità o soppressioni: una futura adozione richiede supporto ufficiale, build pulita, avvio verificato e test funzionali.

## Versioni e architetture

Il target framework e il pin SDK sono intenzionalmente distinti tra i due branch. Pubblicare e testare ogni branch separatamente; `win-x64` è l'unico RID verificato nel checkpoint documentato. Non promuovere il branch .NET 11 preview a release stabile prima che il relativo SDK/runtime sia stabile e che tutti i controlli siano verdi.
