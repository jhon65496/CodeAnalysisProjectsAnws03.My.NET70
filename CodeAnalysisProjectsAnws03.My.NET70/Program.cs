using CodeAnalysisProjectsAnws03.My.NET70.Model;
using CodeAnalysisProjectsAnws03.My.NET70.Services;
using dll = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET70;
using dllModel = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET70.Model;
using dllService = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET70.Services;
using dllModelNET6 = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET60.Model;
using dllServiceNET6 = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET60.Services;
using dllModelNET5 = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET50.Model;
using dllServiceNET5 = CodeAnalysisProjectsAnws03.My.ClassLibrary.NET50.Services;


namespace CodeAnalysisProjectsAnws03.My.NET70
{
    internal class Program
    {
        public static string[] paths = new string[]
        {
           @"e:\Projects\WPF\4587\01_pr\01\github.com\WPFCrudControl-master\fl\GenericCodes.WPF.sln",
           @"e:\Projects\WPF.Core\4071\01_pr\01\fl\Bookinist.sln"
        };


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // Test1(); 
              Test2();
            // Test3();
            // Test4();
            Console.ReadKey();
        }

        #region Current
        // ProjectAnalysisService5  === === 
        static void Test1()
        {
            string path = paths[0];

            List<ProjectEntity> table = ProjectAnalysisService.GetProjects(path);
            string tableToMarkdown = ProjectAnalysisService.ConvertToMarkdownTable(table);
            ProjectAnalysisService.WriteAllTextToMarkdownFile(tableToMarkdown);
        }
        #endregion


        #region CodeAnalysisProjectsAnws03.My.ClassLibrary.NET70
        // ProjectAnalysisService5  === === 
        static void Test2()
        {
            /*
             * * Could not load file or assembly 'Microsoft.Build, Version=15.1.0.0
             */

            string path = paths[1];
            

            List<dllModel.ProjectEntity> table = dllService.ProjectAnalysisService.GetProjects(path);
            string tableToMarkdown = dllService.ProjectAnalysisService.ConvertToMarkdownTable(table);
            dllService.ProjectAnalysisService.WriteAllTextToMarkdownFile(tableToMarkdown);
        }
        #endregion

        #region CodeAnalysisProjectsAnws03.My.ClassLibrary.NET60        
        /// <summary>
        /// +-*
        /// </summary>
        static void Test3()
        {
            /*
             * Could not load file or assembly 'Microsoft.Build, Version=15.1.0.0
             */
            string path = paths[0];            
            
            List<dllModelNET6.ProjectEntity> table = dllServiceNET6.ProjectAnalysisService.GetProjects(path);
            string tableToMarkdown = dllServiceNET6.ProjectAnalysisService.ConvertToMarkdownTable(table);
            dllService.ProjectAnalysisService.WriteAllTextToMarkdownFile(tableToMarkdown);
        }
        #endregion


        #region CodeAnalysisProjectsAnws03.My.ClassLibrary.NET50        
        /// <summary>
        /// +-*
        /// </summary>
        static void Test4()
        {
            /*
             * Could not load file or assembly 'Microsoft.Build, Version=15.1.0.0
             */

            string path = paths[0];            

            List<dllModelNET5.ProjectEntity> table = dllServiceNET5.ProjectAnalysisService.GetProjects(path);
            string tableToMarkdown = dllServiceNET5.ProjectAnalysisService.ConvertToMarkdownTable(table);
            dllService.ProjectAnalysisService.WriteAllTextToMarkdownFile(tableToMarkdown);
        }
        #endregion
    }
}