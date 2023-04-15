using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ussd.App.BLL.Helper
{
    public static class Validation
    {
       
        public static string? GetFirstName(this string name)
        {
            
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

           
            string[] nameParts = name.Split(' ');

           
            return nameParts[0];
        }

        public static bool ValidateFourDigits(string input)
        {
            Console.WriteLine($"Validating input string {input}");

            Console.WriteLine($"Lenght of input {input.Length}");



            // Check if the input is null or not a string
            if (input == null || !(input is string))
            {
                return false;
            }
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            // Check if the input consists of only spaces or is too long
            if (string.IsNullOrWhiteSpace(input) || input.Length != 4)
            {
                return false;
            }

            // Define a regular expression pattern that matches exactly four digits
            string pattern = @"^\d{4}$";

            // Create a regular expression object from the pattern
            Regex regex = new Regex(pattern);

            // Use the IsMatch method of the regular expression object to check if the input matches the pattern
            if (regex.IsMatch(input))
            {
                // The input has exactly four digits
                return true;
            }
            else
            {
                // The input does not have exactly four digits
                return false;
            }
        }



        public static bool ValidateNumber(decimal number, decimal balance, decimal threshold, out string message)
        {
            if (number <= balance && number <= threshold)
            {
                message = "Valid";
                return true;
            }
            else if (number >= balance && number >= threshold)
            {
                message = "Incorrect Selection";
                return false;
            }
            else if (number >= balance)
            {
                message = "Insufficient Funds";
                return false;
            }
            else // number >= threshold
            {
                message = "Maximum withdrawal amount is MK1,000,000";
                return false;
            }
        }


        public static bool ContainsTwoOrMoreNames(string input)
        {
            string[] names = input.Split(' ');

            // Check that each name is alphabetic and has at least two characters
            foreach (string name in names)
            {
                if (!Regex.IsMatch(name, "^[a-zA-Z]{2,}$"))
                {
                    return false;
                }
            }

            return names.Length >= 2;
        }

        public static bool IsPositiveNumber(string input)
        {
            if (decimal.TryParse(input, out decimal number))
            {
                if (number > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



    }




}
