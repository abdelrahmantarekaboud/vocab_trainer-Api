namespace VocabTrainer.Api.Helpers
{
    public static class PasswordHasherHelper
    {
        public static string Hash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public static bool Verify(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}