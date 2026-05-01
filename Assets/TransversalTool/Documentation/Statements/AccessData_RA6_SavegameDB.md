# Accés a dades RA6 - Guardar i recuperar el savegame

En aquesta activitat connectes el treball de serialització (JSON de partida) amb una base de dades.

Objectius:

- Crear una taula per desar snapshots de partida (`savegame`).
- Implementar un component d'accés a dades amb dos mètodes:
  - `SaveSnapshot(...)` per guardar el JSON.
  - `LoadLatestSnapshot(...)` per recuperar l'últim registre.
- Validar que el JSON recuperat és idèntic (o equivalent) al que s'ha guardat.

Pots suposar que la connexió a base de dades ja està disponible i centrar-te en sentències, paràmetres i lectura de resultats.
