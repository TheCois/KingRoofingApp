var JobPONS = {
    originalEstimateItems: [],
    estimateItems: [],
    items: {},
    assemblies: {},
    PO: {},
    vendors: {}
};// End of Global objects
var COValues;
$(document).ready(function () {
    $("#save-PO").on("click", saveJobPO);
    $("#add-PO").on("click", addJobPO);
    $("#cancel-PO").on("click", cancelJobPO);
    $("#poestimates").on("change", POEstimateChange);
    $("#cos").on("change", COEstimateChange);
    $("#vendors").on("change", VendorChange);
});
function GetJobPOList(ID) {
    openModal();
    cancelJobPO();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobPOList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.POs = data.keyValue.POs;
            JobPONS.vendors = data.keyValue.vendors;
            if (ID == 0) {
                emptyJobPOForm(data);
            } else {
                populateJobPOList();
                emptyJobPOForm(data);
            }
            closeModal();
        }
    });
}
function VendorChange()
{
    var vendorID = $('#vendors option:selected').val();
    GetVendorInformation(vendorID);
}
function POEstimateChange() {
    var estimateId = $('#poestimates option:selected').val();
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetEstimateItems',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ estimateID: estimateId }),
        success: function (data) {
            JobPONS.items = data.keyValue.items;
            JobPONS.assemblies = data.keyValue.assemblies;
            if (estimateId == 0) {
                //JobPONS.estimateItems = [];
                //JobPONS.originalEstimateItems = [];
                JobPONS.estimateItems = jQuery.grep(JobPONS.estimateItems, function (value) {
                    return value.COID != 0;
                });
            } else {
                JobPONS.estimateItems = jQuery.grep(JobPONS.estimateItems, function (value) {
                    return value.COID != 0;
                });
                for (i = 0; i < data.estimateItems.length; i++) {
                    data.estimateItems[i].ID = i;
                    var it = data.estimateItems[i];
                    JobPONS.estimateItems.push(it);
                }
                //JobPONS.estimateItems = data.estimateItems;
                JobPONS.originalEstimateItems = data.originalEstimateItems;
            }
            var totalCost = 0;
            for (i = 0; i < JobPONS.estimateItems.length; i++) {
                JobPONS.estimateItems[i].ID = i;
                var it = JobPONS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-PO #totalCost").val(totalCost.toFixed(2));
            $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
            createPOEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
}
function COEstimateChange() {
    var coid = $('#cos').val();
    var token = $('[name=__RequestVerificationToken]').val();
    if (coid == null) {
        COValues = "";
        openModal();
        JobPONS.estimateItems = jQuery.grep(JobPONS.estimateItems, function (value) {
            return value.COID == 0;
        });
        var totalCost = 0;
        for (i = 0; i < JobPONS.estimateItems.length; i++) {
            JobPONS.estimateItems[i].ID = i;
            var it = JobPONS.estimateItems[i]
            totalCost = totalCost + parseFloat(it.Cost);
        }

        $("#job-PO #totalCost").val(totalCost.toFixed(2));
        $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
        var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
        createPOEstimateItemTable(aEstimateItems);
        closeModal();
        return;
    }
    //TODO: check dropdown change event to bind grid
    if (COValues == "")
    {
        COValues = coid;
        coid = Number(coid);
    }
    else
    {
        var currentCOID = coid;
        var earlierCOID = new String(COValues).split(',');
        for(i = 0; i<currentCOID.length;i++)
        {
            currentCOID[i] = Number(currentCOID[i]);
        }
        for (i = 0; i < earlierCOID.length; i++) {
            earlierCOID[i] = Number(earlierCOID[i]);
        }
        var removed = $(earlierCOID).not(currentCOID).get();
        //alert('removed - ' + removed);
        if (removed != "" && removed > 0) {
            openModal();
            JobPONS.estimateItems = jQuery.grep(JobPONS.estimateItems, function (value) {
                return value.COID != removed;
            });
            var totalCost = 0;
            for (i = 0; i < JobPONS.estimateItems.length; i++) {
                JobPONS.estimateItems[i].ID = i;
                var it = JobPONS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-PO #totalCost").val(totalCost.toFixed(2));
            $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
            createPOEstimateItemTable(aEstimateItems);
            closeModal();
        }
        var added = $(currentCOID).not(earlierCOID).get();
        //alert('added - ' + added);
        COValues = coid;
        if(added != "")
        {
            coid = added;
        }
        else
        {
            return;
        }
    }
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetCOItems',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ COID: Number(coid), form: 'PO', __RequestVerificationToken: token }),
        success: function (data) {
            JobPONS.items = data.keyValue.items;
            JobPONS.assemblies = data.keyValue.assemblies;
            var loopcount = JobPONS.estimateItems.length;
            if (coid == 0) {
                JobPONS.estimateItems = jQuery.grep(JobPONS.estimateItems, function (value) {
                    return value.COID == coid;
                });

            } else {
                for (i = 0; i < data.COItems.length; i++) {
                    var it = data.COItems[i]
                    JobPONS.estimateItems.push(it);
                }
                //JobPONS.estimateItems = data.estimateItems;
                //JobPONS.originalEstimateItems = data.originalEstimateItems;
            }
            var totalCost = 0;
            for (i = 0; i < JobPONS.estimateItems.length; i++) {
                JobPONS.estimateItems[i].ID = i;
                var it = JobPONS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-PO #totalCost").val(totalCost.toFixed(2));
            $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
            createPOEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
}
function addJobPO() {
    ClearDivMessages();
    $("#poestimates").prop("selectedIndex", 0);
    $('#dvPOList').hide();
    $('#dvPOAdd').show();
    $("#poid").focus();
}
function cancelJobPO() {
    ClearDivMessages();
    ClearJobPOForm();
    $('#dvPOList').show();
    $('#dvPOAdd').hide();
}
function getJobPO(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobPO',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.POs = data.keyValue.POs;
            closeModal();
        }
    });
}
function emptyJobPOForm(data) {
    var vendors = jQuery.grep(JobPONS.vendors, function (e) {
        return e.Active == true;
    });
    $("#vendors").populateDropDownList(vendors, 0);
    $("#poestimates").populateDropDownList(data.keyValue.estimates, 0);
    $("#cos").populateDropDownListWithoutSelect(data.keyValue.cos, 0);
    $('#cos').multiselect({numberDisplayed : 1});
    ClearJobPOForm();
}
function populateJobPOList() {
    var aPOList = transformPOList(JobNS.POs);
    createPOListTable(aPOList);
}
function ClearJobPOForm() {
    $("#job-po-id").val(0);
    $("#poid").val("Auto Generated");
    $("#vendorphone").val("");
    $("#vendorfax").val("");
    $("#vendorcell").val("");
    $("#vendoraddress").val("");
    $("#vendoremail").val("");
    $("#deliverydate").val("");
    $("#vendorrep").val("");
    $("#totalCostDisplay").val("0");
    $("#totalCost").val("0");
    $("#poestimates").val("");
    $("#vendors option:selected").removeAttr("selected");
    $("#cos option:selected").removeAttr("selected");
    JobPONS.estimateItems = [];
    JobPONS.originalEstimateItems = [];
    var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
    createPOEstimateItemTable(aEstimateItems);
    $('#cos').multiselect("destroy");
    $('#cos').multiselect("rebuild");
    COValues = "";
}
function GetVendorInformation(vendorID)
{
    var vendor = jQuery.grep(JobPONS.vendors, function (e) {
        return e.ID == vendorID && e.Active == true;
    });
    if (vendor != undefined && vendor != "undefined" && vendor.length > 0) {
        $("#vendorphone").val(vendor[0].Phone);
        $("#vendorfax").val(vendor[0].Fax);
        $("#vendorcell").val(vendor[0].Cell);
        $("#vendoremail").val(vendor[0].Email);
        $("#vendoraddress").val(vendor[0].Address);
    }
    else
    {
        $("#vendorphone").val("");
        $("#vendorfax").val("");
        $("#vendorcell").val("");
        $("#vendoremail").val("");
        $("#vendoraddress").val("");
    }
}
function populateJobPOForm(data) {
    COValues = data.JobPO.COIDs;
    $("#poestimates").populateDropDownList(data.keyValue.estimates, data.JobPO.EstimateID);
    $("#job-po-id").val(data.JobPO.POID);
    $("#poid").val(data.JobPO.POCode);
    var vendors = jQuery.grep(JobPONS.vendors, function (e) {
        return e.Active == true;
    });
    $("#vendors").populateDropDownList(vendors, data.JobPO.VendorID);
    GetVendorInformation(data.JobPO.VendorID);
    $("#vendorrep").val(data.JobPO.VendorRep);
    var parsedDeliveryDate = new Date(parseInt(data.JobPO.DeliveryDate.substr(6)));
    var jsDeliveryDate = new Date(parsedDeliveryDate); //Date object
    $("#deliverydate").val(jsDeliveryDate.toLocaleDateString());
    $("#job-PO #totalCost").val(data.JobPO.TotalAmount.toFixed(2));
    $("#job-PO #totalCostDisplay").val(Number(data.JobPO.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US'));
    var valArr = new String(data.JobPO.COIDs).split(',');
    if (valArr != "null") {
        $("#cos").multiselect("select", valArr);
        $('#cos').multiselect('updateButtonText');
    }
    $("#poid").focus();
}
function populateEstimateItems(estimateID) {

}
function saveJobPO() {
    var estimateId = $('#poestimates option:selected').val();
    var poId = $('#poid').val();
    var vendorID = $('#vendors option:selected').val();
    var deliveryDate = $('#deliverydate').val();
    var vendorep = $('#vendorrep').val();
    ClearDivMessages();
    if (poId == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please enter PO Id.');
        return;
    }
    else if (vendorID == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select vendor name.');
        return;
    }
    else if (deliveryDate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select delivery date.');
        return;
    }
    //else if (estimateId == 0) {
    //    $('.alert-danger').show();
    //    $('.alert-danger').find('#dvError').append('Please select estimate name.');
    //    return;
    //}

    if (JobPONS.estimateItems.length == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please provide atleast one item to create purchase order.');
        return;
    }

    var jobData = {
    };

    var jobpo = {
        POID: parseInt($("#job-po-id").val()),
        JobID: parseInt($("#job-id").val()),
        POCode: poId,
        VendorID: vendorID,
        DeliveryDate: deliveryDate,
        VendorRep: vendorep,
        EstimateID: $('#poestimates option:selected').val(),
        TotalAmount: $('#totalCost').val(),
        COIDs: new String($('#cos').val()).replace(",","~"),
        Active: true
    }

    jobData.JobPO = jobpo;
    jobData.POEstimateItems = JobPONS.estimateItems;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobPO',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                JobNS.POs = data.POs;
                populateJobPOList();
                cancelJobPO();
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}
function transformPOEstimateItems(estimateItems) {
    estimateItems = estimateItems.sort(function (a, b) { return parseFloat(a.COID) - parseFloat(b.COID) });

    return estimateItems.map(function (item) {
        var itemAssembly = {};

        //check if item or assembly to be added 
        var unit = "";
        if (item.ItemAssemblyType == 0) {
            itemAssembly = searchObject(item.ItemAssemblyID, JobPONS.items);
            unit = itemAssembly.Unit;
        }
        else {
            itemAssembly = searchObject(item.ItemAssemblyID, JobPONS.assemblies);
        }

        var code = itemAssembly.Code;
        var name = itemAssembly.Name;
        var description = itemAssembly.Description;
        var price = item.Price;
        var itemNames = item.ItemNames;

        return [
           "<span data-item-id="+ item.ItemID +" data-val = " + item.ItemAssemblyID + " data-item-type = " + item.ItemAssemblyType + " data-co-id = " + item.COID + " class='name'>" + code + "</span>",
           "<span>" + name + "</span>",
           "<span>" + itemNames + "<input type='hidden' class='itemname' value='"+ itemNames +"'></span>",
           "<input type='text' maxlength='8' class='qty' onkeypress='enterOnlyNumericForQty(this, event)' style='width:100px' value = " + item.Quantity + ">",
           "<input type='text' maxlength='8' class='price' onkeypress='enterOnlyNumeric(this, event)' style='width:100px' value = " + price + ">",
           //"<span class='price'>" + Number(price.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='cost'>" + Number(item.Cost.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           //"<span class='delete-cust-addr update' data-val=" + item['ID'] + ">Update</span>",
           "<span class='delete-cust-addr delete' data-val=" + item['ID'] + " data-item-id=" + item.ItemID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}
function createPOEstimateItemTable(aEstimateItems) {
    $('#grid-items-po-estimates').dataTable().fnDestroy();

    $('#grid-items-po-estimates').dataTable({
        "bPaginate": true,
        "iDisplayLength": 5,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "bAutoWidth": false,
        "aaData": aEstimateItems,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if ($(aData[0]).attr("data-co-id") != "0") {
                $(nRow).addClass('highlightRow');
            }
        },
        "aaSorting": []
    });
}

$(document).on("click", "#grid-items-po-estimates .delete", function () {
    var tr = $(this).parent().parent();

    var id = $(this).attr("data-val");
    var itemid = $(this).attr("data-item-id");
    if (id == undefined || id == "undefined")
        return;

    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type")
    }

    var tmp = [];
    for (var i = 0; i < JobPONS.estimateItems.length; i++) {
        var it = JobPONS.estimateItems[i];
        if (!(it.ID == id && it.ItemID == itemid)) {
            tmp.push(it)
        }
        //if (!(it.ItemAssemblyID == itemToUpdate.ItemAssemblyID
        //        && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType)) {
        //    tmp.push(it)
        //}
    }

    JobPONS.estimateItems = tmp;

    var totalCost = 0;
    for (i = 0; i < JobPONS.estimateItems.length; i++) {
        JobPONS.estimateItems[i].ID = i;
        var it = JobPONS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    $("#job-PO #totalCost").val(totalCost.toFixed(2));
    $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
    createPOEstimateItemTable(aEstimateItems);
});
$(document).on("blur", "#grid-items-po-estimates .qty", function () {
    var tr = $(this).parents("tr")[0];
    if ($(tr).find(".qty").val() == "") {
        $(tr).find(".qty").val("0");
    }
    // Update row level Cost
    var qty = parseFloat($(tr).find(".qty").val());
    var price = parseFloat($(tr).find(".price").val());

    for (i = 0; i < JobPONS.originalEstimateItems.length; i++) {
        var it = JobPONS.originalEstimateItems[i]
        if (it.ItemAssemblyID == $(tr).find(".name").attr("data-val") && it.ItemAssemblyType == $(tr).find(".name").attr("data-item-type")) {
            if (JobPONS.originalEstimateItems[i].Quantity < qty) {
                $(tr).find('.qty').val(JobPONS.originalEstimateItems[i].Quantity);
            }
        }
    }
    qty = parseFloat($(tr).find(".qty").val());
    var cost = qty * price;
    $(tr).find(".cost").html(cost.toFixed(2));
    $(tr).find(".cost").html(Number(cost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    //Update the in-memory object ItemAssembly and Total Cost and Contract Price
    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        ItemType: $(tr).find(".name").attr("data-val"),
        Price: parseFloat($(tr).find(".price").val().replace(/[^0-9-.]/g, '')),
        Quantity: parseFloat($(tr).find(".qty").val().replace(/[^0-9-.]/g, '')),
        Cost: parseFloat($(tr).find(".cost").html().replace(/[^0-9-.]/g, '')),
        ItemNames: $(tr).find('.itemname').val(),
        COID: $(tr).find('.name').attr("data-co-id"),
        ItemID: $(tr).find('.name').attr("data-item-id")
    }

    for (i = 0; i < JobPONS.estimateItems.length; i++) {
        var it = JobPONS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType && it.ItemID == itemToUpdate.ItemID && it.COID == itemToUpdate.COID) {
            JobPONS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (i = 0; i < JobPONS.estimateItems.length; i++) {
        var it = JobPONS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#job-PO #totalCost").val(totalCost.toFixed(2));
    $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
$(document).on("blur", "#grid-items-po-estimates .price", function () {
    var tr = $(this).parents("tr")[0];
    if ($(tr).find(".price").val() == "") {
        $(tr).find(".price").val("0");
    }
    // Update row level Cost
    var qty = parseFloat($(tr).find(".qty").val());
    var price = parseFloat($(tr).find(".price").val());
    var cost = qty * price;
    $(tr).find(".cost").html(cost.toFixed(2));
    $(tr).find(".cost").html(Number(cost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    //Update the in-memory object ItemAssembly and Total Cost and Contract Price
    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        ItemType: $(tr).find(".name").attr("data-val"),
        Price: parseFloat($(tr).find(".price").val().replace(/[^0-9-.]/g, '')),
        Quantity: parseFloat($(tr).find(".qty").val().replace(/[^0-9-.]/g, '')),
        Cost: parseFloat($(tr).find(".cost").html().replace(/[^0-9-.]/g, '')),
        ItemNames: $(tr).find('.itemname').val(),
        COID: $(tr).find('.name').attr("data-co-id"),
        ItemID: $(tr).find('.name').attr("data-item-id")
    }

    for (i = 0; i < JobPONS.estimateItems.length; i++) {
        var it = JobPONS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType && it.ItemID == itemToUpdate.ItemID && it.COID == itemToUpdate.COID) {
            JobPONS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (i = 0; i < JobPONS.estimateItems.length; i++) {
        var it = JobPONS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#job-PO #totalCost").val(totalCost.toFixed(2));
    $("#job-PO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
function transformPOList(poList) {
    return poList.map(function (item) {

        var estimateName = "";
        var parsedDeliveryDate = new Date(parseInt(item.DeliveryDate.substr(6)));
        var jsDeliveryDate = new Date(parsedDeliveryDate); //Date object

        for (var i = 0; i < JobNS.estimates.length; i++) {
            if (JobNS.estimates[i].ID == item.EstimateID) {
                estimateName = JobNS.estimates[i].Description;
            }
        }
        var vendor = jQuery.grep(JobPONS.vendors, function (e) {
            return e.ID == item.VendorID;
        });
        return [
           "<span data-val = " + item.POID + ">" + item.POCode + "</span>",
           "<span>" + vendor[0].Description + "</span>",
           "<span>" + estimateName + "</span>",
           "<span>" + jsDeliveryDate.toLocaleDateString() + "</span>",
           "<span>" + Number(item.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='edit-PO' data-val=" + item.POID + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>",
           "<span class='delete-PO' data-val=" + item.POID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>",
           "<span class='generate-PO' data-val=" + item.POID + "><ul><li class='download'><a href='#non'>Generate PO</a></li></ul></span>"
        ];
    });
}
function createPOListTable(aPOList) {
    $('#grid-items-po').dataTable().fnDestroy();

    $('#grid-items-po').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 1, 2, 5, 6, 7] },
            { className: "dt-right", targets: [3, 4] },
            { defaultContent: "", render: $.fn.dataTable.render.number(',', '.', 2), targets: [4] }
        ],
        "bAutoWidth": false,
        "aaData": aPOList
    });
}
$(document).on("click", "#grid-items-po .delete-PO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Job/ToggleJobPOStatus',
                type: 'POST',
                data: { poID: id, active: false, __RequestVerificationToken: token },
                success: function (data) {
                    var index = searchPOObjectIndex(id, JobNS.POs);
                    if (index != null)
                        JobNS.POs.splice(index, 1);
                    populateJobPOList();
                    closeModal();
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});
$(document).on("click", "#grid-items-po .edit-PO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobPODetail',
        type: 'POST',
        data: { poID: id, __RequestVerificationToken: token },
        success: function (data) {
            addJobPO();
            populateJobPOForm(data);
            JobPONS.assemblies = data.keyValue.assemblies;
            JobPONS.items = data.keyValue.items;
            JobPONS.originalEstimateItems = data.keyValue.originalEstimateItems;
            JobPONS.estimateItems = data.estimateItems;
            for (i = 0; i < JobPONS.estimateItems.length; i++) {
                JobPONS.estimateItems[i].ID = i;
            }
            var aEstimateItems = transformPOEstimateItems(JobPONS.estimateItems);
            createPOEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
});
$(document).on("click", "#grid-items-po .generate-PO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    window.location = getWebsiteBaseUrl() + 'Job/CreatePODocument?poID=' + id;
});
function searchPOObjectIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].POID == nameKey) {
            return i;
        }
    }
    return null;
}
