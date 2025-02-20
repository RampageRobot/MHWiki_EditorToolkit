using Newtonsoft.Json;
using System.Data;
using System.Text;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;
using WikiClientLibrary;
using MediawikiTranslator.Models.Weapon;
using MediawikiTranslator.Generators;
using WikiClientLibrary.Generators;
using DocumentFormat.OpenXml.Wordprocessing;

namespace MediawikiTranslator
{
	public static class Utilities
    {

		public static Models.Data.MHRS.Items[] GetMHRSItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHRS.Items[]>(Properties.Resources.mhrs_items, Models.Data.MHWI.Converter.Settings)!;
		}

		public static Models.Data.MHWI.Items[] GetMHWIItems()
		{
			return JsonConvert.DeserializeObject<Models.Data.MHWI.Items[]>(Encoding.UTF8.GetString(Properties.Resources.mhwi_items), Models.Data.MHWI.Converter.Settings)!;
		}

		public static DirectoryInfo GetWorkspace()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), @"\MHWikiToolkit_Generation\"));
        }

		public static async Task<string[]> GetWeaponRenders(string game, string weaponType)
		{
			string[] items = []; 
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
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + username,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
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

		public static async Task DeleteAllInDeleteCategory()
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
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + username,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
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

		public static async Task UploadMonsterGameData(string game, string[] monsterNames)
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
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + username
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
					foreach (string monsterName in monsterNames)
					{
						Models.Monsters.Enrage enrage = new(monsterName);
						if (enrage.FileFound)
						{
							WikiPage page = new(site, $"{monsterName}/{game}");
							await page.RefreshAsync(PageQueryOptions.FetchContent);
							if (page.Exists)
							{
								string oldContent = page.Content!;
								string newContent = oldContent.Insert(oldContent.IndexOf($"{{{{:{monsterName}/Lore}}}}") - 1, $@"
==Mechanics==
===Enraged State===
{{{{EnrageDataTable
|MR Changes? ={(enrage.MRChanges ? " X" : "")}
|Duration = {enrage.Duration}
|MR Duration ={(enrage.MRChanges ? $" {enrage.MRDuration}" : "")}
|Monster Damage Modifier = {enrage.DamageMod}
|MR Monster Damage Modifier ={(enrage.MRChanges ? $" {enrage.MRDamageMod}" : "")}
|Speed Modifier = {enrage.SpeedMod}
|MR Speed Modifier ={(enrage.MRChanges ? $" {enrage.MRSpeedMod}" : "")}
|Player Damage Modifier = {enrage.PlayerDamageMod}
|MR Player Damage Modifier ={(enrage.MRChanges ? $" {enrage.MRPlayerDamageMod}" : "")}
}}}}");
								await page.EditAsync(new WikiPageEditOptions()
								{
									Bot = true,
									Content = newContent,
									Minor = true,
									Summary = "Automatically generated game data from source data with Toolkit.",
									Watch = AutoWatchBehavior.None
								});
								Console.WriteLine($"{monsterName}/{game} edited.");
							}
						}
						else
						{
							Console.BackgroundColor = ConsoleColor.Red;
							Console.WriteLine($"{monsterName}/{game} NOT FOUND!");
							Console.ResetColor();
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

		public static async Task CreateWeaponCategories(string game)
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
			using (WikiClient client = new()
			{
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + username,
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
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
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + username
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				int totalCntr = 1;
				int cntr = 1;
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
					cntr++;
					Console.WriteLine($"Edited page \"{data.Key.SetName}\" ({totalCntr}/{src.Count})");
					if (DateTime.Now - lastWrite <= new TimeSpan(0, 1, 0) && cntr >= 8000)
					{
						Thread.Sleep((new TimeSpan(0, 1, 0) - (DateTime.Now - lastWrite)).Milliseconds);
						cntr = 1;
					}
					else
					{
						cntr = 1;
					}
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
			Console.WriteLine("Elapsed: " +  elapsed.ToString());
		}

		public static async Task UploadWeaponsWithAPI(string game)
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
				ClientUserAgent = "MHWikiToolkit/1.1.2 " + username
			})
			{
				WikiSite site = new(client, "https://monsterhunterwiki.org/api.php");
				await site.Initialization;
				try
				{
					await site.LoginAsync(username, password);
				}
				catch (WikiClientException ex)
				{
					Console.WriteLine(ex.Message);
				}
				int cntr = 1;
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
					WikiPage page = new(site, $"{name}_({game})");
					await page.EditAsync(new WikiPageEditOptions()
					{
						Content = Weapon.SingleGenerate(game, data.Key, [.. src.Keys]),
						Minor = true,
						Bot = true,
						Summary = "Auto-updated using the API through MH Wiki Toolkit."
					});
					cntr++;
					Console.WriteLine($"Edited page \"{name}\" ({totalCntr}/{total})");
					if (DateTime.Now - lastWrite <= new TimeSpan(0, 1, 0) && cntr >= 8)
					{
						Thread.Sleep((new TimeSpan(0, 1, 0) - (DateTime.Now - lastWrite)).Milliseconds);
						cntr = 1;
					}
					else
					{
						cntr = 1;
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
	}
}
