using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameProject
{
    public class Player
    {
        public string Name { get; private set; } 
        public int Hitting { get; private set; } 
        public int Power { get; private set; }   
        public int Eye { get; private set; }     
        public int Speed { get; private set; }   
        public int Pitching { get; private set; } 
        public string Position { get; set; }      
        public PlayerStats Stats { get; private set; } 

        
        public bool IsPitcher { get { return Position == "P"; } } // 투수인지 판별


        //생성자 : 선수 능력치와 포지션 초기화
        public Player(string name, int hitting, int power, int eye, int speed, int pitching = 0, string position = "F") 
        {
            Name = name;
            Hitting = Math.Max(0, Math.Min(100, hitting));            //능력치 제한
            Power = Math.Max(0, Math.Min(100, power));                //능력치 제한
            Eye = Math.Max(0, Math.Min(100, eye));                    //능력치 제한
            Speed = Math.Max(0, Math.Min(100, speed));                //능력치 제한
            Pitching = Math.Max(0, Math.Min(100, pitching));          //능력치 제한
            Position = position;
            Stats = new PlayerStats(); //경기 기록 초기화
        }

        public override string ToString()
        {
            return $"{Name} ({Position} H:{Hitting} P:{Power} E:{Eye} S:{Speed} Pitch:{Pitching})";
        }
    }
}
