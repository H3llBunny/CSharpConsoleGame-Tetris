﻿using System.Data;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tetris
{
    public static class Program
    {
        //Settings
        static int TetrisRows = 20;
        static int TetrisCols = 10;
        static List<Tetromino> TetrisFigures = new List<Tetromino>()
        {
            new Tetromino(new bool[,]  // ----
            {
                { true, true, true, true }
            }),
            new Tetromino(new bool[,] // O
            {
                { true, true },
                { true, true }
            }),
            new Tetromino(new bool[,] // T
            {
                { false, true, false },
                { true, true, true }
            }),
            new Tetromino(new bool[,] // S
            {
                { false, true, true },
                { true, true, false }
            }),
            new Tetromino(new bool[,] // Z
            {
                { true, true, false},
                { false, true, true }
            }),
            new Tetromino(new bool[,] // J
            {
                { true, false, false},
                { true, true, true }
            }),
            new Tetromino(new bool[,] // L
            {
                { false, false, true},
                { true, true, true }
            }),
        };
        static int[] ScorePerLines = { 0, 40, 100, 300, 1200 };

        //State
        static TetrisGameState State = new TetrisGameState(TetrisRows, TetrisCols);

        public static void Main()
        {
            ScoreManager scoreManager = new ScoreManager("scores.txt");
            var musicPlayer = new MusicPlayer();
            musicPlayer.Play();

            var tetrisConsoleWriter = new TetrisConsoleWriter(TetrisRows, TetrisCols);
            var random = new Random();
            State.CurrentFigure = TetrisFigures[random.Next(0, TetrisFigures.Count)];

            while (true)
            {
                State.Frame++;
                State.UpdateLevel(scoreManager.Score);
                // Read user input
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        // Environment.Exit(0);
                        return; // Because of Main()
                    }

                    if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A)
                    {

                        if (State.CurrentFigureCol >= 1 && !CollisionLeft(State.CurrentFigure))

                        {
                            State.CurrentFigureCol--;
                        }
                    }

                    if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D)
                    {
                        if (State.CurrentFigureCol < TetrisCols - State.CurrentFigure.Height && !CollisionRight(State.CurrentFigure))
                        {
                            State.CurrentFigureCol++;
                        }

                    }

                    if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                    {
                        State.Frame = 1;
                        scoreManager.AddToScore(State.Level);
                        State.CurrentFigureRow++;
                    }

                    if (key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                    {
                        var newFigure = State.CurrentFigure.GetRotate();

                        if (!Collision(newFigure))
                        {
                            State.CurrentFigure = newFigure;
                        }
                    }
                }

                // Update the game state

                if (State.Frame % (State.FramesToMoveFigure - State.Level) == 0)
                {
                    State.CurrentFigureRow++;
                    State.Frame = 0;
                }

                if (Collision(State.CurrentFigure))
                {
                    AddCurrentFigureToTetrisField();

                    int lines = CheckForFullLines();
                    scoreManager.AddToScore(ScorePerLines[lines] * State.Level);
                    //      if (lines remove) Score++;
                    // 10, 30, 60, 100
                    State.CurrentFigure = TetrisFigures[random.Next(0, TetrisFigures.Count)];
                    State.CurrentFigureRow = 0;
                    State.CurrentFigureCol = 0;

                    if (Collision(State.CurrentFigure)) // Game is over
                    {

                        scoreManager.AddToHighScore();
                        tetrisConsoleWriter.DrawAll(State, scoreManager);
                        tetrisConsoleWriter.WriteGameOver(scoreManager.Score);
                        Thread.Sleep(100000);
                        return;
                    }
                }

                // Redraw UI
                tetrisConsoleWriter.DrawAll(State, scoreManager);

                Thread.Sleep(40);
            }
        }

        private static int CheckForFullLines() // 0, 1, 2, 3, 4
        {
            int lines = 0;

            for (int row = 0; row < State.TetrisField.Width; row++)
            {
                bool rowIsFull = true;

                for (int col = 0; col < State.TetrisField.Height; col++)
                {
                    if (State.TetrisField.Body[row, col] == false)
                    {
                        rowIsFull = false;
                        break;
                    }
                }

                if (rowIsFull)
                {
                    for (int rowToMove = row; rowToMove >= 1; rowToMove--)
                    {
                        for (int col = 0; col < State.TetrisField.Height; col++)
                        {
                            State.TetrisField.Body[rowToMove, col] = State.TetrisField.Body[rowToMove - 1, col];
                        }
                    }

                    lines++;
                }
            }

            return lines;
        }

        static void AddCurrentFigureToTetrisField()
        {
            for (int row = 0; row < State.CurrentFigure.Width; row++)
            {
                for (int col = 0; col < State.CurrentFigure.Height; col++)
                {
                    if (State.CurrentFigure.Body[row, col])
                    {
                        State.TetrisField.Body[State.CurrentFigureRow + row, State.CurrentFigureCol + col] = true;
                    }
                }
            }
        }

        static bool Collision(Tetromino figure)
        {
            if (State.CurrentFigureCol > TetrisCols - figure.Height)
            {
                return true;
            }

            if (State.CurrentFigureRow + figure.Width == TetrisRows)
            {
                return true;
            }

            for (int row = 0; row < figure.Width; row++)
            {
                for (int col = 0; col < figure.Height; col++)
                {
                    if (figure.Body[row, col] && State.TetrisField.Body[State.CurrentFigureRow + row + 1, State.CurrentFigureCol + col])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CollisionRight(Tetromino figure)
        {
            for (int row = 0; row < figure.Width; row++)
            {
                for (int col = 0; col < figure.Height; col++)
                {
                    if (State.CurrentFigureCol < TetrisCols - figure.Height)
                    {
                        if (figure.Body[row, col] && State.TetrisField.Body[State.CurrentFigureRow + row, State.CurrentFigureCol + col + 1])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool CollisionLeft(Tetromino figure)
        {
            for (int row = 0; row < figure.Width; row++)
            {
                for (int col = 0; col < figure.Height; col++)
                {
                    if (State.CurrentFigureCol > 0)
                    {
                        if (figure.Body[row, col] && State.TetrisField.Body[State.CurrentFigureRow + row, State.CurrentFigureCol + col - 1])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}