$(document).ready(function () {
    var clicked = false
    var maxFieldsCounter = 0;

    $("#playerSetupTable").on("mouseleave", function mouseState(e) {
        clicked = false;
    });

    $(".tableCell").on("mousedown mouseover mouseup", function mouseState(e) {

        if (e.type === "mousedown" && $(this).attr("class") === "fieldSelected") {
            clicked = true;
            $(this).attr("class", "tableCell");
            maxFieldsCounter--;
        }
        else if (e.type === "mouseover" && clicked === true && $(this).attr("class") === "fieldSelected") {
            $(this).attr("class", "tableCell");
            maxFieldsCounter--;
        }
        else if (e.type === "mouseup") {
            clicked = false;
        }
        else if (maxFieldsCounter >= 17) {
            return;
        }
        else if (e.type === "mousedown") {
            clicked = true;
            $(this).attr("class", "fieldSelected");
            maxFieldsCounter++;
        }
        else if (e.type === "mouseover" && clicked === true) {
            $(this).attr("class", "fieldSelected");
            maxFieldsCounter++;
        }
    });

    var jsonData = $("#SerializedBFArray").val();
    if (jsonData !== undefined) {
        var data = JSON.parse(jsonData);
        for (var i = 0; i < 10; i++) {
            for (var j = 0; j < 10; j++) {
                if (data[j][i] !== "") {
                    $(`[px = ${j}][py = ${i}]`).attr("class", "fieldSelected");
                    maxFieldsCounter++;
                }
            }
        }
    }
});

function createArray(length) {
    var arr = new Array(length || 0),
        i = length;
    if (arguments.length > 1) {
        var args = Array.prototype.slice.call(arguments, 1);
        while (i--) arr[length - 1 - i] = createArray.apply(this, args);
    }
    return arr;
};

function getArrayData() {
    var shipCoordinates = createArray(10, 10);
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            if ($(`[px = ${i}][py = ${j}]`).attr("class") === "fieldSelected")
                shipCoordinates[i][j] = "x";
            else
                shipCoordinates[i][j] = "";
        }
    }
    return shipCoordinates;
};

function mapArray() {
    $("#SerializedBFArray").val(JSON.stringify(getArrayData()));
};