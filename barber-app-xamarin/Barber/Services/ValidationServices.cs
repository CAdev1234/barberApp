using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Barber.Services
{
    /// <summary>
    /// Vaildation class to check value before make action on it
    /// </summary>
    class ValidationServices
    {
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        //const string phonenumberRegex = @"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$";
        //const string phonenumberRegex = @"/^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0 - 9]{7})$/i";
        //const string phonenumberRegex = @"^(01)[0-9]{7}";
        const string phonenumberRegex = @"\(?[\+|(00)][0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?";

        /// <summary>
        /// Public static method to validate email string
        /// </summary>
        /// <param name="email"></param>
        /// <returns>return true if is vaild email, invaild email return false</returns>
        public static bool EmailValidation(string email)
        {
            bool isVaild = false;
            if (!string.IsNullOrEmpty(email))
                isVaild = (Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            return isVaild;
        }
        /// <summary>
        /// Public static method to validate phonenumber string
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool PhoneNumberVaildation(string phoneNumber)
        {
            bool isVaild = false;
            if (!string.IsNullOrEmpty(phoneNumber))
                isVaild = (Regex.IsMatch(phoneNumber, phonenumberRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            return isVaild;
        }
        /// <summary>
        /// public static method to check password strog with two ref param return boolean
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordMessage"></param>
        /// <param name="messageColor"></param>
        /// <returns></returns>
        public static bool PasswordStrong(string password, ref string passwordMessage, ref Color messageColor)
        {
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    if (IsContainApperChar(password) && IsContainDigit(password) && IsContainSpecialChar(password) && password.Length >= 6)
                    {
                        passwordMessage = "Strong Password";
                        messageColor = Color.Blue;
                        return true;
                    }
                    else if ((IsContainApperChar(password) || IsContainDigit(password) || IsContainSpecialChar(password)) && password.Length >= 6)
                    {
                        passwordMessage = "Good Password";
                        messageColor = Color.LightGreen;
                        return true;
                    }
                    else if (password.Length >= 6)
                    {
                        passwordMessage = "Weak Password";
                        messageColor = Color.Yellow;
                        return true;
                    }
                    else
                    {
                        passwordMessage = "Password too short";
                        messageColor = Color.Red;
                        return false;
                    }
                }
                else
                {
                    passwordMessage = "Password too short";
                    messageColor = Color.Red;
                    return false;
                }
            }
            catch (Exception)
            {
                passwordMessage = "Password too short";
                messageColor = Color.Red;
                return false;
            }
        }
        /// <summary>
        /// Public static method to check if string contain apper character
        /// </summary>
        /// <param name="str"></param>
        /// <returns>true if contain apper character</returns>
        public static bool IsContainApperChar(string str)
        {
            Regex rg = new Regex(@"^(?=.*[A-Z])");
            return rg.IsMatch(str);
        }
        /// <summary>
        /// Public static method to check if string contain white space
        /// </summary>
        /// <param name="str"></param>
        /// <returns>true if contain white space</returns>
        public static bool IsContainSpecialChar(string str)
        {
            Regex rg = new Regex(@"^(?=.*[_\W])");
            return rg.IsMatch(str);
        }
        /// <summary>
        /// Public static method to check if string contain digit
        /// </summary>
        /// <param name="str"></param>
        /// <returns>true if contain digit</returns>
        public static bool IsContainDigit(string str)
        {
            Regex rg = new Regex(@"^(?=.*\d)");
            return rg.IsMatch(str);
        }
    }
}
