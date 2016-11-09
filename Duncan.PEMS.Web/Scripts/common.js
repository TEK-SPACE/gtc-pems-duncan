
// executes when the HTML-Document is loaded and the DOM is ready, even if all the graphics haven’t loaded yet.
$(document).ready(function() {

    // Initialize tooltips
    $('.tooltip').qtip({
        content: {
            text: function(api) {
                // Retrieve content from custom attribute of the $('.selector') elements.
                return $(this).attr('qtip-content');
            }
        },
        position: {
            my: 'bottom left',
            at: 'top center'
        },
        style: {
            classes: 'qtip-blue qtip-shadow qtip-rounded'
        }
    });

    // Submit 'Refresh' button on Enter key
    $('input').keydown(function(e) {
        if (e.keyCode == 13) { // enter key
            // find active element
            var element = $(document.activeElement);
            //if it's a filter
            if (element.hasClass('filter-input') || element.hasClass('filter-dropdown')) {
                // click the filter button
                $('#btnSearch').focus().click();
                return false;
            }
        }
    });

});

// executes when the complete page is fully loaded, including all frames, objects and images.
$(window).load(function () {
    // auto-focus first text box on page
    $('input.k-textbox').filter(':visible:enabled').first().focus();
});

var ControlToSet;
var theForm = document.theForm;
var NN4 = (navigator.appName.indexOf("Netscape")>=0 && !document.getElementById)? true : false;
var NN6 = (document.getElementById && navigator.appName.indexOf("Netscape")>=0 )? true: false;
var TOP;
var LEFT;
var helpWindow;
var helpText;

function popUp(URL) {
    day = new Date();
    id = day.getTime();
    eval("page" + id + " = window.open(URL, '" + id +
        "', 'toolbar=0,scrollbars=0,location=0,statusbar=0,menubar=0,resizable=1,width=400,height=230,left=300,top=200');");
}

function popupHelp(helpMessage) {
    var toolbar = 0;
    var scrollbars = 1;
    var location = 0;
    var statusbar = 0;
    var menubar = 0;
    var resizable = 1;
    var width = 400;
    var height = 200;
    var left = 0;
    var top = 0;
    var features;

    helpText = helpMessage;

    // The following features will position the window's top left corner at the cursor
    //	features = "toolbar=0,scrollbars=" + scrollbars + ",location=0,statusbar=0,menubar=0,resizable=1,width=" + width + ",height=" + height + ",left=" + LEFT + ",top=" + TOP;

    // The following features will center the window on the screen
    left = (screen.width / 2) - (width / 2);
    top = (screen.height / 2) - (height / 2);
    features = "toolbar=" + toolbar + ",scrollbars=" + scrollbars
        + ",location=" + location + ",statusbar=" + statusbar
        + ",menubar=" + menubar + ",resizable=" + resizable
        + ",width=" + width + ",height=" + height
        + ",left=" + left + ",top=" + top;

    helpWindow = window.open("tip.html", 'helpWindow1', features);
}

function callBack() {
    if (document.getElementById) {
        var theDiv = helpWindow.document.getElementById('display');
        if (typeof theDiv.innerHTML == 'string') {
            theDiv.innerHTML = helpText;
            helpText = "";
        }
    }
}

function changeClass(obj, className) {
    obj.className = className;
}

function redirect($location) {
    window.location = $location;
}

function toggleObject(targetID, openClass) {
    if (document.getElementById) {
        var x = document.getElementById(targetID);
        if (x.className == openClass) {
            x.className = "hide";
        } else {
            x.className = openClass;
        }
    }
}

var arrSideContent = new Array(0);

function addSideContent(id, title) {
    var nextIndex = arrSideContent.length;
    arrSideContent.push(nextIndex);
    arrSideContent[nextIndex] = new Array(id, title);

    writeSideContentList();
}

function deleteSideContent(id) {
    if (arrSideContent.length > 0) {
        arrSideContent.splice(id, 1);
    }
    writeSideContentList();
}

function moveSideContentUp(id) {
    tempID = arrSideContent[id][0];
    tempTitle = arrSideContent[id][1];

    arrSideContent.splice(id, 1);
    arrSideContent.splice(id - 1, 0, new Array(tempID, tempTitle));

    writeSideContentList();
}

function moveSideContentDown(id) {
    tempID = arrSideContent[id][0];
    tempTitle = arrSideContent[id][1];

    arrSideContent.splice(id, 1);
    arrSideContent.splice(id + 1, 0, new Array(tempID, tempTitle));

    writeSideContentList();
}

function launchSideContentChooser(pageID) {
    popUp("side_content_chooser.php?page=" + pageID);
}

function writeSideContentList() {
    var output = "";

    output += "<table cellspacing='0' cellpadding='0'>";
    for (i = 0; i < arrSideContent.length; i++) {
        output += "<tr><td><input type='hidden' name='side_content_includes[]' value='" + arrSideContent[i][0] + "' />" + arrSideContent[i][1] + "</td><td class='scrollerOptions'>" + (i != 0 ? "<a href='javascript:moveSideContentUp(" + i + ")'>[^]</a> " : "") + (i != arrSideContent.length - 1 ? "<a href='javascript:moveSideContentDown(" + i + ");'>[v]</a> " : "") + "<a href='javascript:deleteSideContent(" + i + ")'>X</a></td></tr>";
    }
    output += "</table>";

    document.getElementById("sideContentScroller").innerHTML = output;
}

function setAllCheckBoxes(groupName, value) {
    for (var n = 0; n < document.forms[0].length; n++) {
        var oElement = document.forms[0].elements[n];
        if (oElement.type == 'checkbox' && (oElement.name.indexOf(groupName) >= 0))
            oElement.checked = value;
    }
}

// PEMS Popup Dialog functions

function pemsPopupShowError(messageId, message) {
    var popup = $("#pemsPopup").data("kendoWindow");
    popup.title("PEMS Error Message");

    $("#pemsPopupMessageIdSection").show();
    
    document.getElementById('pemsPopupMessageId').innerHTML = "Error Code: " + messageId;
    document.getElementById('pemsPopupMessageContent').innerHTML = message;

    popup.center();
    popup.open();
}


function pemsPopupShowErrorWithTag(area, controller, action, messageId, message) {

    var id = "";
    
    if (area) {
        id += area + ".";
    }
    if (controller) {
        id += controller + ".";
    }
    if (action) {
        id += action + ".";
    }
    id += messageId;

    var popup = $("#pemsPopup").data("kendoWindow");
    popup.title("PEMS Error Message");

    $("#pemsPopupMessageIdSection").show();

    document.getElementById('pemsPopupMessageId').innerHTML = "Error Code: " + id;
    document.getElementById('pemsPopupMessageContent').innerHTML = message;

    popup.center();
    popup.open();
}



function pemsPopupShowMessage(message) {
    var popup = $("#pemsPopup").data("kendoWindow");
    popup.title("PEMS Message");
  $("#pemsPopupMessageIdSection").hide();

    document.getElementById('pemsPopupMessageContent').innerHTML = message;

    popup.open();
    popup.center();
}

function pemsPopupPrint() {
    var w = window.open('','','');
    var printOne;
    var title;
    var message = document.getElementById('pemsPopupMessageContent').innerHTML;

    if ($("#pemsPopupMessageIdSection").is(":visible")) {
        var errorCode = document.getElementById('pemsPopupMessageId').innerHTML;
        printOne = errorCode + "<br/><br/>" + message;
        title = "PEMS Error Message";
    } else {
        printOne = message;
        title = "PEMS Message";
    }

    w.document.write('<html><head><title>' + title + '</title></head><body><h1>' + title + '</h1><hr />' + printOne + '</body></html>');
    w.document.close();

    w.focus();
    w.print();
    w.close();
    $('#pemsPopup').data('kendoWindow').close();
   // return false;
}


function isEmptyObject(data) {
    //stringify the data so we can compare

    var obj = JSON.stringify(data);
    //firefox
    if (obj == '{}' || obj == 'null')
    {
        return true;
    }
    //IE and Chrome
    if (obj == '""' || obj == '[]') {
        return true;
    }
    return false;
}

function set24HourClock() {
    /// <summary>Forces a 24 hour clock in Kendo controls</summary>

    var regex = new RegExp('h', 'g'); // match lower case 'h'
    // edit the short time pattern
    window.kendo.cultures["current"].calendar.patterns.G = kendo.cultures["current"].calendar.patterns.G.replace(regex, 'H').replace('tt','').trim();
    // edit the long time pattern
    window.kendo.cultures["current"].calendar.patterns.g = kendo.cultures["current"].calendar.patterns.g.replace(regex, 'H').replace('tt', '').trim();
}

function getParameterByName(name) {
    /// <summary>Returns the value of the requested query string key</summary>
    /// <param name="name" type="Object"></param>

    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " ")).toLowerCase();
}