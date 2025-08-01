using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Google.Apis.Translate.v2.Data;
using MediawikiTranslator.Generators;
using MediawikiTranslator.Models;
using MediawikiTranslator.Models.Data;
using MediawikiTranslator.Models.Data.MHWI;
using MediawikiTranslator.Models.Monsters;
using MediawikiTranslator.Models.Weapon;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Pages.Parsing;
using WikiClientLibrary.Pages.Queries.Properties;
using WikiClientLibrary.Sites;

namespace MediawikiTranslator
{
	public static class Utilities
	{
		private static async Task _Template()
		{
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					List<WikiPage> _temp = [];
					int totalCount = _temp.Count;
					int cnt = 1;
					foreach (WikiPage page in _temp)
					{
						Console.WriteLine($"({cnt}/{totalCount}) {page.Title} edited.");
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}
		private static Dictionary<string, MonsterNames> MonsterNameDict = FetchMonsterNames();
		private static async Task Login(this WikiSite site)
		{
			bool success = false;
			while (!success)
			{
				string usr = System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!;
				string pswd = System.Configuration.ConfigurationManager.AppSettings.Get("WikiPassword")!;
				if (string.IsNullOrEmpty(usr))
				{
					Console.WriteLine("Please enter your username.");
					usr = Console.ReadLine()!;
					Console.WriteLine("Please enter your password.");
					pswd = Console.ReadLine()!;
				}
				try
				{
					await site.LoginAsync(usr, pswd);
					success = true;
				}
				catch (Exception)
				{
					Console.WriteLine("Login failed. Please enter your credentials again.");
					success = false;
				}
				System.Configuration.ConfigurationManager.AppSettings.Set("WikiUsername", usr);
				System.Configuration.ConfigurationManager.AppSettings.Set("WikiPassword", pswd);
			}
		}
		private static string[] MainlineRelease = ["MH1", "MHG", "MHF1", "MH2", "MHFrontier", "MHF2", "MHFU", "MH3", "MHP3", "MH3U", "MH4", "MHExplore", "MH4U", "MHGen", "MHST1", "MHOnline", "MHGU", "MHWorld", "MHWI", "MHRiders", "MHRise", "MHST2", "MHRS", "MHNow", "MHWilds"];
		private static string[] MainSeriesAcros = [.. new string[] { "MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1" }.Reverse()];
		private static string[] SpinoffAcros = ["MHNow", "MHOutlanders", "MHPuzzles", "MHST1", "MHST2", "MHFrontier", "MHOnline", "MHExplore", "MHi", "MHRiders", "MHDiary", "MHDG", "MHDDX", "MHBGHQ", "MHDH", "MHMH", "MHPIV", "MHSpirits", "MHGii", "MHP1", "MHP2", "MHP2G", "MH3G", "MH4G", "MHX", "MHXX"];
		private static string[] AllGameAcros = ["MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1", "MHNow", "MHOutlanders", "MHPuzzles", "MHST1", "MHST2", "MHFrontier", "MHOnline", "MHExplore", "MHi", "MHRiders", "MHDiary", "MHDG", "MHDDX", "MHBGHQ", "MHDH", "MHMH", "MHPIV", "MHSpirits", "MHGii", "MHP1", "MHP2", "MHP2G", "MH3G", "MH4G", "MHX", "MHXX"];
		private static string[] ElementTypes = ["Fire", "Water", "Thunder", "Ice", "Dragon"];
		private static string[] StatusEffects = ["Fireblight", "Waterblight", "Thunderblight", "Iceblight", "Dragonblight", "Exhaust", "Fatigued", "Stun", "Paralysis", "Sleep", "Poison", "Noxious Poison", "Deadly Poison", "Blast", "Blastblight", "Blastscourge", "Snowman", "Soiled", "Affinity", "Affinity Up", "Attack Up", "Attack Down", "Defense", "Defense Up", "Defense Down", "Defense Down L", "Element Up", "Resistance Down", "Drain", "Muddy", "Webbed", "Frenzy Virus", "Bleeding", "Confusion", "Bubbleblight", "Severe Bubbleblight", "Tarred", "Mucus", "Ossified", "Effluvium"];
		private readonly static string[] SmallMonsterNames = ["Altaroth", "Anteka", "Apceros", "Aptonoth", "Aptonoth EX", "Apypos", "Baggi", "Barnos", "Baunos", "Blango", "Bnahabra", "Boaboa", "Boggi", "Bombadgy", "Bulaqchi", "Bullfango", "Burukku", "Canyne", "Ceanataur", "Cephalos", "Ceratonoth", "Comaqchi", "Conga", "Cortos", "Dalthydon", "Delex", "Egyurasu", "Epioth", "Erupe", "Felyne", "Fish", "Gajalaka", "Gajau", "Gajios", "Gargwa", "Gastodon", "Gelidron", "Genprey", "Giaprey", "Giggi", "Girros", "Goocoo", "Gowngoat", "Great Dracophage Bug", "Great Poogie", "Great Thunderbug", "Grimalkyne", "Guardian Seikret", "Halk", "Harpios", "Hermitaur", "Hornetaur", "Ioprey", "Izuchi", "Jaggi", "Jaggia", "Jagras", "Kelbi", "Kestodon", "Konchu", "Kranodath", "Kumashira", "Kusubami", "Larinoth", "Ludroth", "Maccao", "Melynx", "Mernos", "Moofah", "Moofy", "Mosswine", "Nerscylla Hatchling", "Noios", "Piragill", "Pokara", "Poogie", "Popo", "Porkeplume", "Pyrantula", "Qurio", "Rachnoid", "Rafma", "Raphinos", "Remobra", "Rhenoplos", "Seikret", "Shakalaka", "Shamos", "Slagtoth", "Talioth", "Thanksalot Bnahabra", "Uroktor", "Uroktor EX", "Uruki", "Velociprey", "Vespoid", "Wroggi", "Wudwud", "Wulg", "Zamite"];
		private readonly static string[] MonsterNameAdjs = ["Afflicted", "Barrel", "Blood Orange", "Boltreaver", "Copper", "Emerald", "Explosive Peak", "Flaming", "Frenzied", "Frostfang", "Frozen", "Fulgur", "Glacial", "Goldbeard", "HC", "Hellblade", "Jade", "Lolo", "Magma", "Nightcloak", "Oroshi", "Pale", "Pearl", "Plum", "Primordial", "Purple", "Raging", "Ravenous", "Ray", "Razing", "Red", "Redhelm", "Risen", "Ruby", "Ruiner", "Rust", "Rusted", "Sand", "Savage", "Scorching Heat", "Scorned", "Seething", "Shah", "Shrieking", "Shrouded", "Silver", "Silverwind", "Snowbaron", "Soulseer", "Terra", "Thunder Emperor", "Tidal", "Violent", "Violet", "Viper", "Virulent", "Vivid", "White", "Zenith", "helm", "wind", "Hatchling", "(Black Flying Wyvern)", "Blue", "Brown", "Green", "Yellow", "Queen"];
		public static Dictionary<string, dynamic> MHWIQuestInfo = [];
		public static Dictionary<string, string> Translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\Translations\translations.json"))!;

		public static Dictionary<string, dynamic> GetMHWIQuestInfo()
		{
			if (MHWIQuestInfo.Count == 0)
			{
				MHWIQuestInfo = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText($@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\test monster stuff\MHWI\questInfo.json"))!;
			}
			return MHWIQuestInfo;
		}

		public static Models.Data.MHRS.Items[] GetMHRSItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHRS.Items[]>(Properties.Resources.mhrs_items, Models.Data.MHWI.Converter.Settings)!;
		}

		public static Models.Data.MHWI.Items[] GetMHWIItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHWI.Items[]>(Encoding.UTF8.GetString(Properties.Resources.mhwi_items), Models.Data.MHWI.Converter.Settings)!;
		}

		public static async Task<Dictionary<string, string>> GetMonsterRenders(MonsterData monsterData, WikiSite site, Dictionary<string, string[]>? gamePatterns = null)
		{
			gamePatterns ??= [];
			string patternFile = @"D:\MH_Data Repo\MH_Data\Parsed Files\renderPatterns.json";
			if (File.Exists(patternFile))
			{
				gamePatterns = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(patternFile))!;
			}
			Dictionary<string, string> gameRenders = [];
			Console.WriteLine($"Building render list for monster {monsterData.Name}.");
			foreach (GameAppearance appearance in monsterData.GameAppearances.Where(x => !string.IsNullOrEmpty(x.GameAcronym)))
			{
				Console.WriteLine($"Grabbing renders for {appearance.GameAcronym}.");
				if (gamePatterns.ContainsKey(appearance.GameAcronym))
				{
					foreach (string pattern in gamePatterns[appearance.GameAcronym].Select(x => x.Replace("<REPLACER>MonsterName</REPLACER>", monsterData.Name)))
					{
						if (!gameRenders.ContainsKey(appearance.GameAcronym))
						{
							WikiPage renderFile = new(site, pattern);
							await renderFile.RefreshAsync();
							if (renderFile.Exists)
							{
								gameRenders.Add(appearance.GameAcronym, pattern);
							}
						}
					}
				}
			}
			Console.WriteLine($"Render list built for monster {monsterData.Name}.");
			return gameRenders;
		}

		public static async Task MonsterRenderGenerator(MonsterRenderGeneratorOptions? options = null)
		{
			if (options == null)
			{
				options = new();
			}
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					Dictionary<string, MonsterData> srcDict = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					Dictionary<string, MonsterData> monsterAppearances = srcDict
						.Where(x => (options.WhitelistMonsters != null ? options.WhitelistMonsters.Contains(x.Key) : true) && (options.BlacklistMonsters != null ? options.BlacklistMonsters.Contains(x.Key) : true))
						.ToDictionary(x => x.Key, x => new MonsterData()
						{
							Classification = x.Value.Classification,
							DebutGame = x.Value.DebutGame,
							LangNames = x.Value.LangNames,
							Name = x.Value.Name,
							Title = x.Value.Title,
							GameAppearances = [.. x.Value.GameAppearances.Where(x => (options.WhitelistGameAcronyms != null ? options.WhitelistGameAcronyms.Contains(x.GameAcronym) : true) && (options.BlacklistGameAcronyms != null ? !options.BlacklistGameAcronyms.Contains(x.GameAcronym) : true))]
						})
						.Where(x => x.Value.GameAppearances.Any())
						.ToDictionary(x => x.Key, x => x.Value);
					if (options.WhitelistGameAcronyms != null)
					{
						Console.WriteLine("Included games on whitelist.");
					}
					if (options.BlacklistGameAcronyms != null)
					{
						Console.WriteLine("Excluded games on blacklist.");
					}
					Dictionary<string, string[]> gamePatterns = [];
					string patternFile = @"D:\MH_Data Repo\MH_Data\Parsed Files\renderPatterns.json";
					if (File.Exists(patternFile))
					{
						gamePatterns = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(patternFile))!;
					}
					List<string> finishedOverviews = [];
					List<string> finishedGames = [];
					string overviewsFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\renders_overviews.json";
					string gamesFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\renders_games.json";
					if (!options.RefreshProgressLists)
					{
						if (File.Exists(overviewsFile))
						{
							finishedOverviews = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(overviewsFile))!;
						}
						if (File.Exists(gamesFile))
						{
							finishedGames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(gamesFile))!;
						}
					}
					else
					{
						Console.WriteLine("Refreshed progress lists.");
					}
					if (options.BuildPatternDatabase)
					{
						Console.WriteLine("Building pattern database.");
						AllCategoriesGenerator gen = new(site);
						WikiPage[] cats = await gen.EnumPagesAsync().Where(x => x.Title!.Contains("Render") && x.Title!.Contains("Monster")).ToArrayAsync();
						List<string> allMonsterNames = [];
						allMonsterNames.AddRange(monsterAppearances.Keys);
						allMonsterNames.AddRange(SmallMonsterNames);
						foreach (string game in AllGameAcros)
						{
							Console.WriteLine($"Building game {game}.");
							List<string> patterns = [];
							foreach (WikiPage category in cats.Where(x => Regex.IsMatch(x.Title!, $@"\b{game}\b", RegexOptions.IgnoreCase)))
							{
								CategoryMembersGenerator generator = new(site, category.Title!)
								{
									MemberTypes = CategoryMemberTypes.File
								};
								WikiPage[] renders = await generator.EnumPagesAsync().ToArrayAsync();
								foreach (string renderTitle in renders.Select(x => x.Title!))
								{
									string replace = renderTitle;
									foreach (string monster in allMonsterNames)
									{
										replace = Regex.Replace(replace, $@"\b{monster}\b", "<REPLACER>MonsterName</REPLACER>", RegexOptions.IgnoreCase);
									}
									foreach (string adj in MonsterNameAdjs)
									{
										replace = replace.Replace(" " + adj, "", StringComparison.InvariantCultureIgnoreCase).Replace(adj + " ", "", StringComparison.InvariantCultureIgnoreCase).Replace(adj, "", StringComparison.InvariantCultureIgnoreCase);
									}
									patterns.Add(replace);
								}
							}
							gamePatterns.Add(game, [.. patterns.Distinct()]);
							Console.WriteLine($"{game} built.");
						}
						File.WriteAllText(patternFile, JsonConvert.SerializeObject(gamePatterns));
						Console.WriteLine("Pattern database built.");
					}
					Dictionary<string, List<string>> missingRenders = [];
					int cnt = 1;
					int totalCnt = monsterAppearances.Count;
					string[] migratedGames = ["MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1", "MHNow", "MHPuzzles", "MHST1", "MHST2", "MHFrontier", "MHi", "MHRiders"];
					foreach (KeyValuePair<string, MonsterData> monsterData in monsterAppearances.Where(x => x.Key != "Ahtal-Neset" && !finishedOverviews.Contains(x.Key) && x.Value.GameAppearances.Any(y => migratedGames.Contains(y.GameAcronym))))
					{
						Console.WriteLine($"Building render list for monster {monsterData.Key}.");
						Dictionary<string, string> gameRenders = await GetMonsterRenders(srcDict[monsterData.Key], site, gamePatterns);
						foreach (GameAppearance appearance in monsterData.Value.GameAppearances.Where(x => !string.IsNullOrEmpty(x.GameAcronym)))
						{
							if (options.GenerateMissingRendersReport)
							{
								if (!gameRenders.ContainsKey(appearance.GameAcronym))
								{
									if (missingRenders.ContainsKey(appearance.GameAcronym))
									{
										missingRenders[appearance.GameAcronym].Add(monsterData.Key);
									}
									else
									{
										missingRenders.Add(appearance.GameAcronym, [monsterData.Key]);
									}
								}
							}
						}
						Console.WriteLine($"Render list built for monster {monsterData.Key}.");
						if (options.UpdatePages)
						{
							WikiPage overview = new(site, monsterData.Key);
							await overview.RefreshAsync(PageQueryOptions.FetchContent);
							if (overview.Exists)
							{
								Console.WriteLine("Updating pages.");
								gameRenders = gameRenders.OrderBy(x => Array.IndexOf(AllGameAcros, x.Key)).ToDictionary(x => x.Key, x => x.Value);
								string content = overview.Content!.ReplaceLineEndings("\r\n");
								string latestRender = "";
								if (gameRenders.Count != 0)
								{
									latestRender = gameRenders.First().Value;
									string oldMeta = content.Substring(content.IndexOf("|MetaImage"));
									oldMeta = oldMeta.Substring(0, oldMeta.IndexOf("\r\n"));
									content = content.Replace(oldMeta, $"|MetaImage = {latestRender}");
									string oldRenders = content.Substring(content.IndexOf("{{MonsterGameInfoBox_Overview"));
									oldRenders = oldRenders.Substring(oldRenders.IndexOf("|Renders = "));
									oldRenders = oldRenders.Substring(0, oldRenders.IndexOf("\r\n}}"));
									Dictionary<string, string> gameRendersCombined = [];
									foreach (KeyValuePair<string, string> render in gameRenders)
									{
										string line = $"{render.Value}{{{{!}}}}{(render.Key.StartsWith("MHST") && render.Value.Contains("Monstie") ? render.Key + " Monstie" : render.Key)}";
										if (!gameRendersCombined.ContainsKey(render.Value))
										{
											gameRendersCombined.Add(render.Value, line);
										}
										else
										{
											gameRendersCombined[render.Value] += ", " + render.Key;
										}
									}
									content = content.Replace(oldRenders, "|Renders = " + string.Join("\r\n", gameRendersCombined.Select(x => x.Value)));
									await overview.EditAsync(new WikiPageEditOptions()
									{
										Bot = true,
										Content = content,
										Minor = true,
										Summary = $"Updated InfoBox and Meta renders using dynamic pattern matching",
										Watch = AutoWatchBehavior.None
									});
									Console.WriteLine($"({cnt}/{totalCnt}) {overview.Title} edited.");
								}
								else
								{
									Console.WriteLine($"({cnt}/{totalCnt}) {overview.Title} had no renders; skipped.");
								}
								foreach (GameAppearance appearance in monsterData.Value.GameAppearances.Where(x => !finishedGames.Contains($"{monsterData.Key} ({x.GameAcronym})") && migratedGames.Contains(x.GameAcronym)).OrderBy(x => Array.IndexOf(AllGameAcros, x.GameAcronym)))
								{
									WikiPage gamePage = new(site, $"{monsterData.Key} ({appearance.GameAcronym})");
									await gamePage.RefreshAsync(PageQueryOptions.FetchContent);
									if (gamePage.Exists)
									{
										content = gamePage.Content!.ReplaceLineEndings("\r\n");
										if (gameRenders.Count != 0)
										{
											string newRender = "";
											if (gameRenders.ContainsKey(appearance.GameAcronym))
											{
												newRender = gameRenders[appearance.GameAcronym];
											}
											else
											{
												newRender = latestRender;
											}
											string oldMeta = content.Substring(content.IndexOf("|MetaImage"));
											oldMeta = oldMeta.Substring(0, oldMeta.IndexOf("\r\n"));
											content = content.Replace(oldMeta, $"|MetaImage = {newRender}");
											string oldImage = content.Substring(content.IndexOf("{{MonsterGameInfoBox"));
											if (oldImage.Contains("|Image = "))
											{
												oldImage = oldImage.Substring(oldImage.IndexOf("|Image = "));
												oldImage = oldImage.Substring(0, oldImage.IndexOf("\r\n"));
											}
											else
											{
												oldImage = oldImage.Substring(oldImage.IndexOf("|Image="));
												oldImage = oldImage.Substring(0, oldImage.Substring(1).IndexOf("|"));
											}
											content = content.Replace(oldImage, "|Image = " + newRender.Replace("File:", ""));
										}
										await gamePage.EditAsync(new WikiPageEditOptions()
										{
											Bot = true,
											Content = content,
											Minor = true,
											Summary = $"Updated InfoBox and Meta renders using dynamic pattern matching",
											Watch = AutoWatchBehavior.None
										});
										Console.WriteLine($"({cnt}/{totalCnt}) {gamePage.Title} edited.");
									}
									else
									{
										Console.WriteLine($"({cnt}/{totalCnt}) {gamePage.Title} didn't exist; skipped.");
									}
									finishedGames.Add(gamePage.Title!);
									File.WriteAllText(gamesFile, JsonConvert.SerializeObject(finishedGames));
								}
							}
							else
							{
								Console.WriteLine($"({cnt}/{totalCnt}) {overview.Title} didn't exist; skipped.");
							}
							Console.WriteLine("Pages updated.");
						}
						finishedOverviews.Add(monsterData.Key);
						File.WriteAllText(overviewsFile, JsonConvert.SerializeObject(finishedOverviews));
						cnt++;
					}
					if (options.GenerateMissingRendersReport)
					{
						Console.WriteLine("Generating missing renders report.");
						File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\missingRendersReport.json", JsonConvert.SerializeObject(missingRenders, Formatting.Indented));
						Console.WriteLine("Generated missing renders report.");
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task MonsterAppearanceGenerator(MonsterAppearanceGeneratorOptions? options = null)
		{
			if (options == null)
			{
				options = new();
			}
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					Dictionary<string, MonsterData> srcDict = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					Dictionary<string, MonsterData> monsterAppearances = srcDict
						.Where(x => (options.WhitelistMonsters != null ? options.WhitelistMonsters.Contains(x.Key) : true) && (options.BlacklistMonsters != null ? options.BlacklistMonsters.Contains(x.Key) : true))
						.ToDictionary(x => x.Key, x => new MonsterData()
						{
							Classification = x.Value.Classification,
							DebutGame = x.Value.DebutGame,
							LangNames = x.Value.LangNames,
							Name = x.Value.Name,
							Title = x.Value.Title,
							GameAppearances = [.. x.Value.GameAppearances.Where(x => (options.WhitelistGameAcronyms != null ? options.WhitelistGameAcronyms.Contains(x.GameAcronym) : true) && (options.BlacklistGameAcronyms != null ? !options.BlacklistGameAcronyms.Contains(x.GameAcronym) : true))]
						})
						.Where(x => x.Value.GameAppearances.Any())
						.ToDictionary(x => x.Key, x => x.Value);
					if (options.WhitelistGameAcronyms != null)
					{
						Console.WriteLine("Included games on whitelist.");
					}
					if (options.BlacklistGameAcronyms != null)
					{
						Console.WriteLine("Excluded games on blacklist.");
					}
					List<string> finishedOverviews = [];
					List<string> finishedGames = [];
					string overviewsFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\appearances_overviews.json";
					string gamesFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\appearances_games.json";
					if (!options.RefreshProgressLists)
					{
						if (File.Exists(overviewsFile))
						{
							finishedOverviews = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(overviewsFile))!;
						}
						if (File.Exists(gamesFile))
						{
							finishedGames = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(gamesFile))!;
						}
					}
					else
					{
						Console.WriteLine("Refreshed progress lists.");
					}
					int cnt = 1;
					int totalCnt = monsterAppearances.Count;
					string[] migratedGames = ["MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1", "MHNow", "MHPuzzles", "MHST1", "MHST2", "MHST3", "MHFrontier", "MHi", "MHRiders"];
					foreach (KeyValuePair<string, MonsterData> monsterData in monsterAppearances.Where(x => x.Key != "Ahtal-Neset" && !finishedOverviews.Contains(x.Key) && x.Value.GameAppearances.Any(y => migratedGames.Contains(y.GameAcronym))))
					{
						if (options.UpdatePages)
						{
							WikiPage overview = new(site, monsterData.Key);
							await overview.RefreshAsync(PageQueryOptions.FetchContent);
							if (overview.Exists)
							{
								Console.WriteLine("Updating pages.");
								Dictionary<string, string> gameRenders = await GetMonsterRenders(monsterData.Value, site); 
								string appearancesNav = @$"{{{{MonsterAppearancesNav
|Monster = {monsterData.Key}
{string.Join("\r\n", srcDict[monsterData.Key].GameAppearances.Select(x => $"|{x.GameAcronym} = Y"))}
}}}}";
								string content = overview.Content!.ReplaceLineEndings("\r\n");
								string oldAppearances = content.Substring(content.IndexOf("{{MonsterAppearancesNav"));
								oldAppearances = oldAppearances.Substring(0, oldAppearances.IndexOf("}}") + 2);
								content = content.Replace(oldAppearances, appearancesNav);
								await overview.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = content,
									Minor = true,
									Summary = $"Updated appearances navbar using compiled data",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {overview.Title} edited.");
								foreach (GameAppearance appearance in monsterData.Value.GameAppearances.Where(x => !finishedGames.Contains($"{monsterData.Key} ({x.GameAcronym})") && migratedGames.Contains(x.GameAcronym)).OrderBy(x => Array.IndexOf(AllGameAcros, x.GameAcronym)))
								{
									WikiPage gamePage = new(site, $"{monsterData.Key} ({appearance.GameAcronym})");
									await gamePage.RefreshAsync(PageQueryOptions.FetchContent);
									if (gamePage.Exists)
									{
										content = gamePage.Content!.ReplaceLineEndings("\r\n");
										oldAppearances = content.Substring(content.IndexOf("{{MonsterAppearancesNav"));
										oldAppearances = oldAppearances.Substring(0, oldAppearances.IndexOf("}}") + 2);
										content = content.Replace(oldAppearances, appearancesNav);
										await gamePage.EditAsync(new WikiPageEditOptions()
										{
											Bot = true,
											Content = content,
											Minor = true,
											Summary = $"Updated appearances navbar using compiled data",
											Watch = AutoWatchBehavior.None
										});
										Console.WriteLine($"({cnt}/{totalCnt}) {gamePage.Title} edited.");
									}
									else if (options.GenerateNewPages)
									{
										await gamePage.EditAsync(new WikiPageEditOptions()
										{
											Bot = true,
											Content = await GetDataLessPage(srcDict[monsterData.Key], site, appearance),
											Minor = false,
											Summary = $"Created data-less page via API.",
											Watch = AutoWatchBehavior.None
										});
										Console.WriteLine($"({cnt}/{totalCnt}) {gamePage.Title} created.");
									}
									finishedGames.Add(gamePage.Title!);
									File.WriteAllText(gamesFile, JsonConvert.SerializeObject(finishedGames));
								}
							}
							else if (!options.GenerateNewPages)
							{
								Console.WriteLine($"({cnt}/{totalCnt}) {overview.Title} didn't exist and was not requested to be populated; skipped.");
							}
							Console.WriteLine("Pages updated.");
						}
						finishedOverviews.Add(monsterData.Key);
						File.WriteAllText(overviewsFile, JsonConvert.SerializeObject(finishedOverviews));
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		private static async Task<string> GetDataLessPage(MonsterData monster, WikiSite site, GameAppearance? targetGame = null)
		{
			string appearancesNav = @$"{{{{MonsterAppearancesNav
|Monster = {monster.Name}
{string.Join("\r\n", monster.GameAppearances.Select(x => $"|{x.GameAcronym} = Y"))}
}}}}";
			string languages = $@"{{{{LanguageNames
|ENG Name = {monster.Name}
|JP Name =
|FR Name =
|GER Name =
|SPA Name =
|KOR Name =
|RU Name =
|CHN Name =
|ITA Name =
|POL Name =
|POR Name =
|ARA Name =
|LAT Name =
|Etymology = 
}}}}";
			if (MonsterNameDict.TryGetValue(monster.Name, out MonsterNames? names))
			{
				languages = $@"{{{{LanguageNames
|ENG Name = {names.English}
|JP Name = {names.Japanese}
|FR Name = {names.French}
|GER Name = {names.German}
|SPA Name = {names.Spanish}
|KOR Name = {names.Korean}
|RU Name = {names.Russian}
|CHN Name = {names.SimplifiedChinese} <small>(Simplified)</small> / {names.TraditionalChinese} <small>(Traditional)</small>
|ITA Name = {names.Italian}
|POL Name = {names.Polish}
|POR Name = {names.PortugueseBr}
|ARA Name = {names.Arabic}
|LAT Name = {names.LatinAmericanSpanish}
|Etymology = 
}}}}";
			}
			string validRender = "";
			string validIcon = "";
			Dictionary<string, string> monsterRenders = await GetMonsterRenders(monster, site);
			StringBuilder renders = new();
			foreach (string game in monster.GameAppearances.Select(x => x.GameAcronym).OrderBy(x => MainlineRelease.Contains(x) ? Array.IndexOf(MainlineRelease, x) : -999).ThenBy(x => x).Reverse())
			{
				if (monsterRenders.ContainsKey(game) && !string.IsNullOrEmpty(monsterRenders[game]))
				{
					validRender = monsterRenders[game];
				}
				else
				{
					string[] gameIcons = [$"File:{game}-{monster.Name} Icon.webp", $"File:{game}-{monster.Name} Icon.png"];
					foreach (string icon in gameIcons)
					{
						WikiPage monsterRender = new(site, icon);
						await monsterRender.RefreshAsync(PageQueryOptions.FetchContent);
						if (monsterRender.Exists)
						{
							validIcon = icon.Substring(icon.IndexOf("File:") + 5);
						}
					}
				}
			}
			if (string.IsNullOrEmpty(validIcon))
			{
				validIcon = "wiki.png";
			}
			if (string.IsNullOrEmpty(validRender))
			{
				validRender = validIcon;
			}
			if (targetGame == null)
			{
				return $@"{{{{Meta
|MetaTitle     = {monster.Name}
|MetaDesc      = This page describes {monster.Name}, which is a large monster appearing in the Monster Hunter franchise.
|MetaKeywords  = {monster.Name}
|MetaImage     = {validRender}
}}}}
{appearancesNav}
{{{{MonsterGameInfoBox_Overview
|English Name = {monster.Name}
|Image = {validRender}
|Latest Game = {monster.GameAppearances.OrderBy(x => MainlineRelease.Contains(x.GameAcronym) ? Array.IndexOf(MainlineRelease, x.GameAcronym) : -999).OrderBy(x => x.GameAcronym).Last().GameAcronym}
|Original Game = {monster.GameAppearances.OrderBy(x => MainlineRelease.Contains(x.GameAcronym) ? Array.IndexOf(MainlineRelease, x.GameAcronym) : 999).OrderBy(x => x.GameAcronym).First().GameAcronym}
|Renders = {renders.ToString()}
}}}}
'''{monster.Name}''' is a Large Monster introduced in {GetGameFullName(monster.GameAppearances.OrderBy(x => MainlineRelease.Contains(x.GameAcronym) ? Array.IndexOf(MainlineRelease, x.GameAcronym) : 999).OrderBy(x => x.GameAcronym).First().GameAcronym)}.
__TOC__
= Summary =
= Appearances =
== Main Series ==
{string.Join("\r\n", monster.GameAppearances.Where(x => MainSeriesAcros.Contains(x.GameAcronym)).Select(x => $"* '''''[[{x.GameFull}]]''''': ???"))}
== Spin-Offs ==
{string.Join("\r\n", monster.GameAppearances.Where(x => SpinoffAcros.Contains(x.GameAcronym)).Select(x => $"* '''''[[{x.GameFull}]]''''': ???"))}
=Ecology & Lore=
{{{{LoreInfobox
|Length           =
|Height           =
|Foot             =
|Habitats         =
|Order            =
|Suborder         =
|Infraorder       =
|Superfamily      =
|Family           =
|Genus            =
}}}}
==Physiology==

==Behavior==

=Development=

=Gallery=
<div style=""max-width:500px; margin-left:auto; margin-right:auto;""><gallery mode=slideshow caption=""Concept Art"">
</gallery></div>

=Name in Other Languages=
{languages}

=Trivia=

=Notes=
<references group=""notes""/>
=Sources=
<references />
{{{{MonsterListsBox}}}}
{{{{MonsterListBox}}}}
[[Category:Large Monsters]]
[[Category:Monsters introduced in {monster.GameAppearances.OrderBy(x => MainlineRelease.Contains(x.GameAcronym) ? Array.IndexOf(MainlineRelease, x.GameAcronym) : 999).OrderBy(x => x.GameAcronym).First().GameAcronym}]]
{string.Join("\r\n", monster.GameAppearances.Select(x => $"[[Category:{x.GameAcronym} Monsters]]"))}";
			}
			else
			{
				string finalRender = "";
				string finalIcon = "";
				if (monsterRenders.ContainsKey(targetGame.GameAcronym) && !string.IsNullOrEmpty(monsterRenders[targetGame.GameAcronym]))
				{
					finalRender = monsterRenders[targetGame.GameAcronym];
				}
				string[] gameIcons = [$"File:{targetGame.GameAcronym}-{monster.Name} Icon.webp", $"File:{targetGame.GameAcronym}-{monster.Name} Icon.png"];
				foreach (string icon in gameIcons)
				{
					WikiPage monsterRender = new(site, icon);
					await monsterRender.RefreshAsync(PageQueryOptions.FetchContent);
					if (monsterRender.Exists)
					{
						finalIcon = icon.Substring(icon.IndexOf("File:") + 5);
					}
				}
				if (string.IsNullOrEmpty(finalIcon))
				{
					if (!string.IsNullOrEmpty(validIcon))
					{
						finalIcon = validIcon;
					}
					else
					{
						finalIcon = "wiki.png";
					}
				}
				if (string.IsNullOrEmpty(finalRender))
				{
					if (!string.IsNullOrEmpty(validRender))
					{
						finalRender = validRender;
					}
					else
					{
						finalRender = finalIcon;
					}
				}
				if (finalRender.StartsWith("File:"))
				{
					finalRender = finalRender.Substring(finalRender.IndexOf("File:") + 5);
				}
				if (finalIcon.StartsWith("File:"))
				{
					finalIcon = finalIcon.Substring(finalIcon.IndexOf("File:") + 5);
				}
				return $@"{{{{Meta
|MetaTitle     = {monster.Name} - {targetGame.GameAcronym}
|MetaDesc      = This page describes {monster.Name}, which is a large monster appearing in {targetGame.GameFull} ({targetGame.GameAcronym}). Described here are its weaknesses, elements, attacks, armor, weapons, hitzones (HZVs), parts, drops and drop rates, materials, crown sizes, ecology, lore, history, images, notes, and more.
|MetaKeywords  = {monster.Name}, Wilds, Stats, Materials, Attacks 
|MetaImage     = {finalRender}
}}}}
<div class=subpages style=""color: #54595d; font-size:0.95rem"">< <bdi dir=""ltr"">[[{monster.Name}]]</bdi></div>
{appearancesNav}
{{{{MonsterGameInfoBox
|English Name = {monster.Name}
|Image = {finalRender}
}}}}
=Gameplay Information=
''The following information is for [[{targetGame.GameFull}]]. The Gameplay Information for other titles can be found from the navigation bar at the top of this page.''

<div style=""display: flex;flex-direction: column;"">
==Hunter's Notes==
{{{{MonsterDescription
|Description=
|Icon={finalIcon}
|Game=[[{targetGame.GameFull}]]}}}}
</div>

==Physical Attributes==
<div style=""display: flex;flex-direction: column;"">
===Damage Effectiveness===
{{| class=""wikitable mobile-sm"" style=""text-align:center;width:100%;margin:auto""
! colspan=""12"" | Damage Effectiveness <sup style=""color:mediumvioletred"">Tenderized</sup>(%)
|-
|- class=""sticky-row""
!'''Part'''
![[File:MHWilds-Kinsect_Cutting_Icon_Rare_0.png|24x24px]]
![[File:MHWilds-Great_Sword_Icon_Rare_0.png|24x24px]]
![[File:MHWilds-Hammer_Icon_Rare_0.png|24x24px]]
![[File:MHWilds-Ammo_Icon_White.png|24x24px]]
![[File:UI-Fireblight.png|24x24px]]
![[File:UI-Waterblight.png|24x24px]]
![[File:UI-Thunderblight.png|24x24px]]
![[File:UI-Iceblight.png|24x24px]]
![[File:UI-Dragonblight.png|24x24px]]
![[File:UI-Stun.png|24x24px]]
|-
|}}
{{{{UserHelpBox|
*'''Part''' - the name of the part. Any special states or conditions are shown in parentheses. 
*[[File:MHWilds-Great_Sword_Icon_Rare_0.png|24x24px]][[File:MHWilds-Hammer_Icon_Rare_0.png|24x24px]][[File:MHWilds-Ammo_Icon_White.png|24x24px]] - the effectiveness of '''Cutting''', '''Blunt''', and '''Shot''' raw attack types respectively. For example, if a part has a hitzone value of 40 for Cutting, it means that any Cutting damage dealt to it is reduced by 60%. Hitzone values greater than or equal to '''45''' are considered weak spots by the game, displaying orange damage numbers and activating [[Weakness Exploit ({targetGame.GameAcronym})|Weakness Exploit]]. These numbers are given in '''bold'''. Tenderized values are given in Purple.
*{{{{UI|UI|Fire}}}}{{{{UI|UI|Water}}}}{{{{UI|UI|Thunder}}}}{{{{UI|UI|Ice}}}}{{{{UI|UI|Dragon}}}} - the effectiveness of each elemental damage type. For example, if a part has a hitzone value of 25 for Thunder, it means that any Thunder damage dealt to it is reduced by 75%.
*{{{{UI|UI|Stun}}}} - the effectiveness of [[Stun]] against the specified part. For example, a value of 50 means that only 50% of all stun buildup dealt to that part contributes to the monster's stun threshold. A <code>-</code> means that hitting the part with an attack will never contribute to stun buildup, regardless of its properties.
}}}}
</div>
<div class=""twocol"">
<div>
===Status Effectiveness===
{{{{MHWIStatusData
|Status Data Template: Add the values you want after the ""="" symbol.
|For the ""★"" numbers just copy and paste the star symbol and paste it after the ""=""
|for the ""In Master Rank"" values just input the number, the template will automatically add the ""In MR"" text to the right side of it.
|Use ""N/A"" when the Status does not apply to this monster (Like when Elderseal does nothing).
|Poison data
|Poison ★ number = 
|Poison Threshold =
|Poison Increase =
|Poison Duration =
|Poison Decay =
|Poison Damage =
|Poison Damage in Master Rank=

|Paralysis data (Paralysis doesn't have Damage Values).
|Paralysis ★ number =
|Paralysis Threshold =
|Paralysis Increase =
|Paralysis Duration =
|Paralysis Decay =

|Sleep Data (Sleep doesn't have Damage Values).
|Sleep ★ number =
|Sleep Threshold =
|Sleep Increase =
|Sleep Duration =
|Sleep Decay =

|Blast Data (Blast doesn't have Duration neither Decay Values).
|Blast ★ number =
|Blast Threshold =
|Blast Increase =
|Blast Damage =
|Blast Damage in Master Rank =

|Stun Data (Stun doesn't have Damage Values).
|Stun ★ number =
|Stun Threshold =
|Stun Increase =
|Stun Duration =
|Stun Decay =

|Exhaust Data (The Damage is made directly against the Monster's Stamina, Not the monster health)
|Exhaust Threshold =
|Exhaust Increase =
|Exhaust Duration =
|Exhaust Decay =
|Exhaust Damage =

|Mount Data (Mount doesn't have Duration, Decay and/or Damage Values).
|Mount Threshold =
|Mount Increase =
}}}}
</div>
<div>
===Item Effectiveness===
{{{{ItemEffectivenessTable
|Game = {targetGame.GameAcronym}

|Pitfall Duration =
|Pitfall Exhausted Duration =
|Pitfall Tolerance Reduction =
|Pitfall Min Duration =

|Shock Duration =
|Shock Exhausted Duration =
|Shock Tolerance Reduction =
|Shock Min Duration =

|Ivy Duration =
|Ivy Exhausted Duration =

|Flash Duration =
|Flash Note =
|Flash Tolerance Reduction =
|Flash Min Duration =

|Meat Effectiveness =

|Dung Effectiveness =

|Sonic Effectiveness =
}}}}
</div>
</div>

== Attacks ==
{{| class=""mw-collapsible mw-collapsed wikitable"" style=""width:100%;""
|- class=""sticky-row""
!Name 
!style=""width:10%;"" |Base Damage 
!style=""width:10%;""|Status Buildup 
!style=""width:5%;""|Startup
!style=""width:20%;""|Active
!style=""width:5%;""|Recovery
!style=""width:5%;""|Stamina Cost 
!style=""width:5%;""|Guard Knockback 
!style=""width:20%;""|Animation
|-
|}}
{{{{UserHelpBox|!!Monster Attack Help!!}}}}
== Drop Rates ==
<tabber>
|-| High Rank = 
<div class=""threecen"">
<div style=""padding-bottom: 10px;"">
{{| class=""wikitable itemtable mw-collapsible""
!colspan=""2"" | <big>Gathered</big>
|-
!Item
!Chance
|-
|}}
</div>
<div style=""padding-bottom: 10px;"">
{{| class=""wikitable itemtable mw-collapsible""
!colspan=""2"" | <big>Quest Rewards</big>
|-
!Item
!Chance
|-
|}}
</div>
<div style=""padding-bottom: 10px;"">
{{| class=""wikitable itemtable mw-collapsible""
!colspan=""2"" | <big>Investigation</big>
|-
!Item
!Chance
|-
|}}
</div>
</tabber>
=Sources=
<references />
{{{{MonsterListsBox}}}}
{{{{MonsterListBox}}}}
<noinclude>
[[Category:Large Monsters]]
[[Category:Large Monster {targetGame.GameAcronym} Subpages]]
[[Category:Monsters introduced in {monster.GameAppearances.OrderBy(x => MainlineRelease.Contains(x.GameAcronym) ? Array.IndexOf(MainlineRelease, x.GameAcronym) : 999).ThenBy(x => x.GameAcronym).First().GameAcronym}]]
</noinclude>";
			}
		}

		public static async Task UndoBadEdits()
		{

			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					WikiPage[] monsters = [.. monsterAppearances.Where(x => !x.Key.StartsWith("Hardcore ") && x.Value.GameAppearances.Any(y => y.GameAcronym == "MHFrontier")).Select(x => new WikiPage(site, $"{x.Key}/MHFrontier"))];
					monsters = [.. monsters.Where(x => !x.Title!.Contains("Sandbox"))];
					int cnt = 1;
					int totalCount = monsters.Length;
					foreach (WikiPage page in monsters)
					{
						string monsterName = page.Title!.Substring(0, page.Title!.IndexOf("/MHFrontier"));
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							await page.RefreshAsync(PageQueryOptions.FetchContent);
							RevisionsGenerator gen2 = page.CreateRevisionsGenerator();
							Revision[] revs = await gen2.EnumItemsAsync().ToArrayAsync();
							Revision contentInfo = await Revision.FetchRevisionAsync(site, revs.OrderByDescending(x => x.TimeStamp).First(x => x.TimeStamp < new DateTime(2025, 7, 25, 1, 10, 0)).Id);
							string content = contentInfo.Content!;
							await page.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = content,
								Minor = false,
								Summary = $"Fixed mistake with Large Monster Redesign 2025.",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} reverted.");
						}
						else
						{
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} didn't exist; skipped.");
						}
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				try
				{
					await site.Login();
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					string[] allGameAcros = [.. new string[] { "MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1", "MHNow", "MHOutlanders", "MHPuzzles", "MHST1", "MHST2", "MHFrontier", "MHOnline", "MHExplore", "MHi", "MHRiders", "MHDiary", "MHDG", "MHDDX", "MHBGHQ", "MHDH", "MHMH", "MHPIV", "MHSpirits", "MHGii", "MHP1", "MHP2", "MHP2G", "MH3G", "MH4G", "MHX", "MHXX" }.Where(x => monsterAppearances.Any(y => y.Value.GameAppearances.Any(z => z.GameAcronym == x)))];
					WikiPage[] monsters = [.. monsterAppearances.SelectMany(x => x.Value.GameAppearances.Select(y => new WikiPage(site, $"{x.Key}/{y.GameAcronym}")))];
					monsters = [.. monsters.Where(x => !x.Title!.Contains("Sandbox"))];
					int cnt = 1;
					int totalCount = monsters.Length;
					foreach (WikiPage page in monsters)
					{
						string[] pageElements = page.Title!.Split("/");
						string monsterName = pageElements[0];
						string game = pageElements[1];
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							await page.RefreshAsync(PageQueryOptions.FetchContent);
							RevisionsGenerator gen2 = page.CreateRevisionsGenerator();
							Revision[] revs = await gen2.EnumItemsAsync().ToArrayAsync();
							Revision contentInfo = await Revision.FetchRevisionAsync(site, revs.OrderByDescending(x => x.TimeStamp).First(x => x.TimeStamp < new DateTime(2025, 7, 25, 1, 10, 0)).Id);
							string content = contentInfo.Content!;
							await page.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = content,
								Minor = false,
								Summary = $"Fixed mistake with Large Monster Redesign 2025.",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} reverted.");
						}
						else
						{
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} didn't exist; skipped.");
						}
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task OudatedUpdate()
		{
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					List<string> progress = [];
					string progressFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\outdated_delete.json";
					if (File.Exists(progressFile))
					{
						progress = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(progressFile))!;
					}
					await site.Login(); Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					string[] allGameAcros = [.. new string[] { "MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1", "MHNow", "MHOutlanders", "MHPuzzles", "MHST1", "MHST2", "MHFrontier", "MHOnline", "MHExplore", "MHi", "MHRiders", "MHDiary", "MHDG", "MHDDX", "MHBGHQ", "MHDH", "MHMH", "MHPIV", "MHSpirits", "MHGii", "MHP1", "MHP2", "MHP2G", "MH3G", "MH4G", "MHX", "MHXX" }.Where(x => monsterAppearances.Any(y => y.Value.GameAppearances.Any(z => z.GameAcronym == x)))];
					WikiPage[] monsters = [.. monsterAppearances.Where(x => !SmallMonsterNames.Contains(x.Key)).Select(x => new WikiPage(site, $"{x.Key}/Outdated")).Where(x => !progress.Contains(x.Title!))];
					monsters = [.. monsters.Where(x => !x.Title!.Contains("Sandbox"))];
					int cnt = 1;
					int totalCount = monsters.Length;
					foreach (WikiPage page in monsters)
					{
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							await page.DeleteAsync("Long since outdated, now deleted as of Large Monster Page Redesign 2025.", AutoWatchBehavior.None);
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} deleted.");
						}
						else
						{
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} didn't exist; skipped.");
						}
						progress.Add(page.Title!);
						File.WriteAllText(progressFile, JsonConvert.SerializeObject(progress));
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task RerouteLargeMonsterRedirects()
		{
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					List<string> progress = [];
					string progressFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\phase3.json";
					if (File.Exists(progressFile))
					{
						progress = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(progressFile))!;
					}
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					string[] allGameAcros = [.. new string[] { "MHWilds", "MHRS", "MHRise", "MHWI", "MHWorld", "MHGU", "MHGen", "MH4U", "MH4", "MH3U", "MHP3", "MH3", "MHFU", "MHF2", "MH2", "MHF1", "MHG", "MH1", "MHNow", "MHOutlanders", "MHPuzzles", "MHST1", "MHST2", "MHFrontier", "MHOnline", "MHExplore", "MHi", "MHRiders", "MHDiary", "MHDG", "MHDDX", "MHBGHQ", "MHDH", "MHMH", "MHPIV", "MHSpirits", "MHGii", "MHP1", "MHP2", "MHP2G", "MH3G", "MH4G", "MHX", "MHXX" }.Where(x => monsterAppearances.Any(y => y.Value.GameAppearances.Any(z => z.GameAcronym == x)))];
					WikiPage[] monsters = [.. monsterAppearances.SelectMany(x => x.Value.GameAppearances.Select(y => new WikiPage(site, $"{x.Key}/{y.GameAcronym}")))];
					monsters = [.. monsters.Where(x => !progress.Any(y => y == x.Title!) && !x.Title!.Contains("Sandbox"))];
					int cnt = 1;
					int totalCount = monsters.Length;
					foreach (WikiPage page in monsters)
					{
						string[] pageElements = page.Title!.Split("/");
						string monsterName = pageElements[0];
						string game = pageElements[1];
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							BacklinksGenerator gen = new(site, page.Title!)
							{
								AllowRedirectedLinks = false
							};
							WikiPage[] results = await gen.EnumPagesAsync().ToArrayAsync();
							foreach (WikiPage res in results.Where(x => !x.Title!.Contains(monsterName)))
							{
								await res.RefreshAsync(PageQueryOptions.FetchContent);
								if (res.Content!.Contains(page.Title.Replace(" ", "_")) || res.Content!.Contains(page.Title.Replace(" ", "_")))
								{
									await res.EditAsync(new WikiPageEditOptions()
									{
										Bot = true,
										Content = res.Content!.Replace(page.Title, monsterName + $" ({game})").Replace(page.Title.Replace(" ", "_"), monsterName.Replace(" ", "_") + $" ({game})"),
										Minor = false,
										Summary = $"Rerouted {monsterName}/{game} hard link to {monsterName} ({game}) in accordance with Large Monster Page Redesign 2025.",
										Watch = AutoWatchBehavior.None
									});
									Console.WriteLine($"{res.Title!} adjusted.");
								}
								else
								{
									Console.WriteLine($"{res.Title!} didn't contain a hard link; skipped.");
								}
							}
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} rerouted.");
						}
						else
						{
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} didn't exist; skipped.");
						}
						progress.Add(page.Title!);
						File.WriteAllText(progressFile, JsonConvert.SerializeObject(progress));
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		private static void ConvertDir(string dir)
		{
			FileAttributes attr = File.GetAttributes(dir);
			if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
			{
				foreach (string newDir in Directory.EnumerateDirectories(dir))
				{
					ConvertDir(newDir);
				}
				foreach (string newFile in Directory.EnumerateFiles(dir).Where(x => new FileInfo(x).Extension == ".tex" && !File.Exists(x.Replace(".tex", ".dds"))))
				{
					ConvertDir(newFile);
				}
			}
			else
			{
				Process proc = Process.Start(@"""D:\MH_Data Repo\MH_Data\Tools\MHWI\MHWTexConverter_by_Jodo.exe""", $"\"{dir}\"");
			}
			Console.WriteLine($"{dir} processed!");
		}

		public static void ConvertWorldDDSes()
		{
			ConvertDir(@"D:\MH_Data Repo\MH_Data\Raw Data\MHWI\chunk");
		}

		public static async Task RerouteFrontierLargeMonsterRedirects()
		{
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					List<string> progress = [];
					string progressFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\phase2.json";
					if (File.Exists(progressFile))
					{
						progress = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(progressFile))!;
					}
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					WikiPage[] monsters = [.. monsterAppearances.Where(x => !x.Key.StartsWith("Hardcore ") && x.Value.GameAppearances.Any(y => y.GameAcronym == "MHFrontier")).Select(x => new WikiPage(site, $"{x.Key}/MHFrontier"))];
					monsters = [.. monsters.Where(x => !progress.Any(y => y == x.Title!) && !x.Title!.Contains("Sandbox"))];
					int cnt = 1;
					int totalCount = monsters.Length;
					foreach (WikiPage page in monsters)
					{
						string monsterName = page.Title!.Substring(0, page.Title!.IndexOf("/MHFrontier"));
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							BacklinksGenerator gen = new(site, page.Title!)
							{
								AllowRedirectedLinks = false
							};
							WikiPage[] results = await gen.EnumPagesAsync().ToArrayAsync();
							foreach (WikiPage res in results.Where(x => !x.Title!.Contains(monsterName)))
							{
								await res.RefreshAsync(PageQueryOptions.FetchContent);
								if (res.Content!.Contains(page.Title.Replace(" ", "_")) || res.Content!.Contains(page.Title.Replace(" ", "_")))
								{
									await res.EditAsync(new WikiPageEditOptions()
									{
										Bot = true,
										Content = res.Content!.Replace(page.Title, monsterName + " (MHFrontier)").Replace(page.Title.Replace(" ", "_"), monsterName.Replace(" ", "_") + " (MHFrontier)"),
										Minor = false,
										Summary = $"Rerouted redirect to new Large Monster page in accordance with Large Monster Page Redesign 2025.",
										Watch = AutoWatchBehavior.None
									});
									Console.WriteLine($"{res.Title!} adjusted.");
								}
								else
								{
									Console.WriteLine($"{res.Title!} didn't contain a hard link; skipped.");
								}
							}
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} rerouted.");
						}
						else
						{
							Console.WriteLine($"({cnt}/{totalCount}) {page.Title} didn't exist; skipped.");
						}
						progress.Add(page.Title!);
						File.WriteAllText(progressFile, JsonConvert.SerializeObject(progress));
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task RemoveLargeMonsterRedirects()
		{
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					List<string> progress = [];
					string progressFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\phase1.json";
					if (File.Exists(progressFile))
					{
						progress = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(progressFile))!;
					}
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					CategoryMembersGenerator generator = new(site, $"Category:Monsters To Remove Redirect")
					{
						MemberTypes = CategoryMemberTypes.Page
					};
					WikiPage[] monsters = await generator.EnumPagesAsync().ToArrayAsync();
					monsters = [.. monsters.Where(x => !progress.Any(y => y == x.Title!) && !x.Title!.Contains("Sandbox")).Take(100)];
					int cnt = 1;
					int totalCount = monsters.Length;
					foreach (WikiPage page in monsters)
					{
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						string content = page.Content!;
						string redirectSubstr = content.Substring(content.IndexOf("#REDIRECT"));
						if (content.Contains("[[Category:Monsters To Remove Redirect]]\r\n"))
						{
							redirectSubstr = redirectSubstr.Substring(0, redirectSubstr.IndexOf("[[Category:Monsters To Remove Redirect]]\r\n") + 42);
						}
						else
						{
							redirectSubstr = redirectSubstr.Substring(0, redirectSubstr.IndexOf("[[Category:Monsters To Remove Redirect]]\n") + 41);
						}
						content = content.Replace(redirectSubstr, "");
						await page.EditAsync(new WikiPageEditOptions()
						{
							Bot = true,
							Content = content.ToString().Replace("\r\n\r\n\r\n", "\r\n\r\n"),
							Minor = false,
							Summary = $"Removed redirect to launch new Large Monster page designs in accordance with Large Monster Page Redesign 2025.",
							Watch = AutoWatchBehavior.None
						});
						progress.Add(page.Title!);
						File.WriteAllText(progressFile, JsonConvert.SerializeObject(progress));
						Console.WriteLine($"({cnt}/{totalCount}) {page.Title} edited.");
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task<string[]> GetWeaponRenders(string game, string weaponType)
		{
			string[] items = [];
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					CategoryMembersGenerator generator = new(site, $"Category:{game} {weaponType} Renders")
					{
						MemberTypes = CategoryMemberTypes.File
					};
					items = await generator.EnumPagesAsync().Select(x => x.Title!).ToArrayAsync();
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
			return items;
		}

		public static async Task AddRiseCrowns()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					int cnt = 1;
					await site.Login();
					Dictionary<string, string> riseMonsterIds = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHRS\monsterIds.json"))!;
					JArray sizeList = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\enemy\user_data\system_enemy_sizelist_data.user.2.json"))!.Value<JObject>("snow.enemy.SystemEnemySizeListData")!.Value<JArray>("_SizeInfoList")!;
					Dictionary<string, string[]> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\allMonsterAppearances.json"))!;
					int totalCnt = monsterAppearances.Count(x => x.Value.Contains("MHRS") || x.Value.Contains("MHRise"));
					string[] sizeParams = ["|Gold Crown Small = ", "|Average = ", "|Silver Crown = ", "|Gold Crown Large = "];
					foreach (string monster in monsterAppearances.Where(x => x.Value.Contains("MHRS") || x.Value.Contains("MHRise")).Select(x => x.Key))
					{
						JToken crownObj = sizeList.First(x => x.Value<string>("_EmType") == riseMonsterIds[monster]);
						double avgSize = crownObj.Value<double>("_BaseSize");
						double smallMult = crownObj.Value<double>("_SmallBorder");
						double silverMult = crownObj.Value<double>("_BigBorder");
						double goldMult = crownObj.Value<double>("_KingBorder");
						string[] sizeValues = [Math.Round(smallMult * avgSize, 2).ToString(), Math.Round(avgSize, 2).ToString(), Math.Round(silverMult * avgSize, 2).ToString(), Math.Round(goldMult * avgSize, 2).ToString()];
						WikiPage mhrisePage = new(site, $"{monster} (MHRise)");
						await mhrisePage.RefreshAsync(PageQueryOptions.FetchContent);
						if (mhrisePage.Exists)
						{
							string newContent = mhrisePage.Content!.ReplaceLineEndings("\r\n");
							string infoBox = newContent.Substring(newContent.IndexOf("{{MonsterGameInfoBox"), newContent.IndexOf(@"}}
=Gameplay Information=") + 2 - newContent.IndexOf("{{MonsterGameInfoBox"));
							string oldInfo = infoBox;
							int sizeCntr = 0;
							foreach (string sizeParam in sizeParams)
							{
								string finalParam = $"{sizeParam}{sizeValues[sizeCntr]}";
								if (infoBox.Contains(sizeParam))
								{
									string sizeSubstr = infoBox.Substring(infoBox.IndexOf(sizeParam));
									sizeSubstr = sizeSubstr.Substring(0, sizeSubstr.IndexOf("\r\n"));
									infoBox = infoBox.Replace(sizeSubstr, finalParam);
								}
								else
								{
									infoBox = infoBox.Insert(infoBox.LastIndexOf("}}"), $"{finalParam}\r\n");
								}
								sizeCntr++;
							}
							newContent = newContent.Replace(oldInfo, infoBox);
							await mhrisePage.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = newContent,
								Minor = true,
								Summary = "Added crown data.",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cnt}/{totalCnt}) {mhrisePage.Title} edited.");
						}
						WikiPage mhrsPage = new(site, $"{monster} (MHRS)");
						await mhrsPage.RefreshAsync(PageQueryOptions.FetchContent);
						if (mhrsPage.Exists)
						{
							string newContent = mhrsPage.Content!.ReplaceLineEndings("\r\n");
							string infoBox = newContent.Substring(newContent.IndexOf("{{MonsterGameInfoBox"), newContent.IndexOf(@"}}
=Gameplay Information=") + 2 - newContent.IndexOf("{{MonsterGameInfoBox"));
							string oldInfo = infoBox;
							int sizeCntr = 0;
							foreach (string sizeParam in sizeParams)
							{
								string finalParam = $"{sizeParam}{sizeValues[sizeCntr]}";
								if (infoBox.Contains(sizeParam))
								{
									string sizeSubstr = infoBox.Substring(infoBox.IndexOf(sizeParam));
									sizeSubstr = sizeSubstr.Substring(0, sizeSubstr.IndexOf("\r\n"));
									infoBox = infoBox.Replace(sizeSubstr, finalParam);
								}
								else
								{
									infoBox = infoBox.Insert(infoBox.LastIndexOf("}}"), $"{finalParam}\r\n");
								}
								sizeCntr++;
							}
							newContent = newContent.Replace(oldInfo, infoBox);
							await mhrsPage.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = newContent,
								Minor = true,
								Summary = "Added crown data.",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cnt}/{totalCnt}) {mhrsPage.Title} edited.");
						}
						cnt++;
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static void GetMonstersFiles()
		{
			List<MonsterId> ids = JsonConvert.DeserializeObject<List<MonsterId>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\monsterIds.json"))!;
			JArray frontierRecon = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\frontierNameRecon.json"))!;
			JObject spinoffRecon = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\spinoffRecon.json"))!;
			string[] supportedSpinoffs = ["MHNow", "MHPuzzles", "MHST1", "MHST2", "MHST3", "MHi", "MHRiders", "MHFrontier"];
			Dictionary<string, string[]> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\allMonsterAppearances.json"))!;
			Dictionary<string, string> ultGames = new()
			{
				{ "MH4", "MH4U" },
				{ "MHGen", "MHGU" },
				{ "MHWorld", "MHWI" },
				{ "MHRise", "MHRS" },
			};
			Dictionary<string, MonsterData> monsterData = monsterAppearances.Select(x => new MonsterData()
			{
				Name = x.Key,
				LangNames = MonsterNameDict.TryGetValue(x.Key, out MonsterNames? value) ? value : null,
				GameAppearances = [..x.Value.Where(y => y != "MH4U" || !x.Key.StartsWith("Apex")).Select(y => new GameAppearance() {
					GameAcronym = y,
					GameFull = GetGameFullName(y)
				})]
			}).ToDictionary(x => x.Name, x => x);
			JArray tempMonsters = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\tempMonsters.json"))!;
			Dictionary<string, JToken> tempMonstersDict = [];
			foreach (JToken mon in tempMonsters.Where(x => !x.Value<string>("Index")!.StartsWith("ems") && !x.Value<string>("Monster Name")!.Contains("(Apex4)")))
			{
				string? baseName = mon.Value<string>("Monster Name")!;
				if (frontierRecon.Any(x => x.Value<string>("OrigName")! == baseName))
				{
					baseName = frontierRecon.First(x => x.Value<string>("OrigName") == baseName).Value<string>("Translated")!;
					if (baseName == "#N/A" || string.IsNullOrEmpty(baseName))
					{
						baseName = null;
					}
				}
				else if (baseName.Contains("(ST1)"))
				{
					baseName = baseName.Substring(0, baseName.IndexOf("(ST1)"));
				}
				else if ((baseName.Contains('[') || baseName.Contains('(')) && baseName.Contains('/'))
				{
					baseName = baseName.Substring(baseName.IndexOf("/") + 1);
					if (baseName.Contains("/"))
					{
						baseName = baseName.Substring(0, baseName.IndexOf("/"));
					}
					baseName = baseName.Trim();
				}
				else if (baseName.Contains('[') || baseName.Contains('('))
				{
					string finalName = "";
					if (baseName.Contains('['))
					{
						finalName = baseName.Substring(baseName.IndexOf("[") + 1, baseName.IndexOf("]") - 1 - baseName.IndexOf("[")).Trim() + " ";
					}
					if (baseName.Contains('('))
					{
						while (baseName.Contains('('))
						{
							string parenth = baseName.Substring(baseName.IndexOf("("), baseName.IndexOf(")") + 1 - baseName.IndexOf("("));
							baseName = baseName.Replace(parenth, "");
						}
					}
					baseName = finalName + baseName.Trim();
				}
				else if (baseName.Contains("/"))
				{
					baseName = baseName.Substring(0, baseName.IndexOf("/")).Trim();
				}
				if (baseName != null)
				{
					tempMonstersDict.TryAdd(baseName, mon);
				}
			}
			foreach (KeyValuePair<string, JToken> tempMonster in tempMonstersDict)
			{
				MonsterData data = new()
				{
					Name = tempMonster.Key,
					LangNames = MonsterNameDict.TryGetValue(tempMonster.Key, out MonsterNames? value) ? value : null
				};
				if (monsterData.TryGetValue(tempMonster.Key, out MonsterData? v))
				{
					data = v;
				}
				data.Title = tempMonster.Value.Value<string>("Title")!;
				data.Classification = tempMonster.Value.Value<string>("Classification")!;
				data.DebutGame = tempMonster.Value.Value<string>("Debut")!;
				foreach (string game in AllGameAcros.Where(x => !string.IsNullOrEmpty(tempMonster.Value.Value<string>(x)) && (x != "MH4U" || !tempMonster.Key.StartsWith("Apex"))))
				{
					string id = tempMonster.Value.Value<string>(game)!;
					if (data.GameAppearances.Any(x => x.GameAcronym == game))
					{
						data.GameAppearances.First(x => x.GameAcronym == game).InternalID = id;
					}
					else
					{
						data.GameAppearances.Add(new()
						{
							GameAcronym = game,
							GameFull = GetGameFullName(game),
							InternalID = id
						});
					}
				}
				if (!monsterData.TryAdd(tempMonster.Key, data))
				{
					monsterData[tempMonster.Key] = data;
				}
			}
			foreach (MonsterId id in ids.Where(x => !string.IsNullOrEmpty(x.Name) && Convert.ToInt32(x.Id.Substring(x.Id.IndexOf("EM") + 2, 4)) < 165))
			{
				GameAppearance newApp = new()
				{
					GameAcronym = "MHWilds",
					GameFull = GetGameFullName("MHWilds"),
					InternalID = id.Id
				};
				if (monsterData.ContainsKey(id.Name))
				{
					MonsterData data = monsterData[id.Name];
					data.GameAppearances.Add(newApp);
					data.LangNames = MonsterNameDict[id.Name];
					monsterData[id.Name] = data;
				}
				else
				{
					monsterData.Add(id.Name, new()
					{
						GameAppearances = [newApp],
						LangNames = MonsterNameDict[id.Name],
						DebutGame = "MHWilds",
						Name = id.Name
					});
				}
			}
			foreach (KeyValuePair<string, MonsterData> data in monsterData.Where(x => ultGames.Any(y => x.Value.GameAppearances.Any(z => z.GameAcronym == y.Key && !x.Value.GameAppearances.Any(z => z.GameAcronym == y.Value)))))
			{
				foreach (KeyValuePair<string, string> gameAcron in ultGames.Where(x => data.Value.GameAppearances.Any(y => y.GameAcronym == x.Key)))
				{
					GameAppearance app = data.Value.GameAppearances.First(x => x.GameAcronym == gameAcron.Key);
					data.Value.GameAppearances.Add(new()
					{
						GameAcronym = gameAcron.Value,
						GameFull = GetGameFullName(gameAcron.Value),
						InternalID = app.InternalID
					});
				}
			}
			foreach (KeyValuePair<string, MonsterData> data in monsterData.Where(x => !x.Value.GameAppearances.Any(y => y.GameAcronym == x.Value.DebutGame)))
			{
				data.Value.GameAppearances.Add(new GameAppearance() { GameAcronym = data.Value.DebutGame, GameFull = GetGameFullName(data.Value.DebutGame) });
			}
			string[] supportedFrontierMonsters = ["Rathian", "Fatalis", "Yian Kut-Ku", "Lao-Shan Lung", "Cephadrome", "Rathalos", "Diablos", "Khezu", "Gravios", "Gypceros", "Plesioth", "Basarios", "Monoblos", "Velocidrome", "Gendrome", "Iodrome", "Kirin", "Crimson Fatalis", "Pink Rathian", "Blue Yian Kut-Ku", "Purple Gypceros", "Yian Garuga", "Silver Rathalos", "Gold Rathian", "Black Diablos", "White Monoblos", "Red Khezu", "Green Plesioth", "Black Gravios", "Daimyo Hermitaur", "Azure Rathalos", "Ashen Lao-Shan Lung", "Blangonga", "Congalala", "Rajang", "Kushala Daora", "Shen Gaoren", "Yama Tsukami", "Chameleos", "Rusted Kushala Daora", "Lunastra", "Teostra", "Shogun Ceanataur", "Bulldrome", "Old Fatalis", "Yama Tsukami", "Hypnocatrice", "Lavasioth", "Tigrex", "Akantor", "Vivid Hypnocatrice", "Red Lavasioth", "Espinas", "Flaming Espinas", "Pale Hypnocatrice", "Aqra Vashim", "Aqra Jebia", "Belkuros", "Pariapuria", "Pearl Espinas", "Kamu Orgaron", "Nono Orgaron", "Raviente", "Duragaua", "Draguros", "Barbaceros", "Bulluk", "Alpelo", "Rukodiora", "Unknown", "Gogomoa", "Kokomoa", "Taikun Zamuza", "Abiorg", "Quarzeps", "Odibataurus", "Vorsphyroa", "Levidiora", "Anorpatis", "Hyujikiki", "Midogaron", "Giaorg", "Mi Ru", "Falnocatrice", "Pokaradon", "Shantien", "Pokara", "Aurisioth", "Argasioth", "Barlagual", "Zerulos", "Gougarf", "Uruki", "Foroclore", "Melaginas", "Diorex", "Garbha Daora", "Inagami", "Varsablos", "Pobolbarm", "Dolemdira", "Greadios", "Haldmerg", "Toridcless", "Gasrabazra", "Kusubami", "Yama Kurai", "Zinogre", "Deviljho", "Brachydios", "Violent Raviente", "Berserk Raviente", "Toastra", "Barioth", "Uragaan", "Stygian Zinogre", "Guanzorm", "Deviljho", "Equibra", "Voljang", "Nargacuga", "Keo'arbor", "Zenaseris", "Gore Magala", "Nargacuga", "Shagaru Magala", "Amatsu", "Elzelion", "Seregios", "Bogabadorm", "Zenith Aqra Vashim", "Zenith Anorpatis", "Zenith Barlagual", "Zenith Blangonga", "Razing Bogabadorm", "Zenith Daimyo Hermitaur", "Feral Deviljho", "Flame Tyrant", "Starved Deviljho", "Prideful Dolemdira", "Zenith Draguros", "Frostfire Elzelion", "Zenith Espinas", "Zenith Gasrabazra", "Zenith Giaorg", "Zenith Gravios", "Overlord Guanzorm", "Zenith Haldmerg", "Zenith Hypnocatrice", "Zenith Hyujikiki", "Zenith Inagami", "Zenith Khezu", "Dazzling Mi Ru", "Zenith Midogaron", "Blinking Nargacuga", "Ravenous Pariapuria", "Zenith Plesioth", "Zenith Rathalos", "Radiant Zerulos", "Zenith Rukodiora", "Zenith Taikun Zamuza", "Zenith Tigrex", "Zenith Toridcless", "Howling Zinogre", "Hardcore Blangonga", "Hardcore Diablos", "Hardcore Congalala", "Hardcore Khezu", "Hardcore Lavasioth", "Hardcore Yian Kut-Ku", "Hardcore Basarios", "Hardcore Daimyo Hermitaur", "Hardcore Flaming Espinas", "Hardcore Hypnocatrice", "Hardcore Pariapuria", "Hardcore Plesioth", "Hardcore Teostra", "Hardcore Tigrex", "Hardcore Vivid Hypnocatrice", "Hardcore Kamu Orgaron", "Hardcore Nono Orgaron"];
			foreach (string monsterName in supportedFrontierMonsters.Where(x => !monsterData.ContainsKey(x)).Distinct())
			{
				monsterData.Add(monsterName, new MonsterData()
				{
					DebutGame = "MHFrontier",
					Name = monsterName,
					GameAppearances = [new GameAppearance() { GameAcronym = "MHFrontier", GameFull = GetGameFullName("MHFrontier") }]
				});
			}
			foreach (string game in supportedSpinoffs)
			{
				foreach (string monsterName in spinoffRecon.Value<JArray>(game)!.Select(x => x.Value<string>()!).Where(x => !SmallMonsterNames.Contains(x)).Distinct().Where(x => game != "MHFrontier" || supportedFrontierMonsters.Contains(x)))
				{
					if (monsterData.ContainsKey(monsterName))
					{
						if (!monsterData[monsterName].GameAppearances.Any(x => x.GameAcronym == game))
						{
							monsterData[monsterName].GameAppearances.Add(new GameAppearance() { GameAcronym = game, GameFull = GetGameFullName(game) });
						}
					}
					else
					{
						monsterData.Add(monsterName, new MonsterData()
						{
							Name = monsterName,
							DebutGame = game,
							GameAppearances = [new GameAppearance() { GameAcronym = game, GameFull = GetGameFullName(game) }]
						});
					}
				}
			}
			string[] mainlineRelease = ["MH1", "MHG", "MHF1", "MH2", "MHFrontier", "MHF2", "MHFU", "MH3", "MHP3", "MH3U", "MH4", "MHExplore", "MH4U", "MHGen", "MHST1", "MHOnline", "MHGU", "MHWorld", "MHWI", "MHRiders", "MHRise", "MHST2", "MHRS", "MHNow", "MHWilds"];
			foreach (KeyValuePair<string, MonsterData> data in monsterData)
			{
				data.Value.GameAppearances = [.. data.Value.GameAppearances.OrderBy(x => mainlineRelease.Contains(x.GameAcronym) ? Array.IndexOf(mainlineRelease, x.GameAcronym) : 9999).ThenBy(x => x.GameAcronym)];
			}
			File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json", JsonConvert.SerializeObject(monsterData.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value), Formatting.Indented));
		}

		public static void CreateSpreadsheets()
		{
			Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
			List<string> finishedOverviews = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Large Monster Progress\overviews.json"))!;
			string[] releaseOrder = ["MHNow", "MHST2", "MHST1", "MHPuzzles", "MHFrontier", "MHi", "MHRiders"];
			XLWorkbook workbook = new();
			int sheetCnt = 5;
			string[] sheetHeaders = ["Page Name", "Page Type", "Link (Old)", "New Version created?", "New Version checked?", "Link (New)", "Checked by"];
			foreach (string gameAcro in releaseOrder)
			{
				IXLWorksheet gameSheet = workbook.AddWorksheet($"Phase {sheetCnt} - {gameAcro}");
				for (int i = 1; i <= sheetHeaders.Length; i++)
				{
					gameSheet.Cell(1, i).Value = sheetHeaders[i - 1];
					gameSheet.Cell(1, i).Style.Font.SetBold(true);
					gameSheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.FromArgb(14277081);
				}
				gameSheet.SheetView.FreezeRows(1);
				int rowCnt = 2;
				foreach (KeyValuePair<string, MonsterData> monster in monsterAppearances.Where(x => x.Value.GameAppearances.Any(y => y.GameAcronym == gameAcro)).OrderBy(x => x.Value.GameAppearances.First(y => y.GameAcronym == gameAcro).InternalID))
				{
					gameSheet.Row(rowCnt).Style.Border.TopBorder = XLBorderStyleValues.Thin;
					if (!finishedOverviews.Contains(monster.Key))
					{
						gameSheet.Cell(rowCnt, 1).Value = $"{monster.Key}/Overview";
						gameSheet.Cell(rowCnt, 2).Value = $"Overview Page";
						gameSheet.Cell(rowCnt, 2).CreateDataValidation().List("\"Overview Page,Lore Page,Game Page\"");
						gameSheet.Cell(rowCnt, 3).Value = $"https://monsterhunterwiki.org/wiki/{monster.Key}/Overview";
						gameSheet.Cell(rowCnt, 3).SetHyperlink(new XLHyperlink($"https://monsterhunterwiki.org/wiki/{monster.Key}/Overview") { IsExternal = true });
						gameSheet.Cell(rowCnt, 4).Value = false;
						gameSheet.Cell(rowCnt, 5).Value = false;
						rowCnt++;
						gameSheet.Cell(rowCnt, 1).Value = $"{monster.Key}/Lore";
						gameSheet.Cell(rowCnt, 2).Value = $"Lore Page";
						gameSheet.Cell(rowCnt, 2).CreateDataValidation().List("\"Overview Page,Lore Page,Game Page\"");
						gameSheet.Cell(rowCnt, 3).Value = $"https://monsterhunterwiki.org/wiki/{monster.Key}/Lore";
						gameSheet.Cell(rowCnt, 3).SetHyperlink(new XLHyperlink($"https://monsterhunterwiki.org/wiki/{monster.Key}/Lore") { IsExternal = true });
						gameSheet.Cell(rowCnt, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(14277081);
						gameSheet.Cell(rowCnt, 4).Value = "(Merged into Main Page)";
						gameSheet.Cell(rowCnt, 5).Value = false;
						gameSheet.Cell(rowCnt, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(14277081);
						gameSheet.Cell(rowCnt, 6).Value = "(Merged into Main Page)";
						rowCnt++;
					}
					gameSheet.Cell(rowCnt, 1).Value = $"{monster.Key}/{gameAcro}";
					gameSheet.Cell(rowCnt, 2).Value = $"Game Page";
					gameSheet.Cell(rowCnt, 2).CreateDataValidation().List("\"Overview Page,Lore Page,Game Page\"");
					gameSheet.Cell(rowCnt, 3).Value = $"https://monsterhunterwiki.org/wiki/{monster.Key}/{gameAcro}";
					gameSheet.Cell(rowCnt, 3).SetHyperlink(new XLHyperlink($"https://monsterhunterwiki.org/wiki/{monster.Key}/{gameAcro}") { IsExternal = true });
					gameSheet.Cell(rowCnt, 4).Value = false;
					gameSheet.Cell(rowCnt, 5).Value = false;
					gameSheet.Row(rowCnt).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
					rowCnt++;
					finishedOverviews.Add(monster.Key);
				}
				gameSheet.Columns().AdjustToContents();
				gameSheet.Column(sheetHeaders.Length).Width = 20;
				sheetCnt++;
			}
			workbook.SaveAs(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "MHWiki Large Monster Page Update Tracker - Spinoffs.xlsx");
		}

		public static async Task GetHHMelodiesRefDatas()
		{
			DateTime start = DateTime.Now;
			HHMelody[] allMelodies = HHMelody.Fetch();
			Dictionary<string, string> newPages = [];
			foreach (string game in allMelodies.Select(x => x.Game).Where(x => x == "MHWilds"))
			{
				HHMelody[] matchedMelodies = [.. allMelodies.Where(x => x.Game == game)];
				string[] all2or3SlotNotes = [.. matchedMelodies.SelectMany(x => x.Notes.Where(x => x != "White" && x != "Purple" && x != "Disabled" && x != "Echo").Select(x => x.Replace("Dark_Blue", "Blue").Replace("Light_Blue", "Cyan").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan"))).Distinct()];
				List<string[]> uniquePairs = [.. all2or3SlotNotes.SelectMany((x, i) => all2or3SlotNotes.Skip(i + 1), (x, y) => new string[] { x, y })];
				List<string[]> uniqueLists = [.. new string[] { "White", "Purple" }.SelectMany(x => uniquePairs.Select(y => new string[] { x, y[0], y[1] }))];
				foreach (string[] uniqueList in uniqueLists.Select(x => x.Select(x => x.Replace("Dark_Blue", "Blue").Replace("Light_Blue", "Cyan").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")).OrderBy(y => y).ToArray()).ToArray().Distinct())
				{
					StringBuilder ret = new();
					ret.AppendLine(@"==Melodies==
{| class=""wikitable"" id=""tblMelodies"" style=""margin-left:auto; margin-right:auto; text-align:center;""
! style=""min-width:100px;""|Sequence !! Melody !! Effect");
					foreach (HHMelody melody in matchedMelodies.Where(x => !x.Notes.Where(x => x != "Disabled" && x != "Echo").Any(y => !uniqueList.Contains(y.Replace("Dark_Blue", "Blue").Replace("Light_Blue", "Cyan").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")))))
					{
						string notes = "";
						foreach (string note in melody.Notes.Where(x => x != "Disabled").Select(x => x.Replace("Dark_Blue", "Blue").Replace("Light_Blue", "Cyan").Replace("Dark Blue", "Blue").Replace("Light Blue", "Cyan").Replace("Sky", "Cyan")))
						{
							if (note == "Echo")
							{
								notes += $"{{{{UI|{game}|HH Echo|nolink=true}}}}";
							}
							else
							{
								notes += $"{{{{UI|{game}|HH Note|{{{{{{{note}|1}}}}}} {note}|nolink=true}}}}";
							}
						}
						string name = melody.Name;
						if (name == "Echo Wave" && game == "MHWilds")
						{
							name = "Echo Wave ({{{Echo Wave|???}}})";
						}
						ret.AppendLine($@"<tr><td>{notes}</td><td>{melody.Icon} {name}</td><td>{melody.Description}</tr>");
					}
					ret.AppendLine($@"|}}
<noinclude>
[[Category:Hunting Horn Melody Lists]]
[[Category:{game} Hunting Horn Melody Lists]]
</noinclude>");
					string key = $"RefData:{game}-{uniqueList[0]}-{uniqueList[1]}-{uniqueList[2]}";
					if (!newPages.ContainsKey(key))
					{
						newPages.Add(key, ret.ToString());
					}
				}
			}
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					int totalCount = newPages.Count;
					int cnt = 1;
					foreach (KeyValuePair<string, string> newPage in newPages)
					{
						WikiPage newRefData = new(site, newPage.Key);
						await newRefData.RefreshAsync();
						await newRefData.EditAsync(new WikiPageEditOptions()
						{
							Bot = true,
							Content = newPage.Value,
							Minor = false,
							Summary = $"Created RefDatas for hunting horn melodies in MHWI and MHWilds.",
							Watch = AutoWatchBehavior.None
						});
						Console.WriteLine($"({cnt}/{totalCount}) {newPage.Key} created.");
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task GetAllFilesOnWiki()
		{
			DateTime start = DateTime.Now;
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					AllPagesGenerator gen = new(site)
					{
						NamespaceId = 6
					};
					WikiPageStub[] allFiles = await gen.EnumItemsAsync().ToArrayAsync();
					File.WriteAllText(@"" + System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath") + "allImgNames.json", JsonConvert.SerializeObject(allFiles.Select(x => x.Title!).ToArray(), Formatting.Indented));
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task MigrateLargeMonsterPages()
		{
			DateTime start = DateTime.Now;
			string username = "";
			string password = "";
			if (File.Exists(@"D:\Wiki Files\wikicredentials.txt"))
			{
				string[] creds = File.ReadAllLines(@"D:\Wiki Files\wikicredentials.txt");
				username = creds[0];
				password = creds[1];
			}
			if (username == null)
			{
				Console.WriteLine("Please enter your username.");
				username = Console.ReadLine()!;
				Console.WriteLine("Please enter your password.");
				password = Console.ReadLine()!;
			}
			List<string> finishedOverviews = [];
			string overviewsFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\overviews.json";
			if (File.Exists(overviewsFile))
			{
				finishedOverviews = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(overviewsFile))!;
			}
			Dictionary<string, MonsterNames> monsterNameDict = FetchMonsterNames();
			MonsterId[] monsterIds = JsonConvert.DeserializeObject<MonsterId[]>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\monsterIds.json"))!;
			Dictionary<string, string[]> crosscheck = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\crosscheck.json"))!;
			List<string> finishedGameSpecs = [];
			string gameSpecsFile = @"D:\MH_Data Repo\MH_Data\Large Monster Progress\gameSpecs.json";
			if (File.Exists(gameSpecsFile))
			{
				finishedGameSpecs = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(gameSpecsFile))!;
			}
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					string pageGameAcronym = "MHRiders";
					string[] spinoffRelease = ["MHNow", "MHST2", "MHST1", "MHPuzzles", "MHFrontier", "MHi", "MHRiders"];
					WikiPage[] monsters = monsterAppearances.Where(x => x.Key != "Ahtal-Neset" && (!crosscheck.ContainsKey(pageGameAcronym) || crosscheck[pageGameAcronym].Contains(x.Key)) && x.Value.GameAppearances.Select(y => y.GameAcronym).Contains(pageGameAcronym)).Select(x => new WikiPage(site, x.Key + "/" + pageGameAcronym)).ToArray();
					int totalCnt = monsters.Count(x => !x.Title!.Contains("Sandbox") && !finishedGameSpecs.Contains(x.Title!.Split("/")[0] + " (" + x.Title!.Split("/")[1] + ")"));
					int cnt = 1;
					foreach (WikiPage oldGameSpec in monsters.Where(x => !x.Title!.Contains("Sandbox") && !finishedGameSpecs.Contains(x.Title!.Split("/")[0] + " (" + x.Title!.Split("/")[1] + ")")))
					{
						string[] pageElements = oldGameSpec.Title!.Split('/');
						string monsterName = pageElements[0];
						await oldGameSpec.RefreshAsync(PageQueryOptions.FetchContent);
						string fullGameName = GetGameFullName(pageGameAcronym);
						if (fullGameName == "Game Unknown!")
						{
							Debugger.Break();
						}
						WikiPage monsterRedirect = new(site, $"{pageElements[0]}");
						await monsterRedirect.RefreshAsync(PageQueryOptions.FetchContent);
						WikiPage monsterOverview = new(site, $"{pageElements[0]}/Overview");
						await monsterOverview.RefreshAsync(PageQueryOptions.FetchContent);
						WikiPage monsterLore = new(site, $"{pageElements[0]}/Lore");
						await monsterLore.RefreshAsync(PageQueryOptions.FetchContent);
						WikiPage music = new(site, $"{pageElements[0]}/Music");
						await music.RefreshAsync();
						WikiPage equipment = new(site, $"{pageElements[0]}/Equipment");
						await equipment.RefreshAsync();
						WikiPage appearances = new(site, $"{pageElements[0]}/Appearances");
						await appearances.RefreshAsync();
						List<string> validGames = [.. monsterAppearances[monsterName].GameAppearances.Select(x => x.GameAcronym)];
						if (oldGameSpec.Exists && pageGameAcronym != "MH1")
						{
							MonsterNames? allNames = null;
							if (monsterNameDict.TryGetValue(monsterName, out MonsterNames? value))
							{
								allNames = value;
							}
							string oldContent = oldGameSpec.Content!;
							WikiPage newOverview = new(site, $"{pageElements[0]}"/*$"User:RampageRobot/Sandbox/{pageElements[0]}"*/);
							StringBuilder content = new();
							content.AppendLine($"#REDIRECT [[{monsterName}/{pageGameAcronym}]]");
							content.AppendLine("[[Category:Monsters To Remove Redirect]]");
							string infoBox = monsterOverview.Content!.Substring(monsterOverview.Content!.IndexOf("{{MonsterInfoBoxv2")).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
							string lastParam = infoBox.Substring(infoBox.LastIndexOf("\r\n|"));
							lastParam = lastParam.Substring(0, lastParam.IndexOf("}}"));
							infoBox = infoBox.Substring(0, infoBox.IndexOf(lastParam) + lastParam.Length);
							string latestRender = infoBox.Substring(infoBox.IndexOf("|Image =") + 8);
							string attacksSection = infoBox.Substring(infoBox.IndexOf("|Attacks ="));
							attacksSection = attacksSection.Substring(0, attacksSection.IndexOf("\r\n"));
							latestRender = latestRender.Substring(0, latestRender.IndexOf("\r\n")).Replace("{{{Render|", "").Replace("}}}", "").Trim();
							string lorePage = $"{{{{:{monsterName}/Lore}}}}";
							if (!oldGameSpec.Content!.Contains(lorePage))
							{
								lorePage = $"{{{{:{monsterName.Replace(" ", "_")}/Lore}}}}";
							}
							content.AppendLine($@"{{{{Meta
|MetaTitle     = {monsterName}
|MetaDesc      = This page describes {monsterName}, which is a large monster appearing in the Monster Hunter franchise.
|MetaKeywords  = {monsterName}
|MetaImage     = {latestRender}
}}}}
{{{{MonsterAppearancesNav
|Monster = {monsterName}");
							foreach (string validGame in validGames)
							{
								content.AppendLine($"|{validGame} = Y");
							}
							content.AppendLine(@"}}");
							string gallerySection = monsterLore.Content!.Substring(monsterLore.Content!.IndexOf("=Gallery=")).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n").Replace("<gallery mode=\"slideshow\"", "<gallery mode=slideshow");
							gallerySection = gallerySection.Substring(0, gallerySection.IndexOf("<gallery mode=slideshow caption=\"Concept Art\">"));
							string renders = gallerySection.Substring(gallerySection.IndexOf("<gallery mode=slideshow caption=\"Official Renders\">\r\n") + 53, gallerySection.IndexOf("</gallery>") - gallerySection.IndexOf("<gallery mode=slideshow caption=\"Official Renders\">\r\n") - 53).Replace("|", "{{!}}");
							if (string.IsNullOrEmpty(renders.Trim()))
							{
								renders = latestRender;
							}
							infoBox = infoBox.Replace("MonsterInfoBoxv2", "MonsterGameInfoBox_Overview") + "|Renders = " + renders + "}}";
							StringBuilder newElements = new();
							newElements.Append(@"
|Elements = ");
							bool alreadyHas = false;
							foreach (string element in ElementTypes)
							{
								if (attacksSection.ToUpper().Replace($"{element.ToUpper()}BLIGHT", "").Contains(element.ToUpper()))
								{
									if (alreadyHas)
									{
										newElements.Append(" ");
									}
									newElements.Append($"{{{{UI|UI|{element}}}}}");
									alreadyHas = true;
								}
							}
							newElements.Append(@"
|Status Effects = ");
							alreadyHas = false;
							foreach (string element in StatusEffects)
							{
								if (attacksSection.Contains(element))
								{
									if (alreadyHas)
									{
										newElements.Append(" ");
									}
									newElements.Append($"{{{{Element|{element}}}}}");
									alreadyHas = true;
								}
							}
							infoBox = infoBox.Replace(attacksSection, newElements.ToString());
							string latestGame = infoBox.Substring(infoBox.IndexOf("|Latest Game ="));
							latestGame = latestGame.Substring(0, latestGame.IndexOf("\r\n")).TrimStart('\r', '\n').TrimEnd('\r', '\n').Trim();
							if (latestGame.Replace("|Latest Game =", "") == "")
							{
								string latestReplace = monsterRedirect.Content!.Substring(monsterRedirect!.Content.IndexOf($"#REDIRECT [[{monsterName}/") + 13 + monsterName.Length);
								latestReplace = latestReplace.Substring(0, latestReplace.IndexOf("]]")).Trim();
								infoBox = infoBox.Replace(latestGame, "|Latest Game = " + latestReplace);
							}
							string originalGame = infoBox.Substring(infoBox.IndexOf("|Original Game ="));
							originalGame = originalGame.Substring(0, originalGame.IndexOf("\r\n")).TrimStart('\r', '\n').TrimEnd('\r', '\n').Trim();
							if (originalGame.Replace("|Original Game =", "") == "")
							{
								string originalReplace = monsterRedirect.Content!.Substring(monsterRedirect!.Content.IndexOf("[[Category:Monsters introduced in") + 33);
								originalReplace = originalReplace.Substring(0, originalReplace.IndexOf("]]")).Trim();
								infoBox = infoBox.Replace(originalGame, "|Original Game = " + originalReplace);
							}
							content.AppendLine(infoBox);
							string generalInfo = monsterOverview.Content!.Substring(monsterOverview.Content!.IndexOf("{{MonsterInfoBoxv2")).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
							lastParam = generalInfo.Substring(generalInfo.LastIndexOf("\r\n|"));
							lastParam = lastParam.Substring(0, lastParam.IndexOf("}}"));
							generalInfo = generalInfo.Substring(generalInfo.IndexOf(lastParam) + lastParam.Length + 2);
							content.AppendLine(generalInfo.Substring(0, generalInfo.IndexOf("__TOC__")).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
							content.AppendLine("__TOC__");
							string summary = monsterOverview.Content!.Substring(monsterOverview.Content!.IndexOf("=Overview=") + 10).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
							content.AppendLine("= Summary =");
							content.AppendLine(summary.Substring(0, summary.IndexOf("<noinclude>")).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
							content.AppendLine("= Appearances =");
							string[] mainSeriesGames = [.. validGames.Where(x => MainSeriesAcros.Contains(x))];
							if (mainSeriesGames.Length > 0)
							{
								content.AppendLine("== Main Series ==");
								foreach (string mainSeriesGame in mainSeriesGames.Reverse())
								{
									content.AppendLine($"* '''''[[{GetGameFullName(mainSeriesGame)}]]''''': ???");
								}
							}
							string[] spinoffGames = [.. validGames.Where(x => SpinoffAcros.Contains(x))];
							if (spinoffGames.Length > 0)
							{
								content.AppendLine("== Spin-Offs ==");
								foreach (string spinoffGame in spinoffGames)
								{
									content.AppendLine($"* '''''[[{GetGameFullName(spinoffGame)}]]''''': ???");
								}
							}
							string newLore = monsterLore.Content!.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n").Replace("=Notes=", "=Trivia=").Replace("mode=\"slideshow\"", "mode=slideshow").Replace(gallerySection, "");
							newLore = newLore.Insert(newLore.IndexOf("<gallery"), @"=Gallery=
<div style=""max-width:500px; margin-left:auto; margin-right:auto;"">")
								.Replace("</gallery>\r\n</div>\r\n</div>", "</gallery></div>");
							if (allNames != null)
							{
								string oldLanguageNames = newLore.Substring(newLore.IndexOf("{{LanguageNames"));
								oldLanguageNames = oldLanguageNames.Substring(0, oldLanguageNames.IndexOf("}}"));
								string newLanguageNames = oldLanguageNames;
								string[] languageOrder = ["ENG", "FR", "ITA", "GER", "SPA", "RU", "POL", "POR", "KOR", "CHN", "ARA", "LAT"];
								foreach (string language in languageOrder)
								{
									string newLangString = "";
									if (language == "CHN")
									{
										string trad = GetNameFromLang(allNames, "CHNt");
										string simp = GetNameFromLang(allNames, "CHNs");
										if (trad != "???" && simp != "???")
										{
											newLangString = $"|{language} Name = {simp} <small>(Simplified)</small> / {trad} <small>(Traditional)</small>";
										}
									}
									else
									{
										string val = GetNameFromLang(allNames, language);
										if (val != "???")
										{
											newLangString = $"|{language} Name = {val}";
										}
									}
									if (oldLanguageNames.Contains($"|{language} Name ="))
									{
										string thisLanguageSubstr = oldLanguageNames.Substring(oldLanguageNames.IndexOf($"|{language} Name ="));
										thisLanguageSubstr = thisLanguageSubstr.Substring(0, thisLanguageSubstr.IndexOf("\r\n")).TrimStart('\r', '\n').TrimEnd('\r', '\n').Trim();
										if (language == "KOR")
										{
											if (!thisLanguageSubstr.Contains(" (") && !string.IsNullOrEmpty(newLangString))
											{
												newLanguageNames = newLanguageNames.Replace(thisLanguageSubstr, newLangString);
											}
										}
										else
										{
											newLanguageNames = newLanguageNames.Replace(thisLanguageSubstr, newLangString);
										}
									}
									else if (oldLanguageNames.Contains($"|{language} Name="))
									{
										string thisLanguageSubstr = oldLanguageNames.Substring(oldLanguageNames.IndexOf($"|{language} Name="));
										if (thisLanguageSubstr.Contains("/r/n"))
										{
											thisLanguageSubstr = thisLanguageSubstr.Substring(0, thisLanguageSubstr.IndexOf("\r\n"));
										}
										thisLanguageSubstr = thisLanguageSubstr.TrimStart('\r', '\n').TrimEnd('\r', '\n').Trim();
										if (language == "KOR")
										{
											if (!thisLanguageSubstr.Contains(" (") && !string.IsNullOrEmpty(newLangString))
											{
												newLanguageNames = newLanguageNames.Replace(thisLanguageSubstr, newLangString);
											}
										}
										else
										{
											newLanguageNames = newLanguageNames.Replace(thisLanguageSubstr, newLangString);
										}
									}
									else
									{
										newLanguageNames += $@"
{newLangString}";
									}
								}
								newLore = newLore.Replace(oldLanguageNames, newLanguageNames);
							}
							content.AppendLine(newLore.Replace(newLore.Substring(newLore.IndexOf("=Sources="), newLore.IndexOf("</noinclude>") + 12 - newLore.IndexOf("=Sources=")), ""));
							string finalCats = string.Join("\r\n", monsterRedirect.Content!.Replace("#REDIRECT", "")
								.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n")
								.Replace("\r\n\r\n", "\r\n")
								.Split("\r\n")
								.Where(x => x.StartsWith("[[Category:")));
							content.AppendLine($@"=Notes=
<references group=""notes""/>
=Sources=
<references />
{{{{MonsterListsBox}}}}
{{{{MonsterListBox}}}}
{finalCats}");
							if (!finishedOverviews.Contains(pageElements[0]))
							{
								await newOverview.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = content.ToString().Replace("\r\n\r\n\r\n", "\r\n\r\n"),
									Minor = false,
									Summary = $"Migrated from {pageElements[0]}/Overview to {pageElements[0]} as per Large Monster Page Redesign 2025.",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {newOverview.Title} created.");
								finishedOverviews.Add(pageElements[0]);
								File.WriteAllText(overviewsFile, JsonConvert.SerializeObject(finishedOverviews));
							}
							if (!finishedGameSpecs.Contains($"{pageElements[0]} ({pageGameAcronym})"))
							{
								content = new();
								string overviewPageName = $"{{{{:{monsterName}/Overview}}}}";
								string newMonsterName = monsterName;
								if (!oldGameSpec.Content!.Contains(overviewPageName))
								{
									overviewPageName = $"{{{{:{monsterName}/Overview";
								}
								if (!oldGameSpec.Content!.Contains(overviewPageName))
								{
									overviewPageName = $"{{{{:{newMonsterName}/Overview";
								}
								if (!oldGameSpec.Content!.Contains(overviewPageName))
								{
									newMonsterName = monsterName.Replace(" ", "_");
									overviewPageName = $"{{{{:{monsterName.Replace(" ", "_")}/Overview}}}}";
								}
								if (!oldGameSpec.Content!.Contains(overviewPageName))
								{
									overviewPageName = $"{{{{:{newMonsterName}/Overview";
								}
								string meta = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("{{Meta"), oldGameSpec.Content!.IndexOf(overviewPageName)).ReplaceLineEndings("\r\n");
								lastParam = meta.Substring(meta.LastIndexOf("\r\n|"));
								lastParam = lastParam.Substring(0, lastParam.IndexOf("}}"));
								meta = meta.Substring(0, meta.IndexOf(lastParam) + lastParam.Length + 2);
								content.AppendLine(meta);
								content.AppendLine($"<div class=subpages style=\"color: #54595d; font-size:0.95rem\">< <bdi dir=\"ltr\">[[{monsterName}]]</bdi></div>");
								content.AppendLine($@"{{{{MonsterAppearancesNav
|Monster = {monsterName}");
								foreach (string validGame in validGames)
								{
									content.AppendLine($"|{validGame} = Y");
								}
								content.AppendLine(@"}}");
								string newInfo = infoBox.Substring(0, infoBox.IndexOf("|Latest Game")).Replace("MonsterGameInfoBox_Overview", "MonsterGameInfoBox");
								newInfo += @"
|Game = " + pageGameAcronym;
								if (pageGameAcronym == "MHWilds")
								{
									newInfo += @"
|Capture HP % = 20
|Limp HP % = 15";
								}
								if (oldGameSpec.Content!.Contains("{{ItemEffectivenessTable"))
								{
									if (pageGameAcronym == "MHWilds")
									{
										if (JsonConvert.DeserializeObject<JArray>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Enemy\Em0002\00\Data\Em0002_00_Param_Feel.user.3.json"))!
											.First()
											.Value<JObject>("app.user_data.EmParamFeel")!
											.Value<JObject>("_ReactableSettingData")!
											.Value<JObject>("app.user_data.EmParamFeel.cReactableSettingData")!
											.Value<bool>("_IsReactableTrapMeat"))
											newInfo += @"
|Meat = Y";
									}
									else
									{
										string meat = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("|Meat Effectiveness =") + 21).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
										meat = meat.Substring(0, meat.IndexOf("\r\n")).Trim();
										if (meat != "" && meat != "False" && meat != "X")
										{
											newInfo += @"
|Meat = Y";
										}
									}
									string flash = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("|Flash Duration =") + 17).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
									flash = flash.Substring(0, flash.IndexOf("\r\n")).Trim();
									if (flash != "" && flash != "False" && flash != "X" && flash != "0" && int.TryParse(flash, out _))
									{
										newInfo += @"
|Flash Pods = Y";
									}
									string dung = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("|Dung Effectiveness =") + 21).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
									dung = dung.Substring(0, dung.IndexOf("\r\n")).Trim();
									if (dung != "" && dung != "False" && dung != "X" && dung != "0" && int.TryParse(dung, out _))
									{
										newInfo += @"
|Dung Pods = " + dung + "%";
									}
									string screamer = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("|Sonic Effectiveness =") + 22).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
									screamer = screamer.Substring(0, screamer.IndexOf("\r\n")).Trim();
									if (screamer != "" && screamer != "False" && screamer != "X" && screamer != "0")
									{
										newInfo += @"
|Screamer Pods = Y";
									}
								}
								if (oldGameSpec.Content!.Contains("{{CrownSizes"))
								{
									string crowns = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("{{CrownSizes")).Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
									crowns = crowns.Substring(0, crowns.IndexOf("}}"));
									string smallCrown = crowns.Substring(crowns.IndexOf("|Small Crown cm =") + 17);
									smallCrown = smallCrown.Substring(0, smallCrown.IndexOf("\r\n")).Trim();
									string average = crowns.Substring(crowns.IndexOf("|Average Size cm =") + 18);
									average = average.Substring(0, average.IndexOf("\r\n")).Trim();
									string silverCrown = crowns.Substring(crowns.IndexOf("|Silver Crown cm =") + 18);
									silverCrown = silverCrown.Substring(0, silverCrown.IndexOf("\r\n")).Trim();
									string goldCrown = crowns.Substring(crowns.IndexOf("|Gold Crown cm =") + 16);
									goldCrown = goldCrown.Substring(0, goldCrown.IndexOf("\r\n")).Trim();
									newInfo += $@"
|Gold Crown Small = {smallCrown}
|Average = {average}
|Silver Crown = {silverCrown}
|Gold Crown Large = {goldCrown}";
								}
								newInfo += @"
}}";
								content.AppendLine(newInfo);
								string? gameplayInfo = "=Gameplay Information=";
								if (!oldGameSpec.Content!.Contains(gameplayInfo))
								{
									if (oldGameSpec.Content!.Contains("= Gameplay Information ="))
									{
										gameplayInfo = "= Gameplay Information =";
									}
									else
									{
										gameplayInfo = null;
									}
								}
								if (gameplayInfo != null)
								{
									string fullGameInfo = "";
									if (oldGameSpec.Content!.Contains("==Crown Sizes=="))
									{
										fullGameInfo = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf(gameplayInfo), oldGameSpec.Content!.IndexOf("==Crown Sizes==") - oldGameSpec.Content!.IndexOf(gameplayInfo)).TrimStart('\r', '\n').TrimEnd('\r', '\n');
									}
									else if (oldGameSpec.Content!.Contains("== Damage Data =="))
									{
										fullGameInfo = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf(gameplayInfo), oldGameSpec.Content!.IndexOf("== Damage Data ==") - oldGameSpec.Content!.IndexOf(gameplayInfo)).TrimStart('\r', '\n').TrimEnd('\r', '\n');
									}
									else
									{
										fullGameInfo = oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf(gameplayInfo), oldGameSpec.Content!.IndexOf(lorePage) - oldGameSpec.Content!.IndexOf(gameplayInfo)).TrimStart('\r', '\n').TrimEnd('\r', '\n');
									}
									fullGameInfo = fullGameInfo.Replace("== Hunter's Notes ==", "==Hunter's Notes==");
									if (fullGameInfo.Contains("==Hunter's Notes=="))
									{
										string hunterNotes = fullGameInfo.Substring(fullGameInfo.IndexOf("==Hunter's Notes=="));
										hunterNotes = hunterNotes.Substring(0, hunterNotes.IndexOf("}}") + 2);
										fullGameInfo = fullGameInfo.Replace(hunterNotes, "<div style=\"display: flex;flex-direction: column;\">\r\n" + hunterNotes + "\r\n</div>");
									}
									content.AppendLine(fullGameInfo);
								}
								content.AppendLine("==Physical Attributes==");
								if (oldGameSpec.Content!.Contains("==Parts and Hitzones=="))
								{
									content.AppendLine(oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("==Parts and Hitzones==") + 22, oldGameSpec.Content!.IndexOf($"{lorePage}") - 22 - oldGameSpec.Content!.IndexOf("==Parts and Hitzones==")).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
								}
								else if (oldGameSpec.Content!.Contains("== Damage Data =="))
								{
									content.AppendLine(oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("== Damage Data ==") + 17, oldGameSpec.Content!.IndexOf($"{lorePage}") - 17 - oldGameSpec.Content!.IndexOf("== Damage Data ==")).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
								}
								else if (oldGameSpec.Content!.Contains("==Physical Attributes=="))
								{
									content.AppendLine(oldGameSpec.Content!.Substring(oldGameSpec.Content!.IndexOf("==Physical Attributes==") + 23, oldGameSpec.Content!.IndexOf($"{lorePage}") - 23 - oldGameSpec.Content!.IndexOf("==Physical Attributes==")).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
								}
								content.AppendLine(@"=Sources=
<references />
{{MonsterListsBox}}
{{MonsterListBox}}");
								if (oldGameSpec.Content!.Contains("<noinclude>"))
								{
									content.AppendLine(oldGameSpec.Content!.Substring(oldGameSpec.Content!.LastIndexOf("<noinclude>"), oldGameSpec.Content!.LastIndexOf("</noinclude>") + 12 - oldGameSpec.Content!.LastIndexOf("<noinclude>")).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
								}
								else
								{
									content.AppendLine(oldGameSpec.Content!.Substring(oldGameSpec.Content!.LastIndexOf("/Lore}}") + 7).TrimStart('\r', '\n').TrimEnd('\r', '\n'));
								}
								WikiPage newGameSpec = new(site, $"{monsterName} ({pageGameAcronym})"/*$"User:RampageRobot/Sandbox/{monsterName} ({pageGameAcronym})"*/);
								await newGameSpec.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = content.ToString().Replace("\r\n\r\n\r\n", "\r\n\r\n"),
									Minor = false,
									Summary = $"Migrated from {pageElements[0]}/{pageGameAcronym} to {pageElements[0]} ({pageGameAcronym}) as per Large Monster Page Redesign 2025.",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {newGameSpec.Title} created.");
								finishedGameSpecs.Add($"{pageElements[0]} ({pageGameAcronym})");
								File.WriteAllText(gameSpecsFile, JsonConvert.SerializeObject(finishedGameSpecs));
							}
						}
						else if (!oldGameSpec.Exists || pageGameAcronym == "MH1")
						{
							await monsterRedirect.RefreshAsync(PageQueryOptions.FetchContent);
							string appearancesNav = @$"{{{{MonsterAppearancesNav
|Monster = {monsterName}
{string.Join("\r\n", monsterAppearances[monsterName].GameAppearances.Select(x => $"|{x.GameAcronym} = Y"))}
}}}}";
							string languages = $@"{{{{LanguageNames
|ENG Name = {monsterName}
|JP Name =
|FR Name =
|GER Name =
|SPA Name =
|KOR Name =
|RU Name =
|CHN Name =
|ITA Name =
|POL Name =
|POR Name =
|ARA Name =
|LAT Name =
|Etymology = 
}}}}";
							if (monsterNameDict.TryGetValue(monsterName, out MonsterNames? names))
							{
								languages = $@"{{{{LanguageNames
|ENG Name = {names.English}
|JP Name = {names.Japanese}
|FR Name = {names.French}
|GER Name = {names.German}
|SPA Name = {names.Spanish}
|KOR Name = {names.Korean}
|RU Name = {names.Russian}
|CHN Name = {names.SimplifiedChinese} <small>(Simplified)</small> / {names.TraditionalChinese} <small>(Traditional)</small>
|ITA Name = {names.Italian}
|POL Name = {names.Polish}
|POR Name = {names.PortugueseBr}
|ARA Name = {names.Arabic}
|LAT Name = {names.LatinAmericanSpanish}
|Etymology = 
}}}}";
							}
							if (!finishedOverviews.Contains(pageElements[0]))
							{
								await monsterRedirect.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = await GetDataLessPage(monsterAppearances[monsterName], site),
									Minor = false,
									Summary = $"Created data-less overview page via API call..",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {monsterOverview.Title} created.");
								finishedOverviews.Add(pageElements[0]);
								File.WriteAllText(overviewsFile, JsonConvert.SerializeObject(finishedOverviews));
							}
							if (!finishedGameSpecs.Contains($"{pageElements[0]} ({pageGameAcronym})"))
							{
								WikiPage newGameSpec = new(site, $"{monsterName} ({pageGameAcronym})");
								await newGameSpec.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = await GetDataLessPage(monsterAppearances[monsterName], site, monsterAppearances[monsterName].GameAppearances.First(x => x.GameAcronym == pageGameAcronym)),
									Minor = false,
									Summary = $"Created data-less game-specific page via API call.",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {newGameSpec.Title} created.");
								finishedGameSpecs.Add($"{pageElements[0]} ({pageGameAcronym})");
								File.WriteAllText(gameSpecsFile, JsonConvert.SerializeObject(finishedGameSpecs));
							}
						}
						cnt++;
					}
					Console.WriteLine("======================================================");
					Console.WriteLine("Finished!");
					TimeSpan elapsed = DateTime.Now - start;
					Console.WriteLine("Elapsed: " + elapsed.ToString());
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		private static string GetNameFromLang(MonsterNames monsterNames, string lang)
		{
			switch (lang)
			{
				case "ENG":
					return monsterNames.English;
				case "FR":
					return monsterNames.French;
				case "ITA":
					return monsterNames.Italian;
				case "GER":
					return monsterNames.German;
				case "SPA":
					return monsterNames.Spanish;
				case "RU":
					return monsterNames.Russian;
				case "POL":
					return monsterNames.Polish;
				case "POR":
					return monsterNames.PortugueseBr;
				case "KOR":
					return monsterNames.Korean;
				case "CHNt":
					return monsterNames.TraditionalChinese;
				case "CHNs":
					return monsterNames.SimplifiedChinese;
				case "ARA":
					return monsterNames.Arabic;
				case "LAT":
					return monsterNames.LatinAmericanSpanish;
				default: return "???";

			}
		}

		public static async Task UpdateMHWildsWeaponRenderCategoryBanner()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					string[] overCats = ["Category:MHWilds_Weapon_Renders"];
					foreach (string cat in overCats)
					{
						CategoryMembersGenerator subpageGen = new(site, cat!)
						{
							MemberTypes = CategoryMemberTypes.Subcategory
						};
						WikiPage[] categories = await subpageGen.EnumPagesAsync().ToArrayAsync();
						int cnt = 1;
						foreach (WikiPage category in categories.Where(x => !x.Title!.Contains("Sandbox")))
						{
							CategoryMembersGenerator subPageGen2 = new(site, category.Title!)
							{
								MemberTypes = CategoryMemberTypes.File
							};
							WikiPage[] subCats = await subPageGen2.EnumPagesAsync().ToArrayAsync();
							int totalCnt = subCats.Count(x => !x.Title!.Contains("Sandbox"));
							foreach (WikiPage page in subCats)
							{
								await page.RefreshAsync(PageQueryOptions.FetchContent);
								if (page.Exists)
								{
									string oldContent = page.Content!;
									string newContent = @"{{CustomRenderNotice
|Filetype1=Renders
|Filetype2=page
|Filetype3=3D art
|user=PB472 (on Discord)
|permission=yes
}}
" + oldContent;
									await page.EditAsync(new WikiPageEditOptions()
									{
										Bot = true,
										Content = newContent,
										Minor = true,
										Summary = "Added custom render notice",
										Watch = AutoWatchBehavior.None
									});
									Console.WriteLine($"({cnt}/{totalCnt}) {page.Title} edited.");
									cnt++;
								}
							}
							Console.WriteLine($"Updated category {category}.");
						}
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task DosPlatyDirect()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					Dictionary<string, MonsterData> monsterAppearances = JsonConvert.DeserializeObject<Dictionary<string, MonsterData>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterData.json"))!;
					WikiPage[] res = [.. monsterAppearances.Where(x => x.Value.GameAppearances.Any(y => y.GameAcronym == "MH2")).Select(x => new WikiPage(site, $"{x.Key} (MH2)"))];
					int cnt = 1;
					foreach (WikiPage page in res)
					{
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							string newContent = page.Content!;
							string navStr = newContent.Substring(newContent.IndexOf("{{MonsterAppearancesNav"));
							navStr = navStr.Substring(0, navStr.IndexOf("}}") + 2);
							newContent = newContent.Insert(newContent.IndexOf(navStr) + navStr.Length, "\r\n{{Anotherwiki|gametype=Dos}}");
							await page.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = newContent,
								Minor = true,
								Summary = "Added custom render notice",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cnt}/{res.Length}) {page.Title} edited.");
							cnt++;
						}
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task UpdateEndemicLifeMetaDescs()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					string[] overCats = ["Category:MHWilds_Endemic_Life", "Category:MHRise_Endemic_Life", "Category:MHWI_Endemic_Life", "Category:MHWorld_Endemic_Life"];
					foreach (string cat in overCats)
					{
						CategoryMembersGenerator subpageGen = new(site, cat!)
						{
							MemberTypes = CategoryMemberTypes.Page
						};
						WikiPage[] monsters = await subpageGen.EnumPagesAsync().ToArrayAsync();
						int cnt = 1;
						int totalCnt = monsters.Count(x => !x.Title!.Contains("Sandbox"));
						foreach (WikiPage monster in monsters.Where(x => !x.Title!.Contains("Sandbox")))
						{
							await monster.RefreshAsync(PageQueryOptions.FetchContent);
							if (monster.Exists)
							{
								string oldContent = monster.Content!;
								string appearances = monster.Content![(monster.Content!.IndexOf("|Appearances") + 12)..];
								appearances = appearances[..appearances.IndexOf("\n")].Replace("=", "").Replace("\r", "").Replace("\r\n", "").Trim().Replace("<br>", ",");
								string[] appList = appearances.Split(",");
								string metaDesc = $"This page describes the {monster.Title!}, a species of endemic life that appears in ";
								int cntr = 1;
								foreach (string appearance in appList)
								{
									if (cntr > 1)
									{
										if (cntr > 2)
										{
											metaDesc += ", ";
										}
										if (cntr == appList.Length - 1)
										{
											metaDesc += " and ";
										}
									}
									string appGame = appearance.Substring(appearance.IndexOf('|') + 1, appearance.IndexOf("]]") - appearance.IndexOf('|') - 1);
									metaDesc += $"{GetGameFullName(appGame)} ({appGame})";
								}
								metaDesc += ". In this page is information about its size, habitats and zones, valid placements (if applicable), combat utility (if applicable), notes, ecology, lore, behavior, images, other appearances, and more.";
								string newContent = "";
								if (!oldContent.Contains($"|MetaDescription   = {metaDesc}"))
								{
									if (oldContent.Contains("{{EndemicLifeInfobox"))
									{
										if (oldContent.Contains("|MetaDescription"))
										{
											newContent = Regex.Replace(oldContent, "\\|MetaDesc.*\r\n", $"|MetaDescription   = {metaDesc}\r\n", RegexOptions.IgnoreCase);
											if (!newContent.Contains($"|MetaDescription   = {metaDesc}\r\n"))
											{
												newContent = Regex.Replace(oldContent, "\\|MetaDesc.*\n", $"|MetaDescription   = {metaDesc}\r\n", RegexOptions.IgnoreCase);
											}
										}
										else
										{
											newContent = oldContent.Insert(oldContent.IndexOf("{{EndemicLifeInfobox") + 20, $"\r\n|MetaDescription   = {metaDesc}");
										}
									}
								}
								else
								{
									newContent = oldContent;
								}
								await monster.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = newContent,
									Minor = true,
									Summary = "Modified metadata",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {monster.Title} edited.");
								cnt++;
							}
						}
						Console.WriteLine($"Updated category {cat}.");
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task UpdateSmallMonsterMetaDescs()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					string cat = "Category:Small_Monster_Game_Subpages";
					CategoryMembersGenerator generator = new(site, cat)
					{
						MemberTypes = CategoryMemberTypes.Subcategory
					};
					WikiPage[] items = await generator.EnumPagesAsync().ToArrayAsync();
					foreach (WikiPage page in items)
					{
						CategoryMembersGenerator subpageGen = new(site, page.Title!)
						{
							MemberTypes = CategoryMemberTypes.Page
						};
						WikiPage[] monsters = await subpageGen.EnumPagesAsync().ToArrayAsync();
						int cnt = 1;
						int totalCnt = monsters.Count(x => !x.Title!.Contains("Sandbox"));
						foreach (WikiPage monster in monsters.Where(x => !x.Title!.Contains("Sandbox")))
						{
							await monster.RefreshAsync(PageQueryOptions.FetchContent);
							if (monster.Exists)
							{
								string oldContent = monster.Content!;
								string[] pageElements = monster.Title!.Split('/');
								string fullGameName = GetGameFullName(pageElements[1]);
								if (fullGameName == "Game Unknown!")
								{
									Debugger.Break();
								}
								WikiPage monsterOverview = new(site, $"{pageElements[0]}/Overview");
								await monsterOverview.RefreshAsync(PageQueryOptions.FetchContent);
								string classification = monsterOverview.Content![(monsterOverview.Content!.IndexOf("|Class") + 6)..];
								if (classification.Contains("\r\n"))
								{
									classification = classification[..classification.IndexOf("\r\n")];
								}
								else
								{
									classification = classification[..classification.IndexOf("\n")];
								}
								classification = classification.Replace("[", "").Replace("]", "").Replace("=", "").Trim();
								string originalGame = monsterOverview.Content![(monsterOverview.Content!.IndexOf("|Original Game") + 14)..];
								if (originalGame.Contains("\r\n"))
								{
									originalGame = originalGame[..originalGame.IndexOf("\r\n")];
								}
								else
								{
									originalGame = originalGame[..originalGame.IndexOf("\n")];
								}
								originalGame = originalGame.Replace("[", "").Replace("]", "").Replace("=", "").Trim();
								string metaDesc = $"{pageElements[0]} is a {classification} small monster appearing in {fullGameName} ({pageElements[1]}). Described here are its weaknesses, elements, attacks, armor, weapons, parts, drops and drop rates, materials, crown sizes, ecology, lore, history, images, notes, and more.";
								string newContent = "";
								if (!oldContent.Contains($"|MetaDesc      = {metaDesc}"))
								{
									if (oldContent.Contains("{{Meta"))
									{
										if (oldContent.Contains("|MetaDesc"))
										{
											newContent = Regex.Replace(oldContent, "\\|MetaDesc.*\r\n", $"|MetaDesc      = {metaDesc}\r\n", RegexOptions.IgnoreCase);
											if (!newContent.Contains($"|MetaDesc      = {metaDesc}\r\n"))
											{
												newContent = Regex.Replace(oldContent, "\\|MetaDesc.*\n", $"|MetaDesc      = {metaDesc}\r\n", RegexOptions.IgnoreCase);
											}
										}
										else
										{
											newContent = oldContent.Insert(oldContent.IndexOf("{{Meta") + 6, $"\r\n|MetaDesc      = {metaDesc}");
										}
									}
									else
									{
										newContent = oldContent.Insert(0, $@"{{{{Meta
|MetaTitle     = {pageElements[0]} - {pageElements[1]}
|MetaDesc      = {metaDesc}
|MetaKeywords  = {pageElements[0]}, {fullGameName}, Stats, Materials, Attacks
|MetaImage     = {pageElements[1]}-{pageElements[0]} Render.png
}}}}");
									}
								}
								else
								{
									newContent = oldContent;
								}
								List<string> catsToAdd =
								[
									"[[Category:Large Monsters]]",
									$"[[Category:Large Monster {pageElements[1]} Subpages]]",
									$"[[Category:{classification}]]",
									$"[[Category:Monsters introduced in {originalGame}]]"
								];
								string catString = "";
								foreach (string catToAdd in catsToAdd)
								{
									if (!newContent.Contains(catToAdd))
									{
										if (!string.IsNullOrEmpty(catString))
										{
											catString += "\r\n";
										}
										catString += catToAdd;
									}
								}
								if (!string.IsNullOrEmpty(catString))
								{
									newContent += $@"
<noinclude>
{catString}
</noinclude>";
								}
								await monster.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = newContent,
									Minor = true,
									Summary = "Modified metadata",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"({cnt}/{totalCnt}) {monster.Title} edited.");
								cnt++;
							}
						}
						Console.WriteLine($"Updated category {page.Title}.");
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task UpdateLargeMonsterMetaDescs()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					string[] overCats = ["Category:Monster_Game_Subpages", "Category:Large_Monster_Game_Subpages"];
					foreach (string cat in overCats)
					{
						CategoryMembersGenerator generator = new(site, cat)
						{
							MemberTypes = CategoryMemberTypes.Subcategory
						};
						WikiPage[] items = await generator.EnumPagesAsync().ToArrayAsync();
						foreach (WikiPage page in items)
						{
							CategoryMembersGenerator subpageGen = new(site, page.Title!)
							{
								MemberTypes = CategoryMemberTypes.Page
							};
							WikiPage[] monsters = await subpageGen.EnumPagesAsync().ToArrayAsync();
							int cnt = 1;
							int totalCnt = monsters.Count(x => !x.Title!.Contains("Sandbox"));
							foreach (WikiPage monster in monsters.Where(x => !x.Title!.Contains("Sandbox")))
							{
								await monster.RefreshAsync(PageQueryOptions.FetchContent);
								if (monster.Exists)
								{
									string oldContent = monster.Content!;
									string[] pageElements = monster.Title!.Split('/');
									string fullGameName = GetGameFullName(pageElements[1]);
									if (fullGameName == "Game Unknown!")
									{
										Debugger.Break();
									}
									WikiPage monsterOverview = new(site, $"{pageElements[0]}/Overview");
									await monsterOverview.RefreshAsync(PageQueryOptions.FetchContent);
									string classification = monsterOverview.Content![(monsterOverview.Content!.IndexOf("|Class") + 6)..];
									if (classification.Contains("\r\n"))
									{
										classification = classification[..classification.IndexOf("\r\n")];
									}
									else
									{
										classification = classification[..classification.IndexOf("\n")];
									}
									classification = classification.Replace("[", "").Replace("]", "").Replace("=", "").Trim();
									string originalGame = monsterOverview.Content![(monsterOverview.Content!.IndexOf("|Original Game") + 14)..];
									if (originalGame.Contains("\r\n"))
									{
										originalGame = originalGame[..originalGame.IndexOf("\r\n")];
									}
									else
									{
										originalGame = originalGame[..originalGame.IndexOf("\n")];
									}
									originalGame = originalGame.Replace("[", "").Replace("]", "").Replace("=", "").Trim();
									string metaDesc = $"This page describes {pageElements[0]}, which is a large monster appearing in {fullGameName} ({pageElements[1]}). Described here are its weaknesses, elements, attacks, armor, weapons, hitzones (HZVs), parts, drops and drop rates, materials, crown sizes, ecology, lore, history, images, notes, and more.";
									string newContent = "";
									if (!oldContent.Contains($"|MetaDesc      = {metaDesc}"))
									{
										if (oldContent.Contains("{{Meta"))
										{
											if (oldContent.Contains("|MetaDesc"))
											{
												newContent = Regex.Replace(oldContent, "\\|MetaDesc.*\r\n", $"|MetaDesc      = {metaDesc}\r\n", RegexOptions.IgnoreCase);
												if (!newContent.Contains($"|MetaDesc      = {metaDesc}\r\n"))
												{
													newContent = Regex.Replace(oldContent, "\\|MetaDesc.*\n", $"|MetaDesc      = {metaDesc}\r\n", RegexOptions.IgnoreCase);
												}
											}
											else
											{
												newContent = oldContent.Insert(oldContent.IndexOf("{{Meta") + 6, $"\r\n|MetaDesc      = {metaDesc}");
											}
										}
										else
										{
											newContent = oldContent.Insert(0, $@"{{{{Meta
|MetaTitle     = {pageElements[0]} - {pageElements[1]}
|MetaDesc      = {metaDesc}
|MetaKeywords  = {pageElements[0]}, {fullGameName}, Stats, Materials, Attacks
|MetaImage     = {pageElements[1]}-{pageElements[0]} Render.png
}}}}");
										}
									}
									else
									{
										newContent = oldContent;
									}
									List<string> catsToAdd =
									[
										"[[Category:Large Monsters]]",
										$"[[Category:Large Monster {pageElements[1]} Subpages]]",
										$"[[Category:{classification}]]",
										$"[[Category:Monsters introduced in {originalGame}]]"
									];
									string catString = "";
									foreach (string catToAdd in catsToAdd)
									{
										if (!newContent.Contains(catToAdd))
										{
											if (!string.IsNullOrEmpty(catString))
											{
												catString += "\r\n";
											}
											catString += catToAdd;
										}
									}
									if (!string.IsNullOrEmpty(catString))
									{
										newContent += $@"
<noinclude>
{catString}
</noinclude>";
									}
									await monster.EditAsync(new WikiPageEditOptions()
									{
										Bot = true,
										Content = newContent,
										Minor = true,
										Summary = "Modified metadata",
										Watch = AutoWatchBehavior.None
									});
									Console.WriteLine($"({cnt}/{totalCnt}) {monster.Title} edited.");
									cnt++;
								}
							}
							Console.WriteLine($"Updated category {page.Title}.");
						}
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		private static string GetGameFullName(string game)
		{
			switch (game)
			{
				case "MH1": return "Monster Hunter";
				case "MHG": return "Monster Hunter G";
				case "MHF1": return "Monster Hunter Freedom";
				case "MH2": return "Monster Hunter 2";
				case "MHF2": return "Monster Hunter Freedom 2";
				case "MHFU": return "Monster Hunter Freedom Unite";
				case "MH3": return "Monster Hunter 3";
				case "MHP3": return "Monster Hunter Portable 3rd";
				case "MH3U": return "Monster Hunter 3 Ultimate";
				case "MH4": return "Monster Hunter 4";
				case "MH4U": return "Monster Hunter 4 Ultimate";
				case "MHGen": return "Monster Hunter Generations";
				case "MHGU": return "Monster Hunter Generations Ultimate";
				case "MHWorld": return "Monster Hunter: World";
				case "MHWI": return "Monster Hunter World: Iceborne";
				case "MHRise": return "Monster Hunter Rise";
				case "MHRS": return "Monster Hunter Rise: Sunbreak";
				case "MHWilds": return "Monster Hunter Wilds";
				case "MHNow": return "Monster Hunter Now";
				case "MHOutlanders": return "Monster Hunter Outlanders";
				case "MHPuzzles": return "Monster Hunter Puzzles: Felyne Isles";
				case "MHST1": return "Monster Hunter Stories";
				case "MHST2": return "Monster Hunter Stories 2: Wings of Ruin";
				case "MHST3": return "Monster Hunter Stories 3: Twisted Reflection";
				case "MHFrontier": return "Monster Hunter Frontier";
				case "MHOnline": return "Monster Hunter Online";
				case "MHExplore": return "Monster Hunter Explore";
				case "MHi": return "Monster Hunter i";
				case "MHRiders": return "Monster Hunter Riders";
				case "MHDiary": return "Monster_Hunter_Diary: Poka Poka Airou Village";
				case "MHDG": return "Monster_Hunter_Diary: Poka Poka Airou Village G";
				case "MHDDX": return "Monster_Hunter_Diary: Poka Poka Airou Village Deluxe";
				case "MHBGHQ": return "Monster Hunter Big Game Hunting Quest";
				case "MHDH": return "Monster Hunter Dynamic Hunting";
				case "MHMH": return "Monster Hunter Massive Hunting";
				case "MHPIV": return "Monster Hunter: Phantom Island Voyage";
				case "MHSpirits": return "Monster Hunter Spirits";
				case "MHGii": return "Monster Hunter G (Wii)";
				case "MHP1": return "Monster Hunter Portable (PSP)";
				case "MHP2": return "Monster Hunter Portable 2nd (PSP)";
				case "MHP2G": return "Monster Hunter Portable 2nd G (PSP)";
				case "MH3G": return "Monster Hunter 3 G (Wii U, 3DS)";
				case "MH4G": return "Monster Hunter 4G (3DS)";
				case "MHX": return "Monster Hunter X (3DS)";
				case "MHXX": return "Monster Hunter XX (3DS, Nintendo Switch)";
				default:
					return "Game Unknown!";
			}
		}

		public static async Task DeleteAllInDeleteCategory()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					CategoryMembersGenerator generator = new(site, $"Category:Pages marked for deletion")
					{
						MemberTypes = CategoryMemberTypes.All
					};
					WikiPage[] items = await generator.EnumPagesAsync().ToArrayAsync();
					foreach (WikiPage page in items)
					{
						await page.DeleteAsync("Page was in Marked For Deletion category, and was auto-deleted by the MHWikiToolkit.");
						Console.WriteLine($"Deleted page {page.Title}");
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		private static async Task<string?> GetSectionId(WikiSite site, string page, string sectionHeading)
		{
			ParsedContentInfo parsedInfo = await site.ParsePageAsync(page);
			return parsedInfo.Sections.FirstOrDefault(x => x.Heading == sectionHeading)?.Index;
		}

		public static async Task FixMonsterPageQuests()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					int cntr = 1;
					Dictionary<string, string>[] objInfo = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(File.ReadAllText($@"{System.Configuration.ConfigurationManager.AppSettings.Get("DesktopPath")}\test monster stuff\MHWI\questObjectiveTypes.json"))!;
					CategoryMembersGenerator subpageGen = new(site, "Large_Monster_MHWI_Subpages")
					{
						MemberTypes = CategoryMemberTypes.Page
					};
					Models.Data.MHWI.Quests[] quests = Models.Data.MHWI.Quests.FetchQuests();
					Models.Monsters.Quests[] questInfos = Models.Monsters.Quests.FetchQuests();
					var test = quests.Select(x => x.QuestName).Where(x => !questInfos.Any(y => y.Name == x)).ToArray();
					WikiPage[] monsters = await subpageGen.EnumPagesAsync().ToArrayAsync();
					int totalCnt = monsters.Count(x => !x.Title!.Contains("Sandbox"));
					foreach (WikiPage monster in monsters.Where(x => !x.Title!.Contains("Sandbox")))
					{
						await monster.RefreshAsync(PageQueryOptions.FetchContent);
						if (monster.Content!.Contains("==Quests Targeting This Monster=="))
						{
							int questIndex = monster.Content!.IndexOf("==Quests Targeting This Monster==");
							string oldQuests = monster.Content!.Substring(questIndex, monster.Content!.IndexOf("==Equipment==") - questIndex);
							StringBuilder sb = new();
							sb.AppendLine(@"==Quests Targeting This Monster==
{| class=""wikitable mobile-sm sortable"" style=""text-align:center;margin:auto""
!Rank
!Type
!Name
!Locale
!Targets
|-");
							string[] elderDragons = ["Behemoth", "Kirin", "Kulve Taroth", "Kushala Daora", "Lunastra", "Nergigante", "Teostra", "Vaal Hazak", "Xeno'jiiva", "Zorah Magdaros", "Alatreon", "Namielle", "Ruiner Nergigante", "Safi'jiiva", "Shara Ishvalda", "Blackveil Vaal Hazak", "Velkhana", "Fatalis"];
							foreach (Models.Data.MHWI.Quests quest in quests.OrderBy(x => x.RankLevel + (x.Rank == "Master Rank" ? 10 : 0)).Where(x => x.TargetMonsters.Any(y => y == monster.Title!.Split('/')[0])))
							{
								string rankAbbr = quest.Rank == "Low Rank" ? "LR" : quest.Rank == "High Rank" ? "HR" : "MR";
								string objectiveIconType = quest.QuestType!;
								string assignmentType = objInfo.First(x => quest.Id >= Convert.ToInt32(x["ID >="]) && quest.Id <= Convert.ToInt32(x["ID <="]))["Type"];
								List<string> allObjectivesList = [];
								if (quest.QuestType != "Deliver")
								{
									string goalIcons = "";
									int targetCntr = 1;
									foreach (string monsterName in quest.TargetMonsters!.Select(x => x!))
									{
										goalIcons += $"{{{{IconPickerUniversalAlt|MHWI|{monsterName}";
										if (quest.MonsterSpecialStates.Length > targetCntr - 1)
										{
											string? specialState = quest.MonsterSpecialStates[targetCntr - 1];
											if (!string.IsNullOrEmpty(specialState))
											{
												goalIcons += $"|Type={specialState.Replace("Arch-Tempered", "Arch Tempered")}";
											}
										}
										goalIcons += "|Size=64|Text=|Margin=0|ML=Y}}";
										targetCntr++;
									}
									allObjectivesList.Add(goalIcons);
								}
								string[] allObjectives = [.. allObjectivesList];
								sb.AppendLine($@"|data-sort-value=""{quest.RankLevel + (rankAbbr == "MR" ? 10 : 0)}""|{rankAbbr} {quest.RankLevel}'''★'''
|{(string.IsNullOrEmpty(assignmentType) ? "???" : assignmentType)}
|{{{{UI|UI|{objectiveIconType}|title={objectiveIconType}|nolink=true}}}} [[{quest.QuestName} (MHWI Quest)|{quest.QuestName}]]
|{quest.Locale}
|data-sort-value=""{allObjectives.Length}""|{(allObjectives.Length > 0 ? string.Join("", allObjectives) : "None")}
|-");
							}
							sb.AppendLine("|}");
							string newContent = monster.Content!.Replace(oldQuests, sb.ToString());
							await monster.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = newContent,
								Minor = true,
								Summary = "Changes to Quest Targeting section",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cntr}/{totalCnt}) {monster} edited.");
						}
						else
						{
							Console.WriteLine($"({cntr}/{totalCnt}) {monster} had no quests.");
						}
						cntr++;
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task UploadMHWITitles()
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					Titles[] allTitles = Titles.FetchAllTitles();
					WikiPage page = new(site, $"MHWI/Titles");
					await page.RefreshAsync(PageQueryOptions.FetchContent);
					StringBuilder content = new();
					string[] firstSection = ["Unlocked", "Read_HR"];
					string[] secondSection = ["Story_Unlock", "Arena_Count", "Unk_14", "Unk_7", "Unk_1"]; //unlockParam < 1200000 and unk_1 where Description.Contains("<STYL MOJI_LIGHTBLUE_DEFAULT>")
					string[] thirdSection = ["Unk_10", "Unk_8", "Unk_12"];
					//third section is hunting milestones where name not in iceborneMonsters
					string[] fourthSection = ["Reach_MR"];
					string[] fifthSection = ["Story_Unlock"]; //unlockParam >= 1200000
															  //sixth section is hunting milestones where name in iceborneMonsters
					string[] miscMilestones = ["Unk_1", "Unk_8"]; //where !starts with hunted, captured, or slayed, where !Description.Contains("<STYL MOJI_LIGHTBLUE_DEFAULT>")
					string[] downloadable = ["Unk_11", "Unk_12"]; //where description = "Available via download"
					string[] eventCollabTitles = ["Slayed the Behemoth", "Obtain Bayek Layered Armor or Assassin's Hood.", "Completed the quest \"To Our World\"", "Completed <STYL MOJI_LIGHTBLUE_DEFAULT>A Visitor From Eorzea (Extreme)</STYL>", "Solved all of the mysteries in <STYL MOJI_LIGHTBLUE_DEFAULT>Contract: Trouble in the Ancient Forest</STYL>"];
					string lastType = "";
					List<string> thisTypeTitles = [];
					string[] baseMonsters = [..new string[] {
				"None", "Anjanath" , "Rathalos" , "[s] Aptonoth" , "[s] Jagras" , "Zorah Magdaros" , "[s] Mosswine" , "[s] Gajau" , "Great Jagras" , "[s] Kestodon M" , "Rathian" , "Pink Rathian" , "Azure Rathalos" , "Diablos" ,
			"Black Diablos" , "Kirin" , "Behemoth" , "Kushala Daora" , "Lunastra" , "Teostra" , "Lavasioth" , "Deviljho" , "Barroth" , "Uragaan" , "Leshen" , "Pukei-Pukei" , "Nergigante" , "Xeno'jiiva" , "Kulu-Ya-Ku" , "Tzitzi-Ya-Ku" ,
			"Jyuratodus" , "Tobi-Kadachi" , "Paolumu" , "Legiana" , "Great Girros" , "Odogaron" , "Radobaan" , "Vaal Hazak" , "Dodogama" , "Kulve Taroth" , "Bazelgeuse" , "[s] Apceros" , "[s] Kelbi M" , "[s[ Kelbi F" , "[s] Hornetaur" ,
			"[s] Vespoid" , "[s] Mernos" , "[s] Kestodon F" , "[s] Raphinos" , "[s] Shamos" , "[s] Barnos" , "[s] Girros" , "Ancient Leshen" , "[s] Gastodon" , "[s] Noios" , "[s] Magmacore 1" , "[s] Magmacore 2" , "[s] Gajalaka" ,
			"[s] Small Barrel" , "[s] Large Barrel" , "[s] Training Pole" , "NON-VALID" , "Tigrex (IB)" , "Nargacuga (IB)" , "Barioth (IB)" , "Savage Deviljho (IB)" , "Brachydios (IB)" , "Glavenus (IB)" , "Acidic Glavenus (IB)" ,
			"Fulgur Anjanath (IB)" , "Coral Pukei-Pukei (IB)" , "Ruiner Nergigante (IB)" , "Viper Tobi-Kadachi (IB)" , "Nightshade Paolumu (IB)" , "Shrieking Legiana (IB)" , "Ebony Odogaron (IB)" , "Blackveil Vaal Hazak (IB)" ,
			"Seething Bazelgeuse (IB)" , "Beotodus (IB)" , "Banbaro (IB)" , "Velkhana (IB)" , "Namielle (IB)" , "Shara Ishvalda (IB)" , "[s] Popo (IB)" , "[s] Anteka (IB)" , "[s] Wulg (IB)" , "[s] Cortos (IB)" , "[s] Boaboa (IB)" ,
			"Alatreon (IB)" , "Gold Rathian (IB)" , "Silver Rathalos (IB)" , "Yian Garuga (IB)" , "Rajang (IB)" , "Furious Rajang (IB)" , "Brute Tigrex (IB)" , "Zinogre (IB)" , "Stygian Zinogre (IB)" , "Raging Brachydios (IB)" , "Safi'Jiiva (IB)" , "[s] Wood Dummy (IB)" ,
			"Scarred Yian Garuga (IB)","Frostfang Barioth (IB)", "Fatalis (IB)"}.Where(x => !x.EndsWith(" (IB)")).Select(x => x.Replace("[s] ", "").Replace(" (IB)", "").Trim())];
					string[] ibMonsters = [..new string[] {
				"None", "Anjanath" , "Rathalos" , "[s] Aptonoth" , "[s] Jagras" , "Zorah Magdaros" , "[s] Mosswine" , "[s] Gajau" , "Great Jagras" , "[s] Kestodon M" , "Rathian" , "Pink Rathian" , "Azure Rathalos" , "Diablos" ,
			"Black Diablos" , "Kirin" , "Behemoth" , "Kushala Daora" , "Lunastra" , "Teostra" , "Lavasioth" , "Deviljho" , "Barroth" , "Uragaan" , "Leshen" , "Pukei-Pukei" , "Nergigante" , "Xeno'jiiva" , "Kulu-Ya-Ku" , "Tzitzi-Ya-Ku" ,
			"Jyuratodus" , "Tobi-Kadachi" , "Paolumu" , "Legiana" , "Great Girros" , "Odogaron" , "Radobaan" , "Vaal Hazak" , "Dodogama" , "Kulve Taroth" , "Bazelgeuse" , "[s] Apceros" , "[s] Kelbi M" , "[s[ Kelbi F" , "[s] Hornetaur" ,
			"[s] Vespoid" , "[s] Mernos" , "[s] Kestodon F" , "[s] Raphinos" , "[s] Shamos" , "[s] Barnos" , "[s] Girros" , "Ancient Leshen" , "[s] Gastodon" , "[s] Noios" , "[s] Magmacore 1" , "[s] Magmacore 2" , "[s] Gajalaka" ,
			"[s] Small Barrel" , "[s] Large Barrel" , "[s] Training Pole" , "NON-VALID" , "Tigrex (IB)" , "Nargacuga (IB)" , "Barioth (IB)" , "Savage Deviljho (IB)" , "Brachydios (IB)" , "Glavenus (IB)" , "Acidic Glavenus (IB)" ,
			"Fulgur Anjanath (IB)" , "Coral Pukei-Pukei (IB)" , "Ruiner Nergigante (IB)" , "Viper Tobi-Kadachi (IB)" , "Nightshade Paolumu (IB)" , "Shrieking Legiana (IB)" , "Ebony Odogaron (IB)" , "Blackveil Vaal Hazak (IB)" ,
			"Seething Bazelgeuse (IB)" , "Beotodus (IB)" , "Banbaro (IB)" , "Velkhana (IB)" , "Namielle (IB)" , "Shara Ishvalda (IB)" , "[s] Popo (IB)" , "[s] Anteka (IB)" , "[s] Wulg (IB)" , "[s] Cortos (IB)" , "[s] Boaboa (IB)" ,
			"Alatreon (IB)" , "Gold Rathian (IB)" , "Silver Rathalos (IB)" , "Yian Garuga (IB)" , "Rajang (IB)" , "Furious Rajang (IB)" , "Brute Tigrex (IB)" , "Zinogre (IB)" , "Stygian Zinogre (IB)" , "Raging Brachydios (IB)" , "Safi'Jiiva (IB)" , "[s] Wood Dummy (IB)" ,
			"Scarred Yian Garuga (IB)","Frostfang Barioth (IB)", "Fatalis (IB)"}.Where(x => x.EndsWith(" (IB)")).Select(x => x.Replace("[s] ", "").Replace(" (IB)", "").Trim())];
					content.AppendLine($@"{{{{Meta
|MetaTitle     = MHWI Titles
|MetaDesc      = A list of every Guild Card Title from Monster Hunter World: Iceborne (MHWI).
|MetaKeywords  = MHWI, Monster Hunter Wilds, Title, Titles, Guild Card Titles
|MetaImage     = MHWI-Logo.png
}}}}
{{{{GenericNav|MHWI}}}}

The following is a list of all Guild Card Titles available in [[Monster Hunter World: Iceborne]]:
__TOC__
= Base Game (Monster Hunter World) =
== Middle Title Only ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					foreach (Titles title in allTitles.Where(x => x.TitleType == TitleType.Adj && firstSection.Contains(x.UnlockType)).OrderBy(x => Array.IndexOf(firstSection, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Rank Progression ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => x.TitleType != TitleType.Adj && !eventCollabTitles.Contains(x.Description!) && firstSection.Contains(x.UnlockType)).OrderBy(x => Array.IndexOf(firstSection, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Story/Quest Progression ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => Convert.ToInt32(x.UnlockParam!) < 1200000 && !eventCollabTitles.Contains(x.Description!) && (secondSection.Contains(x.UnlockType) || x.Description!.Contains("<STYL MOJI_LIGHTBLUE_DEFAULT>"))).OrderBy(x => Array.IndexOf(secondSection, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Hunting Milestones ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					foreach (KeyValuePair<string, Titles[]> titleList in allTitles.Where(x => x.UnlockType == "Hunt_XXX" && !eventCollabTitles.Contains(x.Description!) && baseMonsters.Contains(x.MonsterName)).OrderBy(x => Convert.ToInt32(x.Index)).GroupBy(x => x.MonsterName).ToDictionary(x => x.Key!, x => x.Select(y => y).ToArray()))
					{
						string titles = "";
						string numbers = "";
						foreach (Titles title in titleList.Value)
						{
							if (titles != "")
							{
								titles += " / ";
							}
							titles += title.Name;
							if (numbers != "")
							{
								numbers += " / ";
							}
							numbers += string.Join("", title.Description!.Where(char.IsDigit));
						}
						string firstDesc = titleList.Value.First().Description!;
						string firstNumbers = string.Join("", firstDesc.Where(char.IsDigit));
						content.AppendLine($@"| {titles}
| {firstDesc.Replace(titleList.Key, $"[[{titleList.Key}/MHWI|{titleList.Key}]]").Replace(firstNumbers, numbers)}
|-");
					}
					foreach (Titles title in allTitles.Where(x => ((x.UnlockType == "Unk_8" && (x.Description!.StartsWith("Hunted") || x.Description!.StartsWith("Captured") || x.Description!.StartsWith("Slayed"))) || (x.UnlockType == "Unk_12" && x.Description!.Contains("Kulve Taroth"))) && !eventCollabTitles.Contains(x.Description!)).OrderBy(x => Convert.ToInt32(x.Index)).ToArray())
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"|}}
= Expansion (Monster Hunter World: Iceborne) =
== Rank Progression ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => fourthSection.Contains(x.UnlockType) && !eventCollabTitles.Contains(x.Description!)).OrderBy(x => Array.IndexOf(fourthSection, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Story/Quest Progression ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => Convert.ToInt32(x.UnlockParam!) >= 1200000 && !eventCollabTitles.Contains(x.Description!) && fifthSection.Contains(x.UnlockType)).OrderBy(x => Array.IndexOf(fifthSection, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Hunting Milestones ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					foreach (KeyValuePair<string, Titles[]> titleList in allTitles.Where(x => x.UnlockType == "Hunt_XXX" && !eventCollabTitles.Contains(x.Description!) && ibMonsters.Contains(x.MonsterName)).OrderBy(x => Convert.ToInt32(x.Index)).GroupBy(x => x.MonsterName).ToDictionary(x => x.Key!, x => x.Select(y => y).ToArray()))
					{
						string titles = "";
						string numbers = "";
						foreach (Titles title in titleList.Value)
						{
							if (titles != "")
							{
								titles += " / ";
							}
							titles += title.Name;
							if (numbers != "")
							{
								numbers += " / ";
							}
							numbers += string.Join("", title.Description!.Where(char.IsDigit));
						}
						string firstDesc = titleList.Value.First().Description!;
						string firstNumbers = string.Join("", firstDesc.Where(char.IsDigit));
						content.AppendLine($@"| {titles}
| {firstDesc.Replace(titleList.Key, $"[[{titleList.Key}/MHWI|{titleList.Key}]]").Replace(firstNumbers, numbers)}
|-");
					}
					content.AppendLine(@"|}
= Miscellaneous =
== Special Event/Collaboration Milestones ==
{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => eventCollabTitles.Contains(x.Description!)).OrderBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Miscellaneous Milestones ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => miscMilestones.Contains(x.UnlockType) && !eventCollabTitles.Contains(x.Description!) && !x.Description!.StartsWith("Hunted") && !x.Description!.StartsWith("Slayed") && !x.Description!.StartsWith("Captured")).OrderBy(x => Array.IndexOf(miscMilestones, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Downloadable ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => downloadable.Contains(x.UnlockType) && !eventCollabTitles.Contains(x.Description!) && !x.Name!.Contains("HARDUMMY") && !x.Description!.Contains("Kulve Taroth")).OrderBy(x => Array.IndexOf(downloadable, x.UnlockType)).ThenBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
== Festival Participation ==
{{| class=""wikitable"" style=""text-align:center; margin:auto;""
|-
! Titles
! Unlock Condition
|-");
					lastType = "";
					thisTypeTitles = [];
					foreach (Titles title in allTitles.Where(x => x.UnlockType == "Unk_13" && !eventCollabTitles.Contains(x.Description!)).OrderBy(x => Convert.ToInt32(x.UnlockParam)).ThenBy(x => Convert.ToInt32(x.Index)))
					{
						string desc = ParseMHWIDescription(title.Description!);
						if (lastType != desc)
						{
							if (lastType != "")
							{
								content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-");
							}
							thisTypeTitles = [];
							lastType = desc!;
						}
						thisTypeTitles.Add(ParseMHWIDescription(title.Name!));
					}
					content.AppendLine($@"| {string.Join(", ", thisTypeTitles.OrderBy(x => x).Distinct())}
| {lastType}
|-
|}}
[[Category:Titles]]");
					await page.EditAsync(new WikiPageEditOptions()
					{
						Bot = true,
						Content = content.ToString(),
						Minor = true,
						Summary = "Added missing titles and resorted",
						Watch = AutoWatchBehavior.None
					});
					Console.WriteLine($"Titles edited.");
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task UploadMHWIQuests()
		{
			string[] smallMonsters = [..new string[] {
				"None", "Anjanath" , "Rathalos" , "[s] Aptonoth" , "[s] Jagras" , "Zorah Magdaros" , "[s] Mosswine" , "[s] Gajau" , "Great Jagras" , "[s] Kestodon M" , "Rathian" , "Pink Rathian" , "Azure Rathalos" , "Diablos" ,
			"Black Diablos" , "Kirin" , "Behemoth" , "Kushala Daora" , "Lunastra" , "Teostra" , "Lavasioth" , "Deviljho" , "Barroth" , "Uragaan" , "Leshen" , "Pukei-Pukei" , "Nergigante" , "Xeno'jiiva" , "Kulu-Ya-Ku" , "Tzitzi-Ya-Ku" ,
			"Jyuratodus" , "Tobi-Kadachi" , "Paolumu" , "Legiana" , "Great Girros" , "Odogaron" , "Radobaan" , "Vaal Hazak" , "Dodogama" , "Kulve Taroth" , "Bazelgeuse" , "[s] Apceros" , "[s] Kelbi M" , "[s[ Kelbi F" , "[s] Hornetaur" ,
			"[s] Vespoid" , "[s] Mernos" , "[s] Kestodon F" , "[s] Raphinos" , "[s] Shamos" , "[s] Barnos" , "[s] Girros" , "Ancient Leshen" , "[s] Gastodon" , "[s] Noios" , "[s] Magmacore 1" , "[s] Magmacore 2" , "[s] Gajalaka" ,
			"[s] Small Barrel" , "[s] Large Barrel" , "[s] Training Pole" , "NON-VALID" , "Tigrex (IB)" , "Nargacuga (IB)" , "Barioth (IB)" , "Savage Deviljho (IB)" , "Brachydios (IB)" , "Glavenus (IB)" , "Acidic Glavenus (IB)" ,
			"Fulgur Anjanath (IB)" , "Coral Pukei-Pukei (IB)" , "Ruiner Nergigante (IB)" , "Viper Tobi-Kadachi (IB)" , "Nightshade Paolumu (IB)" , "Shrieking Legiana (IB)" , "Ebony Odogaron (IB)" , "Blackveil Vaal Hazak (IB)" ,
			"Seething Bazelgeuse (IB)" , "Beotodus (IB)" , "Banbaro (IB)" , "Velkhana (IB)" , "Namielle (IB)" , "Shara Ishvalda (IB)" , "[s] Popo (IB)" , "[s] Anteka (IB)" , "[s] Wulg (IB)" , "[s] Cortos (IB)" , "[s] Boaboa (IB)" ,
			"Alatreon (IB)" , "Gold Rathian (IB)" , "Silver Rathalos (IB)" , "Yian Garuga (IB)" , "Rajang (IB)" , "Furious Rajang (IB)" , "Brute Tigrex (IB)" , "Zinogre (IB)" , "Stygian Zinogre (IB)" , "Raging Brachydios (IB)" , "Safi'Jiiva (IB)" , "[s] Wood Dummy (IB)" ,
			"Scarred Yian Garuga (IB)","Frostfang Barioth (IB)", "Fatalis (IB)"}.Where(x => x.StartsWith("[s]")).Select(x => x.Replace("[s] ", "").Replace(" (IB)", "").Trim())];
			Models.Data.MHWI.Items[] allItems = Models.Data.MHWI.Items.Fetch();
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					int cntr = 1;
					Models.Data.MHWI.Quests[] quests = [.. Models.Data.MHWI.Quests.FetchQuests().Where(x => x.Locale!.Contains("Arena (New World)"))];
					foreach (Models.Data.MHWI.Quests quest in quests)
					{
						WikiPage page = new(site, $"{quest.QuestName} (MHWI Quest)");
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						StringBuilder content = new();
						string questSummary = "";
						if (quest.TargetMonsters.Length > 0)
						{
							if (smallMonsters.Contains(quest.TargetMonsters[0]))
							{
								if (quest.MonsterAmounts[0] > 1)
								{
									questSummary = $"{quest.QuestType} {quest.MonsterAmounts[0]} [[{quest.TargetMonsters[0]}/MHWI|{quest.TargetMonsters[0]}]]";
								}
								else
								{
									questSummary = $"{quest.QuestType} [[{quest.TargetMonsters[0]}/MHWI|{quest.TargetMonsters[0]}]]";
								}
							}
							else
							{
								if (quest.TargetMonsters.Length > 1)
								{
									questSummary = $"{quest.QuestType} all target monsters";
								}
								else
								{
									questSummary = $"{quest.QuestType} {(quest.MonsterSpecialStates[0] != "" ? $"[[{quest.MonsterSpecialStates[0]} Monsters|{quest.MonsterSpecialStates[0]}]] " : "")}[[{quest.TargetMonsters[0]}/MHWI|{quest.TargetMonsters[0]}]]";
								}
							}
						}
						else
						{
							questSummary = $"{quest.QuestType} {quest.DeliveryItemAmount} [[{quest.DeliveryItemName} (MHWI)|{quest.DeliveryItemName}]]";
						}
						content.AppendLine($@"{{{{GenericNav|MHWI}}}}
{{{{GenericQuestListEntryV2
|Rank = {quest.Rank!.Replace(" Rank", "")}
|Rank Level = {quest.RankLevel}
|Quest Name = {quest.QuestName}
|Quest Type = {quest.QuestType!.Replace("Deliver", "Delivery")!}
|Quest Summary = {questSummary}");
						int targetCntr = 1;
						string goalIcons = "|Goal Icons =";
						if (quest.QuestType == "Deliver")
						{
							Models.Data.MHWI.Items item = allItems.First(x => x.Name == quest.DeliveryItemName);
							goalIcons += $"{{{{IconPickerUniversal|MHWI|{item.WikiIconName}|Color={GetIconColorName(item.WikiIconColor!.Value)}|Size=50|Text=|Margin=0|ML=Y}}}}";
						}
						else
						{
							foreach (string monster in quest.TargetMonsters!.Select(x => x!))
							{
								goalIcons += $" {{{{IconPickerUniversal|MHWI|{monster}";
								if (quest.MonsterSpecialStates.Length > targetCntr - 1)
								{
									string? specialState = quest.MonsterSpecialStates[targetCntr - 1];
									if (!string.IsNullOrEmpty(specialState))
									{
										goalIcons += $"|Type={specialState.Replace("Arch-Tempered", "Arch Tempered")}";
									}
								}
								goalIcons += "|Size=64|Text=|Margin=0|ML=Y}}";
								targetCntr++;
							}
						}
						content.AppendLine(goalIcons);
						content.AppendLine($@"|Requirements = {(quest.Requirements!.Contains("Placeholder") ? "???" : quest.Requirements)}
|Locale = {quest.Locale}
|Environment Details = {quest.EnvironmentDetails}
|Time Limit = {quest.TimeLimit}
|Failure Conditions = {quest.FailureConditions}
|Reward Money = {quest.RewardMoney!.Replace("z", "")}");
						int largeCntr = 1;
						string otherLargeIcons = "|Other Large Monster Icons =";
						foreach (string monster in quest.OtherMonstersLarge.Where(x => !string.IsNullOrEmpty(x) && x != "None" && !quest.TargetMonsters.Contains(x)).Select(x => x!))
						{
							otherLargeIcons += $" {{{{IconPickerUniversal|MHWI|{monster}|Size=64|Text=|Margin=0|ML=Y}}}}";
							largeCntr++;
						}
						if (largeCntr > 1)
						{
							content.AppendLine(otherLargeIcons);
						}
						int smallCntr = 1;
						string otherSmallIcons = "|Other Small Monster Icons =";
						foreach (string monster in quest.OtherMonstersSmall.Where(x => !string.IsNullOrEmpty(x) && x != "None" && !quest.TargetMonsters.Contains(x)).Select(x => x!))
						{
							otherSmallIcons += $" {{{{IconPickerUniversal|MHWI|{monster}|Size=64|Text=|Margin=0|ML=Y}}}}";
							smallCntr++;
						}
						if (smallCntr > 1)
						{
							content.AppendLine(otherSmallIcons);
						}
						content.AppendLine($@"|Client = {quest.Client}
|Description = {quest.Description}
}}}}
[[Category:MHWI Quest Pages]]");
						if (quest.Requirements!.Contains("Placeholder"))
						{
							content.AppendLine("[[Category:MHWI Quests Needing Requirements]]");
						}
						await page.EditAsync(new WikiPageEditOptions()
						{
							Bot = true,
							Content = content.ToString(),
							Minor = false,
							Summary = "Swapped to new template",
							Watch = AutoWatchBehavior.None
						});
						Console.WriteLine($"({cntr}/{quests.Length}) {quest.QuestName} edited.");
					}
					cntr++;
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static Dictionary<string, MonsterNames> FetchMonsterNames()
		{
			Dictionary<string, MonsterNames> finishedMonsterNames = [];
			if (File.Exists(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterNames.json"))
			{
				finishedMonsterNames = JsonConvert.DeserializeObject<Dictionary<string, MonsterNames>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterNames.json"))!;
			}
			else
			{
				Dictionary<string, Dictionary<string, string>> guFiles = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHGU\monsterNameDict.json"))!;
				Dictionary<string, Dictionary<string, string>> worldFiles = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\monsterNameDict.json"))!;
				Dictionary<string, string[]> finalWorldFiles = worldFiles.ToDictionary(x => x.Key, x => x.Value.Values.ToArray());
				for (int i = 0; i < finalWorldFiles.Max(x => x.Value.Length); i++)
				{
					string englishName = finalWorldFiles["English"][i];
					MonsterNames worldMonsterName = new()
					{
						Arabic = finalWorldFiles["Arabic"][i],
						TraditionalChinese = finalWorldFiles["TraditionalChinese"][i],
						SimplifiedChinese = finalWorldFiles["SimplifiedChinese"][i],
						English = finalWorldFiles["English"][i],
						French = finalWorldFiles["French"][i],
						German = finalWorldFiles["German"][i],
						Italian = finalWorldFiles["Italian"][i],
						Japanese = finalWorldFiles["Japanese"][i],
						Korean = finalWorldFiles["Korean"][i],
						Polish = finalWorldFiles["Polish"][i],
						PortugueseBr = finalWorldFiles["PortugueseBr"][i],
						Russian = finalWorldFiles["Russian"][i],
						Spanish = finalWorldFiles["Spanish"][i],
						LatinAmericanSpanish = finalWorldFiles["LatinAmericanSpanish"][i]
					};
					if (!finishedMonsterNames.TryAdd(worldMonsterName.English, worldMonsterName))
					{
						finishedMonsterNames[worldMonsterName.English] = worldMonsterName;
					}
				}
				List<JObject> riseFiles = [.. JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\message\tag\tag_em_name.msg.539100710.json"))!.Value<JObject>("msgs")!.Values().Select(x => x.Value<JObject>("content")!)];
				riseFiles.AddRange([.. JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Raw Data\MHRS\natives\stm\message\tag_mr\tag_em_name_mr.msg.539100710.json"))!.Value<JObject>("msgs")!.Values().Select(x => x.Value<JObject>("content")!)]);
				foreach (JObject file in riseFiles)
				{
					MonsterNames riseMonsterName = new()
					{
						Japanese = file.Value<string>("Japanese")!,
						English = file.Value<string>("English")!,
						French = file.Value<string>("French")!,
						Italian = file.Value<string>("Italian")!,
						German = file.Value<string>("German")!,
						Spanish = file.Value<string>("Spanish")!,
						Russian = file.Value<string>("Russian")!,
						Polish = file.Value<string>("Polish")!,
						Dutch = file.Value<string>("Dutch")!,
						Portuguese = file.Value<string>("Portuguese")!,
						PortugueseBr = file.Value<string>("PortugueseBr")!,
						Korean = file.Value<string>("Korean")!,
						TraditionalChinese = file.Value<string>("TransitionalChinese")!,
						SimplifiedChinese = file.Value<string>("SimplelifiedChinese")!,
						Finnish = file.Value<string>("Finnish")!,
						Swedish = file.Value<string>("Swedish")!,
						Danish = file.Value<string>("Danish")!,
						Norwegian = file.Value<string>("Norwegian")!,
						Czech = file.Value<string>("Czech")!,
						Hungarian = file.Value<string>("Hungarian")!,
						Slovak = file.Value<string>("Slovak")!,
						Arabic = file.Value<string>("Arabic")!,
						Turkish = file.Value<string>("Turkish")!,
						Bulgarian = file.Value<string>("Bulgarian")!,
						Greek = file.Value<string>("Greek")!,
						Romanian = file.Value<string>("Romanian")!,
						Thai = file.Value<string>("Thai")!,
						Ukrainian = file.Value<string>("Ukrainian")!,
						Vietnamese = file.Value<string>("Vietnamese")!,
						Indonesian = file.Value<string>("Indonesian")!,
						Fiction = file.Value<string>("Fiction")!,
						Hindi = file.Value<string>("Hindi")!,
						LatinAmericanSpanish = file.Value<string>("LatinAmericanSpanish")!
					};
					if (!finishedMonsterNames.TryAdd(riseMonsterName.English, riseMonsterName))
					{
						finishedMonsterNames[riseMonsterName.English] = riseMonsterName;
					}
				}
				JArray[] wildsFiles = [.. JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWilds\dtlnor rips\MHWs-in-json-main\natives\STM\GameDesign\Text\Excel_Data\EnemyText.msg.23.json"))!
					.Value<JArray>("entries")!
					.Where(x => x.Value<string>("name")!.StartsWith("EnemyText_NAME_"))
					.Select(x => x.Value<JArray>("content")!)];
				foreach (JArray file in wildsFiles)
				{
					MonsterNames riseMonsterName = new()
					{
						Japanese = file[0].Value<string>()!,
						English = file[1].Value<string>()!,
						French = file[2].Value<string>()!,
						Italian = file[3].Value<string>()!,
						German = file[4].Value<string>()!,
						Spanish = file[5].Value<string>()!,
						Russian = file[6].Value<string>()!,
						Polish = file[7].Value<string>()!,
						Dutch = file[8].Value<string>()!,
						Portuguese = file[9].Value<string>()!,
						PortugueseBr = file[10].Value<string>()!,
						Korean = file[11].Value<string>()!,
						TraditionalChinese = file[12].Value<string>()!,
						SimplifiedChinese = file[13].Value<string>()!,
						Finnish = file[14].Value<string>()!,
						Swedish = file[15].Value<string>()!,
						Danish = file[16].Value<string>()!,
						Norwegian = file[17].Value<string>()!,
						Czech = file[18].Value<string>()!,
						Hungarian = file[19].Value<string>()!,
						Slovak = file[20].Value<string>()!,
						Arabic = file[21].Value<string>()!,
						Turkish = file[22].Value<string>()!,
						Bulgarian = file[23].Value<string>()!,
						Greek = file[24].Value<string>()!,
						Romanian = file[25].Value<string>()!,
						Thai = file[26].Value<string>()!,
						Ukrainian = file[27].Value<string>()!,
						Vietnamese = file[28].Value<string>()!,
						Indonesian = file[29].Value<string>()!,
						Fiction = file[30].Value<string>()!,
						Hindi = file[31].Value<string>()!,
						LatinAmericanSpanish = file[32].Value<string>()!
					};
					if (!finishedMonsterNames.TryAdd(riseMonsterName.English, riseMonsterName))
					{
						finishedMonsterNames[riseMonsterName.English] = riseMonsterName;
					}
				}
				File.WriteAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\monsterNames.json", JsonConvert.SerializeObject(finishedMonsterNames.OrderBy(x => x.Key).Where(x => !string.IsNullOrEmpty(x.Key)).ToDictionary(x => x.Key, x => x.Value), Formatting.Indented));
			}
			return finishedMonsterNames;
		}

		public static async Task UploadMonsterGameData(string game, string[] monsterNames)
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!
			})
			{
				//Armor
				//congalala = conga
				//yian kut-ku = kut - ku
				//guardian fulgur anjanath = guardian fulgur
				//blangonga = blango
				//jin dahaad = dahaad
				//gore magala = gore
				//guardian ebony odogaron = guardian ebony

				//Weapons
				//guardian rathalos = g.rathalos
				//guardian fulgur anjanath = g.fulgur
				//guardian ebony odogaron = g.ebony
				string[] armorNames = [
  "Expedition Tree",
  "Ore Tree",
  "Bone Tree",
  "Chatacabra Tree",
  "Quematrice Tree",
  "Lala Barina Tree",
  "Congalala Tree",
  "Balahara Tree",
  "Doshaguma Tree",
  "Uth Duna Tree",
  "Rompopolo Tree",
  "Rey Dau Tree",
  "Nerscylla Tree",
  "Hirabami Tree",
  "Ajarakan Tree",
  "Nu Udra Tree",
  "G. Doshaguma Tree",
  "G. Rathalos Tree",
  "Jin Dahaad Tree",
  "G. Ebony Tree",
  "Xu Wu Tree",
  "G. Arkveld Tree",
  "Yian Kut-Ku Tree",
  "Gypceros Tree",
  "Rathian Tree",
  "Rathalos Tree",
  "G. Fulgur Tree",
  "Gravios Tree",
  "Blangonga Tree",
  "Gore Magala Tree",
  "Arkveld Tree",
  "Water Element Tree",
  "Paralysis Tree",
  "Speartuna Tree",
  "Vespoid Tree",
  "Workshop Tree"
];
				string[] weaponNames = [
  "Hope",
  "Leather",
  "Chainmail",
  "Bone",
  "Alloy",
  "Ingot",
  "Talioth",
  "Bulaqchi",
  "Piragill",
  "Kranodath",
  "Comaqchi",
  "Guardian Seikret",
  "Vespoid",
  "Chatacabra",
  "Quematrice",
  "Lala Barina",
  "Conga",
  "Balahara",
  "Doshaguma",
  "Rompopolo",
  "Nerscylla",
  "Hirabami",
  "Ajarakan",
  "Xu Wu",
  "Rey Dau",
  "Uth Duna",
  "Nu Udra",
  "Guardian Arkveld α",
  "Guardian Doshaguma",
  "Guardian Rathalos",
  "Guardian Ebony",
  "Guardian Arkveld",
  "Guardian Arkveld β",
  "Hope α",
  "Leather α",
  "Chainmail α",
  "Bulaqchi α",
  "Bulaqchi β",
  "Talioth α",
  "Talioth β",
  "Piragill α",
  "Piragill β",
  "Vespoid α",
  "Vespoid β",
  "Kranodath α",
  "Kranodath β",
  "Comaqchi α",
  "Comaqchi β",
  "Kut-Ku α",
  "Kut-Ku β",
  "Chatacabra α",
  "Chatacabra β",
  "Gypceros α",
  "Gypceros β",
  "Bone α",
  "Alloy α",
  "Quematrice α",
  "Quematrice β",
  "Lala Barina α",
  "Lala Barina β",
  "Rompopolo α",
  "Rompopolo β",
  "Conga α",
  "Conga β",
  "Balahara α",
  "Balahara β",
  "Rathian α",
  "Rathian β",
  "Nerscylla α",
  "Nerscylla β",
  "Doshaguma α",
  "Doshaguma β",
  "Hirabami α",
  "Hirabami β",
  "Guardian Fulgur α",
  "Guardian Fulgur β",
  "Ingot α",
  "Guardian Seikret α",
  "Guardian Seikret β",
  "Ajarakan α",
  "Ajarakan β",
  "Blango α",
  "Blango β",
  "Guardian Doshaguma α",
  "Guardian Doshaguma β",
  "Guardian Rathalos α",
  "Guardian Rathalos β",
  "Gravios α",
  "Gravios β",
  "Guardian Ebony α",
  "Guardian Ebony β",
  "Rathalos α",
  "Rathalos β",
  "Xu Wu α",
  "Xu Wu β",
  "Gore α",
  "Gore β",
  "Dober α",
  "Damascus α",
  "Dahaad α",
  "Dahaad β",
  "Rey Dau α",
  "Rey Dau β",
  "Uth Duna α",
  "Uth Duna β",
  "Nu Udra α",
  "Nu Udra β",
  "Arkveld α",
  "Arkveld β",
  "Kunafa α",
  "Azuz α",
  "Suja α",
  "Sild α",
  "Death Stench α",
  "Butterfly α",
  "King Beetle α",
  "High Metal α",
  "Battle α",
  "Melahoa α",
  "Artian α",
  "Gajau α",
  "Commission α",
  "Mimiphyta α",
  "Guild Knight",
  "Feudal Soldier",
  "Fencer's Eyepatch",
  "Oni Horns Wig",
  "Wyverian Ears",
  "Guild Ace α",
  "Gajau",
  "Dragonking α",
  "Expedition Headgear α"
];
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					int cnt = 1;
					foreach (string monsterName in monsterNames)
					{
						WikiPage page = new(site, $"{monsterName}/{game}");
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						if (page.Exists)
						{
							string oldContent = page.Content!;
							string newContent = "";
							if (game == "MHWI")
							{
								Monster mon = new(monsterName, game);
								if (oldContent.Contains($"{{{{:{monsterName.Replace(" ", "_")}/Lore}}}}"))
								{
									newContent = oldContent.Insert(oldContent.IndexOf($"{{{{:{monsterName.Replace(" ", "_")}/Lore}}}}") - 1, mon.Format());
								}
								else
								{
									newContent = oldContent.Insert(oldContent.IndexOf($"{{{{:{monsterName}/Lore}}}}") - 1, mon.Format());
								}
							}
							else if (game == "MHWilds")
							{
								newContent = oldContent + @"
";
								await page.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = newContent,
									Minor = true,
									Summary = "Added drop tables",
									Watch = AutoWatchBehavior.None
								});
							}
							Console.WriteLine($"({cnt}/{monsterNames.Length}) {monsterName}/{game} edited.");
							cnt++;
						}
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task TranslateJPItemNames()
		{

			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					int cntr = 1;
					Models.Data.MHWI.Items[] mhwiItems = [.. Models.Data.MHWI.Items.Fetch().Where(x => x.Name != "Unavailable" && x.Name != "HARDUMMY" && x.Name != "(None)")];
					foreach (Models.Data.MHWI.Items item in mhwiItems)
					{
						WikiPage page = new(site, $"{item.Name} (MHWI)");
						await page.RefreshAsync(PageQueryOptions.FetchContent);
						string jpnName = Translate(item.Name);
						string jpnString = page.Content![page.Content!.IndexOf("|Japanese Name = ")..];
						jpnString = jpnString[..jpnString.IndexOf("|Type = ")];
						string newContent = page.Content!.Replace(jpnString, $"|Japanese Name = {jpnName}\r\n");
						await page.EditAsync(new WikiPageEditOptions()
						{
							Bot = true,
							Content = newContent,
							Minor = false,
							Summary = "Added JPN names",
							Watch = AutoWatchBehavior.None
						});
						Console.WriteLine($"({cntr}/{mhwiItems.Length}) {item.Name} created.");
						cntr++;
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static string Translate(string src)
		{
			Dictionary<string, string> engNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\itemNames.json"))!;
			Dictionary<string, string> jpnNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\MH_Data Repo\MH_Data\Parsed Files\MHWI\itemNames_JP.json"))!;
			return jpnNames[engNames.First(x => x.Value == src).Key];
		}

		public static async Task<string> GoogleTranslate(string src, string srcLng = "ja", string targetLng = "en")
		{
			if (Translations.TryGetValue(src, out string? value))
			{
				return value;
			}
			else
			{
				using Google.Apis.Translate.v2.TranslateService svc = new(new Google.Apis.Services.BaseClientService.Initializer()
				{
					ApiKey = "${{ vars.GOOGLE_API_KEY }}",
					ApplicationName = "MH Wiki Translation",
				});
				TranslationsListResponse response = await svc.Translations.Translate(new TranslateTextRequest()
				{
					Source = srcLng,
					Format = "text",
					Target = targetLng,
					Q = [src]
				}).ExecuteAsync()!;
				string res = response.Translations.First().TranslatedText;
				Translations.Add(src, res);
				return res;
			}
		}

		public static async Task UploadItems(string game)
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					int cntr = 1;
					if (game == "MHWilds")
					{
						Models.Data.MHWilds.Items[] mhwildsItems = Models.Data.MHWilds.Items.FillSources(Models.Data.MHWilds.Items.Fetch());
						foreach (Models.Data.MHWilds.Items item in mhwildsItems)
						{
							WikiPage page = new(site, $"{item.Name} ({game})");
							StringBuilder content = new();
							content.AppendLine($@"{{{{#css:
.toc
{{
    width:45%;
}}
.item-top
{{
    min-width:100%;
    width:100%;
    display:flex; 
    flex-wrap:wrap; 
    justify-content:space-between;
}}
@media screen and (max-width:600px)
{{
    .toc
    {{
        width:100%;
    }}
    .item-top .content-table-wrapper
    {{
        min-width:100%;
    }}
    .item-table
    {{
        min-width:100%;
    }}
    .item-top
    {{
        display:flex; 
        flex-wrap:wrap; 
        justify-content:center;
    }}
}}
}}}}
{{{{GenericNav|MHWilds}}}}
<div class=""item-top"">
__TOC__
{{{{MHWildsInfobox
|English Name = {item.Name}
|Japanese Name = {item.JPName}
|Type = {item.Category}
|Image = MHWilds-{item.Icon} Icon {item.IconColor}.png
|Description = {item.Description}
|Rarity = {item.Rarity}
|Price = {item.SellPrice}z
|Carry Limit = {item.Carry}x
}}}}
</div>
=Sources=
<div class=""mw-collapsible"">
==Crafting Recipes==
<div class=mw-collapsible-content>
{{| class=""wikitable"" style=""text-align:center;""
! # 
! style=""white-space:wrap;"" | Result
!
! style=""white-space:wrap;"" | Material A 
!
! style=""white-space:wrap;"" | Material B
|-");
							foreach (Models.Data.MHWilds.ItemCrafting combination in item.Combinations)
							{
								Models.Data.MHWilds.Items matA = mhwildsItems.First(x => x.Name == combination.MaterialA);
								Models.Data.MHWilds.Items? matB = null;
								if (!string.IsNullOrEmpty(combination.MaterialB))
								{
									matB = mhwildsItems.First(x => x.Name == combination.MaterialB);
								}
								Models.Data.MHWilds.Items result = mhwildsItems.First(x => x.Name == combination.Result);
								content.AppendLine($@"| {combination.Number} || {{{{GenericItemLink|MHWilds|{result.Name}|{result.Icon.Replace(" Great", "")}|{result.IconColor}}}}} || '''=''' || {{{{GenericItemLink|MHWilds|{matA.Name}|{matA.Icon.Replace(" Great", "")}|{matA.IconColor}}}}} || '''+''' || {(matB == null ? "-" : $"{{{{GenericItemLink|MHWilds|{matB.Name}|{matB.Icon}|{matB.IconColor}}}}}")}
|-");
							}
							content.AppendLine(@"|}
</div>
</div>");
							string[] questSources = [.. item.Sources.Where(x => !string.IsNullOrEmpty(x.QuestName)).Select(source => $"* [[{source.QuestName} (MHWilds)|{source.QuestName}]]").Distinct()];
							Models.Data.MHWilds.ItemSource[] monsterSources = [.. item.Sources.Where(x => !string.IsNullOrEmpty(x.MonsterName)).OrderBy(x => x.Rank).ThenByDescending(x => x.MonsterName).ThenByDescending(x => x.Circumstance).ThenByDescending(x => x.Probability)];
							string[] stageOrder = ["Windward Plains", "Scarlet Forest", "Oilwell Basin", "Iceshard Cliffs", "Ruins of Wyveria"];
							string[] gatheringSources = [.. item.Sources.Where(x => string.IsNullOrEmpty(x.QuestName) && string.IsNullOrEmpty(x.MonsterName)).OrderBy(x => Array.IndexOf(stageOrder, x.Stage)).ThenBy(x => x.Zone).Select(source => $"* [[{source.Stage}]] Zone {source.Zone}").Distinct()];
							if (questSources.Length > 1)
							{
								content.AppendLine(@"<div class=""mw-collapsible"">
==Quests==
<div class=mw-collapsible-content>
<div class=""threecol"" style=""clear:both;"">
<div>");
								int rows = 0;
								int eachCol = Convert.ToInt32(Math.Floor(questSources.Length / 3f));
								Dictionary<int, int> cols = new() {
								{ 0, eachCol },
								{ 1, eachCol },
								{ 2, eachCol },
							};
								int remainder = questSources.Length % 3;
								if (remainder > 0)
								{
									for (int i = 0; i < remainder; i++)
									{
										cols[i]++;
									}
								}
								int cells = 0;
								foreach (string source in questSources)
								{
									if (cells >= cols[rows])
									{
										cells = 0;
										if (rows < (cols.Count - 1))
										{
											rows++;
										}
										content.AppendLine("</div>\r\n<div>");
									}
									content.AppendLine(source);
									cells++;
								}
								content.AppendLine(@"</div>
</div>
</div>
</div>");
							}
							if (monsterSources.Length > 0)
							{
								content.AppendLine(@"<div class=""mw-collapsible"">
==Monsters==
<div class=mw-collapsible-content>
{| class=""wikitable sortable mobile-sm"" style=""width: 100%""
! Name || style=""white-space:wrap""| Source || Chance || Rank
|-");
								foreach (Models.Data.MHWilds.ItemSource monsterSource in monsterSources)
								{
									content.AppendLine($@"| [[{monsterSource.MonsterName}/MHWilds|{monsterSource.MonsterName}]]
| {monsterSource.Circumstance}
| {monsterSource.Probability}%
| {monsterSource.Rank}
|-");
								}
								content.AppendLine(@"
|}
</div>
</div>");
							}
							if (gatheringSources.Length > 0)
							{
								content.AppendLine(@"<div class=""mw-collapsible"">
== Gathering <ref>[[User:Cola|Cola's]] interactive MHWilds map (https://c-ola.github.io/wildsmap/)</ref>==
<div class=mw-collapsible-content>
<div class=""threecol"">
<div>");
								int rows = 0;
								int eachCol = Convert.ToInt32(Math.Floor(gatheringSources.Length / 3f));
								Dictionary<int, int> cols = new() {
									{ 0, eachCol },
									{ 1, eachCol },
									{ 2, eachCol },
								};
								int remainder = gatheringSources.Length % 3;
								if (remainder > 0)
								{
									for (int i = 0; i < remainder; i++)
									{
										cols[i]++;
									}
								}
								int cells = 0;
								foreach (string source in gatheringSources)
								{
									if (cells >= cols[rows])
									{
										cells = 0;
										if (rows < (cols.Count - 1))
										{
											rows++;
										}
										content.AppendLine("</div>\r\n<div>");
									}
									content.AppendLine(source);
									cells++;
								}
								content.AppendLine(@"</div>
</div>
</div>
</div>");
							}
							//if you ever define what "other" is, put it here
							//content.AppendLine(@"<div class=""mw-collapsible"">
							//==Other==
							//<div class=mw-collapsible-content>
							//<div>
							//<div>");
							if (item.Equipment.Any())
							{
								content.AppendLine(@"=Forging=");
								if (item.Equipment.Any(x => x.EquipmentType != "Armor"))
								{
									content.AppendLine(@"<div class=""mw-collapsible"">
==Weapons==
<div class=mw-collapsible-content>
<div class=""threecol"">
<div>");
									Models.Data.MHWilds.ItemEquipment[] weapons = [.. item.Equipment.Where(x => x.EquipmentType != "Armor")];
									int rows = 0;
									int eachCol = Convert.ToInt32(Math.Floor(weapons.Length / 3f));
									Dictionary<int, int> cols = new() {
										{ 0, eachCol },
										{ 1, eachCol },
										{ 2, eachCol },
									};
									int remainder = weapons.Length % 3;
									if (remainder > 0)
									{
										for (int i = 0; i < remainder; i++)
										{
											cols[i]++;
										}
									}
									int cells = 0;
									foreach (Models.Data.MHWilds.ItemEquipment weapon in weapons)
									{
										if (cells >= cols[rows])
										{
											cells = 0;
											if (rows < (cols.Count - 1))
											{
												rows++;
											}
											content.AppendLine("</div>\r\n<div>");
										}
										content.AppendLine($"* [[{weapon.EquipmentName} (MHWilds)|{weapon.EquipmentName}]]");
										cells++;
									}
									content.AppendLine(@"</div>
</div>
</div>
</div>");
								}
								if (item.Equipment.Any(x => x.EquipmentType == "Armor"))
								{
									content.AppendLine(@"<div class=""mw-collapsible"">
== Armor ==
<div class=mw-collapsible-content>
<div class=""threecol"">
<div>");
									Models.Data.MHWilds.ItemEquipment[] armor = [.. item.Equipment.Where(x => x.EquipmentType == "Armor")];
									int rows = 0;
									int eachCol = Convert.ToInt32(Math.Floor(armor.Length / 3f));
									Dictionary<int, int> cols = new() {
										{ 0, eachCol },
										{ 1, eachCol },
										{ 2, eachCol },
									};
									int remainder = armor.Length % 3;
									if (remainder > 0)
									{
										for (int i = 0; i < remainder; i++)
										{
											cols[i]++;
										}
									}
									int cells = 0;
									foreach (Models.Data.MHWilds.ItemEquipment arm in armor)
									{
										if (cells >= cols[rows])
										{
											cells = 0;
											if (rows < (cols.Count - 1))
											{
												rows++;
											}
											content.AppendLine("</div>\r\n<div>");
										}
										content.AppendLine($"* [[{arm.EquipmentName} (MHWilds)|{arm.EquipmentName}]]");
										cells++;
									}
									content.AppendLine(@"</div>
</div>
</div>
</div>");
								}
							}
							content.AppendLine(@"[[Category:MHWilds Items]]");
							await page.RefreshAsync();
							await page.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = content.ToString(),
								Minor = true,
								Summary = "Updated monster source format and added TU1 + Collab data",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cntr}/{mhwildsItems.Length}) {item.Name} created.");
							cntr++;
						}
					}
					else if (game == "MHWI")
					{
						Models.Data.MHWI.Items[] mhwiItems = [.. Models.Data.MHWI.Items.Fetch().Where(x => x.Name != "Unavailable" && x.Name != "HARDUMMY" && x.Name != "(None)")];
						//						StringBuilder overviewPage = new();
						//						overviewPage.AppendLine(@"{{#seo:
						//|title=Items in Monster Hunter World: Iceborne
						//|description=A list of all gatherable items in Monster Hunter World: Iceborne.
						//|image=5thGen-Question Mark Icon White.png
						//}}
						//{{GenericNav|MHWI}}
						//The following is a list of all items that appear in [[Monster Hunter World: Iceborne]].
						//__TOC__");
						//						Dictionary<TypeEnum, string> itemTypeDict = new()
						//						{
						//							{ TypeEnum.Item, "Consumables" },
						//							{ TypeEnum.Material, "Materials" },
						//							{ TypeEnum.AmmoOrCoating, "Ammo / Coatings" },
						//							{ TypeEnum.AccountItem, "Account Items" },
						//							{ TypeEnum.Jewel, "Jewels" },
						//							{ TypeEnum.RoomDecoration, "Room Furnishings" },
						//						};
						//						TypeEnum? lastCat = null;
						//						TypeEnum[] allTypes = [..mhwiItems.Select(x => x.Type!.Value).Distinct()];
						//						foreach (TypeEnum type in allTypes)
						//						{
						//							foreach (Models.Data.MHWI.Items item in mhwiItems.Where(x => x.Type == type).OrderBy(x => x.Id))
						//							{
						//								if (lastCat != item.Type!.Value)
						//								{
						//									if (lastCat != null)
						//									{
						//										overviewPage.AppendLine("|}");
						//									}
						//									overviewPage.AppendLine($@"== {itemTypeDict[item.Type!.Value]} ==
						//{{| class=""wikitable sortable mw-collapsible"" style=""width: 100%""
						//! Name || Rarity || Buy Price || Sell Price || Carry || Description");
						//									lastCat = item.Type!.Value;
						//								}
						//								overviewPage.AppendLine($@"|-
						//| {{{{IconPickerUniversalAlt|MHWI|{(item.WikiIconName != "NOT AVAILABLE" ? item.WikiIconName : "Question Mark")}|{item.Name}|Color={GetIconColorName(item.WikiIconColor!.Value)}}}}}
						//| align=""center"" | {(item.Rarity != null ? item.Rarity.Value.ToString() : "-")}
						//| align=""center"" | {(item.BuyPrice != null ? item.BuyPrice.Value.ToString() + "z" : "-")}
						//| align=""center"" | {(item.SellPrice != null ? item.SellPrice.Value.ToString() + "z" : "-")}
						//| align=""center"" | {(item.CarryLimit != null ? item.CarryLimit.Value.ToString() : "-")}
						//| {item.Description.Replace("<STYL MOJI_YELLOW_DEFAULT>", "<span style=\"color: #b29400;\">").Replace("</STYL>", "</span>")}");
						//							}
						//						}
						//						overviewPage.AppendLine(@"|}
						//[[Category:Items_by_Game_Appearance]]");
						//						WikiPage overviewWikiPage = new(site, "MHWI/Items");
						//						await overviewWikiPage.RefreshAsync();
						//						await overviewWikiPage.EditAsync(new WikiPageEditOptions()
						//						{
						//							Bot = true,
						//							Content = overviewPage.ToString(),
						//							Minor = false,
						//							Summary = "Created MHWI Item overview",
						//							Watch = AutoWatchBehavior.None
						//						});
						//						Console.WriteLine($"Overview created.");
						ItemCrafting[] itemCrafting = ItemCrafting.Fetch();
						Armor[] allArmor = Armor.GetArmors();
						WeaponCraftingData[] weaponCraft = WeaponCraftingData.GetCraftingData();
						WeaponForgingData[] weaponForge = WeaponForgingData.GetForgingData();
						GunnerData[] mhwiGuns = GunnerData.GetGunnerData();
						BlademasterData[] mhwiBlades = BlademasterData.GetBlademasterData();
						foreach (Models.Data.MHWI.Items item in mhwiItems)
						{
							WikiPage page = new(site, $"{item.Name} ({game})");
							StringBuilder content = new();
							content.AppendLine($@"{{{{GenericNav|MHWI}}}}
<br>
{{{{ItemInfobox
|English Name = {item.Name}
|Japanese Name = {GetMHWIItemJPNName(item.Name)}
|Type = {item.Type}
|Image = MHWI-{(item.WikiIconName != "NOT AVAILABLE" ? item.WikiIconName : "Question Mark")} Icon {GetIconColorName(item.WikiIconColor!.Value)}.png
|Description = {item.Description.Replace("<STYL MOJI_YELLOW_DEFAULT>", "<span style=\"color: #b29400;\">").Replace("</STYL>", "</span>")}
|Rarity = {item.Rarity}
|Price = {item.SellPrice}z
|Carry Limit = {item.CarryLimit}x
}}}}

=Crafting Recipes=
{{| class=""wikitable"" style=""text-align:center;""
! Number !! Result !! !! Material A !! !! Material B
|-");
							foreach (ItemCrafting combination in itemCrafting.Where(x => x.ResultId == item.Id))
							{
								Models.Data.MHWI.Items matA = mhwiItems.First(x => x.Id == combination.Mat1Id);
								Models.Data.MHWI.Items? matB = null;
								if (combination.Mat2Id != null && combination.Mat2Id > 0)
								{
									matB = mhwiItems.First(x => x.Id == combination.Mat2Id);
								}
								content.AppendLine($@"| {combination.Qty} || {{{{GenericItemLink|MHWI|{item.Name}|{(item.WikiIconName != "NOT AVAILABLE" ? item.WikiIconName : "Question Mark")}|{GetIconColorName(item.WikiIconColor!.Value)}}}}} || '''=''' || {{{{GenericItemLink|MHWI|{matA.Name}|{(matA.WikiIconName != "NOT AVAILABLE" ? matA.WikiIconName : "Question Mark")}|{GetIconColorName(matA.WikiIconColor!.Value)}}}}}  || '''+''' || {(matB == null ? "-" : $"{{{{GenericItemLink|MHWI|{matB.Name}|{(matB.WikiIconName != "NOT AVAILABLE" ? matB.WikiIconName : "Question Mark")}|{GetIconColorName(matB.WikiIconColor!.Value)}}}}}")}
|-");
							}
							content.AppendLine(@"|}
=Sources=
=Forging=
==Weapons==
<div class=""threecol"">
<div>");
							string[] weapons = GetEquipmentLinks(item, mhwiBlades, mhwiGuns, weaponCraft, weaponForge);
							int rows = 0;
							int eachCol = Convert.ToInt32(Math.Floor(weapons.Length / 3f));
							Dictionary<int, int> cols = new() {
								{ 0, eachCol },
								{ 1, eachCol },
								{ 2, eachCol },
							};
							int remainder = weapons.Length % 3;
							if (remainder > 0)
							{
								for (int i = 0; i < remainder; i++)
								{
									cols[i]++;
								}
							}
							int cells = 0;
							foreach (string weapon in weapons)
							{
								if (cells >= cols[rows])
								{
									cells = 0;
									if (rows < (cols.Count - 1))
									{
										rows++;
									}
									content.AppendLine("</div>\r\n<div>");
								}
								content.AppendLine($"* {weapon}");
								cells++;
							}
							content.AppendLine(@"</div>
</div>
==Armor==
<div class=""threecol"">
<div>");
							string[] armor = GetArmorStrings(item, allArmor);
							rows = 0;
							eachCol = Convert.ToInt32(Math.Floor(armor.Length / 3f));
							cols = new() {
								{ 0, eachCol },
								{ 1, eachCol },
								{ 2, eachCol },
							};
							remainder = armor.Length % 3;
							if (remainder > 0)
							{
								for (int i = 0; i < remainder; i++)
								{
									cols[i]++;
								}
							}
							cells = 0;
							foreach (string arm in armor)
							{
								if (cells >= cols[rows])
								{
									cells = 0;
									if (rows < (cols.Count - 1))
									{
										rows++;
									}
									content.AppendLine("</div>\r\n<div>");
								}
								content.AppendLine($"* {arm}");
								cells++;
							}
							content.AppendLine(@"</div>
</div>
[[Category:MHWI Items]]");
							await page.RefreshAsync(PageQueryOptions.FetchContent);
							await page.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = content.ToString(),
								Minor = false,
								Summary = "Added MHWI Item Pages",
								Watch = AutoWatchBehavior.None
							});
							Console.WriteLine($"({cntr}/{mhwiItems.Length}) {item.Name} created.");
							cntr++;
						}
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		private static string ParseMHWIDescription(string desc)
		{
			string questName = desc;
			if (questName.Contains("<STYL MOJI_LIGHTBLUE_DEFAULT>"))
			{
				questName = questName.Substring(questName.IndexOf("<STYL MOJI_LIGHTBLUE_DEFAULT>") + 29, questName.IndexOf("</STYL>") - 29 - questName.IndexOf("<STYL MOJI_LIGHTBLUE_DEFAULT>"));
			}
			return desc
				.Replace("Zorah Magdaros", "[[Zorah Magdaros/MHWI|Zorah Magdaros]]")
				.Replace("Ancient Forest", "[[Ancient Forest]]")
				.Replace("Wildspire Waste", "[[Wildspire Waste]]")
				.Replace("Coral Highlands", "[[Coral Highlands]]")
				.Replace("Rotten Vale", "[[Rotten Vale]]")
				.Replace("Elder's Recess", "[[Elder's Recess]]")
				.Replace("Hoarfrost Reach", "[[Hoarfrost Reach]]")
				.Replace("Guiding Lands", "[[Guiding Lands]]")
				.Replace("<ICON STAR_1>", "★")
				.Replace("<ICON STAR_2>", "☆")
				.Replace("<STYL MOJI_LIGHTBLUE_DEFAULT>", "[[")
				.Replace("</STYL>", " (MHWI Quest)|" + questName + "]]")
				.Replace("leshen", "[[Leshen/MHWI|Leshen]]")
				.Replace("Kulve Taroth", "[[Kulve Taroth/MHWI|Kulve Taroth]]")
				.Replace("Contract: Trouble in the [[Ancient Forest]]", "Contract: Trouble in the Ancient Forest")
				.Replace("A Visitor From Eorzea", "A Visitor from Eorzea");
		}

		private static string GetIconColorName(WikiIconColor color)
		{
			switch (color)
			{
				case WikiIconColor.NA:
					return "White";
				case WikiIconColor.Blue:
					return "Blue";
				case WikiIconColor.Brown:
					return "Brown";
				case WikiIconColor.DarkPurple:
					return "Dark Purple";
				case WikiIconColor.Emerald:
					return "Emerald";
				case WikiIconColor.Gray:
					return "Gray";
				case WikiIconColor.Green:
					return "Green";
				case WikiIconColor.Lemon:
					return "Lemon";
				case WikiIconColor.LightBlue:
					return "Light Blue";
				case WikiIconColor.Moss:
					return "Moss";
				case WikiIconColor.NotAvailable:
					return "White";
				case WikiIconColor.Orange:
					return "Orange";
				case WikiIconColor.Pink:
					return "Pink";
				case WikiIconColor.Purple:
					return "Purple";
				case WikiIconColor.Red:
					return "Red";
				case WikiIconColor.Rose:
					return "Rose";
				case WikiIconColor.Tan:
					return "Tan";
				case WikiIconColor.Violet:
					return "Violet";
				case WikiIconColor.White:
					return "White";
				case WikiIconColor.Yellow:
					return "Yellow";
				case WikiIconColor.Vermilion:
					return "Vermilion";
				case WikiIconColor.LightGreen:
					return "Light Green";
				default: return "White";
			}
		}

		private static string[] GetEquipmentLinks(Models.Data.MHWI.Items item, BlademasterData[] blades, GunnerData[] guns, WeaponCraftingData[] craftingData, WeaponForgingData[] forgingData)
		{
			List<string> res = [];
			foreach (BlademasterData obj in blades)
			{
#nullable enable
				WeaponCraftingData? thisCraft = craftingData.FirstOrDefault(x => x.EquipmentId!.Value == obj.Index!.Value && x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_"));
				if (obj.TreePosition > 0 && thisCraft != null && thisCraft.Mat1Id > 0)
				{
					WeaponCraftingData? parentCraft = craftingData.FirstOrDefault(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && (x.ChildIndex1!.Value == thisCraft.Index!.Value || x.ChildIndex2!.Value == thisCraft.Index!.Value || x.ChildIndex3!.Value == thisCraft.Index!.Value || x.ChildIndex4!.Value == thisCraft.Index!.Value));
					BlademasterData? parent = blades.FirstOrDefault(x => parentCraft != null && x.WeaponType == obj.WeaponType && x.Name == parentCraft.EquipmentName);
					if (parent == null || parentCraft == null)
					{
						WeaponForgingData forge = forgingData.First(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_"));
						parent = blades.FirstOrDefault(x => x.WeaponType == obj.WeaponType && x.Name == forge.EquipmentName);
					}
					if (DoesMatchItem(parent, thisCraft, item))
					{
						res.Add($"[[{parent!.Name} (MHWI)|{parent!.Name}]]");
					}
				}
#nullable disable
				if (forgingData.Any(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_")))
				{
					WeaponForgingData forge = forgingData.First(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_"));
					if (DoesForgeMatchItem(obj, forge, item))
					{
						res.Add($"[[{obj.Name} (MHWI)|{obj.Name}]]");
					}
				}
				if (thisCraft != null)
				{
					if (thisCraft.ChildIndex1 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex1!.Value);
						BlademasterData child = blades.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
					if (thisCraft.ChildIndex2 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex2!.Value);
						BlademasterData child = blades.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
					if (thisCraft.ChildIndex3 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex3!.Value);
						BlademasterData child = blades.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
					if (thisCraft.ChildIndex4 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex4!.Value);
						BlademasterData child = blades.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
				}
			}

			foreach (GunnerData obj in guns)
			{
#nullable enable
				WeaponCraftingData? thisCraft = craftingData.FirstOrDefault(x => x.EquipmentId!.Value == obj.Index!.Value && x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_"));
				if (obj.TreePosition > 0 && thisCraft != null && thisCraft.Mat1Id > 0)
				{
					WeaponCraftingData? parentCraft = craftingData.FirstOrDefault(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && (x.ChildIndex1!.Value == thisCraft.Index!.Value || x.ChildIndex2!.Value == thisCraft.Index!.Value || x.ChildIndex3!.Value == thisCraft.Index!.Value || x.ChildIndex4!.Value == thisCraft.Index!.Value));
					GunnerData? parent = guns.FirstOrDefault(x => parentCraft != null && x.WeaponType == obj.WeaponType && x.Name == parentCraft.EquipmentName);
					if (parent == null || parentCraft == null)
					{
						WeaponForgingData forge = forgingData.First(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_"));
						parent = guns.FirstOrDefault(x => x.WeaponType == obj.WeaponType && x.Name == forge.EquipmentName);
					}
					if (DoesMatchItem(parent, thisCraft, item))
					{
						res.Add($"[[{parent!.Name} (MHWI)|{parent!.Name}]]");
					}
				}
#nullable disable
				if (forgingData.Any(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_")))
				{
					WeaponForgingData forge = forgingData.First(x => x.EquipmentIndex!.Value == obj.Index!.Value && x.EquipmentType == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_"));
					if (DoesForgeMatchItem(obj, forge, item))
					{
						res.Add($"[[{obj.Name} (MHWI)|{obj.Name}]]");
					}
				}
				if (thisCraft != null)
				{
					if (thisCraft.ChildIndex1 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex1!.Value);
						GunnerData child = guns.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
					if (thisCraft.ChildIndex2 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex2!.Value);
						GunnerData child = guns.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
					if (thisCraft.ChildIndex3 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex3!.Value);
						GunnerData child = guns.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
					if (thisCraft.ChildIndex4 > 0)
					{
						WeaponCraftingData childCraft = craftingData.First(x => x.EquipmentCategory == obj.WeaponType.Replace("Great Sword", "Greatsword").Replace("Long Sword", "Longsword").Replace(" ", "_") && x.Index!.Value == thisCraft.ChildIndex4!.Value);
						GunnerData child = guns.First(x => x.WeaponType == obj.WeaponType && x.Index!.Value == childCraft.EquipmentId!.Value);
						if (DoesMatchItem(child, childCraft, item))
						{
							res.Add($"[[{child.Name} (MHWI)|{child.Name}]]");
						}
					}
				}
			}
			return [.. res];
		}

		private static string[] GetArmorStrings(Models.Data.MHWI.Items item, Armor[] allArmor)
		{
			return [..allArmor.Where(x => x.CraftingData != null && new long?[] { x.CraftingData.Mat1Id, x.CraftingData.Mat2Id, x.CraftingData.Mat3Id, x.CraftingData.Mat4Id }.Any(y => y != null && y.HasValue && y.Value == item.Id))
					.Select(x => $"[[{x.SetName} (MHWI)#{x.Name}|{x.Name}]]")];
		}

		private static bool DoesForgeMatchItem(GunnerData parent, WeaponForgingData parentCraft, Models.Data.MHWI.Items item)
		{
			return new Tuple<long?, long?>[] {
				new(parentCraft.Mat1Id, parentCraft.Mat1Count),
				new(parentCraft.Mat2Id, parentCraft.Mat2Count),
				new(parentCraft.Mat3Id, parentCraft.Mat3Count),
				new(parentCraft.Mat4Id, parentCraft.Mat4Count)
			}
			.Where(x => x.Item1.HasValue && x.Item1.Value > 0 && x.Item2.HasValue && x.Item2.Value > 0)
			.Any(x => x.Item1.Value == item.Id);
		}

		private static bool DoesMatchItem(GunnerData parent, WeaponCraftingData parentCraft, Models.Data.MHWI.Items item)
		{
			return new Tuple<long?, long?>[] {
				new(parentCraft.Mat1Id, parentCraft.Mat1Count),
				new(parentCraft.Mat2Id, parentCraft.Mat2Count),
				new(parentCraft.Mat3Id, parentCraft.Mat3Count),
				new(parentCraft.Mat4Id, parentCraft.Mat4Count)
			}
			.Where(x => x.Item1.HasValue && x.Item1.Value > 0 && x.Item2.HasValue && x.Item2.Value > 0)
			.Any(x => x.Item1.Value == item.Id);
		}

		private static bool DoesForgeMatchItem(BlademasterData parent, WeaponForgingData parentCraft, Models.Data.MHWI.Items item)
		{
			return new Tuple<long?, long?>[] {
				new(parentCraft.Mat1Id, parentCraft.Mat1Count),
				new(parentCraft.Mat2Id, parentCraft.Mat2Count),
				new(parentCraft.Mat3Id, parentCraft.Mat3Count),
				new(parentCraft.Mat4Id, parentCraft.Mat4Count)
			}
			.Where(x => x.Item1.HasValue && x.Item1.Value > 0 && x.Item2.HasValue && x.Item2.Value > 0)
			.Any(x => x.Item1.Value == item.Id);
		}

		private static bool DoesMatchItem(BlademasterData parent, WeaponCraftingData parentCraft, Models.Data.MHWI.Items item)
		{
			return new Tuple<long?, long?>[] {
				new(parentCraft.Mat1Id, parentCraft.Mat1Count),
				new(parentCraft.Mat2Id, parentCraft.Mat2Count),
				new(parentCraft.Mat3Id, parentCraft.Mat3Count),
				new(parentCraft.Mat4Id, parentCraft.Mat4Count)
			}
			.Where(x => x.Item1.HasValue && x.Item1.Value > 0 && x.Item2.HasValue && x.Item2.Value > 0)
			.Any(x => x.Item1.Value == item.Id);
		}

		private static string GetMHWIItemJPNName(string itemName)
		{
			if (itemName.StartsWith("Warrior's Streamstone"))
			{
				itemName = "Warrior's Streamstone";
			}
			if (itemName.StartsWith("Hero's Streamstone"))
			{
				itemName = "Hero's Streamstone";
			}
			Dictionary<string, string> translations = new()
			{
				{ "Potion", "回復薬" },
				{ "Mega Potion", "回復薬グレート" },
				{ "Max Potion", "秘薬" },
				{ "Ancient Potion", "いにしえの秘薬" },
				{ "Antidote", "解毒薬" },
				{ "Herbal Medicine", "漢方薬" },
				{ "Nulberry", "ウチケシの実" },
				{ "Energy Drink", "元気ドリンコ" },
				{ "Ration", "携帯食料" },
				{ "Rare Steak", "生焼け肉" },
				{ "Well-done Steak", "こんがり肉" },
				{ "Burnt Meat", "コゲ肉" },
				{ "Cool Drink", "クーラードリンク" },
				{ "Nutrients", "栄養剤" },
				{ "Mega Nutrients", "栄養剤グレート" },
				{ "Immunizer", "活力剤" },
				{ "Mosswine Jerky", "モスジャーキー" },
				{ "Dash Juice", "強走薬" },
				{ "Mega Dash Juice", "強走薬グレート" },
				{ "Might Seed", "怪力の種" },
				{ "Demondrug", "鬼人薬" },
				{ "Mega Demondrug", "鬼人薬グレート" },
				{ "Might Pill", "怪力の丸薬" },
				{ "Adamant Seed", "忍耐の種" },
				{ "Armorskin", "硬化薬" },
				{ "Mega Armorskin", "硬化薬グレート" },
				{ "Adamant Pill", "忍耐の丸薬" },
				{ "Lifepowder", "生命の粉塵" },
				{ "Herbal Powder", "漢方の粉塵" },
				{ "Demon Powder", "鬼人の粉塵" },
				{ "Hardshell Powder", "硬化の粉塵" },
				{ "Honey", "ハチミツ" },
				{ "Herb", "薬草" },
				{ "Antidote Herb", "げどく草" },
				{ "Fire Herb", "火薬草" },
				{ "Flowfern", "流水草" },
				{ "Snow Herb", "霜ふり草" },
				{ "Sleep Herb", "ネムリ草" },
				{ "Ivy", "ツタの葉" },
				{ "Smokenut", "ケムリの実" },
				{ "Dragonfell Berry", "龍殺しの実" },
				{ "Blue Mushroom", "アオキノコ" },
				{ "Mandragora", "マンドラゴラ" },
				{ "Nitroshroom", "ニトロダケ" },
				{ "Devil's Blight", "鬼ニトロダケ" },
				{ "Parashroom", "マヒダケ" },
				{ "Toadstool", "毒テングダケ" },
				{ "Exciteshroom", "ドキドキノコ" },
				{ "Bitterbug", "にが虫" },
				{ "Flashbug", "光蟲" },
				{ "Godbug", "不死虫" },
				{ "Thunderbug", "雷光虫" },
				{ "Baitbug", "イレグイコガネ" },
				{ "Kelbi Horn", "ケルビの角" },
				{ "Dash Extract", "狂走エキス" },
				{ "Nourishing Extract", "滋養エキス" },
				{ "Screamer Sac", "鳴き袋" },
				{ "Catalyst", "増強剤" },
				{ "Tranq Bomb", "捕獲用麻酔玉" },
				{ "Flash Pod", "スリンガー閃光弾" },
				{ "Screamer Pod", "スリンガー音爆弾" },
				{ "Smoke Bomb", "けむり玉" },
				{ "Poison Smoke Bomb", "毒けむり玉" },
				{ "Farcaster", "モドリ玉" },
				{ "Raw Meat", "生肉" },
				{ "Poisoned Meat", "毒生肉" },
				{ "Tinged Meat", "シビレ生肉" },
				{ "Drugged Meat", "眠り生肉" },
				{ "Gunpowder", "爆薬" },
				{ "Small Barrel", "小タル" },
				{ "Barrel Bomb", "小タル爆弾" },
				{ "Bounce Bomb", "打上げタル爆弾" },
				{ "Mega Bounce Bomb", "打上げタル爆弾Ｇ" },
				{ "Large Barrel", "大タル" },
				{ "Large Barrel Bomb", "大タル爆弾" },
				{ "Mega Barrel Bomb", "大タル爆弾Ｇ" },
				{ "Spider Web", "クモの巣" },
				{ "Net", "ネット" },
				{ "Trap Tool", "トラップツール" },
				{ "Pitfall Trap", "落とし穴" },
				{ "Shock Trap", "シビレ罠" },
				{ "Rolled-up Dung", "ころがされたフン" },
				{ "Dung", "モンスターのフン" },
				{ "Dung Pod", "スリンガーこやし弾" },
				{ "Arowana Bait", "アロワナダンゴ" },
				{ "Gunpowderfish Bait", "デメキンダンゴ" },
				{ "Goldenfish Bait", "黄金ダンゴ" },
				{ "Boomerang", "ブーメラン" },
				{ "Binoculars", "双眼鏡" },
				{ "Powercharm", "力の護符" },
				{ "Powertalon", "力の爪" },
				{ "Armorcharm", "守りの護符" },
				{ "Armortalon", "守りの爪" },
				{ "Needleberry", "ハリの実" },
				{ "Blastnut", "バクレツの実" },
				{ "Dragonstrike Nut", "リュウゲキの実" },
				{ "Slashberry", "ザンレツの実" },
				{ "Latchberry", "ツラヌキの実" },
				{ "Bomberry", "カクサンの実" },
				{ "Flamenut", "チャッカの実" },
				{ "Blazenut", "ハッカの実" },
				{ "Gunpowder Level 2", "LV2 火薬粉" },
				{ "Gunpowder Level 3", "LV3 火薬粉" },
				{ "Whetfish Fin", "キレアジのヒレ" },
				{ "Whetfish Fin+", "キレアジの上ヒレ" },
				{ "Sushifish Scale", "サシミウロコ" },
				{ "Great Sushifish Scale", "大サシミウロコ" },
				{ "Gunpowderfish Scale", "バクヤクウロコ" },
				{ "Great Gunpowderfish Scale", "大バクヤクウロコ" },
				{ "Burst Arowana Scale", "ハレツウロコ" },
				{ "Great Burst Arowana Scale", "大ハレツウロコ" },
				{ "Bomb Arowana Scale", "バクレツウロコ" },
				{ "Great Bomb Arowana Scale", "大バクレツウロコ" },
				{ "Whetstone", "砥石" },
				{ "Capture Net", "捕獲用ネット" },
				{ "Fishing Rod", "釣り竿" },
				{ "BBQ Spit", "肉焼きセット" },
				{ "Ghillie Mantle", "隠れ身の装衣" },
				{ "Temporal Mantle", "転身の装衣" },
				{ "Health Booster", "癒しの煙筒" },
				{ "Rocksteady Mantle", "不動の装衣" },
				{ "Challenger Mantle", "挑発の装衣" },
				{ "Vitality Mantle", "体力の装衣" },
				{ "Fireproof Mantle", "耐熱の装衣" },
				{ "Waterproof Mantle", "耐水の装衣" },
				{ "Iceproof Mantle", "耐寒の装衣" },
				{ "Thunderproof Mantle", "耐雷の装衣" },
				{ "Dragonproof Mantle", "耐龍の装衣" },
				{ "Cleanser Booster", "解除の煙筒" },
				{ "Glider Mantle", "滑空の装衣" },
				{ "Evasion Mantle", "回避の装衣" },
				{ "Impact Mantle", "強打の装衣" },
				{ "Apothecary Mantle", "化合の装衣" },
				{ "Immunity Mantle", "免疫の装衣" },
				{ "Affinity Booster", "達人の煙筒" },
				{ "Bandit Mantle", "追い剥ぎの装衣" },
				{ "Normal Ammo 1", "LV1 通常弾" },
				{ "Normal Ammo 2", "LV2 通常弾" },
				{ "Normal Ammo 3", "LV3 通常弾" },
				{ "Pierce Ammo 1", "LV1 貫通弾" },
				{ "Pierce Ammo 2", "LV2 貫通弾" },
				{ "Pierce Ammo 3", "LV3 貫通弾" },
				{ "Spread Ammo 1", "LV1 散弾" },
				{ "Spread Ammo 2", "LV2 散弾" },
				{ "Spread Ammo 3", "LV3 散弾" },
				{ "Sticky Ammo 1", "LV1 徹甲榴弾" },
				{ "Sticky Ammo 2", "LV2 徹甲榴弾" },
				{ "Sticky Ammo 3", "LV3 徹甲榴弾" },
				{ "Cluster Bomb 1", "LV1 拡散弾" },
				{ "Cluster Bomb 2", "LV2 拡散弾" },
				{ "Cluster Bomb 3", "LV3 拡散弾" },
				{ "Flaming Ammo", "火炎弾" },
				{ "Water Ammo", "水冷弾" },
				{ "Thunder Ammo", "電撃弾" },
				{ "Freeze Ammo", "氷結弾" },
				{ "Dragon Ammo", "滅龍弾" },
				{ "Poison Ammo 1", "LV1 毒弾" },
				{ "Poison Ammo 2", "LV2 毒弾" },
				{ "Paralysis Ammo 1", "LV1 麻痺弾" },
				{ "Paralysis Ammo 2", "LV2 麻痺弾" },
				{ "Sleep Ammo 1", "LV1 睡眠弾" },
				{ "Sleep Ammo 2", "LV2 睡眠弾" },
				{ "Exhaust Ammo 1", "LV1 減気弾" },
				{ "Exhaust Ammo 2", "LV2 減気弾" },
				{ "Recover Ammo 1", "LV1 回復弾" },
				{ "Recover Ammo 2", "LV2 回復弾" },
				{ "Wyvern Ammo", "竜撃弾" },
				{ "Slicing Ammo", "斬裂弾" },
				{ "Tranq Ammo", "捕獲用麻酔弾" },
				{ "Demon Ammo", "鬼人弾" },
				{ "Armor Ammo", "硬化弾" },
				{ "Close-range Coating", "接撃ビン" },
				{ "Empty Phial", "空きビン" },
				{ "Power Coating", "強撃ビン" },
				{ "Poison Coating", "毒ビン" },
				{ "Paralysis Coating", "麻痺ビン" },
				{ "Sleep Coating", "睡眠ビン" },
				{ "Exhaust Coating", "減気ビン" },
				{ "Blast Coating", "爆破ビン" },
				{ "First-aid Med", "応急薬" },
				{ "First-aid Med+", "応急薬グレート" },
				{ "EZ Ration", "支給用携帯食料" },
				{ "EZ Lifepowder", "支給用生命の粉塵" },
				{ "EZ Max Potion", "支給用秘薬" },
				{ "EZ Large Barrel Bomb", "支給用大タル爆弾" },
				{ "EZ Shock Trap", "支給用シビレ罠" },
				{ "EZ Pitfall Trap", "支給用落とし穴" },
				{ "EZ Herbal Powder", "支給用漢方の粉塵" },
				{ "EZ Demon Powder", "支給用鬼人の粉塵" },
				{ "EZ Hardshell Powder", "支給用硬化の粉塵" },
				{ "EZ Dung Pod", "支給用こやし弾" },
				{ "EZ Flash Pod", "支給用閃光弾" },
				{ "EZ Screamer Pod", "支給用音爆弾" },
				{ "Throwing Knife", "投げナイフ" },
				{ "Poison Knife", "毒投げナイフ" },
				{ "Sleep Knife", "眠り投げナイフ" },
				{ "Paralysis Knife", "麻痺投げナイフ" },
				{ "Tranq Knife", "捕獲用麻酔投げナイフ" },
				{ "EZ Farcaster", "支給用モドリ玉" },
				{ "EZ Tranq Bomb", "支給用麻酔玉" },
				{ "Ballista Ammo", "バリスタの弾" },
				{ "One-shot Binder", "単発式拘束弾" },
				{ "Cannon Ammo", "大砲の弾" },
				{ "Iron Ore", "鉄鉱石" },
				{ "Machalite Ore", "マカライト鉱石" },
				{ "Dragonite Ore", "ドラグライト鉱石" },
				{ "Carbalite Ore", "カブレライト鉱石" },
				{ "Fucium Ore", "ユニオン鉱石" },
				{ "Earth Crystal", "大地の結晶" },
				{ "Coral Crystal", "深海の結晶" },
				{ "Dragonvein Crystal", "龍脈の結晶" },
				{ "Lightcrystal", "ライトクリスタル" },
				{ "Novacrystal", "ノヴァクリスタル" },
				{ "Firecell Stone", "獄炎石" },
				{ "Aquacore Ore", "水晶原石" },
				{ "Spiritcore Ore", "精晶原石" },
				{ "Dreamcore Ore", "幻晶原石" },
				{ "Dragoncore Ore", "龍晶原石" },
				{ "Armor Sphere", "鎧玉" },
				{ "Armor Sphere+", "上鎧玉" },
				{ "Advanced Armor Sphere", "尖鎧玉" },
				{ "Hard Armor Sphere", "堅鎧玉" },
				{ "Heavy Armor Sphere", "重鎧玉" },
				{ "Sturdy Bone", "頑丈な骨" },
				{ "Quality Bone", "上質な堅骨" },
				{ "Ancient Bone", "太古の大骨" },
				{ "Boulder Bone", "強固な岩骨" },
				{ "Coral Bone", "サンゴの紅骨" },
				{ "Warped Bone", "いびつな狂骨" },
				{ "Brutal Bone", "荒々しい蛮骨" },
				{ "Dragonbone Relic", "いにしえの龍骨" },
				{ "Unknown Skull", "なぞの頭骨" },
				{ "Great Hornfly", "オオツノアゲハ" },
				{ "Sinister Cloth", "禍々しい布" },
				{ "Monster Bone S", "竜骨【小】" },
				{ "Monster Bone M", "竜骨【中】" },
				{ "Monster Bone L", "竜骨【大】" },
				{ "Monster Bone+", "上竜骨" },
				{ "Monster Keenbone", "尖竜骨" },
				{ "Monster Hardbone", "堅竜骨" },
				{ "Elder Dragon Bone", "古龍骨" },
				{ "Sharp Claw", "とがった爪" },
				{ "Piercing Claw", "鋭利な爪" },
				{ "Monster Fluid", "モンスターの体液" },
				{ "Monster Broth", "モンスターの濃汁" },
				{ "Poison Sac", "毒袋" },
				{ "Toxin Sac", "猛毒袋" },
				{ "Paralysis Sac", "麻痺袋" },
				{ "Omniplegia Sac", "強力麻痺袋" },
				{ "Sleep Sac", "睡眠袋" },
				{ "Coma Sac", "昏睡袋" },
				{ "Flame Sac", "火炎袋" },
				{ "Inferno Sac", "爆炎袋" },
				{ "Aqua Sac", "水袋" },
				{ "Torrent Sac", "大水袋" },
				{ "Frost Sac", "氷結袋" },
				{ "Freezer Sac", "凍結袋" },
				{ "Electro Sac", "電気袋" },
				{ "Thunder Sac", "電撃袋" },
				{ "Bird Wyvern Gem", "鳥竜玉" },
				{ "Wyvern Gem", "竜玉" },
				{ "Elder Dragon Blood", "古龍の血" },
				{ "Mosswine Hide", "モスの苔皮" },
				{ "Warm Pelt", "暖かい毛皮" },
				{ "High-quality Pelt", "上質な毛皮" },
				{ "Vespoid Shell", "ランゴスタの甲殻" },
				{ "Vespoid Carapace", "ランゴスタの堅殻" },
				{ "Vespoid Wing", "ランゴスタの羽" },
				{ "Vespoid Innerwing", "ランゴスタの薄羽" },
				{ "Hornetaur Shell", "カンタロスの甲殻" },
				{ "Hornetaur Wing", "カンタロスの羽" },
				{ "Hornetaur Head", "カンタロスの頭" },
				{ "Hornetaur Carapace", "カンタロスの堅殻" },
				{ "Hornetaur Innerwing", "カンタロスの薄羽" },
				{ "Gajau Skin", "咬魚の皮" },
				{ "Gajau Whisker", "咬魚のヒゲ" },
				{ "Gajau Scale", "咬魚の上皮" },
				{ "Grand Gajau Whisker", "咬魚の大ヒゲ" },
				{ "Wingdrake Hide", "翼竜の皮" },
				{ "Wingdrake Hide+", "翼竜の上皮" },
				{ "Barnos Hide+", "バルノスの上皮" },
				{ "Barnos Talon", "バルノスの尖爪" },
				{ "Kestodon Shell", "ケストドンの甲殻" },
				{ "Kestodon Scalp", "ケストドンの頭殻" },
				{ "Kestodon Carapace", "ケストドンの堅殻" },
				{ "Gastodon Carapace", "ガストドンの堅殻" },
				{ "Gastodon Horn", "ガストドンの尖角" },
				{ "Jagras Scale", "ジャグラスの鱗" },
				{ "Jagras Hide", "ジャグラスの皮" },
				{ "Jagras Scale+", "ジャグラスの上鱗" },
				{ "Jagras Hide+", "ジャグラスの上皮" },
				{ "Shamos Scale", "シャムオスの鱗" },
				{ "Shamos Hide", "シャムオスの皮" },
				{ "Shamos Scale+", "シャムオスの上鱗" },
				{ "Shamos Hide+", "シャムオスの上皮" },
				{ "Girros Scale", "ギルオスの鱗" },
				{ "Girros Hide", "ギルオスの皮" },
				{ "Girros Fang", "ギルオスの麻痺牙" },
				{ "Girros Scale+", "ギルオスの上鱗" },
				{ "Girros Hide+", "ギルオスの上皮" },
				{ "Great Jagras Scale", "賊竜の鱗" },
				{ "Great Jagras Hide", "賊竜の皮" },
				{ "Great Jagras Mane", "賊竜のたてがみ" },
				{ "Great Jagras Claw", "賊竜の爪" },
				{ "Great Jagras Scale+", "賊竜の上鱗" },
				{ "Great Jagras Hide+", "賊竜の上皮" },
				{ "Great Jagras Claw+", "賊竜の尖爪" },
				{ "Kulu-Ya-Ku Scale", "掻鳥の鱗" },
				{ "Kulu-Ya-Ku Hide", "掻鳥の皮" },
				{ "Kulu-Ya-Ku Plume", "掻鳥の飾り羽" },
				{ "Kulu-Ya-Ku Beak", "掻鳥のクチバシ" },
				{ "Kulu-Ya-Ku Scale+", "掻鳥の上鱗" },
				{ "Kulu-Ya-Ku Hide+", "掻鳥の上皮" },
				{ "Kulu-Ya-Ku Plume+", "掻鳥の大飾り羽" },
				{ "Kulu-Ya-Ku Beak+", "掻鳥の大クチバシ" },
				{ "Pukei-Pukei Scale", "毒妖鳥の鱗" },
				{ "Pukei-Pukei Shell", "毒妖鳥の甲殻" },
				{ "Pukei-Pukei Quill", "毒妖鳥の羽根" },
				{ "Pukei-Pukei Sac", "毒妖鳥の喉袋" },
				{ "Pukei-Pukei Tail", "毒妖鳥の尻尾" },
				{ "Pukei-Pukei Scale+", "毒妖鳥の上鱗" },
				{ "Pukei-Pukei Carapace", "毒妖鳥の堅殻" },
				{ "Pukei-Pukei Wing", "毒妖鳥の翼" },
				{ "Pukei-Pukei Sac+", "毒妖鳥の大喉袋" },
				{ "Barroth Shell", "土砂竜の甲殻" },
				{ "Barroth Ridge", "土砂竜の背甲" },
				{ "Barroth Claw", "土砂竜の爪" },
				{ "Barroth Scalp", "土砂竜の頭殻" },
				{ "Barroth Tail", "土砂竜の尻尾" },
				{ "Fertile Mud", "肥沃なドロ" },
				{ "Barroth Carapace", "土砂竜の堅殻" },
				{ "Barroth Ridge+", "土砂竜の堅甲" },
				{ "Barroth Claw+", "土砂竜の鋭爪" },
				{ "Jyuratodus Scale", "泥魚竜の鱗" },
				{ "Jyuratodus Shell", "泥魚竜の甲殻" },
				{ "Jyuratodus Fang", "泥魚竜の牙" },
				{ "Jyuratodus Fin", "泥魚竜のヒレ" },
				{ "Jyuratodus Scale+", "泥魚竜の上鱗" },
				{ "Jyuratodus Carapace", "泥魚竜の堅殻" },
				{ "Jyuratodus Fang+", "泥魚竜の鋭牙" },
				{ "Jyuratodus Fin+", "泥魚竜の上ヒレ" },
				{ "Tobi-Kadachi Scale", "飛雷竜の鱗" },
				{ "Tobi-Kadachi Pelt", "飛雷竜の毛皮" },
				{ "Tobi-Kadachi Membrane", "飛雷竜の皮膜" },
				{ "Tobi-Kadachi Claw", "飛雷竜の爪" },
				{ "Tobi-Kadachi Electrode", "飛雷竜の電極針" },
				{ "Tobi-Kadachi Scale+", "飛雷竜の上鱗" },
				{ "Tobi-Kadachi Pelt+", "飛雷竜の上毛皮" },
				{ "Tobi-Kadachi Claw+", "飛雷竜の尖爪" },
				{ "Tobi-Kadachi Electrode+", "飛雷竜の雷極針" },
				{ "Anjanath Scale", "蛮顎竜の鱗" },
				{ "Anjanath Pelt", "蛮顎竜の毛皮" },
				{ "Anjanath Fang", "蛮顎竜の牙" },
				{ "Anjanath Nosebone", "蛮顎竜の鼻骨" },
				{ "Anjanath Tail", "蛮顎竜の尻尾" },
				{ "Anjanath Plate", "蛮顎竜の逆鱗" },
				{ "Anjanath Scale+", "蛮顎竜の上鱗" },
				{ "Anjanath Pelt+", "蛮顎竜の上毛皮" },
				{ "Anjanath Fang+", "蛮顎竜の鋭牙" },
				{ "Anjanath Nosebone+", "蛮顎竜の大鼻骨" },
				{ "Anjanath Gem", "蛮顎竜の宝玉" },
				{ "Rathian Scale", "雌火竜の鱗" },
				{ "Rathian Shell", "雌火竜の甲殻" },
				{ "Rathian Webbing", "雌火竜の翼膜" },
				{ "Rathian Spike", "雌火竜の棘" },
				{ "Rathian Plate", "雌火竜の逆鱗" },
				{ "Rathian Scale+", "雌火竜の上鱗" },
				{ "Rathian Carapace", "雌火竜の堅殻" },
				{ "Rathian Spike+", "雌火竜の上棘" },
				{ "Rathian Ruby", "雌火竜の紅玉" },
				{ "Pink Rathian Scale+", "桜火竜の上鱗" },
				{ "Pink Rathian Carapace", "桜火竜の堅殻" },
				{ "Tzitzi-Ya-Ku Scale", "眩鳥の鱗" },
				{ "Tzitzi-Ya-Ku Hide", "眩鳥の皮" },
				{ "Tzitzi-Ya-Ku Claw", "眩鳥の爪" },
				{ "Tzitzi-Ya-Ku Photophore", "眩鳥の発光膜" },
				{ "Tzitzi-Ya-Ku Scale+", "眩鳥の上鱗" },
				{ "Tzitzi-Ya-Ku Hide+", "眩鳥の上皮" },
				{ "Tzitzi-Ya-Ku Claw+", "眩鳥の尖爪" },
				{ "Tzitzi-Ya-Ku Photophore+", "眩鳥の閃光膜" },
				{ "Paolumu Pelt", "浮空竜の毛皮" },
				{ "Paolumu Scale", "浮空竜の鱗" },
				{ "Paolumu Shell", "ゴム質の甲殻" },
				{ "Paolumu Webbing", "浮空竜の翼膜" },
				{ "Paolumu Pelt+", "浮空竜の上毛皮" },
				{ "Paolumu Scale+", "浮空竜の上鱗" },
				{ "Paolumu Carapace+", "ゴム質の堅殻" },
				{ "Paolumu Wing", "浮空竜の翼" },
				{ "Great Girros Scale", "痺賊竜の鱗" },
				{ "Great Girros Hide", "痺賊竜の皮" },
				{ "Great Girros Hood", "痺賊竜の頭巾殻" },
				{ "Great Girros Fang", "痺賊竜の牙" },
				{ "Great Girros Tail", "痺賊竜の尻尾" },
				{ "Great Girros Scale+", "痺賊竜の上鱗" },
				{ "Great Girros Hide+", "痺賊竜の上皮" },
				{ "Great Girros Hood+", "痺賊竜の大頭巾" },
				{ "Great Girros Fang+", "痺賊竜の鋭牙" },
				{ "Radobaan Scale", "骨鎚竜の鱗" },
				{ "Radobaan Shell", "骨鎚竜の甲殻" },
				{ "Radobaan Oilshell", "骨鎚竜の黒油殻" },
				{ "Wyvern Bonemass", "竜骨塊" },
				{ "Radobaan Jaw", "骨鎚竜の顎" },
				{ "Radobaan Marrow", "骨鎚竜の骨髄" },
				{ "Radobaan Scale+", "骨鎚竜の上鱗" },
				{ "Radobaan Carapace", "骨鎚竜の堅殻" },
				{ "Radobaan Medulla", "骨鎚竜の延髄" },
				{ "Legiana Scale", "風漂竜の鱗" },
				{ "Legiana Hide", "風漂竜の皮" },
				{ "Legiana Claw", "風漂竜の爪" },
				{ "Legiana Webbing", "風漂竜の翼膜" },
				{ "Legiana Tail Webbing", "風漂竜の尾膜" },
				{ "Legiana Plate", "風漂竜の逆鱗" },
				{ "Legiana Scale+", "風漂竜の上鱗" },
				{ "Legiana Hide+", "風漂竜の上皮" },
				{ "Legiana Claw+", "風漂竜の尖爪" },
				{ "Legiana Wing", "風漂竜の翼" },
				{ "Legiana Gem", "風漂竜の宝玉" },
				{ "Odogaron Scale", "惨爪竜の鱗" },
				{ "Odogaron Sinew", "惨爪竜の硬筋" },
				{ "Odogaron Claw", "惨爪竜の爪" },
				{ "Odogaron Fang", "惨爪竜の牙" },
				{ "Odogaron Tail", "惨爪竜の尻尾" },
				{ "Odogaron Plate", "惨爪竜の逆鱗" },
				{ "Odogaron Scale+", "惨爪竜の上鱗" },
				{ "Odogaron Sinew+", "惨爪竜の大硬筋" },
				{ "Odogaron Claw+", "惨爪竜の尖爪" },
				{ "Odogaron Fang+", "惨爪竜の鋭牙" },
				{ "Odogaron Gem", "惨爪竜の宝玉" },
				{ "Rathalos Scale", "火竜の鱗" },
				{ "Rathalos Shell", "火竜の甲殻" },
				{ "Rathalos Webbing", "火竜の翼膜" },
				{ "Rathalos Tail", "火竜の尻尾" },
				{ "Rath Wingtalon", "火竜の翼爪" },
				{ "Rathalos Marrow", "火竜の骨髄" },
				{ "Rathalos Plate", "火竜の逆鱗" },
				{ "Rathalos Scale+", "火竜の上鱗" },
				{ "Rathalos Carapace", "火竜の堅殻" },
				{ "Rathalos Wing", "火竜の翼" },
				{ "Rathalos Medulla", "火竜の延髄" },
				{ "Rathalos Ruby", "火竜の紅玉" },
				{ "Azure Rathalos Scale+", "蒼火竜の上鱗" },
				{ "Azure Rathalos Carapace", "蒼火竜の堅殻" },
				{ "Azure Rathalos Tail", "蒼火竜の尻尾" },
				{ "Azure Rathalos Wing", "蒼火竜の翼" },
				{ "Diablos Shell", "角竜の甲殻" },
				{ "Diablos Ridge", "角竜の背甲" },
				{ "Diablos Tailcase", "角竜の尾甲" },
				{ "Diablos Fang", "角竜の牙" },
				{ "Twisted Horn", "ねじれた角" },
				{ "Diablos Marrow", "角竜の骨髄" },
				{ "Diablos Carapace", "角竜の堅殻" },
				{ "Diablos Ridge+", "角竜の堅甲" },
				{ "Majestic Horn", "上質なねじれた角" },
				{ "Blos Medulla", "角竜の延髄" },
				{ "Black Diablos Carapace", "黒角竜の堅殻" },
				{ "Black Diablos Ridge+", "黒角竜の堅甲" },
				{ "Black Spiral Horn+", "上質な黒巻き角" },
				{ "Kirin Hide", "幻獣の皮" },
				{ "Kirin Tail", "幻獣の尾" },
				{ "Kirin Mane", "幻獣のたてがみ" },
				{ "Kirin Thunderhorn", "幻獣の雷角" },
				{ "Kirin Hide+", "幻獣の上皮" },
				{ "Kirin Thundertail", "幻獣の雷尾" },
				{ "Kirin Azure Horn", "幻獣の蒼角" },
				{ "Zorah Magdaros Inner Scale", "熔山龍の内鱗" },
				{ "Zorah Magdaros Heat Scale", "熔山龍の熱鱗" },
				{ "Zorah Magdaros Carapace", "熔山龍の岩殻" },
				{ "Zorah Magdaros Ridge", "熔山龍の背甲" },
				{ "Zorah Magdaros Pleura", "熔山龍の胸殻" },
				{ "Zorah Magdaros Brace", "熔山龍の腕甲" },
				{ "Zorah Magdaros Magma", "熔山龍のマグマ" },
				{ "Zorah Magdaros Gem", "熔山龍の宝玉" },
				{ "Dodogama Scale+", "岩賊竜の上鱗" },
				{ "Dodogama Hide+", "岩賊竜の上皮" },
				{ "Dodogama Jaw", "岩賊竜の顎" },
				{ "Dodogama Talon", "岩賊竜の尖爪" },
				{ "Dodogama Tail", "岩賊竜の尻尾" },
				{ "Lavasioth Scale+", "溶岩竜の上鱗" },
				{ "Lavasioth Carapace", "溶岩竜の堅殻" },
				{ "Lavasioth Fang+", "溶岩竜の鋭牙" },
				{ "Lavasioth Fin+", "溶岩竜の上ヒレ" },
				{ "Uragaan Scale+", "爆鎚竜の上鱗" },
				{ "Uragaan Carapace", "爆鎚竜の堅殻" },
				{ "Uragaan Jaw", "爆鎚竜の顎" },
				{ "Uragaan Scute", "爆鎚竜の耐熱殻" },
				{ "Uragaan Marrow", "爆鎚竜の延髄" },
				{ "Uragaan Ruby", "爆鎚竜の紅玉" },
				{ "Lava Nugget", "溶岩塊" },
				{ "Bazelgeuse Scale+", "爆鱗竜の上鱗" },
				{ "Bazelgeuse Carapace", "爆鱗竜の堅殻" },
				{ "Bazelgeuse Tail", "爆鱗竜の尻尾" },
				{ "Bazelgeuse Fuse", "爆鱗竜の爆線" },
				{ "Bazelgeuse Talon", "爆鱗竜の尖爪" },
				{ "Bazelgeuse Wing", "爆鱗竜の翼" },
				{ "Bazelgeuse Gem", "爆鱗竜の宝玉" },
				{ "Immortal Dragonscale", "不滅の龍鱗" },
				{ "Nergigante Carapace", "滅尽龍の堅殻" },
				{ "Nergigante Barbs", "滅尽龍の棘束" },
				{ "Nergigante Tail", "滅尽龍の尻尾" },
				{ "Nergigante Horn+", "滅尽龍の大角" },
				{ "Nergigante Talon", "滅尽龍の尖爪" },
				{ "Nergigante Regrowth Plate", "滅尽龍の再生殻" },
				{ "Nergigante Gem", "滅尽龍の宝玉" },
				{ "Deceased Scale", "死屍の龍鱗" },
				{ "Vaal Hazak Carapace", "屍套龍の堅殻" },
				{ "Vaal Hazak Membrane", "屍套龍の被膜" },
				{ "Vaal Hazak Tail", "屍套龍の尻尾" },
				{ "Vaal Hazak Fang+", "屍套龍の鋭牙" },
				{ "Vaal Hazak Talon", "屍套龍の尖爪" },
				{ "Vaal Hazak Wing", "屍套龍の翼" },
				{ "Vaal Hazak Miasmacryst", "屍套龍の瘴結晶" },
				{ "Vaal Hazak Gem", "屍套龍の宝玉" },
				{ "Teostra Carapace", "炎王龍の堅殻" },
				{ "Teostra Mane", "炎王龍のたてがみ" },
				{ "Teostra Tail", "炎王龍の尻尾" },
				{ "Teostra Horn+", "炎王龍の尖角" },
				{ "Fire Dragon Scale+", "獄炎の龍鱗" },
				{ "Teostra Claw+", "炎龍の尖爪" },
				{ "Teostra Webbing", "炎龍の翼" },
				{ "Teostra Powder", "炎龍の塵粉" },
				{ "Teostra Gem", "炎龍の宝玉" },
				{ "Daora Carapace", "鋼龍の堅殻" },
				{ "Daora Dragon Scale+", "鋼の上龍鱗" },
				{ "Daora Webbing", "鋼龍の翼" },
				{ "Daora Horn+", "鋼龍の尖角" },
				{ "Daora Tail", "鋼龍の尻尾" },
				{ "Daora Claw+", "鋼龍の尖爪" },
				{ "Daora Gem", "鋼龍の宝玉" },
				{ "Xeno'jiiva Soulscale", "冥灯龍の幽鱗" },
				{ "Xeno'jiiva Shell", "冥灯龍の白殻" },
				{ "Xeno'jiiva Veil", "冥灯龍の幽幕" },
				{ "Xeno'jiiva Tail", "冥灯龍の尻尾" },
				{ "Xeno'jiiva Horn", "冥灯龍の幽角" },
				{ "Xeno'jiiva Claw", "冥灯龍の幽爪" },
				{ "Xeno'jiiva Wing", "冥灯龍の幽翼" },
				{ "Xeno'jiiva Crystal", "冥灯龍の幽結晶" },
				{ "Xeno'jiiva Gem", "冥灯龍の幽玉" },
				{ "??? Scale", "？？？の鱗" },
				{ "??? Shell", "？？？の甲殻" },
				{ "??? Membrane", "？？？の皮膜" },
				{ "??? Tail", "？？？の尻尾" },
				{ "??? Horn", "？？？の角" },
				{ "??? Claw", "？？？の爪" },
				{ "??? Wing", "？？？の翼" },
				{ "??? Crystal", "？？？の結晶" },
				{ "??? Gem", "？？？の宝玉" },
				{ "Mysterious Feystone", "なぞの珠" },
				{ "Glowing Feystone", "光る珠" },
				{ "Worn Feystone", "古びた珠" },
				{ "Warped Feystone", "風化した珠" },
				{ "Sullied Streamstone", "汚れた龍脈石" },
				{ "Shining Streamstone", "輝く龍脈石" },
				{ "Streamstone Shard", "龍脈石のかけら" },
				{ "Streamstone", "龍脈石" },
				{ "Gleaming Streamstone", "大龍脈石" },
				{ "Warrior's Streamstone", "猛者の龍脈石・剣" },
				{ "Hero's Streamstone", "英雄の龍脈石・剣" },
				{ "Voucher", "お食事券" },
				{ "First Wyverian Print", "古代竜人の手形" },
				{ "Deluxe First Wyverian Print", "古代竜人の手形Ｇ" },
				{ "Steel Wyverian Print", "鋼の竜人手形" },
				{ "Silver Wyverian Print", "銀の竜人手形" },
				{ "Gold Wyverian Print", "金の竜人手形" },
				{ "Commendation", "勇気の証" },
				{ "High Commendation", "勇気の証Ｇ" },
				{ "Research Commission Ticket", "調査団チケット" },
				{ "Pukei Coin", "プケプケコイン" },
				{ "Kulu Coin", "クルルコイン" },
				{ "Rathian Coin", "レイアコイン" },
				{ "Tzitzi Coin", "ツィツィコイン" },
				{ "Barroth Coin", "ボロスコイン" },
				{ "Gama Coin", "ガマルコイン" },
				{ "Rathalos Coin", "レウスコイン" },
				{ "Brute Coin", "獣竜コイン" },
				{ "Flying Coin", "飛竜コイン" },
				{ "Pinnacle Coin", "闘技王のコイン" },
				{ "Hunter King Coin", "狩猟王のコイン" },
				{ "Ace Hunter Coin", "撃龍王のコイン" },
				{ "Steel Egg", "鋼のたまご" },
				{ "Silver Egg", "銀のたまご" },
				{ "Golden Egg", "金のたまご" },
				{ "Chipped Scale", "欠けたウロコ" },
				{ "Large Scale", "大きなウロコ" },
				{ "Beautiful Scale", "きれいなウロコ" },
				{ "Lustrous Scale", "つやめくウロコ" },
				{ "Glimmering Scale", "かがやくウロコ" },
				{ "Bhernastone", "ベルナストーン" },
				{ "Dundormarin", "ドンドルマリン" },
				{ "Loc Lac Ore", "ロックラック鉱" },
				{ "Val Habar Quartz", "バルバレクォーツ" },
				{ "Minegardenite", "ミナガルデナイト" },
				{ "Golden Scale", "黄金のウロコ" },
				{ "Golden Scale+", "黄金の大ウロコ" },
				{ "Platinum Scale", "白金のウロコ" },
				{ "Platinum Scale+", "白金の大ウロコ" },
				{ "Gilded Scale", "金色のウロコ" },
				{ "Gilded Scale+", "金色の大ウロコ" },
				{ "White Liver", "ホワイトレバー" },
				{ "Wyvern Tear", "竜のナミダ" },
				{ "Large Wyvern Tear", "竜の大粒ナミダ" },
				{ "Dragon Treasure", "龍秘宝" },
				{ "Old Dragon Treasure", "いにしえの龍秘宝" },
				{ "Sunbloom", "サニーフラワー" },
				{ "Shinebloom", "シャインフラワー" },
				{ "Goldbloom", "ゴールドフラワー" },
				{ "Gourmet Shroomcap", "特産スジタケ" },
				{ "Exquisite Shroomcap", "厳選スジタケ" },
				{ "Spirit Shroomcap", "霊光スジタケ" },
				{ "Bauble Cactus", "小玉サボテン" },
				{ "Jewel Cactus", "大玉サボテン" },
				{ "Kingly Cactus", "王様サボテン" },
				{ "Hardfruit", "ハードフルーツ" },
				{ "Rockfruit", "ロックフルーツ" },
				{ "Wildfruit", "ワイルドフルーツ" },
				{ "Super Abalone", "特産ツボアワビ" },
				{ "Choice Abalone", "厳選ツボアワビ" },
				{ "Precious Abalone", "秘蔵ツボアワビ" },
				{ "Light Pearl", "ライトパール" },
				{ "Deep Pearl", "ディープパール" },
				{ "Innocent Pearl", "イノセントパール" },
				{ "Forgotten Fossil", "いにしえの化石" },
				{ "Legendary Fossil", "まぼろしの化石" },
				{ "Mystical Fossil", "神秘の化石" },
				{ "Underground Fruit", "地下の果実" },
				{ "Tainted Fruit", "瘴気の果実" },
				{ "Elysian Fruit", "彼岸の果実" },
				{ "Gaia Amber", "大地のコハク" },
				{ "Dragonvein Amber", "龍脈のコハク" },
				{ "Ancient Amber", "太古のコハク" },
				{ "Blue Beryl", "ブルーマリン" },
				{ "True Beryl", "トゥルーマリン" },
				{ "Abyssal Beryl", "アビスマリン" },
				{ "Sunkissed Grass", "百陽草" },
				{ "Moonlit Mushroom", "月夜茸" },
				{ "Dragonbloom", "ドラゴンフラワー" },
				{ "Divineapple", "トロピカパイン" },
				{ "Violet Abalone", "藤色ツボアワビ" },
				{ "Platinum Pearl", "プラチナパール" },
				{ "Wicked Fossil", "魔の化石" },
				{ "Heavenberry", "ヘブンベリー" },
				{ "Twilight Stone", "黄昏の石" },
				{ "Noahstone", "ノアストーン" },
				{ "Wyvern Egg", "飛竜の卵" },
				{ "Herbivore Egg", "草食竜の卵" },
				{ "Lump of Meat", "肉の塊" },
				{ "Shepherd Hare", "ヨリミチウサギ" },
				{ "Pilot Hare", "ミチビキウサギ" },
				{ "Woodland Pteryx", "シンリンシソチョウ" },
				{ "Forest Pteryx", "コダイジュノツカイ" },
				{ "Cobalt Flutterfly", "コバルトモルフォ" },
				{ "Phantom Flutterfly", "マボロシモルフォ" },
				{ "Climbing Joyperch", "ナキキノボリウオ" },
				{ "Forest Gekko", "モリゲッコー" },
				{ "Wildspire Gekko", "アリヅカゲッコー" },
				{ "Gloom Gekko", "クラヤミゲッコー" },
				{ "Moonlight Gekko", "月光ゲッコー" },
				{ "Vaporonid", "カスミジョロウ" },
				{ "Scavantula", "スカベンチュラ" },
				{ "Revolture", "ニクイドリ" },
				{ "Blissbill", "トウゲンチョウ" },
				{ "Omenfly", "キザシヤンマ" },
				{ "Augurfly", "キッチョウヤンマ" },
				{ "Scalebat", "ウロコウモリ" },
				{ "Dung Beetle", "フンコロガシ" },
				{ "Bomb Beetle", "バクダンイワコロガシ" },
				{ "Pink Parexus", "ピンクパレクス" },
				{ "Great Pink Parexus", "ドスピンクパレクス" },
				{ "Burst Arowana", "ハレツアロワナ" },
				{ "Bomb Arowana", "バクレツアロワナ" },
				{ "Great Burst Arowana", "ドスハレツアロワナ" },
				{ "Great Bomb Arowana", "ドスバクレツアロワナ" },
				{ "Elegant Coralbird", "ドレスサンゴドリ" },
				{ "Dapper Coralbird", "タキシードサンゴドリ" },
				{ "Andangler", "アンドンウオ" },
				{ "Downy Crake", "フワフワクイナ" },
				{ "Bristly Crake", "ゴワゴワクイナ" },
				{ "Hopguppy", "ホッピングッピー" },
				{ "Petricanths", "カセキカンス" },
				{ "Paratoad", "シビレガスガエル" },
				{ "Sleeptoad", "ネムリガスガエル" },
				{ "Nitrotoad", "ニトロガスガエル" },
				{ "Wiggler", "ユラユラ" },
				{ "Wiggler Queen", "ユラユラクイーン" },
				{ "Vigorwasp", "回復ミツムシ" },
				{ "Giant Vigorwasp", "大回復ミツムシ" },
				{ "Flying Meduso", "オソラノエボシ" },
				{ "Carrier Ant", "ハコビアリ" },
				{ "Hercudrome", "ドスヘラクレス" },
				{ "Gold Hercudrome", "ゴールデンヘラクレス" },
				{ "Prism Hercudrome", "虹色ドスヘラクレス" },
				{ "Emperor Hopper", "皇帝バッタ" },
				{ "Tyrant Hopper", "暴君バッタ" },
				{ "Flashfly", "閃光羽虫" },
				{ "Grandfather Mantagrell", "ムカシマンタゲラ" },
				{ "Iron Helmcrab", "テツカブトガニ" },
				{ "Soldier Helmcrab", "ヘイタイカブトガニ" },
				{ "Emerald Helmcrab", "エメラルドカブトガニ" },
				{ "Whetfish", "キレアジ" },
				{ "Great Whetfish", "ドスキレアジ" },
				{ "Gastronome Tuna", "大食いマグロ" },
				{ "Great Gastronome Tuna", "ドス大食いマグロ" },
				{ "King Marlin", "ダイオウカジキ" },
				{ "Great King Marlin", "ドスダイオウカジキ" },
				{ "Goldenfish", "黄金魚" },
				{ "Silverfish", "白金魚" },
				{ "Great Goldenfish", "ドス黄金魚" },
				{ "Great Silverfish", "ドス白金魚" },
				{ "Goldenfry", "小金魚" },
				{ "Great Goldenfry", "ドス小金魚" },
				{ "Sushifish", "サシミウオ" },
				{ "Great Sushifish", "ドスサシミウオ" },
				{ "Gunpowderfish", "バクヤクデメキン" },
				{ "Great Gunpowderfish", "ドスバクヤクデメキン" },
				{ "Antidote Jewel 1", "耐毒珠【１】" },
				{ "Antipara Jewel 1", "耐麻珠【１】" },
				{ "Pep Jewel 1", "耐眠珠【１】" },
				{ "Steadfast Jewel 1", "耐絶珠【１】" },
				{ "Antiblast Jewel 1", "耐爆珠【１】" },
				{ "Suture Jewel 1", "耐裂珠【１】" },
				{ "Def Lock Jewel 1", "耐防珠【１】" },
				{ "Earplug Jewel 3", "防音珠【３】" },
				{ "Wind Resist Jewel 2", "防風珠【２】" },
				{ "Footing Jewel 2", "耐震珠【２】" },
				{ "Fertilizer Jewel 1", "肥弾珠【１】" },
				{ "Heat Resist Jewel 2", "耐熱珠【２】" },
				{ "Attack Jewel 1", "攻撃珠【１】" },
				{ "Defense Jewel 1", "防御珠【１】" },
				{ "Vitality Jewel 1", "体力珠【１】" },
				{ "Recovery Jewel 1", "早復珠【１】" },
				{ "Fire Res Jewel 1", "耐火珠【１】" },
				{ "Water Res Jewel 1", "耐水珠【１】" },
				{ "Ice Res Jewel 1", "耐氷珠【１】" },
				{ "Thunder Res Jewel 1", "耐雷珠【１】" },
				{ "Dragon Res Jewel 1", "耐龍珠【１】" },
				{ "Resistor Jewel 1", "耐属珠【１】" },
				{ "Blaze Jewel 1", "火炎珠【１】" },
				{ "Stream Jewel 1", "流水珠【１】" },
				{ "Frost Jewel 1", "氷結珠【１】" },
				{ "Bolt Jewel 1", "雷光珠【１】" },
				{ "Dragon Jewel 1", "破龍珠【１】" },
				{ "Venom Jewel 1", "毒珠【１】" },
				{ "Paralyzer Jewel 1", "麻痺珠【１】" },
				{ "Sleep Jewel 1", "睡眠珠【１】" },
				{ "Blast Jewel 1", "爆破珠【１】" },
				{ "Poisoncoat Jewel 3", "毒瓶珠【３】" },
				{ "Paracoat Jewel 3", "痺瓶珠【３】" },
				{ "Sleepcoat Jewel 3", "眠瓶珠【３】" },
				{ "Blastcoat Jewel 3", "爆瓶珠【３】" },
				{ "Powercoat Jewel 3", "強瓶珠【３】" },
				{ "Release Jewel 3", "解放珠【３】" },
				{ "Expert Jewel 1", "達人珠【１】" },
				{ "Critical Jewel 2", "超心珠【２】" },
				{ "Tenderizer Jewel 2", "痛撃珠【２】" },
				{ "Charger Jewel 2", "短縮珠【２】" },
				{ "Handicraft Jewel 3", "匠珠【３】" },
				{ "Draw Jewel 2", "抜刀珠【２】" },
				{ "Destroyer Jewel 2", "重撃珠【２】" },
				{ "KO Jewel 2", "ＫＯ珠【２】" },
				{ "Drain Jewel 1", "奪気珠【１】" },
				{ "Rodeo Jewel 2", "乗慣珠【２】" },
				{ "Flight Jewel 2", "飛燕珠【２】" },
				{ "Throttle Jewel 2", "全開珠【２】" },
				{ "Challenger Jewel 2", "挑戦珠【２】" },
				{ "Flawless Jewel 2", "無傷珠【２】" },
				{ "Potential Jewel 2", "底力珠【２】" },
				{ "Fortitude Jewel 1", "逆境珠【１】" },
				{ "Furor Jewel 2", "逆上珠【２】" },
				{ "Sonorous Jewel 1", "鼓笛珠【１】" },
				{ "Magazine Jewel 2", "増弾珠【２】" },
				{ "Trueshot Jewel 1", "特射珠【１】" },
				{ "Artillery Jewel 1", "砲術珠【１】" },
				{ "Heavy Artillery Jewel 1", "砲手珠【１】" },
				{ "Sprinter Jewel 2", "強走珠【２】" },
				{ "Physique Jewel 2", "体術珠【２】" },
				{ "Flying Leap Jewel 1", "飛込珠【１】" },
				{ "Refresh Jewel 2", "早気珠【２】" },
				{ "Hungerless Jewel 1", "無食珠【１】" },
				{ "Evasion Jewel 2", "回避珠【２】" },
				{ "Jumping Jewel 2", "跳躍珠【２】" },
				{ "Ironwall Jewel 1", "鉄壁珠【１】" },
				{ "Sheath Jewel 1", "速納珠【１】" },
				{ "Friendship Jewel 1", "友愛珠【１】" },
				{ "Enduring Jewel 1", "持続珠【１】" },
				{ "Satiated Jewel 1", "節食珠【１】" },
				{ "Gobbler Jewel 1", "早食珠【１】" },
				{ "Grinder Jewel 1", "研磨珠【１】" },
				{ "Bomber Jewel 1", "爆師珠【１】" },
				{ "Fungiform Jewel 1", "茸好珠【１】" },
				{ "Angler Jewel 1", "釣師珠【１】" },
				{ "Chef Jewel 1", "調理珠【１】" },
				{ "Transporter Jewel 1", "運搬珠【１】" },
				{ "Gathering Jewel 1", "採集珠【１】" },
				{ "Honeybee Jewel 1", "蜜集珠【１】" },
				{ "Carver Jewel 1", "皮剥珠【１】" },
				{ "Protection Jewel 1", "加護珠【１】" },
				{ "Meowster Jewel 1", "采配珠【１】" },
				{ "Botany Jewel 1", "植学珠【１】" },
				{ "Geology Jewel 1", "地学珠【１】" },
				{ "Mighty Jewel 2", "渾身珠【２】" },
				{ "Stonethrower Jewel 1", "投石珠【１】" },
				{ "Tip Toe Jewel 1", "潜伏珠【１】" },
				{ "Brace Jewel 3", "耐衝珠【３】" },
				{ "Scoutfly Jewel 1", "導虫珠【１】" },
				{ "Crouching Jewel 1", "屈速珠【１】" },
				{ "Longjump Jewel 1", "幅跳珠【１】" },
				{ "Smoke Jewel 1", "煙復珠【１】" },
				{ "Mirewalker Jewel 1", "沼渡珠【１】" },
				{ "Climber Jewel 1", "壁登珠【１】" },
				{ "Radiosity Jewel 1", "光拡珠【１】" },
				{ "Research Jewel 1", "研究珠【１】" },
				{ "Specimen Jewel 1", "標本珠【１】" },
				{ "Miasma Jewel 1", "耐瘴珠【１】" },
				{ "Scent Jewel 1", "嗅覚珠【１】" },
				{ "Slider Jewel 1", "滑走珠【１】" },
				{ "Intimidator Jewel 1", "威嚇珠【１】" },
				{ "Hazmat Jewel 1", "耐境珠【１】" },
				{ "Mudshield Jewel 1", "耐泥珠【１】" },
				{ "Element Resist Jewel 1", "護属珠【１】" },
				{ "Slider Jewel 2", "滑走珠【２】" },
				{ "Medicine Jewel 1", "治癒珠【１】" },
				{ "Forceshot Jewel 3", "強弾珠【３】" },
				{ "Pierce Jewel 3", "貫通珠【３】" },
				{ "Spread Jewel 3", "散弾珠【３】" },
				{ "Enhancer Jewel 2", "昂揚珠【２】" },
				{ "Crisis Jewel 1", "窮地珠【１】" },
				{ "Dragonseal Jewel 3", "龍封珠【３】" },
				{ "Discovery Jewel 2", "発見珠【２】" },
				{ "Detector Jewel 1", "探知珠【１】" },
				{ "Maintenance Jewel 1", "整備珠【１】" },
				{ "Invalid Message", "蜜虫回復" },
				{ "Stone", "石ころ" },
				{ "Redpit", "ツブテの実" },
				{ "Brightmoss", "ヒカリゴケ" },
				{ "Scatternut", "はじけクルミ" },
				{ "Torch Pod", "スリンガー松明弾" },
				{ "Bomb Pod", "スリンガー爆発弾" },
				{ "Slime Pod", "スリンガー着撃弾" },
				{ "Piercing Pod", "スリンガー貫通弾" },
				{ "Dragon Pod", "スリンガー滅龍弾" },
				{ "Crystalburst", "ハジケ結晶" },
				{ "Puddle Pod", "スリンガー水流弾" },
				{ "Chillshroom", "ヒンヤリダケ" },
				{ "Tailraider Voucher", "オトモダチケット" },
				{ "Emerald Shell", "エメラルドな殻" },
				{ "Gajalaka Sketch", "奇面族スケッチ" },
				{ "Mighty Bow Jewel 2", "強弓珠【２】" },
				{ "Mind's Eye Jewel 2", "心眼珠【２】" },
				{ "Shield Jewel 2", "強壁珠【２】" },
				{ "Sharp Jewel 2", "剛刃珠【２】" },
				{ "Elementless Jewel 2", "無撃珠【２】" }
			};
			if (translations.TryGetValue(itemName, out string value))
			{
				return value;
			}
			else
			{
				return "???";
			}
		}

		public static async Task CreateWeaponCategories(string game)
		{
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
					foreach (string name in new string[] { "Charge Blade", "Dual Blades", "Great Sword", "Gunlance", "Hammer", "Hunting Horn", "Insect Glaive", "Lance", "Long Sword", "Switch Axe", "Sword and Shield", "Bow", "Heavy Bowgun", "Light Bowgun" })
					{
						WikiPage page = new(site, $"Category:{game} {name}");
						await page.RefreshAsync();
						if (!page.Exists)
						{
							await page.EditAsync(new WikiPageEditOptions()
							{
								Bot = true,
								Content = "",
								Minor = true,
								Summary = "Created category pages for all weapons in MHWI and MHRS where needed.",
								Watch = AutoWatchBehavior.None
							});
						}
						Console.WriteLine($"Category:{game} {name} created.");
					}
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				await site.LogoutAsync();
			}
		}

		public static async Task UploadArmorWithAPI(string game)
		{
			string username = "";
			string password = "";
			if (File.Exists(@"D:\Wiki Files\wikicredentials.txt"))
			{
				string[] creds = File.ReadAllLines(@"D:\Wiki Files\wikicredentials.txt");
				username = creds[0];
				password = creds[1];
			}
			if (username == null)
			{
				Console.WriteLine("Please enter your username.");
				username = Console.ReadLine()!;
				Console.WriteLine("Please enter your password.");
				password = Console.ReadLine()!;
			}
			DateTime start = DateTime.Now;
			Dictionary<Models.ArmorSets.WebToolkitData, string> src = ArmorSets.MassGenerate(game);
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				int totalCntr = 1;
				DateTime lastWrite = DateTime.MinValue;
				foreach (KeyValuePair<Models.ArmorSets.WebToolkitData, string> data in src.Where(x => x.Key.SetName != "Unavailable" && x.Key.SetName != "HARDUMMY"))
				{
					WikiPage page = new(site, $"{data.Key.SetName} Set ({game})");
					await page.EditAsync(new WikiPageEditOptions()
					{
						Content = data.Value,
						Minor = false,
						Bot = true,
						Summary = "Auto-updated using the API through MH Wiki Toolkit."
					});
					Console.WriteLine($"Edited page \"{data.Key.SetName}\" ({totalCntr}/{src.Count})");
					totalCntr++;
				}
				StringBuilder setList = new();
				string gameNameFull = Weapon.GetGameFullName(game);
				setList.Append($@"{{{{Meta
|MetaTitle     = {game} Armor Sets
|MetaDesc      = A list of all Armor Sets from {gameNameFull}
|MetaKeywords  = {game}, {gameNameFull}, Armor, Armor Sets
|MetaImage     = {game}-Logo.png
}}}}
{{{{GenericNav|{game}}}}}
<br>
<br>
The following is a list of all armor sets that appear in [[{gameNameFull}]] and their corresponding armor pieces.
__TOC__");
				string lastRank = "";
				long? lastRarity = null;
				foreach (Models.ArmorSets.WebToolkitData data in src.Keys.OrderBy(x => x.Rarity))
				{
					bool newRarity = false;
					if (lastRarity == null || lastRarity != data.Rarity)
					{
						newRarity = true;
						setList.AppendLine("</div>");
					}
					string rank = ArmorSets.GetRank(data.Game, data.Rarity);
					if (rank != lastRank)
					{
						setList.AppendLine("=" + rank + "=");
					}
					if (newRarity)
					{
						setList.AppendLine($@"==Rarity {data.Rarity!.Value}==
<div style=""display:flex; flex-wrap:wrap; height: 100%; align-items:stretch; justify-content:space-around; margin-bottom:20px;"">");
					}
					string setGenderMark = data.OnlyForGender != null && src.Keys.Any(x => x.SetName == data.SetName && x.OnlyForGender != data.OnlyForGender) ? " (" + data.OnlyForGender + ")" : "";
					setList.AppendLine($@"{{{{ArmorSetListItem
|Game               = {data.Game}
|Set Rarity         = {data.Rarity!.Value}
|Set Name           = {data.SetName}{setGenderMark}
|Male Image         = {(string.IsNullOrEmpty(data.MaleFrontImg) ? "wiki.png" : data.MaleFrontImg)}
|Female Image       = {(string.IsNullOrEmpty(data.FemaleFrontImg) ? "wiki.png" : data.FemaleFrontImg)}
|Head Piece Name    = {data.Pieces.FirstOrDefault(x => x.IconType == "Helmet")?.Name ?? "None"}
|Chest Piece Name   = {data.Pieces.FirstOrDefault(x => x.IconType == "Chestplate")?.Name ?? "None"}
|Arm Piece Name     = {data.Pieces.FirstOrDefault(x => x.IconType == "Armguards")?.Name ?? "None"}
|Waist Piece Name   = {data.Pieces.FirstOrDefault(x => x.IconType == "Waist")?.Name ?? "None"}
|Leg Piece Name     = {data.Pieces.FirstOrDefault(x => x.IconType == "Leggings")?.Name ?? "None"}
}}}}");
					lastRank = rank;
					lastRarity = data.Rarity;
				}
				setList.AppendLine("</div>");
				WikiPage setListPage = new(site, $"{game}/Armor");
				await setListPage.EditAsync(new WikiPageEditOptions()
				{
					Content = setList.ToString(),
					Minor = false,
					Bot = true,
					Summary = "Auto-updated using the API through MH Wiki Toolkit."
				});
				Console.WriteLine("Edited armor list");
				await site.LogoutAsync();
			}
			Console.WriteLine("======================================================");
			Console.WriteLine("Finished!");
			TimeSpan elapsed = DateTime.Now - start;
			Console.WriteLine("Elapsed: " + elapsed.ToString());
		}

		public static async Task UploadWeaponsWithAPI(string game, bool onlyNewPages = false)
		{
			Models.Data.MHWI.Skills[] allSkills = Models.Data.MHWI.Skills.GetSkills();
			string username = "";
			string password = "";
			if (File.Exists(@"D:\Wiki Files\wikicredentials.txt"))
			{
				string[] creds = File.ReadAllLines(@"D:\Wiki Files\wikicredentials.txt");
				username = creds[0];
				password = creds[1];
			}
			if (username == null)
			{
				Console.WriteLine("Please enter your username.");
				username = Console.ReadLine()!;
				Console.WriteLine("Please enter your password.");
				password = Console.ReadLine()!;
			}
			DateTime start = DateTime.Now;
			Dictionary<WebToolkitData, string> src = Weapon.MassGenerate(game, false);
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + System.Configuration.ConfigurationManager.AppSettings.Get("WikiUsername")!
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.Login();
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				DateTime lastWrite = DateTime.MinValue;
				int totalCntr = 1;
				Dictionary<string, int[]> ktVals = new()
				{
					{ "GS6", new int[] { 70, 9, 15 } },
					{ "GS7", new int[] { 70, 9, 15 } },
					{ "GS8", new int[] { 90, 9, 15 } },
					{ "GL6", new int[] { 70, 12, 18 } },
					{ "GL7", new int[] { 70, 12, 18 } },
					{ "GL8", new int[] { 90, 12, 18 } },
					{ "LS6", new int[] { 70, 12, 15 } },
					{ "LS7", new int[] { 70, 12, 15 } },
					{ "LS8", new int[] { 90, 12, 15 } },
					{ "SA6", new int[] { 80, 12, 18 } },
					{ "SA7", new int[] { 80, 12, 18 } },
					{ "SA8", new int[] { 100, 12, 18 } },
					{ "SnS6", new int[] { 90, 12, 15 } },
					{ "SnS7", new int[] { 90, 12, 15 } },
					{ "SnS8", new int[] { 110, 12, 15 } },
					{ "CB6", new int[] { 70, 12, 15 } },
					{ "CB7", new int[] { 70, 12, 15 } },
					{ "CB8", new int[] { 90, 12, 15 } },
					{ "DB6", new int[] { 90, 12, 18 } },
					{ "DB7", new int[] { 90, 12, 18 } },
					{ "DB8", new int[] { 110, 12, 18 } },
					{ "IG6", new int[] { 70, 12, 15 } },
					{ "IG7", new int[] { 70, 12, 15 } },
					{ "IG8", new int[] { 90, 12, 15 } },
					{ "Hm6", new int[] { 70, 12, 15 } },
					{ "Hm7", new int[] { 70, 12, 15 } },
					{ "Hm8", new int[] { 90, 12, 15 } },
					{ "Ln6", new int[] { 80, 12, 24 } },
					{ "Ln7", new int[] { 80, 12, 24 } },
					{ "Ln8", new int[] { 100, 12, 24 } },
					{ "HH6", new int[] { 90, 12, 15 } },
					{ "HH7", new int[] { 90, 12, 15 } },
					{ "HH8", new int[] { 110, 12, 15 } },
					{ "Bo6", new int[] { 50, 12, 12 } },
					{ "Bo7", new int[] { 50, 12, 12 } },
					{ "Bo8", new int[] { 70, 12, 12 } },
					{ "HBG6", new int[] { 40, 0, 0 } },
					{ "HBG7", new int[] { 40, 0, 0 } },
					{ "HBG8", new int[] { 60, 0, 0 } },
					{ "LBG6", new int[] { 40, 0, 0 } },
					{ "LBG7", new int[] { 40, 0, 0 } },
					{ "LBG8", new int[] { 60, 0, 0 } }
				};
				Dictionary<WebToolkitData, string> dataDict = src;
				int total = dataDict.Count;
				foreach (KeyValuePair<WebToolkitData, string> data in dataDict)
				{
					WebToolkitData oldWeapon = data.Key.Clone();
					string name = data.Key.Name!;
					if (src.Keys.Count(x => x.Name == data.Key.Name!) > 1 && Weapon.GetRank(game, data.Key.Rarity) == "MR")
					{
						name = data.Key.Name + " (MR)";
						if (!data.Key.Name!.StartsWith("Safi's") && data.Key.Tree == "Unavailable" && data.Key.Game == "MHWI")
						{
							string[] statuses = ["Poison", "Paralysis", "Sleep", "Blast"];
							int otherRarity = (int)src.Keys.First(x => x.Name == data.Key.Name && x.Type == data.Key.Type && x.Rarity != data.Key.Rarity).Rarity!.Value;
							int[] ktVal = ktVals[data.Key.Type + otherRarity];
							data.Key.Attack = (Convert.ToInt32(data.Key.Attack) + ktVal[0]).ToString();
							data.Key.Defense = (Convert.ToInt32(data.Key.Defense) + 20).ToString();
							if (!data.Key.Name!.Contains("Kjárr"))
							{
								data.Key.ArmorSkills = $"{{{{GenericSkillLink|{game}|Kulve Taroth Essence|Armor|Yellow}}}}";
							}
							if (!string.IsNullOrEmpty(data.Key.Element1) && data.Key.Element1 != "None")
							{
								if (statuses.Contains(data.Key.Element1))
								{
									data.Key.ElementDmg1 = (data.Key.ElementDmg1.StartsWith("(") ? "(" : "") + (Convert.ToInt32(data.Key.ElementDmg1.Replace("(", "").Replace(")", "")) + ktVal[1]).ToString() + (data.Key.ElementDmg1.StartsWith("(") ? ")" : "");
								}
								else
								{
									data.Key.ElementDmg1 = (data.Key.ElementDmg1.StartsWith("(") ? "(" : "") + (Convert.ToInt32(data.Key.ElementDmg1.Replace("(", "").Replace(")", "")) + ktVal[2]).ToString() + (data.Key.ElementDmg1.StartsWith("(") ? ")" : "");
								}
							}
							if (!string.IsNullOrEmpty(data.Key.Element2) && data.Key.Element2 != "None")
							{
								if (statuses.Contains(data.Key.Element1))
								{
									data.Key.ElementDmg2 = (data.Key.ElementDmg2.StartsWith("(") ? "(" : "") + (Convert.ToInt32(data.Key.ElementDmg2.Replace("(", "").Replace(")", "")) + ktVal[1]).ToString() + (data.Key.ElementDmg2.StartsWith("(") ? ")" : "");
								}
								else
								{
									data.Key.ElementDmg2 = (data.Key.ElementDmg2.StartsWith("(") ? "(" : "") + (Convert.ToInt32(data.Key.ElementDmg2.Replace("(", "").Replace(")", "")) + ktVal[2]).ToString() + (data.Key.ElementDmg2.StartsWith("(") ? ")" : "");
								}
							}
						}
					}
					//Uncomment for sandboxing
					//WikiPage page = new(site, $"User:RampageRobot/Sandbox/{Weapon.GetWeaponTypeFullName(data.Key.Type!).Item1.Replace(" ", "")}");
					WikiPage page = new(site, $"{name}_({game})");
					await page.RefreshAsync(PageQueryOptions.FetchContent);
					if ((onlyNewPages && !page.Exists) || !onlyNewPages)
					{
						await page.EditAsync(new WikiPageEditOptions()
						{
							Content = Weapon.SingleGenerate(game, data.Key, [.. src.Keys]),
							Minor = true,
							Bot = true,
							Summary = "Auto-updated using the API through MH Wiki Toolkit."
						});
						Console.WriteLine($"({totalCntr}/{total}) Edited page \"{name}\"");
					}
					else
					{
						Console.WriteLine($"({totalCntr}/{total}) Skipped page \"{name}\"");
					}
					totalCntr++;
				}
				await site.LogoutAsync();
			}
			Console.WriteLine("======================================================");
			Console.WriteLine("Finished!");
			TimeSpan elapsed = DateTime.Now - start;
			Console.WriteLine("Elapsed: " + elapsed.ToString());
		}

		public static JArray GetWildsMessages(string file)
		{
			return JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file))!.Value<JArray>("entries")!;
		}
	}

	public class MonsterRenderGeneratorOptions
	{
		public bool RefreshProgressLists { get; set; } = false;
		public bool BuildPatternDatabase { get; set; } = false;
		public bool UpdatePages { get; set; } = false;
		public bool GenerateMissingRendersReport { get; set; } = false;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		public string[]? WhitelistGameAcronyms { get; set; }
		public string[]? BlacklistGameAcronyms { get; set; }
		public string[]? WhitelistMonsters { get; set; }
		public string[]? BlacklistMonsters { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	}

	public class MonsterAppearanceGeneratorOptions
	{
		public bool RefreshProgressLists { get; set; } = false;
		public bool UpdatePages { get; set; } = false;
		public bool GenerateNewPages { get; set; } = true;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		public string[]? WhitelistGameAcronyms { get; set; }
		public string[]? BlacklistGameAcronyms { get; set; }
		public string[]? WhitelistMonsters { get; set; }
		public string[]? BlacklistMonsters { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	}
}
