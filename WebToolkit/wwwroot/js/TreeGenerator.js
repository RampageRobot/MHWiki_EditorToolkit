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
    // Appends before at the specified index
    // 0 means it'll append in the first row.
    else {
        if ($(table).find("tbody > tr").children().length == 0) {
            $(table).append(row);
        }
        console.log(index)
        $(table).find("tbody > tr").eq(index).before(row);
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
    addRow(table, duplicatedRow, row.index() - 1)

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

    if (weaponDict["Sharpness1"]) {
        var sharpness = [parseSharpnessCsv(weaponDict["Sharpness1"]), parseSharpnessCsv(weaponDict["Sharpness2"])];
        row.find("[data-label=sharpness]").val(JSON.stringify(sharpness));
        row.find("[data-label=sharpness]").parent().find("button").addClass("btn-success");
    }
}

function parseSharpnessCsv(sharpnessData) {
    return [
        sharpnessData["Red"] ? sharpnessData["Red"].toString() : "0",
        sharpnessData["Orange"] ? sharpnessData["Orange"].toString() : "0",
        sharpnessData["Yellow"] ? sharpnessData["Yellow"].toString() : "0",
        sharpnessData["Green"] ? sharpnessData["Green"].toString() : "0",
        sharpnessData["Blue"] ? sharpnessData["Blue"].toString() : "0",
        sharpnessData["White"] ? sharpnessData["White"].toString() : "0",
        sharpnessData["Purple"] ? sharpnessData["Purple"].toString() : "0",
    ]
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

function moveRowBefore(row) {
    var rowIndex = getRowIndex(row);

    // First row on the said table
    if (rowIndex == 0) {
        var treeContainer = row.closest("[class*='card-holder']");

        var treeContainerIndex = treeContainer.index();
        // There is a tree before the current tree, remove row and append at the end of that tree
        if (treeContainerIndex > 0) {
            var treesContainer = treeContainer.parent();
            var container = $(treesContainer).children().eq(treeContainerIndex - 1);
            var table = container.find('table');
            row.parent().remove(row);
            addRow(table, row);
        }

        var treesContainer = treeContainer.parent();

        return;
    }
    moveButton.parents('.table-content-row').after(moveButton.parents('.table-content-row').prev())
}

function moveRowAfter(row) {
    var rowIndex = getRowIndex(row);

    // Last row on the said table
    if (rowIndex == getRowsAmount(row.parent().parent()) - 1) {
        var treeContainer = row.closest("[class*='card-holder']");

        var treeContainerIndex = treeContainer.index();
        var treesContainer = treeContainer.parent();
        if (treeContainerIndex < treesContainer.children().length - 1) {
            console.log("tree found after this one")
            // There is a tree after the current tree, remove row and append at the start of that tree
            var container = $(treesContainer).children().eq(treeContainerIndex + 1);
            console.log(container);
            var table = container.find('table');
            console.log(table);
            row.parent().remove(row);
            addRow(table, row, 0);
        }

        var treesContainer = treeContainer.parent();

        return;
    }
    moveButton.parents('.table-content-row').before(moveButton.parents('.table-content-row').next())
}

function getRowIndex(row) {
    return row.parent().parent().find("tbody > tr").index(row);
}

function getRowsAmount(table) {
    return table.find("tbody").children().length;
}