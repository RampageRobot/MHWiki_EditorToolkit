var mdlGenerate;
var rootEl = {};
var rootItemEl = {};
var itemArray = [];
$(window).on("load", function () {
    $(".item-table").attr("id", crypto.randomUUID());
    rootEl = $($(".card")[0]).parent().clone(true);
    var rowEl = $($("#tblGlobalItems tbody tr")[0]);
    rootItemEl = rowEl.clone(true);
    var rowId = crypto.randomUUID();
    $(rowEl).attr('id', rowId);
    $(rowEl).find(".btn-delete-global-row").attr("onclick", "deleteGlobalRow('" + rowId + "');");
});
function addGlobalRow()
{
    var newRow = rootItemEl.clone(true);
    var rowId = crypto.randomUUID();
    $("#tblGlobalItems tbody").append(newRow);
    $(newRow).attr('id', rowId);
    $(newRow).find(".btn-delete-global-row").attr("onclick", "deleteGlobalRow('" + rowId + "');");
}
function deleteGlobalRow(rowId)
{
    $("#" + rowId).remove();
    setItemArray();
}
function setItemArray()
{
    itemArray = [];
    $("#tblGlobalItems tbody tr").each(function () {
        var itemName = $($($(this).children()[1]).children()[0]).val();
        if (itemName != '')
        {
            itemArray.push({
                "Include": $($($($(this).children()[0]).children()[0]).children()[0]).prop("checked"),
                "ItemName": itemName,
                "IconType": $($($(this).children()[2]).children()[0]).val(),
                "IconColor": $($($(this).children()[3]).children()[0]).val(),
                "Description": $($($(this).children()[4]).children()[0]).val(),
                "Rarity": $($($(this).children()[5]).children()[0]).val(),
                "Price": $($($(this).children()[6]).children()[0]).val()
            });
        }
    });
    $(".item-name-input").each(function () {
        var lastVal = $(this).val();
        $(this)
            .find('option')
            .remove()
            .end()
            .append("<option value=''></option>");
        itemArray.map(function (item) { return item.ItemName; }).sort(function (a, b) { return a > b ? 1 : -1;}).forEach((x) => $(this).append("<option value='" + x + "'>" + x + "</option>"));
        $(this).val(lastVal);
    });
    if (itemArray.map(function (item) {return item.ItemName;}).filter((item, index) => itemArray.map(function (item) {return item.ItemName;}).indexOf(item) !== index).length > 0)
    {
        $("#divItemList").addClass("item-list-warn");
        $("#divWarning").removeClass("d-none");
    }
    else
    {
        $("#divItemList").removeClass("item-list-warn");
        $("#divWarning").addClass("d-none");
    }
}
function copyToClipboard()
{
    var copyText = document.getElementById("txtResults");
        copyText.select();
        copyText.setSelectionRange(0, 99999);
        navigator.clipboard.writeText(copyText.value);
        alert("Copied!");
}
function dragend()
{
    $("[draggable=true]").removeAttr("draggable");
    draggedCard = undefined;
    draggedRow = undefined;
    draggedTable = undefined;
    $(".card-holder").each(function() {
        var cardId = $($(this).find("div.card-body")).attr("id");
        $($(this).children()[0]).attr("id", "card_" + cardId);
        $($(this).find("button.btn-drag-card")).attr("onmousedown", "setCardDraggable('card_" + cardId + "');");
        $($(this).find("button.btn-add-table")).attr("onclick", "addTableToRankCard('" + cardId + "');");
        $($(this).find("button.btn-delete-card")).attr("onclick", "$(this).parents('div.card-holder').first().remove();");
        $(this).find(".table-holder").each(function () {
            var tableId = $($(this).find("table")).attr("id");
            $($(this).find("button.btn-add-item")).attr("onclick", "addItemToTable('" + tableId + "');");
            $($(this).find("button.btn-delete-table")).attr("onclick", "$(this).parents('div.table-holder').first().remove();");
            $($(this).find("button.btn-drag-table")).attr("onmousedown", "setTableDraggable('" + tableId + "');");
            $(this).find("tbody tr").each(function() {
                var rowId = $(this).attr("id");
                $($(this).find("button.btn-delete-row")).attr("onclick", "$(this).parent().parent().remove();");
                $($(this).find("button.btn-drag-row")).attr("onmousedown", "setRowDraggable('" + rowId + "');");
            });
        });
    });
}
function setRowDraggable(rowId) {
    if (rowId === '')
    {
        el = $("#trFirstRow");
    }
    else
    {
        el = $("#" + rowId);
    }
    $(el).attr("draggable", true);
}
function setTableDraggable(tableId) {
    if (tableId === '')
    {
        el = $("#divFirstHolder");
    }
    else
    {
        el = $("#" + tableId).parent().parent().parent();
    }
    $(el).attr("draggable", true);
}
function setCardDraggable(cardId) {
    $("#" + cardId).attr("draggable", true);
}

function onlyUnique(value, index, array) {
    return array.indexOf(value) === index;
}

function addRankCard()
{
    let cardId = crypto.randomUUID();
    let tableId = crypto.randomUUID();
    let rowId = crypto.randomUUID();
    let rankCard = rootEl.clone(true);
    $(rankCard.children()[0]).attr("id", "card_" + cardId);
    $(rankCard.find("button.btn-drag-card")).attr("onmousedown", "setCardDraggable('card_" + cardId + "');");
    $(rankCard.find("label.label-rank-name")).attr("for", "txtRankName_" + cardId);
    $(rankCard.find("input.rank-name")).attr("id", "txtRankName_" + cardId);
    $(rankCard.find("button.btn-add-table")).attr("onclick", "addTableToRankCard('" + cardId + "');");
    $(rankCard.find("button.btn-delete-card")).attr("onclick", "$(this).parents('div.card-holder').first().remove();");
    $(rankCard.find("div.card-body")).attr("id", cardId);
    $(rankCard.find(".table-holder")).attr("id", "holder_" + tableId);
    $(rankCard.find("div.table-header-row")).attr("id", "row_" + tableId);
    $(rankCard.find("label.label-table-header")).attr("for", "txtTableHeader_" + tableId);
    $(rankCard.find("input.table-header")).attr("id", "txtTableHeader_" + tableId);
    $(rankCard.find("button.btn-add-item")).attr("onclick", "addItemToTable('" + tableId + "');");
    $(rankCard.find("button.btn-delete-table")).attr("onclick", "$(this).parents('div.table-holder').first().remove();");
    $(rankCard.find("button.btn-drag-table")).attr("onmousedown", "setTableDraggable('" + tableId + "');");
    $(rankCard.find("table")).attr("id", tableId);
    $(rankCard.find("tbody")).attr("id", "body_" + tableId);
    $(rankCard.find("tr.table-content-row")).attr("id", rowId);
    $(rankCard.find("button.btn-delete-row")).attr("onclick", "$(this).parent().parent().remove();");
    $(rankCard.find("button.btn-drag-row")).attr("onmousedown", "setRowDraggable('" + rowId + "');");
    $("#divCardContainer").append(rankCard);
    setItemArray();
}

function addTableToRankCard(cardId)
{
    let tableId = crypto.randomUUID();
    let rowId = crypto.randomUUID();
    var el = {};
    if (cardId === '')
    {
        el = $(".card-body")[0];
    }
    else
    {
        el = $("#" + cardId);
    }
    let newTable = $($(rootEl.find(".table-container")[0]).children()[0]).clone(true);
    $($(el).children().children(".table-container")[0]).append(newTable);
    $(newTable).attr("id", "holder_" + tableId);
    $(newTable.find("div.table-header-row")).attr("id", "row_" + tableId);
    $(newTable.find("label.label-table-header")).attr("for", "txtTableHeader_" + tableId);
    $(newTable.find("input.table-header")).attr("id", "txtTableHeader_" + tableId);
    $(newTable.find("button.btn-add-item")).attr("onclick", "addItemToTable('" + tableId + "');");
    $(newTable.find("button.btn-delete-table")).attr("onclick", "$(this).parents('div.table-holder').first().remove();");
    $(newTable.find("button.btn-drag-table")).attr("onmousedown", "setTableDraggable('" + tableId + "');");
    $(newTable.find("table")).attr("id", tableId);
    $(newTable.find("tbody")).attr("id", "body_" + tableId);
    $(newTable.find("tr.table-content-row")).attr("id", rowId);
    $(newTable.find("button.btn-delete-row")).attr("onclick", "$(this).parent().parent().remove();");
    $(newTable.find("button.btn-drag-row")).attr("onmousedown", "setRowDraggable('" + rowId + "');");
    setItemArray();
}

function addItemToTable(tableId)
{
    let rowId = crypto.randomUUID();
    var el = {};
    if (tableId === '')
    {
        el = $(".item-table")[0];
    }
    else
    {
        el = $("#" + tableId);
    }
    let newRow = $(rootEl.find("tr.table-content-row")[0]).clone(true);
    $($(el).children("tbody")[0]).append(newRow);
    $(newRow).attr("id", rowId);
    $(newRow.find("button.btn-delete-row")).attr("onclick", "$(this).parent().parent().remove();");
    $(newRow.find("button.btn-drag-row")).attr("onmousedown", "setRowDraggable('" + rowId + "');");
    setItemArray();
}

function collapseTable(button)
{
    if ($(button).attr("data-is-collapsed") == "true")
    {
        $(button).parents("div.table-holder").first().children().find("table", true).first().removeClass("d-none");
        $(button).attr("data-is-collapsed", "false");
        $(button).children().first().attr("class", "bi bi-arrows-collapse");
        $(button).attr("title", "Collapse this table.");
    }
    else
    {
        $(button).parents("div.table-holder").first().children().find("table", true).first().addClass("d-none");
        $(button).attr("data-is-collapsed", "true");
        $(button).children().first().attr("class", "bi bi-arrows-expand");
        $(button).attr("title", "Expand this table.");
    }
}

function collapseCard(button)
{
    if ($(button).attr("data-is-collapsed") == "true")
    {
        $(button).parents("div.card-holder").first().find("div.card div.card-body", true).first().removeClass("d-none");
        $(button).attr("data-is-collapsed", "false");
        $(button).children().first().attr("class", "bi bi-arrows-collapse");
        $(button).attr("title", "Collapse this card.");
    }
    else
    {
        $(button).parents("div.card-holder").first().find("div.card div.card-body", true).first().addClass("d-none");
        $(button).attr("data-is-collapsed", "true");
        $(button).children().first().attr("class", "bi bi-arrows-expand");
        $(button).attr("title", "Expand this card.");
    }
}

function generateTables()
{
    var finalData = [];
    let ranks = $("#divCardContainer").find("div.card").toArray();
    for (var i = 0; i < ranks.length; i++)
    {
        var thisData = {
            "Rank": $(ranks[i]).find(".rank-name").val(),
            "Monster": $("#txtMonsterName").val(),
            "Tables": []
        };
        let tables = $(ranks[i]).find(".table-holder").toArray();
        for (var i2 = 0; i2 < tables.length; i2++)
        {
            var thisTable = {
                "Header": $(tables[i2]).find(".table-header").val(),
                "Items": []
            };
            let rows = $(tables[i2]).find(".table-content-row").toArray();
            for (var i3 = 0; i3 < rows.length; i3++)
            {
                let row = $(rows[i3]);
                var itemName = $($(row.children()[1]).children()[0]).val();
                if (itemName != '')
                {
                    var item = itemArray.filter(function (item)
                    {
                        return item.ItemName == itemName;
                    })[0];
                    thisTable["Items"].push({
                        "Include": item.Include,
                        "ItemName": item.ItemName,
                        "Chance": $($(row.children()[2]).children()[0]).val(),
                        "Icon": item.IconType,
                        "IconColor": item.IconColor,
                        "Description": ((item.Description == '' || typeof(item.Description) === 'undefined') ? '[DESCRIPTION]' : item.Description),
                        "Rarity": ((item.Rarity == '' || typeof(item.Rarity) === 'undefined') ? '[RARITY]' : item.Rarity),
                        "Price": ((item.Price == '' || typeof(item.Price) === 'undefined') ? '[PRICE]' : (item.Price.toString() + "z")),
                        "Category": $($(row.children()[3]).children()[0]).val(),
                        "Quantity": $($(row.children()[4]).children()[0]).val()
                    });
                }
            }
            thisData["Tables"].push(thisTable);
        }
        finalData.push(thisData);
    }
    callGenerator(finalData);
}

function convert(num) {
    return ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"][parseInt(num)];
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

function matchTableAncestor(node, targetId)
{
    while (node.className != "table-container" && node.tagName != 'BODY')
    {
        node = node.parentNode;
    }
    return node.id == targetId;
}

var draggedTable;
function table_start(e){
    if (e.target.className == "table-holder")
    {
        draggedTable = e.target;
    }
}
function table_dragover(e){
    if (typeof(draggedTable) !== 'undefined')
    {
        if (matchTableAncestor(e.target, draggedTable.parentNode.id))
        {
            let children= Array.from(draggedTable.parentNode.children);
            if(getTableIndex(e.target)>children.indexOf(draggedTable))
                getTableNode(e.target).after(draggedTable);
            else
                getTableNode(e.target).before(draggedTable);
        }
    }
}

function getTableNode(eventTarget)
{
    while (eventTarget.className != "table-holder")
    {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget;
}

function getTableIndex(eventTarget)
{
    var tableNode;
    var parentNode = eventTarget.parentNode;
    while (parentNode.className != "table-container")
    {
        if (parentNode.className == "table-holder")
        {
            tableNode = parentNode;
        }
        parentNode = parentNode.parentNode;
    }
    return Array.from(parentNode.children).indexOf(tableNode);
}
var draggedCard;
function card_start(event){
    if (event.target.className == 'card')
    {
        console.log(event.target);
        draggedCard = event.target;
    }
}
function card_dragover(e){
    if (typeof(draggedCard) !== 'undefined')
    {
        if (isCardAncestor(e.target))
        {
            let children= Array.from(getCardHolder(e.target).parentNode.children);
            if(children.indexOf(getCardHolder(e.target))>children.indexOf(getCardHolder(draggedCard)))
                getCardHolder(e.target).after(getCardHolder(draggedCard));
            else
                getCardHolder(e.target).before(getCardHolder(draggedCard));
        }
    }
}

function getCardNode(eventTarget)
{
    while (eventTarget.className != "table-holder")
    {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget;
}

function getCardIndex(eventTarget)
{
    var tableNode;
    var parentNode = eventTarget.parentNode;
    while (parentNode.className != "table-container")
    {
        if (parentNode.className == "table-holder")
        {
            tableNode = parentNode;
        }
        parentNode = parentNode.parentNode;
    }
    return Array.from(parentNode.children).indexOf(tableNode);
}

function isCardAncestor(eventTarget)
{
    while (eventTarget.className != "card" && eventTarget.tagName != "BODY")
    {
        eventTarget = eventTarget.parentNode;
    }
    return eventTarget.className == "card";
}

function getCardHolder(node)
{
    while (!node.className.includes("card-holder") && node.tagName != "BODY")
    {
        node = node.parentNode;
    }
    if (node.className.includes("card-holder"))
    {
        return node;
    }
    else
    {
        return null;
    }
}