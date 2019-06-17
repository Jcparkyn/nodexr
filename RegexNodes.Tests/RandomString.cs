using System;
using System.Collections.Generic;
using System.Text;

namespace RegexNodes.Tests
{
    class RandomString
    {
        const string chars = @"ABCDEFGHIJK   LMNOPQR   STUVWXYZabcdefghijklmnopqrstuvwxyz0123456789?!@#$%^&*()_+-=\/{}";
        Random random = new Random();

        public string Next(int length = 8)
        {
            
            var stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
