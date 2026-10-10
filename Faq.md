# FAQ

## Quali sistemi operativi sono supportati?

Windows 10 e Windows 11 su x64. Alcune funzioni dipendono dall'edizione, dagli aggiornamenti installati e dai privilegi disponibili.

## Come installo WinHubX?

Scarica l'archivio dalla [release più recente](https://github.com/LightYagami28/WinHubX/releases/latest), estrailo e avvia `WinHubX.exe`. Non usare mirror non verificati.

## WinHubX richiede sempre l'amministratore?

No. Usa il livello minimo possibile; Windows può richiedere elevazione per modifiche a servizi, registro, rete o componenti protetti.

## Perché una funzione chiede il riavvio?

Alcune modifiche diventano effettive solo dopo il riavvio o dopo il riavvio di un servizio. L'applicazione dovrebbe indicare il motivo e non riavviare automaticamente il computer.

## I download sono sicuri?

I download integrati devono usare HTTPS e fonti ufficiali. Verifica sempre URL, hash della release e firma del file quando disponibile.

## Come segnalo un bug o una vulnerabilità?

Apri una [issue](https://github.com/LightYagami28/WinHubX/issues/new/choose) per i bug, senza dati personali o segreti. Per una vulnerabilità non pubblicare dettagli sfruttabili: usa il canale privato indicato nelle impostazioni di sicurezza del repository.

## Posso contribuire?

Sì. Leggi [CONTRIBUTING.md](CONTRIBUTING.md), crea un branch dedicato e apri una pull request descrittiva.

## Le release sono firmate Authenticode?

Non necessariamente. La firma Authenticode richiede un certificato attendibile; il repository non contiene certificati, password o procedure di firma.
