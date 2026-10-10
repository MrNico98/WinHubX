# WinHubX — documentazione

WinHubX è una toolbox per manutenzione, diagnostica e personalizzazione di Windows. Le operazioni che modificano il sistema sono esplicite; prova le funzioni sensibili in una macchina virtuale o dopo un backup verificato.

## Guide

- [Installazione e primo avvio](Installation.md)
- [Sviluppo, build e test](Development.md)
- [Sicurezza, download e privilegi](Security.md)
- [Risoluzione problemi](Troubleshooting.md)
- [Build e pubblicazione](../RELEASE.md)
- [Roadmap tecnica](../ROADMAP-MODERNIZZAZIONE.md)
- [Politica di sicurezza e segnalazioni](../../SECURITY.md)

## Stato dei target

- `main`: target stabile .NET 10.
- `experiment/net11-modernization`: target sperimentale .NET 11; non usare per release finché la toolchain non è stabile e la pipeline non lo dichiara supportato.

La documentazione non certifica l'esito di operazioni sul computer: le procedure UAC e creazione ISO richiedono un collaudo separato in ambiente di prova.
