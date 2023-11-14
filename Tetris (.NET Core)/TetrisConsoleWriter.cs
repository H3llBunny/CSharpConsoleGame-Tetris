namespace Tetris
{
    public class TetrisConsoleWriter
    {
        private int tetrisRows;
        private int tetrisColumns;
        private int infoColumns;
        private int consoleRows;
        private int consoleColumns;
        private char tetrisCharacter;

        public TetrisConsoleWriter(int tetrisRow, int tetrisColumns, char tetrisCharacter, int inforColumns = 11)
        {
            this.tetrisRows = tetrisRow;
            this.tetrisColumns = tetrisColumns;
            this.infoColumns = inforColumns;
            this.consoleRows = 1 + this.tetrisRows + 1;
            this.consoleColumns = 1 + this.tetrisColumns + 1 + this.infoColumns + 1;
            this.tetrisCharacter = tetrisCharacter;

            this.Frame = 0;
            this.FramesToMoveFigure = 15;

            Console.WindowHeight = this.consoleRows + 1;
            Console.WindowWidth = this.consoleColumns;
            Console.BufferHeight = this.consoleRows + 1;
            Console.BufferWidth = this.consoleColumns;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Title = "Tetris v1.0";
            Console.CursorVisible = false;
        }

        public int Frame { get; set; }

        public int FramesToMoveFigure { get; private set; }

        public void DrawAll(ITetrisGame state, ScoreManager scoreManager)
        {
            this.DrawBorder();
            this.DrawGameState(3 + this.tetrisColumns, state, scoreManager);
            this.DrawTetrisField(state.TetrisField);
            this.DrawCurrentFigure(state.CurrentFigure, state.CurrentFigureRow, state.CurrentFigureCol);
        }

        public void DrawGameState(int startColumn, ITetrisGame state, ScoreManager scoreManager)
        {
            this.Write("Level:", 1, startColumn);
            this.Write(state.Level.ToString(), 2, startColumn);
            this.Write("Score:", 4, startColumn);
            this.Write(scoreManager.Score.ToString(), 5, startColumn);
            this.Write("Best:", 7, startColumn);
            this.Write(scoreManager.HighScore.ToString(), 8, startColumn);
            this.Write("Frame:", 10, startColumn);
            this.Write(this.Frame.ToString() + " / " + (this.FramesToMoveFigure - state.Level).ToString(), 11, startColumn);
            this.Write("Position:", 13, startColumn);
            this.Write($"{state.CurrentFigureRow}, {state.CurrentFigureCol}", 14, startColumn);
            this.Write("Keys:", 16, startColumn);
            this.Write($"  ^ ", 18, startColumn);
            this.Write($"<   > ", 19, startColumn);
            this.Write($"  v  ", 20, startColumn);
        }

        public void DrawBorder()
        {
            Console.SetCursorPosition(0, 0);
            string line = "╔";
            line += new string('═', this.tetrisColumns);
            line += "╦";
            line += new string('═', this.infoColumns);
            line += "╗";

            Console.WriteLine(line);

            for (int i = 0; i < this.tetrisRows; i++)
            {
                string middleLine = "║";
                middleLine += new string(' ', this.tetrisColumns);
                middleLine += "║";
                middleLine += new string(' ', this.infoColumns);
                middleLine += "║";
                Console.Write(middleLine);
            }

            string endLine = "╚";
            endLine += new string('═', this.tetrisColumns);
            endLine += "╩";
            endLine += new string('═', this.infoColumns);
            endLine += "╝";

            Console.Write(endLine);
        }

        public void DrawTetrisField(Tetromino tetrisField)
        {
            for (int row = 0; row < tetrisField.Width; row++)
            {
                string line = "";

                for (int col = 0; col < tetrisField.Height; col++)
                {
                    if (tetrisField.Body[row, col])
                    {
                        line += tetrisCharacter;
                    }
                    else
                    {
                        line += " ";
                    }
                }

                this.Write(line, row + 1, 1);
            }
        }

        public void DrawCurrentFigure(Tetromino currentFigure, int currentFigureRow, int currentFigureColumn)
        {
            for (int row = 0; row < currentFigure.Width; row++)
            {
                for (int col = 0; col < currentFigure.Height; col++)
                {

                    if (currentFigure.Body[row, col])
                    {
                        this.Write(tetrisCharacter.ToString(), row + 1 + currentFigureRow, col + 1 + currentFigureColumn);

                    }
                }
            }
        }

        public void WriteGameOver(int score)
        {
            int row = this.tetrisRows / 2 - 3;
            int column = (this.tetrisColumns + 3 + this.infoColumns) / 2 - 7;

            var scoreAsString = score.ToString();
            scoreAsString = new string(' ', 7 - scoreAsString.Length) + scoreAsString;
            Write("╔═════════════╗", row, column);
            Write("║  Game       ║", row + 1, column);
            Write("║    over!    ║", row + 2, column);
            Write($"║  {scoreAsString}    ║", row + 3, column);
            Write("╚═════════════╝", row + 4, column);
        }

        private void Write(string text, int row, int col)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
    }
}
