using System;
using System.Text;

namespace Utils
{
    public class JwtDecoder
    {
        public static string DecodeJwtToken(string token)
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new InvalidOperationException("JWT token must have 3 parts separated by '.'");
            }

            string header = DecodeBase64Url(parts[0]);
            string payload = DecodeBase64Url(parts[1]);
            string signature = DecodeBase64Url(parts[2]);

            return header + "." + payload + "." + signature;
        }

        private static string DecodeBase64Url(string input)
        {
            string output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new InvalidOperationException("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return Encoding.UTF8.GetString(converted);
        }
    }
}