using MediawikiTranslator.Generators;

namespace MediawikiTranslator.Models.ArmorSets
{
    public class XlsxData
    {
        public string? Name { get; set; } = string.Empty;
        public string? BonusName { get; set; } = string.Empty;
        public string? BonusSkill1 { get; set; } = string.Empty;
        public int? PiecesRequired1 { get; set; }
        public string? BonusSkill2 { get; set; } = string.Empty;
        public int? PiecesRequired2 { get; set; }
        public List<ArmorSetPiece> Pieces { get; set; } = [];
        public WebToolkitData ToToolkitData(string game)
        {
            WebToolkitData retData = new()
            {
                Game = game
            };
            if (Name != null)
            {
                retData.SetName = Name;
            }
            if (BonusSkill1 != null)
            {
                retData.SetSkill1Name = BonusSkill1;
            }
            if (BonusSkill2 != null)
            {
                retData.SetSkill2Name = BonusSkill2;
            }
            List<Piece> retPieces = [];
            foreach (ArmorSetPiece piece in Pieces)
            {
                Piece retPiece = new()
                {
                    Name = piece.Name
                };
                switch (piece.PieceType)
                {
                    case ArmorSetPieceType.Head:
                        retPiece.IconType = "Helmet";
                        break;
                    case ArmorSetPieceType.Chest:
                        retPiece.IconType = "Chestplate";
                        break;
                    case ArmorSetPieceType.Arms:
                        retPiece.IconType = "Armguard";
                        break;
                    case ArmorSetPieceType.Waist:
                        retPiece.IconType = "Waist";
                        break;
                    case ArmorSetPieceType.Legs:
                        retPiece.IconType = "Leggings";
                        break;
                }
                if (piece.Rarity != null)
                {
                    retPiece.Rarity = piece.Rarity;
                }
                if (piece.Cost != null)
                {
                    retPiece.ForgingCost = piece.Cost;
                }
                if (piece.BaseDefense != null)
                {
                    retPiece.Defense = piece.BaseDefense;
                }
                if (piece.MaxDefense != null)
                {
                    retPiece.MaxDefense = piece.BaseDefense;
                }
                if (piece.FireRes != null)
                {
                    retPiece.FireRes = piece.FireRes;
                }
                if (piece.WaterRes != null)
                {
                    retPiece.WaterRes = piece.WaterRes;
                }
                if (piece.ThunderRes != null)
                {
                    retPiece.ThunderRes = piece.ThunderRes;
                }
                if (piece.IceRes != null)
                {
                    retPiece.IceRes = piece.IceRes;
                }
                if (piece.DragonRes != null)
                {
                    retPiece.DragonRes = piece.DragonRes;
                }
                if (!string.IsNullOrEmpty(piece.Description))
                {
                    retPiece.Description = piece.Description;
                }
                if (piece.Level1Slots != null)
                {
                    retPiece.Decos1 = piece.Level1Slots;
                }
                if (piece.Level2Slots != null)
                {
                    retPiece.Decos2 = piece.Level2Slots;
                }
                if (piece.Level3Slots != null)
                {
                    retPiece.Decos3 = piece.Level3Slots;
                }
                if (piece.Level4Slots != null)
                {
                    retPiece.Decos4 = piece.Level4Slots;
                }
                List<Material> mats = [];
                if (!string.IsNullOrEmpty(piece.Material1))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material1,
                        Quantity = piece.Material1Quantity != null ? piece.Material1Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material1IconType) ? piece.Material1IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material1IconColor) ? piece.Material1IconColor : "White",
                    });
                }
                if (!string.IsNullOrEmpty(piece.Material2))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material2,
                        Quantity = piece.Material2Quantity != null ? piece.Material2Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material2IconType) ? piece.Material2IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material2IconColor) ? piece.Material2IconColor : "White",
                    });
                }
                if (!string.IsNullOrEmpty(piece.Material3))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material3,
                        Quantity = piece.Material3Quantity != null ? piece.Material3Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material3IconType) ? piece.Material3IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material3IconColor) ? piece.Material3IconColor : "White",
                    });
                }
                if (!string.IsNullOrEmpty(piece.Material4))
                {
                    mats.Add(new()
                    {
                        Name = piece.Material4,
                        Quantity = piece.Material4Quantity != null ? piece.Material4Quantity : 1,
                        Icon = !string.IsNullOrEmpty(piece.Material4IconType) ? piece.Material4IconType : "Question Mark",
                        Color = !string.IsNullOrEmpty(piece.Material4IconColor) ? piece.Material4IconColor : "White",
                    });
                }
                retPiece.Materials = [.. mats];
                List<Skill> skills = [];
                if (!string.IsNullOrEmpty(piece.Skill1))
                {
                    skills.Add(new Skill()
                    {
                        Level = piece.Skill1Level,
                        Name = piece.Skill1
                    });
                }
                if (!string.IsNullOrEmpty(piece.Skill2))
                {
                    skills.Add(new Skill()
                    {
                        Level = piece.Skill2Level,
                        Name = piece.Skill2
                    });
                }
                retPiece.Skills = [.. skills];
                retPieces.Add(retPiece);
            }
            retData.Pieces = [.. retPieces];
            if (retData.Rarity == null && retData.Pieces.Any())
            {
                retData.Rarity = retData.Pieces[0].Rarity;
            }
            return retData;
        }
    }
}
