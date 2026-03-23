using System;
using System.Collections.Generic;

namespace TransversalTool
{
    [Serializable]
    public class CurriculumCatalog
    {
        public List<CycleDefinition> cycles = new List<CycleDefinition>();
    }

    [Serializable]
    public class CycleDefinition
    {
        public string id;
        public string displayName;
        public List<ModuleDefinition> modules = new List<ModuleDefinition>();
    }

    [Serializable]
    public class ModuleDefinition
    {
        public string code;
        public string name;
        public bool enabled;
        public List<LearningOutcomeDefinition> learningOutcomes = new List<LearningOutcomeDefinition>();
    }

    [Serializable]
    public class LearningOutcomeDefinition
    {
        public string id;
        public string title;
        public bool enabled;
        public List<ActivityDefinition> activities = new List<ActivityDefinition>();
    }

    [Serializable]
    public class ActivityDefinition
    {
        public string id;
        public string title;
        public string type;
        public List<string> studentTemplatePaths = new List<string>();
        public List<string> teacherSolutionPaths = new List<string>();
        public List<string> statementPaths = new List<string>();
        public List<string> resourcePaths = new List<string>();
        public List<string> targetPaths = new List<string>();
        public string deliverableType;
        public string moduleCode;
        public string learningOutcomeId;
    }

    [Serializable]
    public class GenerationConfig
    {
        public string configName;
        public string selectedCycle;
        public string outputRoot;
        public bool includeTeacherPackage = true;
        public bool includeStudentPackage = true;
        public List<ModuleSelection> selectedModules = new List<ModuleSelection>();
    }

    [Serializable]
    public class ModuleSelection
    {
        public string moduleCode;
        public bool enabled;
        public List<LearningOutcomeSelection> learningOutcomes = new List<LearningOutcomeSelection>();
    }

    [Serializable]
    public class LearningOutcomeSelection
    {
        public string learningOutcomeId;
        public bool enabled;
        public List<string> selectedActivityIds = new List<string>();
    }
}
