using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniGameProject
{
    public static class Visuals
    {
        public static void GameProgress(Team home, Team away, int inning, bool top, int outs, BasesState bases)
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============================================");
            Console.WriteLine("           프로야구 게임 시뮬레이터       ");
            Console.WriteLine("==============================================");
            Console.ResetColor();
            Console.WriteLine("{0} (Away)  {1} (HOME)", away.Name, home.Name);
            Console.WriteLine("스코어: {0} - {1}", away.Score, home.Score);
            Console.WriteLine("이닝: {0}회 {1}   아웃: {2}", inning, top ? "초" : "말", outs);
            Console.WriteLine("주자: 1루[{0}] 2루[{1}] 3루[{2}]", bases.First != null ? bases.First.Name : "---", bases.Second != null ? bases.Second.Name : "---", bases.Third != null ? bases.Third.Name : "---");
            Console.WriteLine("--------------------------------------------------");
        }

        

        public static void ShowPlayLog(IEnumerable<string> plays)
        {
            Console.WriteLine("플레이 상황:");
            foreach (var p in plays) Console.WriteLine(" - " + p);
            Console.WriteLine("--------------------------------------------------");
        }

        public static void ShowBoxScore(Team away, Team home)
        {
            Console.WriteLine("==================== 최종 스코어 & 기록 ===================");
            Console.WriteLine("{0} : {1}", away.Name, away.Score);
            foreach (var p in away.Lineup)
            {
                Console.WriteLine($"{p.Name} - {p.Stats}");
            }
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("{0} : {1}", home.Name, home.Score);
            foreach (var p in home.Lineup)
            {
                Console.WriteLine($"{p.Name} - {p.Stats}");
            }
            Console.WriteLine("===========================================================");
        }
    }
}
