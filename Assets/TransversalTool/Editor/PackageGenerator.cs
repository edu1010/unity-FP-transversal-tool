using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TransversalTool
{
    public static class PackageGenerator
    {
        public static bool Validate(GenerationConfig config, CurriculumCatalog catalog, out List<string> errors, out List<string> warnings)
        {
            errors = new List<string>();
            warnings = new List<string>();

            if (config == null)
            {
                errors.Add("Config is null.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(config.configName))
            {
                errors.Add("Config name is required.");
            }

            if (string.IsNullOrWhiteSpace(config.outputRoot))
            {
                errors.Add("Output root is required.");
            }
            else if (!Directory.Exists(config.outputRoot))
            {
                errors.Add("Output root does not exist: " + config.outputRoot);
            }

            var cycle = GetCycle(catalog, config.selectedCycle);
            if (cycle == null)
            {
                errors.Add("A valid cycle must be selected.");
            }
            else
            {
                foreach (var module in cycle.modules)
                {
                    var moduleSel = FindModuleSelection(config, module.code);
                    if (moduleSel != null && moduleSel.enabled)
                    {
                        var anyRa = moduleSel.learningOutcomes.Any(lo => lo.enabled);
                        if (!anyRa)
                        {
                            warnings.Add("Module " + module.code + " marked without active RA.");
                        }
                    }
                }
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            foreach (var activity in EnumerateActivities(cycle))
            {
                foreach (var path in activity.studentTemplatePaths)
                {
                    if (!PathExists(projectRoot, path))
                    {
                        errors.Add("Missing student template: " + path);
                    }
                }
                foreach (var path in activity.teacherSolutionPaths)
                {
                    if (!PathExists(projectRoot, path))
                    {
                        errors.Add("Missing teacher solution: " + path);
                    }
                }
                foreach (var path in activity.statementPaths)
                {
                    if (!PathExists(projectRoot, path))
                    {
                        errors.Add("Missing statement: " + path);
                    }
                }
                foreach (var path in activity.resourcePaths)
                {
                    if (!PathExists(projectRoot, path))
                    {
                        errors.Add("Missing resource: " + path);
                    }
                }
            }

            return errors.Count == 0;
        }

        public static void Generate(GenerationConfig config, CurriculumCatalog catalog)
        {
            if (config == null)
            {
                return;
            }

            var cycle = GetCycle(catalog, config.selectedCycle);
            if (cycle == null)
            {
                return;
            }

            var outputRoot = Path.Combine(config.outputRoot, config.configName);
            if (config.includeTeacherPackage)
            {
                GenerateTeacher(config, cycle, outputRoot);
            }

            if (config.includeStudentPackage)
            {
                GenerateStudent(config, cycle, outputRoot);
            }

            AssetDatabase.Refresh();
        }

        static void GenerateTeacher(GenerationConfig config, CycleDefinition cycle, string outputRoot)
        {
            var teacherRoot = Path.Combine(outputRoot, "Professor");
            var unityProjectRoot = Path.Combine(teacherRoot, "UnityProject");
            var docsRoot = Path.Combine(teacherRoot, "Docs");

            CopyUnityProject(unityProjectRoot);
            ApplyTeacherSolutions(cycle, unityProjectRoot);
            RemoveToolFromProject(unityProjectRoot);
            WriteDocs(docsRoot, "README_Professor.md", config, cycle, true);
            WriteSummary(docsRoot, config, cycle, true);
        }

        static void GenerateStudent(GenerationConfig config, CycleDefinition cycle, string outputRoot)
        {
            var studentRoot = Path.Combine(outputRoot, "Alumnes");
            var unityProjectRoot = Path.Combine(studentRoot, "UnityProject");
            var docsRoot = Path.Combine(studentRoot, "Docs");

            CopyUnityProject(unityProjectRoot);
            ApplyStudentSelection(config, cycle, unityProjectRoot);
            RemoveToolFromProject(unityProjectRoot);
            WriteDocs(docsRoot, "README_Alumnes.md", config, cycle, false);
            WriteSummary(docsRoot, config, cycle, false);
        }

        // The generated Unity project must not contain the tool itself: the Templates folder
        // holds compilable .cs files whose classes are also copied into TransversalExercises by
        // ApplyTeacherSolutions/ApplyStudentSelection, which would cause duplicate-definition
        // (CS0101) errors. The tool is only needed in the authoring project, so strip it out.
        static void RemoveToolFromProject(string unityProjectRoot)
        {
            var toolPath = Path.Combine(unityProjectRoot, "Assets", "TransversalTool");
            if (Directory.Exists(toolPath))
            {
                Directory.Delete(toolPath, true);
            }

            var toolMeta = toolPath + ".meta";
            if (File.Exists(toolMeta))
            {
                File.Delete(toolMeta);
            }
        }

        static void CopyUnityProject(string destinationRoot)
        {
            Directory.CreateDirectory(destinationRoot);
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            CopyDirectory(Path.Combine(projectRoot, "Assets"), Path.Combine(destinationRoot, "Assets"));
            CopyDirectory(Path.Combine(projectRoot, "Packages"), Path.Combine(destinationRoot, "Packages"));
            CopyDirectory(Path.Combine(projectRoot, "ProjectSettings"), Path.Combine(destinationRoot, "ProjectSettings"));
        }

        static void ApplyTeacherSolutions(CycleDefinition cycle, string unityProjectRoot)
        {
            foreach (var activity in EnumerateActivities(cycle))
            {
                CopyActivityFiles(activity.teacherSolutionPaths, activity.targetPaths, unityProjectRoot);
            }
        }

        static void ApplyStudentSelection(GenerationConfig config, CycleDefinition cycle, string unityProjectRoot)
        {
            // Everything the student receives lives inside the Unity project so it is visible in the
            // editor: the activities they must complete go under Assets/Transversal/Exercicis (with
            // their statement and resources next to the editable script), and the parts that are
            // delivered already done go under Assets/Transversal/Material_Resolt, whose name makes
            // clear that this material is provided complete.
            var assetsRoot = Path.Combine(unityProjectRoot, "Assets", "Transversal");
            var exercisesRoot = Path.Combine(assetsRoot, "Exercicis");
            var solvedRoot = Path.Combine(assetsRoot, "Material_Resolt");

            // The same exercise can appear twice in the catalog (a curriculum activity and its AEA
            // annex twin share module, RA and source files). Emitting both would place the same class
            // twice and break compilation, so activities are collapsed by their file signature. If
            // the same exercise is worked in one place and not in another, the worked (template)
            // version wins: a class cannot be present as an exercise and as solved material at once.
            var planned = new Dictionary<string, PlannedActivity>();
            var order = new List<string>();

            foreach (var module in cycle.modules)
            {
                var moduleSelection = FindModuleSelection(config, module.code);
                foreach (var ra in module.learningOutcomes)
                {
                    var raSelection = FindLearningOutcomeSelection(moduleSelection, ra.id);
                    var raSelected = raSelection != null && raSelection.enabled && moduleSelection != null && moduleSelection.enabled;
                    var selectedActivityIds = raSelection != null ? raSelection.selectedActivityIds : null;

                    foreach (var activity in ra.activities)
                    {
                        var signature = ActivitySignature(activity);
                        var worked = raSelected && (selectedActivityIds == null || selectedActivityIds.Count == 0 || selectedActivityIds.Contains(activity.id));

                        if (planned.TryGetValue(signature, out var existing))
                        {
                            if (worked && !existing.worked)
                            {
                                planned[signature] = new PlannedActivity { activity = activity, module = module, ra = ra, worked = true };
                            }
                            continue;
                        }

                        planned[signature] = new PlannedActivity { activity = activity, module = module, ra = ra, worked = worked };
                        order.Add(signature);
                    }
                }
            }

            foreach (var signature in order)
            {
                var plan = planned[signature];
                var basePath = Path.Combine(
                    plan.worked ? exercisesRoot : solvedRoot,
                    Sanitize(plan.module.code + "_" + plan.module.name),
                    Sanitize(plan.ra.id),
                    Sanitize(plan.activity.id));

                if (plan.worked)
                {
                    CopyAssetsInto(plan.activity.studentTemplatePaths, basePath);
                    CopyAssetsInto(plan.activity.statementPaths, Path.Combine(basePath, "Enunciat"));
                    CopyAssetsInto(plan.activity.resourcePaths, Path.Combine(basePath, "Recursos"));
                }
                else
                {
                    CopyAssetsInto(plan.activity.teacherSolutionPaths, basePath);
                }
            }
        }

        // Identifies an activity by the neutralized names of its code/template/solution files, so
        // twin activities that produce the same classes share a signature and are emitted once.
        // Activities without such files fall back to their id so they are always kept (e.g. those
        // that only carry a statement).
        static string ActivitySignature(ActivityDefinition activity)
        {
            var names = new List<string>();
            if (activity.studentTemplatePaths != null)
            {
                foreach (var path in activity.studentTemplatePaths)
                {
                    names.Add(ToolingSuffix.Replace(Path.GetFileName(path), "$1"));
                }
            }
            if (activity.teacherSolutionPaths != null)
            {
                foreach (var path in activity.teacherSolutionPaths)
                {
                    names.Add(ToolingSuffix.Replace(Path.GetFileName(path), "$1"));
                }
            }

            if (names.Count == 0)
            {
                return "id:" + activity.id;
            }

            names.Sort(StringComparer.OrdinalIgnoreCase);
            return string.Join("|", names.ToArray());
        }

        static void CopyAssetsInto(List<string> sourcePaths, string destinationFolder)
        {
            if (sourcePaths == null)
            {
                return;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            foreach (var path in sourcePaths)
            {
                var source = ResolvePath(projectRoot, path);
                if (!File.Exists(source))
                {
                    continue;
                }
                var fileName = ToolingSuffix.Replace(Path.GetFileName(source), "$1");
                CopyAsset(source, Path.Combine(destinationFolder, fileName));
            }
        }

        class PlannedActivity
        {
            public ActivityDefinition activity;
            public ModuleDefinition module;
            public LearningOutcomeDefinition ra;
            public bool worked;
        }

        static void CopyActivityFiles(List<string> sourcePaths, List<string> targetPaths, string unityProjectRoot)
        {
            if (sourcePaths == null || sourcePaths.Count == 0)
            {
                return;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            for (var i = 0; i < sourcePaths.Count; i++)
            {
                var source = ResolvePath(projectRoot, sourcePaths[i]);
                var target = ResolveTargetPath(targetPaths, i, source, unityProjectRoot);
                if (string.IsNullOrEmpty(target))
                {
                    continue;
                }
                CopyAsset(source, target);
            }
        }

        // Tool-internal class names carry the suffixes "Solution"/"Template"/"Given" so the teacher
        // and student versions can coexist (and compile) inside the authoring project without
        // colliding. The generated packages, however, must use neutral names: the script file name
        // has to match its class for MonoBehaviour to load, and the teacher solution and the student
        // template must be drop-in interchangeable for the same slot of the functional base project.
        // For .cs files we therefore strip those suffixes from every identifier while copying.
        static readonly Regex ToolingSuffix = new Regex(@"([A-Za-z0-9_]+?)(Solution|Template|Given)\b");

        static void CopyAsset(string source, string target)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            if (Path.GetExtension(source).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                var content = ToolingSuffix.Replace(File.ReadAllText(source), "$1");
                File.WriteAllText(target, content);
            }
            else
            {
                File.Copy(source, target, true);
            }
        }

        static string ResolveTargetPath(List<string> targetPaths, int index, string source, string unityProjectRoot)
        {
            if (targetPaths == null || targetPaths.Count == 0)
            {
                return null;
            }

            var targetRelative = targetPaths.Count > index ? targetPaths[index] : targetPaths[0];
            var target = ResolvePath(unityProjectRoot, targetRelative);
            if (Directory.Exists(target))
            {
                return Path.Combine(target, Path.GetFileName(source));
            }

            return target;
        }

        static string ResolvePath(string root, string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(root, path.Replace("/", Path.DirectorySeparatorChar.ToString()));
        }

        static bool PathExists(string projectRoot, string path)
        {
            var full = ResolvePath(projectRoot, path);
            return File.Exists(full) || Directory.Exists(full);
        }

        static IEnumerable<ActivityDefinition> EnumerateActivities(CycleDefinition cycle)
        {
            if (cycle == null)
            {
                yield break;
            }

            foreach (var module in cycle.modules)
            {
                foreach (var ra in module.learningOutcomes)
                {
                    foreach (var activity in ra.activities)
                    {
                        yield return activity;
                    }
                }
            }
        }

        static void CopyDirectory(string source, string destination)
        {
            if (!Directory.Exists(source))
            {
                return;
            }

            Directory.CreateDirectory(destination);
            foreach (var file in Directory.GetFiles(source))
            {
                var destFile = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(source))
            {
                var destDir = Path.Combine(destination, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }

        static void WriteDocs(string docsRoot, string readmeName, GenerationConfig config, CycleDefinition cycle, bool isTeacher)
        {
            Directory.CreateDirectory(docsRoot);
            var readmePath = Path.Combine(docsRoot, readmeName);
            var lines = new List<string>
            {
                "# Transversal Tool",
                "",
                "Config: " + config.configName,
                "Cycle: " + cycle.id + " - " + cycle.displayName,
                "Package: " + (isTeacher ? "Professor" : "Alumnes"),
                "Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            File.WriteAllLines(readmePath, lines);
        }

        static void WriteSummary(string docsRoot, GenerationConfig config, CycleDefinition cycle, bool isTeacher)
        {
            var summary = new ConfigSummary
            {
                configName = config.configName,
                cycle = cycle.id,
                packageType = isTeacher ? "Professor" : "Alumnes",
                modules = new List<ConfigSummary.ModuleSummary>()
            };

            foreach (var module in cycle.modules)
            {
                var moduleSelection = FindModuleSelection(config, module.code);
                var moduleSummary = new ConfigSummary.ModuleSummary
                {
                    code = module.code,
                    name = module.name,
                    selected = moduleSelection != null && moduleSelection.enabled,
                    learningOutcomes = new List<ConfigSummary.LearningOutcomeSummary>()
                };

                foreach (var ra in module.learningOutcomes)
                {
                    var raSelection = FindLearningOutcomeSelection(moduleSelection, ra.id);
                    var raSummary = new ConfigSummary.LearningOutcomeSummary
                    {
                        id = ra.id,
                        title = ra.title,
                        selected = raSelection != null && raSelection.enabled,
                        activities = new List<ConfigSummary.ActivitySummary>()
                    };

                    foreach (var activity in ra.activities)
                    {
                        raSummary.activities.Add(new ConfigSummary.ActivitySummary
                        {
                            id = activity.id,
                            title = activity.title,
                            selectedForStudent = !isTeacher && raSelection != null && raSelection.enabled &&
                                (raSelection.selectedActivityIds == null || raSelection.selectedActivityIds.Count == 0 || raSelection.selectedActivityIds.Contains(activity.id))
                        });
                    }

                    moduleSummary.learningOutcomes.Add(raSummary);
                }

                summary.modules.Add(moduleSummary);
            }

            var json = JsonUtility.ToJson(summary, true);
            File.WriteAllText(Path.Combine(docsRoot, "ConfigSummary.json"), json);
        }

        static ModuleSelection FindModuleSelection(GenerationConfig config, string moduleCode)
        {
            if (config == null || config.selectedModules == null)
            {
                return null;
            }

            foreach (var module in config.selectedModules)
            {
                if (module.moduleCode == moduleCode)
                {
                    return module;
                }
            }
            return null;
        }

        static LearningOutcomeSelection FindLearningOutcomeSelection(ModuleSelection moduleSelection, string raId)
        {
            if (moduleSelection == null || moduleSelection.learningOutcomes == null)
            {
                return null;
            }

            foreach (var ra in moduleSelection.learningOutcomes)
            {
                if (ra.learningOutcomeId == raId)
                {
                    return ra;
                }
            }
            return null;
        }

        static CycleDefinition GetCycle(CurriculumCatalog catalog, string id)
        {
            if (catalog == null)
            {
                return null;
            }

            foreach (var cycle in catalog.cycles)
            {
                if (cycle.id == id)
                {
                    return cycle;
                }
            }
            return null;
        }

        static string Sanitize(string value)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }
            return value.Replace(' ', '_');
        }

        [Serializable]
        class ConfigSummary
        {
            public string configName;
            public string cycle;
            public string packageType;
            public List<ModuleSummary> modules;

            [Serializable]
            public class ModuleSummary
            {
                public string code;
                public string name;
                public bool selected;
                public List<LearningOutcomeSummary> learningOutcomes;
            }

            [Serializable]
            public class LearningOutcomeSummary
            {
                public string id;
                public string title;
                public bool selected;
                public List<ActivitySummary> activities;
            }

            [Serializable]
            public class ActivitySummary
            {
                public string id;
                public string title;
                public bool selectedForStudent;
            }
        }
    }
}

