namespace Assignment_wt1.Interfaces
{
    public interface ICodeGenerator
    {
        public string GenerateCode(int minLength, int maxLength);
        public string GenerateCodeChallenge(string code);
    }
}
