using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace planimals
{
    public class Game
    {
        public string username;
        private int time;
        private int score;

        public Game(string u, int t, int s) 
        {
            username = u;
            time = t;
            score = s;
        }
        public void UpdateScore(int points) => score += points;
        public string GetScore() => score.ToString();
        public void UpdateTime() => time -= 1;
        public string GetTime() => time.ToString();
    }
}
