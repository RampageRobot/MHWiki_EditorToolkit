using MediawikiTranslator.Models.DamageTable;
using Newtonsoft.Json;
using System.Text;

namespace MediawikiTranslator.Generators
{
    public class DamageTable
    {
        private static readonly string Boilerplate = @"{{MHWIDamageData";
        private static readonly string Endcap = @"}}";

        public static void ParseZip(string zipFile, string destPath)
        {
            byte[] fileBytes = File.ReadAllBytes(zipFile);
            SourceData[] sources = Parsers.DamageTable.FromGameSource(fileBytes).Result;
            foreach (SourceData source in sources)
            {
                string table = Generate(source).Result;
                Directory.CreateDirectory(destPath);
                File.WriteAllText(Path.Combine(destPath, source.FileName + ".txt"), table);
            }
        }

		public static async Task<string> Generate(SourceData srcData)
        {
            return await Task.Run(() =>
            {
                StringBuilder ret = new();
                ret.AppendLine(Boilerplate);
                if (srcData.Struct?.MeatArray?.DataArray != null)
                {
                    int cntr = 1;
                    foreach (MeatArrayDataArray data in srcData.Struct.MeatArray.DataArray)
                    {
                        if (cntr > 1)
                        {
                            ret.AppendLine();
                        }
                        int slashTndr = Convert.ToInt32(Math.Floor(data.Slash * .75d) + 25);
                        int blowTndr = Convert.ToInt32(Math.Floor(data.Blow * .75d) + 25);
                        int shotTndr = Convert.ToInt32(Math.Floor(data.Shot * .75d) + 25);
                        ret.AppendLine($"|Part {cntr}             = {data.InstanceGuid}");
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
