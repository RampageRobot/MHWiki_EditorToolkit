var mdlGenerate;
var rootEl = {};
var rootItemEl = {};
$(window).on("load", function () {
    $(".item-table").attr("id", crypto.randomUUID());
    rootEl = $($(".card")[0]).parent().clone(true);
    var rowEl = $($("#tblGlobalItems tbody tr")[0]);
    rootItemEl = rowEl.clone(true);
    var rowId = crypto.randomUUID();
    $(rowEl).attr('id', rowId);
    $(rowEl).find(".btn-delete-global-row").attr("onclick", "deleteGlobalRow('" + rowId + "');");
    $('.item-name-input').each(function () { $(this).select2(); });
});
function copyToClipboard()
{
    var copyText = document.getElementById("txtResults");
        copyText.select();
        copyText.setSelectionRange(0, 99999);
        navigator.clipboard.writeText(copyText.value);
        alert("Copied!");
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
    $(rankCard).find(".item-name-input").each(function () {
        var oldVal = $(this).val();
        $(this).find("option").remove();
        $(this).append(dropdowns[$("#ddlGameSelect").val()]);
        $(this).val(oldVal);
        $(this).select2();
    });
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
    $(newTable).find(".item-name-input").each(function () {
        var oldVal = $(this).val();
        $(this).find("option").remove();
        $(this).append(dropdowns[$("#ddlGameSelect").val()]);
        $(this).val(oldVal);
        $(this).select2();
    });
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
    $(newRow).find(".item-name-input").each(function () {
        var oldVal = $(this).val();
        $(this).find("option").remove();
        $(this).append(dropdowns[$("#ddlGameSelect").val()]);
        $(this).val(oldVal);
        $(this).select2();
    });
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
    callGenerator(getFinalData());
}

function getSaveData() {
    return {
        "game": $("#ddlGameSelect").val(),
        "monster": $("#txtMonsterName").val(),
        "data": getFinalData()
    };
    var finalData = getFinalData();
}

function pageLoadData(loadedData) {
    if (typeof (loadedData.game) !== 'undefined') {
        $("#ddlGameSelect").val(loadedData.game);
        $("#ddlGameSelect").trigger("change");
    }
    $("#txtMonsterName").val(loadedData.monster);
    for (var i = 0; i < loadedData.data.length; i++) {
        var rankObj = loadedData.data[i];
        if ($("#divCardContainer .card-holder").length < (i + 1)) {
            addRankCard();
        }
        var rankCard = $("#divCardContainer .card-holder").last();
        rankCard.find("input.rank-name").val(rankObj.Rank);
        for (var i2 = 0; i2 < rankObj.Tables.length; i2++) {
            var tableObj = rankObj.Tables[i2];
            if (rankCard.find(".table-container .table-holder").length < (i2 + 1)) {
                addTableToRankCard(rankCard.find("div.card-body").attr("id"));
            }
            var table = rankCard.find(".table-container .table-holder").last();
            table.find("input.table-header").val(tableObj.Header);
            for (var i3 = 0; i3 < tableObj.Items.length; i3++) {
                var itemObj = tableObj.Items[i3];
                if (table.find(".table-content table tbody tr").length < (i3 + 1)) {
                    addItemToTable(table.find(".table-content table").attr("id"));
                }
                var row = table.find(".table-content table tbody tr").last();
                var nameSelect = row.find(".item-name-input").first();
                if (typeof (itemObj.ItemId) === 'undefined') {
                    itemObj.ItemId = itemObj.ItemName;
                }
                nameSelect.val(itemObj.ItemId);
                if (nameSelect.val() != itemObj.ItemId) {
                    var matches = nameSelect.find("option").filter(function () { return $(this).html() == itemObj.ItemId; });
                    if (matches.length > 0) {
                        nameSelect.val(matches.first().attr("value"));
                    }
                }
                if (row.find(".item-name-input option").first().html())
                row.find('.item-name-input').trigger("change");
                var chanceCol = row.find(".item-chance-input");
                chanceCol.val(itemObj.Chance);
                validateInput(chanceCol[0]);
                row.find(".item-category-input").val(itemObj.Category);
                var quantityCol = row.find(".item-quantity-input");
                quantityCol.val(itemObj.Quantity);
                validateInput(quantityCol[0]);
            }
        }
    }
}

function updateDropdowns() {
    $(".item-name-input").each(function () {
        var oldVal = $(this).val();
        $(this).find("option").remove();
        $(this).append(dropdowns[$("#ddlGameSelect").val()]);
        $(this).val(oldVal);
    });
}

function getFinalData() {
    var finalData = [];
    let ranks = $("#divCardContainer").find("div.card").toArray();
    for (var i = 0; i < ranks.length; i++) {
        var thisData = {
            "Rank": $(ranks[i]).find(".rank-name").val(),
            "Monster": $("#txtMonsterName").val(),
            "Tables": []
        };
        let tables = $(ranks[i]).find(".table-holder").toArray();
        for (var i2 = 0; i2 < tables.length; i2++) {
            var thisTable = {
                "Header": $(tables[i2]).find(".table-header").val(),
                "Items": []
            };
            let rows = $(tables[i2]).find(".table-content-row").toArray();
            for (var i3 = 0; i3 < rows.length; i3++) {
                let row = $(rows[i3]);
                var itemName = $($(row.children()[1]).children()[0]).val();
                if (itemName != '') {
                    thisTable["Items"].push({
                        "Include": true,
                        "ItemName": '',
                        "ItemId": itemName,
                        "Chance": $($(row.children()[2]).children()[0]).val(),
                        "Icon": '',
                        "IconColor": '',
                        "Description": '',
                        "Rarity": '',
                        "Price": '',
                        "Category": $($(row.children()[3]).children()[0]).val(),
                        "Quantity": $($(row.children()[4]).children()[0]).val()
                    });
                }
            }
            thisData["Tables"].push(thisTable);
        }
        finalData.push(thisData);
    }
    return finalData;
}

function convert(num) {
    return ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"][parseInt(num)];
}