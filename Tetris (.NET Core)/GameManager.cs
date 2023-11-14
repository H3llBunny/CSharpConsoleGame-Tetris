﻿namespace Tetris
{
    public class GameManager
    {
        private ITetrisGame tetrisGame;
        private IInputHandler consoleInputHandler;
        private TetrisConsoleWriter tetrisConsoleWriter;
        private ScoreManager scoreManager;

        public GameManager(ITetrisGame tetrisGame, IInputHandler consoleInputHandler, TetrisConsoleWriter tetrisConsoleWriter, ScoreManager scoreManager)
        {
            this.tetrisGame = tetrisGame;
            this.consoleInputHandler = consoleInputHandler;
            this.tetrisConsoleWriter = tetrisConsoleWriter;
            this.scoreManager = scoreManager;
        }

        public void MainLoop()
        {
            while (true)
            {
                tetrisConsoleWriter.Frame++;
                this.tetrisGame.UpdateLevel(scoreManager.Score);

                var input = this.consoleInputHandler.GetInput();

                switch (input)
                {
                    case TetrisGameInput.Left:
                        if (this.tetrisGame.CurrentFigureCol >= 1 
                            && !this.tetrisGame.CollisionLeft(this.tetrisGame.CurrentFigure))
                        {
                            this.tetrisGame.CurrentFigureCol--;
                        }
                        break;
                    case TetrisGameInput.Right:
                        if (this.tetrisGame.CurrentFigureCol < this.tetrisGame.TetrisColumns 
                            - this.tetrisGame.CurrentFigure.Height 
                            && !this.tetrisGame.CollisionRight(this.tetrisGame.CurrentFigure))
                        {
                            this.tetrisGame.CurrentFigureCol++;
                        }
                        break;
                    case TetrisGameInput.Down:
                        tetrisConsoleWriter.Frame = 1;
                        scoreManager.AddToScore(this.tetrisGame.Level, 0);
                        this.tetrisGame.CurrentFigureRow++;
                        break;
                    case TetrisGameInput.Rotate:
                        var newFigure = this.tetrisGame.CurrentFigure.GetRotate();

                        if (!this.tetrisGame.Collision(newFigure))
                        {
                            this.tetrisGame.CurrentFigure = newFigure;
                        }
                        break;
                    case TetrisGameInput.Exit:
                        return;
                }

                // Update the game state

                if (tetrisConsoleWriter.Frame % (tetrisConsoleWriter.FramesToMoveFigure - this.tetrisGame.Level) == 0)
                {
                    this.tetrisGame.CurrentFigureRow++;
                    tetrisConsoleWriter.Frame = 0;
                }

                if (this.tetrisGame.Collision(this.tetrisGame.CurrentFigure))
                {
                    this.tetrisGame.AddCurrentFigureToTetrisField();

                    int lines = this.tetrisGame.CheckForFullLines();
                    scoreManager.AddToScore(this.tetrisGame.Level, lines);
                    //      if (lines remove) Score++;
                    // 10, 30, 60, 100
                    this.tetrisGame.NewRandomFigure();

                    if (this.tetrisGame.Collision(this.tetrisGame.CurrentFigure)) // Game is over
                    {

                        scoreManager.AddToHighScore();
                        tetrisConsoleWriter.DrawAll(this.tetrisGame, scoreManager);
                        tetrisConsoleWriter.WriteGameOver(scoreManager.Score);
                        Thread.Sleep(100000);
                        return;
                    }
                }

                // Redraw UI
                tetrisConsoleWriter.DrawAll(this.tetrisGame, scoreManager);

                Thread.Sleep(40);
            }
        }
    }
}