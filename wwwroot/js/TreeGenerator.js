var srcTreeRow = {};
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
            $("#tblTree thead").append($(WeaponTemplate.getHeader()).clone(true));
            initRow = WeaponTemplate.initRow;
            initTemplate = WeaponTemplate.initTemplate;
            generateTree = WeaponTemplate.generateTree;
            updateIcons = WeaponTemplate.updateIcons;
            addRow();
            break;
    }
    initTemplate();
}

function addRow()
{
    $('#tblTree tbody').append($(srcTreeRow).clone(true));
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