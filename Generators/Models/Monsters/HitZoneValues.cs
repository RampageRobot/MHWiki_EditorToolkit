using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class HitZoneValues
    {
        public string Rank { get; set; } = "Low/High";
        public string Name { get; set; } = string.Empty;
        public KinsectEssence Essence { get; set; }
        public int SeverEffect { get; set; }
		public int BluntEffect { get; set; }
		public int BulletEffect { get; set; }
		public int SeverTndr { get; set; }
		public int BluntTndr { get; set; }
		public int BulletTndr { get; set; }
        public int FireEffect { get; set; }
        public int WaterEffect { get; set; }
        public int ThunderEffect { get; set; }
        public int IceEffect { get; set; }
        public int DragonEffect { get; set; }
        public bool CanStun { get; set; } = false;
	}

    public enum KinsectEssence
	{
		None,
		Red,
        White,
        Orange,
        Green
    }
}
