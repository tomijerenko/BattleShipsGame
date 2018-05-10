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
using BattleShip.Data.Entities;
using Microsoft.Extensions.Configuration;

namespace BattleShip.GameLogic
{
    public class GameHandler : WebSocketHandler
    {
        private static List<Battle> _battlesList;
        private readonly DataBaseContext _context;

        public static int CurrentActiveBattles
        {
            get { return _battlesList.Count; }
        }

        public GameHandler(WebSocketConnectionManager webSocketConnectionManager, IConfiguration Configuration) : base(webSocketConnectionManager)
        {
            _context = new DataBaseContext(
                new DbContextOptionsBuilder<DataBaseContext>()
                .UseSqlServer(Configuration
                .GetConnectionString("DefaultConnection"))
                .Options);

            //_context.Database.EnsureDeleted();
            if (_context.Database.EnsureCreated())
            {
                _context.Statistics.Add(new GameStatistics());
                _context.SaveChanges();
            }

            _battlesList = new List<Battle>();
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            string socketId = WebSocketConnectionManager.GetId(socket);

            if (_battlesList.Count == 0 || _battlesList.FirstOrDefault(x => x.BattleFields.Count == 1) == null)
                _battlesList.Add(new Battle(new BattleField() { SocketId = socketId }));
            else
                _battlesList.FirstOrDefault(x => x.BattleFields.Count == 1).AddSecondBattleField(new BattleField() { SocketId = socketId });
        }

        public override Task OnDisconnected(WebSocket socket)
        {
            Battle playerBattle = _battlesList
                .FirstOrDefault(g => g.BattleFields
                .Any(x => x.SocketId == WebSocketConnectionManager.GetId(socket)) == true);

            if (_battlesList.Remove(playerBattle) && playerBattle.ActiveGameTime != DateTime.MinValue)
            {
                GameStatistics stats = _context.Statistics.ToList().First();
                TimeSpan timePlayed = DateTime.Now - playerBattle.ActiveGameTime;
                stats.TotalTimePlayed += timePlayed;
                if (stats.LongestActiveGame < timePlayed)
                    stats.LongestActiveGame = timePlayed;
                _context.SaveChanges();
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
                _context.Statistics.First().TotalGamesPlayed++;
                _context.SaveChanges();

                string receiverSocketId = playerBattle.BattleFields.FirstOrDefault(battleField => battleField.SocketId != senderSocketId).SocketId;

                string senderMessage = JsonConvert.SerializeObject(new {
                    connected = true,
                    turn = playerBattle.IsPlayersTurn(senderSocketId) ? "player" : "opponent",
                    opponentName = playerBattle.BattleFields.FirstOrDefault(x => x.SocketId == receiverSocketId).PlayerName });

                string receiverMessage = JsonConvert.SerializeObject(new {
                    connected = true,
                    turn = playerBattle.IsPlayersTurn(senderSocketId) ? "opponent" : "player",
                    opponentName = playerBattle.BattleFields.FirstOrDefault(x => x.SocketId == senderSocketId).PlayerName });

                await SendMessageToTwoSockets(senderSocketId, receiverSocketId, senderMessage, receiverMessage);
            }
        }

        public async Task PlayTurn(string senderSocketId, string x, string y)
        {
            Battle playerBattle = _battlesList.FirstOrDefault(g => g.BattleFields.Any(battleField => battleField.SocketId == senderSocketId) == true);
            if (playerBattle.IsPlayersTurn(senderSocketId))
            {
                bool isHit = playerBattle.Shoot(senderSocketId, x, y);
                MissileShootStatsUpdate(isHit);

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

        public void MissileShootStatsUpdate(bool isHit)
        {
            _context.Statistics.First().TotalMissileShoots++;
            if (isHit)
                _context.Statistics.First().TotalMissileHits++;
            else
                _context.Statistics.First().TotalMissileMisses++;
            _context.SaveChanges();
        }
    }
}