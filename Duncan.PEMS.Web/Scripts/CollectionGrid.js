/*BEGIN RETRIEVE FILTER VALUES*/
function LoadFilterDropdowns(currentCityId) {
    LoadStatus();
  //  LoadConfigurations(currentCityId);
  
}

//Asset Types
function LoadStatus() {
    //start with Alarm Status
    $.post('/Collections/GetStatuses', OnLoadStatusLoaded);
}
function OnLoadStatusLoaded(data) {
    viewModel.set("statuses", data);
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




