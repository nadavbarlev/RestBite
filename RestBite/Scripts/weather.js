//weather.js

var weatherCallback = function (data) {
    var wind = data.query.results.channel.wind;
    var item = data.query.results.channel.item;
    $("#temperatureDiv h4").text("Weather: " + item.title);
    var text = "Wind - chill: " + wind.chill + ", speed: " + wind.speed +
        "</br> Temperature: " + item.condition.temp + " °C";
    $("#temperatureDiv p").html(text);

    if (item.condition.temp > 20 && item.condition.temp < 28) {
        $("#temperatureDiv").append("<p><b>Recomendation: </b> It's time to go out to a good restaurant.</p>");
    } else {
        $("#temperatureDiv").append("<p><b>Recomendation: </b> Find an indoor restaurant with style (;.</p>");
    }
};