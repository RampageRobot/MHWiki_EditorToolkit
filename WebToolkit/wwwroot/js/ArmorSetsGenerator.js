var skillRoot = {};
var materialRoot = {};
var rootEl = {};
var mdlGenerate;
$(window).on("load", function () {
    rootEl = $($(".card")[0]).parent().clone(true);
    skillRoot = $($("table.skill-table tbody tr")[0]).clone(true);
    materialRoot = $($("table.material-table tbody tr")[0]).clone(true);
});
function getSaveData() {
    return getFinalData();
}
function getFinalData() {
    var finalData = {
        setName: $("#txtSetName").val(),
        game: $("#ddlGameSelect").val(),
        maleFrontImg: $("#txtMaleFront").val(),
        maleBackImg: $("#txtMaleBack").val(),
        femaleFrontImg: $("#txtFemaleFront").val(),
        femaleBackImg: $("#txtFemaleBack").val(),
        setSkill1Name: $("#txtSetSkill1").val(),
        setSkill2Name: $("#txtSetSkill2").val(),
        groupSkill1Name: $("#txtGroupSkill1").val(),
        groupSkill2Name: $("#txtGroupSkill2").val(),
        rarity: $("#txtSetRarity").val(),
        pieces: []
    };
    $("#divCardContainer div.col-6 div.card").each(function () {
        var dataObj = {
            skills: [],
            materials: []
        };
        $(this).find(".piece-data").each(function () {
            if ($(this).hasClass("form-check-input")) {
                dataObj[$(this).attr("data-label")] = $(this).prop("checked");
            }
            else {
                dataObj[$(this).attr("data-label")] = $(this).val();
            }
        });
        $(this).find(".skill-table tbody tr").each(function () {
            var skillObj = {};
            $(this).find("td:not(.ignore-generate)").children().each(function () {
                if ($(this).hasClass("form-check-input")) {
                    skillObj[$(this).attr("data-label")] = $(this).prop("checked");
                }
                else {
                    skillObj[$(this).attr("data-label")] = $(this).val();
                }
            });
            dataObj.skills.push(skillObj);
        });
        $(this).find(".material-table tbody tr").each(function () {
            var materialObj = {};
            $(this).find("td:not(.ignore-generate)").children().each(function () {
                if ($(this).hasClass("form-check-input")) {
                    materialObj[$(this).attr("data-label")] = $(this).prop("checked");
                }
                else {
                    materialObj[$(this).attr("data-label")] = $(this).val();
                }
            });
            dataObj.materials.push(materialObj);
        });
        finalData.pieces.push(dataObj);
    });
    return finalData;
}
function pageLoadData(loadedData) {
    if (typeof (loadedData.setName) !== 'undefined') {
        $("#txtSetName").val(loadedData.setName);
    }
    if (typeof (loadedData.game) !== 'undefined') {
        $("#ddlGameSelect").val(loadedData.game);
    }
    if (typeof (loadedData.maleFrontImg) !== 'undefined') {
        $("#txtMaleFront").val(loadedData.maleFrontImg);
    }
    if (typeof (loadedData.maleBackImg) !== 'undefined') {
        $("#txtMaleBack").val(loadedData.maleBackImg);
    }
    if (typeof (loadedData.femaleFrontImg) !== 'undefined') {
        $("#txtFemaleFront").val(loadedData.femaleFrontImg);
    }
    if (typeof (loadedData.femaleBackImg) !== 'undefined') {
        $("#txtFemaleBack").val(loadedData.femaleBackImg)
    }
    if (typeof (loadedData.setSkill1Name) !== 'undefined') {
        $("#txtSetSkill1").val(loadedData.setSkill1Name);
    }
    if (typeof (loadedData.setSkill2Name) !== 'undefined') {
        $("#txtSetSkill2").val(loadedData.setSkill2Name);
    }
    if (typeof (loadedData.groupSkill1Name) !== 'undefined') {
        $("#txtGroupSkill1").val(loadedData.groupSkill1Name);
    }
    if (typeof (loadedData.groupSkill2Name) !== 'undefined') {
        $("#txtGroupSkill2").val(loadedData.groupSkill2Name);
    }
    if (typeof (loadedData.rarity) !== 'undefined') {
        $("#txtSetRarity").val(loadedData.rarity);
    }
    for (var i = 0; i < loadedData.pieces.length; i++) {
        var dataObj = loadedData.pieces[i];
        if ($("#divCardContainer div.col-6").length < (i + 1)) {
            $('#divCardContainer').append($(rootEl.clone(true)));
        }
        var card = $("#divCardContainer div.card").last();
        $(card).find(".piece-data").each(function () {
            if ($(this).hasClass("form-check-input")) {
                $(this).prop("checked", dataObj[$(this).attr("data-label")] == true);
            }
            else {
                $(this).val(dataObj[$(this).attr("data-label")]);
            }
        });
        for (var i2 = 0; i2 < dataObj.skills.length; i2++) {
            var skill = dataObj.skills[i2];
            if ($(card).find(".skill-table tbody tr").length < (i2 + 1)) {
                addRow($(card).find(".skill-table").first());
            }
            row = $(card).find(".skill-table tbody tr").last();
            $(row).find("td:not(.ignore-generate)").children().each(function () {
                if ($(this).hasClass("form-check-input")) {
                    $(this).prop("checked", skill[$(this).attr("data-label")] == true);
                }
                else {
                    $(this).val(skill[$(this).attr("data-label")]);
                }
            });
        }
        for (var i2 = 0; i2 < dataObj.materials.length; i2++) {

            var material = dataObj.materials[i2];
            if ($(card).find(".material-table tbody tr").length < (i2 + 1)) {
                addRow($(card).find(".material-table").first());
            }
            row = $(card).find(".material-table tbody tr").last();
            $(row).find("td:not(.ignore-generate)").children().each(function () {
                if ($(this).hasClass("form-check-input")) {
                    $(this).prop("checked", material[$(this).attr("data-label")] == true);
                }
                else {
                    $(this).val(material[$(this).attr("data-label")]);
                }
            });
        }
    }
}
function generateSet() {
    callGenerator(getFinalData());
}
function updateColors() {
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
}
function addRow(table) {
    if ($(table).hasClass("skill-table")) {
        $(table).children("tbody").append(skillRoot.clone(true));
    }
    else {
        $(table).children("tbody").append(materialRoot.clone(true));
    }
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