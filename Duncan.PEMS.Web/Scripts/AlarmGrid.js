/*BEGIN RETRIEVE FILTER VALUES*/
var currCityId;
function LoadFilterDropdowns(currentCityId) {
    currCityId = currentCityId;
    LoadAssetTypes();
}

//Asset Types
function LoadAssetTypes() {
    //start with Alarm Status
    $.post('/Alarms/GetAssetTypes?cityId=' + currCityId, OnAssetTypesLoaded);
}
function OnAssetTypesLoaded(data) {
    viewModel.set("assetTypes", data);
    LoadTargetServices();
}

//Target Service
function LoadTargetServices() {
    //start with Alarm Status
    $.post('/Alarms/GetTargetServices?cityId=' + currCityId, OnTargetServicesLoaded);
}
function OnTargetServicesLoaded(data) {
    viewModel.set("targetServices", data);
    LoadAssetStates();
}

//Asset State
function LoadAssetStates() {
    //start with Alarm Status
    $.post('/Alarms/GetAssetStates', OnAssetStatesLoaded);
}
function OnAssetStatesLoaded(data) {
    viewModel.set("assetStates", data);
    LoadAlarmSources();
}

//Alarm Source
function LoadAlarmSources() {
    //start with Alarm Status
    $.post('/Alarms/GetAlarmSources', OnAlarmSourcesLoaded);
}
function OnAlarmSourcesLoaded(data) {
    viewModel.set("alarmSources", data);
      LoadTimeTypes();
}

//Time Type
function LoadTimeTypes() {
    //start with Alarm Status
    $.post('/Alarms/GetTimeTypes', OnTimeTypesLoaded);
}
function OnTimeTypesLoaded(data) {
    viewModel.set("timeTypes", data);
    LoadAlarmSeveritys();
}

//Alarm Severity
function LoadAlarmSeveritys() {
    //start with Alarm Status
    $.post('/Alarms/GetAlarmSeverities', OnAlarmSeveritysLoaded);
}
function OnAlarmSeveritysLoaded(data) {
    viewModel.set("alarmSeverities", data);
    LoadAlarmCodes();
}


//Alarm Codes
function LoadAlarmCodes() {
    //start with Alarm Status
    $.post('/Alarms/GetAlarmFilters?customerId=' + currCityId, OnAlarmCodesLoaded);
}
function OnAlarmCodesLoaded(data) {
    viewModel.set("alarmCodes", data);
    LoadTechnicianDetails();
}


//Technician ID
function LoadTechnicianDetails() {
    //start with Alarm Status
    $.post('/Alarms/GetTechnicianDetails', OnTechnicianDetailsLoaded);
}
function OnTechnicianDetailsLoaded(data) {
    viewModel.set("technicians", data);
    LoadSorts();
}


//DDLLoadData - with "All" default option
function OnDropDownLoaded(data, ddlId, hideDefault) {
    var $select = $("#" + ddlId);
    $select.html('');
    if (!hideDefault) {
        $select.append($('<option></option>').val('').html("All"));
    }
    for (var index = 0; index < data.length; index++) {
        var option = data[index];
        $select.append($('<option></option>').val(option.Value).html(option.Text));
    }
}
/*END FILTER VALUES*/
