using DocumentFormat.OpenXml.Wordprocessing;
using MediawikiTranslator.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Weapon
{
    public class XlsxData
    {
        public string? Name { get; set; }
        public int? Rarity { get; set; }
        public string? TreeName { get; set; }
        public int? CosttoUpgradeTo { get; set; }
        public string? Material1Upgrade { get; set; }
        public int? Amount1Upgrade { get; set; }
        public string? Material2Upgrade { get; set; }
        public int? Amount2Upgrade { get; set; }
        public string? Material3Upgrade { get; set; }
        public int? Amount3Upgrade { get; set; }
        public string? Material4Upgrade { get; set; }
        public int? Amount4Upgrade { get; set; }
        public int? CosttoForge { get; set; }
        public string? Material1Forge { get; set; }
        public int? Amount1Forge { get; set; }
        public string? Material2Forge { get; set; }
        public int? Amount2Forge { get; set; }
        public string? Material3Forge { get; set; }
        public int? Amount3Forge { get; set; }
        public string? Material4Forge { get; set; }
        public int? Amount4Forge { get; set; }
        public string? Rollback { get; set; }
        public string? PreviousWeapon { get; set; }
        public string? NextWeapon1 { get; set; }
        public string? NextWeapon2 { get; set; }
        public string? NextWeapon3 { get; set; }
        public int? DisplayedRaw { get; set; }
        public int? Affinity { get; set; }
        public string? ElementType { get; set; }
        public int? ElementValue { get; set; }
        public string? Hidden { get; set; }
        public string? Elderseal { get; set; }
        public int? DefenseBonus { get; set; }
        public int? JewelSlot1 { get; set; }
        public int? JewelSlot2 { get; set; }
        public string? ArmorSkill { get; set; }
        public string? Description { get; set; }
        public string? ElementType1 { get; set; }
        public int? ElementValue1 { get; set; }
        public string? ElementType2 { get; set; }
        public int? ElementValue2 { get; set; }
        public string? Note1 { get; set; }
        public string? Note2 { get; set; }
        public string? Note3 { get; set; }
        public string? ShellingType { get; set; }
        public int? ShellingLevel { get; set; }
        public string? PhialType { get; set; }
        public int? ElementalPhialValue { get; set; }
        public string? KinsectBonus { get; set; }
        public string? Deviation { get; set; }
        public string? SpecialAmmo { get; set; }
        public string? PowerCoating { get; set; }
        public string? ParalysisCoating { get; set; }
        public string? PoisonCoating { get; set; }
        public string? SleepCoating { get; set; }
        public string? BlastCoating { get; set; }


        public WebToolkitData ToToolkitData(string game, string type)
        {
            WebToolkitData retData = new()
            {
                Game = game,
                Type = GetWeaponAbbreviation(type)
            };
            if (Name != null)
            {
                retData.Name = Name;
            }
            if (TreeName != null)
            {
                retData.Tree = TreeName;
            }
            if (Description != null)
            {
                retData.Description = Description;
            }
            if (DisplayedRaw != null)
            {
                retData.Attack = DisplayedRaw;
            }
            if (Rarity != null)
            {
                retData.Rarity = Rarity;
            }
            if (CosttoForge != null)
            {
                retData.ForgeCost = CosttoForge;
            }
            if (DefenseBonus != null)
            {
                retData.Defense = DefenseBonus;
            }
            if (Affinity != null)
            {
                retData.Affinity = Affinity;
            }
            if (ElementType != null)
            {
                retData.Element1 = ElementType;
            }
            if (ElementType1 != null)
            {
                retData.Element1 = ElementType1;
            }
            if (ElementType2 != null)
            {
                retData.Element2 = ElementType2;
            }
            if (ElementValue != null)
            {
                retData.ElementDmg1 = ElementValue;
            }
            if (ElementValue1 != null)
            {
                retData.ElementDmg1 = ElementValue1;
            }
            if (ElementValue2 != null)
            {
                retData.ElementDmg2 = ElementValue2;
            }
            if (Rollback != null && Rollback == "x")
            {
                retData.Rollback = "true";
            }
            if (JewelSlot1 != null)
            {
                switch (JewelSlot1)
                {
                    case 1:
                        retData.Decos1++;
                        break;
                    case 2:
                        retData.Decos2++;
                        break;
                    case 3:
                        retData.Decos3++;
                        break;
                    case 4:
                        retData.Decos4++;
                        break;
                }
            }
            if (JewelSlot2 != null)
            {
                switch (JewelSlot2)
                {
                    case 1:
                        retData.Decos1++;
                        break;
                    case 2:
                        retData.Decos2++;
                        break;
                    case 3:
                        retData.Decos3++;
                        break;
                    case 4:
                        retData.Decos4++;
                        break;
                }
            }
            if (Elderseal != null)
            {
                retData.Elderseal = Elderseal;
            }
            if (ArmorSkill != null)
            {
                retData.ArmorSkills = ArmorSkill;
            }
            if (Note1 != null)
            {
                retData.HhNote1 = Note1;
            }
            if (Note2 != null)
            {
                retData.HhNote2 = Note2;
            }
            if (Note3 != null)
            {
                retData.HhNote3 = Note3;
            }
            if (ShellingType != null)
            {
                retData.GlShellingType = ShellingType;
            }
            if (ShellingLevel != null)
            {
                retData.GlShellingLevel = ShellingLevel.ToString();
            }
            if (PhialType != null && retData.Type == "SA")
            {
                retData.SaPhialType = PhialType;
            }
            if (PhialType != null && retData.Type == "CB")
            {
                retData.CbPhialType = PhialType;
            }
            if (KinsectBonus != null)
            {
                retData.IgKinsectBonus = KinsectBonus;
            }
            if (PowerCoating != null && PowerCoating == "x")
            {
                if (!string.IsNullOrEmpty(retData.BoCoatings))
                {
                    retData.BoCoatings += ", ";
                }
                retData.BoCoatings += "Power";
            }
            if (ParalysisCoating != null && ParalysisCoating == "x")
            {
                if (!string.IsNullOrEmpty(retData.BoCoatings))
                {
                    retData.BoCoatings += ", ";
                }
                retData.BoCoatings += "Paralysis";
            }
            if (PoisonCoating != null && PoisonCoating == "x")
            {
                if (!string.IsNullOrEmpty(retData.BoCoatings))
                {
                    retData.BoCoatings += ", ";
                }
                retData.BoCoatings += "Poison";
            }
            if (SleepCoating != null && SleepCoating == "x")
            {
                if (!string.IsNullOrEmpty(retData.BoCoatings))
                {
                    retData.BoCoatings += ", ";
                }
                retData.BoCoatings += "Sleep";
            }
            if (BlastCoating != null && BlastCoating == "x")
            {
                if (!string.IsNullOrEmpty(retData.BoCoatings))
                {
                    retData.BoCoatings += ", ";
                }
                retData.BoCoatings += "Blast";
            }
            if (SpecialAmmo != null)
            {
                retData.HbgSpecialAmmoType = SpecialAmmo;
            }
            if (CosttoUpgradeTo != null)
            {
                retData.UpgradeCost = CosttoUpgradeTo;
            }
            List<MaterialJson> upgradeMaterials = [];
            if (Material1Upgrade != null && Amount1Upgrade != null)
            {
                upgradeMaterials.Add(new MaterialJson() { name = Material1Upgrade, quantity = Amount1Upgrade.Value });
            }
            if (Material2Upgrade != null && Amount2Upgrade != null)
            {
                upgradeMaterials.Add(new MaterialJson() { name = Material2Upgrade, quantity = Amount2Upgrade.Value });
            }
            if (Material3Upgrade != null && Amount3Upgrade != null)
            {
                upgradeMaterials.Add(new MaterialJson() { name = Material3Upgrade, quantity = Amount3Upgrade.Value });
            }
            if (Material4Upgrade != null && Amount4Upgrade != null)
            {
                upgradeMaterials.Add(new MaterialJson() { name = Material4Upgrade, quantity = Amount4Upgrade.Value });
            }
            if (upgradeMaterials.Count != 0)
            {
                retData.UpgradeMaterials = JsonConvert.SerializeObject(upgradeMaterials);
            }
            List<MaterialJson> forgeMaterials = [];
            if (Material1Forge != null && Amount1Forge != null)
            {
                forgeMaterials.Add(new MaterialJson() { name = Material1Forge, quantity = Amount1Forge.Value });
            }
            if (Material2Forge != null && Amount2Forge != null)
            {
                forgeMaterials.Add(new MaterialJson() { name = Material2Forge, quantity = Amount2Forge.Value });
            }
            if (Material3Forge != null && Amount3Forge != null)
            {
                forgeMaterials.Add(new MaterialJson() { name = Material3Forge, quantity = Amount3Forge.Value });
            }
            if (Material4Forge != null && Amount4Forge != null)
            {
                forgeMaterials.Add(new MaterialJson() { name = Material4Forge, quantity = Amount4Forge.Value });
            }
            if (forgeMaterials.Count != 0)
            {
                retData.ForgeMaterials = JsonConvert.SerializeObject(forgeMaterials);
            }
            if (NextWeapon1 != null)
            {
                retData.Next1Name = NextWeapon1;
            }
            if (NextWeapon2 != null)
            {
                retData.Next2Name = NextWeapon2;
            }
            if (NextWeapon3 != null)
            {
                retData.Next3Name = NextWeapon3;
            }
            return retData;
        }

        public static Dictionary<string, WebToolkitData> GetLinkedWeapons(Dictionary<string, WebToolkitData> source)
        {
            foreach (KeyValuePair<string, WebToolkitData> kvp in source)
            {
                if (!string.IsNullOrEmpty(kvp.Value.Next1Name))
                {
                    WebToolkitData? match = source.TryGetValue(kvp.Value.Next1Name, out WebToolkitData? value) ? value : null;
                    if (match != null)
                    {
                        kvp.Value.Next1Materials = match.UpgradeMaterials;
                        kvp.Value.Next1Cost = match.UpgradeCost;
                        kvp.Value.Next1Rarity = match.Rarity;
                    }
                }
                if (!string.IsNullOrEmpty(kvp.Value.Next2Name))
                {
                    WebToolkitData? match = source.TryGetValue(kvp.Value.Next2Name, out WebToolkitData? value) ? value : null;
                    if (match != null)
                    {
                        kvp.Value.Next2Materials = match.UpgradeMaterials;
                        kvp.Value.Next2Cost = match.UpgradeCost;
                        kvp.Value.Next2Rarity = match.Rarity;
                    }
                }
                if (!string.IsNullOrEmpty(kvp.Value.Next3Name))
                {
                    WebToolkitData? match = source.TryGetValue(kvp.Value.Next3Name, out WebToolkitData? value) ? value : null;
                    if (match != null)
                    {
                        kvp.Value.Next3Materials = match.UpgradeMaterials;
                        kvp.Value.Next3Cost = match.UpgradeCost;
                        kvp.Value.Next3Rarity = match.Rarity;
                    }
                }
            }
            return source;
        }

        private static string GetWeaponAbbreviation(string weaponType)
        {
            return weaponType == null ? "GS" : weaponType switch
            {
                "Great Sword" => "GS",
                "Long Sword" => "LS",
                "Sword and Shield" => "SnS",
                "Dual Blades" => "DB",
                "Hammer" => "Hm",
                "Hunting Horn" => "HH",
                "Lance" => "Ln",
                "Gunlance" => "GL",
                "Switch Axe" => "SA",
                "Charge Blade" => "CB",
                "Insect Glaive" => "IG",
                "Bow" => "Bo",
                "Light Bowgun" => "LBG",
                "Heavy Bowgun" => "HBG",
                _ => "GS",
            };
        }
    }

    class MaterialJson
    {
        public string name { get; set; } = string.Empty;
        public string icon { get; set; } = string.Empty;
        public string color { get; set; } = string.Empty;
        public int quantity { get; set; }
    }
}
