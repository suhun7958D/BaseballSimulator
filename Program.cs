using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MiniGameProject
{
   



    internal class Program
    {
        static void Main(string[] args)
        {

            Team away, home;
            AddLineUp.BuildTeams(out away, out home);

            Game game = new Game(away, home);

            
            game.OnInningChanged += (inning, isTop) =>
            {
                Console.WriteLine();
                Console.WriteLine("=== 이닝 변경: {0}회 {1} ===", inning, isTop ? "초" : "말");
                Thread.Sleep(500);
            };

            game.OnScoreChanged += (msg) =>
            {
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[스코어업데이트] " + msg);
                Thread.Sleep(500);
                Console.ResetColor();
            };

            game.Run();
        }
    }
}
