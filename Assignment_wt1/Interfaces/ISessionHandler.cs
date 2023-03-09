namespace Assignment_wt1.Interfaces
{
    public interface ISessionHandler
    {
        public void StorePairInSession(string key, string value);
        public void StorePairInSession(string key, int value);
        public void ClearSession();
        public void ClearSession(string key);
        public string GetSession(string key);
        public int GetSessionCount(string key);
        public Task StoreLoginCookie(string username);
        public Task<string> GetLoginCookie();
    }
}
