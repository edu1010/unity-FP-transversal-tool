using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            WriteDocs(docsRoot, "README_Professor.md", config, cycle, true);
            WriteSummary(docsRoot, config, cycle, true);
        }

        static void GenerateStudent(GenerationConfig config, CycleDefinition cycle, string outputRoot)
        {
            var studentRoot = Path.Combine(outputRoot, "Alumnes");
            var unityProjectRoot = Path.Combine(studentRoot, "UnityProject");
            var docsRoot = Path.Combine(studentRoot, "Docs");
            var exercisesRoot = Path.Combine(studentRoot, "Exercicis");

            CopyUnityProject(unityProjectRoot);
            var teacherTemplates = Path.Combine(unityProjectRoot, "Assets", "TransversalTool", "Templates", "Teacher");
            if (Directory.Exists(teacherTemplates))
            {
                Directory.Delete(teacherTemplates, true);
            }
            ApplyStudentSelection(config, cycle, unityProjectRoot, exercisesRoot);
            WriteDocs(docsRoot, "README_Alumnes.md", config, cycle, false);
            WriteSummary(docsRoot, config, cycle, false);
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

        static void ApplyStudentSelection(GenerationConfig config, CycleDefinition cycle, string unityProjectRoot, string exercisesRoot)
        {
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
                        var activitySelected = raSelected && (selectedActivityIds == null || selectedActivityIds.Count == 0 || selectedActivityIds.Contains(activity.id));
                        if (activitySelected)
                        {
                            CopyActivityFiles(activity.studentTemplatePaths, activity.targetPaths, unityProjectRoot);
                            CreateExercise(activity, module, ra, exercisesRoot);
                        }
                        else
                        {
                            CopyActivityFiles(activity.teacherSolutionPaths, activity.targetPaths, unityProjectRoot);
                        }
                    }
                }
            }
        }

        static void CreateExercise(ActivityDefinition activity, ModuleDefinition module, LearningOutcomeDefinition ra, string exercisesRoot)
        {
            var moduleFolder = Sanitize(module.code + "_" + module.name);
            var raFolder = Sanitize(ra.id);
            var basePath = Path.Combine(exercisesRoot, moduleFolder, raFolder);

            var statementPath = Path.Combine(basePath, "Enunciat");
            var resourcesPath = Path.Combine(basePath, "Recursos");
            var templatePath = Path.Combine(basePath, "Plantilla");
            var deliverablePath = Path.Combine(basePath, "Entrega");

            Directory.CreateDirectory(statementPath);
            Directory.CreateDirectory(resourcesPath);
            Directory.CreateDirectory(templatePath);
            Directory.CreateDirectory(deliverablePath);

            CopyFiles(activity.statementPaths, statementPath);
            CopyFiles(activity.resourcePaths, resourcesPath);
            CopyFiles(activity.studentTemplatePaths, templatePath);

            var deliverableFile = Path.Combine(deliverablePath, "ENTREGA.txt");
            if (!File.Exists(deliverableFile))
            {
                File.WriteAllText(deliverableFile, "Deliverable type: " + activity.deliverableType);
            }
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
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(source, target, true);
            }
        }

        static void CopyFiles(List<string> sourcePaths, string destinationFolder)
        {
            if (sourcePaths == null)
            {
                return;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            foreach (var path in sourcePaths)
            {
                var source = ResolvePath(projectRoot, path);
                var destination = Path.Combine(destinationFolder, Path.GetFileName(source));
                Directory.CreateDirectory(destinationFolder);
                File.Copy(source, destination, true);
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

