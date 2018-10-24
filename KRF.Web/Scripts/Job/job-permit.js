var JobPermitNS = {
    //originalEstimateItems: [],
    employees: [],
    inspections: {},
    permitstatus: {},
    permit: {},
    permits: {}
};// End of Global objects
$(document).ready(function () {
    $("#save-permit").on("click", saveJobPermit);
    $("#cancel-permit").on("click", cancelJobPermit);
    $("#cancel-permit").hide();
});
function GetJobInspectionList(ID) {
    openModal();
    cancelJobPermit();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobInspectionList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.Permits = data.keyValue.Permits;
            if (ID == 0) {
                emptyJobPermitForm(data);
            } else {
                populateJobPermitList();
                emptyJobPermitForm(data);
            }
            closeModal();
        }
    });
}
function cancelJobPermit() {
    ClearDivMessages();
    ClearJobPermitForm();
    //$('#dvWOAdd').show();
    //$('#dvWOList').show();
    //$('#dvWOAdd').hide();
}
function getJobInspection(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobPermit',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.Permits = data.keyValue.Permits;
            closeModal();
        }
    });
}
function emptyJobPermitForm(data) {
    $("#employees").populateDropDownList(data.keyValue.employees, 0);
    $("#permits").populateDropDownList(data.keyValue.permits, 0);
    $("#inspections").populateDropDownList(data.keyValue.permitInspections, 0);
    $("#status").populateDropDownList(data.keyValue.permitStatus, 0);
    $("#inspectors").populateDropDownList(data.keyValue.employees, 0);
    $("#jobnumber").val(data.keyValue.job.Id);
    $("#jobaddress").val(data.keyValue.jobAddress);
    ClearJobPermitForm();
}
function populateJobPermitList() {
    var permitList = transformPermitList(JobNS.Permits);
    createPermitListTable(permitList);
}
function ClearJobPermitForm() {
    $("#jobnumber").focus();
    $("#job-insp-id").val(0);
    $("#calleddate").val("");
    $("#resultdate").val("");
    $("#employees").prop("selectedIndex", 0);
    $("#permits").prop("selectedIndex", 0);
    $("#inspections").prop("selectedIndex", 0);
    $("#status").prop("selectedIndex", 0);
    $("#inspectors").prop("selectedIndex", 0);
    $('#cancel-permit').hide();
}
function populateJobPermitForm(data) {
    $("#employees").populateDropDownList(data.keyValue.employees, data.JobInspection.EmployeeID);
    $("#permits").populateDropDownList(data.keyValue.permits, data.JobInspection.PermitID);
    $("#inspections").populateDropDownList(data.keyValue.permitInspections, data.JobInspection.InspectionID);
    $("#status").populateDropDownList(data.keyValue.permitStatus, data.JobInspection.StatusID);
    $("#inspectors").populateDropDownList(data.keyValue.employees, data.JobInspection.InspectorID);
    $("#job-insp-id").val(data.JobInspection.InspID);
    var parsedDate = new Date(parseInt(data.JobInspection.CalledDate.substr(6)));    var jsDate = new Date(parsedDate); //Date object
    $("#calleddate").val(jsDate.toLocaleDateString());
    if (data.JobInspection.ResultDate != null && data.JobInspection.ResultDate != undefined) {
        var parsedDate = new Date(parseInt(data.JobInspection.ResultDate.substr(6)));        var jsDate = new Date(parsedDate); //Date object
        $("#resultdate").val(jsDate.toLocaleDateString());
    }
    else
    {
        $("#resultdate").val("");
    }
}
function saveJobPermit() {
    var employeeID = $('#employees option:selected').val();
    var permitID = $('#permits option:selected').val();
    var inspectionID = $('#inspections option:selected').val();
    var statusID = $('#status option:selected').val();
    var inspectorID = $('#inspectors option:selected').val();
    var calledDate = $('#calleddate').val();
    var resultDate = $('#resultdate').val();
    ClearDivMessages();
    if (employeeID == "0") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select employee name.');
        return;
    }
    else if (permitID == "0") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select permit.');
        return;
    }
    else if (inspectorID == "0") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select inspector name.');
        return;
    }
    else if (inspectionID == "0") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select inspection.');
        return;
    }
    else if (calledDate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select called date.');
        return;
    }
    if (calledDate != "" && resultDate != "")
    {
        if (new Date(calledDate) > new Date(resultDate)) {
            $('.alert-danger').show();
            $('.alert-danger').find('#dvError').html('Called date must be less than result date.');
            return;
        }
    }
    var jobData = {
    };

    var JobInspection = {
        InspID: parseInt($("#job-insp-id").val()),
        JobID: parseInt($("#job-id").val()),
        JobAddress: $('#jobaddress').val(),
        EmployeeID: $('#employees option:selected').val(),
        PermitID: $('#permits option:selected').val(),
        Phone: $('#number').val(),
        CalledDate: calledDate,
        ResultDate: resultDate,
        InspectionID: $('#inspections option:selected').val(),
        StatusID: $('#status option:selected').val(),
        InspectorID: $('#inspectors option:selected').val(),
        Notes: $('#notes').val(),
        Active: true
    }

    jobData.JobInspection = JobInspection;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobInspection',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                JobNS.Permits = data.Permits;
                //$("#generate-WO").show();
                $("#job-insp-id").val(JobNS.Permits[0].InspID)
                populateJobPermitList();
                $('.alert-success').show();
                $('.alert-success').find('#dvSuccess').text("Permit detail saved successfully.");
                //cancelJobWO();
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
            ClearJobPermitForm();
        }
    });
}
function transformPermitItems(estimateItems) {
    estimateItems = estimateItems.sort(function (a, b) { return parseFloat(a.COID) - parseFloat(b.COID) });

    return estimateItems.map(function (item) {
        var itemAssembly = {};

        //check if item or assembly to be added 
        var unit = "";
        if (item.ItemAssemblyType == 0) {
            itemAssembly = searchObject(item.ItemAssemblyID, JobPermitNS.items);
            unit = itemAssembly.Unit;
        }
        else {
            itemAssembly = searchObject(item.ItemAssemblyID, JobPermitNS.assemblies);
        }

        var code = itemAssembly.Code;
        var name = itemAssembly.Name;
        var description = itemAssembly.Description;
        var price = itemAssembly.Price;
        var itemNames = item.ItemNames;

        return [
           "<span data-val = " + item.ItemAssemblyID + " data-item-type = " + item.ItemAssemblyType + " data-co-id = " + item.COID + " class='name'>" + code + "</span>",
           "<span>" + name + "</span>",
           "<span>" + itemNames + "<input type='hidden' class='itemname' value='" + itemNames + "'></span>",
           "<span class='budget'>" + Number(item.Budget.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<input type='text' maxlength='8' class='used' onkeypress='enterOnlyNumeric(this, event)' style='width:100px' value = " + item.Used + ">",
           "<span class='rate'>" + Number(item.Rate.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='balance'>" + Number(item.Balance.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           "<span class='amount'>" + Number(item.Amount.toFixed(2)).toFixed(2).toLocaleString('en-US') + "</span>",
           //"<span class='delete-cust-addr update' data-val=" + item['ID'] + ">Update</span>",
           "<span class='delete-cust-addr delete' data-val=" + item['ID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
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

function transformPermitList(permitList) {

    return permitList.map(function (item) {

        var parsedDate = new Date(parseInt(item.CalledDate.substr(6)));        var jsCalledDate = new Date(parsedDate); //Date object        var jsResultDate;        if (item.ResultDate != null) {
            parsedDate = new Date(parseInt(item.ResultDate.substr(6)));            jsResultDate = new Date(parsedDate); //Date object        }        var resultDate = '';        var jobaddress = '';        var phone = '';        var notes = '';        if (jsResultDate != undefined)        {
            resultDate = jsResultDate.toLocaleDateString();
        }        if (item.JobAddress != null && item.JobAddress != undefined)        {
            jobaddress = item.JobAddress;
        }        if (item.Phone != null && item.Phone != undefined) {
            phone = item.Phone;
        }        if (item.Notes != null && item.Notes != undefined) {
            notes = item.Notes;
        }        return [
           "<span data-val = " + item.InspID + ">" + item.JobID + "</span>",
           //"<span>" + jobaddress + "</span>",
           "<span>" + item.EmployeeName + "</span>",
           "<span>" + item.Permit + "</span>",
           //"<span>" + phone + "</span>",
           "<span>" + jsCalledDate.toLocaleDateString() + "</span>",
           "<span>" + item.Inspection + "</span>",
           "<span>" + item.Status + "</span>",
           "<span>" + resultDate + "</span>",
           "<span>" + item.InspectorName + "</span>",
           //"<span>" + notes + "</span>",
           "<span class='edit-permit' data-val=" + item.InspID + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>",
           "<span class='delete-permit' data-val=" + item.InspID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}
function createPermitListTable(permitList) {
    $('#grid-permits').dataTable().fnDestroy();

    $('#grid-permits').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 1, 2, 4, 5, 7, 8, 9] },
            { className: "dt-right", targets: [3, 6] }
        ],
        "bAutoWidth": false,
        "aaData": permitList
    });
}
$(document).on("click", "#grid-permits .delete-permit", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
});
$(document).on("click", "#grid-permits .edit-permit", function () {
    var id = $(this).attr("data-val");
    EditPermit(id);
    $('#cancel-permit').show();
});
$(document).on("click", "#grid-permits .delete-permit", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Job/ToggleJobInspectionStatus',
                type: 'POST',
                data: { inspID: id, active: false, __RequestVerificationToken: token },
                success: function (data) {
                    var index = searchPermitObjectIndex(id, JobNS.Permits);
                    if (index != null)
                        JobNS.Permits.splice(index, 1);
                    populateJobPermitList();
                    closeModal();
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});

function EditPermit(id) {
    var token = $('[name=__RequestVerificationToken]').val();
    var jobID = parseInt($("#job-id").val());
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobInspectionDetail',
        type: 'POST',
        data: { inspID: id, jobID: jobID, __RequestVerificationToken: token },
        success: function (data) {
            populateJobPermitForm(data);
            closeModal();
        }
    });
}

function searchPermitObjectIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].InspID == nameKey) {
            return i;
        }
    }
    return null;
}
