# Sicurezza

I download devono usare HTTPS, allowlist di host e verifica dell'integrità quando disponibile. Le operazioni elevate devono usare il minimo privilegio, spiegare l'effetto e non riavviare automaticamente il sistema.

Gli archivi ZIP scaricati sono estratti soltanto dopo la validazione dei nomi e dei percorsi canonici; traversal, link/reparse point e archivi oltre i limiti di quantità/dimensione vengono rifiutati. L'estrazione avviene in una directory di staging nuova e l'app mostra l'errore senza proseguire se il contenuto non è valido.

Le risorse temporanee per la creazione ISO sono conservate in sessioni GUID sotto `%LOCALAPPDATA%\WinHubX\IsoResourceSessions`, con ACL limitate all'utente corrente, agli amministratori e a SYSTEM; il percorso viene validato prima della pulizia e non viene riutilizzata una directory condivisa sotto `%TEMP%`.

I download binari usano BITS dopo che `TrustedHttpsClient` ha seguito e rivalidato ogni redirect HTTPS; il job BITS vieta ulteriori redirect, verifica la revoca del certificato e scrive su un file temporaneo univoco, pubblicato nella destinazione solo dopo il completamento. L'annullamento rimuove anche il job BITS residuo.

Gli URL degli asset attualmente configurati in `Dipendenze.json` sono stati verificati come raggiungibili il 2026-10-09. Alcuni puntano alla release upstream `MrNico98/WinHubX-Resource`; il fork `LightYagami28/WinHubX-Resource` non ha release proprie. La raggiungibilità non equivale a verifica di provenienza o integrità: usare hash pubblicati/validati indipendentemente quando disponibili. L'integrazione DefendNot è esclusa dalla migrazione BITS e dall'esecuzione dell'audit perché può disattivare Defender; il suo hash x64 non è stato corretto né la sua affidabilità di avvio aumentata.

La creazione ISO e le riparazioni di sistema mantengono l'applicazione principale `asInvoker` e richiedono una sola elevazione UAC per operazione. Il broker accetta solo comandi `DISM`, `reg.exe`, `sfc.exe`, `chkdsk.exe` e `regsvr32.exe` con argomenti convalidati; non esegue comandi shell ricevuti dall'interfaccia. Pipe e workspace di sessione hanno ACL dedicate. I file temporanei sono confinati a una cartella casuale sul disco fisso con più spazio e il completamento elimina solo quella cartella, mai directory globali come `C:\ISO` o `C:\mount`. Il controllo disco usa `chkdsk /scan` online e non pianifica scansioni al riavvio. Il test end-to-end UAC va eseguito su Windows con un'immagine ISO e un ambiente di prova prima del rilascio.

Non includere certificati Authenticode, password, token o chiavi private nel repository. La firma delle release viene eseguita fuori dal controllo versione dal maintainer.

Per segnalazioni riservate consulta [SECURITY.md](../../SECURITY.md).
