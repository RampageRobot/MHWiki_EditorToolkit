var srcTreeRow = {};
var srcTreeCard = {};
var mdlGenerate;
function initRow()
{

}
function initTemplate()
{

}
function generateTree()
{

}
function updateIcons()
{

}
function getSaveData() {

}
function getFinalData() {

}
function pageLoadData(loadedData) {

}
function copyToClipboard()
{
    var copyText = document.getElementById("txtResults");
        copyText.select();
        copyText.setSelectionRange(0, 99999);
        navigator.clipboard.writeText(copyText.value);
        alert("Copied!");
}
var dataArray = [];
$(window).on("load", function () {
    loadTemplate();
});

function loadTemplate()
{
    $("#tblTree thead tr").remove();
    $("#tblTree tbody tr").remove();
    switch ($("#ddlTemplateSelect").val())
    {
        case "Weapon":
            srcTreeRow = WeaponTemplate.getRow();
            $(".card .table-tree thead").append($(WeaponTemplate.getHeader()).clone(true));
            srcTreeCard = $(".card-holder").first().clone(true);
            initRow = WeaponTemplate.initRow;
            initTemplate = WeaponTemplate.initTemplate;
            generateTree = WeaponTemplate.generateTree;
            updateIcons = WeaponTemplate.updateIcons;
            getFinalData = WeaponTemplate.getFinalData;
            pageLoadData = WeaponTemplate.pageLoadData;
            getSaveData = WeaponTemplate.getSaveData;
            addEmptyRow($(".card .table-tree tbody"));
            break;
    }
    initTemplate();
}

function addEmptyRow(table)
{
    addRow(table, $(srcTreeRow).clone(true));
}

function addRow(table, row, index = -1) {
    // Append in the end of the table
    if (index == -1) {
        $(table).append(row);
        initRow();
        updateIcons();
    }
    // Append at specified index
    else {
        $(table).find("tbody > tr").eq(index).after(row);
        initRow();
        updateIcons();
    }
}

function collapseCard(button) {
    if ($(button).attr("data-is-collapsed") == "true") {
        $(button).parents("div.card-holder").first().find("div.card div.card-body", true).first().removeClass("d-none");
        $(button).attr("data-is-collapsed", "false");
        $(button).children().first().attr("class", "bi bi-arrows-collapse");
        $(button).attr("title", "Collapse this card.");
    }
    else {
        $(button).parents("div.card-holder").first().find("div.card div.card-body", true).first().addClass("d-none");
        $(button).attr("data-is-collapsed", "true");
        $(button).children().first().attr("class", "bi bi-arrows-expand");
        $(button).attr("title", "Expand this card.");
    }
}

function addTreeCard()
{
    var el = $(srcTreeCard).clone(true);
    $("#divCardContainer").append(el);
    $(el).find(".table-tree tbody").append($(srcTreeRow).clone(true));
    initRow();
    updateIcons();
}

function duplicateRow(row) {
    
    var duplicatedRow = $(srcTreeRow).clone(true);
    var table = $(row).parent().parent();
    addRow(table, duplicatedRow, row.index())

    duplicatedRow.find("[data-label=can-rollback]").prop("checked", row.find("[data-label=can-rollback]").is(":checked"))

    duplicatedRow.find("[data-label=parent]").val(row.find("[data-label=name]").val())

    var icontype = row.find("[data-label=icon-type]").find(":selected").val()
    if (icontype) {
        duplicatedRow.find("[data-label=icon-type]").val(icontype).change()
    }

    var stats = row.find("[data-label=stats]").val()
    if (stats) {
        duplicatedRow.find("[data-label=stats]").val(stats)
        duplicatedRow.find("[data-label=stats]").parent().find("button").addClass("btn-success")
    }

    var decos = row.find("[data-label=decos]").val()
    if (decos) {
        duplicatedRow.find("[data-label=decos]").val(decos)
        duplicatedRow.find("[data-label=decos]").parent().find("button").addClass("btn-success")
    }

    var sharpness = row.find("[data-label=sharpness]").val()
    if (sharpness) {
        duplicatedRow.find("[data-label=sharpness]").val(sharpness)
        duplicatedRow.find("[data-label=sharpness]").parent().find("button").addClass("btn-success")
    }
}

function addRowFromCsv(weaponDict) {
    var row = $(srcTreeRow).clone(true);
    var table = $(".card .table-tree tbody");
    addRow(table, row)

    row.find("[data-label=name]").val(weaponDict["Name"])
    row.find("[data-label=parent]").val(weaponDict["Parent"])
    row.find("[data-label=rarity]").val(weaponDict["Rarity"])

    var attackObject = {
        "attack": weaponDict["Attack"],
        "defense": weaponDict["Defense"],
        "element": weaponDict["Element1"],
        "element-damage": weaponDict["Element1Attack"],
        "element-2": weaponDict["Element2"],
        "element-damage-2": weaponDict["Element2Attack"],
        "affinity": weaponDict["Affinity"],
        "elderseal": weaponDict["Elderseal"],
        "rampage-slots": "",
        "rampage-deco": "",
        "armor-skill": "",
        "armor-skill-2": ""
    }
    row.find("[data-label=stats]").val(JSON.stringify(attackObject))
    row.find("[data-label=stats]").parent().find("button").addClass("btn-success")

    var decos = getDecos(parseInt(weaponDict["DecoSlot1"]), parseInt(weaponDict["DecoSlot2"]), parseInt(weaponDict["DecoSlot3"]));
    row.find("[data-label=decos]").val(decos);
    row.find("[data-label=decos]").parent().find("button").addClass("btn-success");
}

function getDecos(levelSlot1, levelSlot2, levelSlot3) {
    var decos = [];
    var amountPerLevel = [0, 0, 0, 0];
    if (levelSlot1) {
        amountPerLevel[levelSlot1 - 1] += 1
    }
    if (levelSlot2) {
        amountPerLevel[levelSlot2 - 1] += 1
    }
    if (levelSlot3) {
        amountPerLevel[levelSlot3 - 1] += 1
    }
    for (var i = 0; i < amountPerLevel.length; i++) {
        if (amountPerLevel[i] > 0)
            decos.push({ "Level": (i + 1).toString(), "Qty": amountPerLevel[i].toString(), "IsRampage": false });
    }

    var returnValue = JSON.stringify(decos);
    return returnValue;
}
