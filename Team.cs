using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameProject
{
    public class Team
    {
        public string Name { get; private set; }
        public List<Player> Lineup { get; private set; }
        public Queue<Player> Pitchers { get; private set; }
        public Dictionary<string, Player> Roster { get; private set; }
        public int Score { get; set; }
        public int RunsThisHalf { get; set; }

        public Team(string name)
        {
            Name = name;
            Lineup = new List<Player>();
            Pitchers = new Queue<Player>();
            Roster = new Dictionary<string, Player>();
        }

        public void AddPlayer(Player p)
        {
            Lineup.Add(p);
            AddToRoster(p);
        }

        public void AddPitcher(Player p)
        {
            Pitchers.Enqueue(p);
            AddToRoster(p);
        }

        public Player ChangePitcher()
        {
            return Pitchers.Count > 0 ? Pitchers.Dequeue() : null;
        }

        public Player PeekNextPitcher()
        {
            return Pitchers.Count > 0 ? Pitchers.Peek() : null;
        }

        public void ResetHalfInningRuns()
        {
            RunsThisHalf = 0;
        }

        private void AddToRoster(Player p)
        {
            if (!Roster.ContainsKey(p.Name))
                Roster[p.Name] = p;
        }

        public override string ToString() => Name;
    }

}
