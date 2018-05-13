$(document).ready(function () {
    $("#playerOne").html($("#PlayerName").val());
    var data = JSON.parse($("#SerializedBFArray").val());
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            if (data[i][j] !== "") {
                $(`[px = ${i}][py = ${j}]`).attr("class", "fieldSelected");
            }
        }
    }

    var connection = new WebSocketManager.Connection("wss://" + window.location.host + "/game");
    connection.enableLogging = false;
    connection.connectionMethods.onConnected = () => {
        connection.invoke("BindSocketAndPlayerData", connection.connectionId, $("#PlayerName").val(), $("#SerializedBFArray").val());
    };
    connection.connectionMethods.onDisconnected = () => { };

    connection.connectionMethods.onMessageReceived = () => {
        var data = connection.message;
        if (data.disconnected !== undefined) {
            $("#disconnectedContainer").attr("class", "");
            $("#playGameContent").attr("class", "hide");
        }
        else if (data.won !== undefined) {
            $("#winnerContainer").attr("class", "");
            $("#playGameContent").attr("class", "hide");
            if (data.won)
                $("#youWonText").attr("class", "");
            else
                $("#youLostText").attr("class", "");
        }
        else {
            if (data.x !== undefined && data.y !== undefined) {
                if (data.hit === true) {
                    if (data.previousTurn === "player")
                        $(`[ox = ${data.x}][oy = ${data.y}]`).html("<img src = '/images/ExplosionIcon.png' class = 'backgroundFit' width = '30' height = '30'>");
                    else
                        $(`[px = ${data.x}][py = ${data.y}]`).html("<img src = '/images/ExplosionIcon.png' class = 'backgroundFit' width = '30' height = '30'>");
                }
                else {
                    if (data.previousTurn === "player")
                        $(`[ox = ${data.x}][oy = ${data.y}]`).html("<img src = '/images/SplashIcon.png' class = 'backgroundFit' width = '30' height = '30'>");
                    else
                        $(`[px = ${data.x}][py = ${data.y}]`).html("<img src = '/images/SplashIcon.png' class = 'backgroundFit' width = '30' height = '30'>");
                }
            }
            if (data.turn === "player") {
                $("#playerTurn").attr("class", "");
                $("#opponentTurn").attr("class", "hide");
            }
            else {
                $("#playerTurn").attr("class", "hide");
                $("#opponentTurn").attr("class", "");
            }
            if (data.connected === true) {
                $("#playGameContent").attr("class", "");
                $("#playerTwo").html(data.opponentName);                
                $("#loadingContainer").attr("class", "hide");
            }
            else if (data.startGame === true)
                connection.invoke("StartGame", connection.connectionId);
        }        
    };

    connection.start();

    $('.tableCell').click(function () {
        if (!$.trim($(this).html())) {
            console.log($(this).has());
            connection.invoke("PlayTurn", connection.connectionId, $(this).attr('ox'), $(this).attr('oy'));
        }        
    });
});