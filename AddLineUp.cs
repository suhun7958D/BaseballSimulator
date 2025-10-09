using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameProject
{
    public static class AddLineUp
    {
        
        public static void BuildTeams(out Team away, out Team home)
        {
            away = new Team("한화 이글스");
            home = new Team("기아 타이거즈");

            // 원정팀 타자
            away.AddPlayer(new Player("채은성", 82, 72, 48, 42, 0, "1B"));
            away.AddPlayer(new Player("황영묵", 74, 63, 50, 58, 0, "2B"));
            away.AddPlayer(new Player("노시환", 76, 60, 47, 54, 0, "3B"));
            away.AddPlayer(new Player("리베라토", 69, 56, 60, 67, 0, "CF"));
            away.AddPlayer(new Player("이원석", 67, 52, 49, 51, 0, "RF"));
            away.AddPlayer(new Player("문현빈", 65, 51, 53, 63, 0, "LF"));
            away.AddPlayer(new Player("하주석", 62, 36, 44, 46, 0, "SS"));
            away.AddPlayer(new Player("최재훈", 59, 33, 43, 31, 0, "C"));
            away.AddPlayer(new Player("손아섭", 64, 44, 45, 49, 0, "DH"));

            // 원정팀 투수
            away.AddPitcher(new Player("폰세", 20, 10, 20, 20, 80, "P"));
            away.AddPitcher(new Player("박상원", 15, 5, 15, 15, 68, "P"));
            away.AddPitcher(new Player("김서현", 10, 3, 10, 10, 60, "P"));

            // 홈팀 타자
            home.AddPlayer(new Player("위즈덤", 80, 70, 50, 40, 0, "1B"));
            home.AddPlayer(new Player("김선빈", 75, 60, 55, 60, 0, "2B"));
            home.AddPlayer(new Player("김도영", 78, 65, 45, 55, 0, "3B"));
            home.AddPlayer(new Player("김호령", 70, 55, 60, 70, 0, "CF"));
            home.AddPlayer(new Player("나성범", 68, 50, 48, 50, 0, "RF"));
            home.AddPlayer(new Player("오선우", 66, 48, 52, 65, 0, "LF"));
            home.AddPlayer(new Player("박찬호", 60, 35, 40, 45, 0, "SS"));
            home.AddPlayer(new Player("한준수", 58, 30, 42, 30, 0, "C"));
            home.AddPlayer(new Player("최형우", 65, 45, 46, 47, 0, "DH"));

            // 홈팀 투수
            home.AddPitcher(new Player("네일", 22, 12, 18, 18, 82, "P"));
            home.AddPitcher(new Player("전상현", 18, 8, 14, 16, 70, "P"));
            home.AddPitcher(new Player("정해영", 14, 6, 12, 12, 62, "P"));
        }
    }
}
