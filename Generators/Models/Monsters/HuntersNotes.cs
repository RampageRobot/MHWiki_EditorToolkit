namespace MediawikiTranslator.Models.Monsters
{
	public class HuntersNotes(string name, Games game)
	{
		public string Description { get; set; } = string.Empty;
		public Games Game { get; set; } = game;
		public string Icon { get; set; } = $"{game.ToFriendlyString()}-{name} Icon.webp";
	}
}
