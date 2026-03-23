using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TransversalTool
{
    public class TransversalToolWindow : EditorWindow
    {
        CurriculumCatalog _catalog;
        GenerationConfig _config;
        Vector2 _scroll;
        Dictionary<string, bool> _moduleFoldouts = new Dictionary<string, bool>();
        Dictionary<string, bool> _raFoldouts = new Dictionary<string, bool>();

        [MenuItem("Tools/Transversal Tool/Package Generator")]
        public static void Open()
        {
            GetWindow<TransversalToolWindow>("Transversal Tool");
        }

        void OnEnable()
        {
            _catalog = CatalogLoader.LoadCatalog();
            if (_config == null)
            {
                _config = new GenerationConfig
                {
                    configName = "Config_" + System.DateTime.Now.ToString("yyyyMMdd"),
                    outputRoot = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Output"),
                    includeStudentPackage = true,
                    includeTeacherPackage = true
                };
            }

            if (_catalog.cycles.Count > 0 && string.IsNullOrEmpty(_config.selectedCycle))
            {
                _config.selectedCycle = _catalog.cycles[0].id;
            }

            EnsureConfigForCycle();
        }

        void OnGUI()
        {
            if (_catalog == null)
            {
                _catalog = CatalogLoader.LoadCatalog();
            }

            EditorGUILayout.LabelField("Configuració", EditorStyles.boldLabel);
            DrawCycleSelector();
            _config.configName = EditorGUILayout.TextField("Nom configuració", _config.configName);

            using (new EditorGUILayout.HorizontalScope())
            {
                _config.outputRoot = EditorGUILayout.TextField("Ruta sortida", _config.outputRoot);
                if (GUILayout.Button("...", GUILayout.Width(32)))
                {
                    var path = EditorUtility.OpenFolderPanel("Output Root", _config.outputRoot, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        _config.outputRoot = path;
                    }
                }
            }

            _config.includeTeacherPackage = EditorGUILayout.ToggleLeft("Incloure paquet professorat", _config.includeTeacherPackage);
            _config.includeStudentPackage = EditorGUILayout.ToggleLeft("Incloure paquet alumnat", _config.includeStudentPackage);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mòduls i RA", EditorStyles.boldLabel);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawModules();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            DrawButtons();
        }

        void DrawCycleSelector()
        {
            var options = new List<string>();
            var selectedIndex = 0;
            for (var i = 0; i < _catalog.cycles.Count; i++)
            {
                var cycle = _catalog.cycles[i];
                options.Add(cycle.id + " - " + cycle.displayName);
                if (cycle.id == _config.selectedCycle)
                {
                    selectedIndex = i;
                }
            }

            var newIndex = EditorGUILayout.Popup("Cicle base", selectedIndex, options.ToArray());
            if (newIndex != selectedIndex && newIndex >= 0 && newIndex < _catalog.cycles.Count)
            {
                _config.selectedCycle = _catalog.cycles[newIndex].id;
                EnsureConfigForCycle();
            }
        }

        void DrawModules()
        {
            var cycle = GetSelectedCycle();
            if (cycle == null)
            {
                EditorGUILayout.HelpBox("No hi ha cicle carregat.", MessageType.Info);
                return;
            }

            foreach (var module in cycle.modules)
            {
                var moduleSelection = GetOrCreateModuleSelection(module.code, module.learningOutcomes);
                if (!_moduleFoldouts.ContainsKey(module.code))
                {
                    _moduleFoldouts[module.code] = false;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _moduleFoldouts[module.code] = EditorGUILayout.Foldout(_moduleFoldouts[module.code], module.code + " · " + module.name, true);
                    moduleSelection.enabled = EditorGUILayout.ToggleLeft("Participa", moduleSelection.enabled, GUILayout.Width(90));
                }

                if (_moduleFoldouts[module.code])
                {
                    EditorGUI.indentLevel++;
                    foreach (var ra in module.learningOutcomes)
                    {
                        var raSelection = GetOrCreateLearningOutcomeSelection(moduleSelection, ra.id);
                        var raKey = module.code + "_" + ra.id;
                        if (!_raFoldouts.ContainsKey(raKey))
                        {
                            _raFoldouts[raKey] = false;
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            _raFoldouts[raKey] = EditorGUILayout.Foldout(_raFoldouts[raKey], ra.id + " · " + ra.title, true);
                            raSelection.enabled = EditorGUILayout.ToggleLeft("Treballar aquest RA", raSelection.enabled, GUILayout.Width(160));
                        }

                        if (_raFoldouts[raKey])
                        {
                            EditorGUI.indentLevel++;
                            if (ra.activities.Count == 0)
                            {
                                EditorGUILayout.LabelField("(Sense activitats assignades)");
                            }
                            foreach (var activity in ra.activities)
                            {
                                var selected = raSelection.selectedActivityIds.Contains(activity.id);
                                var newSelected = EditorGUILayout.ToggleLeft("Activitat: " + activity.title, selected);
                                if (newSelected != selected)
                                {
                                    if (newSelected)
                                    {
                                        raSelection.selectedActivityIds.Add(activity.id);
                                    }
                                    else
                                    {
                                        raSelection.selectedActivityIds.Remove(activity.id);
                                    }
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        void DrawButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Load Config"))
                {
                    var path = ConfigPersistence.PromptLoadPath();
                    if (!string.IsNullOrEmpty(path))
                    {
                        var loaded = ConfigPersistence.LoadConfig(path);
                        if (loaded != null)
                        {
                            _config = loaded;
                            EnsureConfigForCycle();
                        }
                    }
                }

                if (GUILayout.Button("Save Config"))
                {
                    var path = ConfigPersistence.PromptSavePath(_config.configName);
                    if (!string.IsNullOrEmpty(path))
                    {
                        ConfigPersistence.SaveConfig(_config, path);
                    }
                }
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Generate Student Package"))
                {
                    GeneratePackages(includeTeacher: false, includeStudent: true);
                }
                if (GUILayout.Button("Generate Teacher Package"))
                {
                    GeneratePackages(includeTeacher: true, includeStudent: false);
                }
                if (GUILayout.Button("Generate Both"))
                {
                    GeneratePackages(includeTeacher: true, includeStudent: true);
                }
            }
        }

        void GeneratePackages(bool includeTeacher, bool includeStudent)
        {
            _config.includeTeacherPackage = includeTeacher;
            _config.includeStudentPackage = includeStudent;

            if (!PackageGenerator.Validate(_config, _catalog, out var errors, out var warnings))
            {
                EditorUtility.DisplayDialog("Validation", string.Join("\n", errors), "OK");
                return;
            }

            if (warnings.Count > 0)
            {
                var proceed = EditorUtility.DisplayDialog("Warnings", string.Join("\n", warnings) + "\n\nContinue?", "Continue", "Cancel");
                if (!proceed)
                {
                    return;
                }
            }

            var outputRoot = Path.Combine(_config.outputRoot, _config.configName);
            if (Directory.Exists(outputRoot))
            {
                var overwrite = EditorUtility.DisplayDialog("Overwrite", "Output already exists. Overwrite?", "Overwrite", "Cancel");
                if (!overwrite)
                {
                    return;
                }
                Directory.Delete(outputRoot, true);
            }

            Directory.CreateDirectory(outputRoot);
            PackageGenerator.Generate(_config, _catalog);
            EditorUtility.DisplayDialog("Done", "Packages generated.", "OK");
        }

        void EnsureConfigForCycle()
        {
            var cycle = GetSelectedCycle();
            if (cycle == null)
            {
                return;
            }

            if (_config.selectedModules == null)
            {
                _config.selectedModules = new List<ModuleSelection>();
            }

            foreach (var module in cycle.modules)
            {
                var moduleSelection = GetOrCreateModuleSelection(module.code, module.learningOutcomes);
                foreach (var ra in module.learningOutcomes)
                {
                    GetOrCreateLearningOutcomeSelection(moduleSelection, ra.id);
                }
            }
        }

        CycleDefinition GetSelectedCycle()
        {
            foreach (var cycle in _catalog.cycles)
            {
                if (cycle.id == _config.selectedCycle)
                {
                    return cycle;
                }
            }
            return null;
        }

        ModuleSelection GetOrCreateModuleSelection(string moduleCode, List<LearningOutcomeDefinition> ras)
        {
            foreach (var module in _config.selectedModules)
            {
                if (module.moduleCode == moduleCode)
                {
                    return module;
                }
            }

            var selection = new ModuleSelection
            {
                moduleCode = moduleCode,
                enabled = false,
                learningOutcomes = new List<LearningOutcomeSelection>()
            };
            _config.selectedModules.Add(selection);
            return selection;
        }

        LearningOutcomeSelection GetOrCreateLearningOutcomeSelection(ModuleSelection moduleSelection, string raId)
        {
            foreach (var ra in moduleSelection.learningOutcomes)
            {
                if (ra.learningOutcomeId == raId)
                {
                    return ra;
                }
            }

            var selection = new LearningOutcomeSelection
            {
                learningOutcomeId = raId,
                enabled = false,
                selectedActivityIds = new List<string>()
            };
            moduleSelection.learningOutcomes.Add(selection);
            return selection;
        }
    }
}
