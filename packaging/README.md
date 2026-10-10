# Distribuzione WinHubX

La distribuzione ufficiale passa dalle GitHub Releases generate da
`.github/workflows/release.yml`.

Ogni release deve avere:

1. un tag semantico `vMAJOR.MINOR.PATCH`;
2. l’archivio `win-x64` prodotto dalla pipeline;
3. il file `.sha256` generato dalla pipeline;
4. `update.json` aggiornato con `version`, `updateUrl` e `sha256`;
5. firma del commit e verifica manuale dell’artefatto prima della pubblicazione.

## winget e Chocolatey

I manifest per winget e il pacchetto Chocolatey vanno generati a partire
dall’URL della release e dall’hash pubblicato, mai da URL locali o da file non
firmati. Dopo ogni release:

- aggiornare il manifest winget con `wingetcreate update`;
- eseguire `choco pack` sul nuspec versionato;
- verificare che URL, versione e SHA-256 coincidano;
- sottomettere i manifest ai repository ufficiali dopo la revisione.

Non vengono distribuiti script di attivazione, esclusioni Defender o eseguibili
scaricati dinamicamente.
