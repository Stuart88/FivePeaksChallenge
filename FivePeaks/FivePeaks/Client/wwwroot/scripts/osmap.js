

var map;
var zoom = 6;
var apiKey;
var geojson;
var serviceUrl = 'https://api.os.uk/maps/raster/v1/zxy';
var centreLatLong;

// Setup the EPSG:27700 (British National Grid) projection.
var crs = new L.Proj.CRS('EPSG:27700', '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 +units=m +no_defs', {
    resolutions: [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75],
    origin: [-238375.0, 1376256.0]
});

var mapOptions = {
    crs: crs,
    minZoom: 6,
    maxZoom: 9,
    // center: proj4('EPSG:27700', 'EPSG:4326', [380785, 497235]).reverse(),
    zoom: 6,
    attributionControl: false,
    maxBounds: [
        [54.417332, -2.3928738],    //upper left corner of 5 peaks route area
        [54.322531, -2.2161484]     //lower right corner
    ]
};

function getMarkerOptions(feature) {

    var iconSrc = '';
    var iconSize = [30, 30];
    var iconAnchor = [15, 15];

    switch (feature.geometry.itemType) {
        case "Start End":
            iconSrc = 'finish-flag.png';
            iconSize = [55, 55];
            iconAnchor = [15, 45];
            break;
        case "Peak": iconSrc = 'red-triangle.png'; break;
        case "Hill": iconSrc = 'green-triangle.png'; break;
    }

    return {
        icon: new L.icon({
            iconUrl: 'Site/GetImage/icons%2F' + iconSrc,
            iconSize: iconSize,
            iconAnchor: iconAnchor,
            popupAnchor: [-3, -10]
        })
    }
}

function onEachFeature(feature, layer) {
    // does this feature have a property named popupContent?
    if (feature.properties && feature.properties.popupContent) {
        layer.bindPopup(feature.properties.popupContent);
    }
}

function pointToLayer(feature, latLng) {
    if (feature.geometry.type === "Point") {
        return L.marker(latLng, getMarkerOptions(feature));
    }
}



window.setupLeaflet = function (latLong, routeId = 0) {

    if (!routeId)
        routeId = 0;

    fetch('/Site/GetApiKey/OSMap').then(function (response) {
        return response.json();
    }).then(function (data) {

        if(!latLong || latLong.length === 0) {
            centreLatLong = [54.370259, -2.2972584];
            zoom = 6;
        }
        else {
            var lat = latLong.split(",")[0];
            var long = latLong.split(",")[1];
            centreLatLong = [lat, long];
            zoom = 9;
        }

        apiKey = data;

        var map = L.map('map-frame', mapOptions).setView(centreLatLong, zoom);

        // Load and display ZXY tile layer on the map.
        var basemap = L.tileLayer(serviceUrl + '/Leisure_27700/{z}/{x}/{y}.png?key=' + apiKey).addTo(map);

        // show the scale bar on the lower left corner
        L.control.scale().addTo(map);


        fetch(`/Leaderboard/GetRouteDataAsGeoJSON/${routeId}`).then(function (response) {
            return response.json();
        }).then(function(geojsonResp) {


            var geoJsonOK = false;

            console.log(geojsonResp.data);
            console.log(geojsonResp.ok);
            if (geojsonResp.ok) {
                geojson = geojsonResp.data;
                console.log(geojson);
                geoJsonOK = true;

                const routeLineStyle = {
                    "color": "#ff1500",
                    "weight": 7,
                    "opacity": 1
                };

                L.geoJSON(geojson,
                        {
                            style: routeLineStyle,
                        })
                    .addTo(map);
            }

            const mainLineStyle = {
                "color": "#007bff",
                "weight": geoJsonOK ? 6 : 8,
                "opacity": geoJsonOK ? 0.6 : 1
            };

          

            L.geoJSON(geosonFivePeaksRoute,
                    {
                        style: mainLineStyle,
                        onEachFeature: onEachFeature,
                        pointToLayer: pointToLayer
                    })
                .addTo(map);

          
     
        });

     

    }).catch(function (err) {
        // There was an error
        console.warn('Something went wrong.', err);
    });

}



