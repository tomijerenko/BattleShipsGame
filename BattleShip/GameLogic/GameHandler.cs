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

            _battlesList = new List<Battle>();
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            string socketId = WebSocketConnectionManager.GetId(socket);

            if (_battlesList.Count == 0 || _battlesList.FirstOrDefault(x => x.BattleFields.Count == 1) == null)
            {
                _battlesList.Add(new Battle(new BattleField() { SocketId = socketId }));
            }
            else
            {
                _battlesList.FirstOrDefault(x => x.BattleFields.Count == 1).AddSecondBattleField(new BattleField() { SocketId = socketId });
            }
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
            battleField.Player = new Player() { PlayerName = playerName };
            battleField.PlayerBattleArray = JsonConvert.DeserializeObject<string[,]>(playerArray);
            battleField.Ready = true;

            string senderMessage = JsonConvert.SerializeObject(new { startGame = true });
            await WebSocketConnectionManager.GetSocketById(socketId).SendAsync(
            buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(senderMessage),
                                                            offset: 0,
                                                            count: senderMessage.Length),
                            messageType: WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: CancellationToken.None);
        }

        public async Task StartGame(string socketId)
        {
            Battle playerBattle = _battlesList.FirstOrDefault(g => g.BattleFields.Any(x => x.SocketId == socketId) == true);
            
            if (playerBattle.IsGameReady())
            {
                _context.Statistics.First().TotalGamesPlayed++;
                _context.SaveChanges();

                string playerMessage = JsonConvert.SerializeObject(new {
                    connected = true,
                    turn = playerBattle.IsPlayersTurn(socketId) ? "opponent" : "player",
                    opponentName = playerBattle.BattleFields.FirstOrDefault(x => x.SocketId == socketId).Player.PlayerName });

                string opponentMessage = JsonConvert.SerializeObject(new {
                    connected = true,
                    turn = playerBattle.IsPlayersTurn(socketId) ? "player" : "opponent",
                    opponentName = playerBattle.BattleFields.FirstOrDefault(x => x.SocketId != socketId).Player.PlayerName });

                await WebSocketConnectionManager.GetSocketById(playerBattle.BattleFields.FirstOrDefault().SocketId).SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(playerMessage),
                                                                    offset: 0,
                                                                    count: playerMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
                await WebSocketConnectionManager.GetSocketById(playerBattle.BattleFields.Last().SocketId).SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(opponentMessage),
                                                                    offset: 0,
                                                                    count: opponentMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
            }
        }

        public async Task PlayTurn(string socketId, string x, string y)
        {
            Battle playerBattle = _battlesList.FirstOrDefault(g => g.BattleFields.Any(battleField => battleField.SocketId == socketId) == true);
            if (playerBattle.IsPlayersTurn(socketId))
            {
                bool isHit = playerBattle.Shoot(socketId, x, y);

                if (isHit)
                {
                    _context.Statistics.First().TotalMissileHits++;
                    _context.SaveChanges();
                }

                if (playerBattle.IsGameOver())
                {
                    string sendermsg = JsonConvert.SerializeObject(new
                    {
                        won = (playerBattle.GetWinnerName() == socketId) ? false : true
                    });
                    string rcvrmsg = JsonConvert.SerializeObject(new
                    {
                        won = (playerBattle.GetWinnerName() == socketId) ? true : false
                    });
                    string opntScktId = playerBattle.BattleFields.FirstOrDefault(battleField => battleField.SocketId != socketId).SocketId;
                    await WebSocketConnectionManager.GetSocketById(socketId).SendAsync(
                        buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(sendermsg),
                                                                        offset: 0,
                                                                        count: sendermsg.Length),
                                        messageType: WebSocketMessageType.Text,
                                        endOfMessage: true,
                                        cancellationToken: CancellationToken.None);
                    await WebSocketConnectionManager.GetSocketById(opntScktId).SendAsync(
                        buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(rcvrmsg),
                                                                        offset: 0,
                                                                        count: rcvrmsg.Length),
                                        messageType: WebSocketMessageType.Text,
                                        endOfMessage: true,
                                        cancellationToken: CancellationToken.None);
                }

                string senderMessage = JsonConvert.SerializeObject(new {
                    x,
                    y,
                    hit = isHit,
                    previousTurn = "player",
                    turn = "opponent"
                });
                string receiverMessage = JsonConvert.SerializeObject(new {
                    x,
                    y,
                    hit = isHit,
                    previousTurn = "opponent",
                    turn = "player"
                });

                string opponentSocketId = playerBattle.BattleFields.FirstOrDefault(battleField => battleField.SocketId != socketId).SocketId;
                await WebSocketConnectionManager.GetSocketById(socketId).SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(senderMessage),
                                                                    offset: 0,
                                                                    count: senderMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
                await WebSocketConnectionManager.GetSocketById(opponentSocketId).SendAsync(
                    buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(receiverMessage),
                                                                    offset: 0,
                                                                    count: receiverMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
            }            
        }
    }
}