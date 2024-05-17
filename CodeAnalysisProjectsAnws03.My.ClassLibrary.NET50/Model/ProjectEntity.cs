using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysisProjectsAnws03.My.ClassLibrary.NET50.Model          
{
    public class ProjectEntity
    {
        public string ProjectName { get; set; }
        public string Framework { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Language { get; set; }
        public string UsesProjects { get; set; }
        public string UsedByProjects { get; set; }
    }
}
