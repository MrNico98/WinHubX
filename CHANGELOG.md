# Changelog

Il progetto segue release incrementali. Le modifiche rilevanti vengono riassunte qui e nei dettagli delle release GitHub.

## Prossima versione

- corretta la selezione e l'applicazione dei servizi Windows: conferma esplicita, una sola elevazione UAC per lotto e report di esito per ogni servizio;
- catalogo servizi remoto validato prima dell'uso, con URL corretto e supporto ai tipi di avvio Windows documentati;
- documentazione e template GitHub standardizzati;
- build con warning trattati come errori;
- dipendenze e aggiornamenti gestiti tramite Dependabot;
- miglioramenti progressivi a sicurezza, download HTTPS e prestazioni.
- il controllo aggiornamenti all'avvio non blocca più l'interfaccia con finestre modali; l'avviso apre le impostazioni;
- l'updater usa il manifest del fork, valida redirect HTTPS e richiede la verifica SHA-256 prima di sostituire l'eseguibile.
