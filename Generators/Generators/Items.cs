using DocumentFormat.OpenXml.Office2010.Excel;
using MediawikiTranslator.Models.Data.MHRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Generators
{
	public class Items
	{
		public static Dictionary<int, string> GetMHWIWikiColors()
		{
			return new()
			{
				{ 0, "White" },
				{ 1, "Red" },
				{ 2, "Green" },
				{ 3, "Blue" },
				{ 4, "Yellow" },
				{ 5, "Purple" },
				{ 6, "Light Blue" },
				{ 7, "Orange" },
				{ 8, "Pink" },
				{ 9, "Lemon" },
				{ 10, "Gray" },
				{ 11, "Brown" },
				{ 12, "Emerald" },
				{ 13, "Moss" },
				{ 14, "Rose" },
				{ 15, "Dark Blue" },
				{ 16, "Dark Purple" },
				{ 17, "NOT AVAILABLE" },
				{ 18, "NOT AVAILABLE" },
				{ 19, "Violet" },
				{ 20, "NOT AVAILABLE" },
				{ 21, "NOT AVAILABLE" },
				{ 22, "NOT AVAILABLE" },
				{ 23, "NOT AVAILABLE" },
				{ 24, "Tan" },
				{ 25, "Vermilion" },
				{ 26, "Light Green" }
			};
		}

		public static Dictionary<int, string> GetMHRSWikiColors()
		{
			return new()
			{
				{ 0, "NOT AVAILABLE" },
				{ 1, "White" },
				{ 2, "Gray" },
				{ 3, "Pink" },
				{ 4, "Yellow" },
				{ 5, "Orange" },
				{ 6, "Vermilion" },
				{ 7, "Red" },
				{ 8, "Green" },
				{ 9, "Purple" },
				{ 10, "Blue" },
				{ 11, "Dark Blue" },
				{ 12, "Light Blue" },
				{ 13, "Brown" },
				{ 14, "Dark Purple" },
				{ 51, "Pink" }
			};
		}
	}
}
