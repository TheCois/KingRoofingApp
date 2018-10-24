var JobCONS = {
    originalEstimateItems: {},
    estimateItems: {},
    items: {},
    assemblies: {},
    PO: {},
    reps: {}
};// End of Global objects
$(document).ready(function () {
    $("#save-CO").on("click", saveJobCO);
    $("#add-CO").on("click", addJobCO);
    $("#cancel-CO").on("click", cancelJobCO);
    $("#AddCOItem").on("click", AddCOItem);
    $("#cocustomerreps").on("change", RepsChange)
});
function RepsChange()
{
    var repid = $('#cocustomerreps').val();
    var rep = $.grep(JobCONS.reps, function (e) {
        return e.ID == repid;
    });
    if (rep.length > 0) {
        $("#cucustomerrepemail").val(rep[0].Email);
    }
    else
    {
        $("#cucustomerrepemail").val("");
    }
}
function GetJobCOList(ID) {
    openModal();
    cancelJobCO();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobCOList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            closeModal();
            JobNS.COs = data.keyValue.COs;
            JobCONS.reps = data.keyValue.reps;
            if (ID == 0) {
                emptyJobCOForm(data);
            } else {
                populateJobCOList();
                emptyJobCOForm(data);
            }
        },
        error: function(data)
        {
            closeModal();
        }
    });
}
function addJobCO() {
    ClearDivMessages();
    $("#co-items").prop("selectedIndex", 0);
    $('#dvCOList').hide();
    $('#dvCOAdd').show();
}
function cancelJobCO() {
    ClearDivMessages();
    ClearJobCOForm();
    $('#dvCOList').show();
    $('#dvCOAdd').hide();
}
function getJobCO(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobCO',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.COs = data.keyValue.COs;
            closeModal();
        }
    });
}
function emptyJobCOForm(data) {
    //$("#poestimates").populateDropDownList(data.keyValue.estimates, 0);
    ClearJobCOForm();
}
function populateJobCOList() {
    var aCOList = transformCOList(JobNS.COs);
    createCOListTable(aCOList);
}
function ClearJobCOForm() {
    $("#job-co-id").val(0);
    $("#coid").val("Auto Generated");
    $("#desc").val("");
    $("#codate").val("");
    //$("#cocustomerrep").val("");
    $("#cocustomerreps").populateDropDownList(JobCONS.reps, 0);
    $("#cucustomerrepemail").val("");
    $("#cototalCostDisplay").val("0");
    $("#cototalCost").val("0");
    $("#co-items").val("");
    JobCONS.estimateItems = [];
    JobCONS.originalEstimateItems = [];
    var aEstimateItems = transformCOEstimateItems(JobCONS.estimateItems);
    createCOEstimateItemTable(aEstimateItems);

}
function populateJobCOForm(data) {
    $("#job-co-id").val(data.JobCO.COID);
    $("#coid").val(data.JobCO.COCode);
    $("#desc").val(data.JobCO.Description);
    //$("#cocustomerrep").val(data.JobCO.SalesRep);
    $("#cocustomerreps").populateDropDownList(JobCONS.reps, data.JobCO.SalesRepID);
    $("#cucustomerrepemail").val(data.JobCO.SalesRepEmail);
    var parsedDate = new Date(parseInt(data.JobCO.Date.substr(6)));
    var jsDate = new Date(parsedDate); //Date object
    $("#codate").val(jsDate.toLocaleDateString());
    $("#job-CO #cototalCost").val(data.JobCO.TotalAmount.toFixed(2));
    $("#job-CO #cototalCostDisplay").val(Number(data.JobCO.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US'));
}
function populateEstimateItems(estimateID) {

}
function saveJobCO() {
    var coId = $('#coid').val();
    var desc = $('#desc').val();
    var date = $('#codate').val();
    var customerepid = $('#cocustomerreps').val();
    var cucustomerrepemail = $('#cucustomerrepemail').val();
    ClearDivMessages();
    if (coId == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please enter CO Id.');
        return;
    }
    else if (date == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select date.');
        return;
    }

    var jobData = {
    };

    var jobco = {
        COID: parseInt($("#job-co-id").val()),
        JobID: parseInt($("#job-id").val()),
        COCode: coId,
        Description: desc,
        Date: date,
        SalesRepID: customerepid,
        SalesRepEmail: cucustomerrepemail,
        TotalAmount: $('#cototalCost').val(),
        Active: true
    }

    jobData.JobCO = jobco;
    jobData.COEstimateItems = JobCONS.estimateItems;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobCO',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                JobNS.COs = data.COs;
                populateJobCOList();
                cancelJobCO();
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}
function transformCOEstimateItems(estimateItems) {
    return estimateItems.map(function (item) {
        var itemAssembly = {};

        //check if item or assembly to be added 
        var unit = "";
        if (item.ItemAssemblyType == 0) {
            itemAssembly = searchObject(item.ItemAssemblyID, JobCONS.items);
            unit = itemAssembly.Unit;
        }
        else {
            itemAssembly = searchObject(item.ItemAssemblyID, JobCONS.assemblies);
        }
        if (itemAssembly.Code == undefined)
            return;
        var code = itemAssembly.Code;
        var name = itemAssembly.Name;
        var description = itemAssembly.Description;
        var price = item.Price;
        return [
           "<span data-val = " + item.ItemAssemblyID + " data-item-type = " + item.ItemAssemblyType + " class='name'>" + code + "</span>",
           "<span>"+ name +"</span>",
           "<input type='text' maxlength='8' class='qty' width='100px' onkeypress='enterOnlyNumericForQty(this, event)' value = " + item.Quantity + ">",
           "<input type='text' maxlength='8' class='price' width='100px' onkeypress='enterOnlyNumeric(this, event)' value = " + price + ">",
           //"<span class='price'>" + Number(price.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='cost'>" + Number(item.Cost.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           //"<span class='delete-cust-addr update' data-val=" + item['ID'] + ">Update</span>",
           "<span class='delete-cust-addr delete' data-val=" + item['ID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}
function createCOEstimateItemTable(aEstimateItems) {
    $('#grid-items-assembly-co').dataTable().fnDestroy();

    $('#grid-items-assembly-co').dataTable({
        "bPaginate": true,
        "iDisplayLength": 5,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "bAutoWidth": false,
        "aaData": aEstimateItems
    });
}

$(document).on("click", "#grid-items-assembly-co .delete", function () {
    var tr = $(this).parent().parent();

    var itemToUpdate = {
        ItemAssemblyID: $(tr).find(".name").attr("data-val"),
        ItemAssemblyType: $(tr).find(".name").attr("data-item-type")
    }

    var tmp = [];
    for (var i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i];

        if (!(it.ItemAssemblyID == itemToUpdate.ItemAssemblyID
                && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType)) {
            tmp.push(it)
        }
    }

    JobCONS.estimateItems = tmp;

    var cototalCost = 0;
    for (i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i]
        cototalCost = cototalCost + parseFloat(it.Cost);
    }

    $("#job-CO #cototalCost").val(cototalCost.toFixed(2));
    $("#job-CO #cototalCostDisplay").val(Number(cototalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

    var aEstimateItems = transformCOEstimateItems(JobCONS.estimateItems);
    createCOEstimateItemTable(aEstimateItems);
});
$(document).on("blur", "#grid-items-assembly-co .qty", function () {
    var tr = $(this).parents("tr")[0];
    if ($(tr).find(".qty").val() == "") {
        $(tr).find(".qty").val("0");
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
    }

    for (i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType) {
            JobCONS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var cototalCost = 0;
    for (i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i]
        cototalCost = cototalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#job-CO #cototalCost").val(cototalCost.toFixed(2));
    $("#job-CO #cototalCostDisplay").val(Number(cototalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
$(document).on("blur", "#grid-items-assembly-co .price", function () {
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
    }

    for (i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i]
        if (it.ItemAssemblyID == itemToUpdate.ItemAssemblyID && it.ItemAssemblyType == itemToUpdate.ItemAssemblyType) {
            JobCONS.estimateItems[i] = itemToUpdate;
        }
    }

    // Calculate Total Cost and Contract Price
    var cototalCost = 0;
    for (i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i]
        cototalCost = cototalCost + parseFloat(it.Cost);
    }

    // Update Total Cost 
    $("#job-CO #cototalCost").val(cototalCost.toFixed(2));
    $("#job-CO #cototalCostDisplay").val(Number(cototalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));
});
function transformCOList(poList) {
    return poList.map(function (item) {

        var parsedDate = new Date(parseInt(item.Date.substr(6)));
        var jsDate = new Date(parsedDate); //Date object


        return [
           "<span data-val = " + item.COID + ">" + item.COCode + "</span>",
           "<span>" + item.Description + "</span>",
           "<span>" + jsDate.toLocaleDateString() + "</span>",
           "<span>" + Number(item.TotalAmount.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='edit-CO' data-val=" + item.COID + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>",
           "<span class='delete-CO' data-val=" + item.COID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>",
           "<span class='generate-CO' data-val=" + item.COID + "><ul><li class='download'><a href='#non'>Generate CO</a></li></ul></span>"
        ];
    });
}
function createCOListTable(aCOList) {
    $('#grid-items-co').dataTable().fnDestroy();

    $('#grid-items-co').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 1, 4, 5, 6] },
            { className: "dt-right", targets: [2, 3] },
            { defaultContent: "", render: $.fn.dataTable.render.number(',', '.', 2), targets: [3] }
        ],
        "bAutoWidth": false,
        "aaData": aCOList
    });
}
$(document).on("click", "#grid-items-co .delete-CO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Job/ToggleJobCOStatus',
                type: 'POST',
                data: { coID: id, active: false, __RequestVerificationToken: token },
                success: function (data) {
                    var index = searchCOObjectIndex(id, JobNS.COs);
                    if (index != null)
                        JobNS.COs.splice(index, 1);
                    populateJobCOList();
                    closeModal();
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});
$(document).on("click", "#grid-items-co .edit-CO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobCODetail',
        type: 'POST',
        data: { coID: id, __RequestVerificationToken: token },
        success: function (data) {
            addJobCO();
            populateJobCOForm(data);
            JobCONS.assemblies = data.keyValue.assemblies;
            JobCONS.items = data.keyValue.items;
            //JobCONS.originalEstimateItems = data.keyValue.originalEstimateItems;
            JobCONS.estimateItems = data.estimateItems;
            var aEstimateItems = transformCOEstimateItems(JobCONS.estimateItems);
            createCOEstimateItemTable(aEstimateItems);
            closeModal();
        }
    });
});
$(document).on("click", "#grid-items-co .generate-CO", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    window.location = getWebsiteBaseUrl() + 'Job/CreateCODocument?coID=' + id;
});
function searchCOObjectIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].COID == nameKey) {
            return i;
        }
    }
    return null;
}
function AddCOItem() {
    var coitems = $('#co-items option:selected').val();
    if (coitems == "0") {
        ClearDivMessages();
        $('.alert-danger').show();
        if (type == "0") {
            $('.alert-danger').find('#dvError').append('Please select item.');
        }
        return;
    }
    var type = new String(coitems).split(',')[1];
    var id = new String(coitems).split(',')[0];
    if (type == "0") {
        itemAssembly = searchObject(id, JobCONS.items);
    }
    else {
        itemAssembly = searchObject(id, JobCONS.assemblies);
    }

    var itemAssm = {
        ID: 0,
        COID: $('#coid').val(),
        ItemAssemblyID: itemAssembly.ID,
        ItemAssemblyType: type,
        Price: itemAssembly.Price,
        Quantity: 0,
        Cost: 0
    };

    var exist = false;
    for (var i = 0; i < JobCONS.estimateItems.length; i++) {
        var it = JobCONS.estimateItems[i];

        if ((it.ItemAssemblyID == itemAssm.ItemAssemblyID
                && it.ItemAssemblyType == itemAssm.ItemAssemblyType)) {
            exist = true;
            break;
        }
    }

    if (exist == true) {
        ClearDivMessages();
        $('.alert-danger').show();
        if (type == "0") {
            $('.alert-danger').find('#dvError').append('Selected item already added.');
        }
        else {
            $('.alert-danger').find('#dvError').append('Selected assembly already added.');
        }
        return;
    }

    if (exist == false) {
        JobCONS.estimateItems.push(itemAssm);

        var cototalCost = 0;
        for (i = 0; i < JobCONS.estimateItems.length; i++) {
            var it = JobCONS.estimateItems[i]
            cototalCost = cototalCost + parseFloat(it.Cost);
        }

        $("#job-CO #cototalCost").val(cototalCost.toFixed(2));
        $("#job-CO #cototalCostDisplay").val(Number(cototalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

        var aEstimateItems = transformCOEstimateItems(JobCONS.estimateItems);
        createCOEstimateItemTable(aEstimateItems);
    }
}
function LoadItemAsseblies(items) {
        $("#co-items").populateDropDownList(items, 0);
        //var aaData = items;
        //var req = [];
        //for (var i = 0; i < aaData.length; i++) {
        //    var item = aaData[i];
        //    var it = {
        //        label: item.Name,
        //        value: item.Name,
        //        id: item.Id,
        //        code: item.Code,
        //        name: item.Name,
        //        Type: item.Type,
        //        Qty: 0,
        //        Cost: item.Price,
        //        Amount: item.Amount
        //    };
        //    req.push(it);
        //}

        //$('#search-item-assembly').autocomplete({
        //    source: req,
        //    select: function (event, ui) {
        //        var item = {
        //            ID: ui.item.id,
        //            Name: ui.item.name,
        //            Type: ui.item.Type,
        //        }

        //        if (item.Type == "0")
        //        {
        //            itemAssembly = searchObject(item.ID, JobCONS.items);
        //        }
        //        else
        //        {
        //            itemAssembly = searchObject(item.ID, JobCONS.assemblies);
        //        }
            
        //        var itemAssm = {
        //            ID : 0,
        //            COID : $('#coid').val(),
        //            ItemAssemblyID: itemAssembly.ID,
        //            ItemAssemblyType: item.Type,
        //            Price: itemAssembly.Price,
        //            Quantity: 0,
        //            Cost: 0
        //        };

        //        var exist = false;
        //        for (var i = 0; i < JobCONS.estimateItems.length; i++) {
        //            var it = JobCONS.estimateItems[i];

        //            if ((it.ItemAssemblyID == itemAssm.ItemAssemblyID
        //                    && it.ItemAssemblyType == itemAssm.ItemAssemblyType)) {
        //                exist = true;
        //                break;
        //            }
        //        }

        //        if (exist == true)
        //        {
        //            ClearDivMessages();
        //            $('.alert-danger').show();
        //            if (item.Type == "0") {
        //                $('.alert-danger').find('#dvError').append('Selected item already added.');
        //            }
        //            else {
        //                $('.alert-danger').find('#dvError').append('Selected assembly already added.');
        //            }
        //            return;
        //        }

        //        if (exist == false) {
        //            JobCONS.estimateItems.push(itemAssm);

        //            var cototalCost = 0;
        //            for (i = 0; i < JobCONS.estimateItems.length; i++) {
        //                var it = JobCONS.estimateItems[i]
        //                cototalCost = cototalCost + parseFloat(it.Cost);
        //            }

        //            $("#job-CO #cototalCost").val(cototalCost.toFixed(2));
        //            $("#job-CO #cototalCostDisplay").val(Number(cototalCost.toFixed(2)).toFixed(2).toLocaleString('en-US'));

        //            var aEstimateItems = transformCOEstimateItems(JobCONS.estimateItems);
        //            createCOEstimateItemTable(aEstimateItems);
        //        }
        //    }
        //});

        //$('#search-item-assembly').on("blur click", function () { $(this).val("") });

    }
