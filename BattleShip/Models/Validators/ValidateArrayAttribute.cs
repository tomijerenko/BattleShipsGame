using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BattleShip.Models.Validators
{
    public class ValidateArrayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[,] battleShipArray = JsonConvert.DeserializeObject<string[,]>((string)value);
            int shipFieldsCount = 0;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (battleShipArray[i, j] != "")
                        shipFieldsCount++;

            if (shipFieldsCount < 17)
                return new ValidationResult("There are not enough ships on the battlefield!");
            else if (shipFieldsCount > 17)
                return new ValidationResult("There are too many ships on the battlefield!");

            return ValidationResult.Success;
        }
    }
}