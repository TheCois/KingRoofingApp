// Global object for respective modules
var data;
var JobNS = {
    job: {},
    customers: {},
    leads: {},
    addresses: {},
    custoAddresses: {},
    cities: {},
    states: {},
    jobAssigments: {},
    crewEmps: {},
    tasks: {},
    estimates: {},
    POs: {},
    COs: {},
    itemAssemblies: {},
    documents: {},
    WOs: {},
    INVs: {},
    Permits: {}
};// End of Global objects
$(document).ready(function () {
    $("#job-information").on("change", "#customers", function () {
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        var addresses = $.grep(JobNS.addresses, function (v) { return valueSelected == v.CustomerID });
        $("#address").populateDropDownList(addresses, 0);
        $("#estimates").populateDropDownList([], 0);
        GetCustomerDetail(valueSelected);
    })
    $("#job-information").on("change", "#leads", function () {
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        GetBuildingInformaiton(valueSelected);
    })
    $("#job-information").on("change", "#address", function () {
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        var estimates = $.grep(JobNS.estimates, function (v) { return valueSelected == v.JobAddressID });
        $("#estimates").populateDropDownList(estimates, 0);
        GetJobAddress(valueSelected);
    })
    $("#save-job-information").on("click", saveJobInformation);
    $("#save-job-summary").on("click", saveJobSummary);
});
function GetCustomerDetail(customerID) {
    var contacts = $.grep(JobNS.customers, function (v) { return customerID == v.ID });
    ClearJobAddressInformation();
    if (contacts.length > 0) {
        $("#contact").val(contacts[0].Contact);
        $("#leads").populateDropDownList(JobNS.leads, contacts[0].LeadID);
        GetBuildingInformaiton(contacts[0].LeadID);
    }
    else {
        $("#contact").val("");
        ClearBuildingInformation();
    }
}
function GetBuildingInformaiton(leadID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetBuildingInformation',
        type: 'POST',
        data: { leadID: leadID },
        success: function (data) {
            populateBuildingInformation(data.buildingInformation);
            closeModal();
        }
    });
}
function GetJobAddress(jobAddressID) {
    var address = $.grep(JobNS.custoAddresses, function (v) { return jobAddressID == v.ID });
    if (address.length > 0) {
        var city = $.grep(JobNS.cities, function (v) { return address[0].City == v.ID });
        var state = $.grep(JobNS.states, function (v) { return address[0].State == v.ID });
        $('#address1').val(address[0].Address1);
        $('#address2').val(address[0].Address2);
        if (city.length > 0)
            $('#city').val(city[0].Description);
        if(state.length > 0)
            $('#state').val(state[0].Description);
        $('#zipcode').val(address[0].ZipCode);
    }
    else {
        ClearJobAddressInformation();
    }
}
function getJobInformation(ID) {
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobInformation',
        type: 'POST',
        data: { jobID: ID },
        success: function (data) {
            JobNS.customers = data.keyValue.customers;
            JobNS.leads = data.keyValue.leads;
            JobNS.custoAddresses = data.keyValue.custoAddresses;
            JobNS.addresses = data.keyValue.addresses;
            JobNS.cities = data.keyValue.cities;
            JobNS.states = data.keyValue.states;
            JobNS.crewEmps = data.keyValue.crewEmps;
            JobNS.itemAssemblies = data.keyValue.itemAssemblies;
            JobNS.estimates = data.keyValue.estimates;
            JobCONS.items = data.keyValue.items;
            JobCONS.assemblies = data.keyValue.assemblies;
            //JobNS.POs = data.keyValue.POs;
            //JobNS.COs = data.keyValue.COs;
            LoadCrewEmployees(JobNS.crewEmps);
            LoadItemAsseblies(JobNS.itemAssemblies)
            if (ID == 0) {
                emptyJobInformationForm(data);
                //emptyJobPOForm(data);
                //emptyJobCOForm(data);
            } else {
                populateJobInformationForm(data);
                populateJobAssignmentInformation(data);
                //populateJobTaksInformation(data.keyValue.tasks);
                //populateJobPOList();
                //emptyJobPOForm(data);
                //populateJobCOList();
                //emptyJobCOForm(data);
            }
            closeModal();
        }
    });
}
function getJobSummary(ID) {
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/GetJobSummary',
        type: 'POST',
        data: { jobID: ID },
        beforeSend: function (xhr) {
            openModal();
        },
        success: function (data) {
            populateJobSummaryForm(data);
        },
        complete: function (jqXHR, textStatus) {
            closeModal();
        }
    });
}
function emptyJobInformationForm(data) {
    $("#customers").populateDropDownList(data.keyValue.customers, 0);
    $("#leads").populateDropDownList(data.keyValue.leads, 0);

    $("#job-id").val(0);
    $("#contact").val("");
    $('#jobname').focus();
    ClearBuildingInformation();
}
function populateJobInformationForm(data) {
    $("#customers").populateDropDownList(data.keyValue.customers, data.job.LeadID);
    $("#leads").populateDropDownList(data.keyValue.leads, data.job.LeadID);
    $("#estimates").populateDropDownList(data.keyValue.estimates, data.job.EstimateID);
    var addresses = data.keyValue.addresses;
    if (data.job.LeadID > 0)
        addresses = $.grep(data.keyValue.addresses, function (v) { return data.job.LeadID == v.CustomerID });
    $("#address").populateDropDownList(addresses, data.job.JobAddressID);
    $("#address").change();

    GetCustomerDetail(data.job.LeadID);
    GetBuildingInformaiton(data.job.LeadID);
    GetJobAddress(data.job.JobAddressID);

    $("#job-id").val(data.job.Id);
    $("#jobname").val(data.job.Title);
    $("#note").val(data.job.Notes);
    $("#Description").val(data.job.Description);
    $("#Title").val(data.job.Title);
    $("#CreatedOn").val(data.job.CreatedOn);
    $("#DateUpdated").val(data.job.DateUpdated);
    $("#Status").val(data.job.Status);
}
function populateJobSummaryForm(data) {
    $("#jobID").val(data.job.Id);
    $("#summaryjobid").val(data.job.JobCode);
    $("#customername").val(data.keyValue.customerName);
    $("#jobsiteaddress").val(data.keyValue.jobSiteAddress);
    $("#contactperson").val(data.keyValue.contactPerson);
    $("#salesperson").val(data.keyValue.salesPerson);
    $("#contractprice").val(Number(data.keyValue.contractPrice).toFixed(2).toLocaleString('en-US'));
    $("#totalchangeorder").val(Number(data.keyValue.totalChangeOrder).toFixed(2).toLocaleString('en-US'));
    $("#totalprice").val(Number(data.keyValue.totalPrice).toFixed(2).toLocaleString('en-US'));
    $("#totalpaymentreceived").val(Number(data.keyValue.totalPaymentReceived).toFixed(2).toLocaleString('en-US'));
    $("#totaloutstanding").val(Number(data.keyValue.totalOutstanding).toFixed(2).toLocaleString('en-US'));
    $("#summarynote").val(data.job.Notes);
    $("#jobstatus").populateDropDownList(data.keyValue.statuses, data.job.JobStatusID);
    $("#summaryjobid").focus();
}
function populateBuildingInformation(data) {
    $("#project-type").val(data.ProjectType);
    $("#roof-type").val(data.RoofType);
    $("#age-of-roof").val(data.AgeOfRoof);
    $("#building-stories").val(data.BuildingStories);
    $("#proj-exp-to-begin").val(data.ProjectExpectedToBegin);
    $("#add-information").val(data.AdditionalInformation);
}
function ClearJobAddressInformation() {
    $('#address1').val("");
    $('#address2').val("");
    $('#city').val("");
    $('#state').val("");
    $('#zipcode').val("");
}
function ClearBuildingInformation() {
    $("#project-type").val("");
    $("#roof-type").val("");
    $("#age-of-roof").val("");
    $("#building-stories").val("");
    $("#proj-exp-to-begin").val("");
    $("#add-information").val("");
}
function createJobObject() {
    var customerId = $('#customers option:selected').val();
    var leadId = $('#leads option:selected').val();
    var addressId = $('#address option:selected').val();
    var job = {
        Id: parseInt($("#job-id").val()),
        Description: $("#Description").val(),
        Title: $("#jobname").val(),
        CustomerID: parseInt(customerId),
        LeadID: parseInt(leadId),
        JobAddressID: parseInt(addressId),
        Status: $("#Status").val(),
        Notes: $("#note").val(),
        EstimateID: $('#estimates option:selected').val()
    }
    return job;
}
function saveJobInformation() {
    $('.alert-danger').hide();
    $('.alert-danger').find('#dvError').text('');
    $('.alert-success').hide();
    $('.alert-success').find('#dvSuccess').text('');

    var customerId = $('#customers option:selected').val();
    var leadId = $('#leads option:selected').val();
    var addressId = $('#address option:selected').val();
    var estimateId = $('#estimates option:selected').val();
    $('.alert-danger').find('#dvError').html("");
    if (customerId == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select customer.');
        return;
    }
    else if (leadId == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select lead.');
        return;
    }
    else if (addressId == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select job address.');
        return;
    }
    else if (estimateId == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select estimate name.');
        return;
    }

    var jobData = {
    };

    jobData.Job = createJobObject();;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobInformation',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                if ($('#job-id').val() == "0")
                    window.location.href = $.urls.indexUrl;
                else {
                    $('.alert-success').show();
                    $('.alert-success').find('#dvSuccess').text(data.message);
                }
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}
function saveJobSummary() {
    var statusID = $('#jobstatus option:selected').val();
    $('.alert-danger').find('#dvError').html("");
    if (statusID == 0) {
        $('.alert-danger').show();
        $('.alert-danger').find('#dvError').append('Please select status.');
        return;
    }
    var jobData = {
    };

    var job = {
        Id: parseInt($("#job-id").val()),
        JobStatusID: statusID,
        Notes: $("#summarynote").val()
    };
    jobData.Job = job;
    openModal();
    $.ajax({
        url: getWebsiteBaseUrl() + 'Job/SaveJobSummary',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(jobData),
        success: function (data) {
            if (data.hasSaved) {
                if ($('#job-id').val() == "0")
                    window.location.href = $.urls.indexUrl;
                else {
                    $('.alert-success').show();
                    $('.alert-success').find('#dvSuccess').text(data.message);
                    $('#summaryjobid').focus();
                }
            }
            else {
                $('.alert-danger').show();
                $('.alert-danger').find('#dvError').text(data.message);
            }
            closeModal();
        }
    });
}
function ClearUnusedObject() {
    JobNS = {};
}