using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    class StatusEffectiveness
    {
        public StatusType Type { get; set; }
        public int Threshold { get; set; }
        public int Increase { get; set; }
        public int Duration { get; set; }
        public int Decay { get; set; }
        public int Damage { get; set; }
    }

    public enum StatusType
    {
        Poison,
        Paralysis,
        Sleep,
        Blast,
        Stun,
        Exhaust,
        Mount,
        Elderseal
    }
}
