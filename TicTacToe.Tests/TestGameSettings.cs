

using TicTacToe.Core.Configuration;

namespace TicTacToe.Tests
{
    public static class TestGameSettings
    {
        public static readonly GameSettings NormalSettings = new(3, 3);
        public static readonly GameSettings MediumSettings = new(5, 4);
        public static readonly GameSettings LargeSettings = new(10, 5);

        public static IEnumerable<object[]> GetSettings()
        {
            yield return new object[] { NormalSettings };
            yield return new object[] { MediumSettings };
            yield return new object[] { LargeSettings };
        }
    }
}
