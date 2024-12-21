using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator
{
    public static class Utilities
    {
        public static DirectoryInfo GetWorkspace()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), @"\MHWikiToolkit_Generation\"));
        }
	}
}
