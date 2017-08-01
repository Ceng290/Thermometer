//***************************************************************
//*
//***************************************************************
function initialize() {
    var temp = document.getElementById("hfTemp").value;
    var degrees = document.getElementById("hfDegrees").value;

    drawChart(temp, degrees);
}
function drawChart(temp,degrees) {

    var data = google.visualization.arrayToDataTable([
      ['Label', 'Value'],
      ['Temp', 0]
    ]);

    data.setValue(0, 1, temp);
    var options;

    if (degrees === "C") {
         options = {
            greenColor: '#0000FF',
            yellowColor: '#00FF00',
            min: -30,
            width: 400,
            height: 400,
            greenFrom: -30,
            greenTo: 0,
            yellowFrom: 0,
            yellowTo: 75,
            redFrom: 75,
            redTo: 120,
            max: 120,
            minorTicks: 10,
            majorTicks: ['-30', '-20', '-10', '0', '10', '20', '30', '40', '50', '60', '70', '80', '90', '100', '110', '120'],
            textStyle: { fontName: 'TimesNewRoman', fontSize: 6, bold: false }
        };
    } else {
        options = {
            greenColor: '#0000FF',
            yellowColor: '#00FF00',
            min: 0,
            width: 400,
            height: 400,
            greenFrom: 0,
            greenTo: 32,
            yellowFrom: 32,
            yellowTo: 120,
            redFrom: 120,
            redTo: 160,
            max: 160,
            minorTicks: 10,
            majorTicks: ['0', '10', '20', '32', '40', '50', '60', '70', '80', '90', '100', '120', '130', '140', '150', '160'],
            textStyle: { fontName: 'TimesNewRoman', fontSize: 6, bold: false }
        };
    }
    var chart = new google.visualization.Gauge(document.getElementById('chart_div'));
    chart.draw(data, options);
}

//***************************************************************
//*
//***************************************************************
function OpenThresholdPopUp() {
    var temp = "24.67";
    $('#tempThreshold').text(temp);

    $("#dialog-confirm").dialog({
        resizable: false,
        modal: true,
        draggable: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
            }
        }
    });
}