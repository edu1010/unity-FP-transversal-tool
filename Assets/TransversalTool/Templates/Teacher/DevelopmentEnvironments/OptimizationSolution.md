# Optimització i perfilat

## Mètriques observades
- Temps de càrrega: estable en entorn local.
- GC alloc: concentrat en lectura massiva de fitxers.
- Temps de generació: depèn del nombre d'activitats seleccionades.

## Millores proposades
- Reduir lectures repetides de fitxer durant validació.
- Afegir comprovacions prèvies de ruta per minimitzar errors de còpia.
