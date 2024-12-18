using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Models.Weapon;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace MediawikiTranslator.Generators
{
    public class Weapon
    {
        private static async Task<string> Generate(WebToolkitData weapon)
        {

            return await Task.Run(() =>
            {
                StringBuilder ret = new();
                ret.AppendLine($@"{{{{GenericWeapon
|Game                    = {weapon.Game}
|Weapon Name             = {weapon.Name}
|Weapon Family Name      = {ReplaceRomanNumerals(weapon.Name)}
|Weapon Type             = {weapon.Type}
|Tree                    = {weapon.Tree}
|Rarity                  = {weapon.Rarity}
|Description             = {weapon.Description}
|Attack                  = {weapon.Attack + " (" + Convert.ToInt32(Math.Round(1.5f * Convert.ToInt32(weapon.Attack))) + ")"}
|Affinity                = {weapon.Affinity}
{(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "|Elemental Damage        =" + weapon.ElementDmg1 : "")}
{(!string.IsNullOrEmpty(weapon.Element1) && weapon.ElementDmg1 != null ? "|Elemental Damage Type   =" + weapon.Element1 : "")}
{(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "|Elemental Damage 2      =" + weapon.ElementDmg2 : "")}
{(!string.IsNullOrEmpty(weapon.Element2) && weapon.ElementDmg2 != null ? "|Elemental Damage Type 2 =" + weapon.Element2 : "")}
{(!string.IsNullOrEmpty(weapon.Sharpness) ? GetSharpnessTemplates(weapon.Sharpness) : "")}
{(!string.IsNullOrEmpty(weapon.HhNote1) ? $"|HH Note 1               = {weapon.HhNote1}" : "")}
{(!string.IsNullOrEmpty(weapon.HhNote1) ? $"|HH Note 2               = {weapon.HhNote2}" : "")}
{(!string.IsNullOrEmpty(weapon.HhNote1) ? $"|HH Note 3               = {weapon.HhNote3}" : "")}
{(!string.IsNullOrEmpty(weapon.GlShellingType) ? $"|GL Shelling Type        = {weapon.GlShellingType} Lv {weapon.GlShellingLevel}" : "")}
{(!string.IsNullOrEmpty(weapon.SaPhialType) ? $"|SA Phial Type           = {weapon.SaPhialType}" : "")}
{(!string.IsNullOrEmpty(weapon.CbPhialType) ? $"|CB Phial Type           = {weapon.CbPhialType}" : "")}
{(!string.IsNullOrEmpty(weapon.IgKinsectBonus) ? $"|IG Kinsect Bonus        = {weapon.IgKinsectBonus}" : "")}
{(!string.IsNullOrEmpty(weapon.BoCoatings) ? $"|Bo Coatings             = {weapon.BoCoatings}" : "")}
{(!string.IsNullOrEmpty(weapon.HbgSpecialAmmoType) ? $"|HBG Special Ammo Type   = {weapon.HbgSpecialAmmoType}" : "")}
{(!string.IsNullOrEmpty(weapon.Elderseal) ? $"|Elderseal               = {weapon.Elderseal}" : "")}
{(!string.IsNullOrEmpty(weapon.ArmorSkills) ? $"|Armor Skills            = {weapon.ArmorSkills}" : "")}
{(!string.IsNullOrEmpty(weapon.RampageSkillSlots) ? $"|Rampage Slots           = {weapon.RampageSkillSlots}" : "")}
{(!string.IsNullOrEmpty(weapon.RampageDecoration) ? $"|Rampage Decoration      = {weapon.RampageDecoration}" : "")}
{(!string.IsNullOrEmpty(weapon.Rollback) ? $"|Can Rollback            = {weapon.Rollback}" : "")}");
                if (!string.IsNullOrEmpty(weapon.PreviousName))
                {
                    ret.AppendLine($@"|Upgrade Cost            = {weapon.UpgradeCost}
|Upgrade Materials       = {GetMaterialsTemplates(weapon.UpgradeMaterials, weapon.Game)}
|Previous Name           = {weapon.PreviousName}
|Previous Type           = {weapon.Type}
|Previous Rarity         = {weapon.PreviousRarity}");
                }
                if (!string.IsNullOrEmpty(weapon.Next1Name))
                {
                    ret.AppendLine($@"|Next 1 Name             = {weapon.Next1Name}
|Next 1 Type             = {weapon.Type}
|Next 1 Rarity           = {weapon.Next1Rarity}
|Next 1 Cost             = {weapon.Next1Cost}
|Next 1 Materials        = {GetMaterialsTemplates(weapon.Next1Materials, weapon.Game)}");
                }
                if (!string.IsNullOrEmpty(weapon.Next2Name))
                {
                    ret.AppendLine($@"|Next 2 Name             = {weapon.Next2Name}
|Next 2 Type             = {weapon.Type}
|Next 2 Rarity           = {weapon.Next2Rarity}
|Next 2 Cost             = {weapon.Next2Cost}
|Next 2 Materials        = {GetMaterialsTemplates(weapon.Next2Materials, weapon.Game)}");
                }
                if (!string.IsNullOrEmpty(weapon.Next3Name))
                {
                    ret.AppendLine($@"|Next 3 Name             = {weapon.Next3Name}
|Next 3 Type             = {weapon.Type}
|Next 3 Rarity           = {weapon.Next3Rarity}
|Next 3 Cost             = {weapon.Next3Cost}
|Next 3 Materials        = {GetMaterialsTemplates(weapon.Next3Materials, weapon.Game)}");
                }
                ret.AppendLine(@"}}");
                return ret.ToString().Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
            });
        }

        public static string GenerateFromJson(string json)
        {
            return Generate(WebToolkitData.FromJson(json)).Result;
        }



        public static async Task<string> GenerateFromXlsx(string xlsxBase64, string game)
        {
            DirectoryInfo workspace = Utilities.GetWorkspace();
            string xlsxPath = Path.Combine(workspace.FullName, Guid.NewGuid().ToString() + ".xlsx");
            File.WriteAllBytes(xlsxPath, Convert.FromBase64String(xlsxBase64));
            Dictionary<string, Dictionary<string, XlsxData>> retData = [];
            IXLWorksheet[] sheets = [];
            using (XLWorkbook wb = new(xlsxPath))
            {
                sheets = [.. wb.Worksheets];
                foreach (IXLWorksheet sheet in sheets)
                {
                    Dictionary<string, XlsxData> weaponData = [];
                    Dictionary<string, int> headers = [];
                    int cntr = 1;
                    foreach (IXLRow row in sheet.Rows())
                    {
                        if (cntr == 1)
                        {
                            int cellCntr = 1;
                            string lastVal = "";
                            foreach (IXLCell cell in row.Cells())
                            {
                                string? val = !cell.Value.IsBlank ? cell.Value.GetText().Replace(" ", "").Replace("?", "") : null;
                                if (val != null && val != "Name")
                                {
                                    if (!string.IsNullOrEmpty(lastVal))
                                    {
                                        if (lastVal == "CosttoUpgradeTo" && (val.Contains("Amount") || val.Contains("Material")))
                                        {
                                            int amountCntr = 1;
                                            if (val == "Amount")
                                            {
                                                while (headers.ContainsKey(val + amountCntr + "Upgrade"))
                                                {
                                                    amountCntr++;
                                                }
                                                val += amountCntr;
                                            }
                                            val += "Upgrade";
                                        }
                                        else if (lastVal == "CosttoForge")
                                        {
                                            int amountCntr = 1;
                                            if (val == "Amount")
                                            {
                                                while (headers.ContainsKey(val + amountCntr + "Forge"))
                                                {
                                                    amountCntr++;
                                                }
                                                val += amountCntr;
                                            }
                                            val += "Forge";
                                        }
                                    }
                                    headers.Add(val, cellCntr);
                                    if (!val.Contains("Amount") && !val.Contains("Material"))
                                    {
                                        lastVal = val;
                                    }
                                }
                                cellCntr++;
                            }
                        }
                        else
                        {
                            string? name = !row.Cell(1).Value.IsBlank ? row.Cell(1).Value.GetText() : null;
                            if (!string.IsNullOrEmpty(name))
                            {
                                XlsxData data = new()
                                {
                                    Name = name
                                };
                                foreach (PropertyInfo pi in data.GetType().GetProperties())
                                {
                                    if (headers.TryGetValue(pi.Name, out int value))
                                    {
                                        XLCellValue val = row.Cell(value).Value; 
                                        Type propertyType = pi.PropertyType;
                                        if (propertyType.IsGenericType &&
                                            propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            propertyType = propertyType.GetGenericArguments()[0];
                                        }
                                        try
                                        {
                                            if (propertyType == typeof(string))
                                            {
                                                pi.SetValue(data, !val.IsBlank ? val.GetText() : null);
                                            }
                                            else if (propertyType == typeof(int))
                                            {
                                                if (val.IsNumber)
                                                {
                                                    pi.SetValue(data, Convert.ToInt32(val.GetNumber()));
                                                }
                                                else
                                                {
                                                    string? cellVal = !val.IsBlank ? val.GetText() : null;
                                                    if (cellVal != null && int.TryParse(cellVal, out int cellIntVal))
                                                    {
                                                        pi.SetValue(data, cellIntVal);
                                                    }
                                                    else
                                                    {
                                                        pi.SetValue(data, null);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception exc)
                                        {
                                            Debugger.Break();
                                        }
                                    }
                                }
                                weaponData.Add(name, data);
                            }
                        }
                        cntr++;
                    }
                    retData.Add(sheet.Name, weaponData);
                }
            }
            DirectoryInfo zipDirInfo = Utilities.GetWorkspace();
            string zipDir = Path.Combine(zipDirInfo.FullName, Guid.NewGuid().ToString());
            Directory.CreateDirectory(zipDir);
            foreach (KeyValuePair<string, Dictionary<string, XlsxData>> kvp in retData)
            {
                Dictionary<string, WebToolkitData> thisDict = kvp.Value.ToDictionary(x => x.Key, x => x.Value.ToToolkitData(game, kvp.Key));
                thisDict = XlsxData.GetLinkedWeapons(thisDict);
                DirectoryInfo rankDir = Directory.CreateDirectory(Path.Combine(zipDir, kvp.Key));
                foreach (KeyValuePair<string, WebToolkitData> val in thisDict)
                {
                    string txtPath = Path.Combine(rankDir.FullName, val.Key.Replace("\"", "'") + ".txt");
                    File.WriteAllText(txtPath, await Generate(val.Value));
                }
            }
            DirectoryInfo zipPathInfo = Utilities.GetWorkspace();
            string zipPath = Path.Combine(zipPathInfo.FullName, Guid.NewGuid() + ".zip");
            ZipFile.CreateFromDirectory(zipDir, zipPath);
            string zipBytes = Convert.ToBase64String(File.ReadAllBytes(zipPath));
            File.Delete(zipPath);
            Directory.Delete(workspace.FullName, true);
            return zipBytes;
        }

        private static string ReplaceRomanNumerals(string? input)
        {
            if (input == null)
            {
                return "";
            }
            foreach (string numeral in new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" })
            {
                if (input.EndsWith(" " + numeral))
                {
                    input = input[..input.IndexOf(" " + numeral)];
                }
            }
            return input;
        }

        private static string GetSharpnessTemplates(string? input)
        {
            if (input == null)
            {
                return "";
            }
            string[][] data = JsonConvert.DeserializeObject<string[][]>(input!)!;
            string ret = $"|Sharpness               = {{{{MHWISharpnessBase|{data[0][0]}|{data[0][1]}|{data[0][2]}|{data[0][3]}|{data[0][4]}|{data[0][5]}|{data[0][6]}}}}}";
            if (data.Length > 1)
            {
                ret += $"\r\n|Sharpness Handi+        = {{{{MHWISharpnessBase|{data[1][0]}|{data[1][1]}|{data[1][2]}|{data[1][3]}|{data[1][4]}|{data[1][5]}|{data[1][6]}}}}}";
            }
            return ret;
        }

        private static string GetMaterialsTemplates(string? input, string? game)
        {
            if (input == null)
            {
                return "";
            }
            StringBuilder ret = new();
            Dictionary<string, string>[] data = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(input!)!;
            int cntr = 1;
            foreach (Dictionary<string, string> dataObj in data)
            {
                string prefix = "";
                string suffix = "";
                if (cntr > 1)
                {
                    prefix = "-->";
                }
                if (cntr < data.Length)
                {
                    suffix = "<br><!--";
                }
                ret.AppendLine($@"{prefix}{{{{{game}ItemLink|{dataObj["name"]}|{dataObj["icon"]}|{dataObj["color"]}}}}} x{dataObj["quantity"]}{suffix}");
                cntr++;
            }
            return ret.ToString();
        }
    }
}
