using MKsEMS.Data;
using SQLitePCL;
using System.Linq;
using System.Text;
namespace MKsEMS.Services
{
    public static class EncDecPassword
    { 
        //this function Convert to Encord your Password
        public static string Enc64bitsPass(string password)
        {            
            try
            {  
                string saltStart = AddSaltStart(), saltEnd = AddSaltEnd();

                password = string.Concat(saltStart, password, saltEnd);
                
                byte[] encodeData = new byte[password.Length];
                encodeData = Encoding.UTF8.GetBytes(password);                

                string encPassword = Convert.ToBase64String(encodeData);
                return string.Concat(saltStart.Length, encPassword, saltEnd.Length);
            }
            catch (Exception e)
            {
                throw new Exception("Password could not be encrypted" + e.Message);
            }
        }

        //this function Convert to Decord your Password
        public static string DecodeFrom64(string encryptedPass)
        {              
             //extracting the lengh of added salts
            int saltStartLeng = int.Parse(encryptedPass.Substring(0, 2));
            int saltEndLeng = int.Parse(encryptedPass.Substring(encryptedPass.Length -2));

            //we then remove the overheads saltStartLeng, and saltEndLeng from the encryptedPass
            encryptedPass = encryptedPass.Substring(2);
            encryptedPass = encryptedPass.Substring(0, encryptedPass.Length - 2);

            byte[] toDecodeByte = Convert.FromBase64String(encryptedPass);

            UTF8Encoding encoder = new();
            Decoder utf8Decode = encoder.GetDecoder();
            byte[] passToDecrypt = Convert.FromBase64String(encryptedPass);
            int charCount = utf8Decode.GetCharCount(passToDecrypt, 0, passToDecrypt.Length);
            char[] decryptedChars = new char[charCount];
            utf8Decode.GetChars(passToDecrypt, 0, passToDecrypt.Length, decryptedChars, 0);
            
            string decPassword = new String(decryptedChars);           

            //stripping away all remaing overheads
            decPassword = decPassword.Substring(saltStartLeng);
            decPassword = decPassword.Substring(0, decPassword.Length - saltEndLeng);
        
            return decPassword;
        }
        public static string AddSaltEnd()
        {
            Dictionary<int, string> saltEndDict = new();
            
            saltEndDict[0] = "J";
            saltEndDict[1] = "N";
            saltEndDict[2] = "B";
            saltEndDict[3] = "t";
            saltEndDict[4] = "g";
            saltEndDict[5] = "h";
            saltEndDict[6] = "l";
            saltEndDict[7] = "d";
            saltEndDict[8] = "n";
            saltEndDict[9] = "y";

            string  saltEnd;
            DateTime today = DateTime.Now;           
           
            
            today = DateTime.Now;
            saltEnd = string.Concat(today.Year, today.Month, today.Day, today.Hour, today.Minute, today.Second, today.Millisecond);
           

            for (int i = 0; i < 10; i++)
            {
                if (saltEnd.Contains(i.ToString()))
                {
                    saltEnd = saltEnd.Replace(i.ToString(), saltEndDict[i]);
                }
            }

            return String.Concat(saltEnd);
        }
        public static string AddSaltStart()
        {
            Dictionary<int, string> saltStartDict = new();
            saltStartDict[0] = "y";
            saltStartDict[1] = "K";
            saltStartDict[2] = "c";
            saltStartDict[3] = "X";
            saltStartDict[4] = "I";
            saltStartDict[5] = "a";
            saltStartDict[6] = "q";
            saltStartDict[7] = "d";
            saltStartDict[8] = "m";
            saltStartDict[9] = "Z";

            string saltStart;
            DateTime today = DateTime.Now;

            saltStart = string.Concat(today.Year, today.Month, today.Day, today.Hour, today.Minute, today.Second, today.Millisecond);
            // char[] charToday = SaltStart.ToCharArray();            

            for (int i = 0; i < 10; i++)
            {
                if (saltStart.Contains(i.ToString()))
                {
                    saltStart = saltStart.Replace(i.ToString(), saltStartDict[i]);
                }
            }           

            return String.Concat(saltStart);
        }        

    }
}
