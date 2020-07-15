namespace Authenticator
{
    public class AccountSettings
    {
        public int Id { get; set; }
        public string Email { get; private set; }
        public string Title { get; private set; }
        public string SecretKey { get; private set; }

        public AccountSettings(string title, string email, string secretKey)
        {
            Email = email;
            Title = title;
            SecretKey = secretKey;
        }
    }
}
