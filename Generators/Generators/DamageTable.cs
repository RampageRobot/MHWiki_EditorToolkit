using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.DamageTable;
using MediawikiTranslator.Models.DamageTable.PartsData;
using Newtonsoft.Json;
using System.Text;

namespace MediawikiTranslator.Generators
{
    public class DamageTable
    {
        private static readonly string Boilerplate = @"{{MHWIDamageData";
        private static readonly string Endcap = @"}}";

        public static void ParseFolder(string folder, string destPath)
        {
            foreach (DirectoryInfo dir in Directory.GetDirectories(folder).Select(x => new DirectoryInfo(x)).Where(x => x.Name.StartsWith("em")))
            {
                DirectoryInfo activePath = new(Path.Combine(dir.FullName, "00/data"));
                string partsPath = Path.Combine(activePath.FullName, dir.Name + "_00_param_parts.user.3.json");
                if (File.Exists(partsPath))
                {
                    SourceData data = SourceData.FromJson(File.ReadAllText(partsPath));
                    string table = Generate(data).Result;
                    Directory.CreateDirectory(destPath);
                    File.WriteAllText(Path.Combine(destPath, Path.GetFileNameWithoutExtension(partsPath) + ".txt"), table);
                }
            }
        }

        private static string FindPartName(string partsType)
        {
            if (!string.IsNullOrEmpty(partsType))
            {
                PartsData partData = PartsData.FromJson(Encoding.UTF8.GetString(Properties.Resources.MHWilds_PartTypeData_json))!;
                Dictionary<string, dynamic> partNames = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encoding.UTF8.GetString(Properties.Resources.MHWilds_PartNames_json))!;
                return partNames[partData.Struct!.Values.First(x => x.EmPartsType == partsType).EmPartsName.ToString()].content.ToObject<string[]>()[1];
            }
            else
            {
                return "";
            }
        }

		public static async Task<string> Generate(SourceData srcData)
        {
            return await Task.Run(() =>
            {
                StringBuilder ret = new();
                ret.AppendLine(Boilerplate);
                if (srcData.AppUserDataEmParamParts?.MeatArray?.DataArray != null)
                {
                    int cntr = 1;
                    foreach (MeatArrayDataArray data in srcData.AppUserDataEmParamParts?.MeatArray?.DataArray ?? [])
                    {
                        if (cntr > 1)
                        {
                            ret.AppendLine();
                        }
                        string partName = FindPartName(srcData.AppUserDataEmParamParts?.PartsArray?.DataArray?.FirstOrDefault(x => x.MeatGuidNormal == data.InstanceGuid)?.PartsType ?? "");
                        int slashTndr = Convert.ToInt32(Math.Floor(data.Slash * .75d) + 25);
                        int blowTndr = Convert.ToInt32(Math.Floor(data.Blow * .75d) + 25);
                        int shotTndr = Convert.ToInt32(Math.Floor(data.Shot * .75d) + 25);
                        ret.AppendLine($"|Part {cntr}             = {partName}");
                        ret.AppendLine($"|Part {cntr} sever       = {(data.Slash >= 45 ? "'''" + data.Slash + "'''" : data.Slash)}");
                        ret.AppendLine($"|Part {cntr} sever tndrz = {(slashTndr >= 45 ? "'''" + slashTndr + "'''" : slashTndr)}");
                        ret.AppendLine($"|Part {cntr} blunt       = {(data.Blow >= 45 ? "'''" + data.Blow + "'''" : data.Blow)}");
                        ret.AppendLine($"|Part {cntr} blunt tndrz = {(blowTndr >= 45 ? "'''" + blowTndr + "'''" : blowTndr)}");
                        ret.AppendLine($"|Part {cntr} shot        = {(data.Shot >= 45 ? "'''" + data.Shot + "'''" : data.Shot)}");
                        ret.AppendLine($"|Part {cntr} shot tndrz  = {(shotTndr >= 45 ? "'''" + shotTndr + "'''" : shotTndr)}");
                        ret.AppendLine($"|Part {cntr} fire        = {data.Fire}");
                        ret.AppendLine($"|Part {cntr} water       = {data.Water}");
                        ret.AppendLine($"|Part {cntr} thunder     = {data.Thunder}");
                        ret.AppendLine($"|Part {cntr} ice         = {data.Ice}");
                        ret.AppendLine($"|Part {cntr} dragon      = {data.Dragon}");
                        cntr++;
                    }
                }
                ret.AppendLine(Endcap);
                return ret.ToString();
            });
        }
    }
}
