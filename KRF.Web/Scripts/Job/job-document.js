$(document).ready(function () {
    $("#save-job-doc").on("click", saveJobDocument);
});
function GetJobDocumentList(ID) {
    openModal();    $("#jobID").val(ID);    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobDocumentList',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            closeModal();
            JobNS.documents = data.keyValue.documents;
            if (ID == 0) {
                //this section is not visibile in add mode
            } else {
                populateJobDocumentList();
                emptyJobDocumentForm(data);
            }
        },
        error: function (data) {
            closeModal();
        }
    });
}

function emptyJobDocumentForm(data) {
    $("#documenttypes").populateDropDownList(data.keyValue.documentTypes, 0);
    ClearJobDocumentForm();
}
function populateJobDocumentList() {
    var aDocumentList = transformDocumentList(JobNS.documents);
    createDocumentListTable(aDocumentList);
}
function ClearJobDocumentForm() {
    $("#job-document #file").val("");
    $("#job-document #uploaddatetime").val("");
}
function saveJobDocument() {
    var file = $('#file').val();
    var documenttypes = $('#documenttypes').val();
    ClearDivMessages();
    if (file == "") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select document to upload.');
        return;
    }
    else if (documenttypes == "0") {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').html('Please select document type.');
        return;
    }
    var formData = new FormData();
    formData.append('file', $('#job-document input[type=file]')[0].files[0]);
    formData.append('documentTypes', documenttypes);
    formData.append('jobID', $("#jobID").val());
    formData.append('id', $("#id").val());

    openModal();
    $.ajax({
        type: "POST",
        url: getWebsiteBaseUrl() + 'Job/Upload',
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            GetJobDocumentList($("#jobID").val());
            ClearJobDocumentForm();
            $('.alert-success').show();
            $('.alert-success').find('#dvSuccess').text("Document uploaded successfully.");
            closeModal();
        },
        error: function () {
            closeModal();
            alert("failure");
        }
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
function transformDocumentList(documentList) {
    return documentList.map(function (item) {
        return [
           "<span data-val = " + item.ID + ">" + item.Name + "</span>",
           "<span>" + item.TypeName + "</span>",
           "<span>" + item.UploadDateTime + "</span>",
           "<span class='download-document' data-val=" + item.ID + "><ul><li class='download'><a href='#non'>Download</a></li></ul></span>",
           "<span class='delete-document' data-val=" + item.ID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>",
        ];
    });
}
function createDocumentListTable(aDocumentList) {
    $('#grid-job-docs').dataTable().fnDestroy();

    $('#grid-job-docs').dataTable({
        "bPaginate": true,
        "iDisplayLength": 10,
        "bLengthChange": false,
        "bFilter": true,
        "bInfo": false,
        "bSort": true,
        "columnDefs": [
            { className: "dt-left", targets: [0, 1, 3, 4] },
            { className: "dt-right", targets: [2] }
        ],
        "bAutoWidth": false,
        "aaData": aDocumentList
    });
}
$(document).on("click", "#grid-job-docs .delete-document", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    ClearDivMessages();
    BootstrapDialog.confirm('Are you sure you want to delete?', function (result) {
        if (result) {
            openModal();
            $.ajax({
                url: getWebsiteBaseUrl() + 'Job/DeleteJobDocument',
                type: 'POST',
                data: { jobDocumentID: id, active: false, __RequestVerificationToken: token },
                success: function (data) {
                    var index = searchJobDocumentIndex(id, JobNS.documents);
                    if (index != null) {
                        JobNS.documents.splice(index, 1);
                        populateJobDocumentList();
                        $('.alert-success').show();
                        $('.alert-success').find('#dvSuccess').text("Document deleted successfully.");
                    }
                    closeModal();
                }
            });
        } else {
            //alert('Nope.');
        }
    });
});
$(document).on("click", "#grid-job-docs .download-document", function () {
    var token = $('[name=__RequestVerificationToken]').val();
    var id = $(this).attr("data-val");
    window.location = getWebsiteBaseUrl() + 'Job/Download?id=' + id;
});
function searchJobDocumentIndex(nameKey, myArray) {
    for (var i = 0; i < myArray.length; i++) {
        if (myArray[i].ID == nameKey) {
            return i;
        }
    }
    return null;
}