using ClosedXML.Excel;
using MediawikiTranslator.Models.Data.MHRS;
using Newtonsoft.Json;
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
		public static Items[] GetMHRSItems()
		{
			return JsonConvert.DeserializeObject<Items[]>(Properties.Resources.mhrs_items, Models.Data.MHWI.Converter.Settings)!;
		}

		public static Models.Data.MHWI.Items[] GetMHWIItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHWI.Items[]>(Encoding.UTF8.GetString(Properties.Resources.mhwi_items), Models.Data.MHWI.Converter.Settings)!;
		}

		public static DirectoryInfo GetWorkspace()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), @"\MHWikiToolkit_Generation\"));
        }
	}
}
