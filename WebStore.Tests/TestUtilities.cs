using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Tests
{
    public static class TestUtilities
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[{}]\\\'\"/`,.<>?;:~| ";
        
        public static string CreateRandomString(int min, int max)
        {
            Random random = new Random();
            int length = random.Next(min, max);
            string result = "";
            while(length-- != 0)
            {
                result += validChars[random.Next(validChars.Length)];
            }
            return result;
        }


    }
}
