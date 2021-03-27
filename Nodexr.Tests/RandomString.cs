using System;
using System.Collections.Generic;
using System.Text;

namespace Nodexr.Tests
{
    internal class RandomString
    {
        private const string Chars = @"ABCDEFGHIJK   LMNOPQR   STUVWXYZabcdefghijklmnopqrstuvwxyz0123456789?!@#$%^&*()_+-=\/{}";
        private readonly Random random = new();

        public string Next(int length = 8)
        {
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = Chars[random.Next(Chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
