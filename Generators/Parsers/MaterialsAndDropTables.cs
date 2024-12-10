using MediawikiTranslator.Models.MaterialsAndDropTables;

namespace MediawikiTranslator.Parsers
{
	internal class MaterialsAndDropTables
	{
		internal static WebToolkitData[] FromWebUI(string json)
		{
			return WebToolkitData.FromJson(json);
		}
	}
}
