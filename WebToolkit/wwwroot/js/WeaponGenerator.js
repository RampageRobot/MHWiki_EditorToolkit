var materialRoot;
var currentSharpnessCallback;
var currentSharpnessCallbackArgs;
var currentMaterialsRow;
var currentMaterialsCallback;
var currentMaterialsCallbackArgs;
var mdlGenerate;
$(window).on("load", function () {
    materialRoot = $($("table.material-table tbody tr")[0]).clone(true);
    updateIcons();
    updateWeaponFields();
});
function getSaveData() {
    return getFinalData();
}
function getFinalData() {
    var finalData = {};
    $("#divMainCol").find(".data-value").each(function () {
        finalData[$(this).attr("data-label")] = $(this).val();
    });
    return finalData;
}
function pageLoadData(loadedData) {
    $(".data-value").each(function () {
        $(this).val(loadedData[$(this).attr("data-label")]);
    });
    $(".weapon-game-input").trigger("change");
    $(".weapon-type-input").trigger("change");
}
function generateWeapon() {
    callGenerator(getFinalData());
}
function updatePreviewBar(bar) {
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
function modifySharpness(sharpnessCallback, arg1, arg2, arg3)
{
    var prevVal = null;
    var prevValStr = $(".weapon-sharpness-input").first().val();
    if (prevValStr != '') {
        prevVal = JSON.parse(prevValStr);
    }
    var bars = ["Red", "Orange", "Yellow", "Green", "Blue", "White", "Purple"];
    var colorHexes = ["E41700", "F36A01", "FBFF00", "49DD79", "3E6AFF", "E6E6E5", "E401C8"];
    for (var i = 0; i < bars.length; i++) {
        for (var i2 = 1; i2 <= 2; i2++) {
            var val = "";
            if (prevVal != null) {
                val = prevVal[i2 - 1][i];
                if (val == "0") {
                    val = "";
                }
            }
            $("#txt" + bars[i] + "SharpnessHits" + i2).val(val);
            $($("#divPreviewBar" + i2 + " div div")[i]).attr("style", "background-color:#" + colorHexes[i] + ";height: inherit;flex:0 0 " + (val == 0 ? "0" : (parseInt(val) / 4)) + "%");
        }
    }
    new bootstrap.Modal($("#mdlModifySharpness")).show();
    currentSharpnessCallbackArgs = [arg1, arg2, arg3];
    currentSharpnessCallback = sharpnessCallback;
}
function applySharpness() {
    $(".weapon-sharpness-input").first().val(JSON.stringify([
        [$("#txtRedSharpnessHits1").val(), $("#txtOrangeSharpnessHits1").val(), $("#txtYellowSharpnessHits1").val(), $("#txtGreenSharpnessHits1").val(), $("#txtBlueSharpnessHits1").val(), $("#txtWhiteSharpnessHits1").val(), $("#txtPurpleSharpnessHits1").val()],
        [$("#txtRedSharpnessHits2").val(), $("#txtOrangeSharpnessHits2").val(), $("#txtYellowSharpnessHits2").val(), $("#txtGreenSharpnessHits2").val(), $("#txtBlueSharpnessHits2").val(), $("#txtWhiteSharpnessHits2").val(), $("#txtPurpleSharpnessHits2").val()]
    ]));
    if (typeof (currentSharpnessCallback) !== 'undefined') {
        return currentSharpnessCallback(currentSharpnessCallbackArgs[0], currentSharpnessCallbackArgs[1], currentSharpnessCallbackArgs[2]);
    }
}
function validateSharpness(value) {
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
function modifyUpgradeMaterials(callback, arg1, arg2, arg3) {
    $(".material-table tbody tr").remove();
    var prevVal = null;
    var prevValStr = $(arg2).first().val();
    if (prevValStr != '') {
        prevVal = JSON.parse(prevValStr);
        if (prevVal != null) {
            for (var i = 0; i < prevVal.length; i++) {
                addRow();
                var row = $(".material-table tbody tr").last();
                $(row).find(".material-name-input").val(prevVal[i].name);
                $(row).find(".material-icon-input").val(prevVal[i].icon);
                $(row).find(".material-color-input").val(prevVal[i].color);
                $(row).find(".material-quantity-input").val(prevVal[i].quantity);
            }
        }
    }
    new bootstrap.Modal($("#mdlModifyMaterials")).show();
    currentMaterialsCallbackArgs = [arg1, arg2, arg3];
    currentMaterialsCallback = callback;
    currentMaterialsRow = arg2;
}
function applyMaterials() {
    var materials = [];
    $(".material-table tbody tr").each(function () {
        var materialObj = {};
        $(this).find("td:not(.ignore-generate)").children().each(function () {
            materialObj[$(this).attr("data-label")] = $(this).val();
        });
        materials.push(materialObj);
    });
    $(currentMaterialsRow).val(JSON.stringify(materials));
    if (typeof (currentMaterialsCallback) !== 'undefined') {
        return currentMaterialsCallback(currentMaterialsCallbackArgs[0], currentMaterialsCallbackArgs[1], currentMaterialsCallbackArgs[2]);
    }
}
function validateMaterials(value) {
    return true;
}
function addRow() {
    $(".material-table tbody").first().append(materialRoot.clone(true));
}
function updateWeaponFields() {
    $(".hide-on-invalid-weapon").each(function () {
        if ($(this).attr("data-valid-weapon") != $("#ddlIconSelect").val()) {
            $(this).hide();
        }
        else {
            $(this).show();
        }
    });
}
function updateIcons() {
    var icons = '';
    var colors = '';
    $(".hide-on-invalid-game").each(function () {
        if (!$(this).attr("data-valid-games").split(',').some(function (item) { return item == $("#ddlGameSelect").val(); })) {
            $(this).hide();
        }
        else {
            $(this).show();
        }
    });
    var colors = '';
    switch ($("#ddlGameSelect").val()) {
        case "MHG":
            colors = `<option value=""></option>
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
            colors = `<option value=""></option>
                    <option value="Blue">Blue</option>
                    <option value="Brown">Brown</option>
                    <option value="Dark Blue">Dark Blue</option>
                    <option value="Dark Purple">Dark Purple</option>
                    <option value="Emerald">Emerald</option>
                    <option value="Gray">Gray</option>
                    <option value="Green">Green</option>
                    <option value="Lemon">Lemon</option>
                    <option value="Light Blue">Light Blue</option>
                    <option value="Light Green">Light Green</option>
                    <option value="Moss">Moss</option>
                    <option value="Orange">Orange</option>
                    <option value="Pink">Pink</option>
                    <option value="Purple">Purple</option>
                    <option value="Red">Red</option>
                    <option value="Rose">Rose</option>
                    <option value="Tan">Tan</option>
                    <option value="Vermilion">Vermilion</option>
                    <option value="Violet">Violet</option>
                    <option value="White">White</option>
                    <option value="Yellow">Yellow</option>`;
            break;
        case "MHWilds":
        case "MHRS":
            colors = `<option value=""></option>
                    <option value="Blue">Blue</option>
                    <option value="Brown">Brown</option>
                    <option value="Dark Blue">Dark Blue</option>
                    <option value="Dark Purple">Dark Purple</option>
                    <option value="Gray">Gray</option>
                    <option value="Green">Green</option>
                    <option value="Light Blue">Light Blue</option>
                    <option value="Orange">Orange</option>
                    <option value="Pink">Pink</option>
                    <option value="Purple">Purple</option>
                    <option value="Red">Red</option>
                    <option value="Vermilion">Vermilion</option>
                    <option value="White">White</option>
                    <option value="Yellow">Yellow</option>`;
            break;
    }
    $(".material-color-input").each(function () {
        var oldVal = $(this).val();
        $(this).children("option").remove();
        $(this).append(colors);
        $(this).val(oldVal);
    });
    switch ($("#ddlGameSelect").val()) {
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
        case "MHRS":
        case "MHWilds":
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
					<option value="Bo">Bow</option>
					<option value="LBG">Light Bowgun</option>
					<option value="HBG">Heavy Bowgun</option>`;
            break;
    }
    var oldIcon = $("#ddlIconSelect").val();
    $("#ddlIconSelect option").remove();
    $("#ddlIconSelect").append(icons);
    $("#ddlIconSelect").val(oldIcon);
}

var draggedRow;
var initRowIndex = -1;
function row_start(e) {
    if (e.target.tagName == "TR") {
        draggedRow = e.target;
        initRowIndex = Array.from(draggedRow.parentNode.children).indexOf(draggedRow);
    }
}

function row_dragover(e) {
    if (typeof (draggedRow) !== 'undefined') {
        if (matchRowAncestor(e.target, draggedRow.parentNode.id)) {
            if (getRowIndex(e.target) >= initRowIndex)
                getRowNode(e.target).after(getRowNode(draggedRow));
            else
                getRowNode(e.target).before(getRowNode(draggedRow));
        }
    }
}

function getRowNode(eventTarget) {
    while (eventTarget.tagName != "TR") {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget;
}

function getRowIndex(eventTarget) {
    var rowNode;
    var parentNode = eventTarget.parentNode;
    while (parentNode.tagName != "TBODY" && parentNode.tagName != "THEAD") {
        if (parentNode.tagName == "TR") {
            rowNode = parentNode;
        }
        parentNode = parentNode.parentNode;
    }
    if (parentNode.tagName == "THEAD") {
        return -1;
    }
    else {
        return Array.from(parentNode.children).indexOf(rowNode);
    }
}

function matchRowAncestor(node, targetId) {
    while (node.tagName != "TBODY" && node.tagName != 'BODY') {
        node = node.parentNode;
    }
    return node.id == targetId;
}