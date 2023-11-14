namespace Tetris
{
    public static class Program
    {
        public static void Main()
        {
            new MusicPlayer().Play();
            int TetrisRows = 20;
            int TetrisCols = 10;
            var gameManager = new GameManager(
                new TetrisGame(TetrisRows, TetrisCols),
                new ConsoleInputHandler(),
                new TetrisConsoleWriter(TetrisRows, TetrisCols, '*'),
                new ScoreManager("scores.txt"));
            gameManager.MainLoop();
        }
    }
}