using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediawikiTranslator.Models.Data.MHWI
{
    public class Titles
    {
        public int Index { get; set; }
        public string? Name { get; set; }
		public string? UnlockType { get; set; }
		public string? UnlockParam { get; set; }
		public int? MonsterId { get; set; }
		public string? MonsterName { get; set; }
		public string? Description { get; set; }
		public int Id { get; set; }
		public int Unk { get; set; }
		public TitleType TitleType { get; set; }

		public static Titles[] FetchAllTitles()
		{
			List<Titles> nouns = [..JsonConvert.DeserializeObject<List<Titles>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\titles_1.json"))!
				.Where(x => x.UnlockType != "Unused").Select(x => new Titles()
				{
					Description = x.Description,
					Id = x.Id,
					Index = x.Index,
					MonsterId = x.UnlockType == "Hunt_XXX" ? x.MonsterId : null,
					MonsterName = x.UnlockType == "Hunt_XXX" ? x.MonsterName!.Trim() : null,
					Name = x.Name,
					TitleType = TitleType.Noun,
					Unk = x.Unk,
					UnlockParam = x.UnlockParam,
					UnlockType = x.UnlockType,
				})];
			nouns.AddRange(JsonConvert.DeserializeObject<List<Titles>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\titles_2.json"))!.Where(x => x.UnlockType != "Unused").Select(x => new Titles()
			{
				Description = x.Description,
				Id = x.Id,
				Index = x.Index,
				MonsterId = x.UnlockType == "Hunt_XXX" ? x.MonsterId : null,
				MonsterName = x.UnlockType == "Hunt_XXX" ? x.MonsterName!.Trim() : null,
				Name = x.Name,
				TitleType = TitleType.Adj,
				Unk = x.Unk,
				UnlockParam = x.UnlockParam,
				UnlockType = x.UnlockType,
			}));
			return [.. nouns.Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Description))];
		}
	}
	
	public enum TitleType
	{
		Noun,
		Adj
	}
}
