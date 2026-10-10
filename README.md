# WinHubX

WinHubX è una toolbox open source per Windows 10/11, progettata per manutenzione, diagnostica, personalizzazione e gestione di immagini ufficiali Microsoft.

[![Build](https://github.com/LightYagami28/WinHubX/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LightYagami28/WinHubX/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/LightYagami28/WinHubX)](LICENSE)
[![Latest release](https://img.shields.io/github/v/release/LightYagami28/WinHubX)](https://github.com/LightYagami28/WinHubX/releases/latest)

> Stato: sviluppo attivo. Provare ogni modifica in una macchina virtuale o con un backup verificato.

## Funzionalità

- diagnostica hardware e monitoraggio delle risorse;
- strumenti per privacy, manutenzione e personalizzazione di Windows;
- download da fonti ufficiali HTTPS e verifica dell'integrità quando disponibile;
- strumenti per immagini ISO, driver e componenti Microsoft;
- interfaccia WinForms moderna, localizzata in italiano e inglese.

WinHubX non automatizza attivazioni non autorizzate e non distribuisce certificati, chiavi o credenziali.

## Requisiti

- Windows 10 1809 o successivo, preferibilmente Windows 11;
- architettura x64;
- .NET 10 Desktop Runtime per la release stabile;
- privilegi amministrativi solo per le operazioni di sistema che li richiedono.

## Installazione e compilazione

Scarica il pacchetto dalla [release più recente](https://github.com/LightYagami28/WinHubX/releases/latest), estrailo e avvia `WinHubX.exe`.

```powershell
dotnet restore Project/WinHubX.sln
dotnet build Project/WinHubX.sln --configuration Release --warnaserror
dotnet run --project Project/WinHubX.Tests/WinHubX.Tests.csproj --configuration Release
```

La suite include test di sicurezza per la validazione dei preset `.reg` importati.

Il branch `main` segue .NET 10. `experiment/net11-modernization` è sperimentale e richiede l'SDK .NET 11 preview compatibile.

## Sicurezza

Usa esclusivamente sorgenti ufficiali e controlla gli hash pubblicati con la release. Segnala vulnerabilità responsabilmente senza allegare token, password, certificati o dati personali.

WinHubX non include una firma Authenticode: le release possono essere firmate dal manutentore prima della pubblicazione.

## Documentazione

- [FAQ](Faq.md)
- [Contribuire](CONTRIBUTING.md)
- [Build e pubblicazione](docs/RELEASE.md)
- [Roadmap di modernizzazione](docs/ROADMAP-MODERNIZZAZIONE.md)

## Licenza

Distribuito secondo la [GNU GPL v3.0](LICENSE).

## Crediti

Progetto originario di MrNico98, mantenuto e modernizzato dalla community e dai contributori.
