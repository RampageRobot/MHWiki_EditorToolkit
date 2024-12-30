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
            addRow($(".card .table-tree tbody"));
            break;
    }
    initTemplate();
}

function addRow(table)
{
    $(table).append($(srcTreeRow).clone(true));
    initRow();
    updateIcons();
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