# Installazione e primo avvio

## Requisiti

- Windows 10 versione 1809 o successiva; Windows 11 è consigliato.
- Windows x64.
- .NET 10 Desktop Runtime per la release stabile, salvo che il pacchetto sia pubblicato come self-contained.
- Connessione Internet per componenti o cataloghi scaricati su richiesta.

## Installare una release

1. Scarica l'artefatto dalla [pagina Release di WinHubX](https://github.com/LightYagami28/WinHubX/releases).
2. Estrai l'intera cartella in una posizione locale stabile, per esempio sotto `%LOCALAPPDATA%` o una cartella applicazioni scelta dall'utente. Non eseguire direttamente da un archivio o da una cartella temporanea.
3. Se la release contiene `SHA256SUMS.json`, confronta il digest SHA-256 locale con la voce relativa al file. Il manifest protegge dall'alterazione solo se la release di provenienza è autentica; non sostituisce Authenticode.
4. Avvia `WinHubX.exe`. Concedi UAC solo quando hai scelto un'operazione che richiede privilegi amministrativi e verifica il nome dell'applicazione nel prompt.

Le release possono essere prive di firma Authenticode: il certificato di firma non è incluso nel repository. Non disattivare Defender o SmartScreen per avviare il programma; se la provenienza non è verificabile, non eseguirlo.

## Primo avvio

- Controlla la versione di Windows e crea un punto di ripristino o un backup prima di applicare modifiche di sistema.
- Esamina descrizione e risultato previsto prima di ogni tweak. L'app non riavvia Windows automaticamente.
- Per creazione ISO, usa immagini Windows ufficiali e un ambiente di prova; le procedure di riparazione e modifica offline non sono un collaudo sostitutivo del sistema reale.

## Compilare dai sorgenti

Clona il repository, seleziona il branch desiderato e segui il `global.json` presente in quel branch. I comandi di build e test sono nella guida [Sviluppo](Development.md); i profili di publish sono descritti in [Build e pubblicazione](../RELEASE.md).
