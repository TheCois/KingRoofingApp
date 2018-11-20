using System;
using System.Text;
using System.Text.RegularExpressions;

namespace KRF.Common
{
    public static class EncryptDecrypt
    {
        /// <summary>
        /// Function is used to encrypt the password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string EncryptString(string password)
        {
            var strmsg = string.Empty;
            var encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        /// <summary>
        /// Function is used to Decrypt the password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DecryptString(string encryptpwd)
        {
            var decryptpwd = string.Empty;
            var encodepwd = new UTF8Encoding();
            var Decode = encodepwd.GetDecoder();
            var todecode_byte = Convert.FromBase64String(encryptpwd);
            var charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            var decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
        public static string formatPhoneNumber(string phoneNum, string phoneFormat)
        {

            if (phoneFormat == "")
            {
                // If phone format is empty, code will use default format (###) ###-####
                phoneFormat = "(###) ###-####";
            }

            // First, remove everything except of numbers
            var regexObj = new Regex(@"[^\d]");
            phoneNum = regexObj.Replace(phoneNum, "");

            // Second, format numbers to phone string 
            if (phoneNum.Length > 0)
            {
                phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat);
            }

            return phoneNum;
        }
    }
}
