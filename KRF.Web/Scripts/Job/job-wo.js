var JobWONS = {
    //originalEstimateItems: [],
    estimateItems: [],
    items: {},
    assemblies: {},
    WO: {}
};// End of Global objects
var COValues;
$(document).ready(function () {
    $("#save-WO").on("click", saveJobWO);
    $("#add-WO").on("click", addJobWO);
    $("#cancel-WO").on("click", cancelJobWO);
    $("#generate-WO").on("click", generateJobWO);
    $("#woestimates").on("change", WOEstimateChange);
    $("#wcos").on("change", WCOEstimateChange);
    COValues = "";
});
function GetJobWOList(ID) {
    openModal();
    cancelJobWO();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobWOList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.WOs = data.keyValue.WOs;
            if (ID == 0) {
                emptyJobWOForm(data);
            } else {
                populateJobWOList();
                emptyJobWOForm(data);
            }
            //closeModal();
        }
    });
}
function WOEstimateChange() {
    var estimateId = $('#woestimates option:selected').val();
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetEstimateItemsForWO',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ estimateID: estimateId }),
        success: function (data) {
            JobWONS.items = data.keyValue.items;
            JobWONS.assemblies = data.keyValue.assemblies;
            if (estimateId == 0) {
                //JobWONS.estimateItems = [];
                //JobWONS.originalEstimateItems = [];
                JobWONS.estimateItems = jQuery.grep(JobWONS.estimateItems, function (value) {
                    return value.COID != 0;
                });
            } else {
                JobWONS.estimateItems = jQuery.grep(JobWONS.estimateItems, function (value) {
                    return value.COID != 0;
                });
                for (i = 0; i < data.estimateItems.length; i++) {
                    var it = data.estimateItems[i]
                    JobWONS.estimateItems.push(it);
                }
                //JobWONS.estimateItems = data.estimateItems;
                //JobWONS.originalEstimateItems = data.originalEstimateItems;
            }
            var totalCost = 0;
            for (i = 0; i < JobWONS.estimateItems.length; i++) {
                JobWONS.estimateItems[i].ID = i;
                var it = JobWONS.estimateItems[i];
                totalCost = totalCost + parseFloat(it.Amount);
            }

            $("#job-WO #totalCost").val(totalCost.toFixed(2));
            $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
            createWOEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
}
function WCOEstimateChange() {
    var coid = $('#wcos').val();
    var token = $('[name=__RequestVerificationToken]').val();
    if (coid == null) {
        COValues = "";
        openModal();
        JobWONS.estimateItems = jQuery.grep(JobWONS.estimateItems, function (value) {
            return value.COID == 0;
        });
        var totalCost = 0;
        for (i = 0; i < JobWONS.estimateItems.length; i++) {
            JobWONS.estimateItems[i].ID = i;
            var it = JobWONS.estimateItems[i];
            totalCost = totalCost + parseFloat(it.Amount);
        }

        $("#job-WO #totalCost").val(totalCost.toFixed(2));
        $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
        var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
        createWOEstimateItemTable(aEstimateItems);
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
            JobWONS.estimateItems = jQuery.grep(JobWONS.estimateItems, function (value) {
                return value.COID != removed;
            });
            var totalCost = 0;
            for (i = 0; i < JobWONS.estimateItems.length; i++) {
                JobWONS.estimateItems[i].ID = i;
                var it = JobWONS.estimateItems[i]
                totalCost = totalCost + parseFloat(it.Cost);
            }

            $("#job-WO #totalCost").val(totalCost.toFixed(2));
            $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
            createWOEstimateItemTable(aEstimateItems);
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
        url: getWebsiteBaseUrl() + 'Job/GetCOItemsForWO',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ COID: Number(coid), form: 'WO', __RequestVerificationToken: token }),
        success: function (data) {
            JobWONS.items = data.keyValue.items;
            JobWONS.assemblies = data.keyValue.assemblies;
            var loopcount = JobWONS.estimateItems.length;
            if (coid == 0) {
                JobWONS.estimateItems = jQuery.grep(JobWONS.estimateItems, function (value) {
                    return value.COID == coid;
                });

            } else {
                for (i = 0; i < data.COItems.length; i++) {
                    var it = data.COItems[i]
                    JobWONS.estimateItems.push(it);
                }
                //JobWONS.estimateItems = data.estimateItems;
                //JobWONS.originalEstimateItems = data.originalEstimateItems;
            }
            var totalCost = 0;
            for (i = 0; i < JobWONS.estimateItems.length; i++) {
                JobWONS.estimateItems[i].ID = i;
                var it = JobWONS.estimateItems[i];
                totalCost = totalCost + parseFloat(it.Amount);
            }

            $("#job-WO #totalCost").val(totalCost.toFixed(2));
            $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
            var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
            createWOEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
}
function addJobWO() {
    ClearDivMessages();
    $('#dvWOList').hide();
    $('#dvWOAdd').show();
    $("#woid").focus();
}
function cancelJobWO() {
    ClearDivMessages();
    ClearJobWOForm();
    $('#dvWOAdd').show();
    //$('#dvWOList').show();
    //$('#dvWOAdd').hide();
}
function getJobWO(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobWO',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.WOs = data.keyValue.WOs;
            closeModal();
        }
    });
}
function emptyJobWOForm(data) {
    $("#woestimates").populateDropDownList(data.keyValue.estimates, 0);
    $("#wocrewleads").populateDropDownList(data.keyValue.crewleads, 0);
    $("#wcos").populateDropDownListWithoutSelect(data.keyValue.cos, 0);
    $('#wcos').multiselect({numberDisplayed : 1});
    ClearJobWOForm();
}
function populateJobWOList() {
    //var aWOList = transformWOList(JobNS.WOs);
    //createWOListTable(aWOList);
    if(JobNS.WOs.length > 0)
    {
        EditWo(JobNS.WOs[0].WOID);
    }
    else
    {
        $("#generate-WO").hide();
        closeModal();
    }
}
function ClearJobWOForm() {
    $("#woid").focus();
    $("#job-wo-id").val(0);
    $("#woid").val("Auto Generated");
    $("#weekendingdate").val("");
    $("#wocrewleads").prop("selectedIndex", 0);
    $("#totalCostDisplay").val("0");
    $("#totalCost").val("0");
    $("#woestimates").prop("selectedIndex", 0);
    $("#wcos option:selected").removeAttr("selected");
    JobWONS.estimateItems = [];
    //JobWONS.originalEstimateItems = [];
    var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
    createWOEstimateItemTable(aEstimateItems);
    $('#wcos').multiselect("destroy");
    $('#wcos').multiselect("rebuild");
    COValues = "";
}
function populateJobWOForm(data) {
    COValues = data.JobWO.COIDs;
    $("#woestimates").populateDropDownList(data.keyValue.estimates, data.JobWO.EstimateID);
    $("#wocrewleads").populateDropDownList(data.keyValue.crewleads, data.JobWO.LeadID);
    $("#job-wo-id").val(data.JobWO.WOID);
    $("#woid").val(data.JobWO.WOCode);
    var parsedDate = new Date(parseInt(data.JobWO.WorkWeekEndingDate.substr(6)));
    var jsDate = new Date(parsedDate); //Date object
    $("#weekendingdate").val(jsDate.toLocaleDateString());
    $("#job-WO #totalCost").val(data.JobWO.TotalAmount.toFixed(2));
    $("#job-WO #totalCostDisplay").val(Number(data.JobWO.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US'));
    var valArr = new String(data.JobWO.COIDs).split(',');
    if (valArr != "null") {
        $("#wcos").multiselect("select", valArr);
        $('#wcos').multiselect('updateButtonText');
    }
    $("#woid").focus();
}
function saveJobWO() {
    var crewLeadID = $('#wocrewleads option:selected').val();
    var woId = $('#woid').val();
    var weekEndDate = $('#weekendingdate').val();
    ClearDivMessages();
    if (woId == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please enter WO Id.');
        return;
    }
    else if (weekEndDate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select week ending date.');
        return;
    }
    else if (crewLeadID == undefined || crewLeadID == "undefined" || crewLeadID == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select crew leader name.');
        return;
    }

    if (JobWONS.estimateItems.length == 0)
    {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please provide atleast one item to create work order.');
        return;
    }

    var jobData = {
    };

    var JobWO = {
        WOID: parseInt($("#job-wo-id").val()),
        JobID: parseInt($("#job-id").val()),
        WOCode: woId,
        WODesc: "",
        WorkWeekEndingDate: weekEndDate,
        EstimateID: $('#woestimates option:selected').val(),
        LeadID: $('#wocrewleads option:selected').val(),
        TotalAmount: $('#wo-form').find('#totalCost').val(),
        COIDs: new String($('#wcos').val()).replace(",","~"),
        Active: true
    }

    jobData.JobWO = JobWO;
    jobData.WOEstimateItems = JobWONS.estimateItems;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobWO',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                JobNS.WOs = data.WOs;
                $("#generate-WO").show();
                $("#job-wo-id").val(JobNS.WOs[0].WOID);
                $("#woid").val(JobNS.WOs[0].WOCode);
                //populateJobWOList();
                $('.alert-success').show();
                $('.alert-success').find('#dvSuccess').text("Work order saved successfully.");
                //cancelJobWO();
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}
function transformWOEstimateItems(estimateItems) {
    estimateItems = estimateItems.sort(function (a, b) { return parseFloat(a.COID) - parseFloat(b.COID) });

    return estimateItems.map(function (item) {
        var itemAssembly = {};

        //check if item or assembly to be added 
        var unit = "";
        if (item.ItemAssemblyType == 0) {
            itemAssembly = searchObject(item.ItemAssemblyID, JobWONS.items);
            unit = itemAssembly.Unit;
        }
        else {
            itemAssembly = searchObject(item.ItemAssemblyID, JobWONS.assemblies);
        }

        var code = itemAssembly.Code;
        var name = itemAssembly.Name;
        var description = itemAssembly.Description;
        var price = item.Price;
        var itemNames = item.ItemNames;

        return [
           "<span data-item-id="+ item.ItemID +" data-val = " + item.ItemAssemblyID + " data-item-type = " + item.ItemAssemblyType + " data-co-id = " + item.COID + " class='name'>" + code + "</span>",
           "<span>" + name + "</span>",
           "<span>" + itemNames + "<input type='hidden' class='itemname' value='" + itemNames + "'></span>",
           "<span class='budget'>" + Number(item.Budget.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<input type='text' maxlength='8' class='used' onkeypress='enterOnlyNumericForQty(this, event)' style='width:100px' value = " + item.Used + ">",
           "<input type='text' maxlength='8' class='rate' onkeypress='enterOnlyNumeric(this, event)' style='width:100px' value = " + item.Rate + ">",
           //"<span class='rate'>" + Number(item.Rate.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='balance'>" + Number(item.Balance.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='amount'>" + Number(item.Amount.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           //"<span class='delete-cust-addr update' data-val=" + item['ID'] + ">Update</span>",
           "<span class='delete-cust-addr delete' data-item-id="+ item.ItemID +" data-val=" + item['ID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}
function createWOEstimateItemTable(aEstimateItems) {
    $('#grid-items-wo-estimates').dataTable().fnDestroy();

    $('#grid-items-wo-estimates').dataTable({
        "bPaginate": true,
        "iDisplayLength": 5,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 1, 2, 8] },
            { className: "dt-right", targets: [3, 4, 5, 6, 7] },
            { defaultContent: "", render: $.fn.dataTable.render.number(',', '.', 2), targets: [3, 4, 5, 6, 7] }
        ],
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
$(document).on("click", "#grid-items-wo-estimates .delete", function () {
    var tr = $(this).parent().parent();

    var id = $(this).attr("data-val");
    var itemId = $(this).attr("data-item-id");
    if (id == undefined || id == "undefined")
        return;
    //var itemToUpdate = {
    //    ItemAssemblyID: $(tr).find(".name").attr("data-val"),
    //    ItemAssemblyType: $(tr).find(".name").attr("data-item-type")
    //}

    var tmp = [];
    for (var i = 0; i < JobWONS.estimateItems.length; i++) {
        var it = JobWONS.estimateItems[i];
        if (!(it.ID == id && it.ItemID == itemId)) {
            tmp.push(it)
        }
        //if (!(it.ItemAssemblyID == itemToUpdate.ItemAssemblyID
        //        && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType)) {
        //    tmp.push(it)
        //}
    }

    JobWONS.estimateItems = tmp;

    var totalCost = 0;
    for (i = 0; i < JobWONS.estimateItems.length; i++) {
        JobWONS.estimateItems[i].ID = i;
        var it = JobWONS.estimateItems[i];
        totalCost = totalCost + parseFloat(it.Amount);
    }

    $("#job-WO #totalCost").val(totalCost.toFixed(2));
    $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
    createWOEstimateItemTable(aEstimateItems);
});
$(document).on("blur", "#grid-items-wo-estimates .used", function () {
    var tr = $(this).parents("tr")[0];
    if ($(tr).find(".used").val() == "") {
        $(tr).find(".used").val("0");
    }
    // Update row level Cost
    var budget = parseFloat($(tr).find(".budget").html().replace(/[^0-9-.]/g, ''));
    var used = parseFloat($(tr).find(".used").val());
    var usedToCheck = parseFloat($(tr).find(".used").val());
    var price = parseFloat($(tr).find(".rate").val());

    if (Number(usedToCheck) > Number(budget))
    {
        $(tr).find(".used").val("0");
    }
    used = parseFloat($(tr).find(".used").val());
    var amount = used * price;
    $(tr).find(".amount").html(amount.toFixed(2));
    $(tr).find(".amount").html(Number(amount.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    var balance = budget - used;
    $(tr).find(".balance").html(balance.toFixed(2));
    $(tr).find(".balance").html(Number(balance.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    //Update the in-memory object ItemAssembly and Total Cost and Contract Price
    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        Budget: parseFloat($(tr).find(".budget").html().replace(/[^0-9-.]/g, '')),
        Used: parseFloat($(tr).find(".used").val().replace(/[^0-9-.]/g, '')),
        Rate: parseFloat($(tr).find(".rate").val().replace(/[^0-9-.]/g, '')),
        Balance: parseFloat($(tr).find(".balance").html().replace(/[^0-9-.]/g, '')),
        Amount: parseFloat($(tr).find(".amount").html().replace(/[^0-9-.]/g, '')),
        ItemNames: $(tr).find('.itemname').val(),
        COID: $(tr).find('.name').attr("data-co-id"),
        ItemID: $(tr).find('.name').attr("data-item-id")
    }

    for (var i = 0; i < JobWONS.estimateItems.length; i++) {
        var it = JobWONS.estimateItems[i];
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType && it.ItemID == itemToUpdate.ItemID && it.COID == itemToUpdate.COID) {
            JobWONS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (var i = 0; i < JobWONS.estimateItems.length; i++) {
        var it = JobWONS.estimateItems[i];
        totalCost = totalCost + parseFloat(it.Amount);
    }

    // Update Total Cost 
    $("#job-WO #totalCost").val(totalCost.toFixed(2));
    $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
$(document).on("blur", "#grid-items-wo-estimates .rate", function () {
    var tr = $(this).parents("tr")[0];
    // Update row level Cost
    var budget = parseFloat($(tr).find(".budget").html().replace(/[^0-9-.]/g, ''));
    var used = parseFloat($(tr).find(".used").val());
    var usedToCheck = parseFloat($(tr).find(".used").val());
    var price = parseFloat($(tr).find(".rate").val());

    if (Number(usedToCheck) > Number(budget)) {
        $(tr).find(".used").val("0");
    }
    used = parseFloat($(tr).find(".used").val());
    var amount = used * price;
    $(tr).find(".amount").html(amount.toFixed(2));
    $(tr).find(".amount").html(Number(amount.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    var balance = budget - used;
    $(tr).find(".balance").html(balance.toFixed(2));
    $(tr).find(".balance").html(Number(balance.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    //Update the in-memory object ItemAssembly and Total Cost and Contract Price
    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type"),
        Budget: parseFloat($(tr).find(".budget").html().replace(/[^0-9-.]/g, '')),
        Used: parseFloat($(tr).find(".used").val().replace(/[^0-9-.]/g, '')),
        Rate: parseFloat($(tr).find(".rate").val().replace(/[^0-9-.]/g, '')),
        Balance: parseFloat($(tr).find(".balance").html().replace(/[^0-9-.]/g, '')),
        Amount: parseFloat($(tr).find(".amount").html().replace(/[^0-9-.]/g, '')),
        ItemNames: $(tr).find('.itemname').val(),
        COID: $(tr).find('.name').attr("data-co-id"),
        ItemID: $(tr).find('.name').attr("data-item-id")
    }

    for (i = 0; i < JobWONS.estimateItems.length; i++) {
        var it = JobWONS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType && it.ItemID == itemToUpdate.ItemID && it.COID == itemToUpdate.COID) {
            JobWONS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var totalCost = 0;
    for (i = 0; i < JobWONS.estimateItems.length; i++) {
        var it = JobWONS.estimateItems[i]
        totalCost = totalCost + parseFloat(it.Amount);
    }

    // Update Total Cost 
    $("#job-WO #totalCost").val(totalCost.toFixed(2));
    $("#job-WO #totalCostDisplay").val(Number(totalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
function transformWOList(woList) {

    return woList.map(function (item) {

        var estimateName = "";
        var parsedDate = new Date(parseInt(item.WorkWeekEndingDate.substr(6)));
        var jsDate = new Date(parsedDate); //Date object

        for (var i = 0; i < JobNS.estimates.length; i++) {
            if (JobNS.estimates[i].ID == item.EstimateID) {
                estimateName = JobNS.estimates[i].Description;
            }
        }

        return [
           "<span data-val = " + item.WOID + ">" + item.WOCode + "</span>",
           "<span>" + item.CrewLeader + "</span>",
           "<span>" + estimateName + "</span>",
           "<span>" + jsDate.toLocaleDateString() + "</span>",
           "<span>" + Number(item.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='edit-WO' data-val=" + item.WOID + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>",
           "<span class='delete-WO' data-val=" + item.WOID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>",
           "<span class='generate-WO' data-val=" + item.WOID + "><ul><li class='download'><a href='#non'>Generate WO</a></li></ul></span>"
        ];
    });
}
function createWOListTable(aWOList) {
    $('#grid-items-wo').dataTable().fnDestroy();

    $('#grid-items-wo').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "bAutoWidth": false,
        "aaData": aWOList
    });
}
$(document).on("click", "#grid-items-wo .delete-WO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Job/ToggleJobWOStatus',
                type: 'POST',
                data: { woID: id, active: false, __RequestVerificationToken: token },
                success: function (data) {
                    var index = searchWOObjectIndex(id, JobNS.WOs);
                    if (index != null)
                        JobNS.WOs.splice(index, 1);
                    populateJobWOList();
                    closeModal();
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});
$(document).on("click", "#grid-items-wo .edit-WO", function () {
    var id = $(this).attr("data-val");
    EditWo(id);
});

function EditWo(id) {
    var token = $('[name=__RequestVerificationToken]').val();
    var jobID = parseInt($("#job-id").val())
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobWODetail',
        type: 'POST',
        data: { woID: id, jobID: jobID, __RequestVerificationToken: token },
        success: function (data) {
            populateJobWOForm(data);
            JobWONS.assemblies = data.keyValue.assemblies;
            JobWONS.items = data.keyValue.items;
            //JobWONS.originalEstimateItems = data.keyValue.originalEstimateItems;
            JobWONS.estimateItems = data.estimateItems;
            for (i = 0; i < JobWONS.estimateItems.length; i++) {
                JobWONS.estimateItems[i].ID = i;
            }
            var aEstimateItems = transformWOEstimateItems(JobWONS.estimateItems);
            createWOEstimateItemTable(aEstimateItems);
            addJobWO();
            closeModal();
        }
    });
}

$(document).on("click", "#grid-items-wo .generate-WO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    window.location = getWebsiteBaseUrl() + 'Job/CreateWODocument?woID=' + id;
});
function generateJobWO()
{
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $("#job-wo-id").val();
    window.location = getWebsiteBaseUrl() + 'Job/CreateWODocument?woID=' + id;
}
function searchWOObjectIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].WOID == nameKey) {
            return i;
        }
    }
    return null;
}
