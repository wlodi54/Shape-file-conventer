function createMap(geoJsonObject) {
    var styles = createStyle();

    var styleFunction = function (feature) {
        return styles[feature.getGeometry().getType()];
    };
    var vectorSource = new ol.source.Vector({
        features:
            (new ol.format.GeoJSON()).readFeatures(geoJsonObject, { featureProjection: ol.proj.get('EPSG:3857') })
    });
    var vectorLayer = new ol.layer.Vector({
        source: vectorSource,
        style: styleFunction
    });
    var map = createEmptyMap();
    map.addLayer(vectorLayer);
    var view = map.getView();

    view.fit(vectorSource.getExtent(), map.getSize());
    view.setZoom(2.7);
    showDetails(map);
    $('.loader-section').hide();
}

function createEmptyMap() {
    var view = new ol.View({
        center: [0, 0],
        zoom: 2
    });
    var map = new ol.Map({
        target: "map", layers:
        [
            new ol.layer.Tile({
                source: new ol.source.OSM()
                })
        ],
        view: view
    });
    return map;
}

function createStyle() {
    var image = new ol.style.Circle({
        radius: 5,
        fill: null,
        stroke: new ol.style.Stroke({
            color: "red",
            width: 1
        })
    });
    var styles =
        {
            'Point': new ol.style.Style({
                image: image
            }),
            'LineString': new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'green',
                    width: 1
                })
            }),
            'MultiLineString': new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'green',
                    width: 1
                })
            }),
            'MultiPoint': new ol.style.Style({
                image: image
            }),

            'Polygon': new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: "red",
                    lineDash: [4],
                    width: 2
                }),
                fill: new ol.style.Fill({
                    color: "rgba(255,255,0,0.3)"
                })
            }),
            'MultiPolygon': new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: "red",
                    lineDash: [2],
                    width: 2
                }),
                fill: new ol.style.Fill({
                    color: "rgba(255,255,0,0.3)"
                })
            }),
            'GeometryCollection': new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'magenta',
                    width: 2
                }),
                fill: new ol.style.Fill({
                    color: 'magenta'
                }),
                image: new ol.style.Circle({
                    radius: 10,
                    fill: null,
                    stroke: new ol.style.Stroke({
                        color: 'magenta'
                    })
                })
            }),
            'Circle': new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: "black",
                    width: 2
                }),
                fill: new ol.style.Fill({
                    color: 'rgba(255,0,0,0.2)'
                })
            })

        };
    return styles;
}

function showDetails(map) {
    var select = new ol.interaction.Select();
    map.addInteraction(select);
    var selectedFeatures = select.getFeatures();
    var emptyDetail = "----";
    selectedFeatures.on(['add', 'remove'],
        function () {
            var attributeArray = {};
            attributeArray['NAME'] = 0;
            attributeArray['REGION'] = 0;
            attributeArray['SUBREGION'] = 0;
            attributeArray['LON'] = 0;
            attributeArray['LAT'] = 0;
            selectedFeatures.getArray().map(function (country) {
                
                for (var key in attributeArray) {
                    if (attributeArray.hasOwnProperty(key)) {
                        attributeArray[key] = country.get(key);
                    }
                }
                
            });
            if (attributeArray.NAME.length > 0) {
                    $('#nameFeature').text(attributeArray.NAME);

                    $('#longitudeFeature').text(attributeArray.LON);

                    $('#latitudeFeature').text(attributeArray.LAT);

                    $('#regionFeature').text(attributeArray.REGION);

                    $('#subRegionFeature').text(attributeArray.SUBREGION);
                } else {
                    $('#nameFeature').text("Please select a country.");
                    
                    $('#longitudeFeature').text(emptyDetail);

                    $('#latitudeFeature').text(emptyDetail);

                    $('#regionFeature').text(emptyDetail);

                    $('#subRegionFeature').text(emptyDetail);
                }
            });
}

function ajaxRequest() {
    $('#map').empty();
    $('#fileUploadBtn').attr('aria-disabled="true"');
    $('#fileUploadBtn').addClass('disabled');

    $('.loader-section').show();
    var formData = new FormData();
    var file = $('#fileUpload').get(0).files;
    if (file.length > 0) {
       formData.append("UploadedFile", file[0]);
    }
    $.ajax({
        type: "POST",
        url: "/Map/Show",
        contentType: false,
        processData: false,
        data: formData,
        success: function(result) {
            $('#nameFeature').text("Please select the country.");
            if (result == null) {
                throw new Exception("brak danych");
            }
            createMap(result);
        },
        error: function() {
            $('.loader-section').hide();
            alert("Error. Your file isn't correct. Please be sure that your file is .zip");
        },
        complete: function(jqHrs, textStatus) {
            $('#fileUploadBtn').removeClass('disabled');
        }
    });


}
