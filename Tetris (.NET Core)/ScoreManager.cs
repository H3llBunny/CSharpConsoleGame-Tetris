using System.Text.RegularExpressions;
using static System.Formats.Asn1.AsnWriter;

namespace Tetris
{
    public class ScoreManager
    {
        private readonly string highScoreFile;

        public ScoreManager(string highScoreFile)
        {
            this.highScoreFile = highScoreFile;
            this.HighScore = this.GetHihgScore();
        }

        public int Score { get; private set; }

        public int HighScore { get; private set; }

        public void AddToScore(int addToScore)
        {
            this.Score += addToScore;

            if (this.Score > this.HighScore)
            {
                this.HighScore = this.Score;
            }
        }

        public void AddToHighScore()
        {
            File.AppendAllLines(this.highScoreFile, new List<string>
            {
                            $"[{DateTime.Now.ToString()}] {Environment.UserDomainName} => {this.Score}"
                        });
        }

        private int GetHihgScore()
        {
            var highScore = 0;

            var allScores = File.ReadAllLines(this.highScoreFile);

            foreach (var score in allScores)
            {
                var match = Regex.Match(score, @" => (?<score>[0-9]+)");
                highScore = Math.Max(highScore, int.Parse(match.Groups["score"].Value));
            }

            return highScore;
        }
    }
}
