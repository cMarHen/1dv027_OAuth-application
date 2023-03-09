using Assignment_wt1.Interfaces;
using System.Security.Cryptography;
using System.Text;

// Inspiration from https://stackoverflow.com/questions/65169984/how-to-implement-authorization-code-with-pkce-for-spotify
namespace Assignment_wt1.Utils
{
    public class CodeGenerator : ICodeGenerator
    {
        /// <summary>
        /// Generates a "random" code using Random()
        /// Current version use following characters: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~"
        /// 
        /// </summary>
        /// <param name="minLength">Minimum length of generated code.</param>
        /// <param name="maxLength">Maximum length of generated code.</param>
        /// <returns>A random generated string of length between minlength and maxlength.</returns>
        public string GenerateCode(int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
            var rnd = new Random();
            var length = rnd.Next(minLength, maxLength);
            var code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = chars[rnd.Next(chars.Length)];
            }

            return new string(code);
        }

        /// <summary>
        /// Generate a code challenge.
        /// Takes a string, and hash 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GenerateCodeChallenge(string code)
        {
            var hashFunction = SHA256.Create();
            var hash = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(code));

            return Convert.ToBase64String(hash).Replace("=", "").Replace("+", "-").Replace("/", "_");
        }
    }
}
