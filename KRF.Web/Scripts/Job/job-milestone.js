// Global object for respective modules
$(document).ready(function () {
    $("#save-job-milestone").on("click", saveJobMilestone);
    $("#cancel-job-milestone").on("click", cancelJobMilestone);
    //JobNS.tasks = [];
    $(document).on("click", "#grid-items-milestone .edit-task", function () {
        var taskID = $(this).attr("data-val");
        var task = searchObjectTaskByIDIndex(taskID, JobNS.tasks);
        DisplayTaskData(task);
    })
    $(document).on("click", "#grid-items-milestone .complete-task", function () {
        var taskID = $(this).attr("data-val");
        var isCompleted = $(this).is(":checked");
        UpdateTaskStatus(taskID, isCompleted);
    })
});
function GetJobTaskList(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobTaskList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            populateJobTaksInformation(data.tasks);
            closeModal();
        }
    });
}
function ClearDivMessages()
{
    $('.alert-danger').find('#dvError').html("");
    $('.alert-danger').hide();
    $('.alert-success').find('#dvSuccess').text("");
    $('.alert-success').hide();
}
function UpdateTaskStatus(taskID, isCompleted)
{
    ClearDivMessages();
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/UpdateTaskStatus',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ taskID: taskID, isCompleted: isCompleted }),
        success: function (data) {
            if (data.hasSaved) {
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
function DisplayTaskData(task) {
    var date;
    $('#taskID').val(task.TaskID)
    $("#taskname").val(task.TaskName);
    if (task.InspectionScheduledDate != null && task.InspectionScheduledDate != undefined) {
        date = new Date(parseInt(task.InspectionScheduledDate.substr(6)));
        $('#inspectionscheduled').val(date.toLocaleDateString());
    }
    else {
        $('#inspectionscheduled').val("");
    }
    if (task.InspectionCompletedDate != null && task.InspectionCompletedDate != undefined) {
        date = new Date(parseInt(task.InspectionCompletedDate.substr(6)));
        $('#inspectioncompleted').val(date.toLocaleDateString());
    }
    else {
        $('#inspectioncompleted').val("");
    }
    $('#inspscheduleconfirmation').val(task.InspectionScheduledConfirmationNo);
    $('#inspcompletedconfirmation').val(task.InspectionCompletedConfirmationNo);
    if (task.TaskStartDate != null && task.TaskStartDate != undefined) {
        date = new Date(parseInt(task.TaskStartDate.substr(6)));
        $('#taskstartdate').val(date.toLocaleDateString());
    }
    else {
        $('#taskstartdate').val("");
    }
    if (task.TaskCompletedDate != null && task.TaskCompletedDate != undefined) {
        date = new Date(parseInt(task.TaskCompletedDate.substr(6)));
        $('#taskcompleteddate').val(date.toLocaleDateString());
    }
    else {
        $('#taskcompleteddate').val("");
    }
    $('#inspectionnote').val(task.InspectionNotes);
 }
function cancelJobMilestone() {
    $('#taskID').val("0")
    $("#taskname").val("");
    $('#inspectionscheduled').val("");
    $('#inspectioncompleted').val("");
    $('#inspscheduleconfirmation').val("");
    $('#inspcompletedconfirmation').val("");
    $('#taskstartdate').val("");
    $('#taskcompleteddate').val("");
    $('#inspectionnote').val("");
    ClearDivMessages();
}
function populateJobTaksInformation(tasks) {
    JobNS.tasks = tasks;
    var oTableData = transformTaskTable(JobNS.tasks);
    createTaskTable(oTableData);
    $("#taskname").focus();
}
function searchObjectTaskByIDIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].TaskID == nameKey) {
            return myArray[i];
        }
    }
    return null;
}
function saveJobMilestone() {
    var taskID = $('#taskID').val();
    var taskName = $("#taskname").val();
    var inspScheduled = $('#inspectionscheduled').val();
    var inspCompleted = $('#inspectioncompleted').val();
    var inspSchConfirmation = $('#inspscheduleconfirmation').val();
    var inspCompConfirmation = $('#inspcompletedconfirmation').val();
    var startDate = $('#taskstartdate').val();
    var completedDate = $('#taskcompleteddate').val();
    var inspNotes = $('#inspectionnote').val();

    ClearDivMessages();

    if (taskName == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please enter task name.');
        return;
    }
    //if (inspScheduled == "") {
    //    $('.alert-danger').show();
    //    $('.alert-danger').find('#dvError').html('Please select inspection scheduled date.');
    //    return;
    //}
    if (startDate == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select start date.');
        return;
    }
    if (inspScheduled != "" && inspCompleted != "") {
        if (new Date(inspCompleted) < new Date(inspScheduled)) {
            $('.alert-danger').show();
            $('.alert-danger').find('#dvError').html('Inspection scheduled date must be less than inspection completed date.');
            return;
        }
    }
    if (startDate != "" && completedDate != "") {
        if (new Date(completedDate) < new Date(startDate)) {
            $('.alert-danger').show();
            $('.alert-danger').find('#dvError').html('Start date must be less than completed date.');
            return;
        }
    }
    TaskItem = {
        TaskID: taskID,
        JobID: $("#job-id").val(),
        TaskName: taskName,
        InspectionScheduledDate: inspScheduled,
        InspectionCompletedDate: inspCompleted,
        InspectionScheduledConfirmationNo: inspSchConfirmation,
        InspectionCompletedConfirmationNo: inspCompConfirmation,
        TaskStartDate: startDate,
        TaskCompletedDate: completedDate,
        InspectionNotes: inspNotes,
        IsCompleted: false
    };
    var itemExists = false;
    for (i = 0; i < JobNS.tasks.length; i++) {
        var it = JobNS.tasks[i]
        if (it.TaskName == taskName && it.TaskID != parseInt(taskID)) {
            itemExists = true;
        }
    }
    if (itemExists) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Task name already added.');
        return;
    }
    else {
        //JobNS.tasks.push(TaskItem);
        //var oTableData = transformTaskTable(JobNS.tasks);
        //createTaskTable(oTableData);
    }

    var jobData = {
    };

    jobData.JobTask = TaskItem;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobMilestone',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                //window.location.href = $.urls.indexUrl;
                cancelJobMilestone();
                $('.alert-success').show();
                $('.alert-success').find('#dvSuccess').text(data.message);
                populateJobTaksInformation(data.tasks);
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });

}

function transformTaskTable(aItemList) {
    return aItemList.map(function (item) {
        var completedText = "";
        var tastStartDate = new Date(parseInt(item.TaskStartDate.substr(6)));
        if (item.TaskCompletedDate != null) {
            var completedDate = new Date(parseInt(item.TaskCompletedDate.substr(6)));
            completedText = completedDate.toLocaleDateString();
        }
        return [
               "<span data-val = " + item.TaskID + " class='name'>" + item['TaskName'] + "</span>",
               "<span>" + tastStartDate.toLocaleDateString() + "</span>",
               "<span>" + completedText + "</span>",
               //"<input type='checkbox' class='complete-task' data-val=" + item['TaskID'] + " " + (item['IsCompleted'] == true ? "checked" : "") + "></input>",
               "<span class='edit-task' data-val=" + item["TaskID"] + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>"
        ];
    });
}
function createTaskTable(oTableData) {
    $('#grid-items-milestone').dataTable().fnDestroy();

    $('#grid-items-milestone').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": false,
        "bAutoWidth": false,
        "aaData": oTableData
    });
}
