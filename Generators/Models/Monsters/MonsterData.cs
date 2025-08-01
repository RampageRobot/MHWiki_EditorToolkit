using MediawikiTranslator.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class MonsterData
    {
        public string Name { get; set; } = string.Empty;
        public MonsterNames? LangNames { get; set; }
        public List<GameAppearance> GameAppearances { get; set; } = [];
        public string Title { get; set; } = string.Empty;
		public string Classification { get; set; } = string.Empty;
        public string DebutGame { get; set; } = string.Empty;
    }

    public class GameAppearance
    {
        public required string GameAcronym { get; set; }
        public required string GameFull { get; set; }
        public string InternalID { get; set; } = string.Empty;
    }
}
