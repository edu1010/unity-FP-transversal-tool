//todo You are a Unity developer. Implement the system described in this specification as a Unity Editor tool.
# Especificación para Codex: herramienta Unity de configuración y generación de paquetes docente/alumno

## Objetivo

Implementar en Unity una herramienta de editor que permita usar un proyecto base como **eina transversal** para módulos de DAM y DAW. La herramienta debe permitir que el profesorado marque qué asignaturas participan y qué **resultats d’aprenentatge (RA)** de cada asignatura se trabajarán durante el curso. A partir de esta configuración, la herramienta debe generar una versión para profesorado y una versión para alumnado del proyecto, manteniendo la lógica siguiente:

- **Profesorado**: proyecto completo, con todas las soluciones implementadas.
- **Alumnado**: proyecto con estructura equivalente, pero con los ejercicios seleccionados **sin completar** para que el alumnado los resuelva.
- Si una actividad o RA **no** se marca en la configuración, el alumnado **no** tendrá que resolverla y recibirá esa parte **ya implementada** dentro del proyecto.
- Cada actividad debe poder evaluarse **de forma independiente**. No debe ser obligatorio corregirla dentro de Unity; puede entregarse como script C#, archivo JSON/XML, base de datos SQLite u otro recurso aislado.

## Contexto funcional

La herramienta nace para reducir la necesidad de coordinación intensa entre profesorado de distintas asignaturas y, al mismo tiempo, ofrecer al alumnado un producto común con apariencia de proyecto real. La idea es que, al inicio de curso, el profesorado interesado seleccione:

1. qué ciclo se utilizará como base: **DAM**, **DAW** o una base común configurable;
2. qué módulos participan;
3. qué RA de cada módulo se trabajarán;
4. qué actividades concretas se asociarán a cada RA.

Con esa selección, la herramienta debe generar un paquete de trabajo para el alumnado y mantener una copia completa para profesorado.

## Qué debe implementar Codex

### 1. Ventana de editor en Unity

Crear una `EditorWindow` accesible desde un menú como:

- `Tools/Transversal Tool/Package Generator`

La ventana debe incluir como mínimo:

- selector de ciclo base:
  - `DAM`
  - `DAW`
  - opcionalmente `Base común`
- campo de texto para nombre de configuración
- ruta de salida
- listado de módulos en formato desplegable/foldout
- casilla por módulo: **Participa**
- dentro de cada módulo, un desplegable con todos sus **RA**
- casilla por cada RA: **Treballar aquest RA**
- opción por cada RA para asociar una o varias actividades concretas
- botones:
  - `Load Config`
  - `Save Config`
  - `Generate Student Package`
  - `Generate Teacher Package`
  - `Generate Both`

### 2. Modelo de datos

Codex debe crear un sistema basado en `ScriptableObject` o JSON serializable con estas entidades mínimas:

- `CurriculumCatalog`
- `CycleDefinition`
- `ModuleDefinition`
- `LearningOutcomeDefinition`
- `ActivityDefinition`
- `GenerationConfig`

Campos recomendados:

#### CurriculumCatalog
- `cycles`

#### CycleDefinition
- `id`
- `displayName`
- `modules`

#### ModuleDefinition
- `code`
- `name`
- `enabled`
- `learningOutcomes`

#### LearningOutcomeDefinition
- `id`
- `title`
- `enabled`
- `activities`

#### ActivityDefinition
- `id`
- `title`
- `type`
- `studentTemplatePaths`
- `teacherSolutionPaths`
- `statementPaths`
- `resourcePaths`
- `deliverableType`  
  Ejemplos: `CSharpScript`, `JSON`, `XML`, `SQLite`, `SceneConfig`, `Documentation`

#### GenerationConfig
- `configName`
- `selectedCycle`
- `selectedModules`
- `selectedLearningOutcomes`
- `outputRoot`
- `includeTeacherPackage`
- `includeStudentPackage`

### 3. Lógica de generación

Codex debe implementar la lógica siguiente:

#### Paquete profesorado
Generar una carpeta `Professor` con el proyecto completo y resuelto.

#### Paquete alumnado
Generar una carpeta `Alumnes` con la misma estructura general del proyecto, pero aplicando estas reglas:

- Si un RA está marcado:
  - copiar los enunciados y recursos de la actividad;
  - copiar plantillas incompletas para el alumnado;
  - no copiar la solución final a la carpeta de alumnado.
- Si un RA no está marcado:
  - el alumnado recibe esa parte ya resuelta dentro del proyecto;
  - no se genera ejercicio pendiente para esa parte.

### 4. Estructura de carpetas a generar

La generación debe crear, como mínimo, esta estructura:

```text
Output/
  ConfigName/
    Professor/
      UnityProject/
        Assets/
        Packages/
        ProjectSettings/
      Docs/
        README_Professor.md
        ConfigSummary.json
    Alumnes/
      UnityProject/
        Assets/
        Packages/
        ProjectSettings/
      Exercicis/
        [ModuloCode]_[ModuloName]/
          [RA_ID]/
            Enunciat/
            Recursos/
            Plantilla/
            Entrega/
      Docs/
        README_Alumnes.md
        ConfigSummary.json
```

### 5. Estructura interna recomendada dentro de Unity

```text
Assets/
  TransversalTool/
    Editor/
      TransversalToolWindow.cs
      PackageGenerator.cs
      ConfigPersistence.cs
    Runtime/
      Models/
      Data/
      Utilities/
    Data/
      Curriculum/
        dam_catalog.json
        daw_catalog.json
      Activities/
    Templates/
      Student/
      Teacher/
    Documentation/
```

### 6. Regla de independencia de actividades

La herramienta debe estar preparada para que las actividades sean evaluables de forma aislada. Algunos ejemplos válidos:

- **Bases de dades**:
  - diseño del modelo de base de datos;
  - implementación SQLite;
  - consultas independientes.
- **Programació**:
  - script de spawn usando `for`;
  - script de movimiento;
  - script de colisiones.
- **Llenguatges de marques**:
  - lectura/escritura de JSON;
  - lectura/escritura de XML;
  - transformación o validación de datos.
- **Sistemes informàtics**:
  - configuración de estructura de ficheros;
  - automatización;
  - documentación técnica.
- **Entorns de desenvolupament**:
  - configuración del proyecto;
  - control de versiones;
  - pruebas;
  - documentación.
- **DAW**:
  - actividades de cliente, servidor, despliegue o diseño web desacopladas del resto del proyecto Unity, pero vinculadas a recursos, datos o servicios del proyecto base.

No se debe asumir que toda actividad se corrige ejecutando la escena de Unity. Debe poder entregarse y evaluarse como archivo o conjunto de archivos independiente.

### 7. Persistencia

Codex debe implementar:

- guardado de configuraciones en JSON;
- carga de configuraciones anteriores;
- posibilidad de duplicar una configuración base para varios grupos o cursos;
- resumen exportable con:
  - ciclo seleccionado;
  - módulos marcados;
  - RA marcados;
  - actividades generadas;
  - archivos incluidos.

### 8. Validaciones mínimas

La herramienta debe validar:

- que exista una ruta de salida válida;
- que haya al menos un ciclo seleccionado;
- que si un módulo está marcado tenga al menos un RA marcado, o bien que se advierta de que participa sin ejercicios activos;
- que los paths de plantillas y soluciones existan;
- que la generación no sobrescriba silenciosamente una exportación previa sin confirmación.

### 9. Criterios de aceptación

La implementación se considerará válida si:

1. se puede abrir la ventana desde el editor;
2. se puede seleccionar un ciclo base;
3. se pueden marcar módulos y desplegar sus RA;
4. se puede marcar individualmente cada RA;
5. se pueden guardar y cargar configuraciones;
6. se genera una carpeta de profesorado con el proyecto resuelto;
7. se genera una carpeta de alumnado con plantillas solo para los RA marcados;
8. los RA no marcados aparecen resueltos en la versión del alumnado;
9. se genera un resumen de configuración legible;
10. la herramienta puede ampliarse añadiendo nuevas actividades sin reescribir el sistema.

### 10. Consideraciones de implementación

- Usar Unity Editor tooling estándar.
- Priorizar legibilidad y extensibilidad.
- Separar datos curriculares, lógica de generación y UI.
- No hardcodear la lógica por escena concreta si puede modelarse con datos.
- Diseñar el sistema para que se puedan añadir nuevas actividades mediante assets o archivos JSON.
- Mantener nombres exactos de módulos y RA según catálogo curricular incluido abajo.
- El texto de UI puede mostrarse en catalán.

---

## Catálogo curricular a cargar en la herramienta

### DAM — Desenvolupament d’aplicacions multiplataforma

- **0373 · Llenguatges de marques i sistemes de gestió d’informació**
  - RA1: Reconeix les característiques de llenguatges de marques analitzant i interpretant fragments de codi.
  - RA2: Utilitza llenguatges de marques per a la transmissió i presentació de informació a través del web analitzant l'estructura dels documents i identificant-ne els elements.
  - RA3: Accedeix i manipula documents web utilitzant llenguatges de guions de client.
  - RA4: Estableix mecanismes de validació de documents per a l'intercanvi d'informació utilitzant mètodes per definir-ne la sintaxi i l'estructura.
  - RA5: Realitza conversions sobre documents per a l'intercanvi de informació utilitzant tècniques, llenguatges i eines de processament.
  - RA6: Gestiona la informació en formats d'intercanvi de dades analitzant i utilitzant tecnologies d'emmagatzematge i llenguatges de consulta.
  - RA7: Efectua operacions amb sistemes empresarials de gestió d'informació realitzant tasques d'importació, integració, assegurament i extracció de la informació.
- **0483 · Sistemes informàtics**
  - RA1: Avalua sistemes informàtics, identificant els seus components i característiques
  - RA2: Instal·la sistemes operatius planificant el procés i interpretant documentació tècnica.
  - RA3: Gestiona la informació del sistema identificant les estructures d'emmagatzematge i aplicant mesures per assegurar la integritat de les dades.
  - RA4: Gestiona sistemes operatius utilitzant comandes i eines gràfiques i avaluant les necessitats del sistema.
  - RA5: Interconnecta sistemes en xarxa configurant dispositius i protocols.
  - RA6: Treballa amb sistemes en xarxa gestionant-ne els recursos i identificant les restriccions de seguretat existents.
  - RA7: Elabora documentació valorant i utilitzant aplicacions informàtiques de propòsit general.
- **0484 · Bases de dades**
  - RA1: Reconeix els elements de les bases de dades analitzant-ne les funcions i valorant la utilitat dels sistemes gestors.
  - RA2: Crea bases de dades definint-ne l'estructura i les característiques dels elements segons el model relacional.
  - RA3: Consulta la informació emmagatzemada en una base de dades fent servir assistents, eines gràfiques i el llenguatge de manipulació de dades.
  - RA4: Modifica la informació emmagatzemada a la base de dades utilitzant assistents, eines gràfiques i el llenguatge de manipulació de dades.
  - RA5: Desenvolupa procediments emmagatzemats avaluant i utilitzant les sentències del llenguatge incorporat al sistema gestor de bases de dades.
  - RA6: Dissenya models relacionals normalitzats interpretant diagrames entitat/relació.
  - RA7: Gestiona la informació emmagatzemada en bases de dades no relacionals, avaluant i utilitzant les possibilitats que proporciona el sistema gestor.
- **0485 · Programació**
  - RA1: Reconeix l'estructura d'un programa informàtic, identificant i relacionant els elements propis del llenguatge de programació utilitzat.
  - RA2: Escriu i prova programes senzills, reconeixent i aplicant els fonaments de la programació orientada a objectes.
  - RA3: Escriu i depura codi, analitzant i utilitzant les estructures de control del llenguatge.
  - RA4: Desenvolupa programes organitzats en classes analitzant i aplicant els principis de la programació orientada a objectes.
  - RA5: Realitza operacions d'entrada i sortida d'informació, utilitzant procediments específics del llenguatge i llibreries de classes.
  - RA6: Escriu programes que manipulin informació seleccionant i utilitzant tipus avançats de dades.
  - RA7: Desenvolupa programes aplicant característiques avançades dels llenguatges orientats a objectes i de l'entorn de programació.
  - RA8: Utilitza bases de dades orientades a objectes, analitzant-ne les característiques i aplicant tècniques per mantenir la persistència de la informació.
  - RA9: Gestiona informació emmagatzemada en bases de dades mantenint la integritat i consistència de les dades.
- **0486 · Accés a dades**
  - RA1: Desenvolupa aplicacions que gestionen informació emmagatzemada en fitxers identificant-ne el camp d'aplicació i utilitzant classes específiques.
  - RA2: Desenvolupa aplicacions que gestionen informació emmagatzemada en bases de dades relacionals identificant i utilitzant mecanismes de connexió.
  - RA3: Gestiona la persistència de les dades identificant eines de mapatge objecte relacional (ORM) i desenvolupant aplicacions que les utilitzen.
  - RA4: Desenvolupa aplicacions que gestionen la informació emmagatzemada en bases de dades objecte relacionals i orientades a objectes valorant-ne les característiques i utilitzant els mecanismes d'accés incorporats.
  - RA5: Desenvolupa aplicacions que gestionen la informació emmagatzemada en bases de dades documentals natives avaluant i utilitzant classes específiques.
  - RA6: Programa components d'accés a dades identificant les característiques que ha de posseir un component i utilitzant eines de desenvolupament.
- **0487 · Entorns de desenvolupament**
  - RA1: Reconeix els elements i les eines que intervenen en el desenvolupament d'un programa informàtic, analitzant-ne les característiques i les fases en què actuen fins a arribar a la posada en funcionament.
  - RA2: Avalua entorns integrats de desenvolupament analitzant-ne les característiques per editar codi font i generar executables.
  - RA3: Verifica el funcionament de programes dissenyant i realitzant proves.
  - RA4: Optimitza codi utilitzant les eines disponibles a l'entorn de desenvolupament.
  - RA5: Genera diagrames de classes valorant-ne la importància en el desenvolupament d'aplicacions i emprant eines específiques.
  - RA6: Genera diagrames de comportament valorant-ne la importància en el desenvolupament d'aplicacions i emprant eines específiques.
- **0488 · Desenvolupament d’interfícies.**
  - RA1: Genera interfícies gràfiques d'usuari mitjançant editors visuals utilitzant les funcionalitats de l'editor i adaptant el codi generat.
  - RA2: Genera interfícies naturals d'usuari utilitzant eines visuals.
  - RA3: Crea components visuals valorant i emprant eines específiques.
  - RA4: Dissenya interfícies gràfiques identificant i aplicant criteris d'usabilitat i accessibilitat.
  - RA5: Crea informes avaluant i utilitzant eines gràfiques.
  - RA6: Documenta aplicacions seleccionant i utilitzant eines específiques.
  - RA7: Prepara aplicacions per a la distribució avaluant i utilitzant eines específiques.
  - RA8: Avalua el funcionament d'aplicacions dissenyant i executant proves.
- **0489 · Programació multimèdia i dispositius mòbils**
  - RA1: Aplica tecnologies de desenvolupament per a dispositius mòbils avaluant-ne les característiques i capacitats.
  - RA2: Desenvolupa aplicacions per a dispositius mòbils analitzant i emprant les tecnologies i llibreries específiques.
  - RA3: Desenvolupa programes que integren continguts multimèdia analitzant i emprant les tecnologies i llibreries específiques.
  - RA4: Selecciona i prova motors de jocs analitzant l'arquitectura de jocs 2D i 3D.
  - RA5: Desenvolupa jocs 2D i 3D senzills utilitzant motors de jocs.
- **0490 · Programació de serveis i processos**
  - RA1: Desenvolupa aplicacions compostes per diversos processos reconeixent i aplicant principis de programació paral·lela.
  - RA2: Desenvolupa aplicacions compostes per diversos fils d'execució analitzant i aplicant llibreries específiques del llenguatge de programació.
  - RA3: Programa mecanismes de comunicació en xarxa emprant sockets i analitzant l'escenari d'execució.
  - RA4: Desenvolupa aplicacions que ofereixen serveis en xarxa, utilitzant llibreries de classes i aplicant criteris d'eficiència i disponibilitat.
  - RA5: Protegeix les aplicacions i les dades definint i aplicant criteris de seguretat en l'accés, l'emmagatzematge i la transmissió de la informació.
- **0491 · Sistemes de gestió empresarial**
  - RA1: Identifica sistemes de planificació de recursos empresarials i de gestió de relacions amb clients (ERP-CRM) reconeixent-ne les característiques i verificant la configuració del sistema informàtic.
  - RA2: Implanta sistemes ERP-CRM interpretant la documentació tècnica i identificant les diferents opcions i mòduls.
  - RA3: Realitza operacions de gestió, consulta i anàlisi de la informació seguint les especificacions de disseny i utilitzant les eines proporcionades pels sistemes ERP-CRM.
  - RA4: Adapta sistemes ERP-CRM identificant els requeriments d'un supòsit empresarial i utilitzant les eines proporcionades per aquests.
  - RA5: Desenvolupa components per a un sistema ERP-CRM analitzant i utilitzant el llenguatge de programació incorporat.
- **0179 · Anglès professional**
  - RA1: Comprèn informació, d'índole professional, acadèmica i quotidiana, continguda en tota mena de discursos orals, emesos per qualsevol mitjà de comunicació en llengua estàndard, interpretant amb precisió el contingut del missatge.
  - RA2: Comprèn missatges escrits, de naturalesa professional, acadèmica i quotidiana, de relativa dificultat, analitzant de manera comprensiva el seu contingut.
  - RA3: Emet missatges orals clars i ben estructurats, analitzant el contingut de la situació i adaptant-se al registre lingüístic de l'interlocutor.
  - RA4: Redacta documents i informes, propis del sector o de la vida acadèmica i quotidiana, relacionant els recursos lingüístics amb el seu propòsit.
  - RA5: Aplica actituds i comportaments professionals en situacions de comunicació, descrivint les relacions típiques característiques del país de la llengua estrangera.
- **1665 · Digitalització aplicada als sectors productius**
  - RA1: Analitza el concepte de digitalització i la seva repercussió en els sectors productius tenint en compte l'activitat de l'empresa i identificant entorns IT (Information Technology: tecnologia de la informació) i OT (Operation Technology: tecnologia d'operació) característics.
  - RA2: Caracteritza les tecnologies habilitadores digitals necessàries per a l'adequació/transformació de les empreses a entorns digitals descrivint les seves característiques i aplicacions.
  - RA3: Identifica sistemes basats en cloud/núvol i la seva influència en el desenvolupament dels sistemes digitals.
  - RA4: Identifica aplicacions de la IA (intel·ligència artificial) en entorns del sector on està emmarcat el títol descrivint les millores implícites en la seva implementació
  - RA5: Avalua la importància de les dades, així com la seva protecció en una economia digital globalitzada, definint sistemes de seguretat i ciberseguretat tant a nivell d'equip/sistema, com a globals.
  - RA6: Desenvolupa un projecte de transformació digital d'una empresa d'un sector relacionat amb el títol, tenint en compte els canvis que s'han de produir en funció dels objectius de l'empresa.
- **1708 · Sostenibilitat Aplicada al Sistema Productiu**
  - RA1: Identifica els aspectes ambientals, socials i de governança (ASG) relatius a la sostenibilitat tenint en compte el concepte de desenvolupament sostenible i els marcs internacionals que contribueixen a la seva consecució.
  - RA2: Caracteritza els reptes ambientals i socials als quals s'enfronta la societat, descrivint els impactes sobre les persones i els sectors productius i proposant accions per a minimitzar-los.
  - RA3: Estableix l'aplicació de criteris de sostenibilitat en l'acompliment professional i personal, identificant els elements necessaris.
  - RA4: Proposa productes i serveis responsables tenint en compte els principis de l’economia circular.
  - RA5: Realitza activitats sostenibles minimitzant l'impacte de les mateixes en el medi ambient.
  - RA6: Analitza un pla de sostenibilitat d'una empresa del sector, identificant els seus grups d'interès, els aspectes ASG materials i justificant accions per a la seva gestió i mesurament.
- **1709 · Itinerari personal per a l'ocupabilitat I**
  - RA1: Distingeix les característiques del sector productiu i defineix els llocs de treball, relacionant-los amb les competències professionals expressades en el títol.
  - RA2: Assoleix les competències necessàries per a l'obtenció del títol de Tècnic Bàsic en Prevenció de Riscos Laborals.
  - RA3: Analitza i avalua el seu potencial professional i els seus interessos per a guiar-se en el procés d'autoorientació i elabora un full de ruta per a la inserció professional partint de l'anàlisi de les competències, interessos i destreses personals.
  - RA4: Aplica les estratègies per a l'aprenentatge autònom reconeixent el seu valor professionalitzador, dissenyant i optimitzant el seu propi entorn d'aprenentatge fent ús de les tecnologies digitals com a eines d'aprenentatge autònom, sent coherent amb la seva identitat digital i els seus objectius professionals plantejats en el seu pla de desenvolupament individual.
- **1710 · Itinerari personal per a l'ocupabilitat II**
  - RA1: Planifica i posa en marxa estratègies en els diferents processos selectius d'ocupació que li permeten millorar les seves possibilitats d'inserció laboral.
  - RA2: Aplica estratègies relacionades amb les competències personals, socials i emocionals per al desenvolupament de la seva iniciativa emprenedora i la millora de la seva ocupabilitat.
  - RA3: Posa en pràctica les habilitats emprenedores necessàries per al desenvolupament de processos d’innovació i recerca aplicades que promouen la modernització del sector productiu cap a un model sostenible.
  - RA4: Identifica, defineix i valida idees d’emprenedoria generadores de noves oportunitats a partir d’estratègies d’anàlisi de l’entorn socioproductiu utilitzant metodologies àgils per l’emprenedoria.
  - RA5: Desenvolupa un projecte emprenedor d’innovació social i/o tecnològica aplicada en col·laboració amb l’entorn.
- **0492 · Projecte intermodular de desenvolupament d'aplicacions multiplataforma**
  - RA1: Identifica necessitats del sector productiu, relacionant-les amb projectes tipus que les puguin satisfer.
  - RA2: Dissenya projectes relacionats amb les competències expressades al títol, desenvolupant explícitament les fases que el componen.
  - RA3: Planifica l'execució del projecte, determinant el pla d'intervenció i la documentació associada.
  - RA4: Defineix els procediments per al seguiment i el control en l'execució del projecte, justificant la selecció de variables i instruments emprats.

### DAW — Desenvolupament d’aplicacions web

- **0373 · Llenguatges de marques i sistemes de gestió d’informació**
  - RA1: Reconeix les característiques de llenguatges de marques analitzant i interpretant fragments de codi.
  - RA2: Utilitza llenguatges de marques per a la transmissió i presentació de informació a través del web analitzant l'estructura dels documents i identificant-ne els elements.
  - RA3: Accedeix i manipula documents web utilitzant llenguatges de guions de client.
  - RA4: Estableix mecanismes de validació de documents per a l'intercanvi d'informació utilitzant mètodes per definir-ne la sintaxi i l'estructura.
  - RA5: Realitza conversions sobre documents per a l'intercanvi de informació utilitzant tècniques, llenguatges i eines de processament.
  - RA6: Gestiona la informació en formats d'intercanvi de dades analitzant i utilitzant tecnologies d'emmagatzematge i llenguatges de consulta.
  - RA7: Efectua operacions amb sistemes empresarials de gestió d'informació realitzant tasques d'importació, integració, assegurament i extracció de la informació.
- **0483 · Sistemes informàtics**
  - RA1: Avalua sistemes informàtics, identificant els seus components i característiques
  - RA2: Instal·la sistemes operatius planificant el procés i interpretant documentació tècnica.
  - RA3: Gestiona la informació del sistema identificant les estructures d'emmagatzematge i aplicant mesures per assegurar la integritat de les dades.
  - RA4: Gestiona sistemes operatius utilitzant comandes i eines gràfiques i avaluant les necessitats del sistema.
  - RA5: Interconnecta sistemes en xarxa configurant dispositius i protocols.
  - RA6: Treballa amb sistemes en xarxa gestionant-ne els recursos i identificant les restriccions de seguretat existents.
  - RA7: Elabora documentació valorant i utilitzant aplicacions informàtiques de propòsit general.
- **0484 · Bases de dades**
  - RA1: Reconeix els elements de les bases de dades analitzant-ne les funcions i valorant la utilitat dels sistemes gestors.
  - RA2: Crea bases de dades definint-ne l'estructura i les característiques dels elements segons el model relacional.
  - RA3: Consulta la informació emmagatzemada en una base de dades fent servir assistents, eines gràfiques i el llenguatge de manipulació de dades.
  - RA4: Modifica la informació emmagatzemada a la base de dades utilitzant assistents, eines gràfiques i el llenguatge de manipulació de dades.
  - RA5: Desenvolupa procediments emmagatzemats avaluant i utilitzant les sentències del llenguatge incorporat al sistema gestor de bases de dades.
  - RA6: Dissenya models relacionals normalitzats interpretant diagrames entitat/relació.
  - RA7: Gestiona la informació emmagatzemada en bases de dades no relacionals, avaluant i utilitzant les possibilitats que proporciona el sistema gestor.
- **0485 · Programació**
  - RA1: Reconeix l'estructura d'un programa informàtic, identificant i relacionant els elements propis del llenguatge de programació utilitzat.
  - RA2: Escriu i prova programes senzills, reconeixent i aplicant els fonaments de la programació orientada a objectes.
  - RA3: Escriu i depura codi, analitzant i utilitzant les estructures de control del llenguatge.
  - RA4: Desenvolupa programes organitzats en classes analitzant i aplicant els principis de la programació orientada a objectes.
  - RA5: Realitza operacions d'entrada i sortida d'informació, utilitzant procediments específics del llenguatge i llibreries de classes.
  - RA6: Escriu programes que manipulin informació seleccionant i utilitzant tipus avançats de dades.
  - RA7: Desenvolupa programes aplicant característiques avançades dels llenguatges orientats a objectes i de l'entorn de programació.
  - RA8: Utilitza bases de dades orientades a objectes, analitzant-ne les característiques i aplicant tècniques per mantenir la persistència de la informació.
  - RA9: Gestiona informació emmagatzemada en bases de dades mantenint la integritat i consistència de les dades.
- **0487 · Entorns de desenvolupament**
  - RA1: Reconeix els elements i les eines que intervenen en el desenvolupament d'un programa informàtic, analitzant-ne les característiques i les fases en què actuen fins a arribar a la posada en funcionament.
  - RA2: Avalua entorns integrats de desenvolupament analitzant-ne les característiques per editar codi font i generar executables.
  - RA3: Verifica el funcionament de programes dissenyant i realitzant proves.
  - RA4: Optimitza codi utilitzant les eines disponibles a l'entorn de desenvolupament.
  - RA5: Genera diagrames de classes valorant-ne la importància en el desenvolupament d'aplicacions i emprant eines específiques.
  - RA6: Genera diagrames de comportament valorant-ne la importància en el desenvolupament d'aplicacions i emprant eines específiques.
- **0612 · Desenvolupament web en entorn client**
  - RA1: Selecciona les arquitectures i tecnologies de programació sobre clients web, identificant i analitzant les capacitats i les característiques de cadascuna.
  - RA2: Escriu sentències simples, aplicant la sintaxi del llenguatge i verificant la seva execució sobre navegadors web.
  - RA3: Escriu codi, identificant i aplicant les funcionalitats aportades pels objectes predefinits del llenguatge.
  - RA4: 4. Programa codi per a clients web analitzant i utilitzant estructures definides per l’usuari.
  - RA5: Desenvolupa aplicacions web interactives integrant mecanismes de maneig d'esdeveniments.
  - RA6: Desenvolupa aplicacions web analitzant i aplicant les característiques del model d'objectes del document.
  - RA7: Desenvolupa aplicacions web dinàmiques, reconeixent i aplicant mecanismes de comunicació asíncrona entre client i servidor.
- **0613 · Desenvolupament web en entorn servidor**
  - RA1: Selecciona les arquitectures i tecnologies de programació web en entorn servidor, analitzant les capacitats i característiques pròpies.
  - RA2: Escriu sentències executables per un servidor web reconeixent i aplicant procediments d’integració del codi en llenguatges de marques.
  - RA3: Escriu blocs de sentències embeguts en llenguatges de marques, seleccionant i utilitzant les estructures de programació.
  - RA4: Desenvolupa aplicacions web embegudes en llenguatges de marques analitzant i incorporant funcionalitats segons especificacions.
  - RA5: Desenvolupa aplicacions web identificant i aplicant mecanismes per separar el codi de presentació de la lògica de negoci.
  - RA6: Desenvolupa aplicacions web d'accés a magatzems de dades, aplicant mesures per mantenir la seguretat i la integritat de la informació.
  - RA7: Desenvolupa serveis web reutilitzables i accessibles mitjançant protocols web, verificant-ne el funcionament.
  - RA8: Genera pàgines web dinàmiques analitzant i utilitzant tecnologies i frameworks del servidor web que afegeixin codi al llenguatge de marques.
  - RA9: Desenvolupa aplicacions web híbrides seleccionant i utilitzant tecnologies, frameworks servidor i repositoris heterogenis d'informació.
- **0614 · Desplegament d’aplicacions web**
  - RA1: Implanta arquitectures web analitzant i aplicant criteris de funcionalitat.
  - RA2: Implanta aplicacions web en servidors web, avaluant i aplicant criteris de configuració per al funcionament segur.
  - RA3: Implanta aplicacions web en servidors d'aplicacions, avaluant i aplicant criteris de configuració per al funcionament segur.
  - RA4: Administra servidors de transferència de fitxers, avaluant i aplicant criteris de configuració que garanteixin la disponibilitat del servei.
  - RA5: Verifica l'execució d'aplicacions web comprovant els paràmetres de configuració de serveis de xarxa.
  - RA6: Elabora la documentació de l’aplicació web avaluant i seleccionant eines de generació de documentació, control de versions i d’integració contínua.
- **0615 · Disseny d’interfícies web**
  - RA1: Planifica la creació d'una interfície web valorant i aplicant especificacions de disseny.
  - RA2: Crea interfícies web homogènies definint i aplicant estils.
  - RA3: Prepara arxius multimèdia per al web, analitzant-ne les característiques i manejant eines específiques.
  - RA4: Integra contingut multimèdia en documents web valorant la seva aportació i seleccionant adequadament els elements interactius.
  - RA5: Desenvolupa interfícies web accessibles, analitzant les pautes establertes i aplicant-hi tècniques de verificació.
  - RA6: Desenvolupa interfícies web amigables analitzant i aplicant les pautes d'usabilitat establertes.
  - RA7: Principis i Pautes d'Accessibilitat al Contingut a la Web (WCAG).
- **0179 · Anglès professional**
  - RA1: Comprèn informació, d'índole professional, acadèmica i quotidiana, continguda en tota mena de discursos orals, emesos per qualsevol mitjà de comunicació en llengua estàndard, interpretant amb precisió el contingut del missatge.
  - RA2: Comprèn missatges escrits, de naturalesa professional, acadèmica i quotidiana, de relativa dificultat, analitzant de manera comprensiva el seu contingut.
  - RA3: Emet missatges orals clars i ben estructurats, analitzant el contingut de la situació i adaptant-se al registre lingüístic de l'interlocutor.
  - RA4: Redacta documents i informes, propis del sector o de la vida acadèmica i quotidiana, relacionant els recursos lingüístics amb el seu propòsit.
  - RA5: Aplica actituds i comportaments professionals en situacions de comunicació, descrivint les relacions típiques característiques del país de la llengua estrangera.
- **1665 · Digitalització aplicada als sectors productius**
  - RA1: Analitza el concepte de digitalització i la seva repercussió en els sectors productius tenint en compte l'activitat de l'empresa i identificant entorns IT (Information Technology: tecnologia de la informació) i OT (Operation Technology: tecnologia d'operació) característics.
  - RA2: Caracteritza les tecnologies habilitadores digitals necessàries per a l'adequació/transformació de les empreses a entorns digitals descrivint les seves característiques i aplicacions.
  - RA3: Identifica sistemes basats en cloud/núvol i la seva influència en el desenvolupament dels sistemes digitals.
  - RA4: Identifica aplicacions de la IA (intel·ligència artificial) en entorns del sector on està emmarcat el títol descrivint les millores implícites en la seva implementació
  - RA5: Avalua la importància de les dades, així com la seva protecció en una economia digital globalitzada, definint sistemes de seguretat i ciberseguretat tant a nivell d'equip/sistema, com a globals.
  - RA6: Desenvolupa un projecte de transformació digital d'una empresa d'un sector relacionat amb el títol, tenint en compte els canvis que s'han de produir en funció dels objectius de l'empresa.
- **1708 · Sostenibilitat Aplicada al Sistema Productiu**
  - RA1: Identifica els aspectes ambientals, socials i de governança (ASG) relatius a la sostenibilitat tenint en compte el concepte de desenvolupament sostenible i els marcs internacionals que contribueixen a la seva consecució.
  - RA2: Caracteritza els reptes ambientals i socials als quals s'enfronta la societat, descrivint els impactes sobre les persones i els sectors productius i proposant accions per a minimitzar-los.
  - RA3: Estableix l'aplicació de criteris de sostenibilitat en l'acompliment professional i personal, identificant els elements necessaris.
  - RA4: Proposa productes i serveis responsables tenint en compte els principis de l’economia circular.
  - RA5: Realitza activitats sostenibles minimitzant l'impacte de les mateixes en el medi ambient.
  - RA6: Analitza un pla de sostenibilitat d'una empresa del sector, identificant els seus grups d'interès, els aspectes ASG materials i justificant accions per a la seva gestió i mesurament.
- **1709 · Itinerari personal per a l'ocupabilitat I**
  - RA1: Distingeix les característiques del sector productiu i defineix els llocs de treball, relacionant-los amb les competències professionals expressades en el títol.
  - RA2: Assoleix les competències necessàries per a l'obtenció del títol de Tècnic Bàsic en Prevenció de Riscos Laborals.
  - RA3: Analitza i avalua el seu potencial professional i els seus interessos per a guiar-se en el procés d'autoorientació i elabora un full de ruta per a la inserció professional partint de l'anàlisi de les competències, interessos i destreses personals.
  - RA4: Aplica les estratègies per a l'aprenentatge autònom reconeixent el seu valor professionalitzador, dissenyant i optimitzant el seu propi entorn d'aprenentatge fent ús de les tecnologies digitals com a eines d'aprenentatge autònom, sent coherent amb la seva identitat digital i els seus objectius professionals plantejats en el seu pla de desenvolupament individual.
- **1710 · Itinerari personal per a l'ocupabilitat II**
  - RA1: Planifica i posa en marxa estratègies en els diferents processos selectius d'ocupació que li permeten millorar les seves possibilitats d'inserció laboral.
  - RA2: Aplica estratègies relacionades amb les competències personals, socials i emocionals per al desenvolupament de la seva iniciativa emprenedora i la millora de la seva ocupabilitat.
  - RA3: Posa en pràctica les habilitats emprenedores necessàries per al desenvolupament de processos d’innovació i recerca aplicades que promouen la modernització del sector productiu cap a un model sostenible.
  - RA4: Identifica, defineix i valida idees d’emprenedoria generadores de noves oportunitats a partir d’estratègies d’anàlisi de l’entorn socioproductiu utilitzant metodologies àgils per l’emprenedoria.
  - RA5: Desenvolupa un projecte emprenedor d’innovació social i/o tecnològica aplicada en col·laboració amb l’entorn.
- **0616 · Projecte intermodular de desenvolupament d'aplicacions web**
  - RA1: Identifica necessitats del sector productiu, relacionant-les amb projectes tipus que les puguin satisfer.
  - RA2: Dissenya projectes relacionats amb les competències expressades al títol, desenvolupant explícitament les fases que el componen.
  - RA3: Planifica l'execució del projecte, determinant el pla d'intervenció i la documentació associada.
  - RA4: Defineix els procediments per al seguiment i el control en l'execució del projecte, justificant la selecció de variables i instruments emprats.

---

## Requisitos adicionales importantes

1. La herramienta debe permitir reutilizar una misma base de Unity durante todo el curso.
2. El profesorado debe poder adherirse o no adherirse al sistema al inicio de curso.
3. La ausencia de adhesión de una asignatura no debe romper el proyecto del alumnado.
4. El alumnado debe recibir un proyecto funcional aunque solo participe una parte del profesorado.
5. El sistema debe estar preparado para crecer con nuevas actividades por RA.

## Entregables esperados de Codex

Codex debe producir como mínimo:

- scripts C# del editor;
- clases de datos;
- serialización JSON;
- generador de carpetas y copia de archivos;
- uno o varios catálogos curriculares iniciales;
- documentación breve de uso;
- ejemplo funcional con al menos:
  - 1 actividad de Programació,
  - 1 actividad de Bases de dades,
  - 1 actividad de Llenguatges de marques.

## Nota para Codex

Si hay ambigüedad entre la carpeta del proyecto Unity y la carpeta de ejercicios externos, priorizar este comportamiento:

- los recursos que afecten al proyecto global deben quedar integrados en `UnityProject`;
- los ejercicios entregables por separado deben quedar además reflejados en `Exercicis/`;
- la versión de profesorado siempre conserva la solución completa;
- la versión de alumnado solo deja sin resolver lo que haya sido marcado como trabajo activo.
