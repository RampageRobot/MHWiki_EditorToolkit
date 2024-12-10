class WeaponTemplate
{
	static currentSharpnessRow;
	static currentDecosRow;
	static finalData = [];
    static initTemplate()
    {
        $("body div.extra-modals").remove();
		$("body").append(`<div class="modal fade extra-modals" id="mdlModifySharpness" tabindex="-1" aria-labelledby="mdlModifySharpnessLabel" aria-hidden="true">
			<div class="modal-dialog modal-lg">
				<div class="modal-content">
					<div class="modal-header">
						<h5 class="modal-title" id="mdlModifySharpnessLabel">Modify Sharpness</h5>
						<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
					</div>
					<div class="modal-body" style="max-height: 78vh; overflow-y:auto;">
						<div class="row">
							<div class="col">
								<h5>First Bar</h5>
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Red Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtRedSharpnessHits1" />
							</div>
							<div class="col-6">
								<label>Orange Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtOrangeSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Yellow Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtYellowSharpnessHits1" />
							</div>
							<div class="col-6">
								<label>Green Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtGreenSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Blue Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtBlueSharpnessHits1" />
							</div>
							<div class="col-6">
								<label>White Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtWhiteSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Purple Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" id="txtPurpleSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Preview:</label>
								<div style="display:flex;flex-direction:column;align-items:center" id="divPreviewBar1">
									<div class="mw-no-invert" style="background-color:black;width:300px;height:30px;border-collapse:collapse;border:1px solid black;margin: auto;display:flex;flex-direction:row;flex-wrap:nowrap">
										<div style="background-color:#E41700;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#F36A01;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#FBFF00;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#49DD79;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#3E6AFF;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#E6E6E5;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#E401C8;height: inherit;flex:0 0 0%"></div>
									</div>
								</div>
							</div>
						</div>
						<hr/>
						<div class="row">
							<div class="col">
								<h5>Second Bar</h5>
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Red Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtRedSharpnessHits2" />
							</div>
							<div class="col-6">
								<label>Orange Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtOrangeSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Yellow Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtYellowSharpnessHits2" />
							</div>
							<div class="col-6">
								<label>Green Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtGreenSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Blue Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtBlueSharpnessHits2" />
							</div>
							<div class="col-6">
								<label>White Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtWhiteSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Purple Sharpness Hits</label>
								<input type="number" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" id="txtPurpleSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Preview:</label>
								<div style="display:flex;flex-direction:column;align-items:center" id="divPreviewBar2">
									<div class="mw-no-invert" style="background-color:black;width:300px;height:30px;border-collapse:collapse;border:1px solid black;margin: auto;display:flex;flex-direction:row;flex-wrap:nowrap">
										<div style="background-color:#E41700;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#F36A01;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#FBFF00;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#49DD79;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#3E6AFF;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#E6E6E5;height: inherit;flex:0 0 0%"></div>
										<div style="background-color:#E401C8;height: inherit;flex:0 0 0%"></div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="WeaponTemplate.applySharpness();">Apply</button>
						<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
					</div>
				</div>
			</div>
		</div>
		<div class="modal fade extra-modals" id="mdlModifyDecos" tabindex="-1" aria-labelledby="mdlModifyDecosLabel" aria-hidden="true">
			<div class="modal-dialog modal-lg">
				<div class="modal-content">
					<div class="modal-header">
						<h5 class="modal-title" id="mdlModifyDecosLabel">Modify Decorations</h5>
						<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
					</div>
					<div class="modal-body" style="max-height: 78vh; overflow-y:auto;">
						<div class="row">
							<div class="col">
								<button class="btn btn-primary float-end" type="button" onclick="WeaponTemplate.addRowToTblDecos();">Add Decoration</button>
							</div>
						</div>
						<div class="row">
							<div class="col">
								<table class="table table-striped table-dark" id="tblDecos">
									<thead>
										<tr>
											<th scope="col" style="width:40%;">Level</th>
											<th scope="col" style="width:40%;">Quantity</th>
											<th scope="col" style="width:10%;">Rampage?</th>
											<th scope="col" style="width:10%;">Delete</th>
										</tr>
									</thead>
									<tbody>
									</tbody>
								</table>
							</div>
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="WeaponTemplate.applyDecos();">Apply</button>
						<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
					</div>
				</div>
			</div>
		</div>`);
    }
    static getHeader()
    {
        return `<tr style="vertical-align:middle;" data-template="header">
    <th scope="col" style="width:0.375rem">Move</th>
    <th scope="col" style="width:1rem">Forge?</th>
    <th scope="col" style="width:15rem;">Weapon</th>
    <th scope="col" style="width:15rem;">Upgraded From</th>
    <th scope="col" style="width:10rem;">Icon Type</th>
    <th scope="col" style="width:4.5rem;">Rarity</th>
    <th scope="col" style="width:7.25rem;">Attack</th>
    <th scope="col" style="width:4.5rem;">Defense</th>
    <th scope="col" style="width:7.35rem;">Element</th>
    <th scope="col" style="width:6rem;">Element Dmg</th>
    <th scope="col" style="width:4.5rem;">Affinity</th>
    <th scope="col" style="width:4rem;">Decos</th>
    <th scope="col" style="width:4rem;">Sharpness</th>
    <th scope="col" style="width:0.375rem">Delete</th>
</tr>`;
    }
    static getRow()
    {
        return `<tr style="vertical-align:middle;" class="table-content-row" ondragend="dragend(event)" ondragover="row_dragover(event)" ondragstart="row_start(event)" data-template="row">
	<td class="ignore-generate" style="text-align:center">
		<button type="button" onmousedown="$(this).parent().parent().attr('draggable', true);" class="btn btn-primary btn-drag-row" title="Drag this button to move this weapon."><i class="bi bi-arrows-move"></i></button>
	</td>
	<td style="text-align:center; align-content:center">
		<input style="font-size:1.5rem;" class="form-check-input weapon-can-forge-input data-value m-auto" data-label="can-forge" type="checkbox" value="">
	</td>
	<td>
		<input type="text" class="form-control weapon-name-input data-value" onchange="initRow()" data-label="name">
	</td>
	<td>
		<select class="form-control form-select weapon-parent-input data-value" data-label="parent">
			<option value=""></option>
		</select>
	</td>
	<td>
		<select class="form-control form-select weapon-icon-type-input data-value" data-label="icon-type">
			<option value=""></option>
		</select>
	</td>
	<td>
		<input type="number" class="form-control weapon-rarity-input data-value" data-label="rarity">
	</td>
	<td>
		<input type="text" class="form-control weapon-attack-input data-value" data-label="attack">
	</td>
	<td>
		<input type="number" class="form-control weapon-defense-input data-value" data-label="defense">
	</td>
	<td>
		<select class="form-control form-select weapon-element-input data-value" data-label="element">
			<option value=""></option>
			<option value="Fire">Fire</option>
			<option value="Water">Water</option>
			<option value="Thunder">Thunder</option>
			<option value="Dragon">Dragon</option>
			<option value="Ice">Ice</option>
			<option value="Poison">Poison</option>
			<option value="Paralysis">Para</option>
			<option value="Sleep">Sleep</option>
			<option value="Blast">Blast</option>
		</select>
	</td>
    <td>
		<input class="form-control weapon-element-damage-input data-value" type="text" data-label="element-damage">
	</td>
	<td>
		<input type="number" class="form-control weapon-affinity-input data-value" data-label="affinity">
	</td>
	<td style="text-align:center; align-content:center">
		<input class="form-control weapon-decos-input data-value" hidden="hidden" type="text" data-label="decos"/>
		<button class="btn btn-primary" type="button" onclick="WeaponTemplate.modifyDecos($(this).parent().parent());">Modify</button>
	</td>
	<td style="text-align:center; align-content:center">
		<input class="form-control weapon-sharpness-input data-value" hidden="hidden" type="text" data-label="sharpness"/>
		<button class="btn btn-primary" type="button" onclick="WeaponTemplate.modifySharpness($(this).parent().parent());">Modify</button>
	</td>
	</td>
	<td class="ignore-generate" style="text-align:center">
		<button type="button" onclick="$(this).parent().parent().remove();" class="btn btn-danger btn-delete-row" title="Delete this weapon."><i class="bi bi-trash"></i></button>
	</td>
</tr>`;
    }
	static modifySharpness(row)
	{
		WeaponTemplate.currentSharpnessRow = row;
		var prevVal = null;
		var prevValStr = $(WeaponTemplate.currentSharpnessRow).find(".weapon-sharpness-input").first().val();
		if (prevValStr != '')
		{
			prevVal = JSON.parse(prevValStr);
		}
		var bars = ["Red", "Orange", "Yellow", "Green", "Blue", "White", "Purple"];
		var colorHexes = ["E41700", "F36A01", "FBFF00", "49DD79", "3E6AFF", "E6E6E5", "E401C8"];
		for (var i = 0; i < bars.length; i++)
		{
			for (var i2 = 1; i2 <= 2; i2++)
			{
				var val = "";
				if (prevVal != null)
				{
					val = prevVal[i2 - 1][i];
					if (val == "0")
					{
						val = "";
					}
				}
				$("#txt" + bars[i] + "SharpnessHits" + i2).val(val);
				$($("#divPreviewBar" + i2 + " div div")[i]).attr("style", "background-color:#" + colorHexes[i] + ";height: inherit;flex:0 0 " + (val == 0 ? "0" : (parseInt(val) / 4)) + "%");
			}
		}
		new bootstrap.Modal($("#mdlModifySharpness")).show();
	}
	static modifyDecos(row)
	{
		WeaponTemplate.currentDecosRow = row;
		var prevVal = null;
		$("#tblDecos tbody tr").remove();
		var prevValStr = $(WeaponTemplate.currentDecosRow).find(".weapon-decos-input").first().val();
		if (prevValStr != '')
		{
			prevVal = JSON.parse(prevValStr);
			if (prevVal != null)
			{
				for (var i = 0; i < prevVal.length; i++)
				{
					WeaponTemplate.addRowToTblDecos();
					var row = $("#tblDecos tbody tr").last();
					$(row).find(".decoration-level-input").val(prevVal[i].Level);
					$(row).find(".decoration-qty-input").val(prevVal[i].Qty);
					$(row).find(".decoration-rampage-input").prop("checked", prevVal[i].IsRampage == true);
				}
			}
		}
		new bootstrap.Modal($("#mdlModifyDecos")).show();
	}
	static updatePreviewBar(bar)
	{
		var redSharp = $("#txtRedSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[0]).attr("style", "background-color:#E41700;height: inherit;flex:0 0 " + (parseInt(redSharp) / 4) + "%");
		var orangeSharp = $("#txtOrangeSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[1]).attr("style", "background-color:#F36A01;height: inherit;flex:0 0 " + (parseInt(orangeSharp) / 4) + "%");
		var yellowSharp = $("#txtYellowSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[2]).attr("style", "background-color:#FBFF00;height: inherit;flex:0 0 " + (parseInt(yellowSharp) / 4) + "%");
		var greenSharp = $("#txtGreenSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[3]).attr("style", "background-color:#49DD79;height: inherit;flex:0 0 " + (parseInt(greenSharp) / 4) + "%");
		var blueSharp = $("#txtBlueSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[4]).attr("style", "background-color:#3E6AFF;height: inherit;flex:0 0 " + (parseInt(blueSharp) / 4) + "%");
		var whiteSharp = $("#txtWhiteSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[5]).attr("style", "background-color:#E6E6E5;height: inherit;flex:0 0 " + (parseInt(whiteSharp) / 4) + "%");
		var purpleSharp = $("#txtPurpleSharpnessHits" + bar).val();
		$($("#divPreviewBar" + bar + " div div")[6]).attr("style", "background-color:#E401C8;height: inherit;flex:0 0 " + (parseInt(purpleSharp) / 4) + "%");
	}
	static applySharpness()
	{
		if (typeof(WeaponTemplate.currentSharpnessRow) !== 'undefined')
		{
			$(WeaponTemplate.currentSharpnessRow).find(".weapon-sharpness-input").first().val(JSON.stringify([
				[ $("#txtRedSharpnessHits1").val(), $("#txtOrangeSharpnessHits1").val(), $("#txtYellowSharpnessHits1").val(), $("#txtGreenSharpnessHits1").val(), $("#txtBlueSharpnessHits1").val(), $("#txtWhiteSharpnessHits1").val(), $("#txtPurpleSharpnessHits1").val() ],
				[ $("#txtRedSharpnessHits2").val(), $("#txtOrangeSharpnessHits2").val(), $("#txtYellowSharpnessHits2").val(), $("#txtGreenSharpnessHits2").val(), $("#txtBlueSharpnessHits2").val(), $("#txtWhiteSharpnessHits2").val(), $("#txtPurpleSharpnessHits2").val() ]
			]));
		}
	}
	static addRowToTblDecos()
	{
		$("#tblDecos tbody").append(`<tr style="vertical-align:middle;">
			<td>
				<input type="number" class="form-control decoration-level-input" data-label="level" />
			</td>
			<td>
				<input type="number" class="form-control decoration-qty-input" data-label="qty" />
			</td>
			<td style="text-align:center; align-content:center">
				<input style="font-size:1.5rem;" class="form-check-input decoration-rampage-input data-value m-auto" data-label="rampage" type="checkbox" value="">
			</td>
			<td style="text-align:center">
				<button type="button" onclick="$(this).parent().parent().remove();" class="btn btn-danger btn-delete-row" title="Delete this decoration."><i class="bi bi-trash"></i></button>
			</td>
		</tr>`);
	}
	static applyDecos()
	{
		if (typeof(WeaponTemplate.currentDecosRow) !== 'undefined')
		{
			var vals = [];
			var rows = $("#tblDecos tbody tr");
			for (var i = 0; i < rows.length; i++)
			{
				vals.push({
					"Level": $(rows[i]).find(".decoration-level-input").first().val(),
					"Qty": $(rows[i]).find(".decoration-qty-input").first().val(),
					"IsRampage": $(rows[i]).find(".decoration-rampage-input").first().prop("checked")
				});
			}
			$(WeaponTemplate.currentDecosRow).find(".weapon-decos-input").first().val(JSON.stringify(vals));
		}
	}
    static initRow()
    {
        let itemNames = Array.from($(".weapon-name-input").map(function() {
            return $(this).val();
        })).filter(function (value, index, array) {
            return value != '' && array.indexOf(value) === index;
        }).sort(function (a, b) {
            return a > b ? 1 : -1;
        }).map(function (item) {
            return `<option value="${item}">${item}</option>`;
        });
        $(".weapon-parent-input").each(function () {
            var oldVal = $(this).val();
            $(this).find("option").remove();
            $(this).append('<option value=""></option');
            $(this).append(itemNames.concat("\n"));
            $(this).val(oldVal);
        });
    }
	static updateIcons()
	{
		var icons = '';
		switch ($("#ddlGameSelect").val())
		{
			case "MHG":
				icons = `<option value=""></option>
					<option value="GS">Great Sword</option>
					<option value="LS">Long Sword</option>
					<option value="SnS">Sword and Shield</option>
					<option value="DB">Dual Blades</option>
					<option value="Hm">Hammer</option>
					<option value="HH">Hunting Horn</option>
					<option value="Ln">Lance</option>
					<option value="GL">Gunlance</option>
					<option value="LBG">Light Bowgun</option>
					<option value="HBG">Heavy Bowgun</option>`;
				break;
			case "MHWI":
				icons = `<option value=""></option>
					<option value="GS">Great Sword</option>
					<option value="LS">Long Sword</option>
					<option value="SnS">Sword and Shield</option>
					<option value="DB">Dual Blades</option>
					<option value="Hm">Hammer</option>
					<option value="HH">Hunting Horn</option>
					<option value="Ln">Lance</option>
					<option value="GL">Gunlance</option>
					<option value="SA">Switch Axe</option>
					<option value="CB">Charge Blade</option>
					<option value="IG">Insect Glaive</option>
					<option value="LBG">Light Bowgun</option>
					<option value="HBG">Heavy Bowgun</option>`;
				break;
			case "MHRS":
				icons = `<option value=""></option>
					<option value="GS">Great Sword</option>
					<option value="LS">Long Sword</option>
					<option value="SnS">Sword and Shield</option>
					<option value="DB">Dual Blades</option>
					<option value="Hm">Hammer</option>
					<option value="HH">Hunting Horn</option>
					<option value="Ln">Lance</option>
					<option value="GL">Gunlance</option>
					<option value="SA">Switch Axe</option>
					<option value="CB">Charge Blade</option>
					<option value="IG">Insect Glaive</option>
					<option value="LBG">Light Bowgun</option>
					<option value="HBG">Heavy Bowgun</option>`;
				break;
		}
		var oldIcon = $("#ddlIconSelect").val();
		$("#ddlIconSelect option").remove();
		$("#ddlIconSelect").append(icons);
		$("#ddlIconSelect").val(oldIcon);
		$(".weapon-icon-type-input option").remove();
		$(".weapon-icon-type-input").each(function () {
			var oldVal = $(this).val();
			$(this).children("option").remove();
			$(this).append(icons);
			$(this).val(oldVal);
		});
	}
    static generateTree()
    {
		WeaponTemplate.finalData = [];
		$("#tblTree tbody tr").each(function () {
			var dataObj = {};
			$(this).find("td:not(.ignore-generate)").children().each(function () {
				if ($(this).hasClass("form-check-input"))
				{
					dataObj[$(this).attr("data-label")] = $(this).prop("checked");
				}
				else
				{
					dataObj[$(this).attr("data-label")] = $(this).val();
				}
			});
			WeaponTemplate.finalData.push(dataObj);
		});
		var sharpnessBase = '';
		var weaponLink = '';
		var maxSharpnessCount = -1;
		switch ($("#ddlGameSelect").val())
		{
			case "MHG":
				{
					sharpnessBase = "MHGSharpnessBase";
					weaponLink = "2ndGenWeaponLink|MHG";
					maxSharpnessCount = 6;
				}
				break;
			case "MHWI":
				{
					sharpnessBase = "MHWISharpnessBase";
					weaponLink = "MHWIWeaponLink";
				}
				break;
			case "MHRS":
				{
					sharpnessBase = "MHRSSharpnessBase";
					weaponLink = "MHRSWeaponLink";
				}
				break;
		}
		callGenerator(WeaponTemplate.finalData, sharpnessBase, weaponLink, maxSharpnessCount, $("#txtPathName").val());
    }
}