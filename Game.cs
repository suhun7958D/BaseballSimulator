using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MiniGameProject
{
    public class Game
    {
        private Team away;
        private Team home;
        private Player currentAwayPitcher;
        private Player currentHomePitcher;
        private int awayLineupIndex = 0;
        private int homeLineupIndex = 0;
        private BasesState bases = new BasesState();
        private int outs = 0;
        private int inning = 1;
        private bool top = true;
        private Queue<string> playLog = new Queue<string>();
        private Random rng = new Random();
        private int maxPlayLog = 10;
        private bool autoMode = false;
        private bool quitRequested = false;

        public event Action<int, bool> OnInningChanged;
        public event Action<string> OnScoreChanged;

        public Game(Team awayTeam, Team homeTeam)
        {
            away = awayTeam;
            home = homeTeam;

            //예외처리
            currentAwayPitcher = away.ChangePitcher() ?? throw new InvalidOperationException("원정팀에 남은 투수가 없습니다. 최소 1명의 투수가 필요합니다.");
            currentHomePitcher = home.ChangePitcher() ?? throw new InvalidOperationException("홈팀에 남은 투수가 없습니다. 최소 1명의 투수가 필요합니다.");
        }

        private void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            playLog.Enqueue(message);
            if (playLog.Count > maxPlayLog) playLog.Dequeue();

            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = prev;
        }

        private void ClearPlayLog()
        {
            playLog.Clear();
        }

        public void Run()
        {
            ShowIntro();

            while (!quitRequested && !IsGameOver())
            {
                Visuals.GameProgress(home, away, inning, top, outs, bases);
                Visuals.ShowPlayLog(playLog);

                Team battingTeam = top ? away : home;
                Team fieldingTeam = top ? home : away;
                Player pitcher = top ? currentHomePitcher : currentAwayPitcher;
                Player batter = top ? away.Lineup[awayLineupIndex] : home.Lineup[homeLineupIndex];

                Console.WriteLine("\n조작: S = 진행 | F = 번트 | A = 자동모드 | P = 투수교체 | Q = 종료");
                Console.Write("입력> ");
                char input = ReadUserCharWithTimeout(autoMode ? 150 : -1);

                if (char.ToUpper(input) == 'Q') { quitRequested = true; break; }
                if (char.ToUpper(input) == 'A') { autoMode = !autoMode; Log("[자동모드 " + (autoMode ? "ON" : "OFF") + "]", ConsoleColor.Cyan); continue; }

                if (char.ToUpper(input) == 'P')
                {
                    var next = fieldingTeam.ChangePitcher();
                    if (next != null)
                    {
                        Log($"{fieldingTeam.Name} 투수 교체: {next.Name} 투입", ConsoleColor.Magenta);
                        if (top) currentHomePitcher = next; else currentAwayPitcher = next;
                    }
                    else Log($"{fieldingTeam.Name} 교체 가능한 투수 없음", ConsoleColor.DarkRed);
                    continue;
                }

                bool isBunt = char.ToUpper(input) == 'F' || (autoMode && rng.Next(100) < 10);

                Outcome outcome = PlayEngine.ResolveAtBat(batter, pitcher, isBunt);
                ProcessOutcome(battingTeam, fieldingTeam, batter, pitcher, outcome);

                if (top) awayLineupIndex = (awayLineupIndex + 1) % away.Lineup.Count;
                else homeLineupIndex = (homeLineupIndex + 1) % home.Lineup.Count;

                if (autoMode) Thread.Sleep(1000);
            }

            Console.Clear();
            Console.WriteLine("경기 종료");
            Visuals.ShowBoxScore(away, home);
            Console.WriteLine("\n아무 키나 누르면 종료합니다.");
            Console.ReadKey(true);
        }

        private char ReadUserCharWithTimeout(int timeout)
        {
            if (timeout < 0)
            {
                var key = Console.ReadKey(true);
                return key.KeyChar;
            }
            else
            {
                DateTime start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < timeout)
                {
                    if (Console.KeyAvailable)
                        return Console.ReadKey(true).KeyChar;
                    Thread.Sleep(10);
                }
                return 'S';
            }
        }

        private void BroadcastScoreChange(string msg)
        {
            OnScoreChanged?.Invoke(msg);
            Log(msg, ConsoleColor.Yellow);
        }

        private void BroadcastInningChange()
        {
            OnInningChanged?.Invoke(inning, top);
        }

        private void ProcessOutcome(Team batting, Team fielding, Player batter, Player pitcher, Outcome outcome)
        {
            string logMsg = $"{batter.Name} {outcome}";
            ConsoleColor color = ConsoleColor.White;
            Console.WriteLine();
            string liveMsg = "";
            List<Player> scorers;
            int runsScored = 0;

            switch (outcome)
            {
                case Outcome.HomeRun:
                    batter.Stats.HR++; batter.Stats.H++; batter.Stats.AB++;
                    int br = bases.Runners().Count();
                    runsScored += br + 1;
                    foreach (var r in bases.Runners()) { r.Stats.R++; }
                    batter.Stats.R++; batter.Stats.RBI += br + 1;
                    batting.Score += br + 1; batting.RunsThisHalf += br + 1;
                    bases.Clear();
                    logMsg += " (홈런)";
                    liveMsg = $"{batter.Name}의 홈런! 관중석이 떠나갈 듯한 환호!";
                    color = ConsoleColor.Yellow;
                    break;
                case Outcome.Triple:
                    batter.Stats.H++; batter.Stats.AB++;
                    runsScored += bases.AdvanceRunners(3, out scorers, batter);
                    foreach (var s in scorers) { batting.Score++; s.Stats.R++; batter.Stats.RBI++; }
                    logMsg += " (3루타)"; liveMsg = $"{batter.Name} 빠른발로 3루타를 만들어냅니다 대단하네요!"; color = ConsoleColor.Magenta;
                    break;
                case Outcome.Double:
                    batter.Stats.H++; batter.Stats.AB++;
                    runsScored += bases.AdvanceRunners(2, out scorers, batter);
                    foreach (var s in scorers) { batting.Score++; s.Stats.R++; batter.Stats.RBI++; }
                    logMsg += " (2루타)"; liveMsg = $"{batter.Name} 담장을 맞추는 타구 2루까지 충분하네요!"; color = ConsoleColor.DarkGreen;
                    break;
                case Outcome.Single:
                    batter.Stats.H++; batter.Stats.AB++;
                    runsScored += bases.AdvanceOne(batter, out scorers);
                    foreach (var s in scorers) { batting.Score++; s.Stats.R++; batter.Stats.RBI++; }
                    logMsg += " (안타)"; liveMsg = $"{batter.Name} 전력 질주로 출루에 성공합니다."; color = ConsoleColor.Green;
                    break;
                case Outcome.StrikeOut:
                    outs++; batter.Stats.SO++; batter.Stats.AB++;
                    logMsg += " (삼진)"; liveMsg = $"{batter.Name} 헛스윙 삼진! 투수가 강력한 구위로 삼진을 잡아냅니다.!"; color = ConsoleColor.Red;
                    break;
                case Outcome.GroundOut:
                    outs++; batter.Stats.AB++;
                    logMsg += " (땅볼 아웃)"; liveMsg = $"{batter.Name} 힘없이 굴러간 땅볼, 수비가 손쉽게 처리합니다!"; color = ConsoleColor.Red;
                    break;
                case Outcome.FlyOut:
                    outs++; batter.Stats.AB++;
                    logMsg += " (플라이 아웃)"; liveMsg = $"{batter.Name} 높게 뜬볼, 그러나 수비가 손쉽게 처리합니다!"; color = ConsoleColor.Red;
                    break;
                case Outcome.Walk:
                    batter.Stats.BB++; batter.Stats.AB++;
                    runsScored += bases.ForceWalk(batter, out scorers);
                    foreach (var s in scorers) { batting.Score++; s.Stats.R++; batter.Stats.RBI++; }
                    logMsg += " (볼넷)"; liveMsg = $"{batter.Name} 좋은 선구안으로 볼넷을 만듭니다 !"; color = ConsoleColor.Cyan;
                    break;
                case Outcome.Bunt:
                    bool buntSuccess = rng.Next(100) < (55 + batter.Speed / 3);
                    if (buntSuccess)
                    {
                        batter.Stats.AB++;
                        runsScored += bases.AdvanceRunners(1, out scorers, null);
                        foreach (var s in scorers) { batting.Score++; s.Stats.R++; }
                        if (bases.First == null) bases.First = batter;
                        logMsg += " (번트 성공)"; liveMsg = $"{batter.Name} 번트 성공!"; color = ConsoleColor.Green;
                    }
                    else
                    {
                        outs++; batter.Stats.SO++; batter.Stats.AB++;
                        logMsg += " (번트 실패)"; liveMsg = $"{batter.Name} 번트 실패!"; color = ConsoleColor.Red;
                    }
                    break;
                case Outcome.Error:
                    batter.Stats.AB++;
                    runsScored += bases.AdvanceRunners(1, out scorers, batter);
                    foreach (var s in scorers) { batting.Score++; s.Stats.R++; batter.Stats.RBI++; }
                    logMsg += " (실책)"; liveMsg = $"실책! {batter.Name} 진루"; color = ConsoleColor.Red;
                    break;
            }

            if (runsScored > 0 && outcome != Outcome.HomeRun)
            {
                batting.Score += runsScored;
                batting.RunsThisHalf += runsScored;
            }

            Log(logMsg, color);
            Console.WriteLine(liveMsg);
            BroadcastScoreChange($"{away.Name} {away.Score} - {home.Score} {home.Name}");

            if (outs >= 3)
            {
                Log($" {inning}회 {(top ? "초" : "말")} 종료. 득점: {batting.RunsThisHalf}");
                outs = 0;
                batting.RunsThisHalf = 0;
                bases.Clear();
                if (!top) inning++;
                top = !top;
                BroadcastInningChange();
                ClearPlayLog();
                Log($"=== {inning}회 {(top ? "초" : "말")} 시작 ===");
            }

            // 랜덤 투수 교체
            if (rng.Next(1000) < 8)
            {
                var next = fielding.ChangePitcher();
                if (next != null)
                {
                    Log($"{fielding.Name} 투수 교체: {next.Name} 투입", ConsoleColor.Magenta);
                    if (top) currentHomePitcher = next; else currentAwayPitcher = next;
                }
            }
        }

        private bool IsGameOver()
        {
            if (inning > 9 && home.Score != away.Score) return true;
            if (inning == 9 && !top && home.Score != away.Score) return true;
            return false;
        }

        private void ShowIntro()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("######     #     #####  ####### ######     #    #       #        #####     #    #     # ####### ");
            Console.WriteLine("#     #   # #   #     # #       #     #   # #   #       #       #     #   # #   ##   ## #");
            Console.WriteLine("#     #  #   #  #       #       #     #  #   #  #       #       #        #   #  # # # # #");
            Console.WriteLine("######  #     #  #####  #####   ######  #     # #       #       #  #### #     # #  #  # #####");
            Console.WriteLine("#     # #######       # #       #     # ####### #       #       #     # ####### #     # # ");
            Console.WriteLine("#     # #     # #     # #       #     # #     # #       #       #     # #     # #     # #");
            Console.WriteLine("######  #     #  #####  ####### ######  #     # ####### #######  #####  #     # #     # #######");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("S = 진행 | F = 번트 | A = 자동모드 (ON/OFF) | P = 투수교체 | Q = 종료");;
            Console.ResetColor();
            Console.WriteLine("아무 키를 누르면 시작...");
            Console.ReadKey(true);
        }
    }

}
