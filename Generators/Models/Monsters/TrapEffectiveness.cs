using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    class TrapEffectiveness
    {
        public TrapType Type { get; set; }
        public int Duration { get; set; }
        public int DurationExhaust { get; set; }
        public int ToleranceReduction { get; set; }
        public int MinDuration { get; set; }
        public int Effectiveness { get; set; }
    }

    public enum TrapType
    {
        Pitfall,
        Shock,
        Vine,
        FlashPod,
        Meat,
        DungPod,
        SonicPod
    }
}
