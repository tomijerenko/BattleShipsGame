using BattleShip.Models.Validators;
using System.ComponentModel.DataAnnotations;

namespace BattleShip.Models
{
    public class GameInitModel
    {
        [Required(ErrorMessage = "You forgot your name!")]
        public string PlayerName { get; set; }
        [ValidateArray]
        public string SerializedBFArray { get; set; }
        public string SocketId { get; set; }
    }
}
