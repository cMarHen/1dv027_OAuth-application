namespace Assignment_wt1.Interfaces
{
    public interface IJwtHandler
    {
        public Dictionary<string, string> ExtractFieldsFromJwt(string jwtToken, List<string> fieldsToExtract);
    }
}
