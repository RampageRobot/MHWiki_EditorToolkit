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