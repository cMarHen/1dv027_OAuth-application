using Assignment_wt1.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Assignment_wt1.Utils
{
    public class JwtHandler : IJwtHandler
    {
        /// <summary>
        /// Extract key-value pairs from fields in a provided jwt.
        /// </summary>
        /// <param name="jwt">The jwt to extract data from.</param>
        /// <param name="fields">A list of fields to extract from a jwt.</param>
        /// <returns>Dictionary with key-value pairs.</returns>
        public Dictionary<string, string> ExtractFieldsFromJwt(string jwt, List<string> fields)
        {
            var token = new JwtSecurityToken(jwt);

            var extractedFields = new Dictionary<string, string>();

            foreach (var field in fields)
            {
                extractedFields[field] = token.Payload[field]?.ToString() ?? "";
            }

            return extractedFields;
        }
    }
}
