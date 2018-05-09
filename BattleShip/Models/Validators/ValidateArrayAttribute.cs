using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BattleShip.Models.Validators
{
    public class ValidateArrayAttributeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[,] battleShipArray = JsonConvert.DeserializeObject<string[,]>((string)value);
            int shipFieldsCount = 0;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (battleShipArray[i, j] != "")
                        shipFieldsCount++;
                }
            }

            if (shipFieldsCount != 17)
            {
                return new ValidationResult("You forgot to place all the ships!");
            }

            return ValidationResult.Success;
        }
    }
}
