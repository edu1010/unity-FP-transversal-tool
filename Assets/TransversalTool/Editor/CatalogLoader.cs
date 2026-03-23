using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TransversalTool
{
    public static class CatalogLoader
    {
        const string CatalogRelativePath = "TransversalTool/Data/Curriculum/unity_transversal_tool_curriculum_catalog.json";
        const string ActivitiesRelativePath = "TransversalTool/Data/Activities/activities.json";

        public static CurriculumCatalog LoadCatalog()
        {
            var catalogPath = Path.Combine(Application.dataPath, CatalogRelativePath);
            if (!File.Exists(catalogPath))
            {
                Debug.LogWarning("Curriculum catalog not found: " + catalogPath);
                return new CurriculumCatalog();
            }

            var json = File.ReadAllText(catalogPath);
            var root = MiniJson.Deserialize(json) as Dictionary<string, object>;
            var catalog = new CurriculumCatalog();
            if (root == null || !root.ContainsKey("cycles"))
            {
                return catalog;
            }

            var cycles = root["cycles"] as Dictionary<string, object>;
            if (cycles == null)
            {
                return catalog;
            }

            foreach (var cycleEntry in cycles)
            {
                var cycle = new CycleDefinition
                {
                    id = cycleEntry.Key,
                    displayName = GetString(cycleEntry.Value, "name")
                };

                var modulesList = GetList(cycleEntry.Value, "modules");
                var moduleIndex = 0;
                foreach (var moduleObj in modulesList)
                {
                    var moduleDict = moduleObj as Dictionary<string, object>;
                    if (moduleDict == null)
                    {
                        continue;
                    }

                    var module = new ModuleDefinition
                    {
                        code = GetString(moduleDict, "code"),
                        name = GetString(moduleDict, "name"),
                        enabled = false
                    };

                    var ras = moduleDict.ContainsKey("ras") ? moduleDict["ras"] as List<object> : null;
                    if (ras != null)
                    {
                        var raIndex = 1;
                        foreach (var ra in ras)
                        {
                            var raTitle = ra as string ?? string.Empty;
                            module.learningOutcomes.Add(new LearningOutcomeDefinition
                            {
                                id = "RA" + raIndex,
                                title = raTitle,
                                enabled = false
                            });
                            raIndex++;
                        }
                    }

                    cycle.modules.Add(module);
                    moduleIndex++;
                }

                catalog.cycles.Add(cycle);
            }

            AttachActivities(catalog);
            return catalog;
        }

        static void AttachActivities(CurriculumCatalog catalog)
        {
            var activitiesPath = Path.Combine(Application.dataPath, ActivitiesRelativePath);
            if (!File.Exists(activitiesPath))
            {
                return;
            }

            var json = File.ReadAllText(activitiesPath);
            var root = MiniJson.Deserialize(json) as Dictionary<string, object>;
            if (root == null || !root.ContainsKey("activities"))
            {
                return;
            }

            var activitiesList = root["activities"] as List<object>;
            if (activitiesList == null)
            {
                return;
            }

            foreach (var activityObj in activitiesList)
            {
                var dict = activityObj as Dictionary<string, object>;
                if (dict == null)
                {
                    continue;
                }

                var activity = new ActivityDefinition
                {
                    id = GetString(dict, "id"),
                    title = GetString(dict, "title"),
                    type = GetString(dict, "type"),
                    deliverableType = GetString(dict, "deliverableType"),
                    moduleCode = GetString(dict, "moduleCode"),
                    learningOutcomeId = GetString(dict, "learningOutcomeId"),
                    studentTemplatePaths = GetStringList(dict, "studentTemplatePaths"),
                    teacherSolutionPaths = GetStringList(dict, "teacherSolutionPaths"),
                    statementPaths = GetStringList(dict, "statementPaths"),
                    resourcePaths = GetStringList(dict, "resourcePaths"),
                    targetPaths = GetStringList(dict, "targetPaths")
                };

                var module = FindModule(catalog, activity.moduleCode);
                if (module == null)
                {
                    continue;
                }

                var ra = FindLearningOutcome(module, activity.learningOutcomeId);
                if (ra == null)
                {
                    continue;
                }

                ra.activities.Add(activity);
            }
        }

        static ModuleDefinition FindModule(CurriculumCatalog catalog, string code)
        {
            foreach (var cycle in catalog.cycles)
            {
                foreach (var module in cycle.modules)
                {
                    if (module.code == code)
                    {
                        return module;
                    }
                }
            }
            return null;
        }

        static LearningOutcomeDefinition FindLearningOutcome(ModuleDefinition module, string id)
        {
            foreach (var ra in module.learningOutcomes)
            {
                if (ra.id == id)
                {
                    return ra;
                }
            }
            return null;
        }

        static string GetString(object obj, string key)
        {
            var dict = obj as Dictionary<string, object>;
            if (dict == null || !dict.ContainsKey(key))
            {
                return string.Empty;
            }
            return dict[key] as string ?? string.Empty;
        }

        static List<object> GetList(object obj, string key)
        {
            var dict = obj as Dictionary<string, object>;
            if (dict == null || !dict.ContainsKey(key))
            {
                return new List<object>();
            }
            var list = dict[key] as List<object>;
            return list ?? new List<object>();
        }

        static List<string> GetStringList(Dictionary<string, object> dict, string key)
        {
            var result = new List<string>();
            if (!dict.ContainsKey(key))
            {
                return result;
            }

            var list = dict[key] as List<object>;
            if (list == null)
            {
                return result;
            }

            foreach (var item in list)
            {
                if (item is string value)
                {
                    result.Add(value);
                }
            }
            return result;
        }
    }
}
