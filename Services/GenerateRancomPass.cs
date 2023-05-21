namespace MKsEMS.Services
{
    public static class GenerateRandomPass
    {
        /// <summary>
        /// Generates a Microsoft style temporary random password for the user
        /// </summary>
        /// <returns></returns>
        public static string GeTempPassword()
        {
            string[] alphabet = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            int index;

            string randomPassword = "";
            Random rand = new Random();

            //first characters in Upoercase and 3 in lower 
            for (int i = 0; i < 4; i++)
            {
                index = rand.Next(0, 25); //the 26 letters of the alphabet
                if (i == 0)
                {
                    randomPassword += alphabet[index].ToUpper();
                }
                else
                {
                    randomPassword += alphabet[index];
                }
            }
            //now adding 3 random numbers to the passoword
            for (int i = 0; i < 3; i++)
            {
                index = rand.Next(0, 9);
                randomPassword += index;
            }

            return randomPassword;
        }
    }
}
