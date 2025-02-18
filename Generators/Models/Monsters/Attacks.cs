using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
    class Attacks
    {
        public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Element { get; set; } = string.Empty;
		public int Status { get; set; }
        public int StatusType { get; set; }
        public bool Guardable { get; set; }
    }
}
