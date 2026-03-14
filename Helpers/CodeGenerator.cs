namespace VocabTrainer.Api.Helpers
{
    public static class CodeGenerator
    {
        private static readonly char[] Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

        public static string Generate(string prefix)
        {
            string block()
            {
                var a = new char[4];
                for (int i = 0; i < 4; i++)
                    a[i] = Chars[Random.Shared.Next(Chars.Length)];
                return new string(a);
            }

            return $"{prefix}-{block()}-{block()}";
        }
    }
}