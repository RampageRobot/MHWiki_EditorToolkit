class WeaponTemplate
{
	static currentSharpnessRow;
	static currentSharpnessCallback;
	static currentSharpnessCallbackArgs;
	static currentDecosRow;
	static currentDecosCallback;
	static currentDecosCallbackArgs;
	static currentStatsRow;
	static currentStatsCallback;
	static currentStatsCallbackArgs;
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
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtRedSharpnessHits1" />
							</div>
							<div class="col-6">
								<label>Orange Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtOrangeSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Yellow Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtYellowSharpnessHits1" />
							</div>
							<div class="col-6">
								<label>Green Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtGreenSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Blue Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtBlueSharpnessHits1" />
							</div>
							<div class="col-6">
								<label>White Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtWhiteSharpnessHits1" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Purple Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('1');" value="0" id="txtPurpleSharpnessHits1" />
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
							<div class="col-6">
								<h5>Second Bar</h5>
							</div>
							<div class="col-6" style="display: flex;justify-content: flex-end;">
								<button type="button" onclick="WeaponTemplate.duplicateSharpness()" class="btn btn-primary" title="Duplicate first bar sharpness in the second bar."><i class="bi bi-copy"></i></button>
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Red Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtRedSharpnessHits2" />
							</div>
							<div class="col-6">
								<label>Orange Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtOrangeSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Yellow Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtYellowSharpnessHits2" />
							</div>
							<div class="col-6">
								<label>Green Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtGreenSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Blue Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtBlueSharpnessHits2" />
							</div>
							<div class="col-6">
								<label>White Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtWhiteSharpnessHits2" />
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Purple Sharpness Hits</label>
								<input type="number" onblur="validateInput(this);" class="form-control" onchange="WeaponTemplate.updatePreviewBar('2');" value="0" id="txtPurpleSharpnessHits2" />
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
		</div>
		<div class="modal fade extra-modals" id="mdlModifyStats" tabindex="-1" aria-labelledby="mdlModifyStatsLabel" aria-hidden="true">
			<div class="modal-dialog modal-lg">
				<div class="modal-content">
					<div class="modal-header">
						<h5 class="modal-title" id="mdlModifyStatsLabel">Modify Stats</h5>
						<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
					</div>
					<div class="modal-body" id="mdlModifyStatsBody" style="max-height: 78vh; overflow-y:auto;">
						<div class="row">
							<div class="col-6">
								<label>Attack</label>
								<input type="number" class="form-control weapon-attack-input data-value default-to-zero" data-label="attack" value="0">
							</div>
							<div class="col-6">
								<label>Defense</label>
								<input type="number" onblur="validateInput(this);" class="form-control weapon-defense-input data-value default-to-zero" data-label="defense" value="0">
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Element Damage Type</label>
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
							</div>
							<div class="col-6">
								<label>Element Damage</label>
								<input class="form-control weapon-element-damage-input data-value" type="text" data-label="element-damage">
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Element Damage Type 2</label>
								<select class="form-control form-select weapon-element-2-input data-value" data-label="element-2">
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
							</div>
							<div class="col-6">
								<label>Element Damage 2</label>
								<input class="form-control weapon-element-damage-2-input data-value" type="text" data-label="element-damage-2">
							</div>
						</div>
						<div class="row">
							<div class="col-6">
								<label>Affinity</label>
								<input type="number" onblur="validateInput(this);" class="form-control weapon-affinity-input data-value default-to-zero" data-label="affinity" value="0">
							</div>
							<div class="col-6 hide-on-invalid-game" data-valid-games="MHWI">
								<label>Elderseal</label>
								<input type="text" class="form-control weapon-elderseal-input data-value" data-label="elderseal">
							</div>
						</div>
						<div class="row hide-on-invalid-game" data-valid-games="MHRS">
							<div class="col-6">
								<label>Rampage Slots</label>
								<input type="number" onblur="validateInput(this);" class="form-control weapon-rampage-slots-input data-value" data-label="rampage-slots">
							</div>
							<div class="col-6">
								<label>Rampage Decoration Level</label>
								<input type="number" onblur="validateInput(this);" class="form-control weapon-rampage-deco-input data-value" data-label="rampage-deco">
							</div>
						</div>
						<div class="row hide-on-invalid-game" data-valid-games="MHWilds,MHWI">
							<div class="col-6">
								<label>Armor Skill 1</label>
								<input type="text" class="form-control weapon-armor-skill-input data-value" data-label="armor-skill">
							</div>
							<div class="col-6">
								<label>Armor Skill 2</label>
								<input type="text" class="form-control weapon-armor-skill-2-input data-value" data-label="armor-skill-2">
							</div>
						</div>
						<div class="row hide-on-invalid-weapon" id="divSpecialWeaponField" data-valid-weapons="Bo,CB,GL,HH,IG,SA,HBG,LBG">
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="WeaponTemplate.applyStats();">Apply</button>
						<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
					</div>
				</div>
			</div>
		</div>`);
		$(".hide-on-invalid-game").each(function () {
			if (!$(this).attr("data-valid-games").split(',').some(function (item) { return item == $("#ddlGameSelect").val(); })) {
				$(this).hide();
			}
			else {
				$(this).show();
			}
		});
    }
    static getHeader()
    {
		return `<tr style="vertical-align:middle;" data-template="header">
    <th scope="col" style="width:0.375rem">Move</th>
    <th scope="col" style="width:1rem">Forge?</th>
    <th scope="col" style="width:1rem">Rollback?</th>
    <th scope="col" style="width:15rem;">Weapon</th>
    <th scope="col" style="width:10rem;">Upgraded From</th>
    <th scope="col" style="width:9rem;">Path Link</th>
    <th scope="col" style="width:8rem;">Icon Type</th>
    <th scope="col" style="width:4.5rem;">Rarity</th>
    <th scope="col" style="width:4rem;">Stats</th>
    <th scope="col" style="width:4rem;">Decos</th>
    <th scope="col" style="width:4rem;">Sharpness</th>
    <th scope="col" style="width:0.375rem">Options</th>
</tr>`;
    }
    static getRow()
    {
        return `<tr style="vertical-align:middle;" class="table-content-row" data-template="row">
	<td class="ignore-generate" style="text-align:center">
		<div class="col">
			<div class="row pb-1">
				<div class="col">
					<button style="padding: .25rem; margin-right:.25rem;" class="btn btn-primary bi bi-arrow-up float-start" onclick="$(this).parents('.table-content-row').after($(this).parents('.table-content-row').prev())"></button>
					<button style="padding: .25rem;" class="btn btn-primary bi bi-arrow-down float-start" onclick="$(this).parents('.table-content-row').before($(this).parents('.table-content-row').next())"></button>
				</div>
			</div>
		</div>
	</td>
	<td style="text-align:center; align-content:center">
		<input style="font-size:1.5rem;" class="form-check-input weapon-can-forge-input data-value m-auto" data-label="can-forge" type="checkbox" value="">
	</td>
	<td style="text-align:center; align-content:center">
		<input style="font-size:1.5rem;" class="form-check-input weapon-can-rollback-input data-value m-auto" data-label="can-rollback" type="checkbox" value="">
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
		<select class="form-control form-select weapon-path-link-input data-value" data-label="path-link">
			<option value=""></option>
		</select>
	</td>
	<td>
		<select class="form-control form-select weapon-icon-type-input data-value" data-label="icon-type">
			<option value=""></option>
		</select>
	</td>
	<td>
		<input type="number" onblur="validateInput(this);" class="form-control weapon-rarity-input data-value" data-label="rarity">
	</td>
	<td style="text-align:center; align-content:center">
		<input class="form-control weapon-stats-input data-value" hidden="hidden" type="text" data-label="stats"/>
		<button class="btn btn-primary ignore-generate" type="button" onclick="WeaponTemplate.modifyStats($(this).parent().parent(), validateComplexData, this, $(this).parent().children().first(), WeaponTemplate.validateStats);">Modify</button>
	</td>
	<td style="text-align:center; align-content:center">
		<input class="form-control weapon-decos-input data-value" hidden="hidden" type="text" data-label="decos"/>
		<button class="btn btn-primary ignore-generate" type="button" onclick="WeaponTemplate.modifyDecos($(this).parent().parent(), validateComplexData, this, $(this).parent().children().first(), WeaponTemplate.validateDecos);">Modify</button>
	</td>
	<td style="text-align:center; align-content:center" class="hide-on-invalid-weapon" data-invalid-weapons="Bo,HBG,LBG">
		<input class="form-control weapon-sharpness-input data-value" hidden="hidden" type="text" data-label="sharpness"/>
		<button class="btn btn-primary ignore-generate" type="button" onclick="WeaponTemplate.modifySharpness($(this).parent().parent(), validateComplexData, this, $(this).parent().children().first(), WeaponTemplate.validateSharpness);">Modify</button>
	</td>
	<td class="ignore-generate" style="text-align:center">
		<button type="button" onclick="duplicateRow($(this).parent().parent())" class="btn btn-primary" title="Duplicate the weapon."><i class="bi bi-copy"></i></button>
		<button type="button" onclick="$(this).parent().parent().remove();" class="btn btn-danger btn-delete-row" title="Delete this weapon."><i class="bi bi-trash"></i></button>
	</td>
</tr>`;
    }
	static modifySharpness(row, sharpnessCallback, arg1, arg2, arg3)
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
				var val = "0";
				if (prevVal != null)
				{
					val = prevVal[i2 - 1][i];
				}
				$("#txt" + bars[i] + "SharpnessHits" + i2).val(val);
				$($("#divPreviewBar" + i2 + " div div")[i]).attr("style", "background-color:#" + colorHexes[i] + ";height: inherit;flex:0 0 " + (val == 0 ? "0" : (parseInt(val) / 4)) + "%");
			}
		}
		new bootstrap.Modal($("#mdlModifySharpness")).show();
		WeaponTemplate.currentSharpnessCallbackArgs = [arg1, arg2, arg3];
		WeaponTemplate.currentSharpnessCallback = sharpnessCallback;
	}
	static modifyStats(row, statsCallback, arg1, arg2, arg3) {
		WeaponTemplate.currentStatsRow = row;
		$("#mdlModifyStats .modal-body").find(".data-value").each(function () {
			if ($(this).attr("type") == "number" && $(this).hasClass("default-to-zero")) {
				$(this).val("0");
			}
			else {
				$(this).val("");
			}
		})
		var prevVal = null;
		var prevValStr = $(WeaponTemplate.currentStatsRow).find(".weapon-stats-input").first().val();
		if (prevValStr != '') {
			prevVal = JSON.parse(prevValStr);
			if (prevVal != null) {
				var propertyNames = Object.getOwnPropertyNames(prevVal);
				for (var i = 0; i < propertyNames.length; i++) {
					var el = $("#mdlModifyStats .modal-body").find(".weapon-" + propertyNames[i] + "-input");
					var val = prevVal[propertyNames[i]];
					if (el.attr("type") == "number" && val == "") {
						el.val("0");
					}
					else {
						el.val(val);
					}
				}
			}
		}
		new bootstrap.Modal($("#mdlModifyStats")).show();
		WeaponTemplate.currentStatsCallbackArgs = [arg1, arg2, arg3];
		WeaponTemplate.currentStatsCallback = statsCallback;
	}
	static modifyDecos(row, decoCallback, arg1, arg2, arg3)
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
		WeaponTemplate.currentDecosCallbackArgs = [arg1, arg2, arg3];
		WeaponTemplate.currentDecosCallback = decoCallback;
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
		if (typeof (WeaponTemplate.currentSharpnessCallback) !== 'undefined') {
			return WeaponTemplate.currentSharpnessCallback(WeaponTemplate.currentSharpnessCallbackArgs[0], WeaponTemplate.currentSharpnessCallbackArgs[1], WeaponTemplate.currentSharpnessCallbackArgs[2]);
		}
	}
	static duplicateSharpness() {
		$("#txtRedSharpnessHits2").val($("#txtRedSharpnessHits1").val());
		$("#txtOrangeSharpnessHits2").val($("#txtOrangeSharpnessHits1").val());
		$("#txtYellowSharpnessHits2").val($("#txtYellowSharpnessHits1").val());
		$("#txtGreenSharpnessHits2").val($("#txtGreenSharpnessHits1").val());
		$("#txtBlueSharpnessHits2").val($("#txtBlueSharpnessHits1").val());
		$("#txtWhiteSharpnessHits2").val($("#txtWhiteSharpnessHits1").val());
		$("#txtPurpleSharpnessHits2").val($("#txtPurpleSharpnessHits1").val());
		WeaponTemplate.updatePreviewBar("2");
	}
	static applyStats() {
		if (typeof (WeaponTemplate.currentStatsRow) !== 'undefined') {
			var dataObj = {};
			$("#mdlModifyStatsBody").find(".data-value").each(function () {
				dataObj[$(this).attr("data-label")] = $(this).val();
			});
			$(WeaponTemplate.currentStatsRow).find(".weapon-stats-input").first().val(JSON.stringify(dataObj));
		}
		if (typeof (WeaponTemplate.currentStatsCallback) !== 'undefined') {
			return WeaponTemplate.currentStatsCallback(WeaponTemplate.currentStatsCallbackArgs[0], WeaponTemplate.currentStatsCallbackArgs[1], WeaponTemplate.currentStatsCallbackArgs[2]);
		}
	}
	static addRowToTblDecos()
	{
		$("#tblDecos tbody").append(`<tr style="vertical-align:middle;">
			<td>
				<input type="number" onblur="validateInput(this);" class="form-control decoration-level-input" data-label="level" value="0" />
			</td>
			<td>
				<input type="number" onblur="validateInput(this);" class="form-control decoration-qty-input" data-label="qty" value="0" />
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
		if (typeof (WeaponTemplate.currentDecosCallback) !== 'undefined') {

			return WeaponTemplate.currentDecosCallback(WeaponTemplate.currentDecosCallbackArgs[0], WeaponTemplate.currentDecosCallbackArgs[1], WeaponTemplate.currentDecosCallbackArgs[2]);
		}
	}
	static validateStats(value) {
		return true;
	}
	static validateDecos(value)
	{
		var valid = true;
		try {
			var obj = JSON.parse(value);
			for (var i = 0; i < obj.length; i++) {
				if (obj[i].Level == '' || obj[i].Qty == '' || /\D/.test(obj[i].Level) || /\D/.test(obj[i].Qty) || typeof obj[i].IsRampage != "boolean") {
					valid = false;
				}
			}
		}
		catch {
			valid = false;
		}
		return valid;
	}
	static validateSharpness(value)
	{
		var valid = true;
		try {
			var obj = JSON.parse(value);
			for (var i = 0; i < obj.length; i++) {
				for (var i2 = 0; i2 < obj[i].length; i2++) {
					if (obj[i][i2] == '' || /\D/.test(obj[i][i2])) {
						valid = false;
					}
				}
			}
		}
		catch {
			valid = false;
		}
		return valid;
	}
	static validateStats(value) {
		return true;
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
		let pathNames = Array.from($(".weapon-path-name-input").map(function () {
			return $(this).val();
		})).filter(function (value, index, array) {
			return value != '' && array.indexOf(value) === index;
		}).sort(function (a, b) {
			return a > b ? 1 : -1;
		}).map(function (item) {
			return `<option value="${item}">${item}</option>`;
		});
		$(".weapon-path-link-input").each(function () {
			var oldVal = $(this).val();
			$(this).find("option").remove();
			$(this).append('<option value=""></option');
			$(this).append(pathNames.concat("\n"));
			$(this).val(oldVal);
		});
    }
	static updateIcons()
	{
		$(".hide-on-invalid-game").each(function () {
			if (!$(this).attr("data-valid-games").split(',').some(function (item) { return item == $("#ddlGameSelect").val(); })) {
				$(this).hide();
			}
			else {
				$(this).show();
			}
		});
		var icons = `<option value=""></option>
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
					<option value="Bo">Bow</option>
					<option value="LBG">Light Bowgun</option>
					<option value="HBG">Heavy Bowgun</option>`;;
		var oldIcon = $("#ddlIconSelect").val();
		$("#ddlIconSelect option").remove();
		$("#ddlIconSelect").append(icons);
		$("#ddlIconSelect").val(oldIcon);
		$("#ddlIconSelect").on("change", function () {
			WeaponTemplate.weaponLoadSpecialTypes();
		});
		$(".weapon-icon-type-input").each(function () {
			var oldVal = $(this).val();
			$(this).children("option").remove();
			$(this).append(icons);
			$(this).val(oldVal);
		});
	}

	static weaponLoadSpecialTypes() {
		var weaponType = $("#ddlIconSelect").val();
		$(".hide-on-invalid-weapon").each(function () {
			var invalidWeapons = $(this).attr("data-invalid-weapons");
			if (invalidWeapons != "" && typeof (invalidWeapons) !== "undefined" && invalidWeapons.split(',').some(function (item) { return item == weaponType; })) {
				$(this).hide();
			}
			else {
				$(this).show();
			}
			var validWeapons = $(this).attr("data-valid-weapons");
			if (validWeapons != "" && typeof (validWeapons) !== "undefined" && !validWeapons.split(',').some(function (item) { return item == weaponType; })) {
				$(this).hide();
			}
			else {
				$(this).show();
			}
		});
		$("#divSpecialWeaponField").empty();
		switch (weaponType) {
			case "DB":
			case "GS":
			case "Hm":
			case "Ln":
			case "LS":
			case "SnS":
				break;
			case "Bo":
				$("#divSpecialWeaponField").append(`<div class="col-6">
								<label>Coatings (Comma-Separated)</label>
								<input type="text" class="form-control weapon-bo-coatings-input data-value" data-label="bo-coatings">
							</div>`);
				break;
			case "CB":
			case "SA":
				$("#divSpecialWeaponField").append(`<div class="col-6">
								<label>Phial Type</label>
								<input type="text" class="form-control weapon-` + weaponType.toLowerCase() + `-phial-type-input data-value" data-label="` + weaponType.toLowerCase() + `-phial-type">
							</div>`);
				break;
			case "GL":
				$("#divSpecialWeaponField").append(`<div class="col-6">
								<label>Shelling Type</label>
								<input type="text" class="form-control weapon-gl-shelling-type-input data-value" data-label="gl-shelling-type">
							</div>`);
				break;
			case "HH":
				$("#divSpecialWeaponField").append(`<div class="col-6">
								<label>Note 1</label>
								<input type="text" class="form-control weapon-hh-note-1-input data-value" data-label="hh-note-1">
							</div>
							<div class="col-6">
								<label>Note 2</label>
								<input type="text" class="form-control weapon-hh-note-2-input data-value" data-label="hh-note-2">
							</div>
							<div class="col-6">
								<label>Note 3</label>
								<input type="text" class="form-control weapon-hh-note-3-input data-value" data-label="hh-note-3">
							</div>`);
				break;
			case "IG":
				$("#divSpecialWeaponField").append(`
							<div class="col-6">
								<label>Kinsect Bonus</label>
								<input type="text" class="form-control weapon-ig-kinsect-bonus-input data-value" data-label="ig-kinsect-bonus">
							</div>`);
				break;
			case "HBG":
			case "LBG":
				if ($("#ddlGameSelect").val() != "MHWI") {
					$("#divSpecialWeaponField").append(`<div class="col-6">
								<label>Reload / Recoil</label>
								<input type="text" class="form-control weapon-` + weaponType.toLowerCase() + `-reload-recoil-input data-value" data-label="` + weaponType.toLowerCase() + `-reload-recoil">
							</div>
							<div class="col-6">
								<label>Special Ammo Type</label>
								<input type="text" class="form-control weapon-` + weaponType.toLowerCase() + `-special-ammo-type-input data-value" data-label="` + weaponType.toLowerCase() + `-special-ammo-type">
							</div>`);
				}
				else {
					$("#divSpecialWeaponField").append(`<div class="col-6">
								<label>Special Ammo Type</label>
								<input type="text" class="form-control weapon-` + weaponType.toLowerCase() + `-special-ammo-type-input data-value" data-label="` + weaponType.toLowerCase() + `-special-ammo-type">
							</div>`);
				}
				break;
		}
	}

	static pageLoadData(loadedData) {
		var pathLinkDict = [];
		var parentNameDict = [];
		$(".card-holder").remove();
		for (var i2 = 0; i2 < loadedData.length; i2++) {
			var pathData = loadedData[i2];
			if ($(".card-holder").length < (i2 + 1)) {
				addTreeCard();
			}
			var card = $($(".card-holder")[i2]);
			$(card).find(".card .card-header .weapon-path-name-input").val(pathData.pathName);
			if (typeof (pathData.treeType) !== 'undefined') {
				$("#ddlTemplateSelect").val(pathData.treeType);
			}
			if (typeof (pathData.game) !== 'undefined') {
				$("#ddlGameSelect").val(pathData.game);
			}
			if (typeof (pathData.defaultIconType) !== 'undefined') {
				$("#ddlIconSelect").val(pathData.defaultIconType);
			}
			for (var i = 0; i < pathData.data.length; i++) {
				var dataObj = pathData.data[i];
				if ($(card).find(".card .card-body .table-tree tbody tr").length < (i + 1)) {
					addEmptyRow($(card).find(".card .card-body .table-tree"));
				}
				var row = $($(card).find(".card .card-body .table-tree tbody tr")[i]);
				var classes = [".weapon-parent-input", ".weapon-path-link-input", ".weapon-name-input", ".weapon-icon-type-input", ".weapon-rarity-input", ".weapon-affinity-input", ".weapon-attack-input", ".weapon-defense-input", ".weapon-element-input", ".weapon-element-damage-input", ".weapon-sharpness-input", ".weapon-decos-input"];
				for (var i3 = 0; i3 < classes.length; i3++) {
					var thisClass = classes[i3];
					var el = row.find("td " + thisClass);
					var value = dataObj[thisClass.substring(thisClass.indexOf(".weapon-") + 8, thisClass.indexOf("-input"))];
					el.val(value);
					if (el.attr("type") == "number") {
						validateInput(el[0]);
					}
				}
				var statsInput = $($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-stats-input");
				var statsEls = $("#mdlModifyStats .modal-body").find(".data-value");
				var statObj = {};
				for (var incr = 0; incr < statsEls.length; incr++) {
					var statEl = $(statsEls[incr]);
					statObj[statEl.attr("data-label")] = dataObj[statEl.attr("data-label")];
				}
				$(statsInput[0]).val(JSON.stringify(statObj));
				validateComplexData($(statsInput.parent().children()[1]), statsInput[0], WeaponTemplate.validateStats);
				var decosInput = $($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-decos-input");
				validateComplexData($(decosInput.parent().children()[1]), decosInput[0], WeaponTemplate.validateDecos);
				var sharpnessInput = $($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-sharpness-input");
				validateComplexData($(sharpnessInput.parent().children()[1]), sharpnessInput[0], WeaponTemplate.validateSharpness);
				$($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-can-forge-input").prop("checked", dataObj["can-forge"] == true);
				$($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-can-rollback-input").prop("checked", dataObj["can-rollback"] == true);
				parentNameDict.push({ el: $($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-parent-input").first(), val: dataObj["parent"] });
				pathLinkDict.push({ el: $($(card).find(".card .card-body .table-tree tbody tr")[i]).find(".weapon-path-link-input").first(), val: dataObj["path-link"]});
			}
		}
		initRow();
		for (var i = 0; i < pathLinkDict.length; i++) {
			var pathLinkKvp = pathLinkDict[i];
			pathLinkKvp.el.val(pathLinkKvp.val);
		}
		for (var i = 0; i < parentNameDict.length; i++) {
			var parentNameKvp = parentNameDict[i];
			parentNameKvp.el.val(parentNameKvp.val);
		}
	}
	static getSaveData() {
		return getFinalData();
	}
	static getFinalData() {
		WeaponTemplate.finalData = [];
		$(".card-holder").each(function () {
			var dataObj = {
				pathName: $(this).find(".weapon-path-name-input").val(),
				"treeType": $("#ddlTemplateSelect").val(),
				"game": $("#ddlGameSelect").val(),
				"defaultIconType": $("#ddlIconSelect").val(),
				data: []
			};
			$(this).find(".card .card-body .table-tree tbody tr").each(function () {
				var newObj = {};
				$(this).find("td:not(.ignore-generate)").children().each(function ()
				{
					if (!$(this).hasClass("ignore-generate")) {
						if ($(this).hasClass("weapon-stats-input") && $(this).val() != '') {
							var statsObj = JSON.parse($(this).val());
							var propertyNames = Object.getOwnPropertyNames(statsObj);
							for (var i = 0; i < propertyNames.length; i++) {
								newObj[propertyNames[i]] = statsObj[propertyNames[i]];
							}
						}
						else if ($(this).hasClass("form-check-input")) {
							newObj[$(this).attr("data-label")] = $(this).prop("checked");
						}
						else {
							newObj[$(this).attr("data-label")] = $(this).val();
						}
					}
				});
				dataObj.data.push(newObj);
			});
			WeaponTemplate.finalData.push(dataObj);
		});
		return WeaponTemplate.finalData;
	}
    static generateTree()
    {
		var sharpnessBase = 'MHWISharpnessBase';
		var weaponLink = '';
		var maxSharpnessCount = -1;
		getFinalData();
		callGenerator(WeaponTemplate.finalData, sharpnessBase, maxSharpnessCount, $("#txtPathName").val());
    }
}