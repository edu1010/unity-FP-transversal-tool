# Diagrama de classes (descripció)

## Classes principals
- `CatalogLoader`: càrrega catàleg i activitats.
- `PackageGenerator`: genera paquets docents i d'alumnat.
- `TransversalToolWindow`: interfície d'editor i flux d'usuari.

## Relacions
- `TransversalToolWindow` depèn de `CatalogLoader` i `PackageGenerator`.
- `PackageGenerator` utilitza `ActivityDefinition` i `GenerationConfig`.
