using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Monsters
{
	public class Drops
    {
        public string Category { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public List<Subcat> Tables { get; set; } = [];

    }

    public class Subcat
    {
        public string Name { get; set; } = string.Empty;
        public List<Tuple<Data.MHWI.Items, int>> ItemDrops { get; set; } = [];
    }
}
