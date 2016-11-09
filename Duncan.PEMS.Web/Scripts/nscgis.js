//** Declare variables
//** JS variables
var map;
var printCanvasURL;
var printImage;
var invMgr = [];
var hdbMgr = [];
var pmoMgr = [];
var InvMarkerRepository = [];
var HdbMarkerRepository = [];
var PmoMarkerRepository = [];
var animateOnce = 0;
var combinedResults;
var directionsResultsReturned = 0;
var carMarkers = [];
var CustomerLat;
var CustomerLng;
var CustomerIDIs;
var legs;
var markerPositions = [];

var InvPolygonRepository_Area = [];
var InvPolylineRepository_Area = [];
var InvPolygonRepository_Zone = [];
var InvPolylineRepository_Zone = [];

var HdbPolygonRepository_Area = [];
var HdbPolylineRepository_Area = [];
var HdbPolygonRepository_Zone = [];
var HdbPolylineRepository_Zone = [];

var PmoPolygonRepository_Area = [];
var PmoPolylineRepository_Area = [];
var PmoPolygonRepository_Zone = [];
var PmoPolylineRepository_Zone = [];
var infowindow = new google.maps.InfoWindow();
var legendControlDiv;

//**************************
//var map;
var directionDisplay;
var directionsService;
var position;
var marker = null;
var polyline = null;
var poly2 = null;
var speed = "0.000005";
//var infowindow = null;
var stepDisplay;
var step = 5; //meters
var steps = []
var stepnum = 0;
var tick = 70; // milliseconds
var eol;
var k = 0;
var speed = "";
var lastVertex = 0;
var timerHandle = null;
var poly2_Arr = [];
var polyDistance;
var isAnimStopped = false;
//****************************


$(document).ready(function () {

    //** Display Google map for JS v3
    // loadGoogleMap(34.068030, -118.473127);
});



function goFullscreen(id) {

    if (window.fullScreenApi.supportsFullScreen) {

        // Get the element that we want to take into fullscreen mode
        var element = document.getElementById(id);

        // These function will not exist in the browsers that don't support fullscreen mode yet, 
        // so we'll have to check to see if they're available before calling them.
        if (element.mozRequestFullScreen) {
            // This is how to go into fullscren mode in Firefox
            // Note the "moz" prefix, which is short for Mozilla.
            element.mozRequestFullScreen();
        } else if (element.webkitRequestFullScreen) {
            // This is how to go into fullscreen mode in Chrome and Safari
            // Both of those browsers are based on the Webkit project, hence the same prefix.
            var mapContent = document.getElementById('map_canvas');
            mapContent.style.maxWidth = "100%";
            mapContent.style.minWidth = "100%";
            element.webkitRequestFullScreen();
        } else {

        }


    } else {
        alert('SORRY: Your browser does not support FullScreen');
    }

}

function zoomCustomerToCenter(LatIs, LngIs) {
    //alert('LatIs' + LatIs + "  " + LngIs);
    map.setCenter(new google.maps.LatLng(LatIs, LngIs))
    map.setZoom(14)
    //alert('gzs after' + map.getZoom());

}

function zoomCustomerToCenterForOrleans(LatIs, LngIs) {
    map.setZoom(15)
    map.setCenter(new google.maps.LatLng(LatIs, LngIs))


}

function orleans(custID) {
    CustomerIDIs = custID;
}

function loadGoogleMap(LatIs, LngIs) {

    poly2_Arr = [];

    CustomerLat = LatIs;
    CustomerLng = LngIs;


    // Instantiate a directions service.
    directionsService = new google.maps.DirectionsService();

    // Create a map and center it. 
    var myOptions = {
        zoom: 16,
        rotateControl: true,
        zoomControl: true,
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.LARGE
        },
        center: new google.maps.LatLng(LatIs, LngIs),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }

    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

    // Create a renderer for directions and bind it to the map.
    var rendererOptions = {
        map: map,
        suppressMarkers: true
    }
    directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);

    // Instantiate an info window to hold step text.
    stepDisplay = new google.maps.InfoWindow();

    polyline = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 3
    });

    poly2 = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 3
    });


    // Create the DIV to hold the control and call the HomeControl() constructor
    // passing in this DIV.

    var fullScreenControlDiv = document.createElement('div');
    var fSControl = new FullScreenControl(fullScreenControlDiv, map);

    fullScreenControlDiv.index = 1;

    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(fullScreenControlDiv);

    var printControlDiv = document.createElement('div');
    var printControl = new printMapControl(printControlDiv, map);

    printControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(printControlDiv);

    legendControlDiv = document.createElement('div');
    var legendControl = new showLegendControl(legendControlDiv, map);
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(legendControlDiv);


    clearMapControlDiv = document.createElement('div');
    var cmControl = new clearMapControl(clearMapControlDiv, map);
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(clearMapControlDiv);

    //if (viewName == "officerRoute")
    //{
    var playPauseControlDiv = document.createElement('div');
    var ppControl = new playPauseControl(playPauseControlDiv, map);
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(playPauseControlDiv);
    //}

    map.addListener('zoom_changed', function () {
        //alert('my zoom' + map.getZoom());
    });



}



function onAnimationPlayPause(control) {

    if (control == "stop") {
        isAnimStopped = true;
        clearTimeout(timerHandle); //stop the animation
        timerHandle = null;
        tick = null;
    } else {
        isAnimStopped = false;
        animate(polyDistance);
    }
}

function runRoutes(routeData, legendImagesArr, isAnimate) {

    var batches = [];
    var itemsPerBatch = 10; // google API max - 1 start, 1 stop, and 8 waypoints
    var itemsCounter = 0;
    var wayptsExist = routeData.length > 0;

    while (wayptsExist) {
        var subBatch = [];
        var subitemsCounter = 0;

        for (var j = itemsCounter; j < routeData.length; j++) {
            subitemsCounter++;
            subBatch.push(routeData[j]);
            if (subitemsCounter == itemsPerBatch)
                break;
        }

        itemsCounter += subitemsCounter;
        batches.push(subBatch);
        wayptsExist = itemsCounter < routeData.length;
        // If it runs again there are still points. Minus 1 before continuing to
        // start up with end of previous tour leg
        itemsCounter--;
    }
    //*****************************
    isAnimStopped = false;
    playPauseTxt.innerHTML = 'Pause';
    polyDistance = 50;

    //** Clear all the previous plotted overlays on the map
    if (poly2_Arr.length > 0) {
        for (var i = 0; i < poly2_Arr.length; i++) {
            poly2_Arr[i].setMap(null);
        }
        poly2_Arr = [];
    }
    if (timerHandle) {
        clearTimeout(timerHandle);
    }



    if (polyline != null) {
        polyline.setMap(null);
        polyline = null;
    }

    if (poly2.getPath().getLength() > 1) {
        poly2.getPath().removeAt(poly2.getPath().getLength() - 1)
    }

    if (poly2 != null) {
        poly2.setMap(null);
        poly2 = null;
    }

    if (directionsDisplay != null) {
        directionsDisplay.setMap(null);
        directionsDisplay = null;
    }


    polyline = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 0
    });

    poly2 = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 3
    });

    if (marker) {
        marker.setMap(null);
    }

    directionsResultsReturned = 0;
    //carMarkers = [];

    //** Run the Route for every 10 waypoints in the result object
    if (isAnimate == "true") {
        //** Animate route
        legs = [];
        for (var k = 0; k < batches.length; k++) {
            drawRoute_Animate(batches[k], legendImagesArr, isAnimate, batches);
        }


    } else {
        //** Static Route
        for (var k = 0; k < batches.length; k++) {
            drawRoute_Static(batches[k], legendImagesArr, isAnimate, batches);
        }

    }


}

function drawRoute_Animate(my_routeData, legendImagesArr, isAnimate, batches) {
    var localArr = [];
    for (var i = 0; i < my_routeData.length; i++) {
        if (my_routeData[i].Latitude != 0 || my_routeData[i].Longitude != 0)
            localArr.push(my_routeData[i]);
    }

    // Create a renderer for directions and bind it to the map.

    if (isAnimate == "true") {
        //** Animate Route
        var rendererOptions = {
            map: map,
            preserveViewport: true,
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'green',
                strokeWeight: 0  //** Hide the direction route created by direction service
            }
        }

    } else {

        var rendererOptions = {
            map: map,
            preserveViewport: true,
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'black',
                strokeWeight: 3  //** Hide the direction route created by direction service
            }
        }

    }

    //** Create an instance of directions renderer and also make the direction service request parameters ready
    directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
    //var start = new google.maps.LatLng(34.068030, -118.473127);
    //var end = new google.maps.LatLng(34.070593, -118.218250);
    var start = new google.maps.LatLng(localArr[0].Latitude, localArr[0].Longitude);
    var end = new google.maps.LatLng(localArr[localArr.length - 1].Latitude, localArr[localArr.length - 1].Longitude);
    var travelMode = google.maps.DirectionsTravelMode.DRIVING
    // localArr.shift(); //** Remove first element from the array
    // localArr.pop(); //** Remove last element from the array

    localArr.splice(0, 1);
    localArr.splice(localArr.length - 1, 1);

    var waypts = [];
    for (var i = 0; i < localArr.length; i++) {
        waypts.push({
            location: new google.maps.LatLng(localArr[i].Latitude, localArr[i].Longitude),
            stopover: true
        });
    }
    var request = {
        origin: start,
        destination: end,
        waypoints: waypts,
        //optimizeWaypoints: true,
        // optimizeWaypoints: false,
        travelMode: travelMode
    };

    lastVertex = 0;
    //lastVertex = my_routeData.length - 1;
    // Route the directions and pass the response to a
    // function to create markers for each step.

    directionsService.route(request, function (result, status) {

        if (status == google.maps.DirectionsStatus.OK) {

            if (directionsResultsReturned == 0) { // first bunch of results in. new up the combinedResults object
                //   combinedResults = result;
                directionsResultsReturned++;
            }
            else {

                // only building up legs, overview_path, and bounds in my consolidated object. This is not a complete
                // directionResults object, but enough to draw a path on the map, which is all I need
                //  combinedResults.routes[0].legs = combinedResults.routes[0].legs.concat(result.routes[0].legs);
                //combinedResults.routes[0].overview_path = combinedResults.routes[0].overview_path.concat(result.routes[0].overview_path);

                // combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(result.routes[0].bounds.getNorthEast());
                //combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(result.routes[0].bounds.getSouthWest());
                directionsResultsReturned++;
            }
            combinedResults = result;
            directionsDisplay.setOptions({ preserveViewport: true });
            directionsDisplay.setDirections(combinedResults);

            //if (directionsResultsReturned == batches.length) // we've received all the results. put to map
            //{


            //    directionsDisplay.setOptions({ preserveViewport: true });
            //    directionsDisplay.setDirections(combinedResults);
            //    //directionsDisplay.setDirections(response);
            //}

            // var bounds = new google.maps.LatLngBounds();
            var bounds = combinedResults.routes[0].bounds;
            var route = combinedResults.routes[0];

            // For each route, display summary information.
            var path = combinedResults.routes[0].overview_path;
            legs = combinedResults.routes[0].legs;
            for (i = 0; i < legs.length; i++) {
                if (i == 0) {
                    if (isAnimate == "true") {
                        //alert('legs[i].start_location: ' + legs[i].start_location);
                        marker = createMarker(legs[i].start_location, "start", legs[i].start_location, legendImagesArr);
                        if (carMarkers.length > 0) {
                            for (var a = 0; a < carMarkers.length; a++) {
                                carMarkers[a].setMap(null);
                            }
                            carMarkers = [];
                        }
                        carMarkers.push(marker);
                    }

                }
                var steps = legs[i].steps;

                for (j = 0; j < steps.length; j++) {
                    var nextSegment = steps[j].path;
                    for (k = 0; k < nextSegment.length; k++) {
                        polyline.getPath().push(nextSegment[k]);
                        bounds.extend(nextSegment[k]);

                    }
                }
            }
            if (isAnimate == "false") {

                map.fitBounds(bounds);
            } else {
                startAnimation();
            }


        }
    });
}


function drawRoute_Static(my_routeData, legendImagesArr, isAnimate, batches) {
    var localArr = [];
    for (var i = 0; i < my_routeData.length; i++) {
        if (my_routeData[i].Latitude != 0 || my_routeData[i].Longitude != 0)
            localArr.push(my_routeData[i]);
    }

    // Create a renderer for directions and bind it to the map.

    if (isAnimate == "true") {
        //** Animate Route
        var rendererOptions = {
            map: map,
            preserveViewport: true,
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'green',
                strokeWeight: 0  //** Hide the direction route created by direction service
            }
        }

    } else {

        var rendererOptions = {
            map: map,
            preserveViewport: true,
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'black',
                strokeWeight: 3  //** Hide the direction route created by direction service
            }
        }

    }

    //** Create an instance of directions renderer and also make the direction service request parameters ready
    directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
    //var start = new google.maps.LatLng(34.068030, -118.473127);
    //var end = new google.maps.LatLng(34.070593, -118.218250);
    var start = new google.maps.LatLng(localArr[0].Latitude, localArr[0].Longitude);
    var end = new google.maps.LatLng(localArr[localArr.length - 1].Latitude, localArr[localArr.length - 1].Longitude);
    var travelMode = google.maps.DirectionsTravelMode.DRIVING
    // localArr.shift(); //** Remove first element from the array
    // localArr.pop(); //** Remove last element from the array

    localArr.splice(0, 1);
    localArr.splice(localArr.length - 1, 1);

    var waypts = [];
    for (var i = 0; i < localArr.length; i++) {
        waypts.push({
            location: new google.maps.LatLng(localArr[i].Latitude, localArr[i].Longitude),
            stopover: true
        });
    }
    var request = {
        origin: start,
        destination: end,
        waypoints: waypts,
        //optimizeWaypoints: true,
        // optimizeWaypoints: false,
        travelMode: travelMode
    };

    lastVertex = 0;
    //lastVertex = my_routeData.length - 1;
    // Route the directions and pass the response to a
    // function to create markers for each step.

    directionsService.route(request, function (result, status) {

        if (status == google.maps.DirectionsStatus.OK) {



            if (directionsResultsReturned == 0) { // first bunch of results in. new up the combinedResults object
                combinedResults = result;
                directionsResultsReturned++;
            }
            else {

                // only building up legs, overview_path, and bounds in my consolidated object. This is not a complete
                // directionResults object, but enough to draw a path on the map, which is all I need
                combinedResults.routes[0].legs = combinedResults.routes[0].legs.concat(result.routes[0].legs);
                combinedResults.routes[0].overview_path = combinedResults.routes[0].overview_path.concat(result.routes[0].overview_path);

                combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(result.routes[0].bounds.getNorthEast());
                combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(result.routes[0].bounds.getSouthWest());
                directionsResultsReturned++;
            }
            //alert('batches.length' + directionsResultsReturned+"  "+batches.length);
            //if (directionsResultsReturned == batches.length) // we've received all the results. put to map
            //{


            directionsDisplay.setOptions({ preserveViewport: true });
            directionsDisplay.setDirections(combinedResults);
            //directionsDisplay.setDirections(response);
            //  }

            // var bounds = new google.maps.LatLngBounds();
            var bounds = combinedResults.routes[0].bounds;
            var route = combinedResults.routes[0];

            // For each route, display summary information.
            var path = combinedResults.routes[0].overview_path;
            var legs = combinedResults.routes[0].legs;
            for (i = 0; i < legs.length; i++) {
                if (i == 0) {
                    if (isAnimate == "true") {
                        marker = createMarker(legs[i].start_location, "start", legs[i].start_location, legendImagesArr);
                    }

                }
                var steps = legs[i].steps;

                for (j = 0; j < steps.length; j++) {
                    var nextSegment = steps[j].path;
                    for (k = 0; k < nextSegment.length; k++) {
                        polyline.getPath().push(nextSegment[k]);
                        bounds.extend(nextSegment[k]);

                    }
                }
            }
            if (isAnimate == "false") {

                map.fitBounds(bounds);
            } else {
                startAnimation();
            }


        }
    });
}



function calcRoute(routeData, legendImagesArr, isAnimate) {


    isAnimStopped = false;
    playPauseTxt.innerHTML = 'Pause';
    polyDistance = 50;
    var localArr = [];
    for (var i = 0; i < routeData.length; i++) {
        if (my_routeData[i].Latitude != 0 || my_routeData[i].Longitude != 0)
            localArr.push(routeData[i]);
    }

    //** Clear all the previous plotted overlays on the map
    if (poly2_Arr.length > 0) {
        for (var i = 0; i < poly2_Arr.length; i++) {
            poly2_Arr[i].setMap(null);
        }
        poly2_Arr = [];
    }
    if (timerHandle) {
        clearTimeout(timerHandle);
    }
    if (marker) {
        marker.setMap(null);
    }

    if (polyline != null) {
        polyline.setMap(null);
        polyline = null;
    }
    if (poly2.getPath().getLength() > 1) {
        poly2.getPath().removeAt(poly2.getPath().getLength() - 1)
    }

    if (poly2 != null) {
        poly2.setMap(null);
        poly2 = null;
    }

    if (directionsDisplay != null) {
        directionsDisplay.setMap(null);
        directionsDisplay = null;
    }


    polyline = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 0
    });

    poly2 = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 3
    });
    // Create a renderer for directions and bind it to the map.

    if (isAnimate == "true") {
        //** Animate Route
        var rendererOptions = {
            map: map,
            preserveViewport: true,
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'green',
                strokeWeight: 0  //** Hide the direction route created by direction service
            }
        }

    } else {

        var rendererOptions = {
            map: map,
            preserveViewport: true,
            suppressMarkers: true,
            polylineOptions: {
                strokeColor: 'black',
                strokeWeight: 3  //** Hide the direction route created by direction service
            }
        }

    }

    //** Create an instance of directions renderer and also make the direction service request parameters ready
    directionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);

    //var start = new google.maps.LatLng(34.068030, -118.473127);
    //var end = new google.maps.LatLng(34.070593, -118.218250);
    var start = new google.maps.LatLng(localArr[0].Latitude, localArr[0].Longitude);
    var end = new google.maps.LatLng(localArr[localArr.length - 1].Latitude, localArr[localArr.length - 1].Longitude);

    var travelMode = google.maps.DirectionsTravelMode.DRIVING
    localArr.shift(); //** Remove first element from the array
    localArr.pop(); //** Remove last element from the array

    var waypts = [];
    for (var i = 0; i < localArr.length; i++) {
        waypts.push({
            location: new google.maps.LatLng(localArr[i].Latitude, localArr[i].Longitude),
            stopover: true
        });
    }
    var request = {
        origin: start,
        destination: end,
        waypoints: waypts,
        //optimizeWaypoints: true,
        // optimizeWaypoints: false,
        travelMode: travelMode
    };

    lastVertex = 0;
    // Route the directions and pass the response to a
    // function to create markers for each step.
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setOptions({ preserveViewport: true });
            directionsDisplay.setDirections(response);

            var bounds = new google.maps.LatLngBounds();
            var route = response.routes[0];

            // For each route, display summary information.
            var path = response.routes[0].overview_path;
            var legs = response.routes[0].legs;
            for (i = 0; i < legs.length; i++) {
                if (i == 0) {
                    if (isAnimate == "true") {
                        marker = createMarker(legs[i].start_location, "start", legs[i].start_location, legendImagesArr);
                    }

                }
                var steps = legs[i].steps;

                for (j = 0; j < steps.length; j++) {
                    var nextSegment = steps[j].path;
                    for (k = 0; k < nextSegment.length; k++) {
                        polyline.getPath().push(nextSegment[k]);
                        bounds.extend(nextSegment[k]);

                    }
                }
            }
            if (isAnimate == "false") {

                map.fitBounds(bounds);
            } else {
                startAnimation();
            }


        }
    });
}

function alterAnimSpeed(speedVal) {
    if (speedVal == 1) {
        //** Normal speed
        tick = 70 //milliseconds
        step = 25 //meters

    } else if (speedVal == 2) {
        //** High speed
        tick = 14 //milliseconds
        step = 40 //meters

    } else {
        //** Low speed
        tick = 140 //milliseconds
        step = 14 //meters

    }
    //if (speedVal == 1) {
    //    //** Normal speed
    //    tick = 30 //milliseconds
    //    step = 10 //meters

    //} else if (speedVal == 2) {
    //    //** High speed
    //    tick = 8 //milliseconds
    //    step = 29 //meters

    //} else {
    //    //** Low speed
    //    tick = 120 //milliseconds
    //    step = 6 //meters

    //}

}
function startAnimation() {
    if (isAnimStopped == true) {
        return;
    }

    eol = polyline.Distance();
    map.setCenter(polyline.getPath().getAt(0));

    //** Set the route stroke color as blue and increase the stroke weight to 4; 
    poly2 = new google.maps.Polyline({
        path: [polyline.getPath().getAt(0)],
        strokeColor: "black",
        strokeWeight: 3
    });
    poly2.setMap(map);

    poly2_Arr.push(poly2);
    polyDistance = 50;
    setTimeout("animate(50)", 1000);
    // Allow time for the initial map display
}

function createMarker(latlng, label, html, legendImagesArr) {

    //** Creating Marker holding Car Image
    var contentString = '<b>' + 'Officer: D MCCarthy' + '</b><br>' + 'Start Time: 8:30am' + '<br>Activity:Parking<br>' + html;
    var layerIcon = legendImagesArr[28]; //** Patrol Car image

    var image = {
        url: layerIcon,

        // The origin for this image is 0,0.
        origin: new google.maps.Point(0, 0),

        // The anchor for this image is the base of the flagpole at 0,32.
        anchor: new google.maps.Point(16, 9)
    };

    var marker = new google.maps.Marker({
        position: latlng,
        map: map,
        title: label,
        icon: image,
        zIndex: Math.round(latlng.lat() * -100000) << 5
    });
    marker.myname = label;

    google.maps.event.addListener(marker, 'click', function () {
        //  alert('GIS:'+this.position);
        //  infowindow.setContent(contentString);
        // infowindow.open(map, marker);
    });


    return marker;
}

function SaveToDatabase(myData) {
    if (myData.length == 0) {
        return;
    } else {
        // alert('no. of dragged markers:' + markerPositions.length );
        updateAllMarkersOnDemand(markerPositions);
    }
}

//=============== animation functions ======================
function updatePoly(d) {

    // Spawn a new polyline every 20 vertices, because updating a 100-vertex poly is too slow
    if (poly2.getPath().getLength() > 20) {

        poly2 = new google.maps.Polyline({
            path: [polyline.getPath().getAt(lastVertex - 1)],
            strokeColor: 'black',
            strokeWeight: 3
        });
        //  map.addOverlay(poly2)
        poly2.setMap(map);
        poly2_Arr.push(poly2);
    }

    if (polyline.GetIndexAtDistance(d) < lastVertex + 2) {
        if (poly2.getPath().getLength() > 1) {
            poly2.getPath().removeAt(poly2.getPath().getLength() - 1)
        }
        poly2.getPath().insertAt(poly2.getPath().getLength(), polyline.GetPointAtDistance(d));
    } else {
        // poly2.getPath().insertAt(poly2.getPath().getLength(), endLocation.latlng);
        poly2.getPath().insertAt(poly2.getPath().getLength(), polyline.getPath().getAt(lastVertex++));
    }
}

function animate(d) {
    if (isAnimStopped == true) {
        return;
    }
    if (d > eol) {
        map.panTo(endLocation.latlng);
        marker.setPosition(endLocation.latlng);
        polyDistance = 50;
        return;
    }
    var p = polyline.GetPointAtDistance(d);
    map.panTo(p);

    marker.setPosition(p);
    polyDistance = d;
    updatePoly(d);
    timerHandle = setTimeout("animate(" + (d + step) + ")", tick);
}

function plotInvPolygon_Area(isInvLocAggEnabled) {

    //** Plot multiple polygons
    var polygonJSONData =

        {
            "record_1": {
                locName: 'Area 5',
                locId: 1,
                latLngSeq: [
                    //{lat:"-33.820722", lng:"151.194363"},
                    //{lat:"-33.820793", lng:"151.204576"},
                    //{lat:"-33.828851", lng:"151.203332"},
                    //{ lat: "-33.826819", lng: "151.194277" },
                    //{ lat: "-33.820722", lng: "151.194363" }

                    { lat: "32.955412", lng: "-97.065094" },
                    { lat: "32.955398", lng: "-97.063758" },
                    { lat: "32.954705", lng: "-97.063007" },
                    { lat: "32.953832", lng: "-97.062927" },
                    { lat: "32.953769", lng: "-97.063957" },

                    { lat: "32.954057", lng: "-97.064965" },
                    { lat: "32.954773", lng: "-97.065244" },
                    { lat: "32.955412", lng: "-97.065094" }
                ],
                totSSM: 23,
                totMSM: 0,
                totSensor: 0,
                totGateways: 0
            }

        }

    var polygonData;
    InvPolygonRepository_Area = []; //** First clear all the old polygons
    InvPolylineRepository_Area = [];

    var lineSymbol = {
        path: 'M 0,-1 0,1', //** dotted lines
        strokeOpacity: 2,
        strokeColor: 'white',
        scale: 2
    };


    for (var key in polygonJSONData) {
        polygonData = polygonJSONData[key]

        //** create an array of lat and lng with proper google object and then assign to polygon line or polygons
        var latLngSeq = []
        for (var i = 0; i < polygonData.latLngSeq.length; i++) {
            latLngSeq.push(new google.maps.LatLng(polygonData.latLngSeq[i].lat, polygonData.latLngSeq[i].lng))
        }


        //**Create Polyline with Dotted lines
        myPolyline = new google.maps.Polyline({
            infoWinData: polygonData,
            path: latLngSeq,//polygonData.latLngSeq,
            strokeOpacity: 1,
            icons: [{
                icon: lineSymbol,
                offset: '0',
                repeat: '12px'
            }],
        });

        myPolyline.setMap(map);

        InvPolylineRepository_Area.push(myPolyline);

        //** Create Polygon
        myPolygon = new google.maps.Polygon({
            infoWinData: polygonData,
            paths: latLngSeq,//polygonData.latLngSeq,
            strokeColor: "white",
            strokeOpacity: 0.8,
            draggable: false,
            strokeWeight: 0.5,
            visible: isInvLocAggEnabled,
            fillOpacity: 0.0
        });


        myPolygon.setMap(map);

        //** To create Polygon Labels, First find the center of the polygon so that the label can be centered.
        var lat = myPolygon.my_getBounds().getCenter().lat();
        var lng = myPolygon.my_getBounds().getCenter().lng();

        var myPolygonLabel = new MapLabel({
            text: polygonData.locName,
            position: new google.maps.LatLng(lat, lng),
            map: map,
            fontSize: 12
        });
        myPolygonLabel.set('position', new google.maps.LatLng(lat, lng));

        //** The below array holds all the created polygons so as to make the polygons visible/invisible on the map for the layer.
        InvPolygonRepository_Area.push(myPolygon);


        map.fitBounds(myPolygon.my_getBounds());

        // Add an event listener for the polygon click event
        google.maps.event.addListener(myPolygon, 'click', onInvPolyClick_Area);

        // Add an event listener for the polyline click event
        google.maps.event.addListener(myPolyline, 'click', onInvPolyClick_Area);

    }

}

function plotInvPolygon_Zone(isInvLocAggEnabled) {


    //** Plot multiple polygons
    var polygonJSONData =

        {

            "record_1": {
                locName: 'Zone 3',
                locId: 1,
                latLngSeq: [
                    //{ lat: "-33.835267", lng: "151.200542" },
                    //{ lat: "-33.833414", lng: "151.21376" },
                    //{ lat: "-33.842717", lng: "151.213546" },
                    //{ lat: "-33.843074", lng: "151.20419" },
                    //{ lat: "-33.835267", lng: "151.200542" }
                     { lat: "32.955412", lng: "-97.065094" },
                    { lat: "32.955398", lng: "-97.063758" },
                    { lat: "32.954705", lng: "-97.063007" },
                    { lat: "32.953832", lng: "-97.062927" },
                    { lat: "32.953769", lng: "-97.063957" },

                    { lat: "32.954057", lng: "-97.064965" },
                    { lat: "32.954773", lng: "-97.065244" },
                    { lat: "32.955412", lng: "-97.065094" }
                ],
                totSSM: 23,
                totMSM: 0,
                totSensor: 0,
                totGateways: 0
            }//,

            //"record_2": {
            //    locName: 'Zone 2',
            //    locId: 2,
            //    latLngSeq: [
            //        { lat: "-33.829314", lng: "151.220284" },
            //        { lat: "-33.832665", lng: "151.219897" },
            //        { lat: "-33.832808", lng: "151.224875" },
            //        { lat: "-33.829849", lng: "151.225305" },
            //         { lat: "-33.829314", lng: "151.220284" }
            //    ],
            //    totMSM: 0,
            //    totSensor: 0,
            //    totGateways: 0
            //}



        }

    var polygonData;
    InvPolylineRepository_Zone = [];
    InvPolygonRepository_Zone = []; //** First clear all the old polygons

    var lineSymbol = {
        path: 'M 0,-1 0,1', //** dotted lines
        strokeOpacity: 2,
        strokeColor: 'white',
        scale: 2
    };


    for (var key in polygonJSONData) {
        polygonData = polygonJSONData[key]

        //** create an array of lat and lng with proper google object and then assign to polygon line or polygons
        var latLngSeq = []
        for (var i = 0; i < polygonData.latLngSeq.length; i++) {
            latLngSeq.push(new google.maps.LatLng(polygonData.latLngSeq[i].lat, polygonData.latLngSeq[i].lng))
        }

        //**Create Polyline with Dotted lines
        myPolyline = new google.maps.Polyline({
            infoWinData: polygonData,
            path: latLngSeq,//polygonData.latLngSeq,
            strokeOpacity: 1,
            icons: [{
                icon: lineSymbol,
                offset: '0',
                repeat: '12px'
            }],
        });

        myPolyline.setMap(map);
        InvPolylineRepository_Zone.push(myPolyline);

        //** Create Polygon
        myPolygon = new google.maps.Polygon({
            infoWinData: polygonData,
            paths: latLngSeq,//polygonData.latLngSeq,
            strokeOpacity: 0,
            draggable: false,
            strokeWeight: 0.5,
            fillColor: "#4f372d",
            visible: isInvLocAggEnabled,
            fillOpacity: 0.0
        });


        myPolygon.setMap(map);

        //** To create Polygon Labels, First find the center of the polygon so that the label can be centered.
        var lat = myPolygon.my_getBounds().getCenter().lat();
        var lng = myPolygon.my_getBounds().getCenter().lng();

        var myPolygonLabel = new MapLabel({
            text: polygonData.locName,
            position: new google.maps.LatLng(lat, lng),
            map: map,
            fontSize: 12
        });
        myPolygonLabel.set('position', new google.maps.LatLng(lat, lng));

        //** The below array holds all the created polygons so as to make the polygons visible/invisible on the map for the layer.
        InvPolygonRepository_Zone.push(myPolygon);

        map.fitBounds(myPolygon.my_getBounds());

        // Add an event listener for the polygon click event
        google.maps.event.addListener(myPolygon, 'click', onInvPolyClick_Zone);

        // Add an event listener for the polyline click event
        google.maps.event.addListener(myPolyline, 'click', onInvPolyClick_Zone);


    }

}


function onInvPolyClick_Area(event) {

    var contentString = "<div  style=height:100%;width:100%; id=ifw><label id='ifwTitle'>Area Aggregation Summary</label><br />";
    contentString += "Area Name: " + this.infoWinData.locName + "<br />";

    contentString += "Total No. of SSM: " + this.infoWinData.totSSM + "<br />" + "Total No. of MSM: " + this.infoWinData.totMSM + "<br />" + "Total No. of Sensors: " + this.infoWinData.totSensor + "<br />" + "Total No. of Gateways: " + this.infoWinData.totGateways + "<br />"

    // Replace our Info Window's content and position
    infowindow.setContent(contentString + "</div>");
    infowindow.open(map);
    infowindow.setPosition(this.my_getBounds().getCenter());
}

function onInvPolyClick_Zone(event) {

    var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle>Zone Aggregation Summary</label><br />";
    contentString += "Zone Name: " + this.infoWinData.locName + "<br />";

    contentString += "Total No. of SSM: " + this.infoWinData.totSSM + "<br />" + "Total No. of MSM: " + this.infoWinData.totMSM + "<br />" + "Total No. of Sensors: " + this.infoWinData.totSensor + "<br />" + "Total No. of Gateways: " + this.infoWinData.totGateways + "<br />"

    // Replace our Info Window's content and position
    infowindow.setContent(contentString + "</div>");
    infowindow.open(map);
    infowindow.setPosition(this.my_getBounds().getCenter());
}
//************************************************************************************************************


function plotMarkers_CitIssuedByOfficer(layerMarkerArr, legendImagesArr) {

    var hdbLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    //** The below lines added on oct 24th 2014 to clear POI for off route
    clearMap(HdbMarkerRepository, hdbMgr, HdbPolygonRepository_Area, HdbPolylineRepository_Area, HdbPolygonRepository_Zone, HdbPolylineRepository_Zone)

    HdbMarkerRepository = [];  //** Holds the High Demand Parking Bays Layer Markers
    hdbMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(hdbMgr, 'loaded', function () {

        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker

        for (var i = 0; i < layerMarkerArr.length; i++) {

            //** Inventory Layer - Assign the legend icon as per the Layer Type.

            layerIcon = legendImagesArr[29]; //**Occupied Status 
            toolTipText = "Citation ID: ";

            myLatlng = new google.maps.LatLng(layerMarkerArr[i].LocLatitude, layerMarkerArr[i].LocLongitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + layerMarkerArr[i].citationID)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].LocLatitude) == "0" || String(layerMarkerArr[i].LocLongitude) == "0" || String(layerMarkerArr[i].LocLatitude) == "null" || String(layerMarkerArr[i].LocLongitude) == "null") {
                //** Don't add in HdbMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                HdbMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('GIS:' + this.position);
                //** The code below are added on 24th oct 2014 to give info window functionality
                //** Create the HTML content ready to show it in Info Window
                var contentString = "<div style='height:100%;width:295px;' id='ifw'><label id='ifwTitle'>Ticket Summary</label><br />";
                contentString += "Citation ID: " + "<span style='color:blue'>" + this.infoWinData.citationID + '</span>' + "<br />";
                contentString += "Street: " + this.infoWinData.LocationType + "<br />" + "Remark 1: " + this.infoWinData.Remark_1 + "<br />" + "Remark 2: " + this.infoWinData.Remark_2 + "<br />" + "Remark 3: " + this.infoWinData.Remark_3 + "<br />";

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].LocLatitude == 0 || layerMarkerArr[i].LocLongitude == 0 || String(layerMarkerArr[i].LocLatitude) == "null" || String(layerMarkerArr[i].LocLongitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            } else {
                hdbLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }


        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].LocLatitude == 0 || layerMarkerArr[0].LocLongitude == 0 || String(layerMarkerArr[0].LocLatitude) == "null" || String(layerMarkerArr[0].LocLongitude) == "null") && HdbMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.
        if (HdbMarkerRepository.length > 1000) {
            //hdbMgr.addMarkers(HdbMarkerRepository, 13);
            hdbMgr.addMarkers(HdbMarkerRepository, 10);
        } else if (HdbMarkerRepository.length >= 500 && HdbMarkerRepository.length <= 1000) {
            // hdbMgr.addMarkers(HdbMarkerRepository, 12);
            hdbMgr.addMarkers(HdbMarkerRepository, 9);
        } else {
            hdbMgr.addMarkers(HdbMarkerRepository, 5);
        }

        // hdbMgr.refresh();
        // map.fitBounds(hdbLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.


    });




}


function plotMarkers_Revenue(layerMarkerArr, legendImagesArr) {
    var invLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    clearMap(InvMarkerRepository, invMgr, InvPolygonRepository_Area, InvPolylineRepository_Area, InvPolygonRepository_Zone, InvPolylineRepository_Zone)

    InvMarkerRepository = [];  //** Holds the Inventory Layer Markers
    invMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(invMgr, 'loaded', function () {// alert('layerMarkerArr.length:' + layerMarkerArr.length)


        //alert('ss' +layerMarkerArr.length+" :: "+ layerMarkerArr[0].MeterGroup + "  : " + layerMarkerArr[0].DemandZoneId)
        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {

            if (layerMarkerArr[i].revCategory == "High") {
                layerIcon = legendImagesArr[31]; //**Asset with 'High' Revenue (Green)
            } else if (layerMarkerArr[i].revCategory == "Medium") {
                layerIcon = legendImagesArr[32]; //**Asset with 'Med' Revenue (blue)
            }
            else {
                layerIcon = legendImagesArr[33]; //**Asset with 'Low' Revenue (yellow)
            }

            //** Inventory Layer - Assign the legend icon as per the Layer Type.

            //if (layerMarkerArr[i].MeterGroup == 0  && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
            //    layerIcon = legendImagesArr[27]; //** Single Space Meter with 'High' value (RED)
            //    toolTipText = "Single Space Meter";
            //} else if (layerMarkerArr[i].MeterGroup == 0 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
            //    layerIcon = legendImagesArr[28]; //** Single Space Meter with 'Low' value (BLUE)
            //    toolTipText = "Single Space Meter";
            //} else if (layerMarkerArr[i].MeterGroup == 0 && layerMarkerArr[i].DemandZoneId == 5) {
            //    layerIcon = legendImagesArr[29]; //** Single Space Meter with 'Low/Med' value (YELLOW)
            //    toolTipText = "Single Space Meter";
            //} else if (layerMarkerArr[i].MeterGroup == 0 && layerMarkerArr[i].DemandZoneId == 6) {
            //    layerIcon = legendImagesArr[30]; //** Single Space Meter with 'Med/Hig' value (GREEN)
            //    toolTipText = "Single Space Meter";
            //}

            //if (layerMarkerArr[i].MeterGroup == 1 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
            //    layerIcon = legendImagesArr[0]; //** Multi Space Meter with 'High' value (RED)
            //    toolTipText = "Multi Space Meter";
            //} else if (layerMarkerArr[i].MeterGroup == 1 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
            //    layerIcon = legendImagesArr[1]; //** Multi Space Meter with 'Low' value (BLUE)
            //    toolTipText = "Multi Space Meter";
            //} else if (layerMarkerArr[i].MeterGroup == 1 && layerMarkerArr[i].DemandZoneId == 5) {
            //    layerIcon = legendImagesArr[2]; //** Multi Space Meter with 'Low/Med' value (YELLOW)
            //    toolTipText = "Multi Space Meter";
            //} else if (layerMarkerArr[i].MeterGroup == 1 && layerMarkerArr[i].DemandZoneId == 6) {
            //    layerIcon = legendImagesArr[3]; //** Multi Space Meter with 'Med/Hig' value (GREEN)
            //    toolTipText = "Multi Space Meter";
            //}

            toolTipText = "AssetID:";
            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " " + layerMarkerArr[i].longAssetID)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in InvMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                InvMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('GIS:' + this.position);
                //** Create the HTML content ready to show it in Info Window
                var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'><b>Revenue Summary</b></label><br />";
                contentString += "Asset Id: " + this.infoWinData.longAssetID + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />";
                contentString += "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Model: " + this.infoWinData.assetModelDesc + "<br />" + "Demand Zone: " + this.infoWinData.DemandZoneDesc + "<br />" + "Street: " + this.infoWinData.Location + "<br />" + "Zone: " + this.infoWinData.ZoneName + "<br />" + "Area: " + this.infoWinData.ZoneName + "<br />" + "Cash Total: $" + Math.floor(this.infoWinData.cashAmount / 100) + "." + (this.infoWinData.cashAmount % 100) + "<br />" + "Credit Card Total: $" + Math.floor(this.infoWinData.creditCardAmount / 100) + "." + (this.infoWinData.creditCardAmount % 100) + "<br />" + "Smart Card Total: $" + Math.floor(this.infoWinData.smartCardAmount / 100) + "." + (this.infoWinData.smartCardAmount % 100) + "<br />" + "Pay By Phone Total: $" + Math.floor(this.infoWinData.cellAmount / 100) + "." + (this.infoWinData.cellAmount % 100) + "<br />" + "Total Revenue:" + "<span style='color:blue'>" + this.infoWinData.totalRevenue + '</span>' + "<br />"

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            }
            else {
                invLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }
        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && InvMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.

        if (InvMarkerRepository.length > 1000) {
            invMgr.addMarkers(InvMarkerRepository, 10);
        } else if (InvMarkerRepository.length >= 500 && InvMarkerRepository.length <= 1000) {
            invMgr.addMarkers(InvMarkerRepository, 9);
        } else {
            invMgr.addMarkers(InvMarkerRepository, 5);
        }

        invMgr.refresh();
        map.fitBounds(invLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.


    });

}


function plotMarkers_Citation(layerMarkerArr, legendImagesArr) {
    var invLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;
    clearMap(InvMarkerRepository, invMgr, InvPolygonRepository_Area, InvPolylineRepository_Area, InvPolygonRepository_Zone, InvPolylineRepository_Zone)

    InvMarkerRepository = [];  //** Holds the Inventory Layer Markers
    invMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(invMgr, 'loaded', function () {// alert('layerMarkerArr.length:' + layerMarkerArr.length)

        // alert("iss "+layerMarkerArr[0].IssueNo + " " + layerMarkerArr[0].Latitude + "  " + layerMarkerArr[0].Longitude)
        // alert("mid " + layerMarkerArr[0].MeterID + " " + layerMarkerArr[0].AssetID + " " + layerMarkerArr[0].officerName)
        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {

            layerIcon = legendImagesArr[28]; //** Multi Space Meter with 'High' value (RED)
            toolTipText = "Citation ID:";
            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " " + layerMarkerArr[i].IssueNo)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in InvMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                InvMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('GIS:' + this.position);
                //** Create the HTML content ready to show it in Info Window
                var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style='width:300px;'><b>Citation Summary</b></label><br />";
                contentString += "Officer Id: " + this.infoWinData.officerID + "<br />";
                contentString += "Officer Name: " + this.infoWinData.officerName + "<br />" + "<span style='color:blue'>" + "Issue No: " + this.infoWinData.IssueNo + '</span>' + "<br />" + "Issue Date: " + this.infoWinData.dateOnly + "<br />" + "Issue Time: " + this.infoWinData.endTime + "<br />" + "MeterID: " + this.infoWinData.MeterID + "<br />" + "Space ID: " + this.infoWinData.AssetID + "<br />" + "Latitude: " + this.infoWinData.Latitude + "<br />" + "Longitude: " + this.infoWinData.Longitude + "<br />"

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            }
            else {
                invLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }

        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && InvMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.

        if (InvMarkerRepository.length > 1000) {
            invMgr.addMarkers(InvMarkerRepository, 10);
        } else if (InvMarkerRepository.length >= 500 && InvMarkerRepository.length <= 1000) {
            invMgr.addMarkers(InvMarkerRepository, 9);
        } else {
            invMgr.addMarkers(InvMarkerRepository, 5);
        }

        invMgr.refresh();
        map.fitBounds(invLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.


    });

}


function plotMarkers_Officers(layerMarkerArr, legendImagesArr) {
    var invLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    clearMap(InvMarkerRepository, invMgr, InvPolygonRepository_Area, InvPolylineRepository_Area, InvPolygonRepository_Zone, InvPolylineRepository_Zone)

    InvMarkerRepository = [];  //** Holds the Inventory Layer Markers
    invMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(invMgr, 'loaded', function () {// alert('layerMarkerArr.length:' + layerMarkerArr.length)



        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {

            layerIcon = legendImagesArr[27]; //** Multi Space Meter with 'High' value (RED)
            toolTipText = "Officer:";
            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " " + layerMarkerArr[i].officerName)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in InvMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                InvMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('GIS:' + this.position);
                //** Create the HTML content ready to show it in Info Window
                var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style='width:300px;'><b>Officer Summary</b></label><br />";
                contentString += "Officer Id: " + this.infoWinData.officerID + "<br />";
                contentString += "Officer Name: " + this.infoWinData.officerName + "<br />" + "Last Reported DateTime: " + "<span style='color:blue'>" + this.infoWinData.activityDateOff + '</span>' + "<br />" + "Latitude: " + this.infoWinData.Latitude + "<br />" + "Longitude: " + this.infoWinData.Longitude + "<br />"

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            }
            else {
                invLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }
        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && InvMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.

        if (InvMarkerRepository.length > 1000) {
            invMgr.addMarkers(InvMarkerRepository, 10);
        } else if (InvMarkerRepository.length >= 500 && InvMarkerRepository.length <= 1000) {
            invMgr.addMarkers(InvMarkerRepository, 9);
        } else {
            invMgr.addMarkers(InvMarkerRepository, 5);
        }

        invMgr.refresh();
        map.fitBounds(invLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.


    });

}

function plotMarkers_RouteActivity(layerMarkerArr, legendImagesArr) {

    map.setZoom(16);
    var invLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    //** The below line added on October 24th 2014 to clear POI for off route bug
    clearMap(InvMarkerRepository, invMgr, InvPolygonRepository_Area, InvPolylineRepository_Area, InvPolygonRepository_Zone, InvPolylineRepository_Zone)

    InvMarkerRepository = [];  //** Holds the Inventory Layer Markers
    invMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;
    google.maps.event.addListener(invMgr, 'loaded', function () {

        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {

            if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "LOGIN") {
                layerIcon = legendImagesArr[30]; //** DAMAGEDSIGN activity (YELLOW)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "LOGOUT") {
                layerIcon = legendImagesArr[31]; //** LOGOUT activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "PARKING") {
                layerIcon = legendImagesArr[32]; //** Parking activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "PLATEPERMIT") {
                layerIcon = legendImagesArr[33]; //** PLATEPERMIT activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "MARKMODE") {
                layerIcon = legendImagesArr[34]; //** MARKMODE activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "LUNCH") {
                layerIcon = legendImagesArr[35]; //** MARKMODE activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "DIRECT TRAFFIC") {
                layerIcon = legendImagesArr[36]; //** DIRECT TRAFFIC activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "DAMAGEDSIGN") {
                layerIcon = legendImagesArr[37]; //** LOGIN activity (Green)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "COFFEE BREAK") {
                layerIcon = legendImagesArr[39]; //** COFFEE BREAK activity (Blue)
            } else if (String(layerMarkerArr[i].officerActivity).toUpperCase().trim() == "METERSTATUS") {
                layerIcon = legendImagesArr[40]; //** METERSTATUS activity (Blue)
            }
            else {
                layerIcon = legendImagesArr[35]; //** Lunch break activity (Pink)
            }

            toolTipText = "Activity:";
            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            var pinIcon = new google.maps.MarkerImage(
                                        layerIcon,
                                        null, /* size is determined at runtime */
                                        null, /* origin is 0,0 */
                                        null, /* anchor is bottom center of the scaled image */
                                        new google.maps.Size(52, 38)
                                    );

            //** Create marker object
            var marker = new MarkerWithLabel({
                position: myLatlng,
                labelContent: String(layerMarkerArr[i].startTime),
                //labelContent: String(Number(i+1)),
                // labelAnchor: new google.maps.Point(10, 0),
                labelAnchor: new google.maps.Point(20, 0),
                labelClass: "labels", // the CSS class for the label
                labelStyle: { opacity: 0.75 },
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                //icon: pinIcon,
                title: String(toolTipText + " " + layerMarkerArr[i].officerActivity)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in InvMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                InvMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('GIS:' + this.position);
                //** Create the HTML content ready to show it in Info Window
                var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'>Activity Summary</label><br />";
                contentString += "Officer Name: " + this.infoWinData.officerName + "<br />";
                contentString += "Start Time: " + this.infoWinData.startTime + "<br />" + "Activity: " + "<span style='color:blue'>" + this.infoWinData.officerActivity + '</span>' + "<br />";

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            }
            else {
                invLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }
        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && InvMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.

        if (InvMarkerRepository.length > 1000) {
            invMgr.addMarkers(InvMarkerRepository, 10);
        } else if (InvMarkerRepository.length >= 500 && InvMarkerRepository.length <= 1000) {
            invMgr.addMarkers(InvMarkerRepository, 9);
        } else {
            invMgr.addMarkers(InvMarkerRepository, 5);
        }

        invMgr.refresh();
        //  map.fitBounds(invLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.


    });

}


//** The below function was commented to avoid demandzones
function plotMarkers_Inventory(layerMarkerArr, legendImagesArr, isHomePage) {



    var invLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    InvMarkerRepository = [];  //** Holds the Inventory Layer Markers
    invMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(invMgr, 'loaded', function () {
        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {
            // alert('lay' + layerMarkerArr[i].MeterGroup + " " + layerMarkerArr[i].DemandZoneId)
            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0)
                continue;
            //** Inventory Layer - Assign the legend icon as per the Layer Type.

            if (layerMarkerArr[i].MeterGroup == 0 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {

                layerIcon = legendImagesArr[27]; //** Single Space Meter with 'High' value (RED)
                toolTipText = "Single Space Meter";
            } else if (layerMarkerArr[i].MeterGroup == 0 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[28]; //** Single Space Meter with 'Low' value (BLUE)
                toolTipText = "Single Space Meter";
            } else if (layerMarkerArr[i].MeterGroup == 0 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[29]; //** Single Space Meter with 'Low/Med' value (YELLOW)
                toolTipText = "Single Space Meter";
            } else if (layerMarkerArr[i].MeterGroup == 0 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[30]; //** Single Space Meter with 'Med/Hig' value (GREEN)
                toolTipText = "Single Space Meter";
            }

            if (layerMarkerArr[i].MeterGroup == 1 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[0]; //** Multi Space Meter with 'High' value (RED)
                toolTipText = "Multi Space Meter";
            } else if (layerMarkerArr[i].MeterGroup == 1 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[1]; //** Multi Space Meter with 'Low' value (BLUE)
                toolTipText = "Multi Space Meter";
            } else if (layerMarkerArr[i].MeterGroup == 1 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[2]; //** Multi Space Meter with 'Low/Med' value (YELLOW)
                toolTipText = "Multi Space Meter";
            } else if (layerMarkerArr[i].MeterGroup == 1 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[3]; //** Multi Space Meter with 'Med/Hig' value (GREEN)
                toolTipText = "Multi Space Meter";
            }

            else if (layerMarkerArr[i].MeterGroup == 10 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                // alert('lay' + layerMarkerArr[i].MeterGroup + " " + layerMarkerArr[i].DemandZoneId)
                layerIcon = legendImagesArr[4]; //** Sensor with 'High' value (RED)
                toolTipText = "Sensor";
            } else if (layerMarkerArr[i].MeterGroup == 10 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[5]; //** Sensor with 'Low' value (BLUE)
                toolTipText = "Sensor";
            } else if (layerMarkerArr[i].MeterGroup == 10 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[6]; //** Sensor with 'Low/Med' value (YELLOW)
                toolTipText = "Sensor";
            } else if (layerMarkerArr[i].MeterGroup == 10 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[7]; //** Sensor with 'Med/Hig' value (GREEN)
                toolTipText = "Sensor";
            }

            else if (layerMarkerArr[i].MeterGroup == 31 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[8]; //** Mechanism with 'High' value (RED)
                toolTipText = "Mechanism";
            } else if (layerMarkerArr[i].MeterGroup == 31 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[9]; //** Mechanism with 'Low' value (BLUE)
                toolTipText = "Mechanism";
            } else if (layerMarkerArr[i].MeterGroup == 31 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[10]; //** Mechanism with 'Low/Med' value (YELLOW)
                toolTipText = "Mechanism";
            } else if (layerMarkerArr[i].MeterGroup == 31 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[11]; //** Mechanism with 'Med/Hig' value (GREEN)
                toolTipText = "Mechanism";
            }

            else if (layerMarkerArr[i].MeterGroup == 11 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[12]; //** Cashbox with 'High' value (RED)
                toolTipText = "Cashbox";
            } else if (layerMarkerArr[i].MeterGroup == 11 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[13]; //** Cashbox with 'Low' value (BLUE)
                toolTipText = "Cashbox";
            } else if (layerMarkerArr[i].MeterGroup == 11 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[14]; //** Cashbox with 'Low/Med' value (YELLOW)
                toolTipText = "Cashbox";
            } else if (layerMarkerArr[i].MeterGroup == 11 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[15]; //** Cashbox with 'Med/Hig' value (GREEN)
                toolTipText = "Cashbox";
            }

            else if (layerMarkerArr[i].MeterGroup == 20 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[31]; //** Parking Space with 'High' value (RED)
                toolTipText = "Parking Space";
            } else if (layerMarkerArr[i].MeterGroup == 20 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[32]; //** Parking Space with 'Low' value (BLUE)
                toolTipText = "Parking Space";
            } else if (layerMarkerArr[i].MeterGroup == 20 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[33]; //** Parking Space with 'Low/Med' value (YELLOW)
                toolTipText = "Parking Space";
            } else if (layerMarkerArr[i].MeterGroup == 20 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[34]; //** Parking Space with 'Med/Hig' value (GREEN)
                toolTipText = "Parking Space";
            }

            else if (layerMarkerArr[i].MeterGroup == 2 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[36]; //** Sensor Only with 'High' value (RED)
                toolTipText = "Sensor Only Location";
            } else if (layerMarkerArr[i].MeterGroup == 2 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[36]; //** Sensor Only with 'Low' value (BLUE)
                toolTipText = "Sensor Only Location";
            } else if (layerMarkerArr[i].MeterGroup == 2 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[36]; //** Sensor Only with 'Low/Med' value (YELLOW)
                toolTipText = "Sensor Only Location";
            } else if (layerMarkerArr[i].MeterGroup == 2 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[36]; //** Sensor Only with 'Med/Hig' value (GREEN)
                toolTipText = "Sensor Only Location";
            }

            else if (layerMarkerArr[i].MeterGroup == 13 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[35]; //** Gateway Only with 'High' value (RED)
                toolTipText = "Gateway";
            } else if (layerMarkerArr[i].MeterGroup == 13 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[35]; //** Gateway Only with 'Low' value (BLUE)
                toolTipText = "Gateway";
            } else if (layerMarkerArr[i].MeterGroup == 13 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[35]; //** Gateway Only with 'Low/Med' value (YELLOW)
                toolTipText = "Gateway";
            } else if (layerMarkerArr[i].MeterGroup == 13 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[35]; //** Gateway Only with 'Med/Hig' value (GREEN)
                toolTipText = "Gateway";
            }

            else if (layerMarkerArr[i].MeterGroup == 16 && (layerMarkerArr[i].DemandZoneId == 3 || layerMarkerArr[i].DemandZoneId == 1)) {
                layerIcon = legendImagesArr[37]; //** Gateway Only with 'High' value (RED)
                toolTipText = "CSPark";
            } else if (layerMarkerArr[i].MeterGroup == 16 && (layerMarkerArr[i].DemandZoneId == 4 || layerMarkerArr[i].DemandZoneId == 2)) {
                layerIcon = legendImagesArr[37]; //** Gateway Only with 'Low' value (BLUE)
                toolTipText = "CSPark";
            } else if (layerMarkerArr[i].MeterGroup == 16 && layerMarkerArr[i].DemandZoneId == 5) {
                layerIcon = legendImagesArr[37]; //** Gateway Only with 'Low/Med' value (YELLOW)
                toolTipText = "CSPark";
            } else if (layerMarkerArr[i].MeterGroup == 16 && layerMarkerArr[i].DemandZoneId == 6) {
                layerIcon = legendImagesArr[37]; //** Gateway Only with 'Med/Hig' value (GREEN)
                toolTipText = "CSPark";
            }

            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                // draggable: true,
                //draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " Name: " + layerMarkerArr[i].MeterName)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in InvMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                InvMarkerRepository.push(marker);
            }

            markerPositions = [];
            google.maps.event.addListener(marker, 'dragend', function (evt) {
                //** Before adding a marker, please check duplicates
                for (var i = 0; i < markerPositions.length; i++) {
                    if (markerPositions[i].id == this.infoWinData.AssetId) {
                        //** duplicate found, update the latest position again
                        //alert('dup' + markerPositions[i].id + "  " + evt.latLng.lat().toFixed(6));
                        markerPositions[i].id = this.infoWinData.AssetId;
                        markerPositions[i].Lat = evt.latLng.lat().toFixed(6);
                        markerPositions[i].Lng = evt.latLng.lng().toFixed(6);

                        return;
                    }
                }
                //** Add all draggable markers to this array if NO Duplicate
                //alert('add' + this.infoWinData.AssetId);
                markerPositions.push({ id: this.infoWinData.AssetId, Lat: evt.latLng.lat().toFixed(6), Lng: evt.latLng.lng().toFixed(6) });


            });


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                // alert('aGIS:' + this.position);
                //   alert('Lati:' + this.position.lat().toFixed(6) + " " + this.position.lng().toFixed(6));
                //  alert('Lati:' + this.position.lng());
                //updateOnDemand(this.infoWinData.AssetId, this.position.lat(), this.position.lng());
                //** Assign the correct Inventory Type.
                if (this.infoWinData.MeterGroup == 0) {
                    toolTipText = "Single Space Meter";
                    //  updateOnDemand(this.infoWinData.AssetId, this.position.lat(), this.position.lng());
                } else if (this.infoWinData.MeterGroup == 1) {
                    toolTipText = "Multi Space Meter";
                } else if (this.infoWinData.MeterGroup == 10) {
                    toolTipText = "Sensor";
                    updateSensorsOnDemand(this.infoWinData.AssetId, this.position.lat(), this.position.lng());
                } else if (this.infoWinData.MeterGroup == 11) {
                    toolTipText = "Cash Box";
                } else if (this.infoWinData.MeterGroup == 31) {
                    toolTipText = "Mechanism";
                } else if (this.infoWinData.MeterGroup == 13) {
                    toolTipText = "Gateway";
                } else if (this.infoWinData.MeterGroup == 20) {
                    toolTipText = "Parking Space";
                }

                //** Create the HTML content ready to show it in Info Window

                //** Check if demand zone is enabled for customer.
                var isDemZoneVisible = $("#demandZones").css('display');
                if (isDemZoneVisible == "none") {
                    //** Demand zones are not enabled for this customer
                    var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:250px;><b>Inventory Summary</b></label><br />";
                    contentString += "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />";
                    contentString += "Asset Type: " + toolTipText + "<br />" + "Asset Status: " + this.infoWinData.AssetStateDesc + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Latitude: " + this.infoWinData.Latitude + "<br />" + "Longitude: " + this.infoWinData.Longitude + "<br />"// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

                } else {
                    //** Demand zones is ENABLED for this customer
                    var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:250px;><b>Inventory Summary</b></label><br />";
                    contentString += "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />";
                    contentString += "Asset Type: " + toolTipText + "<br />" + "Demand Zone: " + "<span style='color:blue'>" + this.infoWinData.DemandZoneDesc + '</span>' + "<br />" + "Asset Status: " + this.infoWinData.AssetStateDesc + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Latitude: " + this.infoWinData.Latitude + "<br />" + "Longitude: " + this.infoWinData.Longitude + "<br />"// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                }
                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            }
            else {
                invLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }
        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && InvMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.

        if (InvMarkerRepository.length > 1000) {
            invMgr.addMarkers(InvMarkerRepository, 10);
        } else if (InvMarkerRepository.length >= 500 && InvMarkerRepository.length <= 1000) {
            invMgr.addMarkers(InvMarkerRepository, 9);
        } else {
            invMgr.addMarkers(InvMarkerRepository, 5); //** Changes need to be implemented in this line to show markers at the lowest zoom level.
        }

        invMgr.refresh();
        //alert("zoom level"+map.getZoom());
        if (CustomerIDIs == 5001) {
            zoomCustomerToCenterForOrleans(29.957356, -90.080981);
        } else {

            //** Check if the zoom level goes below 10, then keep the POI on the map. dont hide.
            //if (map.getZoom() >= 10) {
            //    map.fitBounds(invLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.
            //}
            map.fitBounds(invLayerBounds);
        }


        // zoomCustomerToCenter(CustomerLat, CustomerLng);

        //** Zoom to the center of the cusotmer city 
        //if (isHomePage == "Home") {
        //    zoomCustomerToCenter(CustomerLat, CustomerLng);
        //}


    });

}


function plotMarkers_HighDemandBays_mobile(layerMarkerArr, legendImagesArr, isHomePage) {

    var hdbLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;


    HdbMarkerRepository = [];  //** Holds the High Demand Parking Bays Layer Markers
    hdbMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;
    google.maps.event.addListener(hdbMgr, 'loaded', function () {
        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {
            //** Inventory Layer - Assign the legend icon as per the Layer Type.
            if (layerMarkerArr[i].OccupancyStatusDesc == "Paid and Occupied" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[46]; //**Paid and Occupied Status (Non communicating Sensor)
                toolTipText = "Paid and Occupied Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Vacant" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[47]; //**Vacant Status (Non communicating Sensor)
                toolTipText = "Vacant Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Violated" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[45]; //**Violated Status (Non communicating Sensor)
                toolTipText = "Violated Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Occupied" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[48]; //**Occupied Status (Non communicating Sensor)
                toolTipText = "Occupied Space";
            }

            else if (layerMarkerArr[i].OccupancyStatusDesc == "Paid and Occupied") {
                layerIcon = legendImagesArr[17]; //**Occupied Status 
                toolTipText = "Paid and Occupied Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Vacant") {
                layerIcon = legendImagesArr[18]; //**Vacant Status 
                toolTipText = "Vacant Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Violated") {
                layerIcon = legendImagesArr[19]; //**Violated Status 
                toolTipText = "Violated Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Paid") {
                layerIcon = legendImagesArr[21]; //**Paid Status 
                toolTipText = "Paid Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Expired") {
                layerIcon = legendImagesArr[20]; //**Expired Status 
                toolTipText = "Expired Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Occupied") {
                layerIcon = legendImagesArr[16]; //**Occupied for sensor only Status 
                toolTipText = "Occupied Space";
            }

            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " Name: " + layerMarkerArr[i].MeterName)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in HdbMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                HdbMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('Lati:' + this.position.lat().toFixed(6) + " " + this.position.lng().toFixed(6));

                //** Create the HTML content ready to show it in Info Window
                //** Check if demand zone is enabled for customer.
                var contentString;
                var isOccupied;

                if (String(this.infoWinData.OccupancyStatusDesc) == "Paid and Occupied") {
                    isOccupied = 1;
                } else if (String(this.infoWinData.OccupancyStatusDesc) == "Violated") {
                    isOccupied = 1;
                } else {
                    isOccupied = 1;
                }

                meterInfo = { "MeterId": this.infoWinData.MeterID, "MeterName": this.infoWinData.MeterName, "MeterStreet": this.infoWinData.Location, "MeterType": this.infoWinData.MeterType, "LastUpdated ": this.infoWinData.PaymentTime, "BayID ": this.infoWinData.BayID, "BayName": this.infoWinData.BayNumber, "ExpiredTime": this.infoWinData.endDateTimeMob, "IsOccupied": isOccupied, "SensorEventTime": this.infoWinData.startDateTimeMob };

                if (String(this.infoWinData.HasSensor) == "true") {
                    //** Spaces Having Sensor
                    if (this.infoWinData.OccupancyStatusDesc == "Vacant") {

                        //** Change 1
                        var amt;
                        if (this.infoWinData.MeterType == "Sensor Only Location") {
                            // IT indicates Sensor only locations where No payment info is there
                            amt = "";
                            this.infoWinData.PaymentTime = "";
                            this.infoWinData.PaymentType = "";
                        } else {
                            amt = "$" + Number(this.infoWinData.amountInCent / 100).toFixed(2);
                            if (String(amt).length == 1) {
                                amt = "$" + Number(this.infoWinData.amountInCent / 100) + ".00";
                            }
                        }

                        //***
                        if (this.infoWinData.OccupancyStatusDesc == "Vacant" && this.infoWinData.NonCommStatusDesc == true) {
                            //** non communicating sensors
                            contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[51] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br/>" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + String(this.infoWinData.PaymentTime) + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        } else {
                            //**  communicating sensors
                            contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[23] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br/>" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + String(this.infoWinData.PaymentTime) + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        }

                        //***



                    } else if (this.infoWinData.OccupancyStatusDesc == "Paid and Occupied") {
                        var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                        if (String(amt).length == 1) {
                            amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                        }

                        //***
                        if (this.infoWinData.OccupancyStatusDesc == "Paid and Occupied" && this.infoWinData.NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[50] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);
                        } else {
                            //**  comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[22] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);
                        }

                        //***



                    } else if (this.infoWinData.OccupancyStatusDesc == "Occupied") {
                        //** For Sensor only locations
                        //var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                        //if (String(amt).length == 1) {
                        //    amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                        //}
                        //contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[22] + "'/></label><br /><br />";
                        //contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "SensorIn/Out Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                        //infowindow.setContent(contentString);
                        //infowindow.open(map, this);


                        //** Change 1
                        var amt;
                        if (this.infoWinData.MeterType == "Sensor Only Location") {
                            // IT indicates Sensor only locations where No payment info is there
                            amt = "";
                            this.infoWinData.PaymentTime = "";
                            this.infoWinData.PaymentType = "";
                        } else {
                            amt = "$" + Number(this.infoWinData.amountInCent / 100).toFixed(2);
                            if (String(amt).length == 1) {
                                amt = "$" + Number(this.infoWinData.amountInCent / 100) + ".00";
                            }
                        }


                        //***
                        if (this.infoWinData.OccupancyStatusDesc == "Occupied" && this.infoWinData.NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[52] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        } else {
                            //**  comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[27] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        }

                        //***



                    }


                    else {
                        //** Violated

                        var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                        if (String(amt).length == 1) {
                            amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                        }


                        if (String(this.infoWinData.ViolationType) == "Unpaid") {
                            //** Unpaid type

                            //***
                            if (this.infoWinData.NonCommStatusDesc == true) {
                                //** Non comm sensors
                                contentString = "<div  style=height:301px;width:323px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[49] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            } else {
                                //**  comm sensors
                                contentString = "<div  style=height:301px;width:323px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[24] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            }

                            //***



                        } else {
                            //** Overstay type

                            if (this.infoWinData.NonCommStatusDesc == true) {
                                //** Non comm sensors
                                contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[49] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            } else {
                                //**  comm sensors
                                contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[24] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);


                            }




                        }


                    }



                    infowindow.open(map, this);

                } else {
                    var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                    if (String(amt).length == 1) {
                        amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                    }

                    if (this.infoWinData.OccupancyStatusDesc == "Expired") {
                        contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[25] + "'/></label><br /><br />";
                        contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "No" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                        infowindow.setContent(contentString);
                        infowindow.open(map, this);


                    } else {
                        contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[26] + "'/></label><br /><br />";
                        contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "No" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                        infowindow.setContent(contentString);
                        infowindow.open(map, this);

                    }

                }
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            } else {
                hdbLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }


        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && HdbMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.
        if (HdbMarkerRepository.length > 1000) {
            hdbMgr.addMarkers(HdbMarkerRepository, 13);
        } else if (HdbMarkerRepository.length >= 500 && HdbMarkerRepository.length <= 1000) {
            hdbMgr.addMarkers(HdbMarkerRepository, 12);
        } else {

            hdbMgr.addMarkers(HdbMarkerRepository, 5);
        }

        hdbMgr.refresh();
        map.fitBounds(hdbLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.

        //** Zoom to the center of the cusotmer city 
        //if (isHomePage == "Home") {
        //    zoomCustomerToCenter(CustomerLat, CustomerLng);
        //}


    });

}

function plotMarkers_HighDemandBays_mobileGIS(layerMarkerArr, legendImagesArr, isHomePage) {

    var hdbLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;


    HdbMarkerRepository = [];  //** Holds the High Demand Parking Bays Layer Markers
    hdbMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;
    google.maps.event.addListener(hdbMgr, 'loaded', function () {
        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {
            //** Inventory Layer - Assign the legend icon as per the Layer Type.

            if (layerMarkerArr[i].OccupancyStatusDesc == "Paid and Occupied" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[51]; //**Paid and Occupied Status (Non communicating Sensor)
                toolTipText = "Paid and Occupied Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Vacant" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[52]; //**Vacant Status (Non communicating Sensor)
                toolTipText = "Vacant Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Violated" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[50]; //**Violated Status (Non communicating Sensor)
                toolTipText = "Violated Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Occupied" && layerMarkerArr[i].NonCommStatusDesc == true) {
                layerIcon = legendImagesArr[53]; //**Occupied Status (Non communicating Sensor)
                toolTipText = "Occupied Space";
            }

            else if (layerMarkerArr[i].OccupancyStatusDesc == "Paid and Occupied") {
                layerIcon = legendImagesArr[38]; //**Occupied Status 
                toolTipText = "Paid and Occupied Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Vacant") {
                layerIcon = legendImagesArr[39]; //**Vacant Status 
                toolTipText = "Vacant Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Violated") {
                layerIcon = legendImagesArr[40]; //**Violated Status 
                toolTipText = "Violated Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Paid") {
                layerIcon = legendImagesArr[41]; //**Paid Status 
                toolTipText = "Paid Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Expired") {
                layerIcon = legendImagesArr[42]; //**Expired Status 
                toolTipText = "Expired Space";
            } else if (layerMarkerArr[i].OccupancyStatusDesc == "Occupied") {
                layerIcon = legendImagesArr[43]; //**Occupied for sensor only Status 
                toolTipText = "Occupied Space";
            }

            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " Name: " + layerMarkerArr[i].MeterName)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in HdbMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                HdbMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('Lati:' + this.position.lat().toFixed(6) + " " + this.position.lng().toFixed(6));

                //** Create the HTML content ready to show it in Info Window
                //** Check if demand zone is enabled for customer.
                var contentString;
                var isOccupied;

                if (String(this.infoWinData.OccupancyStatusDesc) == "Paid and Occupied") {
                    isOccupied = 1;
                } else if (String(this.infoWinData.OccupancyStatusDesc) == "Violated") {
                    isOccupied = 1;
                } else {
                    isOccupied = 1;
                }

                meterInfo = { "MeterId": this.infoWinData.MeterID, "MeterName": this.infoWinData.MeterName, "MeterStreet": this.infoWinData.Location, "MeterType": this.infoWinData.MeterType, "LastUpdated ": this.infoWinData.PaymentTime, "BayID ": this.infoWinData.BayID, "BayName": this.infoWinData.BayNumber, "ExpiredTime": this.infoWinData.endDateTimeMob, "IsOccupied": isOccupied, "SensorEventTime": this.infoWinData.startDateTimeMob };

                if (String(this.infoWinData.HasSensor) == "true") {
                    //** Spaces Having Sensor
                    if (this.infoWinData.OccupancyStatusDesc == "Vacant") {

                        //** Change 1
                        var amt;
                        if (this.infoWinData.MeterType == "Sensor Only Location") {
                            // IT indicates Sensor only locations where No payment info is there
                            amt = "";
                            this.infoWinData.PaymentTime = "";
                            this.infoWinData.PaymentType = "";
                        } else {
                            amt = "$" + Number(this.infoWinData.amountInCent / 100).toFixed(2);
                            if (String(amt).length == 1) {
                                amt = "$" + Number(this.infoWinData.amountInCent / 100) + ".00";
                            }
                        }

                        if (this.infoWinData.OccupancyStatusDesc == "Vacant" && this.infoWinData.NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[56] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br/>" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + String(this.infoWinData.PaymentTime) + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        } else {
                            //**  comm sensors
                            contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[45] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br/>" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + String(this.infoWinData.PaymentTime) + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        }




                    } else if (this.infoWinData.OccupancyStatusDesc == "Paid and Occupied") {
                        var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                        if (String(amt).length == 1) {
                            amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                        }

                        if (this.infoWinData.OccupancyStatusDesc == "Paid and Occupied" && this.infoWinData.NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[55] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        } else {
                            //**  comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[44] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        }


                    } else if (this.infoWinData.OccupancyStatusDesc == "Occupied") {
                        //** For Sensor only locations

                        //** Change 1
                        var amt;
                        if (this.infoWinData.MeterType == "Sensor Only Location") {
                            // IT indicates Sensor only locations where No payment info is there
                            amt = "";
                            this.infoWinData.PaymentTime = "";
                            this.infoWinData.PaymentType = "";
                        } else {
                            amt = "$" + Number(this.infoWinData.amountInCent / 100).toFixed(2);
                            if (String(amt).length == 1) {
                                amt = "$" + Number(this.infoWinData.amountInCent / 100) + ".00";
                            }
                        }

                        if (this.infoWinData.OccupancyStatusDesc == "Occupied" && this.infoWinData.NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[57] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        } else {
                            //**  comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[49] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                            infowindow.setContent(contentString);
                            infowindow.open(map, this);

                        }



                    }


                    else {
                        //** Violated

                        var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                        if (String(amt).length == 1) {
                            amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                        }


                        if (String(this.infoWinData.ViolationType) == "Unpaid") {
                            //** Unpaid type

                            if (this.infoWinData.NonCommStatusDesc == true) {
                                //** Non comm sensors
                                contentString = "<div  style=height:301px;width:323px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[54] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            } else {
                                //**  comm sensors
                                contentString = "<div  style=height:301px;width:323px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[46] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            }



                        } else {
                            //** Overstay type

                            if (this.infoWinData.NonCommStatusDesc == true) {
                                //** Non comm sensors
                                contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[54] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            } else {
                                //**  comm sensors
                                contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[46] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.ViolationType + "</span>" + "<br/><br/>";
                                infowindow.setContent(contentString);

                            }




                        }


                    }



                    infowindow.open(map, this);

                } else {
                    var amt = Number(this.infoWinData.amountInCent / 100).toFixed(2);
                    if (String(amt).length == 1) {
                        amt = Number(this.infoWinData.amountInCent / 100) + ".00";
                    }

                    if (this.infoWinData.OccupancyStatusDesc == "Expired") {
                        contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[48] + "'/></label><br /><br />";
                        contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "No" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                        infowindow.setContent(contentString);
                        infowindow.open(map, this);


                    } else {
                        contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[47] + "'/></label><br /><br />";
                        contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + this.infoWinData.MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "No" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + this.infoWinData.endDateTimeMob + "</span>" + "<br /><br />";
                        infowindow.setContent(contentString);
                        infowindow.open(map, this);

                    }

                }
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            } else {
                hdbLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }


        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && HdbMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.
        if (HdbMarkerRepository.length > 1000) {
            hdbMgr.addMarkers(HdbMarkerRepository, 13);
        } else if (HdbMarkerRepository.length >= 500 && HdbMarkerRepository.length <= 1000) {
            hdbMgr.addMarkers(HdbMarkerRepository, 12);
        } else {

            hdbMgr.addMarkers(HdbMarkerRepository, 5);
        }

        hdbMgr.refresh();
        map.fitBounds(hdbLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.

        //** Zoom to the center of the cusotmer city 
        //if (isHomePage == "Home") {
        //    zoomCustomerToCenter(CustomerLat, CustomerLng);
        //}


    });

}

function SendDataToAndroid() {
    //alert('meterInfo' + JSON.stringify(meterInfo));
    Android.LoadMeterInformation(JSON.stringify(meterInfo));
}

//**********************************************************

//************************************************************************************************************
function plotMarkers_HighDemandBays(layerMarkerArr, legendImagesArr, isHomePage) {
    var hdbLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    HdbMarkerRepository = [];  //** Holds the High Demand Parking Bays Layer Markers
    hdbMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(hdbMgr, 'loaded', function () {

        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker

        for (var i = 0; i < layerMarkerArr.length; i++) {
            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0)
                continue;
            //** Inventory Layer - Assign the legend icon as per the Layer Type.

            if (layerMarkerArr[i].OccupancyStatusID == 1) {
                layerIcon = legendImagesArr[16]; //**Occupied Status 
                toolTipText = "Occupied Bay";
            } else if (layerMarkerArr[i].OccupancyStatusID == 2) {
                layerIcon = legendImagesArr[17]; //**Vacant Status 
                toolTipText = "Vacant Bay";
            } else if (layerMarkerArr[i].OccupancyStatusID == 3) {
                layerIcon = legendImagesArr[18]; //**Compliant Status 
                toolTipText = "Compliant Bay";
            } else if (layerMarkerArr[i].OccupancyStatusID == 4) {
                layerIcon = legendImagesArr[19]; //**Non-Compliant Status 
                toolTipText = "Non-Compliant Bay";
            }

            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " Name: " + layerMarkerArr[i].MeterName)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in HdbMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                HdbMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //  alert('GIS:' + this.position);
                //alert('Lati:' + this.position.lat().toFixed(6) + " " + this.position.lng().toFixed(6));
                //** Create the HTML content ready to show it in Info Window
                //** Check if demand zone is enabled for customer.
                var isDemZoneVisible = $("#demandZones").css('display');
                if (isDemZoneVisible == "none") {
                    //** Demand zones are not enabled for this customer
                    var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:280px;><b>High Demand Bays Summary</b></label><br />";
                    contentString += "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Occupancy Status: " + "<span style='color:blue'>" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />";


                } else {
                    var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:280px;><b>High Demand Bays Summary</b></label><br />";
                    contentString += "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Occupancy Status: " + "<span style='color:blue'>" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />";

                    //contentString += "Occupancy Status: " + "<span style='color:blue'>" + this.infoWinData.OccupancyStatusDesc + '</span>' + "<br />";
                    //contentString += "Asset Id: " + this.infoWinData.AssetID + "<br />";
                    //contentString += "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Demand Zone: " + this.infoWinData.DemandZoneDesc + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />"  + "Area ID: " + this.infoWinData.AreaID + "<br />" + "Location: " + this.infoWinData.Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

                }

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            } else {
                hdbLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }


        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && HdbMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.
        if (HdbMarkerRepository.length > 1000) {
            hdbMgr.addMarkers(HdbMarkerRepository, 10);
        } else if (HdbMarkerRepository.length >= 500 && HdbMarkerRepository.length <= 1000) {
            hdbMgr.addMarkers(HdbMarkerRepository, 9);
        } else {
            hdbMgr.addMarkers(HdbMarkerRepository, 5);
        }

        hdbMgr.refresh();
        if (CustomerIDIs == 5001) {
            zoomCustomerToCenterForOrleans(29.956945, -90.078149);
        } else {
            map.fitBounds(hdbLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.
        }
        // map.fitBounds(hdbLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.
        // zoomCustomerToCenter(CustomerLat, CustomerLng);

        //** Zoom to the center of the cusotmer city 



    });

}

//**********************************************************

//************************************************************************************************************
function plotMarkers_MeterOperations(layerMarkerArr, legendImagesArr) {

    var pmoLayerBounds = new google.maps.LatLngBounds(); //** Creating LatLngBounds object so as to zoom and center the marker on the map.
    var layerIcon;
    var myLatlng;
    var toolTipText;
    var contentString;

    PmoMarkerRepository = [];  //** Holds the High Demand Parking Bays Layer Markers
    pmoMgr = new MarkerManager(map); //** Create an instance of Marker Manager Utility;

    google.maps.event.addListener(pmoMgr, 'loaded', function () {

        //** Loop thru the Service JSON array data for various inventory types to assign lat and lng for each newly created marker
        for (var i = 0; i < layerMarkerArr.length; i++) {
            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0)
                continue;
            //** Inventory Layer - Assign the legend icon as per the Layer Type.

            if (layerMarkerArr[i].OperationalStatusId == 0) {
                layerIcon = legendImagesArr[20]; //**Inactive Status 
                toolTipText = "Inactive Bay";
            } else if (layerMarkerArr[i].OperationalStatusId == 1) {
                layerIcon = legendImagesArr[21]; //**Operational Status 
                toolTipText = "Operational Bay";
            } else if (layerMarkerArr[i].OperationalStatusId == 2) {
                layerIcon = legendImagesArr[22]; //**Non-Operational with Alarm Status 
                toolTipText = "Non-Operational with Alarm Bay";
            } else if (layerMarkerArr[i].OperationalStatusId == 3) {
                layerIcon = legendImagesArr[23]; //**Operational with Alarm Status 
                toolTipText = "Operational with Alarm Bay";
            } else if (layerMarkerArr[i].OperationalStatusId == 4) {
                layerIcon = legendImagesArr[24]; //**Non-Operational/Special Event Status 
                toolTipText = "Non-Operational/Special Event Bay";
            } else if (layerMarkerArr[i].OperationalStatusId == 5) {
                layerIcon = legendImagesArr[25]; //**Non-Operational/Work Zone Status 
                toolTipText = "Non-Operational/Work Zone Bay";
            } else if (layerMarkerArr[i].OperationalStatusId == 6) {
                layerIcon = legendImagesArr[26]; //**Non-Operational/Maintenance Status 
                toolTipText = "Non-Operational/Maintenance Bay";
            }

            myLatlng = new google.maps.LatLng(layerMarkerArr[i].Latitude, layerMarkerArr[i].Longitude)

            //** Create marker object
            var marker = new google.maps.Marker({
                position: myLatlng,
                infoWinData: layerMarkerArr[i], //** Create custom property to each meter related details
                draggable: false,
                icon: new google.maps.MarkerImage(layerIcon),
                title: String(toolTipText + " ID: " + layerMarkerArr[i].AssetId)  //** Tooltip
            });

            //** The below array holds all the created markers so as to make the markers visible/invisible on the map for the layer.
            if (String(layerMarkerArr[i].Latitude) == "0" || String(layerMarkerArr[i].Longitude) == "0" || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in PmoMarkerRepository if the lat and lng of inventory item is ZERO
            } else {
                PmoMarkerRepository.push(marker);
            }


            //** Display Info Window for the marker which is CLICKED
            google.maps.event.addListener(marker, 'click', function () {
                //alert('GIS:' + this.position);
                var checkAlarmStatus = this.infoWinData.OperationalStatusId;
                //** Check if demand zone is enabled for customer.
                var isDemZoneVisible = $("#demandZones").css('display');

                //if (checkAlarmStatus == 2 || checkAlarmStatus == 3) {
                if (checkAlarmStatus != 0 && checkAlarmStatus != 1) {  //** It covers all other statuses also.
                    //** It indicates Operational with Alarm ,r Non-Operational with Alarm status, etc.. are chosen)

                    if (isDemZoneVisible == "none") {
                        //** Demand zones are not enabled for this customer
                        var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle style=width:347px;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Model: " + this.infoWinData.assetModelDesc + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Operational Status: " + "<span style='color:blue'>" + this.infoWinData.OperationalStatusDesc + '</span>' + "<br />";
                        contentString += "Alarm Code: " + this.infoWinData.EventCode + "<br />";
                        contentString += "Alarm Description: " + this.infoWinData.EventDescVerbose + "<br />";

                    } else {
                        var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle style=width:347px;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Model: " + this.infoWinData.assetModelDesc + "<br />" + "Demand Zone: " + this.infoWinData.DemandZoneDesc + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Operational Status: " + "<span style='color:blue'>" + this.infoWinData.OperationalStatusDesc + '</span>' + "<br />";
                        contentString += "Alarm Code: " + this.infoWinData.EventCode + "<br />";
                        contentString += "Alarm Description: " + this.infoWinData.EventDescVerbose + "<br />";
                    }

                } else {
                    //** Create the HTML content ready to show it in Info Window
                    if (isDemZoneVisible == "none") {
                        //** Demand zones are not enabled for this customer
                        //var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle>Asset Operational Status Summary</label><br />";
                        //contentString += "Operational Status: " + "<span style='color:blue'>" + this.infoWinData.OperationalStatusDesc + '</span>' + "<br />";
                        //contentString += "Asset Id: " + this.infoWinData.AssetID + "<br />";
                        //contentString += "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />"  + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Asset Status: " + this.infoWinData.AssetStateDesc + "<br />" + "Area ID: " + this.infoWinData.AreaID + "<br />" + "Location: " + this.infoWinData.Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

                        var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle style=width:347px;;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Model: " + this.infoWinData.assetModelDesc + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Operational Status: " + "<span style='color:blue'>" + this.infoWinData.OperationalStatusDesc + '</span>' + "<br />";

                    } else {
                        //var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle>Asset Operational Status Summary</label><br />";
                        //contentString += "Operational Status: " + "<span style='color:blue'>" + this.infoWinData.OperationalStatusDesc + '</span>' + "<br />";
                        //contentString += "Asset Id: " + this.infoWinData.AssetID + "<br />";
                        //contentString += "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Demand Zone: " + this.infoWinData.DemandZoneDesc + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Asset Status: " + this.infoWinData.AssetStateDesc + "<br />" + "Area ID: " + this.infoWinData.AreaID + "<br />" + "Location: " + this.infoWinData.Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                        var contentString = "<div  style=height:100%;width:100%; id=ifw><label id=ifwTitle style=width:347px;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + this.infoWinData.AssetId + "<br />" + "Asset Name: " + this.infoWinData.MeterName + "<br />" + "Asset Type: " + this.infoWinData.MeterGroupDesc + "<br />" + "Asset Model: " + this.infoWinData.assetModelDesc + "<br />" + "Demand Zone: " + this.infoWinData.DemandZoneDesc + "<br />" + "Area: " + this.infoWinData.AreaName + "<br />" + "Location: " + this.infoWinData.Location + "<br />" + "Operational Status: " + "<span style='color:blue'>" + this.infoWinData.OperationalStatusDesc + '</span>' + "<br />";
                    }

                }

                infowindow.setContent(contentString + "</div>");
                infowindow.open(map, this);
            });

            if (layerMarkerArr[i].Latitude == 0 || layerMarkerArr[i].Longitude == 0 || String(layerMarkerArr[i].Latitude) == "null" || String(layerMarkerArr[i].Longitude) == "null") {
                //** Don't add in bounds if the lat and lng of inventory item is ZERO
            } else {
                pmoLayerBounds.extend(myLatlng); //** Add marker lat/lng to 'bounds' which makes the markers zoomed and centered on the map.
            }

        }

        //** Code for the fix relating to 'If no data is found it goes to the middle of the ocean'
        if ((layerMarkerArr[0].Latitude == 0 || layerMarkerArr[0].Longitude == 0 || String(layerMarkerArr[0].Latitude) == "null" || String(layerMarkerArr[0].Longitude) == "null") && PmoMarkerRepository.length == 0) {
            //** Don't add in bounds if the lat and lng of inventory item is ZERO
            return false;
        }

        //** Add markers in the Marker Manager as per the map viewport.

        if (PmoMarkerRepository.length > 1000) {
            pmoMgr.addMarkers(PmoMarkerRepository, 10);
        } else if (PmoMarkerRepository.length >= 500 && PmoMarkerRepository.length <= 1000) {
            pmoMgr.addMarkers(PmoMarkerRepository, 9);
        } else {
            pmoMgr.addMarkers(PmoMarkerRepository, 5);
        }

        pmoMgr.refresh();
        if (CustomerIDIs == 5001) {
            zoomCustomerToCenterForOrleans(29.947279, -90.077205);
        } else {
            map.fitBounds(pmoLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.
        }
        //map.fitBounds(pmoLayerBounds); //** In Google map V3, we need to use fitBounds method to show the marker centered and zoomed correctly.
        // zoomCustomerToCenter(CustomerLat, CustomerLng);

    });

}

//**********************************************************

function showInfoOnGridRowSelected_Citation(gridRowsArr) {
    var markerRepository;
    markerRepository = InvMarkerRepository


    for (var i = 0; i < markerRepository.length; i++) {

        var gridRow_Latitude = String(gridRowsArr[0].Latitude);//.toFixed(6);
        var gridRow_Longitude = String(gridRowsArr[0].Longitude);//.toFixed(6);

        //** Find the index of dot operator in coord
        var dotLatIndex = Number(gridRow_Latitude.indexOf(".") + 1);
        var dotLngIndex = Number(gridRow_Longitude.indexOf(".") + 1);

        //** Extract after dot operator
        var lenGridRowLat = gridRow_Latitude.substr(dotLatIndex, 20);
        var lenGridRowLng = gridRow_Longitude.substr(dotLngIndex, 20);

        var marker_Longitude = Number(markerRepository[i].position.lng(i)).toFixed(lenGridRowLng.length);
        var marker_Latitude = Number(markerRepository[i].position.lat(i)).toFixed(lenGridRowLat.length);

        if (String(marker_Latitude) == String(gridRow_Latitude) && String(marker_Longitude) == String(gridRow_Longitude)) {

            //** Create the HTML content ready to show it in Info Window
            var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style='width:300px;'><b>Citation Summary</b></label><br />";
            contentString += "Officer Id: " + gridRowsArr[0].officerID + "<br />";
            contentString += "Officer Name: " + gridRowsArr[0].officerName + "<br />" + "<span style='color:blue'>" + "Issue No: " + gridRowsArr[0].IssueNo + '</span>' + "<br />" + "MeterID: " + gridRowsArr[0].MeterID + "<br />" + "Space ID: " + gridRowsArr[0].AssetID + "<br />" + "Latitude: " + gridRowsArr[0].Latitude + "<br />" + "Longitude: " + gridRowsArr[0].Longitude + "<br />"

            infowindow.setContent(contentString);
            infowindow.open(map);
            map.setZoom(21)
            map.setCenter(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude))
            infowindow.setPosition(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude));
            break;
        }
    }


}

function showInfoOnGridRowSelected_Revenue(gridRowsArr) {

    var markerRepository;
    markerRepository = InvMarkerRepository


    for (var i = 0; i < markerRepository.length; i++) {
        var gridRow_Latitude = String(gridRowsArr[0].Latitude);//.toFixed(6);
        var gridRow_Longitude = String(gridRowsArr[0].Longitude);//.toFixed(6);

        //** Find the index of dot operator in coord
        var dotLatIndex = Number(gridRow_Latitude.indexOf(".") + 1);
        var dotLngIndex = Number(gridRow_Longitude.indexOf(".") + 1);

        //** Extract after dot operator
        var lenGridRowLat = gridRow_Latitude.substr(dotLatIndex, 20);
        var lenGridRowLng = gridRow_Longitude.substr(dotLngIndex, 20);

        var marker_Longitude = Number(markerRepository[i].position.lng(i)).toFixed(lenGridRowLng.length);
        var marker_Latitude = Number(markerRepository[i].position.lat(i)).toFixed(lenGridRowLat.length);

        if (String(marker_Latitude) == String(gridRow_Latitude) && String(marker_Longitude) == String(gridRow_Longitude)) {


            //** Create the HTML content ready to show it in Info Window
            var contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'><b>Revenue Summary:</b></label><br />";
            contentString += "Asset Id: " + gridRowsArr[0].longAssetID + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
            contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Street: " + gridRowsArr[0].Location + "<br />" + "Zone: " + gridRowsArr[0].ZoneName + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Cash Total: $" + (Math.floor(gridRowsArr[0].cashAmount / 100)) + "." + (gridRowsArr[0].cashAmount % 100) + "<br />" + "Credit Card Total: $" + Math.floor(gridRowsArr[0].creditCardAmount / 100) + "." + (gridRowsArr[0].creditCardAmount % 100) + "<br />" + "Smart Card Total: $" + Math.floor(gridRowsArr[0].smartCardAmount / 100) + "." + (gridRowsArr[0].smartCardAmount % 100) + "<br />" + "Pay By Phone Total: $" + Math.floor(gridRowsArr[0].cellAmount / 100) + "." + (gridRowsArr[0].cellAmount % 100) + "<br />" + "Total Revenue:" + "<span style='color:blue'>" + gridRowsArr[0].totalRevenue + '</span>' + "<br />"

            infowindow.setContent(contentString);
            infowindow.open(map);
            map.setZoom(21)
            map.setCenter(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude))
            infowindow.setPosition(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude));
            break;
        }
    }


}


function showInfoOnGridRowSelected_OffRoute(gridRowsArr) {

    var markerRepository;
    markerRepository = InvMarkerRepository


    for (var i = 0; i < markerRepository.length; i++) {

        var gridRow_Latitude = String(gridRowsArr[0].Latitude);//.toFixed(6);
        var gridRow_Longitude = String(gridRowsArr[0].Longitude);//.toFixed(6);

        //** Find the index of dot operator in coord
        var dotLatIndex = Number(gridRow_Latitude.indexOf(".") + 1);
        var dotLngIndex = Number(gridRow_Longitude.indexOf(".") + 1);

        //** Extract after dot operator
        var lenGridRowLat = gridRow_Latitude.substr(dotLatIndex, 20);
        var lenGridRowLng = gridRow_Longitude.substr(dotLngIndex, 20);

        var marker_Longitude = Number(markerRepository[i].position.lng(i)).toFixed(lenGridRowLng.length);
        var marker_Latitude = Number(markerRepository[i].position.lat(i)).toFixed(lenGridRowLat.length);

        if (String(marker_Latitude) == String(gridRow_Latitude) && String(marker_Longitude) == String(gridRow_Longitude)) {


            //** Create the HTML content ready to show it in Info Window
            var contentString;
            //** Officer Current  Location info window
            contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'>Activity Summary</label><br />";
            contentString += "Officer Name: " + gridRowsArr[0].officerName + "<br />" + "Start Time: " + "<span style='color:blue'>" + gridRowsArr[0].startTime + '</span>' + "<br />" + "Activity: " + gridRowsArr[0].officerActivity + "<br />"

            infowindow.setContent(contentString);
            infowindow.open(map);
            map.setZoom(21)
            map.setCenter(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude))
            infowindow.setPosition(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude));
            break;
        }
    }


}

function showInfoOnGridRowSelected_AIChicago(gridRowsArr) {

    var markerRepository;
    markerRepository = InvMarkerRepository


    for (var i = 0; i < markerRepository.length; i++) {
        var gridRow_Latitude = String(gridRowsArr[0].Latitude);//.toFixed(6);
        var gridRow_Longitude = String(gridRowsArr[0].Longitude);//.toFixed(6);

        //** Find the index of dot operator in coord
        var dotLatIndex = Number(gridRow_Latitude.indexOf(".") + 1);
        var dotLngIndex = Number(gridRow_Longitude.indexOf(".") + 1);

        //** Extract after dot operator
        var lenGridRowLat = gridRow_Latitude.substr(dotLatIndex, 20);
        var lenGridRowLng = gridRow_Longitude.substr(dotLngIndex, 20);

        var marker_Longitude = Number(markerRepository[i].position.lng(i)).toFixed(lenGridRowLng.length);
        var marker_Latitude = Number(markerRepository[i].position.lat(i)).toFixed(lenGridRowLat.length);

        if (String(marker_Latitude) == String(gridRow_Latitude) && String(marker_Longitude) == String(gridRow_Longitude)) {


            //** Create the HTML content ready to show it in Info Window
            var contentString;
            //** Officer Current  Location info window
            contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style='width:300px;'><b>Officer Summary</b></label><br />";
            contentString += "Officer Id: " + gridRowsArr[0].officerID + "<br />";
            contentString += "Officer Name: " + gridRowsArr[0].officerName + "<br />" + "Last Reported DateTime: " + "<span style='color:blue'>" + gridRowsArr[0].activityDateOff + '</span>' + "<br />" + "Latitude: " + gridRowsArr[0].Latitude + "<br />" + "Longitude: " + gridRowsArr[0].Longitude + "<br />"

            infowindow.setContent(contentString);
            infowindow.open(map);
            map.setZoom(21)
            map.setCenter(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude))
            infowindow.setPosition(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude));
            break;
        }
    }


}

function showInfoOnGridRowSelected(gridRowsArr, layerNameIs) {

    var markerRepository;
    if (layerNameIs == "1") {
        markerRepository = InvMarkerRepository
    } else if (layerNameIs == "2") {
        markerRepository = HdbMarkerRepository
    } else {
        markerRepository = PmoMarkerRepository
    }

    for (var i = 0; i < markerRepository.length; i++) {
        var gridRow_Latitude = String(gridRowsArr[0].Latitude);//.toFixed(6);
        var gridRow_Longitude = String(gridRowsArr[0].Longitude);//.toFixed(6);

        //** Find the index of dot operator in coord
        var dotLatIndex = Number(gridRow_Latitude.indexOf(".") + 1);
        var dotLngIndex = Number(gridRow_Longitude.indexOf(".") + 1);

        //** Extract after dot operator
        var lenGridRowLat = gridRow_Latitude.substr(dotLatIndex, 20);
        var lenGridRowLng = gridRow_Longitude.substr(dotLngIndex, 20);

        var marker_Longitude = Number(markerRepository[i].position.lng(i)).toFixed(lenGridRowLng.length);
        var marker_Latitude = Number(markerRepository[i].position.lat(i)).toFixed(lenGridRowLat.length);

        if (String(marker_Latitude) == String(gridRow_Latitude) && String(marker_Longitude) == String(gridRow_Longitude)) {


            //** Create the HTML content ready to show it in Info Window
            var contentString;
            var isDemZoneVisible = $("#demandZones").css('display');

            if (layerNameIs == "1") {
                //** Inventory Layer info window

                if (isDemZoneVisible == "none") {
                    //** Demand zones are not enabled for this customer
                    contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:250px;><b>Inventory Summary</b></label><br />";
                    contentString += "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
                    contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />" + "Latitude: " + gridRowsArr[0].Latitude + "<br />" + "Longitude: " + gridRowsArr[0].Longitude + "<br />"// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

                } else {
                    contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:250px;><b>Inventory Summary</b></label><br />";
                    contentString += "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
                    contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Demand Zone: " + "<span style='color:blue'>" + gridRowsArr[0].DemandZoneDesc + '</span>' + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + gridRowsArr[0].Location + "<br />" + "Latitude: " + gridRowsArr[0].Latitude + "<br />" + "Longitude: " + gridRowsArr[0].Longitude + "<br />"// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                }
            } else if (layerNameIs == "2") {

                //** High Demand Parking Bays Layer info window

                var contentString;
                var isOccupied;


                if (String(gridRowsArr[0].OccupancyStatusDesc) == "Paid and Occupied") {
                    isOccupied = 1;
                } else if (String(gridRowsArr[0].OccupancyStatusDesc) == "Violated") {
                    isOccupied = 1;
                } else {
                    isOccupied = 1;
                }

                //   meterInfo = { "MeterId": gridRowsArr[0].MeterID, "MeterName": gridRowsArr[0].MeterName, "MeterStreet": gridRowsArr[0].Location, "MeterType": gridRowsArr[0].MeterType, "LastUpdated ": gridRowsArr[0].PaymentTime, "BayID ": gridRowsArr[0].BayID, "BayName": gridRowsArr[0].BayNumber, "ExpiredTime": gridRowsArr[0].endDateTimeMob, "IsOccupied": isOccupied, "SensorEventTime": gridRowsArr[0].startDateTimeMob };

                if (String(gridRowsArr[0].HasSensor) == "true") {
                    //** Spaces Having Sensor
                    if (gridRowsArr[0].OccupancyStatusDesc == "Vacant") {

                        //** Change 1
                        var amt;
                        if (gridRowsArr[0].MeterType == "Sensor Only Location") {
                            // IT indicates Sensor only locations where No payment info is there
                            amt = "";
                            gridRowsArr[0].PaymentTime = "";
                            gridRowsArr[0].PaymentType = "";
                        } else {
                            amt = "$" + Number(gridRowsArr[0].amountInCent / 100).toFixed(2);
                            if (String(amt).length == 1) {
                                amt = "$" + Number(gridRowsArr[0].amountInCent / 100) + ".00";
                            }
                        }

                        if (gridRowsArr[0].OccupancyStatusDesc == "Vacant" && gridRowsArr[0].NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[56] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br/>" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + String(gridRowsArr[0].PaymentTime) + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";

                        } else {
                            //** Comm sensors
                            contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[45] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br/>" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + String(gridRowsArr[0].PaymentTime) + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";

                        }



                    } else if (gridRowsArr[0].OccupancyStatusDesc == "Paid and Occupied") {
                        var amt = Number(gridRowsArr[0].amountInCent / 100).toFixed(2);
                        if (String(amt).length == 1) {
                            amt = Number(gridRowsArr[0].amountInCent / 100) + ".00";
                        }

                        if (gridRowsArr[0].OccupancyStatusDesc == "Paid and Occupied" && gridRowsArr[0].NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[55] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";

                        } else {
                            //** Comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[44] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";

                        }


                    } else if (gridRowsArr[0].OccupancyStatusDesc == "Occupied") {
                        //** For Sensor only locations

                        //** Change 1
                        var amt;
                        if (gridRowsArr[0].MeterType == "Sensor Only Location") {
                            // IT indicates Sensor only locations where No payment info is there
                            amt = "";
                            gridRowsArr[0].PaymentTime = "";
                            gridRowsArr[0].PaymentType = "";
                        } else {
                            amt = "$" + Number(gridRowsArr[0].amountInCent / 100).toFixed(2);
                            if (String(amt).length == 1) {
                                amt = "$" + Number(gridRowsArr[0].amountInCent / 100) + ".00";
                            }
                        }

                        if (gridRowsArr[0].OccupancyStatusDesc == "Occupied" && gridRowsArr[0].NonCommStatusDesc == true) {
                            //** Non comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[57] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";

                        } else {
                            //** Comm sensors
                            contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[49] + "'/></label><br /><br />";
                            contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";

                        }

                    }


                    else {
                        //** Violated

                        var amt = Number(gridRowsArr[0].amountInCent / 100).toFixed(2);
                        if (String(amt).length == 1) {
                            amt = Number(gridRowsArr[0].amountInCent / 100) + ".00";
                        }


                        if (String(gridRowsArr[0].ViolationType) == "Unpaid") {
                            //** Unpaid type

                            if (gridRowsArr[0].NonCommStatusDesc == true) {
                                //** Non comm sensors
                                contentString = "<div  style=height:301px;width:323px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[54] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].ViolationType + "</span>" + "<br/><br/>";

                            } else {
                                //** Comm sensors
                                contentString = "<div  style=height:301px;width:323px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[46] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].ViolationType + "</span>" + "<br/><br/>";

                            }



                        } else {
                            //** Overstay type
                            if (gridRowsArr[0].NonCommStatusDesc == true) {
                                //** Non comm sensors
                                contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[54] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].ViolationType + "</span>" + "<br/><br/>";

                            } else {
                                //** Comm sensors
                                contentString = "<div  style=height:301px;width:340px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[46] + "'/></label><br /><br />";
                                contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "Yes" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Sensor Activity " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].startDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Violation Type" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].ViolationType + "</span>" + "<br/><br/>";

                            }



                        }


                    }



                    //  infowindow.open(map, this);

                } else {

                    var amt = Number(gridRowsArr[0].amountInCent / 100).toFixed(2);
                    if (String(amt).length == 1) {
                        amt = Number(gridRowsArr[0].amountInCent / 100) + ".00";
                    }

                    if (gridRowsArr[0].OccupancyStatusDesc == "Expired") {

                        contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[48] + "'/></label><br /><br />";
                        contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "No" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";
                        //infowindow.setContent(contentString);
                        //infowindow.open(map, this);


                    } else {
                        contentString = "<div  style=height:301px;width:317px;overflow:hidden; id=ifw><br/><label id='ifwTitle' color='blue' style=padding-top:5px;width:330px;><img id='img' src='" + legendImagesArr[47] + "'/></label><br /><br />";
                        contentString += "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter ID" + '</div>' + "<span style='color:blue; '>:&nbsp;&nbsp;" + gridRowsArr[0].MeterID + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Meter Name" + '</div>' + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].MeterName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Has Sensor" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + "No" + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Area" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].AreaName + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Location" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].Location + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Status" + "</div>" + "<span style='color:blue; '>" + "<span style='color:blue'>:&nbsp;&nbsp;" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Type " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentType + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Amount" + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;$" + amt + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Last Payment Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].PaymentTime + "</span>" + "<br />" + "<div style='width:136px;height:10px;float:left;color:#000000'>" + "Expired Time " + "</div>" + "<span style='color:blue;'>:&nbsp;&nbsp;" + gridRowsArr[0].endDateTimeMob + "</span>" + "<br /><br />";
                        //infowindow.setContent(contentString);
                        //infowindow.open(map, this);

                    }

                }

            } else if (layerNameIs == "3") {

                //** Meter Operational Status Layer info window
                var checkAlarmStatus = gridRowsArr[0].OperationalStatusId;
                if (checkAlarmStatus == 2 || checkAlarmStatus == 3) {
                    //** It indicates Operational with Alarm or Non-Operational with Alarm status are chosen)
                    if (isDemZoneVisible == "none") {
                        //** Demand zones are not enabled for this customer
                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
                        contentString += "Alarm Code: " + gridRowsArr[0].EventCode + "<br />";
                        contentString += "Alarm Description: " + gridRowsArr[0].EventDescVerbose + "<br />";

                    } else {
                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
                        contentString += "Alarm Code: " + gridRowsArr[0].EventCode + "<br />";
                        contentString += "Alarm Description: " + gridRowsArr[0].EventDescVerbose + "<br />";

                    }

                } else {
                    //** Create the HTML content ready to show it in Info Window
                    if (isDemZoneVisible == "none") {
                        //** Demand zones are not enabled for this customer
                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";

                    } else {
                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;><b>Asset Operational Status Summary</b></label>";
                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";


                    }
                }

            }

            infowindow.setContent(contentString);
            infowindow.open(map);
            map.setZoom(21)
            map.setCenter(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude))
            infowindow.setPosition(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude));

            break;
        }
    }


}

//function showInfoOnGridRowSelected(gridRowsArr, layerNameIs) {

//    var markerRepository;
//    if (layerNameIs == "1") {
//        markerRepository = InvMarkerRepository
//    } else if (layerNameIs == "2") {
//        markerRepository = HdbMarkerRepository
//    } else {
//        markerRepository = PmoMarkerRepository
//    }

//    for (var i = 0; i < markerRepository.length; i++) {
//        var marker_Longitude = Number(markerRepository[i].position.lng(i)).toFixed(6);
//        var marker_Latitude = Number(markerRepository[i].position.lat(i)).toFixed(6);
//        var gridRow_Latitude = Number(gridRowsArr[0].Latitude);
//        var gridRow_Longitude = Number(gridRowsArr[0].Longitude);

//        if (marker_Latitude == gridRow_Lcatitude && marker_Longitude == gridRow_Longitude) {

//            //** Create the HTML content ready to show it in Info Window
//            var contentString;
//            var isDemZoneVisible = $("#demandZones").css('display');

//            if (layerNameIs == "1") {
//                //** Inventory Layer info window

//                if (isDemZoneVisible == "none") {
//                    //** Demand zones are not enabled for this customer
//                    contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:250px;><b>Inventory Summary</b></label><br />";
//                    contentString += "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                    contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />" + "Latitude: " + gridRowsArr[0].Latitude + "<br />" + "Longitude: " + gridRowsArr[0].Longitude + "<br />"// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

//                } else {
//                    contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:250px;><b>Inventory Summary</b></label><br />";
//                    contentString += "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                    contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Demand Zone: " + "<span style='color:blue'>" + gridRowsArr[0].DemandZoneDesc + '</span>' + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + gridRowsArr[0].Location + "<br />" + "Latitude: " + gridRowsArr[0].Latitude + "<br />" + "Longitude: " + gridRowsArr[0].Longitude + "<br />"// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                }
//            } else if (layerNameIs == "2") {

//                //** High Demand Parking Bays Layer info window
//                if (isDemZoneVisible == "none") {
//                    //** Demand zones are not enabled for this customer
//                    contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:280px;><b>High Demand Bays Summary</b></label><br />";
//                    contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />" + "Occupancy Status: " + "<span style='color:blue'>" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "Non-Compliance Status: " + gridRowsArr[0].NonCompliantStatusDesc + "<br />";


//                    //contentString += "Occupancy Status: " + "<span style='color:blue'>" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />";
//                    //contentString += "Asset Id: " + gridRowsArr[0].AssetID + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                    //contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />"  + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area ID: " + gridRowsArr[0].AreaID + "<br />" + "Location: " + gridRowsArr[0].Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

//                } else {
//                    contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle' style=width:280px;><b>High Demand Bays Summary</b></label><br />";
//                    contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />" + "Occupancy Status: " + "<span style='color:blue'>" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />" + "Non-Compliance Status: " + gridRowsArr[0].NonCompliantStatusDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />";

//                    //contentString += "Occupancy Status: " + "<span style='color:blue'>" + gridRowsArr[0].OccupancyStatusDesc + '</span>' + "<br />";
//                    //contentString += "Asset Id: " + gridRowsArr[0].AssetID + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                    //contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area ID: " + gridRowsArr[0].AreaID + "<br />" + "Location: " + gridRowsArr[0].Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

//                }
//            } else if (layerNameIs == "3") {

//                //** Meter Operational Status Layer info window
//                var checkAlarmStatus = gridRowsArr[0].OperationalStatusId;
//                if (checkAlarmStatus == 2 || checkAlarmStatus == 3) {
//                    //** It indicates Operational with Alarm or Non-Operational with Alarm status are chosen)
//                    if (isDemZoneVisible == "none") {
//                        //** Demand zones are not enabled for this customer
//                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;;><b>Asset Operational Status Summary</b></label>";
//                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
//                        contentString += "Alarm Code: " + gridRowsArr[0].EventCode + "<br />";
//                        contentString += "Alarm Description: " + gridRowsArr[0].EventDescVerbose + "<br />";

//                    } else {
//                        //contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'>Asset Operational Status Summary</label><br />";
//                        //contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
//                        //contentString += "Alarm Code: " + gridRowsArr[0].EventCode + "<br />";
//                        //contentString += "Alarm Description: " + gridRowsArr[0].EventDescVerbose + "<br />";
//                        //contentString += "Asset Id: " + gridRowsArr[0].AssetID + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        //contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area ID: " + gridRowsArr[0].AreaID + "<br />" + "Location: " + gridRowsArr[0].Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;;><b>Asset Operational Status Summary</b></label>";
//                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
//                        contentString += "Alarm Code: " + gridRowsArr[0].EventCode + "<br />";
//                        contentString += "Alarm Description: " + gridRowsArr[0].EventDescVerbose + "<br />";

//                    }

//                } else {
//                    //** Create the HTML content ready to show it in Info Window
//                    if (isDemZoneVisible == "none") {
//                        //** Demand zones are not enabled for this customer
//                        //contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'>Asset Operational Status Summary</label><br />";
//                        //contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
//                        //contentString += "Asset Id: " + gridRowsArr[0].AssetID + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        //contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />"  + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area ID: " + gridRowsArr[0].AreaID + "<br />" + "Location: " + gridRowsArr[0].Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;;><b>Asset Operational Status Summary</b></label>";
//                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";

//                    } else {
//                        contentString = "<div style='height:100%;width:100%;'><label style=width:347px;><b>Asset Operational Status Summary</b></label>";
//                        contentString += "</br>" + "Asset Id: " + gridRowsArr[0].AssetId + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Asset Model: " + gridRowsArr[0].assetModelDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Area: " + gridRowsArr[0].AreaName + "<br />" + "Location: " + gridRowsArr[0].Location + "<br />"// + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"
//                        contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";

//                        //contentString = "<div style='height:100%;width:100%;' id='ifw'><label id='ifwTitle'>Asset Operational Status Summary</label><br />";
//                        //contentString += "Operational Status: " + "<span style='color:blue'>" + gridRowsArr[0].OperationalStatusDesc + '</span>' + "<br />";
//                        //contentString += "Asset Id: " + gridRowsArr[0].AssetID + "<br />" + "Asset Name: " + gridRowsArr[0].MeterName + "<br />";
//                        //contentString += "Asset Type: " + gridRowsArr[0].MeterGroupDesc + "<br />" + "Demand Zone: " + gridRowsArr[0].DemandZoneDesc + "<br />" + "Asset Status: " + gridRowsArr[0].AssetStateDesc + "<br />" + "Area ID: " + gridRowsArr[0].AreaID + "<br />" + "Location: " + gridRowsArr[0].Location// + "<br />" + "Bay Start: " + this.infoWinData.BayStart + "<br />" + "Bay End: " + this.infoWinData.BayEnd + "<br />"

//                    }
//                }

//            }

//            infowindow.setContent(contentString);
//            infowindow.open(map);
//            map.setZoom(21)
//            map.setCenter(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude))
//            infowindow.setPosition(new google.maps.LatLng(gridRow_Latitude, gridRow_Longitude));
//            break;
//        }
//    }


//}

function convertDivToCanvas() {
    // execute the html2canvas script
    var WindowObject;
    WindowObject = window.open('', "PrintMap", "width=1000,height=405,top=200,left=10,resizable=yes");
    WindowObject.document.write("Please wait. Map is getting ready for print.");
    printImage = new Image();



    var script,
    $this = this,
    options = this.options,

    runH2c = function () {
        try {
            var printCanvas = window.html2canvas([document.getElementById('map_canvas')], {
                proxy: "/server.js",
                useCORS: true,
                onrendered: function (printCanvas) {
                    /*
                    canvas is the actual canvas element,
                    to append it to the page call for example
                    */
                    printCanvasURL = printCanvas.toDataURL();
                    printImage.setAttribute('width', '100%');
                    printImage.setAttribute('height', '100%');
                    printImage.src = printCanvasURL;

                }
            });
        } catch (e) {
            $this.h2cDone = true;

        }
    };

    if (window.html2canvas === undefined && script === undefined) {

    } else {
        // html2canvas already loaded, just run it then
        runH2c();
    }

    printImage.onload = function () {
        WindowObject.document.body.innerHTML = "<img id='img' src='" + printCanvasURL + "'/>"
        WindowObject.focus();
        WindowObject.print();
        WindowObject.close();


    }


}


function gmapPrint_Latest() {

    //** Check for which browser the print map is used. If it is for IE, use the old code to print though not center the markers.


    //** Hide legend and its title while printing
    var legend = document.getElementById('legendPanel');
    legend.style.backgroundColor = "white";
    legend.style.background = "FFF";
    legend.style.width = "226px";
    legend.style.height = "275px";
    legend.style.padding = "0px";
    legend.style.margin = "0px";
    legend.style.border = "solid";
    legend.style.borderColor = "darkgray";
    legend.style.borderWidth = "thin";
    legend.style.overflow = "scroll";
    legend.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans - serif";


    var legTitle = document.getElementById('legendTitle');
    legTitle.style.backgroundColor = "#174A7D";
    legTitle.style.width = "223px";
    legTitle.style.height = "27px";
    legTitle.style.margin = "0px";
    legTitle.style.paddingRight = "5px";
    legTitle.style.color = "white";
    legTitle.style.position = "absolute"
    legTitle.style.overflow = "hidden";
    legend.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans - serif";


    convertDivToCanvas()



}

function gmapPrint_IE_Browser() {


    //** Hide legend and its title while printing
    var legend = document.getElementById('legendPanel');
    legend.style.backgroundColor = "white";
    legend.style.background = "FFF";
    legend.style.width = "226px";
    legend.style.height = "275px";
    legend.style.padding = "0px";
    legend.style.margin = "0px";
    legend.style.border = "solid";
    legend.style.borderColor = "darkgray";
    legend.style.borderWidth = "thin";
    legend.style.overflow = "scroll";
    legend.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans - serif";


    var legTitle = document.getElementById('legendTitle');
    legTitle.style.backgroundColor = "#174A7D";
    legTitle.style.width = "223px";
    legTitle.style.height = "27px";
    legTitle.style.margin = "0px";
    legTitle.style.paddingRight = "5px";
    legTitle.style.color = "white";
    legTitle.style.position = "absolute"
    legTitle.style.overflow = "hidden";
    legend.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans - serif";

    var dataUrl = [];
    var i = 0;

    $("#map_canvas canvas").filter(function () {
        dataUrl.push(this.toDataURL("image/png"));
    });


    var DocumentContainer = document.getElementById('mHolder');
    var DocumentContainer_temp = $(DocumentContainer).clone();


    $(DocumentContainer_temp).find('canvas').each(function () {
        $(this).replaceWith('<img title="abc" src="' + dataUrl[i] + '" style="position: absolute; left: 0px; top: 0px; width: 256px; height: 256px;font-family:futura-pt-n4, futura-pt, Arial, sans-serif">');
        i++;
    });


    var WindowObject = window.open('', "PrintMap", "width=1000,height=425,top=200,left=10,resizable=yes");

    WindowObject.document.writeln(DocumentContainer_temp.html());

    WindowObject.document.close();
    WindowObject.focus();
    WindowObject.print();
    WindowObject.close();

}


function clearRoute() {
    //** Clear all the plotted routes on the map
    if (poly2_Arr.length > 0) {
        for (var i = 0; i < poly2_Arr.length; i++) {
            poly2_Arr[i].setMap(null);
        }
        poly2_Arr = [];
    }

    if (timerHandle) {
        clearTimeout(timerHandle); //stop the animation
    }


    if (marker) {
        marker.setMap(null); // clear the car icon
    }

    if (polyline != null) {
        polyline.setMap(null);
        polyline = null;
    }

    if (poly2.getPath().getLength() > 1) {
        poly2.getPath().removeAt(poly2.getPath().getLength() - 1)
    }

    if (poly2 != null) {
        poly2.setMap(null);
        poly2 = null;
    }

    if (directionsDisplay != null) {
        directionsDisplay.setMap(null);
        directionsDisplay = null;
    }


    polyline = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 0
    });

    poly2 = new google.maps.Polyline({
        path: [],
        strokeColor: 'black',
        strokeWeight: 3
    });

    //** Clear all the plotted way point markers
    clearMap(InvMarkerRepository, invMgr, InvPolygonRepository_Area, InvPolylineRepository_Area, InvPolygonRepository_Zone, InvPolylineRepository_Zone)
}


function clearMap(MarkerRepository, markerMgr, PolygonRepository_Area, PolylineRepository_Area, PolygonRepository_Zone, PolylineRepository_Zone) {

    //** Clearing the Inventory markers from the Map
    if (MarkerRepository.length > 0) {
        markerMgr.clearMarkers();
        MarkerRepository = [];
    }


    //** Clearing the Inventory Area polygons from the Map
    if (PolygonRepository_Area.length > 0) {
        for (var i = 0; i < PolygonRepository_Area.length; i++) {
            PolygonRepository_Area[i].setMap(null);
        }
        PolygonRepository_Area = [];
    }

    if (PolylineRepository_Area.length > 0) {
        for (var i = 0; i < PolylineRepository_Area.length; i++) {
            PolylineRepository_Area[i].setMap(null);
        }
        PolylineRepository_Area = [];
    }

    //** Clearing the Inventory Zone polygons from the Map
    if (PolygonRepository_Zone.length > 0) {
        for (var i = 0; i < PolygonRepository_Zone.length; i++) {
            PolygonRepository_Zone[i].setMap(null);
        }
        PolygonRepository_Zone = [];
    }

    if (PolylineRepository_Zone.length > 0) {
        for (var i = 0; i < PolylineRepository_Zone.length; i++) {
            PolylineRepository_Zone[i].setMap(null);
        }
        PolylineRepository_Zone = [];
    }

    //if (carMarkers.length > 0) {
    //    alert('clearmap');
    //    for (var i = 0; i < carMarkers.length; i++) {
    //        carMarkers[i].setMap(null);
    //    }
    //    carMarkers = [];
    //}


    infowindow.close(); //** close any open info windows;

}


function showLegendPanel() {
    //First push legend title with blue bg.
    var legTitle = document.getElementById('legendTitle');
    legTitle.style.display = "inline";
    map.controls[google.maps.ControlPosition.RIGHT_TOP].push(legTitle);

    var legend = document.getElementById('legendPanel');
    legend.style.display = "inline";
    map.controls[google.maps.ControlPosition.RIGHT_TOP].push(legend);
}

google.maps.Polygon.prototype.my_getBounds = function () {
    var bounds = new google.maps.LatLngBounds()
    this.getPath().forEach(function (element, index) { bounds.extend(element) })
    return bounds
}


function FullScreenControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '0px';
    controlDiv.style.paddingTop = '5px';
    controlDiv.style.paddingRight = '0px';


    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.height = '21px';
    controlUI.style.width = '82px';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = 'thin';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Show the Map in Full Screen';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans-serif";
    controlText.style.fontSize = '13px';
    controlText.style.paddingLeft = '3px';
    controlText.style.paddingTop = '3px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingRight = '3px';
    controlText.innerHTML = '  Full Screen  ';
    controlUI.appendChild(controlText);

    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        goFullscreen('map_canvas')
    });
}

function redrawMap() {
    google.maps.event.trigger(map, "resize");
}

function printMapControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '0px';
    controlDiv.style.paddingTop = '5px';


    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.height = '21px';
    controlUI.style.width = '87px';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = 'thin';
    controlUI.style.borderRight = 'none';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Print the Map.';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans-serif";
    controlText.style.fontSize = '13px';
    controlText.style.paddingLeft = '3px';
    controlText.style.paddingTop = '3px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingRight = '3px';
    controlText.innerHTML = '  Print Map  ';
    controlUI.appendChild(controlText);

    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        var userAgent = navigator.userAgent.toLowerCase();
        if (/msie/.test(userAgent) && parseFloat((userAgent.match(/.*(?:rv|ie)[\/: ](.+?)([ \);]|$)/) || [])[1]) >= 8) {
            //** IE browser
            gmapPrint_IE_Browser();
            //gmapPrint_Latest();
        } else {
            //** Other Browsers
            gmapPrint_Latest()
        }
    });
}

function showLegendControl(controlDiv, map) {
    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '0px';
    controlDiv.style.paddingTop = '5px';


    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.height = '21px';
    controlUI.style.width = '87px';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = 'thin';
    controlUI.style.borderRight = 'none';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Show/Hide the Map Legend Items';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans-serif";
    controlText.style.fontSize = '13px';
    controlText.style.paddingLeft = '3px';
    controlText.style.paddingTop = '3px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingRight = '3px';
    controlText.innerHTML = 'Hide Legend';
    controlUI.appendChild(controlText);


    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        var legTitle = document.getElementById('legendTitle');

        var legend = document.getElementById('legendPanel');
        if (controlText.innerHTML == 'Hide Legend') {
            controlText.innerHTML = 'Show Legend';
            legend.style.display = "none";
            legTitle.style.display = "none";
        } else {
            controlText.innerHTML = 'Hide Legend';
            legend.style.display = "inline";
            legTitle.style.display = "inline";
        }
    });
}



function clearMapControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '0px';
    controlDiv.style.paddingTop = '5px';
    controlDiv.style.paddingLeft = '0px';
    controlDiv.style.paddingBottom = '4px';
    controlDiv.style.paddingRight = '0px';

    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.height = '21px';
    controlUI.style.width = '82px';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = 'thin';
    controlUI.style.borderRight = 'none';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Clear the points of interest on the map';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.id = 'clearMapTxt';
    controlText.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans-serif";
    controlText.style.fontSize = '13px';
    controlText.style.paddingLeft = '3px';
    controlText.style.paddingTop = '3px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingRight = '3px';
    controlText.innerHTML = 'Clear Map';
    controlUI.appendChild(controlText);

    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        //** Clears all the POI on the map for all 3 layers
        clearMap(InvMarkerRepository, invMgr, InvPolygonRepository_Area, InvPolylineRepository_Area, InvPolygonRepository_Zone, InvPolylineRepository_Zone);
        clearMap(HdbMarkerRepository, hdbMgr, HdbPolygonRepository_Area, HdbPolylineRepository_Area, HdbPolygonRepository_Zone, HdbPolylineRepository_Zone);
        clearMap(PmoMarkerRepository, pmoMgr, PmoPolygonRepository_Area, PmoPolylineRepository_Area, PmoPolygonRepository_Zone, PmoPolylineRepository_Zone);

        //** CLear the route
        clearRoute();
    });
}

function playPauseControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '0px';
    controlDiv.style.paddingTop = '5px';
    controlDiv.style.paddingLeft = '4px';
    controlDiv.style.paddingBottom = '4px';
    controlDiv.style.paddingRight = '0px';

    // Set CSS for the control border.
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.height = '21px';
    controlUI.style.width = '82px';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = 'thin';
    controlUI.style.borderRight = 'none';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Play / Pause the officer route animation on the map';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('div');
    controlText.id = 'playPauseTxt';
    controlText.style.fontFamily = "futura-pt-n4, futura-pt, Arial, sans-serif";
    controlText.style.fontSize = '13px';
    controlText.style.paddingLeft = '3px';
    controlText.style.paddingTop = '3px';
    controlText.style.paddingBottom = '2px';
    controlText.style.paddingRight = '1px';
    controlText.innerHTML = 'Pause';
    controlUI.appendChild(controlText);

    // Setup the click event listeners: simply set the map to Chicago.
    google.maps.event.addDomListener(controlUI, 'click', function () {
        var animBtn = document.getElementById('playPauseTxt');
        if (controlText.innerHTML == 'Pause') {
            controlText.innerHTML = 'Play';
            onAnimationPlayPause('stop');
        } else {
            controlText.innerHTML = 'Pause';
            onAnimationPlayPause('start');
        }
    });
}

/* 
Native FullScreen JavaScript API
-------------
Assumes Mozilla naming conventions instead of W3C for now
*/

(function () {
    var
		fullScreenApi = {
		    supportsFullScreen: false,
		    isFullScreen: function () { return false; },
		    requestFullScreen: function () { },
		    cancelFullScreen: function () { },
		    fullScreenEventName: '',
		    prefix: ''
		},
		browserPrefixes = 'webkit moz o ms khtml'.split(' ');

    // check for native support
    if (typeof document.cancelFullScreen != 'undefined') {
        fullScreenApi.supportsFullScreen = true;
    } else {
        // check for fullscreen support by vendor prefix
        for (var i = 0, il = browserPrefixes.length; i < il; i++) {
            fullScreenApi.prefix = browserPrefixes[i];

            if (typeof document[fullScreenApi.prefix + 'CancelFullScreen'] != 'undefined') {
                fullScreenApi.supportsFullScreen = true;

                break;
            }
        }
    }

    // update methods to do something useful
    if (fullScreenApi.supportsFullScreen) {
        fullScreenApi.fullScreenEventName = fullScreenApi.prefix + 'fullscreenchange';

        fullScreenApi.isFullScreen = function () {
            switch (this.prefix) {
                case '':
                    return document.fullScreen;
                case 'webkit':
                    return document.webkitIsFullScreen;
                default:
                    return document[this.prefix + 'FullScreen'];
            }
        }
        fullScreenApi.requestFullScreen = function (el) {
            return (this.prefix === '') ? el.requestFullScreen() : el[this.prefix + 'RequestFullScreen']();
        }
        fullScreenApi.cancelFullScreen = function (el) {
            return (this.prefix === '') ? document.cancelFullScreen() : document[this.prefix + 'CancelFullScreen']();
        }
    }

    // jQuery plugin
    if (typeof jQuery != 'undefined') {
        jQuery.fn.requestFullScreen = function () {

            return this.each(function () {
                var el = jQuery(this);
                if (fullScreenApi.supportsFullScreen) {
                    fullScreenApi.requestFullScreen(el);
                }
            });
        };
    }

    // export api
    window.fullScreenApi = fullScreenApi;
})();



