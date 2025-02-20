using MediawikiTranslator.Models.WeaponTree;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebToolkit.Controllers
{
	[Route("WeaponTreeController")]
	public class WeaponTreeController : Controller
	{
		[HttpPost("GenerateTree")]
		public string GenerateTree(string json, string sharpnessBase, int maxSharpnessCount, string pathName, string defaultIcon)
		{
			try
			{
				return MediawikiTranslator.Generators.WeaponTree.ParseJson(json, sharpnessBase, maxSharpnessCount, pathName, defaultIcon);
			}
			catch (Exception ex)
			{
				Response.Clear();
				Response.StatusCode = 500;
				Response.WriteAsync(ex.Message);
				return string.Empty;
			}
		}

        [HttpPost("ParseCsv")]
        public Tuple<string, string>? ParseCsv(string csvFile, bool duplicateSharpness)
        {
            try
            {
				string objVals = MediawikiTranslator.Generators.WeaponTree.ParseCsv(csvFile, duplicateSharpness);
				return new Tuple<string, string>(objVals, LoadTree(objVals));
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.StatusCode = 500;
                Response.WriteAsync(ex.Message);
                return null;
            }
        }

		[HttpPost("LoadTree")]
		public string LoadTree(string fileContents)
		{
			try
			{
				WebToolkitData[] contents = WebToolkitData.FromJson(fileContents);
				string pathLinkOptions = string.Join("\r\n", contents.Select(x => $"<option value=\"{x.PathName}\">{x.PathName}</option>"));
				StringBuilder sb = new();
				foreach (WebToolkitData path in contents)
				{
					sb.AppendLine($@"<div class=""row py-1 card-holder"">
						<div class=""card"" id=""card_1"">
							<div class=""card-header"">
								<div class=""row"">
									<div class=""col-2"">
										<div class=""row pb-1"">
											<div class=""col"">
												<button style=""padding: .25rem; margin-right:.25rem;"" class=""btn btn-primary bi bi-arrow-up float-start"" onclick=""$(this).parents('.card-holder').after($(this).parents('.card-holder').prev())""></button>
												<button style=""padding: .25rem;"" class=""btn btn-primary bi bi-arrow-down float-start"" onclick=""$(this).parents('.card-holder').before($(this).parents('.card-holder').next())""></button>
											</div>
										</div>
									</div>
									<div class=""col"">
										<button type=""button"" class=""btn btn-primary btn-collapse-card float-end"" data-is-collapsed=""false"" onmousedown=""collapseCard($(this));"" title=""Collapse this tree.""><i class=""bi bi-arrows-collapse""></i></button>
									</div>
								</div>
								<div class=""row"">
									<div class=""col-6"">
										<label>Path Name</label>
										<input type=""text"" class=""w-100 form-control weapon-path-name-input data-value"" onchange=""initRow()"" data-label=""path-name"" value=""{path.PathName}"" />
									</div>
									<div class=""col m-auto"">
										<div class=""float-end"">
											<button type=""button"" onclick=""$(this).parents('div.card-holder').first().remove();"" class=""btn btn-danger btn-delete-card"" title=""Delete this tree.""><i class=""bi bi-trash""></i></button>
										</div>
									</div>
								</div>
							</div>
							<div class=""card-body"" id=""card-body_1"">
								<div class=""row"">
									<div class=""col m-auto"">
										<div class=""align-self-end float-end form-group"">
											<button type=""button"" onclick=""addEmptyRow($(this).parents('.card-body').children().find('table.table-tree tbody'));"" class=""btn btn-primary"" title=""Add new row that represents a weapon in the tree."">Add New Weapon</button>
										</div>
									</div>
								</div>
								<div class=""row"">
									<div class=""col"">
										<table class=""table table-striped table-dark table-tree"">
											<thead>
												<tr style=""vertical-align:middle;"" data-template=""header"">
													<th scope=""col"" style=""width:0.375rem"">Move</th>
													<th scope=""col"" style=""width:1rem"">Forge?</th>
													<th scope=""col"" style=""width:1rem"">Rollback?</th>
													<th scope=""col"" style=""width:15rem;"">Weapon</th>
													<th scope=""col"" style=""width:10rem;"">Upgraded From</th>
													<th scope=""col"" style=""width:9rem;"">Path Link</th>
													<th scope=""col"" style=""width:8rem;"">Icon Type</th>
													<th scope=""col"" style=""width:4.5rem;"">Rarity</th>
													<th scope=""col"" style=""width:4rem;"">Stats</th>
													<th scope=""col"" style=""width:4rem;"">Decos</th>
													<th scope=""col"" style=""width:4rem;"">Sharpness</th>
													<th scope=""col"" style=""width:0.375rem"">Options</th>
												</tr>
											</thead>
											<tbody>");
					string parentNameOptions = string.Join("\r\n", contents.SelectMany(x => x.Data.Select(x => $"<option value=\"{x.Name.Replace("\"", "&quot;")}{(x.Name == x.Parent ? "selected='true'" : "")}\">{x.Name.Replace("\"", "&quot;")}</option>")).Distinct());
					foreach (Datum data in path.Data)
					{
						string statString = $"{{\"attack\":\"{data.Attack}\",\"defense\":\"{data.Defense}\",\"element\":\"{data.Element}\",\"element-damage\":\"{data.ElementDamage}\",\"element-2\":\"{data.Element2}\",\"element-damage-2\":\"{data.ElementDamage2}\",\"affinity\":\"{data.Affinity}\",\"elderseal\":\"{data.Elderseal}\",\"rampage-slots\":\"{data.RampageSlots}\",\"rampage-deco\":\"{data.RampageDeco}\",\"armor-skill\":\"{data.ArmorSkill}\",\"armor-skill-2\":\"{data.ArmorSkill2}\"}}";
						sb.AppendLine($@"<tr style=""vertical-align:middle;"" class=""table-content-row"" data-template=""row"">
	<td class=""ignore-generate"" style=""text-align:center"">
		<div class=""col"">
			<div class=""row pb-1"">
				<div class=""col"">
					<button style=""padding: .25rem; margin-right:.25rem;"" class=""btn btn-primary bi bi-arrow-up float-start"" onclick=""insertRowBefore($(this));""></button>
					<button style=""padding: .25rem;"" class=""btn btn-primary bi bi-arrow-down float-start"" onclick=""insertRowAfter($(this));""></button>
				</div>
			</div>
		</div>
	</td>
	<td style=""text-align:center; align-content:center"">
		<input style=""font-size:1.5rem;"" class=""form-check-input weapon-can-forge-input data-value m-auto"" data-label=""can-forge"" type=""checkbox"" value=""{data.CanForge == true}""{(data.CanForge == true ? " checked=\"checked\"" : "")}>
	</td>
	<td style=""text-align:center; align-content:center"">
		<input style=""font-size:1.5rem;"" class=""form-check-input weapon-can-rollback-input data-value m-auto"" data-label=""can-rollback"" type=""checkbox"" value=""{data.CanRollback == true}""{(data.CanRollback == true ? " checked=\"checked\"" : "")}>
	</td>
	<td>
		<input type=""text"" class=""form-control weapon-name-input data-value"" onchange=""initRow()"" data-label=""name"" value=""{data.Name.Replace("\"", "&quot;")}"">
	</td>
	<td>
		<select class=""form-control form-select weapon-parent-input data-value"" data-label=""parent"">
			<option value=""""></option>
			{parentNameOptions}
		</select>
	</td>
	<td>
		<select class=""form-control form-select weapon-path-link-input data-value"" data-label=""path-link"">
			<option value=""""></option>
			{pathLinkOptions}
		</select>
	</td>
	<td>
		<select class=""form-control form-select weapon-icon-type-input data-value"" data-label=""icon-type"">
			<option value=""""></option>
			<option value=""GS"">Great Sword</option>
			<option value=""LS"">Long Sword</option>
			<option value=""SnS"">Sword and Shield</option>
			<option value=""DB"">Dual Blades</option>
			<option value=""Hm"">Hammer</option>
			<option value=""HH"">Hunting Horn</option>
			<option value=""Ln"">Lance</option>
			<option value=""GL"">Gunlance</option>
			<option value=""SA"">Switch Axe</option>
			<option value=""CB"">Charge Blade</option>
			<option value=""IG"">Insect Glaive</option>
			<option value=""Bo"">Bow</option>
			<option value=""LBG"">Light Bowgun</option>
			<option value=""HBG"">Heavy Bowgun</option>
		</select>
	</td>
	<td>
		<input type=""number"" onblur=""validateInput(this);"" class=""form-control weapon-rarity-input data-value"" data-label=""rarity"" value=""{data.Rarity}"">
	</td>
	<td style=""text-align:center; align-content:center"">
		<input class=""form-control weapon-stats-input data-value data-value-hidden"" hidden=""hidden"" type=""text"" data-label=""stats"" loaded-value='{statString}' />
		<button class=""btn btn-primary ignore-generate"" type=""button"" onclick=""WeaponTemplate.modifyStats($(this).parent().parent(), validateComplexData, this, $(this).parent().children().first(), WeaponTemplate.validateStats);"">Modify</button>
	</td>
	<td style=""text-align:center; align-content:center"">
		<input class=""form-control weapon-decos-input data-value data-value-hidden"" hidden=""hidden"" type=""text"" data-label=""decos"" loaded-value='{data.Decos}' />
		<button class=""btn btn-primary ignore-generate"" type=""button"" onclick=""WeaponTemplate.modifyDecos($(this).parent().parent(), validateComplexData, this, $(this).parent().children().first(), WeaponTemplate.validateDecos);"">Modify</button>
	</td>
	<td style=""text-align:center; align-content:center"" class=""hide-on-invalid-weapon"" data-invalid-weapons=""Bo,HBG,LBG"">
		<input class=""form-control weapon-sharpness-input data-value data-value-hidden"" hidden=""hidden"" type=""text"" data-label=""sharpness"" loaded-value='{data.Sharpness}' />
		<button class=""btn btn-primary ignore-generate"" type=""button"" onclick=""WeaponTemplate.modifySharpness($(this).parent().parent(), validateComplexData, this, $(this).parent().children().first(), WeaponTemplate.validateSharpness);"">Modify</button>
	</td>
	<td class=""ignore-generate"" style=""text-align:center"">
		<button type=""button"" onclick=""duplicateRow($(this).parent().parent())"" class=""btn btn-primary"" title=""Duplicate the weapon.""><i class=""bi bi-copy""></i></button>
		<button type=""button"" onclick=""$(this).parent().parent().remove();"" class=""btn btn-danger btn-delete-row"" title=""Delete this weapon.""><i class=""bi bi-trash""></i></button>
	</td>
</tr>");
					}
					sb.AppendLine(@"</tbody>
										</table>
									</div>
								</div>
							</div>
						</div>
					</div>");
				}
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.StatusCode = 500;
                Response.WriteAsync(ex.Message);
                return string.Empty;
            }
        }
    }
}
