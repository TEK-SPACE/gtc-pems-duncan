/*BEGIN RETRIEVE FILTER VALUES*/
function LoadFilterDropdowns(currentCityId) {
    //LoadConfigurations(currentCityId);
  //  LoadZones(currentCityId);
  //  LoadSuburbs(currentCityId);
  //  LoadAreas(currentCityId);
}

//Configurations
function LoadConfigurations(currentCityId) {
    //start with Alarm Status
    $.post('/Collections/GetConfigurations?cityId=' + currentCityId, OnConfigurationsLoaded);
}
function OnConfigurationsLoaded(data) {
    OnDropDownLoaded(data, "ddlConfigurations");
}

//Zones
function LoadZones(currentCityId) {
    //start with Alarm Status
    $.post('/Alarms/GetAlarmZones?cityId=' + currentCityId, OnZonesLoaded);
}
function OnZonesLoaded(data) {
    //OnDropDownLoaded(data, "ddlAlarmZones");
    viewModel.set("zones", data);
}

//Suburbs
function LoadSuburbs(currentCityId) {
    //start with Alarm Status
    $.post('/Collections/GetSuburbs?cityId=' + currentCityId, OnSuburbsLoaded);
}
function OnSuburbsLoaded(data) {
    //OnDropDownLoaded(data, "ddlAlarmSuburb");
    viewModel.set("suburbs", data);
}

//Areas
function LoadAreas(currentCityId) {
    //start with Alarm Status
    $.post('/Alarms/GetAlarmAreas?cityId=' + currentCityId, OnAreasLoaded);
}
function OnAreasLoaded(data) {
    //OnDropDownLoaded(data, "ddlAlarmAreas");
    viewModel.set("areas", data);
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




