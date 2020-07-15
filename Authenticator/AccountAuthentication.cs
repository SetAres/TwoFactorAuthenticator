using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Authenticator
{
    public abstract class AccountAuthentication
    {
        private readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private string secretKey;

        public AccountAuthentication(string secretKey)
        {
            this.secretKey = secretKey;
        }
        public string GetCode()
        {
            var decodeSecretCode = Base32Encoding.ToBytes(secretKey);

            return GenerateCode(decodeSecretCode, GetCurrentCounter());
        }

        public string GeneratePassword(string secretKey, long iterationNumber, int digits = 6)
        {
            byte[] counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            HMACSHA1 hmac = new HMACSHA1(key, true);

            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[hash.Length - 1] & 0xf;

            int binary =
                ((hash[offset] & 0x7f) << 24)
                | ((hash[offset + 1] & 0xff) << 16)
                | ((hash[offset + 2] & 0xff) << 8)
                | (hash[offset + 3] & 0xff);

            int password = binary % (int)Math.Pow(10, digits); // 6 digits

            return password.ToString(new string('0', digits));
        }

        public string GenerateCode(byte[] secretKey, long iterationNumber, int digits = 6)
        {
            byte[] counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            HMACSHA1 hmac = new HMACSHA1(secretKey, true);

            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[hash.Length - 1] & 0xf;

            int binary =
                ((hash[offset] & 0x7f) << 24)
                | ((hash[offset + 1] & 0xff) << 16)
                | ((hash[offset + 2] & 0xff) << 8)
                | (hash[offset + 3] & 0xff);

            int password = binary % (int)Math.Pow(10, digits); // 6 digits

            return password.ToString(new string('0', digits));
        }

        public bool IsValid(string secret, string password, int checkAdjacentIntervals = 1)
        {
            if (password == GetCode())
                return true;

            for (int i = 1; i <= checkAdjacentIntervals; i++)
            {
                if (password == GeneratePassword(secret, GetCurrentCounter() + i))
                    return true;

                if (password == GeneratePassword(secret, GetCurrentCounter() - i))
                    return true;
            }

            return false;
        }

        private long GetCurrentCounter()
        {
            long counter = (long)(DateTime.UtcNow - UNIX_EPOCH).TotalSeconds / 30;
            return counter;
        }
    }
}
