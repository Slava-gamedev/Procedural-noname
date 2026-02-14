namespace ExtensionMethods
{

    public static class RandomExtensions
    {
        public static bool TossACoin(this System.Random random)
        {
            return random.NextDouble() > 0.5f;
        }
    }
}
