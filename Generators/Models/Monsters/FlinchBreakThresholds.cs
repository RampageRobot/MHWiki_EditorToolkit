using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    class FlinchBreakThresholds
	{
		public string Rank { get; set; } = "Low/High";
		public string Name { get; set; } = string.Empty;
		public bool CanFlinch { get; set; } = false;
		public string TripConditions { get; set; } = string.Empty;
		public int TripDuration { get; set; }
		public string BreakConditions { get; set; } = string.Empty;
	}
}
