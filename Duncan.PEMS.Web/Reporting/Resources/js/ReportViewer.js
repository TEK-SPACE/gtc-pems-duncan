var currentDatasource;
var selectionsList;
var wereChecked = new Array();
var curPropCdsInd;
var curPropFInd;
var additionalCategories = new Array();
var needClick = false;
var realFilterParent;
var visualFilterParent;
var filtersTable;
var clonedFilters;
var groupingUsed;
var prevCatValue;

//Util-----------------------------------------------------------------------------------------------------
function modifyUrl(parameterName, parameterValue) {
    var queryParameters = {}, queryString = location.search.substring(1),
            re = /([^&=]+)=([^&]*)/g, m;
    while (m = re.exec(queryString)) {
        queryParameters[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
    }
    queryParameters[parameterName] = parameterValue;
    location.search = jq$.param(queryParameters);
}

function updateURLParameter(url, param, paramVal) {
    var newAdditionalURL = "";
    var tempArray = url.split("?");
    var baseURL = tempArray[0];
    var additionalURL = tempArray[1];
    var temp = "";
    if (additionalURL) {
        tempArray = additionalURL.split("&");
        for (i = 0; i < tempArray.length; i++) {
            if (tempArray[i].split('=')[0] != param) {
                newAdditionalURL += temp + tempArray[i];
                temp = "&";
            }
        }
    }

    var rows_txt = temp + "" + param + "=" + paramVal;
    return baseURL + "?" + newAdditionalURL + rows_txt;
}

function IsIE() {
    if (navigator.appName == 'Microsoft Internet Explorer')
        return true;
    return false;
}
//------------------------------------------------------------------------------------------------------------------

//Ajax--------------------------------------------------------------------------------------------------------------
function AjaxRequest(url, parameters, callbackSuccess, callbackError, id, dataToKeep) {
    var thisRequestObject;
    if (window.XMLHttpRequest)
        thisRequestObject = new XMLHttpRequest();
    else if (window.ActiveXObject)
        thisRequestObject = new ActiveXObject('Microsoft.XMLHTTP');
    thisRequestObject.requestId = id;
    thisRequestObject.dtk = dataToKeep;
    thisRequestObject.onreadystatechange = ProcessRequest;

    /*thisRequestObject.open('GET', url + '?' + parameters, true);
    thisRequestObject.send();*/
    thisRequestObject.open('POST', url, true);
    thisRequestObject.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    thisRequestObject.send(parameters);

    function DeserializeJson() {
        var responseText = thisRequestObject.responseText;
        while (responseText.indexOf('"\\/Date(') >= 0) {
            responseText = responseText.replace('"\\/Date(', 'eval(new Date(');
            responseText = responseText.replace(')\\/"', '))');
        }
        if (responseText.charAt(0) != '[' && responseText.charAt(0) != '{')
            responseText = '{' + responseText + '}';
        var isArray = true;
        var isHtml = false;
        if (responseText.charAt(0) != '[') {
            responseText = '[' + responseText + ']';
            isArray = false;
        }
        var retObj;
        try {
            retObj = eval(responseText);
        }
        catch (e) {
            retObj = null;
        }
        if (retObj == null) {
            try {
                isHtml = true;
                retObj = eval(thisRequestObject.responseText);
            }
            catch (e) {
                retObj = null;
            }
        }
        if (retObj == null)
            return null;
        if (isHtml)
            return retObj;
        if (!isArray)
            return retObj[0];
        return retObj;
    }

    function ProcessRequest() {
        if (thisRequestObject.readyState == 4) {
            if (thisRequestObject.status == 200 && callbackSuccess) {
                var toRet;
                if (thisRequestObject.requestId != 'getrenderedreportset' && thisRequestObject.requestId != 'getcrsreportpartpreview' && thisRequestObject.requestId != 'renderedreportpart')
                    toRet = DeserializeJson();
                else
                    toRet = thisRequestObject.responseText;
                callbackSuccess(toRet, thisRequestObject.requestId, thisRequestObject.dtk);
            }
            else if (callbackError) {
                callbackError(thisRequestObject);
            }
        }
    }
}

function clearNames(elem) {
    $(elem).attr('name', '');
    $(elem).attr('id', '');
    for (var index = 0; index < elem.children.length; index++)
        clearNames(elem.children[index]);
}

function SendFieldsData(data) {
    var requestString = 'wscmd=updatecrsfields&wsarg0=' + data;
    AjaxRequest('./rs.aspx', requestString, FieldsDataSent, null, 'updatecrsfields');
}

function FieldsDataSent(returnObj, id) {
    if (id != 'updatecrsfields' || returnObj == undefined || returnObj == null || returnObj.Value == null)
        return;
    GetRenderedReportSet(true);
}
//------------------------------------------------------------------------------------------------------------------

//Common interface----------------------------------------------------------------------------------------------------
function DetectCurrentDs(dbName) {
    var newValue = -1;
    for (var index = 0; index < selectionsList.length; index++) {
        if (dbName == selectionsList[index].DbName) {
            newValue = index;
            break;
        }
    }
    currentDatasource = newValue;
}

function GetSelectedFieldInd() {
    var res = -1;
    var idCnt = 0;
    while (true) {
        var cb = document.getElementById('ufcb' + idCnt);
        if (cb == null)
            break;
        if (cb.checked) {
            if (res == -1)
                res = idCnt;
            else {
                res = -1;
                break;
            }
        }
        idCnt++;
    }
    return res;
}

function GetFieldIndexes(fieldDbName) {
    for (var dsCnt = 0; dsCnt < selectionsList.length; dsCnt++) {
        for (var fCnt = 0; fCnt < selectionsList[dsCnt].Fields.length; fCnt++) {
            if (selectionsList[dsCnt].Fields[fCnt].DbName == fieldDbName) {
                var indexes = new Array();
                indexes[indexes.length] = dsCnt;
                indexes[indexes.length] = fCnt;
                return indexes;
            }
        }
    }
    return null;
}

function UpdateMSV(lid) {
    var label = document.getElementById(lid);
    var msvs = label.getAttribute('msvs').split(',');
    var msv = label.getAttribute('msv');
    var curInd = -1;
    for (var ind = 0; ind < msvs.length; ind++) {
        if (msvs[ind] == msv) {
            curInd = ind;
            break;
        }
    }
    curInd++;
    if (curInd >= msvs.length)
        curInd = 0;
    label.setAttribute('msv', msvs[curInd]);
    label.innerHTML = msvs[curInd];
}

function InitiateEmail() {
    var fieldWithRn = document.getElementById('reportNameFor2ver');
    var rnVal;
    if (fieldWithRn != null)
        rnVal = fieldWithRn.value;
    else if (reportName == undefined || reportName == null)
        rnVal = '';
    else
        rnVal = reportName;
    while (rnVal.indexOf('+') >= 0) {
        rnVal = rnVal.replace('+', ' ');
    }
    TB_EMailReport(encodeURIComponent(rnVal), '?subject=' + encodeURIComponent(rnVal) + '&body=' + encodeURIComponent(location));
}

function ChangeTopRecords(recsNum, updateReportData) {
    if (updateReportData) {
      SetTopRecords(uvcVal);
    }
}

function SetTopRecords(topRecords) {
    var requestString = 'wscmd=settoprecords&wsarg0=' + topRecords;
    AjaxRequest('./rs.aspx', requestString, TopRecordsSet, null, 'settoprecords');
}

function TopRecordsSet(returnObj, id) {
    if (id != 'settoprecords' || returnObj == undefined || returnObj == null)
        return;
    if (returnObj.Value != 'OK') {
        alert(returnObj.Value);
        return;
    }
    GetRenderedReportSet(false);
}

function RefreshFieldsList() {
    var remainingHtml = "<table>";
    var usedHtml = "<table>";
    var uCnt = 0;
    var rCnt = 0;
    var cdsBegin = currentDatasource;
    var cdsEnd = currentDatasource + 1;
    if (currentDatasource == -1) {
        cdsBegin = 0;
        cdsEnd = selectionsList.length;
    }
    var selectedFields = new Array();
    for (var cds = cdsBegin; cds < cdsEnd; cds++) {
        for (var fCnt = 0; fCnt < selectionsList[cds].Fields.length; fCnt++) {
            var idPrefix;
            if (selectionsList[cds].Fields[fCnt].Selected > -1) {
                idPrefix = 'ufcb' + uCnt;
                uCnt++;
            }
            else {
                idPrefix = 'rfcb' + rCnt;
                rCnt++;
            }
            var wasChecked = '';
            for (var index = 0; index < wereChecked.length; index++) {
                if (wereChecked[index] == selectionsList[cds].Fields[fCnt].DbName) {
                    wasChecked = ' checked="checked"';
                    break;
                }
            }
            var clickProcess = '';
            if (selectionsList[cds].Fields[fCnt].Selected > -1)
                clickProcess = 'onchange="javascript:var fpButton = document.getElementById(\'fpButton\'); if (GetSelectedFieldInd() >= 0) {$(fpButton).removeClass(\'disabled\');} else {$(fpButton).addClass(\'disabled\');}"';
            var lColor = '';
            var fieldOpt = '';
            if (!selectionsList[cds].Fields[fCnt].Hidden) {
              fieldOpt += '<tr><td><input type="checkbox" id="' + idPrefix + '" cdsInd="' + cds + '" fInd="' + fCnt + '" value="' + selectionsList[cds].Fields[fCnt].DbName + '"' + wasChecked + clickProcess + ' /></td>';
            }
            else {
              fieldOpt += '<tr><td><input type="checkbox" id="' + idPrefix + '" cdsInd="' + cds + '" fInd="' + fCnt + '" value="' + selectionsList[cds].Fields[fCnt].DbName + '" disabled="disabled" /></td>';
              lColor = 'color:#808080;';              
            }
            fieldOpt += '<td><label style=\"cursor:pointer;' + lColor + '\" for=\"' + idPrefix + '\">' + selectionsList[cds].Fields[fCnt].FriendlyName + '</label></td></tr>';
            if (selectionsList[cds].Fields[fCnt].Selected > -1) {
                var selectedHtml = new Object();
                selectedHtml.Text = fieldOpt;
                selectedHtml.OrderNum = selectionsList[cds].Fields[fCnt].Selected;
                selectedFields[selectedFields.length] = selectedHtml;
            }
            else
                remainingHtml += fieldOpt;
        }
    }
    for (var ind1 = 0; ind1 < selectedFields.length - 1; ind1++) {
        for (var ind2 = ind1 + 1; ind2 < selectedFields.length; ind2++) {
            if (selectedFields[ind1].OrderNum > selectedFields[ind2].OrderNum) {
                var tmpText = selectedFields[ind1].Text;
                var tmpOrder = selectedFields[ind1].OrderNum;
                selectedFields[ind1].Text = selectedFields[ind2].Text;
                selectedFields[ind1].OrderNum = selectedFields[ind2].OrderNum;
                selectedFields[ind2].Text = tmpText;
                selectedFields[ind2].OrderNum = tmpOrder;
            }
        }
    }
    for (var index = 0; index < selectedFields.length; index++)
        usedHtml += selectedFields[index].Text;
    usedHtml += '</table>';
    remainingHtml += '</table>';
    var remaining = document.getElementById('remainingFieldsSel');
    var used = document.getElementById('usedFieldsSel');
    remaining.innerHTML = remainingHtml;
    used.innerHTML = usedHtml;
    var fpButton = document.getElementById('fpButton');
    if (wereChecked.length != 1)
        $(fpButton).addClass('disabled');
    else
        $(fpButton).removeClass('disabled');
}

function updateFields() {
    var usageData = new Object();
    usageData.Fields = new Array();
    for (var dsCnt = 0; dsCnt < selectionsList.length; dsCnt++) {
        for (var fCnt = 0; fCnt < selectionsList[dsCnt].Fields.length; fCnt++)
            if (selectionsList[dsCnt].Fields[fCnt].Selected > -1) {
                var usedField = new Object();
                usedField.FriendlyName = selectionsList[dsCnt].Fields[fCnt].FriendlyName;
                usedField.DbName = selectionsList[dsCnt].Fields[fCnt].DbName;
                usedField.Selected = selectionsList[dsCnt].Fields[fCnt].Selected;
                usedField.Total = selectionsList[dsCnt].Fields[fCnt].Total;
                usedField.VG = selectionsList[dsCnt].Fields[fCnt].VG;
                usedField.Description = selectionsList[dsCnt].Fields[fCnt].Description;
                usedField.Format = selectionsList[dsCnt].Fields[fCnt].Format;
                usedField.LabelJ = selectionsList[dsCnt].Fields[fCnt].LabelJ;
                usedField.ValueJ = selectionsList[dsCnt].Fields[fCnt].ValueJ;
                usageData.Fields[usageData.Fields.length] = usedField;
            }
    }
    var s = JSON.stringify(usageData);
    wereChecked.length = 0;
    RefreshFieldsList();
    SendFieldsData(s);
}
//------------------------------------------------------------------------------------------------------------------

//Pivots control--------------------------------------------------------------------------------------------------
function RefreshPivots() {
    var requestString = 'wscmd=getpivotguidata';
    AjaxRequest('./rs.aspx', requestString, PivotsDataGot, null, 'getpivotguidata');
}

function PivotsDataGot(returnObj, id) {
    if (id != 'getpivotguidata' || returnObj == undefined || returnObj == null)
        return;
    if (returnObj.CurrentField == '' || returnObj.FieldNames == null) {
        jq$(".visibility-pivots").hide();
        return;
    }
    jq$(".visibility-pivots").show();
    var pivotHtml = '<div>COLUMN</div>';
    pivotHtml += '<select id="pivot-field" onchange="SetPivotField();">';
    for (var i = 0; i < returnObj.FieldNames.length; i++) {
        var selected = '';
        if (returnObj.CurrentField == returnObj.FieldIds[i])
            selected = ' selected="selected"';
        if (returnObj.FieldIds[i] == '') {
            if (returnObj.FieldNames[i] == '------') {
                pivotHtml += '<option disabled="">------</option>';
            }
            else {
                if (returnObj.FieldNames[i] == '') {
                    pivotHtml += '</optgroup>';
                }
                else {
                    pivotHtml += '<optgroup label="' + returnObj.FieldNames[i] + '">';
                }
            }
            continue;
        }
        pivotHtml += '<option value="' + returnObj.FieldIds[i] + '"' + selected + '>' + returnObj.FieldNames[i] + '</option>';
    }
    pivotHtml += '</select><br />';
    pivotHtml += '<div>FUNCTION</div>';
    pivotHtml += '<select id="pivot-function" onchange="SetPivotFunction();">';
    for (var i = 0; i < returnObj.FunctionNames.length; i++) {
        var selected = '';
        if (returnObj.CurrentFunction == returnObj.FunctionIds[i])
            selected = ' selected="selected"';
        pivotHtml += '<option value="' + returnObj.FunctionIds[i] + '"' + selected + '>' + returnObj.FunctionNames[i] + '</option>';
    }
    pivotHtml += '</select><br />';
    var pivotSelector = document.getElementById('pivot-selector');
    pivotSelector.innerHTML = pivotHtml;
}

function SetPivotField() {
    var requestString = 'wscmd=setpivotfield&wsarg0=' + document.getElementById('pivot-field').value;
    AjaxRequest('./rs.aspx', requestString, PivotFieldSet, null, 'setpivotfield');
}

function PivotFieldSet(returnObj, id) {
    if (id != 'setpivotfield' || returnObj == undefined || returnObj == null || returnObj.Value == null)
        return;
    if (returnObj.Value != 'OK')
        alert(returnObj.Value);
    RefreshPivots();
}

function SetPivotFunction() {
    var requestString = 'wscmd=setpivotfunction&wsarg0=' + document.getElementById('pivot-function').value;
    AjaxRequest('./rs.aspx', requestString, PivotFunctionSet, null, 'setpivotfunction');
}

function PivotFunctionSet(returnObj, id) {
    if (id != 'setpivotfunction' || returnObj == undefined || returnObj == null || returnObj.Value == null)
        return;
    if (returnObj.Value != 'OK')
        alert(returnObj.Value);
    RefreshPivots();
}
//------------------------------------------------------------------------------------------------------------------

//Field advanced properties-------------------------------------------------------------------------------------------------------
function ShowFieldProperties() {
    var fieldInd = GetSelectedFieldInd();
    var fieldCb = document.getElementById('ufcb' + fieldInd);
    if (fieldCb == null)
        return;
    curPropCdsInd = fieldCb.getAttribute('cdsInd');
    curPropFInd = fieldCb.getAttribute('fInd');
    var titleDiv = document.getElementById('titleDiv');
    titleDiv.innerHTML = 'Field Properties for ' + selectionsList[curPropCdsInd].Fields[curPropFInd].FriendlyName;
    document.getElementById('propDescription').value = selectionsList[curPropCdsInd].Fields[curPropFInd].Description;
    var propTotal = document.getElementById('propTotal');
    propTotal.checked = false;
    if (selectionsList[curPropCdsInd].Fields[curPropFInd].Total == 1)
        propTotal.checked = true;
    var propVG = document.getElementById('propVG');
    propVG.checked = false;
    if (selectionsList[curPropCdsInd].Fields[curPropFInd].VG == 1)
        propVG.checked = true;
    var propFormats = document.getElementById('propFormats');
    propFormats.options.length = 0;
    for (var formatCnt = 0; formatCnt < selectionsList[curPropCdsInd].Fields[curPropFInd].FormatValues.length; formatCnt++) {
        var opt = new Option();
        opt.value = selectionsList[curPropCdsInd].Fields[curPropFInd].FormatValues[formatCnt];
        opt.text = selectionsList[curPropCdsInd].Fields[curPropFInd].FormatNames[formatCnt];
        if (selectionsList[curPropCdsInd].Fields[curPropFInd].Format == selectionsList[curPropCdsInd].Fields[curPropFInd].FormatValues[formatCnt])
            opt.selected = 'selected';
        propFormats.add(opt);
    }
    var labelJ = document.getElementById('labelJ');
    var msvs = labelJ.getAttribute('msvs').split(',');
    labelJ.innerHTML = msvs[selectionsList[curPropCdsInd].Fields[curPropFInd].LabelJ - 1];
    labelJ.setAttribute('msv', msvs[selectionsList[curPropCdsInd].Fields[curPropFInd].LabelJ - 1]);
    var valueJ = document.getElementById('valueJ');
    msvs = valueJ.getAttribute('msvs').split(',');
    valueJ.innerHTML = msvs[selectionsList[curPropCdsInd].Fields[curPropFInd].ValueJ];
    valueJ.setAttribute('msv', msvs[selectionsList[curPropCdsInd].Fields[curPropFInd].ValueJ]);
    if (IsIE()) {
        labelJ.style.left = '-17px';
        valueJ.style.left = '-17px';
    }
    fieldPopup.dialog("option", "title", selectionsList[curPropCdsInd].Fields[curPropFInd].FriendlyName);
    fieldPopup.dialog("open");
}

function updateFieldProperties() {
    selectionsList[curPropCdsInd].Fields[curPropFInd].Description = document.getElementById('propDescription').value;
    var propTotal = document.getElementById('propTotal');
    if (propTotal.checked)
        selectionsList[curPropCdsInd].Fields[curPropFInd].Total = 1;
    else
        selectionsList[curPropCdsInd].Fields[curPropFInd].Total = 0;
    var propVG = document.getElementById('propVG');
    if (propVG.checked)
        selectionsList[curPropCdsInd].Fields[curPropFInd].VG = 1;
    else
        selectionsList[curPropCdsInd].Fields[curPropFInd].VG = 0;
    var propFormats = document.getElementById('propFormats');
    selectionsList[curPropCdsInd].Fields[curPropFInd].Format = propFormats.value;
    var labelJ = document.getElementById('labelJ');
    var msvs = labelJ.getAttribute('msvs').split(',');
    var msv = labelJ.getAttribute('msv');
    var curInd = 0;
    for (var ind = 0; ind < msvs.length; ind++) {
        if (msvs[ind] == msv) {
            curInd = ind;
            break;
        }
    }
    selectionsList[curPropCdsInd].Fields[curPropFInd].LabelJ = curInd + 1;
    var valueJ = document.getElementById('valueJ');
    var msvs = valueJ.getAttribute('msvs').split(',');
    var msv = valueJ.getAttribute('msv');
    var curInd = 0;
    for (var ind = 0; ind < msvs.length; ind++) {
        if (msvs[ind] == msv) {
            curInd = ind;
            break;
        }
    }
    selectionsList[curPropCdsInd].Fields[curPropFInd].ValueJ = curInd;
    updateFields();
}
//------------------------------------------------------------------------------------------------------------------

//Fields list-----------------------------------------------------------------------------------------------------------
function AddRemainingFields() {
    var newInd = -1;
    cdsBegin = 0;
    cdsEnd = selectionsList.length;
    for (var cds = cdsBegin; cds < cdsEnd; cds++) {
        for (var fCnt = 0; fCnt < selectionsList[cds].Fields.length; fCnt++) {
            if (newInd < selectionsList[cds].Fields[fCnt].Selected)
                newInd = selectionsList[cds].Fields[fCnt].Selected;
        }
    }
    newInd++;
    var idCnt = 0;
    while (true) {
        var cb = document.getElementById('rfcb' + idCnt);
        if (cb == null)
            break;
        if (cb.checked) {
            var ci = cb.getAttribute('cdsInd');
            var fi = cb.getAttribute('fInd');
            selectionsList[ci].Fields[fi].Selected = newInd;
            selectionsList[ci].Fields[fi].LabelJ = 1;
            selectionsList[ci].Fields[fi].ValueJ = 0;
            selectionsList[ci].Fields[fi].Description = selectionsList[ci].Fields[fi].FriendlyName;
            newInd++;
        }
        idCnt++;
    }
    wereChecked.length = 0;
    RefreshFieldsList();
}

function RemoveUsedFields() {
    var idCnt = 0;
    while (true) {
        var cb = document.getElementById('ufcb' + idCnt);
        if (cb == null)
            break;
        if (cb.checked) {
            selectionsList[cb.getAttribute('cdsInd')].Fields[cb.getAttribute('fInd')].Selected = -1;
        }
        idCnt++;
    }
    var selectedFields = new Array();
    cdsBegin = 0;
    cdsEnd = selectionsList.length;
    for (var cds = cdsBegin; cds < cdsEnd; cds++) {
        for (var fCnt = 0; fCnt < selectionsList[cds].Fields.length; fCnt++) {
            if (selectionsList[cds].Fields[fCnt].Selected > -1) {
                var selectedHtml = new Object();
                selectedHtml.DsNum = cds;
                selectedHtml.FNum = fCnt;
                selectedHtml.OrderNum = selectionsList[cds].Fields[fCnt].Selected;
                selectedFields[selectedFields.length] = selectedHtml;
            }
        }
    }
    for (var ind1 = 0; ind1 < selectedFields.length - 1; ind1++) {
        for (var ind2 = ind1 + 1; ind2 < selectedFields.length; ind2++) {
            if (selectedFields[ind1].OrderNum > selectedFields[ind2].OrderNum) {
                var tmpDsNum = selectedFields[ind1].DsNum;
                var tmpFNum = selectedFields[ind1].FNum;
                var tmpOrder = selectedFields[ind1].OrderNum;
                selectedFields[ind1].DsNum = selectedFields[ind2].DsNum;
                selectedFields[ind1].FNum = selectedFields[ind2].FNum;
                selectedFields[ind1].OrderNum = selectedFields[ind2].OrderNum;
                selectedFields[ind2].DsNum = tmpDsNum;
                selectedFields[ind2].FNum = tmpFNum;
                selectedFields[ind2].OrderNum = tmpOrder;
            }
        }
    }
    for (var index = 0; index < selectedFields.length; index++)
        selectionsList[selectedFields[index].DsNum].Fields[selectedFields[index].FNum].Selected = index;
    wereChecked.length = 0;
    RefreshFieldsList();
}

function MoveUp() {
    var selectedFields = new Array();
    for (var dsCnt = 0; dsCnt < selectionsList.length; dsCnt++) {
        for (var fCnt = 0; fCnt < selectionsList[dsCnt].Fields.length; fCnt++) {
            if (selectionsList[dsCnt].Fields[fCnt].Selected > -1) {
                var usedField = new Object();
                usedField.FriendlyName = selectionsList[dsCnt].Fields[fCnt].FriendlyName;
                usedField.DbName = selectionsList[dsCnt].Fields[fCnt].DbName;
                usedField.Selected = selectionsList[dsCnt].Fields[fCnt].Selected;
                usedField.ToMove = false;
                usedField.DbNum = dsCnt;
                usedField.FNum = fCnt;
                selectedFields[selectedFields.length] = usedField;
            }
        }
    }
    var idCnt = 0;
    wereChecked = new Array();
    while (true) {
        var cb = document.getElementById('ufcb' + idCnt);
        if (cb == null)
            break;
        if (cb.checked) {
            for (var sCnt = 0; sCnt < selectedFields.length; sCnt++) {
                if (selectedFields[sCnt].DbName == cb.value) {
                    selectedFields[sCnt].ToMove = true;
                    wereChecked[wereChecked.length] = selectedFields[sCnt].DbName;
                }
            }
        }
        idCnt++;
    }
    for (var ind1 = 0; ind1 < selectedFields.length - 1; ind1++) {
        for (var ind2 = ind1 + 1; ind2 < selectedFields.length; ind2++) {
            if (selectedFields[ind1].Selected > selectedFields[ind2].Selected) {
                var tmp = selectedFields[ind1];
                selectedFields[ind1] = selectedFields[ind2];
                selectedFields[ind2] = tmp;
            }
        }
    }
    for (var cnt = 1; cnt < selectedFields.length; cnt++) {
        if (selectedFields[cnt].ToMove && !selectedFields[cnt - 1].ToMove) {
            var tmp = selectedFields[cnt - 1];
            selectedFields[cnt - 1] = selectedFields[cnt];
            selectedFields[cnt] = tmp;
        }
    }
    for (var cnt = 0; cnt < selectedFields.length; cnt++)
        selectionsList[selectedFields[cnt].DbNum].Fields[selectedFields[cnt].FNum].Selected = cnt;
    RefreshFieldsList();
}

function MoveDown() {
    var selectedFields = new Array();
    for (var dsCnt = 0; dsCnt < selectionsList.length; dsCnt++) {
        for (var fCnt = 0; fCnt < selectionsList[dsCnt].Fields.length; fCnt++) {
            if (selectionsList[dsCnt].Fields[fCnt].Selected > -1) {
                var usedField = new Object();
                usedField.FriendlyName = selectionsList[dsCnt].Fields[fCnt].FriendlyName;
                usedField.DbName = selectionsList[dsCnt].Fields[fCnt].DbName;
                usedField.Selected = selectionsList[dsCnt].Fields[fCnt].Selected;
                usedField.ToMove = false;
                usedField.DbNum = dsCnt;
                usedField.FNum = fCnt;
                selectedFields[selectedFields.length] = usedField;
            }
        }
    }
    var idCnt = 0;
    wereChecked = new Array();
    while (true) {
        var cb = document.getElementById('ufcb' + idCnt);
        if (cb == null)
            break;
        if (cb.checked) {
            for (var sCnt = 0; sCnt < selectedFields.length; sCnt++) {
                if (selectedFields[sCnt].DbName == cb.value) {
                    selectedFields[sCnt].ToMove = true;
                    wereChecked[wereChecked.length] = selectedFields[sCnt].DbName;
                }
            }
        }
        idCnt++;
    }
    for (var ind1 = 0; ind1 < selectedFields.length - 1; ind1++) {
        for (var ind2 = ind1 + 1; ind2 < selectedFields.length; ind2++) {
            if (selectedFields[ind1].Selected > selectedFields[ind2].Selected) {
                var tmp = selectedFields[ind1];
                selectedFields[ind1] = selectedFields[ind2];
                selectedFields[ind2] = tmp;
            }
        }
    }
    for (var cnt = selectedFields.length - 2; cnt >= 0; cnt--) {
        if (selectedFields[cnt].ToMove && !selectedFields[cnt + 1].ToMove) {
            var tmp = selectedFields[cnt + 1];
            selectedFields[cnt + 1] = selectedFields[cnt];
            selectedFields[cnt] = tmp;
        }
    }
    for (var cnt = 0; cnt < selectedFields.length; cnt++)
        selectionsList[selectedFields[cnt].DbNum].Fields[selectedFields[cnt].FNum].Selected = cnt;
    RefreshFieldsList();
}
//------------------------------------------------------------------------------------------------------------------------


//Save and Save As code----------------------------------------------------------------------------------------------------------
function GetCategoriesList(setRn) {
    var requestString = 'wscmd=crscategories';
    AjaxRequest('./rs.aspx', requestString, GotCategoriesList, null, 'crscategories', setRn);
}

function GotCategoriesList(returnObj, id, setRn) {
    if (id != 'crscategories' || returnObj == undefined || returnObj == null)
        return;
    var fieldWithRn = document.getElementById('reportNameFor2ver');
    var rnVal;
    if (fieldWithRn != null)
        rnVal = fieldWithRn.value;
    else if (reportName == undefined || reportName == null)
        rnVal = '';
    else
        rnVal = reportName;
    while (rnVal.indexOf('+') >= 0) {
        rnVal = rnVal.replace('+', ' ');
    }
    var nodes = rnVal.split('\\\\');
    var curCatName = '';
    var curRepName = nodes[0];
    if (nodes.length > 1) {
        curCatName = nodes[0];
        curRepName = nodes[1];
    }
    var newReportName = document.getElementById('newReportName');
    var newCategoryName = document.getElementById('newCategoryName');
    if (setRn) {
        newReportName.value = curRepName;
    }
    var catsArray = new Array();
    catsArray[catsArray.length] = '';
    for (var acCnt = 0; acCnt < additionalCategories.length; acCnt++)
        catsArray[catsArray.length] = additionalCategories[acCnt];
    if (returnObj.AdditionalData != null && returnObj.AdditionalData.length > 0)
        for (var index = 0; index < returnObj.AdditionalData.length; index++)
            catsArray[catsArray.length] = returnObj.AdditionalData[index];
    newCategoryName.options.length = 0;
    var opt = new Option();
    opt.value = '(Create new)';
    opt.text = '(Create new)';
    newCategoryName.add(opt);
    for (var index = 0; index < catsArray.length; index++) {
        var opt = new Option();
        opt.value = catsArray[index];
        var ot = catsArray[index];
        while (ot.indexOf('+') >= 0) {
            ot = ot.replace('+', ' ');
        }
        opt.text = ot;
        if (opt.text == curCatName && additionalCategories.length == 0)
            opt.selected = 'selected';
        if (additionalCategories.length > 0 && opt.text == additionalCategories[additionalCategories.length - 1])
            opt.selected = 'selected';
        newCategoryName.add(opt);
    }
    var saveAsDialog = document.getElementById('saveAsDialog');
    var windowHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
    saveAsDialog.style.height = windowHeight + 'px';
    saveAsDialog.style.paddingTop = ((windowHeight / 2) - 100) + 'px';
    saveAsDialog.style.display = '';
    prevCatValue = newCategoryName.value;
}

function ShowSaveAsDialog() {
    additionalCategories.length = 0;
    GetCategoriesList(true);
}

function SaveReportAs() {
    var newRepName = document.getElementById('newReportName').value;
    var newCatName = document.getElementById('newCategoryName').value;
    var fieldWithRn = document.getElementById('reportNameFor2ver');
    var newFullName = newRepName;
    if (newCatName != null && newCatName != '' && newCatName != 'Uncategorized') {
        newFullName = newCatName + '\\\\' + newFullName;
    }
    while (newFullName.indexOf(' ') >= 0) {
        newFullName = newFullName.replace(' ', '+');
    }
    if (fieldWithRn != null) {
        fieldWithRn.value = newFullName;
    }
    else {
        reportName = newFullName;
    }
    var saveAsDialog = document.getElementById('saveAsDialog');
    saveAsDialog.style.display = 'none';
    SaveReportSet();
}

function SaveReportSet() {
    var fieldWithRn = document.getElementById('reportNameFor2ver');
    var rnVal;
    if (fieldWithRn != null)
        rnVal = fieldWithRn.value;
    else if (reportName == undefined || reportName == null)
        rnVal = '';
    else
        rnVal = reportName;
    while (rnVal.indexOf('+') >= 0) {
        rnVal = rnVal.replace('+', ' ');
    }
    if (rnVal == null || rnVal == '') {
        ShowSaveAsDialog();
        return;
    }
    var loadingrv2 = document.getElementById('loadingrv2');
    var windowHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
    loadingrv2.style.height = windowHeight + 'px';
    loadingrv2.style.paddingTop = ((windowHeight / 2) - 100) + 'px';
    loadingrv2.style.display = '';
    var requestString = 'wscmd=savecurrentreportset';
    requestString += '&wsarg0=' + rnVal;
    AjaxRequest('./rs.aspx', requestString, ReportSetSaved, null, 'savecurrentreportset');
}

function ReportSetSaved(returnObj, id) {
    if (id != 'savecurrentreportset' || returnObj == undefined || returnObj == null || returnObj.Value == null)
        return;
    document.getElementById('loadingrv2').style.display = 'none';
    if (returnObj.Value != 'OK') {
        alert(returnObj.Value);
    }
    else {
        var fieldWithRn = document.getElementById('reportNameFor2ver');
        var rnVal;
        if (fieldWithRn != null) {
            rnVal = fieldWithRn.value;
        }
        else {
            rnVal = reportName;
        }
        modifyUrl('rn', rnVal);
    }
}

function ShowNewCatDialog() {
    document.getElementById('addedCatName').value = '';
    var saveAsDialog = document.getElementById('saveAsDialog');
    saveAsDialog.style.display = 'none';
    var newCatDialog = document.getElementById('newCatDialog');
    var windowHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
    newCatDialog.style.height = windowHeight + 'px';
    newCatDialog.style.paddingTop = ((windowHeight / 2) - 100) + 'px';
    newCatDialog.style.display = '';
}

function AddNewCategory() {
    additionalCategories[additionalCategories.length] = document.getElementById('addedCatName').value;
    var newCatDialog = document.getElementById('newCatDialog').style.display = 'none';
    GetCategoriesList(false);
}
//------------------------------------------------------------------------------------------------------------------------


//Initialization----------------------------------------------------------------------------------------------------------------------
var reportName;
var urlSettings;
var initialized;
var nrvConfig;

function InitializePopup() {
    fieldPopup = $("#data-source-field").dialog({
        autoOpen: false,
        width: 860,
        height: "auto",
        modal: true,
        buttons: {
            "OK": function () {
                updateFieldProperties();
                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        open: function () {
            $(this).parents(".ui-dialog-buttonpane button:eq(0)").focus();
        },
        show: { effect: "fade", duration: 200 },
        hide: { effect: "fade", duration: 200 }
    });
}

function InitializeViewer() {
    initialized = false;
    InitializePopup();
    GetReportViewerConfig();
}

function GetReportViewerConfig() {
    var requestString = 'wscmd=reportviewerconfig';
    AjaxRequest('./rs.aspx', requestString, GotReportViewerConfig, null, 'reportviewerconfig');
}

function GotReportViewerConfig(returnObj, id) {
    if (id != 'reportviewerconfig' || returnObj == undefined || returnObj == null)
        return;
    nrvConfig = returnObj;

    if (urlSettings == undefined) {
        urlSettings = new UrlSettings(nrvConfig.ReportListUrl, nrvConfig.ResponseServerUrl);
    }

  	responseServer = new AdHoc.ResponseServer(urlSettings.urlRsPage, 0);
  	responseServerWithDelimeter = responseServer;
  	ChangeTopRecords(nrvConfig.InitialResults, false);
		if (urlSettings.reportInfo.exportType != null) {
		  responseServer.OpenUrlWithModalDialogNewCustomRsUrl('rs.aspx?output=' + urlSettings.reportInfo.exportType, 'aspnetForm', 'reportFrame', nrvConfig.ResponseServerUrl);
		};
    ApplySecurityOptions();
    GetRenderedReportSet(false);
}

function ApplySecurityOptions() {
    if (nrvConfig.ReportIsReadOnly == true) {
        $('.hide-readonly').hide();
        $('#btnSaveDirect').attr('disabled', 'disabled');
    }
    if (nrvConfig.ReportIsViewOnly == true)
        $('.hide-viewonly').hide();
    if (nrvConfig.ReportIsLocked == true)
        $('.hide-locked').hide();
}

function GetRenderedReportSet(invalidateInCache) {
    var renderedReportDiv = document.getElementById('renderedReportDiv');
    var loadingHtml = '<div id="loadingDiv" style="width: 100%; text-align: center;">';
    loadingHtml += '<div id="loadingWord" style="font-size: 20px;color:#1D5987;font-family:Verdana,Arial,Helvetica,sans-serif;font-weight:normal !important;font-size: 20px; font-style: normal;">Loading...</div>';
    loadingHtml += '<img style="padding-top: 40px;" id="loadingImg" alt="" src=\'rs.aspx?image=loading.gif\' />';
    loadingHtml += '</div>';
    renderedReportDiv.innerHTML = loadingHtml;
    var requestString = 'wscmd=getrenderedreportset';
    var urlParams = '';
    var queryParameters = {}, queryString = location.search.substring(1),
              re = /([^&=]+)=([^&]*)/g, m;
    while (m = re.exec(queryString)) {
        var pName = decodeURIComponent(m[1]).toLowerCase();
        queryParameters[pName] = decodeURIComponent(m[2]);
        if (pName != 'rn') {
            if (urlParams == '') {
                urlParams += '?';
            }
            else {
                urlParams += '&';
            }
            urlParams += pName + '=' + decodeURIComponent(m[2]);
        }
    }
    if (queryParameters['rn'] != null && queryParameters['rn'].length > 0 && !initialized) {
        reportName = '';
        var rnParam = '';    
        rnParam = queryParameters['rn'];
        reportName = queryParameters['rn'];
        requestString += '&wsarg0=' + rnParam;
    }
    if (invalidateInCache) {
        if (urlParams == '') {
            urlParams += '?';
        }
        else {
            urlParams += '&';
        }
        urlParams += '&iic=1';
    }
    AjaxRequest('./rs.aspx' + urlParams, requestString, GotRenderedReportSet, null, 'getrenderedreportset');
}

function EvalGlobally(data) {
    (window.execScript || function (data) {
        window["eval"].call(window, data);
    })(data);
}

function GotRenderedReportSet(returnObj, id) {
    if (id != 'getrenderedreportset' || returnObj == undefined || returnObj == null)
        return;
    if (returnObj == null || returnObj.length == 0)
        return;
    var renderedReportDiv = document.getElementById('renderedReportDiv');
    renderedReportDiv.innerHTML = returnObj;
    var containerToLaunch = document.getElementById('launchmeplease');
    while (containerToLaunch != null) {
        var codeToLaunch = containerToLaunch.innerHTML;
        if (codeToLaunch != null && codeToLaunch != '') {
            try {
                if (codeToLaunch.substr(0, 4) == '<!--') {
                  codeToLaunch = codeToLaunch.substr(4, codeToLaunch.length - 7);
                }
                EvalGlobally(codeToLaunch)
            }
            catch (e) {
                alert('Form embedded JS error: ' + e);
            }
        }
        containerToLaunch.id = '';
        containerToLaunch = document.getElementById('launchmeplease');
    }
    if (initialized)
        return;
    initialized = true;
    FirstLoadInit();
}

function FirstLoadInit() {
    var designerBtn = document.getElementById('designerBtn');
    designerBtn.onclick = function () { window.location = nrvConfig.ReportDesignerUrl + '?rn=' + reportName; };
    $('#navdiv ul li a').click(function () {
        var currentTab = $(this).attr('href');
        var vis = $(currentTab).is(':visible');
        $('#tabsContentsDiv > div').hide();
        $(this).parent().removeClass('active');
        if (!vis) {
            $(currentTab).show();
            document.getElementById('updateBtnPC').style.display = 'none';
        }
        else {
            document.getElementById('updateBtnPC').style.display = '';
            setTimeout(function () {
                $('#navdiv ul li').removeClass('active');
                $('#navdiv ul li a').removeClass('active');
            }, 100);
        }
    });
    var fieldWithRn = document.getElementById('reportNameFor2ver');
    var rnVal;
    if (fieldWithRn != null)
        rnVal = fieldWithRn.value;
    else if (reportName == undefined || reportName == null)
        rnVal = '';
    else
        rnVal = reportName;

    while (rnVal.indexOf('+') >= 0) {
        rnVal = rnVal.replace('+', ' ');
    }
    var frNodes = rnVal.split('\\\\');
    var hdr = '<h1 style=\"margin-left:40px;\">' + frNodes[frNodes.length - 1] + (frNodes.length <= 1 ? '' : ' <i>(' + frNodes[frNodes.length - 2] + ')</i>') + '</h1>';
    var repHeader = document.getElementById('repHeader');
    repHeader.innerHTML = hdr;
    InitializeFields();
    RefreshPivots();
    var tab1a = document.getElementById('tab1a');
    tab1a.click();
    GetFiltersData();
}

function GetDatasourcesList() {
    var requestString = 'wscmd=crsdatasources';
    AjaxRequest('./rs.aspx', requestString, GotDatasourcesList, null, 'crsdatasources');
}

function GotDatasourcesList(returnObj, id) {
    if (id != 'crsdatasources' || returnObj == undefined || returnObj == null)
        return;
    if (returnObj.SelectionsList == null || returnObj.SelectionsList.length == 0)
        return;
    selectionsList = returnObj.SelectionsList;
    var dsUlList = document.getElementById('dsUlList');
    var allOpt = new Option();
    allOpt.value = "###all###";
    allOpt.text = "All";
    dsUlList.add(allOpt);
    for (var dsCnt = 0; dsCnt < selectionsList.length; dsCnt++) {
        var opt = new Option();
        opt.value = selectionsList[dsCnt].DbName;
        opt.text = selectionsList[dsCnt].FriendlyName;
        dsUlList.add(opt);
    }
    if (selectionsList.length == 1) {
        var dsUlList = document.getElementById('dsUlList');
        dsUlList.style.display = 'none';
    }
    currentDatasource = -1;
    wereChecked.length = 0;
    RefreshFieldsList();
    //var saveControls = document.getElementById('saveControls');
    //if (returnObj.CanSaveReport)
    //    saveControls.style.display = '';
    //else
    //    saveControls.style.display = 'none';
}

function InitializeFields() {
    GetDatasourcesList();
}

function CheckNewCatName() {
    var newCategoryName = document.getElementById('newCategoryName');
    if (newCategoryName.value == '(Create new)')
        ShowNewCatDialog();
    else
        prevCatValue = newCategoryName.value;
}

function CancelSave() {
    var saveAsDialog = document.getElementById('saveAsDialog');
    saveAsDialog.style.display = 'none';
}

function CancelAddCategory() {
    var newCatDialog = document.getElementById('newCatDialog').style.display = 'none';
    var saveAsDialog = document.getElementById('saveAsDialog');
    var windowHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
    saveAsDialog.style.height = windowHeight + 'px';
    saveAsDialog.style.paddingTop = ((windowHeight / 2) - 100) + 'px';
    saveAsDialog.style.display = '';
    var newCategoryName = document.getElementById('newCategoryName');
    newCategoryName.value = prevCatValue;
}
//---------------------------------------------------------------------------------------------------------------------------

//Tooltip functionality from old reportViewer----------------------------------------------------------------------------
// Href tooltips functions
var FADINGTOOLTIP;
var FadingTooltipList = {};
var wnd_height, wnd_width;
var tooltip_height, tooltip_width;
var tooltip_shown=false;
var	transparency = 100;
var timer_id = 1;
			
function DisplayTooltip(tooltip_url)
{
	tooltip_shown = (tooltip_url != "")? true : false;
	
	if (tooltip_shown)
	{
		FADINGTOOLTIP = FadingTooltipList[tooltip_url];
		if (FADINGTOOLTIP==null)
		{
			var obody=document.getElementsByTagName('body')[0];
			var frag=document.createDocumentFragment();
			FADINGTOOLTIP = document.createElement('div');
			FADINGTOOLTIP.style.border = "darkgray 1px outset";
			FADINGTOOLTIP.style.width = "auto";
			FADINGTOOLTIP.style.height = "auto";
			FADINGTOOLTIP.style.color = "black";
			FADINGTOOLTIP.style.backgroundColor = "white";
			FADINGTOOLTIP.style.position = "absolute";
			FADINGTOOLTIP.style.zIndex = 1000;
			frag.appendChild(FADINGTOOLTIP);
			obody.insertBefore(frag,obody.firstChild);
			window.onresize = UpdateWindowSize;
			document.onmousemove = AdjustToolTipPosition;
			
			FADINGTOOLTIP.innerHTML = "Loading...<br><image src='" + nrvConfig.ResponseServerUrl + "?image=loading.gif'/>";
			//EBC_GetData(tooltip_url, null, DisplayTooltip_CallBack, tooltip_url);
			responseServer.RequestData(tooltip_url, DisplayTooltip_CallBack);
		}
		FADINGTOOLTIP.style.display = "";
		FADINGTOOLTIP.style.visibility = "visible";
		FadingTooltipList[tooltip_url] = FADINGTOOLTIP;
	}
	else
	{
		if (FADINGTOOLTIP != null)
		{
			clearTimeout(timer_id);
			FADINGTOOLTIP.style.visibility="hidden";
			FADINGTOOLTIP.style.display = "none";
		}
	}
}

function DisplayTooltip_CallBack(url, xmlHttpRequest)
{
	if (xmlHttpRequest.status==200)
	{	
		var toolTip = FadingTooltipList[url];
		toolTip.innerHTML = xmlHttpRequest.responseText;
		if (toolTip == FADINGTOOLTIP)
			tooltip_height=(FADINGTOOLTIP.style.pixelHeight)? FADINGTOOLTIP.style.pixelHeight : FADINGTOOLTIP.offsetHeight;
		transparency=0;
	}
}

function AdjustToolTipPosition(evt)
{
	evt = (evt) ? evt : window.event;
	if(tooltip_shown)
	{
		var scrollTop = document.documentElement.scrollTop + document.body.scrollTop;
		var scrollLeft = document.documentElement.scrollLeft + document.body.scrollLeft;
		WindowLoading();
		
		var offset_y = evt.clientY + tooltip_height + 15;
		var top = evt.clientY + scrollTop + 15;
		if (offset_y > wnd_height)
		{
			var offset_y2 = evt.clientY - tooltip_height - 15;
			top = evt.clientY - tooltip_height + scrollTop -15;
			if (offset_y2 < 0)
			{
				top = (wnd_height - tooltip_height) / 2 + scrollTop;
			}
		}
		
		var offset_x = evt.clientX + tooltip_width + 15;
		var left = evt.clientX + scrollLeft + 15;
		if (offset_x > wnd_width)
		{
			var dx = offset_x - wnd_width;
			var offset_x2 = evt.clientX - tooltip_width - 15;
			var left2 = evt.clientX - tooltip_width + scrollLeft - 15;
			if (offset_x2 < 0)
			{
				var dx2 = -offset_x2;
				if (dx2 < dx)
					left = left2;
			}
			else
			{
				left = left2;
			}
	}
	if (tooltip_height > 100) {
	  top = scrollTop + evt.clientY - tooltip_height / 3;
	}
	
		FADINGTOOLTIP.style.left =  left + 'px';
		FADINGTOOLTIP.style.top = top + 'px';
	}
}

function WindowLoading()
{
	tooltip_width = (FADINGTOOLTIP.style.pixelWidth) ? FADINGTOOLTIP.style.pixelWidth : FADINGTOOLTIP.offsetWidth;
	tooltip_height=(FADINGTOOLTIP.style.pixelHeight)? FADINGTOOLTIP.style.pixelHeight : FADINGTOOLTIP.offsetHeight;
	UpdateWindowSize();
}
			
function ToolTipFading()
{
	if(transparency <= 100)
	{
		FADINGTOOLTIP.style.filter="alpha(opacity="+transparency+")";
		transparency += 5;
		timer_id = setTimeout('ToolTipFading()', 35);
	}
}

function UpdateWindowSize() 
{
	wnd_height=document.body.clientHeight;
	wnd_width=document.body.clientWidth;
}
//---------------------------------------------------------------------------------------------------------