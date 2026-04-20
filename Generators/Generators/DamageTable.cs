using MediawikiTranslator.Models.DamageTable;
using MediawikiTranslator.Models.DamageTable.PartsData;
using MediawikiTranslator.Models.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MediawikiTranslator.Generators
{
    public class DamageTable
    {
        private static readonly string Boilerplate = @"{{MHWIDamageData";
        private static readonly string Endcap = @"}}";

        public static void ParseFolder(string folder, string destPath)
        {
            foreach (DirectoryInfo dir in Directory.GetDirectories(folder).Select(x => new DirectoryInfo(x)).Where(x => x.Name.StartsWith("Em")))
            {
                DirectoryInfo activePath = new(Path.Combine(dir.FullName, "00/Data"));
                string partsPath = Path.Combine(activePath.FullName, dir.Name + "_00_Param_Parts.user.3.json");
                if (File.Exists(partsPath))
                {
                    SourceData data = SourceData.FromJson(Utilities.ReadAllText(partsPath));
                    string table = HitZoneValues.Format(ToHZV(data));
                    Directory.CreateDirectory(destPath);
                    File.WriteAllText(Path.Combine(destPath, Path.GetFileNameWithoutExtension(partsPath) + ".txt"), table);
                }
            }
        }

        private static string FindPartName(string partsType)
        {
            if (!string.IsNullOrEmpty(partsType))
            {
                PartsData partData = PartsData.FromJson(Utilities.ReadAllText("D:\\MH_Data Repo\\MH_Data\\Parsed Files\\MHWilds\\dtlnor rips\\MHWs-in-json-main\\natives\\STM\\GameDesign\\Common\\Enemy\\EnemyPartsTypeData.user.3.json"))[0];
				JArray partNames = JsonConvert.DeserializeObject<JObject>(Utilities.ReadAllText("D:\\MH_Data Repo\\MH_Data\\Parsed Files\\MHWilds\\dtlnor rips\\MHWs-in-json-main\\natives\\STM\\GameDesign\\Text\\Excel_Data\\EnemyPartsTypeName.msg.23.json"))!.Value<JArray>("entries")!;
                return partNames.First(x => x.Value<string>("guid") == partData.AppUserDataEnemyPartsTypeData?.Values.First(x => x.AppUserDataEnemyPartsTypeDataCData?.EmPartsType == partsType).AppUserDataEnemyPartsTypeDataCData?.EmPartsName.ToString()).Value<JArray>("content")![1].Value<string>()!;
            }
            else
            {
                return "";
            }
        }

        public static HitZoneValues[] ToHZV(SourceData srcData)
        {
            List<HitZoneValues> vals = [];
            if (srcData.AppUserDataEmParamParts?.MeatArray?.AceCInstanceGuidArray1AppUserDataEmParamPartsCMeat?.DataArray != null)
            {
                int cntr = 1;
                foreach (AceCInstanceGuidArray1AppUserDataEmParamPartsCMeatDataArray data in srcData.AppUserDataEmParamParts?.MeatArray?.AceCInstanceGuidArray1AppUserDataEmParamPartsCMeat?.DataArray ?? [])
                {
                    if (cntr > 1)
                    {
                    }
                    string partName = FindPartName(srcData.AppUserDataEmParamParts?.PartsArray?.AceCInstanceGuidArray1AppUserDataEmParamPartsCParts?.DataArray?.FirstOrDefault(x => x.AppUserDataEmParamPartsCParts?.MeatGuidNormal == data.AppUserDataEmParamPartsCMeat?.InstanceGuid)?.AppUserDataEmParamPartsCParts?.PartsType?.AppEnemyDefPartsTypeSerializable?.Value ?? "");
                    long? slash = data.AppUserDataEmParamPartsCMeat?.Slash!.Value!;
                    long? blow = data.AppUserDataEmParamPartsCMeat?.Blow!.Value!;
                    long? shot = data.AppUserDataEmParamPartsCMeat?.Shot!.Value!;
                    long? fire = data.AppUserDataEmParamPartsCMeat?.Fire!.Value!;
                    long? water = data.AppUserDataEmParamPartsCMeat?.Water!.Value!;
                    long? thunder = data.AppUserDataEmParamPartsCMeat?.Thunder!.Value!;
                    long? ice = data.AppUserDataEmParamPartsCMeat?.Ice!.Value!;
                    long? dragon = data.AppUserDataEmParamPartsCMeat?.Dragon!.Value!;
                    int slashTndr = Convert.ToInt32(Math.Floor((double)slash * .75d) + 25);
                    int blowTndr = Convert.ToInt32(Math.Floor((double)blow * .75d) + 25);
                    int shotTndr = Convert.ToInt32(Math.Floor((double)shot * .75d) + 25);
                    vals.Add(new HitZoneValues()
                    {
                        BluntEffect = (int)blow,
                        BluntTndr = blowTndr,
                        BulletEffect = (int)shot,
                        BulletTndr = shotTndr,
                        DragonEffect = (int)dragon,
                        FireEffect = (int)fire,
                        IceEffect = (int)ice,
                        Name = string.IsNullOrEmpty(partName) ? "???" : partName,
                        SeverEffect = (int)slash,
                        SeverTndr = slashTndr,
                        ThunderEffect = (int)thunder,
                        WaterEffect = (int)water
                    });
                }
            }
            return [.. vals];
        }

		public static async Task<string> Generate(SourceData srcData)
        {
            return await Task.Run(() =>
            {
                StringBuilder ret = new();
                ret.AppendLine(Boilerplate);
                if (srcData.AppUserDataEmParamParts?.MeatArray?.AceCInstanceGuidArray1AppUserDataEmParamPartsCMeat?.DataArray != null)
                {
                    int cntr = 1;
                    foreach (AceCInstanceGuidArray1AppUserDataEmParamPartsCMeatDataArray data in srcData.AppUserDataEmParamParts?.MeatArray?.AceCInstanceGuidArray1AppUserDataEmParamPartsCMeat?.DataArray ?? [])
                    {
                        if (cntr > 1)
                        {
                            ret.AppendLine();
                        }
                        string partName = FindPartName(srcData.AppUserDataEmParamParts?.PartsArray?.AceCInstanceGuidArray1AppUserDataEmParamPartsCParts?.DataArray?.FirstOrDefault(x => x.AppUserDataEmParamPartsCParts?.MeatGuidNormal == data.AppUserDataEmParamPartsCMeat?.InstanceGuid)?.AppUserDataEmParamPartsCParts?.PartsType?.AppEnemyDefPartsTypeSerializable?.Value ?? "");
                        long? slash = data.AppUserDataEmParamPartsCMeat?.Slash!.Value!;
						long? blow = data.AppUserDataEmParamPartsCMeat?.Blow!.Value!;
						long? shot = data.AppUserDataEmParamPartsCMeat?.Shot!.Value!;
						long? fire = data.AppUserDataEmParamPartsCMeat?.Fire!.Value!;
						long? water = data.AppUserDataEmParamPartsCMeat?.Water!.Value!;
						long? thunder = data.AppUserDataEmParamPartsCMeat?.Thunder!.Value!;
						long? ice = data.AppUserDataEmParamPartsCMeat?.Ice!.Value!;
						long? dragon = data.AppUserDataEmParamPartsCMeat?.Dragon!.Value!;
						int slashTndr = Convert.ToInt32(Math.Floor((double)slash * .75d) + 25);
                        int blowTndr = Convert.ToInt32(Math.Floor((double)blow * .75d) + 25);
                        int shotTndr = Convert.ToInt32(Math.Floor((double)shot * .75d) + 25);
                        ret.AppendLine($"|Part {cntr}             = {partName}");
                        ret.AppendLine($"|Part {cntr} sever       = {(slash >= 45 ? "'''" + slash + "'''" : slash)}");
                        ret.AppendLine($"|Part {cntr} sever tndrz = {(slashTndr >= 45 ? "'''" + slashTndr + "'''" : slashTndr)}");
                        ret.AppendLine($"|Part {cntr} blunt       = {(blow >= 45 ? "'''" + blow + "'''" : blow)}");
                        ret.AppendLine($"|Part {cntr} blunt tndrz = {(blowTndr >= 45 ? "'''" + blowTndr + "'''" : blowTndr)}");
                        ret.AppendLine($"|Part {cntr} shot        = {(shot >= 45 ? "'''" + shot + "'''" : shot)}");
                        ret.AppendLine($"|Part {cntr} shot tndrz  = {(shotTndr >= 45 ? "'''" + shotTndr + "'''" : shotTndr)}");
                        ret.AppendLine($"|Part {cntr} fire        = {fire}");
                        ret.AppendLine($"|Part {cntr} water       = {water}");
                        ret.AppendLine($"|Part {cntr} thunder     = {thunder}");
                        ret.AppendLine($"|Part {cntr} ice         = {ice}");
                        ret.AppendLine($"|Part {cntr} dragon      = {dragon}");
                        cntr++;
                    }
                }
                ret.AppendLine(Endcap);
                return ret.ToString();
            });
        }
    }
}
