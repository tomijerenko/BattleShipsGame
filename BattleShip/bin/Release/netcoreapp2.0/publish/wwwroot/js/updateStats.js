$(document).ready(function () {
    setInterval(refreshStatisticsData, 5000)
});

function refreshStatisticsData() {
    $.ajax({
        url: "https://" + window.location.host+"/StatisticsData",
        type: 'GET',
        success: function (data) {
            $("#timeTotal").html(data.totalTimePlayed);
            $("#gamesTotal").html(data.totalGamesPlayed);
            $("#longestActiveGame").html(data.longestActiveGame);
            $("#totalMissileHits").html(data.totalMissileHits);
            $("#totalMissileShoots").html(data.totalMissileShoots);
            $("#totalMissileMisses").html(data.totalMissileMisses);
            $("#currentActiveGames").html(data.currentActiveGames);
        }
    });
}