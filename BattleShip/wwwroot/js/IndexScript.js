$(document).ready(function () {
    setInterval(refreshStatisticsData, 5000)
});

function refreshStatisticsData() {
    $.ajax({
        url: "http://localhost:5000/StatisticsData",
        type: 'GET',
        success: function (data) {
            $("#timeTotal").html(data.totalTimePlayed);
            $("#gamesTotal").html(data.totalGamesPlayed);
            $("#longestActiveGame").html(data.longestActiveGame);
            $("#totalMissileHits").html(data.totalMissileHits);
            $("#currentActiveGames").html(data.currentActiveGames);
        }
    });
}