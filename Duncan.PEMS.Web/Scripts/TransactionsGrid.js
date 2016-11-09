/*

*/


/*
BEGIN RETRIEVE FILTER VALUES
*/

function GetAssetTypes(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
   // controllerURL = controllerURL + "/GetAssetTypes"
   // alert(controllerURL);
    dataurl = "";//dataurl.replace("//", "/");
    $.ajax({
        url: controllerURL,
        success: function (data) {
           // alert(data.length);
            var ddlbox = document.getElementById('ddlAssetType');
            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {
                    
                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}
function GetOccupancies(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
    // controllerURL = controllerURL + "/GetTypeTypes"
    dataurl = "";//dataurl.replace("//", "/");
    $.ajax({
        url: controllerURL,
        success: function (data) {
            //alert(data);
            var ddlbox = document.getElementById('ddlOccumpancy');
            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {

                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}


function GetTransactionTypes(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
    // controllerURL = controllerURL + "/GetTypeTypes"
    dataurl = "";//dataurl.replace("//", "/");
    $.ajax({
        url: controllerURL,
        success: function (data) {
            //alert(data);
            var ddlbox = document.getElementById('ddlTransactionType');
            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {

                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}
function GetLocationTypes(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
   // controllerURL = controllerURL + "/GetTypeTypes"
    dataurl = "";//dataurl.replace("//", "/");
    alert(controllerURL);
    $.ajax({
        url: controllerURL,
        success: function (data) {
            //alert(data);
            var ddlbox = document.getElementById('ddlLocationTypes');

            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {

                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}
function GetTimeTypes(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
    // controllerURL = controllerURL + "/GetTypeTypes"
    dataurl = "";//dataurl.replace("//", "/");
    $.ajax({
        url: controllerURL,
        success: function (data) {
            //alert(data);
            var ddlbox = document.getElementById('ddlTimeType');
            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {

                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}

function GetPaymentStatuses(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
    // controllerURL = controllerURL + "/GetTypeTypes"
    dataurl = "";//dataurl.replace("//", "/");
    $.ajax({
        url: controllerURL,
        success: function (data) {
            //alert(data);
            var ddlbox = document.getElementById('ddlPaymentStatus');
            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {

                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}


function GetPaymentTypes(dataurl) {
    //alert("i was called severities " + dataurl);
    var controllerURL = "";
    controllerURL = dataurl.replace("//", "/");
    // controllerURL = controllerURL + "/GetTypeTypes"
    dataurl = "";//dataurl.replace("//", "/");
    $.ajax({
        url: controllerURL,
        success: function (data) {
            //alert(data);
            var ddlbox = document.getElementById('ddlPaymentTypes');
            if (ddlbox != null) {
                for (var i = 0; i < data.length; i++) {

                    ddlbox.options.add(new Option(data[i].DDLValue, data[i].DDLValue));
                }
            }
        }
    });
}