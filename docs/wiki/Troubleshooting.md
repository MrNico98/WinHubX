# Risoluzione problemi

## WinHubX non si avvia

1. Verifica Windows x64 e il Desktop Runtime .NET 10 richiesto dalla release framework-dependent.
2. Estrai nuovamente tutti i file in una cartella locale stabile; non avviare l'eseguibile direttamente dallo ZIP.
3. Verifica l'integrità usando il manifest della stessa release e una fonte di download autentica. Non aggirare avvisi Defender o SmartScreen.
4. Se continua a non avviarsi, apri una issue con versione Windows, versione WinHubX, branch/commit e messaggio d'errore. Rimuovi prima nomi utente, percorsi personali e altri dati sensibili.

## Download o catalogo non disponibile

- Verifica connessione, data/ora, proxy aziendale e disponibilità della fonte ufficiale.
- WinHubX accetta solo HTTPS e un insieme ristretto di host; un redirect verso un host non attendibile viene rifiutato intenzionalmente.
- Non sostituire l'URL con mirror o file eseguibili trovati in siti terzi. Riprova più tardi oppure segnala l'URL configurato e l'errore, senza allegare token.

## Il prompt UAC non compare o un'operazione elevata fallisce

Annulla e ripeti l'operazione solo dopo averne compreso l'effetto. Il programma principale resta non elevato; il prompt dovrebbe essere limitato all'azione che richiede privilegi. Se il broker rifiuta un comando, non avviare manualmente comandi amministrativi equivalenti: raccogli il testo dell'errore e segnala il problema.

## Un tweak sembra richiedere il riavvio

Alcune impostazioni di Windows o servizi diventano effettive dopo un nuovo accesso o un riavvio. Salva il lavoro e riavvia manualmente solo quando il programma o Windows lo richiede; WinHubX non deve forzare il riavvio.

## Build o test falliscono

Assicurati che il branch e il SDK corrispondano al `global.json`, esegui `dotnet restore` e poi i comandi della guida [Sviluppo](Development.md). Se i test MTP falliscono durante l'handshake, ripeti con `dotnet run --project Project/WinHubX.Tests/WinHubX.Tests.csproj --configuration Release --no-build --no-restore` e conserva l'errore completo.

## Segnalare un problema

Usa il [modello issue](https://github.com/LightYagami28/WinHubX/issues/new/choose) appropriato. Includi i passaggi minimi per riprodurlo e l'esito atteso/effettivo; non pubblicare dump, configurazioni personali o dati di sistema non necessari. Per vulnerabilità usa la procedura privata in [`SECURITY.md`](../../SECURITY.md).
