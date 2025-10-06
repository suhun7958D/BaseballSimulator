using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameProject
{
    
    //선수경기기록
    // AB : 타수 H: 안타 HR: 홈런 RBI :타점 BB: 볼넷 SO: 삼진 R: 득점 AVG : 타율
        
    public class PlayerStats 
    {
        public int AB { get; set; }
        public int H { get; set; }
        public int HR { get; set; }
        public int RBI { get; set; }
        public int BB { get; set; }
        public int SO { get; set; }
        public int R { get; set; }

        public override string ToString()
        {
            double avg = AB == 0 ? 0.0 : (double)H / AB;
            return $"AB:{AB}, H:{H}, HR:{HR}, RBI:{RBI}, BB:{BB}, SO:{SO}, R:{R}, AVG:{avg:0.000}";
        }
    }
}
