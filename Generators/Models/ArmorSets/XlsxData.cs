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
    }
}
