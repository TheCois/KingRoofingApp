// Global object for respective modules
$(document).ready(function () {
    $("#save-job-assignment").on("click", saveJobAssignment);
    $("#add-assignment").on("click", addAssignment);
    $("#averagehour").on("change", JobStartDateAndAvgWorkingHourChange);
    $("#crew-employees").on("change", crewEmployeeChange);
    JobNS.jobAssigments = [];
    $(document).on("click", "#grid-items-assignment .delete-crewemp", function () {
        var objectPKID = $(this).attr("data-val1");
        var type = $(this).attr("data-val2");
        var jobAssigments = {};
        var index = searchObjectAssignmentIndex(objectPKID, type, JobNS.jobAssigments);
        if (index != null)
            JobNS.jobAssigments.splice(index, 1);
            var oTableData = transformCrewEmp(JobNS.jobAssigments);
            createCrewEmpTable(oTableData);
            CalculateEstimatedLaborHoursAndJobEndDate();
    })
});
function crewEmployeeChange() {
    var val = $('#crew-employees').val();
    if (val != "0") {
        var id = (new String(val)).split('~')[0];
        var type = (new String(val)).split('~')[1];
        $("#crewempID").val(id);
        $("#crewempType").val(type);
    } else {
        $("#crewempID").val("0");
        $("#crewempType").val("0");
    }
}
function populateJobAssignmentInformation(data)
{
    if (data.job.StartDate != null) {
        var parsedStartDate = new Date(parseInt(data.job.StartDate.substr(6)));        var jsStartDate = new Date(parsedStartDate); //Date object        $('#jobstartdate').val(jsStartDate.toLocaleDateString());
    }    if (data.job.EndDate != null) {
        var parsedEndDate = new Date(parseInt(data.job.EndDate.substr(6)));        var jsEndDate = new Date(parsedEndDate); //Date object        $('#jobenddate').val(jsEndDate.toLocaleDateString());
    }
    $('#labourhour').val(data.job.EstimatedLabourHours);
    $('#averagehour').val(data.job.AverageWorkingHours);

    JobNS.jobAssigments = data.keyValue.jobAssigments;
    var oTableData = transformCrewEmp(data.keyValue.jobAssigments);
    createCrewEmpTable(oTableData);
}
function searchObjectAssignmentIndex(nameKey, type, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].ObjectPKID == nameKey && myArray[i].Type == type) {
            return i;
        }
    }
    return null;
}
function addAssignment()
{
    ClearDivMessages();
    //var crewemp = $("#search-crew-employee").val();
    var crewemp = $('#crew-employees option:selected').val();
    var fromdate = $('#fromdate').val();
    var todate = $('#todate').val();
    $('.alert-danger').find('#dvError').html("");
    $('.alert-danger').hide();
    if (crewemp == "0")
    {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select crew/employee.');
        return;
    }
    if (fromdate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select from date.');
        return;
    }
    else if (todate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select to date.');
        return;
    }
    if (new Date(todate) < new Date(fromdate)) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('From date must be less than to date.');
        return;
    }
    CrewEmpItem = {
        ObjectPKID: $('#crewempID').val(),
        Name: $('#crew-employees option:selected').text(),
        FromDate: fromdate,
        ToDate: todate,
        Type: $('#crewempType').val()
    };
    var itemExists = false;
    for (i = 0; i < JobNS.jobAssigments.length; i++) {
        var it = JobNS.jobAssigments[i]
        if (it.ObjectPKID == $('#crewempID').val() && it.Type == $('#crewempType').val()) {
            itemExists = true;
        }
    }
    if (!itemExists) {
        JobNS.jobAssigments.push(CrewEmpItem);
        var oTableData = transformCrewEmp(JobNS.jobAssigments);
        createCrewEmpTable(oTableData);
        CalculateEstimatedLaborHoursAndJobEndDate();
    }
    else
    {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Selected crew/employee already added.');
        return;
    }
    
}
function saveJobAssignment()
{
    var jobstartdate = $('#jobstartdate').val();
    var jobenddate = $('#jobenddate').val();
    var avgworkinghours = $('#averagehour').val();
    $('.alert-danger').hide();
    $('.alert-danger').find('#dvError').html("");
    if(jobstartdate == "")
    {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select job start date.');
        return;
    }
    else if (avgworkinghours == "" || (avgworkinghours != "" && Number(avgworkinghours) == 0))
    {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please enter average working hours.');
        return;
    }
    if (jobenddate != "") {
        if (new Date(jobenddate) < new Date(jobstartdate)) {
            $('.alert-danger').show();
            $('.alert-danger').find('#dvError').html('Job start date must be less than job end date.');
            return;
        }
    }
    var job = createJobObject();
    job.StartDate = jobstartdate,
    job.EndDate = jobenddate;
    job.EstimatedLabourHours = parseFloat($("#labourhour").val() == "" ? "0" : $("#labourhour").val());
    job.AverageWorkingHours = parseFloat($("#averagehour").val() == "" ? "0" : $("#averagehour").val());

    var jobData = {
    };

    jobData.Job = job;
    jobData.JobAssignments = JobNS.jobAssigments;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobAssignment',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                //window.location.href = $.urls.indexUrl;
                $('.alert-success').show();
                $('.alert-success').find('#dvSuccess').text(data.message);
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}

function LoadCrewEmployees(items)
{
    $("#crew-employees").populateDropDownList(items, 0);
    //var aaData = items;
    //var req = [];
    //for (var i = 0; i < aaData.length; i++) {
    //    var item = aaData[i];
    //    var it = {
    //        label: item.Name,
    //        value: item.Name,
    //        id: item.Id,
    //        name: item.Name,
    //        CrewEmpID: item.Id,
    //        Type: item.Type
    //    };
    //    req.push(it);
    //}

    //$('#search-crew-employee').autocomplete({
    //    source: req,
    //    select: function (event, ui) {
    //        var item = {
    //            ID: 0,
    //            CrewEmpID: ui.item.CrewEmpID,
    //            Name: ui.item.name,
    //            Type: ui.item.Type
    //        }
    //        $('#crewempID').val(ui.item.CrewEmpID);
    //        $('#crewempType').val(ui.item.Type);
    //    }
    //});

    //$('#search-crew-employee').on("blur click", function () { $(this).val("") });

}

function transformCrewEmp(aItemList) {
    return aItemList.map(function (item) {
        var fromDate = item.FromDate;
        var toDate = item.ToDate;
        return [
               "<span data-val = " + item.ObjectPKID + " class='name'>" + item['Name'] + "</span>",
               "<span>" + item['FromDate'].replace(/ /g, '') + "</span>",
               "<span>" + item['ToDate'].replace(/ /g, '') + "</span>",
               "<span class='delete-crewemp delete' data-val2="+ item["Type"] +" data-val1=" + item['ObjectPKID'] + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
        ];
    });
}
function createCrewEmpTable(oTableData)
{
    $('#grid-items-assignment').dataTable().fnDestroy();

    $('#grid-items-assignment').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false,
        "bSort": false,
        "columnDefs": [
            { className: "dt-left", targets: [0, 3] },
            { className: "dt-right", targets: [1, 2] }
        ],
        "bAutoWidth": false,
        "aaData": oTableData
    });
}
function JobStartDateAndAvgWorkingHourChange()
{
    if (Number($('#averagehour').val()) > 24)
    {
        $('#averagehour').val("0");
    }
    else
        CalculateEstimatedLaborHoursAndJobEndDate();
}
function CalculateEstimatedLaborHoursAndJobEndDate()
{
    if ($('#jobstartdate').val() != "" && $('#averagehour').val() != "" && (JobNS.jobAssigments != null && JobNS.jobAssigments.length > 0))
    {
        var jobStartDate = $('#jobstartdate').val();
        var averageWorkingHour = $('#averagehour').val();
        var jobID = $("#job-id").val();
        openModal();
        $.ajax({
            url: getWebsiteBaseUrl() + 'Job/CalculateEstimatedLaborHoursAndJobEndDate',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ jobID: jobID, jobStartDate: jobStartDate, averageWorkingHour: averageWorkingHour, jobAssigments: JobNS.jobAssigments }),
            success: function (data) {
                if (data.result) {
                    $("#labourhour").val(data.estimatedHour);
                    $('#jobenddate').val(data.jobEndDate);
                }
                else {
                    $('.alert-danger').show();
                    $('.alert-danger').find('#dvError').text("Error occured in calculating estimated hours. Please try again later.");
                }
                closeModal();
            }
        });
    }
    else
    {
        $('#labourhour').val("0");
        $('#jobenddate').val("");
    }
}
