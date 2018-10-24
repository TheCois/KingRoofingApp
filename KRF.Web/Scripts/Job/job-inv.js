var JobINVNS = {
    originalEstimateItems: [],
    estimateItems: [],
    items: {},
    assemblies: {},
    INV: {}
};// End of Global objects
var COValues;
$(document).ready(function () {
    $("#save-INV").on("click", saveJobINV);
    $("#add-INV").on("click", addJobINV);
    $("#cancel-INV").on("click", cancelJobINV);
    $("#INVestimates").on("change", INVEstimateChange);
    $("#invcos").on("change", INVCOEstimateChange);
});
function GetJobINVList(ID) {
    openModal();
    cancelJobINV();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobInvoiceList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.INVs = data.keyValue.INVs;
            if (ID == 0) {
                emptyJobINVForm(data);
            } else {
                populateJobINVList();
                emptyJobINVForm(data);
            }
            closeModal();
        }
    });
}

function INVEstimateChange() {
    var estimateId = $('#INVestimates option:selected').val();
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetEstimateItemsForInvoice',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ estimateID: estimateId }),
        success: function (data) {
            JobINVNS.items = data.keyValue.items;
            JobINVNS.assemblies = data.keyValue.assemblies;
            if (estimateId == 0) {
                //JobINVNS.estimateItems = [];
                //JobINVNS.originalEstimateItems = [];
                JobINVNS.estimateItems = jQuery.grep(JobINVNS.estimateItems, function (value) {
                    return value.COID != 0;
                });
            } else {
                JobINVNS.estimateItems = jQuery.grep(JobINVNS.estimateItems, function (value) {
                    return value.COID != 0;
                });
                for (i = 0; i < data.estimateItems.length; i++) {
                    data.estimateItems[i].ID = i;
                    var it = data.estimateItems[i];
                    JobINVNS.estimateItems.push(it);
                }
                //JobINVNS.estimateItems = data.estimateItems;
                JobINVNS.originalEstimateItems = data.originalEstimateItems;
            }
            var totalCost = 0;
            for (i = 0; i < JobINVNS.estimateItems.length; i++) {
                JobINVNS.estimateItems[i].ID = i;
                var it = JobINVNS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-INV #totalCost").val(totalCost.toFixed(2));
            $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
            createINVEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
}
function INVCOEstimateChange() {
    var coid = $('#invcos').val();
    var token = $('[name=__RequestVerificationToken]').val();
    if (coid == null) {
        COValues = "";
        openModal();
        JobINVNS.estimateItems = jQuery.grep(JobINVNS.estimateItems, function (value) {
            return value.COID == 0;
        });
        var totalCost = 0;
        for (i = 0; i < JobINVNS.estimateItems.length; i++) {
            JobINVNS.estimateItems[i].ID = i;
            var it = JobINVNS.estimateItems[i]
            totalCost = totalCost + parseFloat(it.Cost);
        }

        $("#job-INV #totalCost").val(totalCost.toFixed(2));
        $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
        var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
        createINVEstimateItemTable(aEstimateItems);
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
            JobINVNS.estimateItems = jQuery.grep(JobINVNS.estimateItems, function (value) {
                return value.COID != removed;
            });
            var totalCost = 0;
            for (i = 0; i < JobINVNS.estimateItems.length; i++) {
                JobINVNS.estimateItems[i].ID = i;
                var it = JobINVNS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-INV #totalCost").val(totalCost.toFixed(2));
            $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
            createINVEstimateItemTable(aEstimateItems);
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
        url: getWebsiteBaseUrl() + 'Job/GetInvoiceCOItems',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ COID: Number(coid), __RequestVerificationToken: token }),
        success: function (data) {
            JobINVNS.items = data.keyValue.items;
            JobINVNS.assemblies = data.keyValue.assemblies;
            var loopcount = JobINVNS.estimateItems.length;
            if (coid == 0) {
                JobINVNS.estimateItems = jQuery.grep(JobINVNS.estimateItems, function (value) {
                    return value.COID == coid;
                });

            } else {
                for (i = 0; i < data.COItems.length; i++) {
                    var it = data.COItems[i]
                    JobINVNS.estimateItems.push(it);
                }
                //JobINVNS.estimateItems = data.estimateItems;
                //JobINVNS.originalEstimateItems = data.originalEstimateItems;
            }
            var totalCost = 0;
            for (i = 0; i < JobINVNS.estimateItems.length; i++) {
                JobINVNS.estimateItems[i].ID = i;
                var it = JobINVNS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-INV #totalCost").val(totalCost.toFixed(2));
            $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
            createINVEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
}
function addJobINV() {
    ClearDivMessages();
    $("#INVestimates").prop("selectedIndex", 0);
    $('#dvINVList').hide();
    $('#dvINVAdd').show();
    $("#invid").focus();
}
function cancelJobINV() {
    ClearDivMessages();
    ClearJobINVForm();
    $('#dvINVList').show();
    $('#dvINVAdd').hide();
}
function getJobINV(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobInvoice',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.INVs = data.keyValue.INVs;
            closeModal();
        }
    });
}
function emptyJobINVForm(data) {
    $("#INVestimates").populateDropDownList(data.keyValue.estimates, 0);
    $("#invcos").populateDropDownListWithoutSelect(data.keyValue.cos, 0);
    $('#invcos').multiselect({numberDisplayed : 1});
    ClearJobINVForm();
}
function populateJobINVList() {
    var aINVList = transformINVList(JobNS.INVs);
    createINVListTable(aINVList);
}
function ClearJobINVForm() {
    $("#job-inv-id").val(0);
    $("#invid").val("");
    $("#invnotes").val("");
    $("#invoicedate").val("");
    $("#job-INV #totalCostDisplay").val("0");
    $("#job-INV #totalCost").val("0");
    $("#INVestimates").val("");
    $("#invcos option:selected").removeAttr("selected");
    JobINVNS.estimateItems = [];
    JobINVNS.originalEstimateItems = [];
    var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
    createINVEstimateItemTable(aEstimateItems);
    $('#invcos').multiselect("destroy");
    $('#invcos').multiselect("rebuild");
    COValues = "";
}
function populateJobINVForm(data) {
    COValues = data.JobINV.COIDs;
    $("#INVestimates").populateDropDownList(data.keyValue.estimates, data.JobINV.EstimateID);
    $("#job-inv-id").val(data.JobINV.InvoiceID);
    $("#invid").val(data.JobINV.InvoiceCode);
    $("#invnotes").val(data.JobINV.Notes);
    var parsedDate = new Date(parseInt(data.JobINV.InvoiceDate.substr(6)));    var jsDate = new Date(parsedDate); //Date object
    $("#invoicedate").val(jsDate.toLocaleDateString());
    $("#job-INV #totalCost").val(data.JobINV.TotalAmount.toFixed(2));
    $("#job-INV #totalCostDisplay").val(Number(data.JobINV.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US'));
    var valArr = new String(data.JobINV.COIDs).split(',');
    if (valArr != "null") {
        $("#invcos").multiselect("select", valArr);
        $('#invcos').multiselect('updateButtonText');
    }
    $("#invid").focus();
}
function populateEstimateItems(estimateID) {

}
function saveJobINV() {
    var estimateId = $('#INVestimates option:selected').val();
    var invId = $('#invid').val();
    var invoiceDate = $('#invoicedate').val();
    ClearDivMessages();
    if (invId == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please enter Invoice Id.');
        return;
    }
    else if (invoiceDate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select invoice date.');
        return;
    }
    //else if (estimateId == 0) {
    //    $('.alert-danger').show();
    //    $('.alert-danger').find('#dvError').append('Please select estimate name.');
    //    return;
    //}

    if (JobINVNS.estimateItems.length == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please provide atleast one item to create invoice.');
        return;
    }

    var jobData = {
    };

    var jobInv = {
        InvoiceID: parseInt($("#job-inv-id").val()),
        JobID: parseInt($("#job-id").val()),
        InvoiceCode: invId,
        InvoiceDate: invoiceDate,
        Notes: $("#invnotes").val(),
        EstimateID: $('#INVestimates option:selected').val(),
        TotalAmount: $("#job-INV #totalCost").val(),
        COIDs: new String($('#invcos').val()).replace(",","~"),
        Active: true
    }

    jobData.JobInvoice = jobInv;
    jobData.InvoiceItems = JobINVNS.estimateItems;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobInvoice',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                JobNS.INVs = data.INVs;
                populateJobINVList();
                cancelJobINV();
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}
function transformINVEstimateItems(estimateItems) {
    estimateItems = estimateItems.sort(function (a, b) { return parseFloat(a.COID) - parseFloat(b.COID) });

    return estimateItems.map(function (item) {
        var itemAssembly = {};

        //check if item or assembly to be added 
        var unit = "";
        if (item.ItemAssemblyType == 0) {
            itemAssembly = searchObject(item.ItemAssemblyID, JobINVNS.items);
            unit = itemAssembly.Unit;
        }
        else {
            itemAssembly = searchObject(item.ItemAssemblyID, JobINVNS.assemblies);
        }

        var code = itemAssembly.Code;
        var name = itemAssembly.Name;
        var description = itemAssembly.Description;
        var price = item.Price;
        var itemNames = item.ItemNames;

        return [
           "<span data-val = " + item.ItemAssemblyID + " data-item-type = " + item.ItemAssemblyType + " data-co-id = " + item.COID + " class='name'>" + code + "</span>",
           "<span>" + name + "</span>",
           "<input type='text' maxlength='8' class='qty' onkeypress='enterOnlyNumericForQty(this, event)' style='width:100px' value = " + item.Quantity + ">",
           "<input type='text' maxlength='8' class='price' onkeypress='enterOnlyNumeric(this, event)' style='width:100px' value = " + price + ">",
           "<span class='cost'>" + Number(item.Cost.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='delete-cust-addr delete' data-val=" + item['ID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}
function createINVEstimateItemTable(aEstimateItems) {
    $('#grid-items-inv-estimates').dataTable().fnDestroy();

    $('#grid-items-inv-estimates').dataTable({
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

$(document).on("click", "#grid-items-inv-estimates .delete", function () {
    var tr = $(this).parent().parent();

    var id = $(this).attr("data-val");

    if (id == undefined || id == "undefined")
        return;

    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        COID: $(tr).find(".name").attr("data-co-id")
    }

    var tmp = [];
    for (var i = 0; i < JobINVNS.estimateItems.length; i++) {
        var it = JobINVNS.estimateItems[i];
        if (!(it.ID == id)) {
            tmp.push(it)
        }
        //if (!(it.ItemAssemblyID == itemToUpdate.ItemAssemblyID
        //        && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType)) {
        //    tmp.push(it)
        //}
    }

    JobINVNS.estimateItems = tmp;

    var totalCost = 0;
    for (i = 0; i < JobINVNS.estimateItems.length; i++) {
        JobINVNS.estimateItems[i].ID = i;
        var it = JobINVNS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    $("#job-INV #totalCost").val(totalCost.toFixed(2));
    $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
    createINVEstimateItemTable(aEstimateItems);
});
$(document).on("blur", "#grid-items-inv-estimates .qty", function () {
    var tr = $(this).parents("tr")[0];
    if ($(tr).find(".qty").val() == "") {
        $(tr).find(".qty").val("0");
    }
    // Update row level Cost
    var qty = parseFloat($(tr).find(".qty").val());
    var price = parseFloat($(tr).find(".price").val());

    //for (i = 0; i < JobINVNS.originalEstimateItems.length; i++) {
    //    var it = JobINVNS.originalEstimateItems[i]
    //    if (it.ItemAssemblyID == $(tr).find(".name").attr("data-val") && it.ItemAssemblyType == $(tr).find(".name").attr("data-item-type")) {
    //        if (JobINVNS.originalEstimateItems[i].Quantity < qty) {
    //            $(tr).find('.qty').val(JobINVNS.originalEstimateItems[i].Quantity);
    //        }
    //    }
    //}
    //qty = parseFloat($(tr).find(".qty").val());
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
        COID: $(tr).find('.name').attr("data-co-id")
    }

    for (i = 0; i < JobINVNS.estimateItems.length; i++) {
        var it = JobINVNS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType && it.COID == itemToUpdate.COID) {
            JobINVNS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (i = 0; i < JobINVNS.estimateItems.length; i++) {
        var it = JobINVNS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#job-INV #totalCost").val(totalCost.toFixed(2));
    $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
$(document).on("blur", "#grid-items-inv-estimates .price", function () {
    var tr = $(this).parents("tr")[0];
    if ($(tr).find(".price").val() == "") {
        $(tr).find(".price").val("0");
    }
    // Update row level Cost
    var qty = parseFloat($(tr).find(".qty").val());
    var price = parseFloat($(tr).find(".price").val());

    //for (i = 0; i < JobINVNS.originalEstimateItems.length; i++) {
    //    var it = JobINVNS.originalEstimateItems[i]
    //    if (it.ItemAssemblyID == $(tr).find(".name").attr("data-val") && it.ItemAssemblyType == $(tr).find(".name").attr("data-item-type")) {
    //        if (JobINVNS.originalEstimateItems[i].Quantity < qty) {
    //            $(tr).find('.qty').val(JobINVNS.originalEstimateItems[i].Quantity);
    //        }
    //    }
    //}
    //qty = parseFloat($(tr).find(".qty").val());
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
        COID: $(tr).find('.name').attr("data-co-id")
    }

    for (i = 0; i < JobINVNS.estimateItems.length; i++) {
        var it = JobINVNS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType && it.COID == itemToUpdate.COID) {
            JobINVNS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (i = 0; i < JobINVNS.estimateItems.length; i++) {
        var it = JobINVNS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#job-INV #totalCost").val(totalCost.toFixed(2));
    $("#job-INV #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
function transformINVList(invList) {
    return invList.map(function (item) {

        var estimateName = "";
        var parsedDate = new Date(parseInt(item.InvoiceDate.substr(6)));        var jsDate = new Date(parsedDate); //Date object
        for (var i = 0; i < JobNS.estimates.length; i++) {
            if (JobNS.estimates[i].ID == item.EstimateID) {
                estimateName = JobNS.estimates[i].Description;
            }
        }

        return [
           "<span data-val = " + item.InvoiceID + ">" + item.InvoiceCode + "</span>",
           "<span>" + jsDate.toLocaleDateString() + "</span>",
           "<span>" + estimateName + "</span>",
           "<span>" + Number(item.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='edit-INV' data-val=" + item.InvoiceID + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>",
           "<span class='delete-INV' data-val=" + item.InvoiceID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>",
           "<span class='generate-INV' data-val=" + item.InvoiceID + "><ul><li class='download'><a href='#non'>Generate Invoice</a></li></ul></span>"
        ];
    });
}
function createINVListTable(aINVList) {
    $('#grid-items-inv').dataTable().fnDestroy();

    $('#grid-items-inv').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 2, 4, 5, 6] },
            { className: "dt-right", targets: [1, 3] },
            { defaultContent: "", render: $.fn.dataTable.render.number(',', '.', 2), targets: [3] }
        ],
        "bAutoWidth": false,
        "aaData": aINVList
    });
}
$(document).on("click", "#grid-items-inv .delete-INV", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Job/ToggleJobInvoiceStatus',
                type: 'POST',
                data: { invoiceID: id, active: false, __RequestVerificationToken: token },
                success: function (data) {
                    var index = searchINVObjectIndex(id, JobNS.INVs);
                    if (index != null)
                        JobNS.INVs.splice(index, 1);
                    populateJobINVList();
                    closeModal();
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});
$(document).on("click", "#grid-items-inv .edit-INV", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobInvoiceDetail',
        type: 'POST',
        data: { invoiceID: id, __RequestVerificationToken: token },
        success: function (data) {
            addJobINV();
            populateJobINVForm(data);
            JobINVNS.assemblies = data.keyValue.assemblies;
            JobINVNS.items = data.keyValue.items;
            JobINVNS.originalEstimateItems = data.keyValue.originalEstimateItems;
            JobINVNS.estimateItems = data.estimateItems;
            for (i = 0; i < JobINVNS.estimateItems.length; i++) {
                JobINVNS.estimateItems[i].ID = i;
            }
            var aEstimateItems = transformINVEstimateItems(JobINVNS.estimateItems);
            createINVEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
});
$(document).on("click", "#grid-items-inv .generate-INV", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    window.location = getWebsiteBaseUrl() + 'Job/CreateInvoiceDocument?invoiceID=' + id;
});
function searchINVObjectIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].InvoiceID == nameKey) {
            return i;
        }
    }
    return null;
}
