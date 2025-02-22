using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    public class Monster(string name)
	{
		public required string Name { get; set; } = name;
		public Attacks[] Attacks { get; set; } = [];
		public Drops[] Drops { get; set; } = [];
		public Enrage Enrage { get; set; } = new(name);
		public Stamina Stamina { get; set; } = new(name);
		public StatusEffectiveness[] StatusEffectiveness { get; set; } = Monsters.StatusEffectiveness.GetStatuses(name);
		public TrapEffectiveness[] TrapEffectiveness { get; set; } = Monsters.TrapEffectiveness.GetTraps(name);
		public HitZoneValues[] HitZoneValues { get; set; } = [];
        public FlinchBreakThresholds[] FlinchBreakThresholds { get; set; } = Monsters.FlinchBreakThresholds.GetFlinchBreakThresholds(name);
	}
}
