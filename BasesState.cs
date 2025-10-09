using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameProject
{
    public class BasesState
    {

        //First : 1루  Second : 2루 Third : 3루
        public Player First { get; set; }
        public Player Second { get; set; }
        public Player Third { get; set; }

        
        //현 베이스에 있는 주자들을 null체크 후 반환하는 IEnumerable<Player> 구현   / return : Occupants(주자)
        public IEnumerable<Player> Runners()
        {
            if (First != null) yield return First;
            if (Second != null) yield return Second;
            if (Third != null) yield return Third;
        }

        //베이스 초기화 시킴.
        public void Clear()
        {
            First = Second = Third = null;
        }

        //주자 이동 처리 : 3루-> 2루->1루 순서로 이동  (득점 주자 먼저 처리!)
        //bases : 주자가 이동할 베이스 수
        //batter: 타자
        //return : 이동한 주자 수
       
        public int AdvanceRunners(int bases, out List<Player> scorers, Player batter = null)
        {
            scorers = new List<Player>();
            int runs = 0;

            
            if (Third != null)
            {
                if (bases >= 1) { runs++; scorers.Add(Third); Third = null; }
            }

            
            if (Second != null)
            {
                if (bases >= 2) { runs++; scorers.Add(Second); Second = null; }
                else if (bases == 1) { Third = Second; Second = null; }
            }

            
            if (First != null)
            {
                if (bases >= 3) { runs++; scorers.Add(First); First = null; }
                else if (bases == 2) { Third = First; First = null; }
                else if (bases == 1) { Second = First; First = null; }
            }

            //타자 이동 처리
            //return : 득점한 주자 수
            if (batter != null)
            {
                if (bases >= 4) { runs++; scorers.Add(batter); }
                else if (bases == 3) Third = batter;
                else if (bases == 2) Second = batter;
                else if (bases == 1) First = batter;
            }

            return runs; 
        }

        //1루 진루처리 (단일 이동) / return : 득점한 주자 수
        public int AdvanceOne(Player batter, out List<Player> scorers)
        {
            scorers = new List<Player>();
            if (Third != null) { scorers.Add(Third); Third = null; }
            Third = Second; Second = First; First = batter;
            return scorers.Count; 
        }

       
       
        //ForceWalk : 볼넷
        //볼넷 처리 : 만루 시 3루 주자 득점 / return : 득점한 주자 수
        public int ForceWalk(Player batter, out List<Player> scorers)
        {
            scorers = new List<Player>();
            if (First == null) { First = batter; return 0; }
            if (Second == null) { Second = First; First = batter; return 0; }
            if (Third == null) { Third = Second; Second = First; First = batter; return 0; }
            scorers.Add(Third); Third = Second; Second = First; First = batter;              
            return 1;
        }
    }

}
