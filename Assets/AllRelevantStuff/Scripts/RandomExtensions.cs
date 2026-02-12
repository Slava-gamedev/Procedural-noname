namespace ExtensionMethods
{

    public static class RandomExtensions
    {
        public static bool TossACoin(this System.Random random)
        {
            return random.Next(0, 2) == 0;
        }
    }
}
