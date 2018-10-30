// Global object for respective modules
var aItemList = [];

var EstimateNS = {
    addresses: {},
    estimateItems: {},
    items: {},
    assemblies: {}
};
// End of Global objects

function getWebsiteBaseUrl() {
    return window.location.origin;
}

function openModal() {
    document.getElementById('modal').style.display = 'block';
    document.getElementById('fade').style.display = 'block';
}

function closeModal() {
    document.getElementById('modal').style.display = 'none';
    document.getElementById('fade').style.display = 'none';
}

$(document).on("click", "#searchPopup", function (e) {
    e.stopPropagation();
});

$(document).on("click", "#glbsearch", function (e) {
    e.stopPropagation();
    $("#searchPopup").show("fast");
    var ele = document.getElementById("glbsearch");
    var top = 0;
    var left = 0;
    var ele = document.getElementById("glbsearch");

    while (ele.tagName != "BODY") {
        top += ele.offsetTop;
        left += ele.offsetLeft;
        ele = ele.offsetParent;
    }
    document.getElementById("searchPopup").style.top = (top + 10) + "px";
    document.getElementById("searchPopup").style.left = (left - 310) + "px";
});

$(document).on("click", "#accountPopup", function (e) {
    e.stopPropagation();
});

$(document).on("click", "#accountDD", function (e) {
    e.stopPropagation();
    $("#accountPopup").show("fast");
    var ele = document.getElementById("accountDD");
    var top = 0;
    var left = 0;
    var ele = document.getElementById("accountDD");
    while (ele.tagName != "BODY") {
        top += ele.offsetTop;
        left += ele.offsetLeft;
        ele = ele.offsetParent;
    }
    document.getElementById("accountPopup").style.top = (top + 10) + "px";
    document.getElementById("accountPopup").style.left = "calc(100% - 207px)";
});

$(document).on("click", function () {
    $("#searchPopup, #accountPopup ").hide("fast");
});


// Product module
function productReady() {
    //--------------------------global
    var token = $('[name=__RequestVerificationToken]').val();

    //--------------------------global end

    $("#addItem").click(function () {
        $('.alert-success').hide();
        $('.alert-success').find('#dvSuccessItem').text('');
        $('#dangerItem').hide();
        $('#dangerItem').find('#dvErrorItem').html('');
        $("#item-type").populateDropdown("Select item", Product.itemTypes);
        $("#Division").populateDropdown("Select division", Product.divisions);
        $("#Category_pop").populateDropdown("Select category", Product.categories);
        $("#Manufacturer_pop").populateDropdown("Select manufacturer", Product.manufacturers);
        $("#Measure_pop").populateDropdown("Select unit of measure", Product.unitsOfMeasure);

        $("#item-type").populateDropDownList(Product.itemTypes, 0);
        $("#Category_pop").populateDropDownList(Product.categories, 0);
        $("#Manufacturer_pop").populateDropDownList(Product.manufacturers, 0);
        $("#Measure_pop").populateDropDownList(Product.unitsOfMeasure, 0);

        $("#Item_Code").val("");
        $("#Name").val("");
        $("myNotes").text("")
        $("#Price").val("");
        $("#addItem_popup").show();

        $("#madals").css({ "height": $(document).height(), "width": $(document).width() });
        $("#madals").show();
    });

    $("#addAssembly").click(function () {
        $("#assembly-form").attr("data-val", "0");
        $("#Assembly_ID").val("");
        $("#Assembly_Name").val("");
        $("#Assembly_Description").val("")
        $("#TotalCost").val("");
        $("#TotalRetailCost").val("");
        $("#LaborHour").val("");
        $("#LaborCost").text("")
        $("#proposaltxt").val("")
        $("#rightBlockrowHolder").empty();
        //$("#chkIsItemAssembly").prop("checked", false);
        $("#Assembly_UOM").populateDropdown("Select unit of measure", Product.unitsOfMeasure);

        //$('#grid-itemassembly').dataTable().fnDestroy();
        aItemList.length = 0
        $('#grid-itemassembly').dataTable().fnClearTable();

        populateAllItems(Product.items);

        $("#items").populateDropDownList(Product.items, 0);
        $("#Assembly_UOM").populateDropDownList(Product.unitsOfMeasure, 0);

        //var aaData = Product.items;
        //debugger;
        //var req = [];
        //for (var i = 0; i < aaData.length; i++) {
        //    var item = aaData[i];
        //    var it = {
        //        label: item.ItemCode + "[" + item.Name + "]",
        //        value: item.ItemCode + "[" + item.Name + "]",
        //        id: item.Id,
        //        name: item.Name,
        //        itemID: item.Id,
        //        itemCode: item.ItemCode,
        //        price: item.Price.toFixed(2).toLocaleString('en-US')
        //    };
        //    req.push(it);
        //}

        //$('#items').autocomplete({
        //    source: req,
        //    select: function (event, ui) {
        //        var item = {
        //            ID: 0,
        //            ItemID: ui.item.itemID,
        //            Name: ui.item.name,
        //            ItemCode: ui.item.itemCode,
        //            Price: ui.item.price.toFixed(2).toLocaleString('en-US'),
        //            PercentageOfItem: 0,
        //            Value: 0,
        //            TaxPercent: 0,
        //            Cost: 0,
        //            CostPercent: 0,
        //            RetailCost: 0
        //        }

        //        var itemExists = false;
        //        for (i = 0; i < aItemList.length; i++) {
        //            var it = aItemList[i]
        //            if (it.ItemID == ui.item.id) {
        //                itemExists = true;
        //            }
        //        }
        //        if (!itemExists) {
        //            aItemList.push(item);
        //            var oTableData = transformItem(aItemList);
        //            createItemAssemblyTable(oTableData);
        //        }
        //    }
        //});

        //$('#items').on("blur click", function () { $(this).val("") });



        $("#addAssembly_popup").show();

        $("#madals").css({ "height": $(document).height(), "width": $(document).width() });
        $("#madals").show();
        $('.alert-danger').hide();
        $('.alert-danger').find('#dvSuccessAssembly').text('');
        $('.alert-success').hide();
        $('.alert-success').find('#dvErrorAssembly').text('');
        //$('#update-assembly').hide();
    });

    $("#Cancel_Pop").click(function () {
        $("#madals").hide();
        $("#addItem_popup").hide();
        //window.location.href = '../Product/Index';
    });
    $("#Cancel_PopAss").click(function () {
        $("#madals").hide();
        $("#addAssembly_popup").hide();
    });

    var data = {
        "inventory": [{ "Item Code": "FHR-1001", "Name": "Coil Nails", "Category": "Category 1", "Manufacturer": "Manufacturer1", "Price": "$100" },
          { "Item Code": "FHR-1002", "Name": "Wire", "Category": "Category 1", "Manufacturer": "Manufacturer1", "Price": "$300" },
          { "Item Code": "FHR-1004S", "Name": "Frames", "Category": "Category 1", "Manufacturer": "Manufacturer1", "Price": "$700" }]
    };
    var b = 0;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Product/Products',
        type: 'POST',
        data: { __RequestVerificationToken: token },
        success: function (data) {
            b = data.items.length;
            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];
                $("<div id='inventoryRow_" + b + "'  class='inventoryRow' data-val = " + item['Id'] + "><div class='inventoryColumn1'>" + item['ItemCode'] + "</div><div class='inventoryColumn2'>" + item['Name'] + "</div><div class='inventoryColumn3'>" + item['Category'] + "</div><div class='inventoryColumn4'>" + item['Manufacturer'] + "</div><div class='inventoryColumn5'><span id='myval'>" + item['Price'] + "</span><input type='text' class='inpTextInv' /></div><div class='inventoryColumn6'><a href='#' id='edit_" + b + "' class='edit_class inventoryColumn_linkBtn' onclick='editItem(this.id)'>Edit</a><a href='#' id='save_" + b + "' class='save_class inventoryColumn_linkBtn' onclick='save_class(this.id)'>Save</a><a href='#' id='cancel_" + b + "' class='cancel_class inventoryColumn_linkBtn' onclick='cancel_class(this.id)'>Cancel</a></div><div class='inventoryColumn7'><a id='delete_" + b + "' href='#' class='delete_class inventoryColumn_linkBtn' onclick='deleteItem(this.id)'></a></div></div>").appendTo($("#gridHolder"));
                b++;
            }

            for (var i = 0; i < data.assemblies.length; i++) {
                var main = data.assemblies[i];
                $("<div class='inventoryRow' data-val='" + main['ID'] + "'>" +
                    "<div class='inventoryColumn1'>" + main['Code'] + "</div>" +
                    "<div class='inventoryColumn2'>" + main['Name'] + "</div>" +
                    "<div class='inventoryColumn3' style='width: calc(46% - 11px)'>" + main['Description'] + "</div>" +
                    "<div class='inventoryColumn6'><a href='#' class='edit_class inventoryColumn_linkBtn'>Edit</a></div>" +
                    "<div class='inventoryColumn7'><a id='delete_3' class='delete_class inventoryColumn_linkBtn' onclick='deleteAssembly(this.id)' href='#'></a></div>" +
                    "</div>").appendTo($("#gridHolder2"));
            }

            Product = data;
            closeModal();
        }
    });


    $("#addItemSave").click(function () {
        var item = new Object();
        item.Id = $("#itemForm").attr("data-val");
        item.ItemTypeId = parseInt($("#item-type option:selected").val());
        item.ItemCode = $("#Item_Code").val();
        item.Name = $("#Name").val();
        item.Description = $("#myNotes").val();
        item.Division = $("#Division").find('ul li .selected').attr("data-id");
        item.CategoryId = parseInt($("#Category_pop option:selected").val());
        item.ManufacturerId = parseInt($("#Manufacturer_pop option:selected").val());
        item.UnitOfMeasureId = parseInt($("#Measure_pop option:selected").val());
        item.Price = $("#Price").val();
        item.IsInventoryItem = $('#chkIsInventoryItem').prop('checked') == true;

        if (item.ItemTypeId == 0) {
            $('#dangerItem').show();
            $('#dangerItem').find('#dvErrorItem').html('Please select item type.');
            return false;
        }
        if (item.ItemCode == "") {
            $('#dangerItem').show();
            $('#dangerItem').find('#dvErrorItem').html('Please enter item code.');
            return false;
        }
        if (item.Name == "") {
            $('#dangerItem').show();
            $('#dangerItem').find('#dvErrorItem').html('Please enter item name.');
            return false;
        }



        //var Category = $("#Category_pop").find('ul li .selected').text();
        //var Manufacturer = $("#Manufacturer_pop").find('ul li .selected').text();
        //var UnitOfMeasure = $("#Measure_pop").find('ul li .selected').text();

        var isNew = "n";
        if (item.id == "0" || item.id == "")
            isNew = "y"
        openModal();
        $.ajax({
            url: getWebsiteBaseUrl() + 'Product/SaveItem',
            type: 'POST',
            datatype: "json",
            data: { "item": JSON.stringify(item), "__RequestVerificationToken": token }
        }).done(function (data) {
            $("#madals").hide();
            $("#addItem_popup").hide();
            closeModal();
            window.location.href = getWebsiteBaseUrl() + '/Product/Index';
        }); 
    });

    $("#update-item").click(function () {

        $('.alert-success').hide();
        $('.alert-success').find('#dvSuccessItem').text('');

        var item = new Object();
        item.Id = $("#itemForm").attr("data-val");
        item.ItemTypeId = parseInt($("#item-type option:selected").val());
        item.ItemCode = $("#Item_Code").val();
        item.Name = $("#Name").val();
        item.Description = $("#myNotes").val()
        item.Division = $("#Division").find('ul li .selected').attr("data-id");
        item.CategoryId = parseInt($("#Category_pop option:selected").val());
        item.ManufacturerId = parseInt($("#Manufacturer_pop option:selected").val());
        item.UnitOfMeasureId = parseInt($("#Measure_pop option:selected").val());
        item.Price = $("#Price").val();
        item.IsInventoryItem = $('#chkIsInventoryItem').prop('checked') == true;

        if (item.ItemTypeId == 0) {
            $('#dangerItem').show();
            $('#dangerItem').find('#dvErrorItem').html('Please select item type.');
            return false;
        }
        if (item.ItemCode == "") {
            $('#dangerItem').show();
            $('#dangerItem').find('#dvErrorItem').html('Please enter item code.');
            return false;
        }
        if (item.Name == "") {
            $('#dangerItem').show();
            $('#dangerItem').find('#dvErrorItem').html('Please enter item name.');
            return false;
        }



        //var Category = $("#Category_pop").find('ul li .selected').text();
        //var Manufacturer = $("#Manufacturer_pop").find('ul li .selected').text();
        //var UnitOfMeasure = $("#Measure_pop").find('ul li .selected').text();

        var isNew = "n";
        if (item.id == "0" || item.id == "")
            isNew = "y"
        openModal();
        $.ajax({
            url: getWebsiteBaseUrl() + 'Product/SaveItem',
            type: 'POST',
            datatype: "json",
            data: { "item": JSON.stringify(item), "__RequestVerificationToken": token }
        }).done(function (data) {
            closeModal();
            $('.alert-success').show();
            $('.alert-success').find('#dvSuccessItem').text(data.message);
            $("#itemForm").attr("data-val", data.id);
            $('#grid-item').dataTable().fnDestroy();
            $('#grid-item').dataTable({
                "order": [[0, "asc"]],
                "sAjaxSource": "../Product/GetItems",
                "iDisplayLength": 100,
                "bSort": true,
                "aoColumns": [{ "sType": "html" },
                    { "sType": "html" },
                    { "sType": "html" },
                    { "sType": "html" },
                    { "sType": "html" },
                    { "sType": "html" },
                    { "sType": "html" },
                    { "sType": "html" }],
                "fnServerData": function (sSource, aoData, fnCallback) {
                    aoData.push({ "name": "ID", "value": 5 });
                    $.getJSON(sSource, aoData, function (json) {
                        keyValue = json.keyValue;
                        fnCallback(json);
                    });
                }
            });
            
            
        });
    });

    $('#gridHolder').enscroll({
        showOnHover: true,
        verticalTrackClass: 'track3',
        verticalHandleClass: 'handle3'
    });
    $('#gridHolder2').enscroll({
        showOnHover: true,
        verticalTrackClass: 'track3',
        verticalHandleClass: 'handle3'
    });
    $('#leftBlockrowHolder').enscroll({
        showOnHover: true,
        verticalTrackClass: 'track3',
        verticalHandleClass: 'handle3'
    });
    $('#rightBlockrowHolder').enscroll({
        showOnHover: true,
        verticalTrackClass: 'track3',
        verticalHandleClass: 'handle3'
    });

    $('#_tab02').hide();

    var openedTab = "_tab01";
    var openerBtn = "tab01";
    $(".tabHolder a").click(function () {
        var tabId = this.id;
        $(this).removeClass("selected");
        $("#" + openerBtn).addClass("selected");
        $("#" + openedTab).css("display", "none");
        $("#_" + tabId).css("display", "block");
        openedTab = "_" + tabId;
        openerBtn = tabId;
        if (openedTab == "_tab01") {
            $("#addItem").show();
            $("#addAssembly").hide();

        } else if (openedTab == "_tab02") {
            $("#addItem").hide();
            $("#addAssembly").show();

        }
    });

    $.fn.populateDropdown = function (defaultText, data) {
        $(this).find(".dropdown").empty();
        $(this).find('span').text(defaultText);

        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            var a = $("<a>").attr({ "href": "#", "data-id": item.ID }).text(item.Description);
            var li = $('<li>').append(a);
            $(this).find(".dropdown").append(li);
        }

        $(this).abcDropDownRefresh();
    }

    $.fn.selectDropdownValue = function (id) {
        var text = $(this).find('ul li [data-id=' + id + ']').addClass("selected").text();
        $(this).find('span').text(text);
    }


    $(document).on("click", "#grid-assembly .edit-assembly", function () {
        var id = $(this).attr("data-val");

        var token = $('[name=__RequestVerificationToken]').val();
        openModal();
        $.ajax({
            url: getWebsiteBaseUrl() + 'Product/GetAssembly',
            type: 'POST',
            data: { __RequestVerificationToken: token, id: id },
            success: function (data) {
                $("#item-type").populateDropdown("Select item type", Product.itemTypes);
                $("#Division").populateDropdown("Select division", Product.divisions);
                $("#Category_pop").populateDropdown("Select category", Product.categories);
                $("#Manufacturer_pop").populateDropdown("Select manufacturer", Product.manufacturers);
                $("#Measure_pop").populateDropdown("Select unit of measure", Product.unitsOfMeasure);
                $("#Assembly_UOM").populateDropdown("Select unit of measure", Product.unitsOfMeasure);

                $("#assembly-form").attr("data-val", id);
                $("#Assembly_ID").val(data.assembly.Code);
                $("#Assembly_Name").val(data.assembly.Name);
                $("#Assembly_UOM").populateDropDownList(Product.unitsOfMeasure, data.assembly.UnitOfMeasureId);
                $("#Assembly_Description").val(data.assembly.Description)
                $("#TotalCost").val(data.assembly.TotalCost);
                $("#TotalRetailCost").val(data.assembly.TotalRetailCost);
                $("#LaborHour").val(data.assembly.LaborHour);
                $("#LaborCost").val(data.assembly.LaborCost);
                $("#proposaltxt").val(data.assembly.ProposalText);
                //$("#chkIsItemAssembly").prop("checked", data.assembly.IsItemAssembly);

                var aaData = data.assembly.AvailableAssemblyItems;
                var item1 = data.assembly.AssemblyItems;
                $('#items').populateDropDownList(aaData, 0);

                //var req = [];
                //for (var i = 0; i < aaData.length; i++) {
                //    var item = aaData[i];
                //    var it = {
                //        label: item.ItemCode + "[" + item.Name + "]",
                //        value: item.ItemCode + "[" + item.Name + "]",
                //        id: item.ItemID,
                //        name: item.Name,
                //        itemID: item.ItemID,
                //        itemCode: item.ItemCode,
                //        price: item.Price.toFixed(2).toLocaleString('en-US')
                //    };
                //    req.push(it);
                //}
                aItemList = data.assembly.AssemblyItems;

                var oTableData = transformItem(aItemList);

                createItemAssemblyTable(oTableData);

                //$('#items').autocomplete({
                //    source: req,
                //    select: function (event, ui) {
                //        var item = {
                //            ID: 0,
                //            ItemID: ui.item.itemID,
                //            Name: ui.item.name,
                //            ItemCode: ui.item.itemCode,
                //            Price: ui.item.price.toFixed(2).toLocaleString('en-US'),
                //            PercentageOfItem: 0,
                //            Value: 0,
                //            TaxPercent: 0,
                //            Cost: 0,
                //            CostPercent: 0,
                //            RetailCost: 0
                //        }

                //        var itemExists = false;
                //        for (i = 0; i < aItemList.length; i++) {
                //            var it = aItemList[i]
                //            if (it.ItemID == ui.item.id) {
                //                itemExists = true;
                //            }
                //        }
                //        if (!itemExists) {
                //            aItemList.push(item);
                //            var oTableData = transformItem(aItemList);
                //            createItemAssemblyTable(oTableData);
                //        }
                //    }
                //});

                //$('#items').on("blur click", function () { $(this).val("") })

                $("#addAssembly_popup").show();
                $('#addAssembly_popup').css(
                    { "top": (window.pageYOffset + 20) + 'px' });
                $("#madals").css({ "height": $(document).height(), "width": $(document).width() });
                $("#madals").show();
                $('.alert-danger').hide();
                $('.alert-danger').find('#dvSuccessAssembly').text('');
                $('.alert-success').hide();
                $('.alert-success').find('#dvErrorAssembly').text('');
                //$('#update-assembly').show();
                closeModal();
            }
        });
    });

    $(document).on("click", "#assembly-items #AddAssemblyItem", function () {
        AddAssemblyItemClick();

        $("#editedItem").focus(function () { $(this).select(); });
        $("#editedItem").focus();
    });
    function AddAssemblyItemClick() {
        var itemval = $('#items option:selected').val();
        if (itemval == "0") {
            $('.alert-danger').show();
            $('.alert-danger').find('#dvErrorAssembly').text('Please select item.');
            return;
        }
        var itemname = $('#items option:selected').text();
        var itemid = new String(itemval).split(',')[0];
        var itemcode = new String(itemval).split(',')[1];
        var itemprice = new String(itemval).split(',')[2];
        var item = {
            ID: 0,
            ItemID: itemid,
            Name: itemname,
            ItemCode: itemcode,
            Price: Number(itemprice).toFixed(2).toLocaleString('en-US'),
            PercentageOfItem: 0,
            Value: 0,
            TaxPercent: 0,
            Cost: 0,
            CostPercent: 0,
            RetailCost: 0,
            IsEdited: 1
        }

        var itemExists = false;
        for (i = 0; i < aItemList.length; i++) {
            var it = aItemList[i];
            if (it.ItemID == itemid) {
                itemExists = true;
            }
            else
            {
                it.IsEdited = 0;
            }
        }
        if (!itemExists) {
            //if (aItemList.length >= 1 && $('#chkIsItemAssembly').prop('checked') == true)
            //{
            //    BootstrapDialog.alert('Multiple items are not alowed. Only material item can be added.');
            //    return;
            //}
            //else
            //{
            //    if ($('#chkIsItemAssembly').prop('checked') == true) {
            //        if (item.Name.indexOf('Labor') > 0) {
            //            BootstrapDialog.alert('Multiple items are not alowed. Only material item can be added.');
            //            return;
            //        }
            //    }
            //}
            aItemList.push(item);
            var oTableData = transformItem(aItemList);
            createItemAssemblyTable(oTableData);
        }
    }

    function transformItem(aItemList) {
        return aItemList.map(function (item) {
            var idAttr = "";
            if (item['IsEdited'] != null && item['IsEdited'] != undefined && item['IsEdited'] == 1)
                idAttr = "id='editedItem' ";

            return [
               "<span data-val = " + item['ItemID'] + " class='itemCode'>" + item['ItemCode'] + "</span>",
               "<span data-val = " + item['ID'] + " class='name'>" + item['Name'] + "</span>",
               //"<span class='price'>" + itemType + "</span>",
               "<span class='price'>" + item.Price + "</span>",
               "<input " + idAttr + "type='number' class='perc1 qty' maxlength='3' min='0' max='100' value = " + item.PercentageOfItem + ">",
               "<span class='val'>" + item.Value + "</span>",
               "<input type='number' class='perc2 qty' maxlength='3' min='0' max='100' value = " + item.TaxPercent + ">",
               "<span class='cost'>" + item.Cost + "</span>",
               "<input type='number' class='perc3 qty' maxlength='3' min='0' max='100' value = " + item.CostPercent + ">",
               "<span class='retail-cost'>" + item.RetailCost + "</span>",
               //"<span class='delete-cust-addr update' data-val=" + item['ItemID'] + "><ul><li class='edit'><a href='#non'>Update</a></li></ul></span>",
               "<span class='delete-cust-addr delete' data-val=" + item['ItemID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
            ];
        });
    }

    function getItemType(itemID) {
        var itemTypeID = 0;

        var items = keyValue.items;
        for (var i = 0; i < items.length; i++) {
            if (items[i].ID == itemID) {
                itemTypeID = items[i].ItemTypeId;
                break;
            }
        }

        if (parseInt(itemTypeID) > 0) {
            var itemTypes = keyValue.itemTypes;
            for (var i = 0; i < itemTypes.length; i++) {
                if (itemTypes[i].ID == itemTypeID) {
                    return itemTypes[i];
                }
            }
        }
    }

    $(document).on("change", "#grid-itemassembly .perc1", function () {
        var tr = $(this).parents("tr")[0];

        var price = parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, ''));
        var perc1 = $(this).val();

        var perc2 = parseFloat($(tr).find(".perc2").val().replace(/[^0-9-.]/g, ''));
        var perc3 = parseFloat($(tr).find(".perc3").val().replace(/[^0-9-.]/g, ''));

        var value = (price * perc1) / 100;;
        var cost = value + (value * perc2) / 100;;
        var retailCost = (cost / (1 - perc3 / 100));

        $(tr).find(".val").html(value.toFixed(2));

        $(tr).find(".cost").html(cost.toFixed(2));

        $(tr).find(".retail-cost").html(retailCost.toFixed(2));
        GridItemAssemblyUpdate(tr);
    });

    $(document).on("change", "#grid-itemassembly .perc2", function () {
        var tr = $(this).parents("tr")[0];

        var price = parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, ''));
        var perc2 = $(this).val();

        var perc1 = parseFloat($(tr).find(".perc1").val().replace(/[^0-9-.]/g, ''));
        var perc3 = parseFloat($(tr).find(".perc3").val().replace(/[^0-9-.]/g, ''));

        var value = (price * perc1) / 100;;
        var cost = value + (value * perc2) / 100;;
        var retailCost = (cost / (1 - perc3 / 100));

        $(tr).find(".val").html(value.toFixed(2));

        $(tr).find(".cost").html(cost.toFixed(2));

        $(tr).find(".retail-cost").html(retailCost.toFixed(2));
        GridItemAssemblyUpdate(tr);
    });

    $(document).on("change", "#grid-itemassembly .perc3", function () {
        var tr = $(this).parents("tr")[0];

        var price = parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, ''));
        var perc3 = $(this).val();

        var perc2 = parseFloat($(tr).find(".perc2").val().replace(/[^0-9-.]/g, ''));
        var perc1 = parseFloat($(tr).find(".perc1").val().replace(/[^0-9-.]/g, ''));

        var value = (price * perc1) / 100;;
        var cost = value + (value * perc2) / 100;;
        var retailCost = (cost / (1 - perc3 / 100));

        $(tr).find(".val").html(value.toFixed(2));

        $(tr).find(".cost").html(cost.toFixed(2));

        $(tr).find(".retail-cost").html(retailCost.toFixed(2));
        GridItemAssemblyUpdate(tr);
    });

    $(document).on("click", "#grid-itemassembly .update", function () {
        var tr = $(this).parent().parent();
        GridItemAssemblyUpdate(tr);
    });

    function GridItemAssemblyUpdate(tr)
    {
        var itemToUpdate = {
            ID: $(tr).find(".name").attr("data-val"),
            ItemID: $(tr).find(".itemCode").attr("data-val"),
            ItemCode: $(tr).find(".itemCode").html(),
            Name: $(tr).find(".name").html(),
            Price: parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, '')),
            PercentageOfItem: parseFloat($(tr).find(".perc1").val().replace(/[^0-9-.]/g, '')),
            Value: parseFloat($(tr).find(".val").html().replace(/[^0-9-.]/g, '')),
            TaxPercent: parseFloat($(tr).find(".perc2").val().replace(/[^0-9-.]/g, '')),
            Cost: parseFloat($(tr).find(".cost").html().replace(/[^0-9-.]/g, '')),
            CostPercent: parseFloat($(tr).find(".perc3").val().replace(/[^0-9-.]/g, '')),
            RetailCost: parseFloat($(tr).find(".retail-cost").html().replace(/[^0-9-.]/g, ''))
        }
        for (i = 0; i < aItemList.length; i++) {
            var it = aItemList[i]
            if (it.ItemID == itemToUpdate.ItemID) {
                aItemList[i] = itemToUpdate;
            }
        }

        var retailCost = 0;
        var totalCost = 0;
        for (i = 0; i < aItemList.length; i++) {
            var it = aItemList[i]
            retailCost = retailCost + parseFloat(it.RetailCost);
            totalCost = totalCost + parseFloat(it.Cost);
        }

        $("#addAssembly_popup #TotalRetailCost").val(retailCost.toFixed(2));
        $("#addAssembly_popup #TotalCost").val(totalCost.toFixed(2));
    }

    $(document).on("click", "#grid-itemassembly .delete", function () {
        var tr = $(this).parent().parent();

        var ItemID = $(tr).find(".itemCode").attr("data-val");
        aItemList = removeItemFromList(aItemList, ItemID)

        var retailCost = 0;
        var totalCost = 0;
        for (i = 0; i < aItemList.length; i++) {
            var it = aItemList[i]
            retailCost = retailCost + parseFloat(it.RetailCost);
            totalCost = totalCost + parseFloat(it.Cost);
        }

        $("#addAssembly_popup #TotalCost").val(totalCost.toFixed(2));
        $("#addAssembly_popup #TotalRetailCost").val(retailCost.toFixed(2));

        var oTableData = transformItem(aItemList);

        createItemAssemblyTable(oTableData);
    });

    function removeItemFromList(items, ItemID) {
        var tmp = [];
        for (var i = 0; i < items.length; i++) {
            var item = items[i];

            if (item.ItemID != ItemID) {
                tmp.push(item)

            }
        }
        return tmp;
    }

    function createItemAssemblyTable(aItemData) {
        $('#grid-itemassembly').dataTable().fnDestroy();

        oTable = $('#grid-itemassembly').dataTable({
            "bPaginate": true,
            "iDisplayLength": 10,
            "bLengthChange": false,
            "bFilter": false,
            "bInfo": false,
            "bSort": true,
            "bAutoWidth": false,
            "aaData": aItemData,
            "columnDefs": [
                { "width": "10%", "targets": 0 },
                { "width": "30%", "targets": 1 }
            ]
        });
    }

    //-----

}


function edit_class(id) {
    var myId = id;
    var actVal = $("#" + myId).parent().prev().find("span").text();
    $("#" + myId).parent().prev().find("span").css("display", "none");
    $("#" + myId).parent().prev().find("input").css("display", "block");
    $("#" + myId).parent().prev().find("input").val(actVal);
    $("#" + myId).next().show();
    $("#" + myId).next().next().show();
    $("#" + myId).hide();
};

$(document).on("click", "#grid-item .edit-item", function () {
    $('.alert-success').hide();
    $('.alert-success').find('#dvSuccessItem').text('');
    $('#dangerItem').hide();
    $('#dangerItem').find('#dvErrorItem').html('');
    var id = new String($(this).attr("data-val")).split(',')[0];
    var token = $('[name=__RequestVerificationToken]').val();

    //var row = $("#" + id).parents(".inventoryRow").first();
    //var rowId = "#" + row.attr("id");
    //var itemCode = $(rowId + " .inventoryColumn1 a").text();
    //var name = $(rowId + " .inventoryColumn2 a").text();
    //var category = $(rowId + " .inventoryColumn3 a").text();
    //var id = $(rowId).attr("data-val");
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Product/GetItem',
        type: 'POST',
        data: { __RequestVerificationToken: token, id: id },
        success: function (data) {
            //$("#item-type").populateDropdown("Select item type", Product.itemTypes);
            //$("#Division").populateDropdown("Select division", Product.divisions);
            //$("#Category_pop").populateDropdown("Select category", Product.categories);
            //$("#Manufacturer_pop").populateDropdown("Select manufacturer", Product.manufacturers);
            //$("#Measure_pop").populateDropdown("Select unit of measure", Product.unitsOfMeasure);

            $("#itemForm").attr("data-val", id);
            $("#Item_Code").val(data.item.ItemCode);
            $("#Name").val(data.item.Name);
            $("#myNotes").text(data.item.Description);

            $("#item-type").populateDropDownList(Product.itemTypes, data.item.ItemTypeId);
            $("#Category_pop").populateDropDownList(Product.categories, data.item.CategoryId);
            $("#Manufacturer_pop").populateDropDownList(Product.manufacturers, data.item.ManufacturerId);
            $("#Measure_pop").populateDropDownList(Product.unitsOfMeasure, data.item.UnitOfMeasureId);

            $("#Price").val(data.item.Price);
            $("#chkIsInventoryItem").prop("checked", data.item.IsInventoryItem);

            $("#addItem_popup").show();

            $("#madals").css({ "height": $(document).height(), "width": $(document).width() });
            $("#madals").show();
            //$("#update-item").show();
            closeModal();
        }
    });
});

$(document).on("click", "#grid-item .delete-item", function () {
    var id = new String($(this).attr("data-val")).split(',')[0];
    var token = $('[name=__RequestVerificationToken]').val();
    //var row = $("#" + id).parents(".inventoryRow").first();
    //var rowId = "#" + row.attr("id");
    //var id = $(rowId).attr("data-val");
    //var itemId = id;
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Product/DeleteItem',
                type: 'POST',
                data: { __RequestVerificationToken: token, itemId: id },
                success: function (data) {
                    closeModal();
                    window.location.href = getWebsiteBaseUrl() + 'Product/Index';
                }
            });
        } else {
            //alert('Nope.');
        }
    });

});


function save_class(id) {
    var myId = id;
    var actVal = $("#" + myId).parent().prev().find("input").val();
    $("#" + myId).parent().prev().find("input").css("display", "none");
    $("#" + myId).parent().prev().find("span").css("display", "block");
    $("#" + myId).parent().prev().find("span").text(actVal);
    $("#" + myId).next().hide();
    $("#" + myId).prev().show();
    $("#" + myId).hide();
};

function cancel_class(id) {
    var myId = id;
    var actVal = $("#" + myId).parent().prev().find("input").val();
    $("#" + myId).parent().prev().find("input").css("display", "none");
    $("#" + myId).parent().prev().find("span").css("display", "block");
    $("#" + myId).prev().prev().show();
    $("#" + myId).prev().hide();
    $("#" + myId).hide();
};
function delete_class(id) {
    var myId = id;
    var indexIng = $("#" + myId).parent().parent().attr("data-index")
    var actVal = $("#" + myId).parent().prev().find("input").val();
    $("#" + myId).parent().parent().remove();
};


var d = 0;
function plus_class(id) {
    var myId = id;
    var parenNode = $("#" + myId).parent().parent();
    var mainId = $("#" + myId).parent().parent().attr("data-ItemID");
    //var getId = mainId.split("_");
    var showId = d;
    var indexIng = $("#" + myId).parent().prev().prev().prev().prev().text();
    var indexIng2 = $("#" + myId).parent().prev().prev().prev().text();
    $("<div data-ItemID='" + mainId + "'  class='row'><div class='assColumn1'>" + indexIng + "</div><div class='assColumn2'>" + indexIng2 + "</div><div class='assColumn3'><div class='myspin'></div></div><div class='assColumn4'><a id='minus_" + showId + "' href='#' class='minus_class' onclick='minus_class(this.id)'></a></div></div>").appendTo($("#rightBlockrowHolder"));
    $("#rightBlockrowHolder [data-ItemID=" + mainId + "] .myspin").abcSpinner();
    d++;
    $("#leftBlockrowHolder  [data-ItemID=" + mainId + "]").hide();

};



function minus_class(id) {
    var myId = id;
    var mainId = $("#" + myId).parent().parent().attr("data-ItemID");

    $("#leftBlockrowHolder  [data-ItemID=" + mainId + "]").show();
    $("#" + myId).parent().parent().remove();

};

function editAssemblies(id) {
    //var row = $("#" + id).parents(".inventoryRow").first();

}


// Product page
function save_Assemblies(id) {
    var assemblyItems = new Array();
    //$("#rightBlockrowHolder .row").each(function (index) {
    //    var assemblyItem = new Object();
    //    assemblyItem.ItemID = $(this).attr("data-ID");
    //    assemblyItem.ItemID = $(this).attr("data-ItemID");
    //    assemblyItem.ItemCode = $(this).find(".assColumn1").text();
    //    assemblyItem.Quantity = $(this).find(".assColumn3 .myspin").attr("data-val");
    //    assemblyItems.push(assemblyItem);
    //});
    assemblyItems = aItemList;
    var token = $('[name=__RequestVerificationToken]').val();
    var assembly = new Object();
    assembly.Id = parseInt($("#assembly-form").attr("data-val"));
    assembly.Code = $("#Assembly_ID").val();
    assembly.Name = $("#Assembly_Name").val();
    assembly.UnitOfMeasureId = $("#Assembly_UOM").val();
    assembly.Description = $("#Assembly_Description").val();
    assembly.TotalCost = $("#TotalCost").val();
    assembly.TotalRetailCost = $("#TotalRetailCost").val();
    assembly.LaborHour = $("#LaborHour").val();
    assembly.LaborCost = $("#LaborCost").val();
    assembly.ProposalText = $("#proposaltxt").val();
    assembly.AssemblyItems = assemblyItems;
    //assembly.IsItemAssembly = $("#chkIsItemAssembly").prop("checked");
    assembly.IsItemAssembly = false;

    if (assembly.Code == 0) {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter assembly code.');
        return false;
    }
    if (assembly.Name == "") {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter assembly name.');
        return false;
    }
    if (assembly.TotalCost == "") {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter assembly total cost.');
        return false;
    }
    if (assembly.TotalRetailCost == "") {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter total retail cost.');
        return false;
    }

    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Product/SaveAssembly',
        type: 'POST',
        datatype: "json",
        data: { "assembly": JSON.stringify(assembly), "__RequestVerificationToken": token }
    }).done(function (data) {
        //$("#_tab02 #gridHolder2 [data-val=" + data.id + "]").remove();

        //$("<div class='inventoryRow' data-val=" + data.id + "><div class='inventoryColumn1'>" + assembly.Code + "</div>" +
        //    "<div class='inventoryColumn2'>" + assembly.Name + "</div>" +
        //    "<div class='inventoryColumn3' style='width: calc(46% - 11px)'>" + assembly.Description + "</div>" +
        //    "<div class='inventoryColumn6'><a href='#' id='edit_3' class='edit_class inventoryColumn_linkBtn' onclick='editAssemblies(this.id)'>Edit</a></div>" +
        //    "<div class='inventoryColumn7'> <a id='delete_3' class='delete_class inventoryColumn_linkBtn' onclick='deleteAssembly(this.id)' href='#'></a></div>" +
        //    "</div>").appendTo($("#gridHolder2"));
        $("#addAssembly_popup").hide();
        $("#madals").hide();
        $("#assembly-form").attr("data-val", data.id);
        closeModal();
        //window.location.href = getWebsiteBaseUrl() + 'Product/Index';
        $("#addItem").hide();
        $("#addAssembly").show();
        $('#grid-assembly').dataTable().fnDestroy();
        $('#grid-assembly').dataTable({
            "order": [[0, "asc"]],
            "iDisplayLength": 100,
            "sAjaxSource": getWebsiteBaseUrl() + "Product/GetAssemblies",
            "bSort": true,
            "aoColumns": [
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" }
            ],
            //"aLengthMenu": [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "ID", "value": 5 });
                $.getJSON(sSource, aoData, function (json) {
                    keyValue = json.keyValue;
                    fnCallback(json);
                });
            }
        });
        
    });
}

function save_AssembliesWithoutClose(id) {
    var assemblyItems = new Array();
    //$("#rightBlockrowHolder .row").each(function (index) {
    //    var assemblyItem = new Object();
    //    assemblyItem.ItemID = $(this).attr("data-ID");
    //    assemblyItem.ItemID = $(this).attr("data-ItemID");
    //    assemblyItem.ItemCode = $(this).find(".assColumn1").text();
    //    assemblyItem.Quantity = $(this).find(".assColumn3 .myspin").attr("data-val");
    //    assemblyItems.push(assemblyItem);
    //});
    assemblyItems = aItemList;
    var token = $('[name=__RequestVerificationToken]').val();
    var assembly = new Object();
    assembly.Id = parseInt($("#assembly-form").attr("data-val"));
    assembly.Code = $("#Assembly_ID").val();
    assembly.Name = $("#Assembly_Name").val();
    assembly.UnitOfMeasureId = $("#Assembly_UOM").val();
    assembly.Description = $("#Assembly_Description").val();
    assembly.TotalCost = $("#TotalCost").val();
    assembly.TotalRetailCost = $("#TotalRetailCost").val();
    assembly.LaborHour = $("#LaborHour").val();
    assembly.LaborCost = $("#LaborCost").val();
    assembly.ProposalText = $("#proposaltxt").val();
    assembly.AssemblyItems = assemblyItems;
    //assembly.IsItemAssembly = $("#chkIsItemAssembly").prop("checked");
    assembly.IsItemAssembly = false;

    if (assembly.Code == 0) {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter assembly code.');
        return false;
    }
    if (assembly.Name == "") {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter assembly name.');
        return false;
    }
    if (assembly.TotalCost == "") {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter assembly total cost.');
        return false;
    }
    if (assembly.TotalRetailCost == "") {
        $('#dangerAssembly').show();
        $('#dangerAssembly').find('#dvErrorAssembly').html('Please enter total retail cost.');
        return false;
    }

    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Product/SaveAssembly',
        type: 'POST',
        datatype: "json",
        data: { "assembly": JSON.stringify(assembly), "__RequestVerificationToken": token }
    }).done(function (data) {
        //$("#_tab02 #gridHolder2 [data-val=" + data.id + "]").remove();

        //$("<div class='inventoryRow' data-val=" + data.id + "><div class='inventoryColumn1'>" + assembly.Code + "</div>" +
        //    "<div class='inventoryColumn2'>" + assembly.Name + "</div>" +
        //    "<div class='inventoryColumn3' style='width: calc(46% - 11px)'>" + assembly.Description + "</div>" +
        //    "<div class='inventoryColumn6'><a href='#' id='edit_3' class='edit_class inventoryColumn_linkBtn' onclick='editAssemblies(this.id)'>Edit</a></div>" +
        //    "<div class='inventoryColumn7'> <a id='delete_3' class='delete_class inventoryColumn_linkBtn' onclick='deleteAssembly(this.id)' href='#'></a></div>" +
        //    "</div>").appendTo($("#gridHolder2"));
        //$("#addAssembly_popup").hide();
        //$("#madals").hide();
        closeModal();
        $("#assembly-form").attr("data-val", data.id);
        $('.alert-success').show();
        $('.alert-success').find('#dvSuccessAssembly').text(data.message);


        $("#addItem").hide();
        $("#addAssembly").show();
        $('#grid-assembly').dataTable().fnDestroy();
        $('#grid-assembly').dataTable({
            "order": [[0, "asc"]],
            "iDisplayLength": 100,
            "sAjaxSource": getWebsiteBaseUrl() + "Product/GetAssemblies",
            "bSort": true,
            "aoColumns": [
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" },
                { "sType": "html" }
            ],
            //"aLengthMenu": [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
            "fnServerData": function (sSource, aoData, fnCallback) {
                aoData.push({ "name": "ID", "value": 5 });
                $.getJSON(sSource, aoData, function (json) {
                    keyValue = json.keyValue;
                    fnCallback(json);
                });
            }
        });

    });
}

$(document).on("click", "#grid-assembly .delete-assembly", function () {
    var id = $(this).attr("data-val");
    var token = $('[name=__RequestVerificationToken]').val();
    //var assemblyId = $("#_tab02 #" + id).parent().parent().attr("data-val");
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Product/DeleteAssembly',
                type: 'POST',
                data: { __RequestVerificationToken: token, assemblyId: id },
                success: function (data) {
                    //delete_class(id)
                    $('#grid-assembly').dataTable().fnDestroy();
                    $('#grid-assembly').dataTable({
                        "order": [[0, "asc"]],
                        "iDisplayLength": 100,
                        "sAjaxSource": getWebsiteBaseUrl() + "Product/GetAssemblies",
                        "bSort": true,
                        "aoColumns": [
                            { "sType": "html" },
                            { "sType": "html" },
                            { "sType": "html" },
                            { "sType": "html" },
                            { "sType": "html" }
                        ],
                        //"aLengthMenu": [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
                        "fnServerData": function (sSource, aoData, fnCallback) {
                            aoData.push({ "name": "ID", "value": 5 });
                            $.getJSON(sSource, aoData, function (json) {
                                keyValue = json.keyValue;
                                fnCallback(json);
                            });
                        }
                    });
                    closeModal();
                    //window.location.href = getWebsiteBaseUrl() + 'Product/Index';
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});

//$(document).on("change", "#addAssembly_popup #chkIsItemAssembly", function () {
//    var chk = $(this).prop("checked");
//    var token = $('[name=__RequestVerificationToken]').val();
//    if (chk == true) {
//        if ($('#grid-itemassembly tbody tr').length >= 1) {
//            if ($('#grid-itemassembly tbody tr').length == 1 && $('#grid-itemassembly tbody tr td').length > 1)
//            {
//                var tableRow = $('#grid-itemassembly').find("td").filter(function () {
//                    return $(this).text().indexOf("Labor") > 0;
//                }).closest("tr");
//                if (tableRow.length >= 1) {
//                    BootstrapDialog.alert('Multiple items are not alowed. Only material item can be added.');
//                    $('#chkIsItemAssembly').prop('checked', false);
//                }
//            }
//            else if ($('#grid-itemassembly tbody tr').length > 1 && $('#grid-itemassembly tbody tr td').length > 1)
//            {
//                BootstrapDialog.alert('Multiple items are not alowed. Only material item can be added.');
//                $('#chkIsItemAssembly').prop('checked', false);
//            }
//        }
//    }
//});

function populateAvailableItems(items) {
    $("#leftBlockrowHolder").empty();
    var c = 0;
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        $("<div data-ItemID='" + item['ItemID'] + "'  class='row'><div class='assColumn1'>" + item['ItemCode'] +
            "</div><div class='assColumn2'>" + item['Name'] + "</div><div class='assColumn3'>" + item['Category'] +
            "</div><div class='assColumn4'>" + item['Manufacturer'] + "</div><div class='assColumn5'><a id='plus_" + c + "' href='#' class='plus_class' onclick='plus_class(this.id)'></a></div></div>").appendTo($("#leftBlockrowHolder"));
        c++;
    }
}

function populateAllItems(items) {
    $("#leftBlockrowHolder").empty();
    var c = 0;
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        $("<div data-ItemID='" + item['Id'] + "'  class='row'><div class='assColumn1'>" + item['ItemCode'] +
            "</div><div class='assColumn2'>" + item['Name'] + "</div><div class='assColumn3'>" + item['Category'] +
            "</div><div class='assColumn4'>" + item['Manufacturer'] + "</div><div class='assColumn5'><a id='plus_" + c + "' href='#' class='plus_class' onclick='plus_class(this.id)'></a></div></div>").appendTo($("#leftBlockrowHolder"));
        c++;
    }
}

function populateSelectedItems(items) {
    $("#rightBlockrowHolder").empty();
    var d = items.length;

    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        $("<div  data-ID=" + item['ID'] + " data-ItemID='" + item['ItemID'] + "'  class='row'><div class='assColumn1'>" + item['ItemCode'] +
            "</div><div class='assColumn2'>" + item['Name'] + "</div><div class='assColumn3'><div class='myspin' data-val='" + item['Quantity'] + "'></div>" +
            "</div><div class='assColumn4'><a id='minus" + i + "' href='#' class='minus_class' onclick='minus_class(this.id)'></a></div></div>").appendTo($("#rightBlockrowHolder"));
        $("#rightBlockrowHolder [data-ItemID=" + item['ItemID'] + "] .myspin").abcSpinner();
        d++;
    }
}

function hideSelectedItemsinAvailableItems(selItems, availItems) {
    for (var i = 0; i < selItems.length; i++) {
        var selItem = selItems[i];
        var itemCode = selItem["ItemCode"];
        for (var j = 0; j < availItems.length; j++) {
            var availItem = availItems[j];
            if (availItem["ItemCode"] == itemCode) {
                $("#leftBlockrowHolder  [data-ItemID=" + availItem["ItemID"] + "]").hide();
            }
        }
    }
}
// end of product module

///Common funtions

$.fn.populateDropDownList = function (data, selectedId) {
    $(this).empty();
    var index;
    $(this).append($('<option/>').prop("selected", true).text("--Select--").attr('value', 0));
    for (index in data) {
        var item = data[index];
        if (item.ID == selectedId)
            $(this).append($('<option/>').prop({ "selected": "selected" }).text(item.Description).attr('value', item.ID));
        else
            $(this).append($('<option/>').text(item.Description).attr('value', item.ID));
    }
}
$.fn.populateDropDownListWithoutSelect = function (data, selectedId) {
    $(this).empty();
    var index;
    for (index in data) {
        var item = data[index];
        if (item.ID == selectedId)
            $(this).append($('<option>').prop({ "selected": "selected" }).text(item.Description).attr('value', item.ID));
        else
            $(this).append($('<option>').text(item.Description).attr('value', item.ID));
    }
}

$(document).on("click", ".left_Nav .btnHolders .linkBtn", function () {
    $(".linkBtnAdd_Selected").removeClass("linkBtnAdd_Selected");
    $(this).addClass("linkBtnAdd_Selected");
});


jQuery.fn.center = function () {
    //this.css("position","fixed");
    this.css("top", ($(window).height() / 2) - (this.outerHeight() / 2));
    this.css("left", ($(window).width() / 2) - (this.outerWidth() / 2));
    return this;
}


// Method that checks that the browser supports the HTML5 File API
function browserSupportFileUpload() {
    var isCompatible = false;
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        isCompatible = true;
    }
    return isCompatible;
}

function createObjList(ProspectModel) {
    return function (index, element) {
        if ($.type(element) === "array") { $.each(element, createObjList); }
        if ($.type(element) === "object") {
            var ProspectData = {};
            ProspectData.LastName = element["Last Name"];
            ProspectData.FirstName = element["First Name"];
            ProspectData.Address1 = element["Address Line 1"];
            ProspectData.City = element["City"];
            ProspectData.State = element["State"];
            ProspectData.ZipCode = element["Zip/Postal Code"];

            ProspectModel.push(ProspectData);
        }
    }
}


//Method that reads and processes the selected file
function upload(evt) {
    debugger;
    if (!browserSupportFileUpload()) {
        alert('The File APIs are not fully supported in this browser!');
    } else {
        var data = null;
        var file = evt.target.files[0];
        var reader = new FileReader();
        reader.readAsText(file);
        reader.onload = function (event) {

            var csvData = event.target.result;
            data = $.csv.toObjects(csvData);
            var ProspectModel = new Array();
            $.each(data, createObjList(ProspectModel));

            var token = $('[name=__RequestVerificationToken]').val();
            var prospectModel = {};
            prospectModel.ProspectData = ProspectModel;
            $("#progress-mesg").html(" - Importing prospect...");
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Prospect/ImportProspect',
                type: 'POST',
                data: { "prospectModel": JSON.stringify(prospectModel), "__RequestVerificationToken": token },
                success: function (data) {
                    closeModal();
                    if (data && data.length > 0) {
                        window.location.href = getWebsiteBaseUrl() + 'Prospect/Index';
                    } else {
                        window.location.href = getWebsiteBaseUrl() + 'Prospect/Index';
                    }
                    $("#progress-mesg").html(" - Imported prospects successfully!");
                    alert('Imported prospects successfully!');
                }
            });
        };
        reader.onerror = function () {
            alert('Unable to read ' + file.fileName);
        };
    }
}

var updateProspectCSV = function (event) {
    debugger;
    var csvData = event.target.result;
    data = $.csv.toObjects(csvData);
    var objectList = new Array();
    $.each(data, createObjList(objectList));

    var token = $('[name=__RequestVerificationToken]').val();
    var prospectModel = {};
    prospectModel.ProspectData = objectList;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Prospect/ImportProspect',
        type: 'POST',
        data: { "prospectModel": JSON.stringify(prospectModel), "__RequestVerificationToken": token },
        success: function (data) {
            closeModal();
            if (data && data.length > 0) {
                alert('Imported -' + data.length + '- rows successfully!');
                window.location.href = getWebsiteBaseUrl() + 'Prospect/Index';
            } else {
                alert('No data to import!');
            }
        }
    });
};


// This section has javascrip code for Estimate functionality

var assembliesList = new Array();

function populateEstimateForm(data) {
    $("#status").populateDropDownList(data.keyValue.status, data.estimate.Status);

    if (data.keyValue.estimateForLead == true) {
        $("#rdbLead").prop("checked", true);
        $("#customerDdlDiv").hide();
        $("#leadDdlDiv").show();
        $("#lead").populateDropDownList(data.keyValue.leads, data.estimate.LeadID);
        $("#customer").populateDropDownList(data.keyValue.customers, 0);
    } else {
        $("#rdbCustomer").prop("checked", true);
        $("#leadDdlDiv").hide();
        $("#customerDdlDiv").show();
        $("#customer").populateDropDownList(data.keyValue.customers, data.estimate.LeadID);
        $("#lead").populateDropDownList(data.keyValue.leads, 0);
    }

    $('#custname').text($("#customer option:selected").text());
    $("#items").populateDropDownList(data.keyValue.items, 0);
    $("#assemblyCategories").populateDropDownList(data.keyValue.assemblyCategories, 0);
    assembliesList = data.keyValue.assemblies;
    $("#assemblies").populateDropDownList(assembliesList, 0);
    $("#roof-type").populateDropDownList(data.keyValue.roofTypes, data.estimate.RoofType);

    var addresses = $.grep(data.keyValue.address, function (v) { return data.estimate.LeadID == v.CustomerID });
    $("#address").populateDropDownList(addresses, data.estimate.JobAddressID);

    $("#id").val(data.estimate.ID);
    $("#estimateID").val(data.estimate.ID);
    var createdDate = new Date(parseInt(data.estimate.CreatedDate.substr(6)));
    $("#createdDate").val(createdDate);
    $("#name").val(data.estimate.Name);
    $("#totalCost").val(data.estimate.TotalCost);
    $("#totalCostDisplay").val(data.estimate.TotalCost.toFixed(2).toLocaleString('en-US'));

    $("#totalMaterialCost").val(data.estimate.TotalMaterialCost);
    $("#totalMaterialCostDisplay").val(data.estimate.TotalMaterialCost.toFixed(2).toLocaleString('en-US'));
    $("#totalLabourCost").val(data.estimate.TotalLaborCost);
    $("#totalLabourCostDisplay").val(data.estimate.TotalLaborCost.toFixed(2).toLocaleString('en-US'));

    $("#priceAdj").val(data.estimate.PriceAdj);
    $("#priceAdjDisplay").val(data.estimate.PriceAdj.toFixed(2).toLocaleString('en-US'));
    $("#reasonForAdj").val(data.estimate.ReasonForAdj);
    $("#contractPrice").val(data.estimate.ContractPrice);
    $("#contractPriceDisplay").val(data.estimate.ContractPrice.toFixed(2).toLocaleString('en-US'));

    var estimateStatus = $("#status option:selected").val();
    if(estimateStatus == 6)// Completed = 6
    {
        $("#status").prop('disabled', true);
    }
}

function emptyEstimateForm(data) {
    $("#status").populateDropDownList(data.keyValue.status, 0);
    $("#customer").populateDropDownList(data.keyValue.customers, 0);
    $("#lead").populateDropDownList(data.keyValue.leads, 0);
    $("#address").populateDropDownList([], 0);
    $("#items").populateDropDownList(data.keyValue.items, 0);
    $("#assemblyCategories").populateDropDownList(data.keyValue.assemblyCategories, 0);
    assembliesList = data.keyValue.assemblies;
    $("#assemblies").populateDropDownList(assembliesList, 0);
    $("#roof-type").populateDropDownList(data.keyValue.roofTypes, 0);

    $("#id").val(0);
    $("#name").val("");
    $("#totalCost").val("0.00");
    $("#priceAdj").val("0.00");
    $("#reasonForAdj").val("");
    $("#contractPrice").val("0.00");

    $("#totalCostDisplay").val("0.00");
    $("#priceAdjDisplay").val("0.00");
    $("#contractPriceDisplay").val("0.00");
}

function transformEstimateItems(estimateItems) {
    return estimateItems.map(function (item) {
        var itemAssembly = {};
        //check if item or assembly to be added 
        var unit = "";
        if (item.ItemAssemblyType == 0) {
            itemAssembly = searchObject(item.ItemAssemblyID, EstimateNS.items);
        }
        else {
            itemAssembly = searchObject(item.ItemAssemblyID, EstimateNS.assemblies);
        }
        unit = itemAssembly.Unit;

        var code = itemAssembly.Code;
        var name = itemAssembly.Name;
        var description = itemAssembly.Description;
        var price = item.Price;
        if (item.ID == 0)
        {
            price = itemAssembly.Price;
        }
        var materialCost = item.MaterialCost;
        if (item.ID == 0) {
            materialCost = itemAssembly.MaterialCost;
        }
        var laborCost = item.LaborCost;
        if (item.ID == 0) {
            laborCost = itemAssembly.LaborCost;
        }

        return [
           "<span data-val = " + item.ItemAssemblyID + " data-item-type = " + item.ItemAssemblyType + " class='name'>" + code + "</span>",
           "<span>" + name + "</span>",
           "<span class='uom'>" + unit + "</span>",
           "<span class='materialcost'>" + Number(materialCost.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='laborcost'>" + Number(laborCost.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='price'>" + Number(price.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<input type='text' style='width:100px' maxlength='8' onkeypress='enterOnlyNumericForQty(this, event)' onkeydown='enterOnlyNumericOnKeyDownForEstimate(this, event)' class='qty' value = " + ((item.Quantity == 0) ? '' : item.Quantity) + ">",
           "<span class='cost'>" + Number(item.Cost.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           //"<span class='delete-cust-addr update' data-val=" + item['ID'] + ">Update</span>",
           "<span class='delete-cust-addr delete' data-val=" + item['ID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}

function createEstimateItemTable(aEstimateItems) {
    $('#grid-items').dataTable().fnDestroy();

    $('#grid-items').dataTable({
        "bPaginate": false,
        "iDisplayLength": 5,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 1, 2, 8] },
            { className: "dt-right", targets: [3, 4, 5, 6, 7] }
        ],
        "bAutoWidth": false,
        "aaData": aEstimateItems
    });
}

function populateEstimateViewModel() {
    //var d = new Date(year, month, day, hours, minutes, seconds, milliseconds);
    var date = new Date();

    if ($("#createdDate").val() == "") {

    } else {
        var d = new Date($("#createdDate").val());
        var curr_date = d.getDate();
        var curr_month = d.getMonth();
        var curr_year = d.getFullYear();

        var date = new Date(curr_year, curr_month, curr_date);
    }

    var leadOrCustomerID = 0;
    var value = $("input[name$='rdbLeadCustomer']").val();
    if (value == 1) {
        leadOrCustomerID = parseInt($("#lead option:selected").val());
    }
    else {
        leadOrCustomerID = parseInt($("#customer option:selected").val());
    }

    var estimate = {
        ID: $("#id").val(),
        Name: $("#name").val(),
        CreatedDate: date,
        TotalCost: $("#totalCost").val(),
        PriceAdj: $("#priceAdj").val(),
        ReasonForAdj: $("#reasonForAdj").val(),
        ContractPrice: $("#contractPrice").val(),
        Status: parseInt($("#status option:selected").val()),
        LeadID: leadOrCustomerID,
        JobAddressID: parseInt($("#address option:selected").val()),
        TotalMaterialCost: $("#totalMaterialCost").val(),
        TotalLaborCost: $("#totalLabourCost").val(),
        RoofType: parseInt($("#roof-type option:selected").val())
    }
    return estimate;
}

function getEstimate(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Estimate/GetEstimate',
        type: 'POST',
        data: { id: ID },
        success: function (data) {
            EstimateNS.estimateForLead = data.keyValue.estimateForLead;
            EstimateNS.leads = data.keyValue.leads;
            EstimateNS.customers = data.keyValue.customers;
            EstimateNS.addresses = data.keyValue.address;
            EstimateNS.items = data.keyValue.items;
            EstimateNS.assemblies = data.keyValue.assemblies;
            EstimateNS.roofTypes = data.keyValue.roofTypes;

            if (ID == 0) {
                $('#estimate').find('.ui-tabs-nav').find('li').last().hide();
                emptyEstimateForm(data);
                EstimateNS.estimateItems = [];
            } else {
                populateEstimateForm(data);
                EstimateNS.estimateItems = data.estimateItems;
                var aEstimateItems = transformEstimateItems(EstimateNS.estimateItems);
                createEstimateItemTable(aEstimateItems);
                $('#update-estimate').show();
                
            }
            closeModal();
        }
    });
}

$(document).on("click", "#grid-items .update", function () {
    var tr = $(this).parent().parent();

    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        ItemType: $(tr).find(".name").attr("data-val"),
        Price: parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, '')),
        Quantity: parseFloat($(tr).find(".qty").val()),
        Cost: parseFloat($(tr).find(".cost").html().replace(/[^0-9-.]/g, '')),
    }

    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType) {
            EstimateNS.estimateItems[i] = itemToUpdate;
        }
    }

    var totalCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    $("#estimate-form #totalCost").val(totalCost.toFixed(2));
    $("#estimate-form #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

});

$(document).on("click", "#grid-items .delete", function () {
    var tr = $(this).parent().parent();

    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type")
    }

    var tmp = [];
    for (var i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i];

        if (!(it.ItemAssemblyID == itemToUpdate.ItemAssemblyID
                && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType)) {
            tmp.push(it)
        }
    }

    EstimateNS.estimateItems = tmp;

    var totalCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    $("#estimate-form #totalCost").val(totalCost.toFixed(2));
    $("#estimate-form #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    // Calculate Total MaterialCost
    var totalMatCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalMatCost = totalMatCost + (parseFloat(it.MaterialCost) * parseFloat(it.Quantity));
    }

    // Update Total Cost 
    $("#estimate-form #totalMaterialCost").val(totalMatCost.toFixed(2));
    $("#estimate-form #totalMaterialCostDisplay").val(Number(totalMatCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    // Calculate Total Laborcost
    var totalLabCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalLabCost = totalLabCost + (parseFloat(it.LaborCost) * parseFloat(it.Quantity));
    }

    // Update Total Cost 
    $("#estimate-form #totalLabourCost").val(totalLabCost.toFixed(2));
    $("#estimate-form #totalLabourCostDisplay").val(Number(totalLabCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    // Update Contract Price
    var priceAdj = parseFloat($("#priceAdj").val());
    var contractPrice = totalCost + priceAdj;

    $("#contractPrice").val(contractPrice.toFixed(2));
    $("#contractPriceDisplay").val(Number(contractPrice.toFixed(2)).toFixed(2).toLocaleString('en-US'));


    var aEstimateItems = transformEstimateItems(EstimateNS.estimateItems);
    createEstimateItemTable(aEstimateItems);
});

$(document).on("blur", "#grid-items .qty", function () {
    var tr = $(this).parents("tr")[0];

    // Update row level Cost
    if ($(tr).find(".qty").val() == "") {
        $(tr).find(".qty").val("0");
    }
    var qty = parseFloat($(tr).find(".qty").val());
    var price = parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, ''));
    var materialcost = parseFloat($(tr).find(".materialcost").html().replace(/[^0-9-.]/g, ''));
    var laborcost = parseFloat($(tr).find(".laborcost").html().replace(/[^0-9-.]/g, ''));
    var cost = qty * price;
    //var matCost = qty * materialcost;
    //var labCost = qty * laborcost;
    $(tr).find(".cost").html(cost.toFixed(2));
    $(tr).find(".cost").html(Number(cost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    
    //Update the in-memory object ItemAssembly and Total Cost and Contract Price
    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        ItemType: $(tr).find(".name").attr("data-val"),
        Price: parseFloat($(tr).find(".price").html().replace(/[^0-9-.]/g, '')),
        Quantity: parseFloat($(tr).find(".qty").val().replace(/[^0-9-.]/g, '')),
        Cost: parseFloat($(tr).find(".cost").html().replace(/[^0-9-.]/g, '')),
        MaterialCost: parseFloat(materialcost),
        LaborCost: parseFloat(laborcost),
        Unit: $(tr).find(".uom").html()
    }

    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType) {
            EstimateNS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#estimate-form #totalCost").val(totalCost.toFixed(2));
    $("#estimate-form #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    // Calculate Total MaterialCost
    var totalMatCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalMatCost = totalMatCost + (parseFloat(it.MaterialCost) * parseFloat(it.Quantity));
    }

    // Update Total Cost 
    $("#estimate-form #totalMaterialCost").val(totalMatCost.toFixed(2));
    $("#estimate-form #totalMaterialCostDisplay").val(Number(totalMatCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    // Calculate Total Laborcost
    var totalLabCost = 0;
    for (i = 0; i < EstimateNS.estimateItems.length; i++) {
        var it = EstimateNS.estimateItems[i]
        totalLabCost = totalLabCost + (parseFloat(it.LaborCost) * parseFloat(it.Quantity));
    }

    itemToUpdate.LaborCost = laborcost;
    itemToUpdate.MaterialCost = materialcost;

    // Update Total Cost 
    $("#estimate-form #totalLabourCost").val(totalLabCost.toFixed(2));
    $("#estimate-form #totalLabourCostDisplay").val(Number(totalLabCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    // Update Contract Price
    var priceAdj = parseFloat($("#priceAdj").val());
    var contractPrice = totalCost + priceAdj;

    //var rowindex = $(this).closest('tr').index();
    //if (rowindex == $('#estimate-form tr:last').index()) {
    //    $("#estimate-form #totalMaterialCostDisplay").focus();
    //}
    $("#contractPrice").val(contractPrice.toFixed(2));
    $("#contractPriceDisplay").val(Number(contractPrice.toFixed(2)).toFixed(2).toLocaleString('en-US'));

});

$(document).on("blur", "#form-estimate #priceAdjDisplay", function () {
    var totalCost = parseFloat($("#totalCost").val());
    var priceAdj = parseFloat($("#priceAdjDisplay").val());
    var contractPrice = totalCost + priceAdj;
    $('#priceAdj').val(priceAdj);
    $("#contractPrice").val(contractPrice);
    $("#contractPriceDisplay").val(contractPrice.toFixed(2).toLocaleString('en-US'));
});


$(document).on("click", "#form-estimate #add-item", function () {
    // Get selected AssemblyItem object
    var itemSelectedID = parseInt($("#items option:selected").val())
    // Create estimate item object
    EstimateItem = {
        ID: 0,
        EstimateID: 0,
        ItemAssemblyID: itemSelectedID,
        ItemAssemblyType: 0,
        Price: 0.00,
        Quantity: 0,
        Cost: 0.00,
        MaterialCost: 0.00,
        LaborCost: 0.00,
        Unit: ''
    };

    // Add estimate item to the data source
    EstimateNS.estimateItems.push(EstimateItem);

    // Refresh table with data source
    var aEstimateItems = transformEstimateItems(EstimateNS.estimateItems);
    createEstimateItemTable(aEstimateItems);
});

$(document).on("click", "#form-estimate #add-assembly", function () {
    // Get selected AssemblyItem object
    var itemSelectedID = parseInt($("#assemblies option:selected").val());
    var token = $('[name=__RequestVerificationToken]').val();
    var UOM = '';
    $.ajax({
        async: false,
        url: getWebsiteBaseUrl() + 'Estimate/GetAssemblyUOM',
        type: 'POST',
        data: { __RequestVerificationToken: token, assemblyID : itemSelectedID },
        success: function (data) {
            UOM = data.UOMs;
        }
    });
    // Create estimate item object
    EstimateItem = {
        ID: 0,
        EstimateID: 0,
        ItemAssemblyID: itemSelectedID,
        ItemAssemblyType: 1,
        Price: 0.00,
        Quantity: 0,
        Cost: 0.00,
        MaterialCost: 0.00,
        LaborCost: 0.00,
        Unit: UOM
    };

    // Add estimate item to the data source
    EstimateNS.estimateItems.push(EstimateItem);

    // Refresh table with data source
    var aEstimateItems = transformEstimateItems(EstimateNS.estimateItems);
    createEstimateItemTable(aEstimateItems);
});

//Utility function to serach object based on the ID
function searchObject(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].ID == nameKey) {
            return myArray[i];
        }
    }
    return {};
}


BootstrapDialog.confirm = function (message, callback) {
    new BootstrapDialog({
        title: 'Confirmation',
        message: message,
        closable: false,
        data: {
            'callback': callback
        },
        buttons: [{
            label: 'Cancel',
            action: function (dialog) {
                typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(false);
                dialog.close();
            }
        }, {
            label: 'OK',
            cssClass: 'btn-primary',
            action: function (dialog) {
                typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(true);
                dialog.close();
            }
        }]
    }).open();
};

function enterOnlyNumeric(obj, event) {
    var code = event.keyCode || event.which;
    if (code == 8 || code == 9 || code == 37 || code == 39)
        return true;
    if (code < 46
    || code > 59) {
        event.preventDefault();
        return false;
    } // prevent if not number/dot

    if (code == 46
    && $(obj).val().indexOf('.') != -1) {
        event.preventDefault();
        return false;
    } // prevent if already dot
}

function enterOnlyNumericForQty(obj, event) {
    var selectionLength = $(obj)[0].selectionEnd - $(obj)[0].selectionStart;

    var code = event.keyCode || event.which;
    if (code == 8 || code == 9 || code == 37 || code == 39)
        return true;

    if (code < 46
    || code > 59) {
        event.preventDefault();
        return false;
    } // prevent if not number/dot

    if (code == 46
    && $(obj).val().indexOf('.') != -1) {
        event.preventDefault();
        return false;
    } // prevent if already dot

    if ($(obj).val().length == selectionLength) {
        $(obj).val('');
        return true;
    }

    var txt = $(obj).val().split('.');
    if (txt.length > 0) {
        if (txt[1].length > 1) {
            event.preventDefault();
            return false;
        }
    }
}

function enterOnlyNumericOnKeyDown(obj, event)
{
    if (event.keyCode == 9)
    {
        var rowindex = $(obj).closest('tr').index();
        if (rowindex == $('#estimate-form tr:last').index()) {
            $("#estimate-form #totalMaterialCostDisplay").focus();
            event.preventDefault();
        }
        else {
            $(obj).closest('tr').next('tr').find('.qty').focus();
            event.preventDefault();
        }
    }
    // Allow only backspace and delete
    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 9) {
        // let it happen, don't do anything
    }
    else {
        if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))
        {
        }
        // Ensure that it is a number and stop the keypress
        else if (event.keyCode < 48 || event.keyCode > 57) {
            event.preventDefault();
        }
    }
}

function enterOnlyNumericOnKeyDownForEstimate(obj, event) {
    if (event.keyCode == 9) {
        var rowindex = $(obj).closest('tr').index();
        if (rowindex == $('#estimate-form tr:last').index()) {
            $("#estimate-form #totalMaterialCostDisplay").focus();
            event.preventDefault();
        }
        else {
            $(obj).closest('tr').next('tr').find('.qty').focus();
            event.preventDefault();
        }
    }
}

function sortByProperty(property) {
    'use strict';
    return function (a, b) {
        var sortStatus = 0;
        if (a[property] < b[property]) {
            sortStatus = -1;
        } else if (a[property] > b[property]) {
            sortStatus = 1;
        }

        return sortStatus;
    };
}

$.ajaxSetup({
    statusCode: {
        200: function (jqXHR, textStatus, errorThrown) {
            if (errorThrown.getResponseHeader("AJAX_EXECUTION_ERROR") != null) {
                window.location.href = getWebsiteBaseUrl() + 'Error/General';
            }
            else if (errorThrown.getResponseHeader("AJAX_SESSION_TIMEOUT") != null) {
                window.location.href = getWebsiteBaseUrl() + 'Account/Login';
            }
        },
        401: function (jqXHR, textStatus, errorThrown) {
            window.location.href = getWebsiteBaseUrl() + 'Account/Login';
        },
        500: function (jqXHR, textStatus, errorThrown) {
            alert('error 500 = ' + errorThrown);
            window.location.href = getWebsiteBaseUrl() + 'Account/Login';
        }
    }
});

function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}

var states = {
    "AL": 1,
    "AK": 2,
    "AZ": 3,
    "AR": 4,
    "CA": 5,
    "CO": 6,
    "CT": 7,
    "DE": 8,
    "DC": 9,
    "FM": 10,
    "FL": 11,
    "GA": 12,
    "GU": 13,
    "HI": 14,
    "ID": 15,
    "IL": 16,
    "IN": 17,
    "IA": 18,
    "KS": 19,
    "KY": 20,
    "LA": 21,
    "ME": 22,
    "MH": 23,
    "MD": 24,
    "MA": 25,
    "MI": 26,
    "MN": 27,
    "MS": 28,
    "MO": 29,
    "MT": 30,
    "NE": 31,
    "NV": 32,
    "NH": 33,
    "NJ": 34,
    "NM": 35,
    "NY": 36,
    "NC": 37,
    "ND": 38,
    "MP": 39,
    "OH": 40,
    "OK": 41,
    "OR": 42,
    "PW": 43,
    "PA": 44,
    "PR": 45,
    "RI": 46,
    "SC": 47,
    "SD": 48,
    "TN": 49,
    "TX": 50,
    "UT": 51,
    "VT": 52,
    "VI": 53,
    "VA": 54,
    "WA": 55,
    "WV": 56,
    "WI": 57,
    "WY": 58
};