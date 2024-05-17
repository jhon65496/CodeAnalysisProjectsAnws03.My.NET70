using CodeAnalysisProjectsAnws03.My.ClassLibrary.NET60.Model;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//


namespace CodeAnalysisProjectsAnws03.My.ClassLibrary.NET60.Services
{
    public static class ProjectAnalysisService
    {

        public static List<ProjectEntity> GetProjects(string solutionPath)
        {
            // Используйте MSBuildLocator для регистрации версии MSBuild
            MSBuildLocator.RegisterDefaults();
            using var collection = new ProjectCollection();
            using var workspace = MSBuildWorkspace.Create();
            var solution = SolutionFile.Parse(solutionPath);

            var solutionProjects = solution.ProjectsInOrder
                                        .Select(project => workspace.OpenProjectAsync(project.AbsolutePath).Result)
                                        .Cast<Microsoft.Build.Evaluation.Project>() // Добавьте эту строку
                                        .ToList();

            var projects = new List<ProjectEntity>();

            foreach (var projectInSolution in solutionProjects)
            {
                var project = projectInSolution;

                var projectInfo = GetProjectInfo2(project, solutionProjects);

                projects.Add(projectInfo);
            }

            return projects;
        }

        public static ProjectEntity GetProjectInfo2(Microsoft.Build.Evaluation.Project project,
                                                    IEnumerable<Microsoft.Build.Evaluation.Project> solutionProjects)
        {
            var projectReferences = project.GetItems("ProjectReference")
                                            .Select(item => item.EvaluatedInclude);

            var solutionDirectory = Path.GetDirectoryName(project.FullPath);

            var referencedProjects = projectReferences
                                        .Select(path => Path.Combine(solutionDirectory, path))
                                        .Select(path => solutionProjects.FirstOrDefault(p => p.FullPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
                                        .Where(p => p != null)
                                        .Select(p => p.GetPropertyValue("AssemblyName"));

            var usedByProjects = solutionProjects.Where(p => p.GetItems("ProjectReference")
                                                            .Any(item => Path.GetFileNameWithoutExtension(item.EvaluatedInclude)
                                                                        == project.GetPropertyValue("AssemblyName")))
                                                    .Select(p => p.GetPropertyValue("AssemblyName"));

            var framework = project.GetPropertyValue("TargetFrameworkVersion") ??
                                    project.GetPropertyValue("TargetFrameworks");

            var languageVersion = GetLanguageVersion(project, framework);
            string formattedFramework = FormatFramework(framework);
            var projectType = project.GetPropertyValue("OutputType");

            string type1;
            switch (projectType)
            {
                case "Exe":
                    type1 = "Desktop";
                    break;

                case "Library":
                    type1 = "dll";
                    break;

                default:
                    type1 = projectType;
                    break;
            }

            var type2 = GetProjectType(project);

            var projectEntity = new ProjectEntity()
            {
                ProjectName = project.GetPropertyValue("AssemblyName"),
                Framework = formattedFramework,
                Type1 = type1,
                Type2 = type2,
                Language = languageVersion,
                UsesProjects = string.Join(", ", referencedProjects),
                UsedByProjects = string.Join(", ", usedByProjects)
            };

            return projectEntity;
        }

        public static ProjectEntity GetProjectInfo(Microsoft.Build.Evaluation.Project project,
                                               ProjectCollection collection,
                                               IEnumerable<Microsoft.Build.Evaluation.Project> solutionProjects)
        {
            var projectReferences = project.GetItems("ProjectReference")
                                            .Select(item => item.EvaluatedInclude);

            var solutionDirectory = Path.GetDirectoryName(project.FullPath);

            var referencedProjects = projectReferences
                                            .Select(path => Path.Combine(solutionDirectory, path))
                                            .Select(path => collection.LoadProject(path))
                                            .Select(p => p.GetPropertyValue("AssemblyName"));

            var usedByProjects = solutionProjects.Where(p => p.GetItems("ProjectReference")
                                                  .Any(item => Path.GetFileNameWithoutExtension(item.EvaluatedInclude)
                                                        == project.GetPropertyValue("AssemblyName")))
                                                  .Select(p => p.GetPropertyValue("AssemblyName"));

            var framework = project.GetPropertyValue("TargetFrameworkVersion") ??
                            project.GetPropertyValue("TargetFrameworks");


            // --- --- --- --- ---
            // var languageVersion = project.GetProperty("LanguageVersion")?.UnevaluatedValue;
            var languageVersion = GetLanguageVersion(project, framework);
            string formattedFramework = FormatFramework(framework);
            var projectType = project.GetPropertyValue("OutputType");

            string type1;
            switch (projectType) // Useful when comparing a single variable to many values.
            {
                case "Exe":
                    type1 = "Desktop";
                    break; // Exits the switch statement.

                case "Library":
                    type1 = "dll";
                    break; // Exits the switch statement.
                           // ... other cases ...
                default:
                    type1 = projectType;
                    break;
            }

            var type2 = GetProjectType(project);

            var projectEntity = new ProjectEntity()
            {
                ProjectName = project.GetPropertyValue("AssemblyName"),
                Framework = formattedFramework,
                Type1 = type1,
                Type2 = type2,
                Language = languageVersion,
                UsesProjects = string.Join(", ", referencedProjects),
                UsedByProjects = string.Join(", ", usedByProjects)
            };

            return projectEntity;
        }



        #region private
        private static string FormatFramework(string framework)
        {
            if (string.IsNullOrEmpty(framework))
                return string.Empty;

            if (framework.StartsWith("v"))
            {
                framework = framework.Substring(1);
            }

            var frameworkParts = framework.Split('.');

            if (frameworkParts.Length == 1)
            {
                return $".NET {frameworkParts[0]}";
            }

            if (frameworkParts[0] == "net")
            {
                return $".NET {string.Join(".", frameworkParts.Skip(1))}";
            }

            if (int.TryParse(frameworkParts[0], out int majorVersion))
            {
                if (majorVersion < 5)
                {
                    return $".NET Framework {string.Join(".", frameworkParts)}";
                }
                else
                {
                    return $".NET {string.Join(".", frameworkParts.Skip(1))}";
                }
            }

            return framework;
        }

        private static string GetProjectType(Microsoft.Build.Evaluation.Project project)
        {
            var references = project.GetItems("Reference");

            if (references.Any(r => r.EvaluatedInclude.Contains("PresentationFramework")))
            {
                return "WPF";
            }

            if (references.Any(r => r.EvaluatedInclude.Contains("System.Web.Mvc")))
            {
                return "ASP.NET MVC";
            }

            if (references.Any(r => r.EvaluatedInclude.Contains("System.Web.Http")))
            {
                return "ASP.NET Web API";
            }

            if (references.Any(r => r.EvaluatedInclude.Contains("System.Windows.Forms")))
            {
                return "WinForms";
            }

            return string.Empty;
        }

        private static string GetLanguageVersion(Microsoft.Build.Evaluation.Project project, string framework)
        {
            var languageVersion = project.GetPropertyValue("LangVersion");

            if (!string.IsNullOrEmpty(languageVersion))
            {
                return languageVersion;
            }

            if (string.IsNullOrEmpty(framework))
            {
                return string.Empty;
            }

            if (framework.StartsWith("net"))
            {
                var netCoreVersion = framework.Split('.')[1];
                switch (netCoreVersion)
                {
                    case "1":
                    case "2":
                        return "C# 7.3";
                    case "3":
                        return "C# 8.0";
                    case "5":
                        return "C# 9.0";
                    case "6":
                        return "C# 10.0";
                    default:
                        return string.Empty;
                }
            }

            var frameworkVersion = new Version(framework);
            if (frameworkVersion >= new Version("4.7.2"))
            {
                return "C# 7.3";
            }

            if (frameworkVersion >= new Version("4.6.1"))
            {
                return "C# 7.0";
            }

            if (frameworkVersion >= new Version("4.6"))
            {
                return "C# 6.0";
            }

            if (frameworkVersion >= new Version("4.5.2"))
            {
                return "C# 5.0";
            }

            return string.Empty;
        }
        #endregion

        public static string ConvertToMarkdownTable(List<ProjectEntity> table)
        {
            table = table.OrderBy(x => x.ProjectName).ToList();

            var headerNames = typeof(ProjectEntity).GetProperties().Select(property => property.Name);
            var header = "| " + string.Join(" | ", headerNames.Select(h => "**" + h + "**")) + " |";
            var separator = "| " + string.Join(" | ", Enumerable.Repeat("---", headerNames.Count())) + " |";
            var rows = table.Select(r => "| " + string.Join(" | ", r.GetType().GetProperties().Select(property => property.GetValue(r))) + " |");

            return $"{header}\n{separator}\n{string.Join("\n", rows)}";
        }

        public static void WriteAllTextToMarkdownFile(string tableForMarkdown)
        {
            File.WriteAllText("Projects-ProjectAnalysisService4.md", tableForMarkdown);
        }
    }
}
