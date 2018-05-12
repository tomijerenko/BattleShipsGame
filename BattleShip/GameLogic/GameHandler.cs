using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;
using System.Text;
using System;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;
using BattleShip.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BattleShip.Data.Entities;

namespace BattleShip.GameLogic
{
    public class GameHandler : WebSocketHandler
    {        
        private readonly DataBaseContext _context;
        private static List<Battle> _battlesList;
     
        public GameHandler(WebSocketConnectionManager webSocketConnectionManager, IConfiguration Configuration) : base(webSocketConnectionManager)
        {
            _context = new DataBaseContext(
                new DbContextOptionsBuilder<DataBaseContext>()
                .UseMySQL(Configuration
                .GetConnectionString("DefaultConnection"))
                .Options);

            //_context.Database.EnsureDeleted();
            //if (_context.Database.EnsureCreated())
            //{
            //    _context.Statistics.Add(new GameStatistics());
            //    _context.SaveChanges();
            //}
            _battlesList = ActiveGameLogic.BattlesList;
        }
                
        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            string socketId = WebSocketConnectionManager.GetId(socket);
            ActiveGameLogic.AddSocketToEmptyBattle(socketId, _battlesList);
        }        

        public override Task OnDisconnected(WebSocket socket)
        {
            Battle playerBattle = _battlesList
                            .FirstOrDefault(g => g.BattleFields
                            .Any(x => x.SocketId == WebSocketConnectionManager.GetId(socket)) == true);

            if (ActiveGameLogic.RemoveDisconnectedBattle(playerBattle, _battlesList))
            {
                ActiveGameLogic.IncrementTotalGamesPlayed(_context);
                ActiveGameLogic.UpdateLongestActiveGame(playerBattle.ActiveGameTime, _context);
            }            

            return base.OnDisconnected(socket);
        }

        public async Task BindSocketAndPlayerData(string socketId, string playerName, string playerArray)
        {
            BattleField battleField = _battlesList.SelectMany(g => g.BattleFields).FirstOrDefault(t => t.SocketId == socketId);
            battleField.PlayerName = playerName;
            battleField.PlayerBattleArray = JsonConvert.DeserializeObject<string[,]>(playerArray);
            battleField.Ready = true;
            string senderMessage = JsonConvert.SerializeObject(new { startGame = true });

            await SendMessageToSingleSocket(socketId, senderMessage);
        }

        public async Task StartGame(string senderSocketId)
        {
            Battle playerBattle = _battlesList.FirstOrDefault(g => g.BattleFields.Any(x => x.SocketId == senderSocketId) == true);
            
            if (playerBattle.IsGameReady())
            {
                string receiverSocketId = playerBattle.BattleFields.FirstOrDefault(battleField => battleField.SocketId != senderSocketId).SocketId;

                string senderMessage = JsonConvert.SerializeObject(new
                {
                    connected = true,
                    turn = playerBattle.IsPlayersTurn(senderSocketId) ? "player" : "opponent",
                    opponentName = playerBattle.BattleFields.FirstOrDefault(x => x.SocketId == receiverSocketId).PlayerName
                });

                string receiverMessage = JsonConvert.SerializeObject(new
                {
                    connected = true,
                    turn = playerBattle.IsPlayersTurn(senderSocketId) ? "opponent" : "player",
                    opponentName = playerBattle.BattleFields.FirstOrDefault(x => x.SocketId == senderSocketId).PlayerName
                });

                await SendMessageToTwoSockets(senderSocketId, receiverSocketId, senderMessage, receiverMessage);
            }
        }        

        public async Task PlayTurn(string senderSocketId, string x, string y)
        {
            Battle playerBattle = _battlesList.FirstOrDefault(g => g.BattleFields.Any(battleField => battleField.SocketId == senderSocketId) == true);
            if (playerBattle.IsPlayersTurn(senderSocketId))
            {
                bool isHit = playerBattle.Shoot(senderSocketId, x, y);                

                string receiverSocketId = playerBattle.BattleFields.FirstOrDefault(battleField => battleField.SocketId != senderSocketId).SocketId;
                string senderMessage;
                string receiverMessage;                

                if (playerBattle.IsGameOver())
                {
                    senderMessage = JsonConvert.SerializeObject(new {
                        won = (playerBattle.GetWinnerName() == senderSocketId) ? false : true
                    });
                    receiverMessage = JsonConvert.SerializeObject(new
                    {
                        won = (playerBattle.GetWinnerName() == senderSocketId) ? true : false
                    });
                }
                else
                {
                    senderMessage = JsonConvert.SerializeObject(new {
                        x,
                        y,
                        hit = isHit,
                        previousTurn = "player",
                        turn = "opponent"
                    });
                    receiverMessage = JsonConvert.SerializeObject(new {
                        x,
                        y,
                        hit = isHit,
                        previousTurn = "opponent",
                        turn = "player"
                    });
                }
                await SendMessageToTwoSockets(senderSocketId,receiverSocketId,senderMessage,receiverMessage);
                ActiveGameLogic.MissileShootStatsUpdate(isHit, _context);
            }            
        }

        public async Task SendMessageToSingleSocket(string socketId, string message)
        {
            await WebSocketConnectionManager.GetSocketById(socketId).SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                    offset: 0,
                                                                    count: message.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageToTwoSockets(string senderSocketId, string receiverSocketId, string senderMessage, string receiverMessage)
        {
            await WebSocketConnectionManager.GetSocketById(senderSocketId).SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(senderMessage),
                                                                    offset: 0,
                                                                    count: senderMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
            await WebSocketConnectionManager.GetSocketById(receiverSocketId).SendAsync(
                buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(receiverMessage),
                                                                offset: 0,
                                                                count: receiverMessage.Length),
                                messageType: WebSocketMessageType.Text,
                                endOfMessage: true,
                                cancellationToken: CancellationToken.None);
        }        
    }
}