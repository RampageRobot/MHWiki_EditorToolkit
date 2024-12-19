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

var draggedRow;
var initRowIndex = -1;
function row_start(e){
    if (e.target.tagName == "TR")
    {
        draggedRow = e.target;
        initRowIndex = Array.from(draggedRow.parentNode.children).indexOf(draggedRow);
    }
}

function row_dragover(e){
    if (typeof(draggedRow) !== 'undefined')
    {
        if (matchRowAncestor(e.target, draggedRow.parentNode.id))
        {
            if(getRowIndex(e.target)>=initRowIndex)
                getRowNode(e.target).after(getRowNode(draggedRow));
            else
                getRowNode(e.target).before(getRowNode(draggedRow));
        }
    }
}

function getRowNode(eventTarget)
{
    while (eventTarget.tagName != "TR")
    {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget;
}

function getRowIndex(eventTarget)
{
    var rowNode;
    var parentNode = eventTarget.parentNode;
    while (parentNode.tagName != "TBODY" && parentNode.tagName != "THEAD")
    {
        if (parentNode.tagName == "TR")
        {
            rowNode = parentNode;
        }
        parentNode = parentNode.parentNode;
    }
    if (parentNode.tagName == "THEAD")
    {
        return -1;
    }
    else
    {
        return Array.from(parentNode.children).indexOf(rowNode);
    }
}

function matchRowAncestor(node, targetId)
{
    while (node.tagName != "TBODY" && node.tagName != 'BODY')
    {
        node = node.parentNode;
    }
    return node.id == targetId;
}

var draggedCard;
function card_start(event) {
    if (event.target.className == 'card') {
        console.log(event.target);
        draggedCard = event.target;
    }
}
function card_dragover(e) {
    if (typeof (draggedCard) !== 'undefined') {
        if (isCardAncestor(e.target)) {
            let children = Array.from(getCardHolder(e.target).parentNode.children);
            if (children.indexOf(getCardHolder(e.target)) > children.indexOf(getCardHolder(draggedCard)))
                getCardHolder(e.target).after(getCardHolder(draggedCard));
            else
                getCardHolder(e.target).before(getCardHolder(draggedCard));
        }
    }
}

function getCardNode(eventTarget) {
    while (eventTarget.className != "table-holder") {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget;
}

function getCardIndex(eventTarget) {
    var tableNode;
    var parentNode = eventTarget.parentNode;
    while (parentNode.className != "table-container") {
        if (parentNode.className == "table-holder") {
            tableNode = parentNode;
        }
        parentNode = parentNode.parentNode;
    }
    return Array.from(parentNode.children).indexOf(tableNode);
}

function isCardAncestor(eventTarget) {
    while (eventTarget.className != "card" && eventTarget.tagName != "BODY") {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget.className == "card";
}

function getCardHolder(node) {
    while (!node.className.includes("card-holder") && node.tagName != "BODY") {
        node = node.parentNode;
    }
    if (node.className.includes("card-holder")) {
        return node;
    }
    else {
        return null;
    }
}