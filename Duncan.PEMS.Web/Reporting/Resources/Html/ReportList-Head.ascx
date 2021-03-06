<%@ Control Language="C#" AutoEventWireup="true" %>

	<link rel="stylesheet" type="text/css" href="./rs.aspx?css=ModalDialogStyle" />
	<link rel="stylesheet" type="text/css" href="./rs.aspx?css=reportThumbnail" />
    <script type="text/javascript" src="./rs.aspx?js=jQuery.jq"></script>
	<script type="text/javascript" src="./rs.aspx?js=jQuery.jqui"></script>
	<script type="text/javascript" src="./rs.aspx?js=Utility"></script>
	<script type="text/javascript" src="./rs.aspx?js=ModalDialog"></script>
	<script type="text/javascript" src="./rs.aspx?js=AdHocServer"></script>
	<script type="text/javascript" src="./rs.aspx?js=ReportList"></script>
	<script type="text/javascript">
		var lastlySelectedCat = 0;
		var tabNames = new Array();

		//Ajax request for JSON methods-----------------------------------------------------------
		function AjaxRequest(url, parameters, callbackSuccess, callbackError, id) {
			var thisRequestObject = null;
			if (window.XMLHttpRequest)
				thisRequestObject = new XMLHttpRequest();
			else if (window.ActiveXObject)
				thisRequestObject = new ActiveXObject("Microsoft.XMLHTTP");
			thisRequestObject.requestId = id;
			thisRequestObject.onreadystatechange = ProcessRequest;

			thisRequestObject.open('GET', url + '?' + parameters, true);
			thisRequestObject.send();

			function DeserializeJson() {
				var responseText = thisRequestObject.responseText;
				while (responseText.indexOf('"\\/Date(') >= 0) {
					responseText = responseText.replace('"\\/Date(', 'eval(new Date(');
					responseText = responseText.replace(')\\/"', '))');
				}
				if (responseText.charAt(0) != '[' && responseText.charAt(0) != '{')
					responseText = '{' + responseText + '}';
				var isArray = true;
				if (responseText.charAt(0) != '[') {
					responseText = '[' + responseText + ']';
					isArray = false;
				}
				var retObj = eval(responseText);
				if (!isArray)
					return retObj[0];
				return retObj;
			}

			function ProcessRequest() {
				if (thisRequestObject.readyState == 4) {
					if (thisRequestObject.status == 200 && callbackSuccess) {
						callbackSuccess(DeserializeJson(), thisRequestObject.requestId);
					}
					else if (callbackError) {
						callbackError(thisRequestObject);
					}
				}
			}
		}

		//Delete ReportSet methods--------------------------------------------------------------------
		function RL_DeleteNew(message, reportName) {
			var userData = new ud();
			userData.reportName = reportName;
			modal_confirm(message, userData, RL_DeleteCallbackNew);
		}

		function RL_DeleteCallbackNew(userData, isConfirmed) {
			if (isConfirmed) {
			  ShowContentLoading(function(){PerformDelete(userData);}, 'Deleting...');
			}
		}
		
		function PerformDelete(userData) {
			var requestString = 'wscmd=deletereportset';
			requestString += '&wsarg0=' + userData.reportName;
			AjaxRequest('./rs.aspx', requestString, ReportSetDeleted, null, 'deletereportset');		
		}

		function ReportSetDeleted(returnObj, id) {
			if (id != 'deletereportset')
				return;
			RL_SearchReports();
		}

		//Elements UI events methods-----------------------------------------------------------------
		var searchInputTimer;
		var searchKeyword = '';

		function ShowHideCategory(catId) {
			var catDiv = document.getElementById(catId);
			if (catDiv.style.display == 'none')
				catDiv.style.display = '';
			else
				catDiv.style.display = 'none';
		}

		function RL_FocusSearch(searchBox) {
			if (searchBox.value == "Search") {
				searchBox.value = "";
				searchBox.style.color = "#000000";
			}
		}

		function RL_BlurSearch(searchBox) {
			if (searchBox.value == "") {
				searchBox.value = "Search";
				searchBox.style.color = "#B0B0B0";
			}
		}

		function RL_SearchReports() {
			GetReports(searchKeyword, tabNames[lastlySelectedCat]);
		}

		function RL_SearchInputStartTimeout(searchString) {
			jq$("#RL_SearchingIcon").css("display", "none");
			searchKeyword = searchString;
			clearTimeout(searchInputTimer);
			jq$("#RL_SearchingIcon").css("display", "");
			searchInputTimer = setTimeout(RL_SearchReports, 1000);
		}

		//Exchange data with server and list rendering methods--------------------------------------------
		var UseDefaultDialogs;
		var responseServer;
		var nrlConfigObj;
		var currentThId = 0;

		function GetUrlParam(name) {
			name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
			var regexS = "[\\?&]" + name + "=([^&#]*)";
			var regex = new RegExp(regexS);
			var results = regex.exec(window.location.href);
			if (results == null)
				return "";
			else
				return results[1];
		}

		function GetConfig() {
			var instant = GetUrlParam('instant');
			if (instant != '1')
				instant = '0';
			var requestString = 'wscmd=reportlistconfig&wsarg0=' + instant;
			AjaxRequest('./rs.aspx', requestString, AcceptConfig, null, 'reportlistconfig');
		}

		function AcceptConfig(returnObj, id) {
			if (id != 'reportlistconfig' || returnObj == undefined || returnObj == null)
				return;
			nrlConfigObj = returnObj;
			if (!nrlConfigObj.Operational) {
			  window.location = nrlConfigObj.SettingsLink;
			  return;
			}			
			UseDefaultDialogs = nrlConfigObj.UseDefaultDialogs;
			responseServer = new AdHoc.ResponseServer(nrlConfigObj.ResponseServerUrl, nrlConfigObj.TimeOut);
			var reportDesignerLink = jq$("#newReportLink");
			var dashboardDesignerLink = jq$("#newDashboardLink");
			if (reportDesignerLink != null) {
				reportDesignerLink.href = nrlConfigObj.ReportDesignerLink;
				reportDesignerLink.target = nrlConfigObj.ReportDesignerTarget;
			}
			if (dashboardDesignerLink != null)
			    dashboardDesignerLink.href = nrlConfigObj.DashboardDesignerLink;
			var irdivlink = document.getElementById('irdivlink');
			if (irdivlink != null)
			  irdivlink.onclick = function () { window.location = nrlConfigObj.InstantReportUrl; }
			GetReports(searchKeyword, '');
		}

		function GetReports(keyword, category) {
			var requestString = 'wscmd=reportlistdatalite';
			requestString += '&wsarg0=' + category;
			if (keyword != null && keyword != '')
				requestString += '&wsarg1=' + keyword;
			AjaxRequest('./rs.aspx', requestString, AcceptReports, GetReportsFail, 'reportlistdatalite');
		}

		function LeftTabClicked(tabClicked, tabsNum) {
			for (var index = 0; index < tabsNum; index++) {
				jq$('#ltab' + index).removeClass('selected');
			}
			//jq$('#tabs').tabs({ selected: tabClicked });
			jq$('#ltab' + tabClicked).addClass('selected');
			lastlySelectedCat = tabClicked;
		}

    function GetReportsFail(returnObj) {
      if (returnObj.requestId != 'reportlistdatalite') {
        return;
      }
      var startEx = returnObj.responseText.indexOf('<' + '!' + '-' + '-');
      if (startEx < 0) {
        return;
      }
      var exMsg = returnObj.responseText.substr(startEx + 4, returnObj.responseText.length - startEx - 7);
			var reportsDiv = document.getElementById('reportListDiv');
			jq$("#RL_SearchingIcon").css("display", "none");
			jq$("#loadingDiv").css("display", "none");			
			reportsDiv.innerHTML = exMsg;
			document.getElementById('reportListDiv').style.visibility = 'visible';			
    }

		function AcceptReports(returnObj, id) {
			if (id != 'reportlistdatalite' || returnObj == undefined || returnObj == null ||
						returnObj.ReportSets == undefined || returnObj.ReportSets == null) {
				jq$("#RL_SearchingIcon").css("display", "none");
				jq$("#loadingDiv").css("display", "none");
				return;
			}
			var content = '';
			var leftContent = '';
			var tabs = new Array();
			if (returnObj.ReportSets.length == 0) {
				jq$("#loadingDiv").css("display", "none");
				jq$("#addInstantReportContainerDiv").css("display", "");
				ShowContentLoading(setTimeout(function() {window.location = nrlConfigObj.ReportDesignerLink;}, 11000), 'No reports currently exist. Please create and save at least one in the report designer.<br />Redirecting to the Report Designer in 10 seconds.');
				return;
		} else {
				for (var rCnt1 = 0; rCnt1 < returnObj.ReportSets.length; rCnt1++) {
					var catName = returnObj.ReportSets[rCnt1].Category;
					if (catName == null || catName == '')
						catName = 'Uncategorized';
					var tabIndex = -1;
					for (var tabCnt1 = 0; tabCnt1 < tabs.length; tabCnt1++) {
						if (tabs[tabCnt1].TabName == catName) {
							tabIndex = tabCnt1;
							break;
						}
					}
					if (tabIndex < 0) {
						tabIndex = tabs.length;
						tabs[tabIndex] = new Object();
						tabs[tabIndex].TabName = catName;
						tabs[tabIndex].Reports = new Array();
						tabNames[tabIndex] = catName;
					}
					tabs[tabIndex].Reports[tabs[tabIndex].Reports.length] = returnObj.ReportSets[rCnt1];
				}
				if (tabs.length <= 0) {
					jq$("#RL_SearchingIcon").css("display", "none");
					return;
				}
		  }
			content += '<div id="tabs"><ul style="display:none;">';
			leftContent += '<ul>';
			for (var tabCnt2 = 0; tabCnt2 < tabs.length; tabCnt2++) {
				content += '<li style="font-size:0.9em;"><a href="#tab' + tabCnt2 + '">' + tabs[tabCnt2].TabName + '</a></li>';
				var leftSelected = '';
				if (tabCnt2 == lastlySelectedCat)
					leftSelected = ' class="selected"';
				leftContent += '<li><a id="ltab' + tabCnt2 + '" href="#"' + leftSelected
                    + ' onclick="LeftTabClicked(' + tabCnt2 + ', ' + tabs.length + '); ShowContentLoading(function(){GetReports(searchKeyword, tabNames[' + tabCnt2 + '])}, \'Loading...\');">'
                    + tabs[tabCnt2].TabName + '</a></li>';
			}
			content += '</ul>';
			leftContent += '</ul>';
			var grCnt = 0;
			for (var tabCnt = 0; tabCnt < tabs.length; tabCnt++) {
				content += '<div id="tab' + tabCnt + '">';
				content += '<div class="thumbs">';
				for (var rCnt = 0; rCnt < tabs[tabCnt].Reports.length; rCnt++) {
				  if (tabs[tabCnt].Reports[rCnt].Name == null || tabs[tabCnt].Reports[rCnt].Name == '')
				    continue;
				  grCnt++;
				  var report = tabs[tabCnt].Reports[rCnt];
				  if (report.Dashboard && forSelection)
				    continue;
				  var fullReportName = report.Name;
				  if (report.Category != null && report.Category != '')
				    fullReportName = report.Category + '\\' + fullReportName;
				  var escapedFullRn = fullReportName.replace('\'', '\\\'');
				  var templateLink = nrlConfigObj.InstantReportUrl + "?rn=" + report.UrlEncodedName;
				  var printLink = "\'rs.aspx?rn=" + report.UrlEncodedName + "&print=1\'";
				  var editLink = report.UrlEncodedName;
				  if (report.Dashboard)
				    editLink = nrlConfigObj.DashboardDesignTemplate[0] + editLink + nrlConfigObj.DashboardDesignTemplate[1];
				  else
				    editLink = nrlConfigObj.ReportDesignTemplate[0] + editLink + nrlConfigObj.ReportDesignTemplate[1];
				  var deleteLink = 'javascript:RL_DeleteNew(\'Are you sure you want to delete ' + escapedFullRn + '?\', \'' + report.UrlEncodedName + '\');';
				  var viewLink = "";
				  if (report.Dashboard)
				      viewLink = nrlConfigObj.DashboardViewTemplate[0] + report.UrlEncodedName + nrlConfigObj.DashboardViewTemplate[1];
				  else
				    viewLink = nrlConfigObj.ReportViewTemplate[0] + report.UrlEncodedName + nrlConfigObj.ReportViewTemplate[1];
				  if (!forSelection) {
				    content += '<div class="thumb" onclick="' + viewLink + '" id="">';
				    content += '<div class="thumb-container"><img src="' + report.ImgUrl + '" />';
				    content += '<div class="thumb-buttons">';
				    if (!report.ReadOnly && !report.ViewOnly && !report.IsLocked) {
				        content += '<div class="thumb-edit" onclick="event.cancelBubble = true;(event.stopPropagation) ? event.stopPropagation() : event.returnValue = false;(event.preventDefault) ? event.preventDefault() : event.returnValue = false;' + editLink + '" title="Edit"></div>';
				        content += '<div class="thumb-remove" onclick="event.cancelBubble = true;(event.stopPropagation) ? event.stopPropagation() : event.returnValue = false;(event.preventDefault) ? event.preventDefault() : event.returnValue = false;' + deleteLink + '" title="Remove"></div>';
				    }
                    content += '<div class="thumb-print" onclick="event.cancelBubble = true;(event.stopPropagation) ? event.stopPropagation() : event.returnValue = false;(event.preventDefault) ? event.preventDefault() : event.returnValue = false;window.open(' + printLink + ', \'_blank\');" title="Print"></div>';
				    content += '</div>';
				    content += '</div>';
				    content += '<div class="thumb-title">' + report.Name + '</div>';
				    content += '</div>';
				  }
				  else {
				    content += '<div class="thumb" onclick="parent.ReportForPartSelected(\'' + report.UrlEncodedName + '\');" id="">';
				    content += '<div class="thumb-container"><img src="' + report.ImgUrl + '" />';
				    content += '</div>';
				    content += '<div class="thumb-title">' + report.Name + '</div>';
				    content += '</div>';				    
				  }
				}
			  content += '</div>';
				content += '</div>';
			}
			content += '</div>';
			var recentContent = '<ul>';
			for (var index = 0; index < returnObj.Recent.length; index++) {
				var reportR = returnObj.Recent[index];
				var viewLinkR = nrlConfigObj.ReportViewTemplate[0] + reportR.UrlEncodedName + nrlConfigObj.ReportViewTemplate[1];
				if (reportR.Dashboard)
				  viewLinkR = nrlConfigObj.DashboardViewTemplate[0] + reportR.UrlEncodedName + nrlConfigObj.DashboardViewTemplate[1];
				recentContent += '<li><a onclick="' + viewLinkR + '" href="#">' + reportR.Name + '</a></li>';
			}
			recentContent += '</ul>';
			jq$("#reportListDiv").css("display", "none");
			var reportsDiv = document.getElementById('reportListDiv');
			reportsDiv.innerHTML = content;
			var leftCatsDiv = document.getElementById('leftSideCats');
			leftCatsDiv.innerHTML = leftContent;
			var recentDiv = document.getElementById('recentReports');
			recentDiv.innerHTML = recentContent;
			jq$('#tabs').tabs({
			  fx: { opacity: 'toggle' }
			});
			if (lastlySelectedCat > 0)
			    LeftTabClicked(lastlySelectedCat, tabs.length);
			
			jq$('#loadingDiv').hide('fade', null, 400, function () {
			    document.getElementById('reportListDiv').style.visibility = 'visible';
			    jq$('#contentDiv').css('cursor', 'default');
			    jq$('#reportListDiv').show('fade', 1000);
			});
			document.getElementById('RL_SearchingIcon').style.display = "none";
		}

		function ShowContentLoading(callback, text) {
		    jq$('#contentDiv').css('cursor', 'wait');
		    jq$('#reportListDiv').hide('fade', null, 600, ShowLoadingAfterFade(text, callback));
		}
		
		function ShowLoadingAfterFade(text, callback) {
      var vSize = document.body.offsetHeight;
      jq$('#loadingDiv').css('height', vSize + 'px');
      jq$('#loadingWord').css('margin-top', (vSize / 3) + 'px');
      var loadingWord = document.getElementById('loadingWord');
      loadingWord.innerHTML = text;
      jq$('#loadingDiv').show('fade', null, 400, callback);		
		}

		var forSelection;

		jq$(document).ready(function() {
		  forSelection = GetUrlParam('justSelect') == '1';
		  if (forSelection) {
		    document.getElementById('whiteHeader').style.display = 'none';
		    document.getElementById('blueHeader').style.display = 'none';
		  }

		  var vSize = document.body.offsetHeight;
		  jq$('#loadingDiv').css('height', vSize + 'px');
		  jq$('#loadingWord').css('margin-top', (vSize / 3) + 'px');
		  
			GetConfig();
		});
	</script>
