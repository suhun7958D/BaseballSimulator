using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGameProject
{

    public enum Outcome
    {
        HomeRun, Triple, Double, Single, Walk, Bunt, StrikeOut, GroundOut, FlyOut, Error
    }

    public static class PlayEngine
    {
        private static Random rng = new Random();

        // 차이값(타자-투수)의 최대 보정치
        private const int MaxDiff = 30;
        
        
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }


        // 타석 결과를 계산하는 메서드
        public static Outcome ResolveAtBat(Player batter, Player pitcher, bool isBunt)
        {
            // 번트 시도라면 번트 전용 로직으로
            if (isBunt)
                return ResolveBunt(batter);

            // 타자와 투수 능력치 비교 → 차이값 계산
            int diff = CalculateDiff(batter, pitcher);

            // 능력치 기반 Outcome 확률 테이블 계산
            var chances = CalculateChances(batter, pitcher, diff);

            // 랜덤 Outcome 결정
            return RollOutcome(chances);
        }

  
        // 번트 시도 처리 로직  
        private static Outcome ResolveBunt(Player batter)
        {
            int buntRoll = rng.Next(100);

            // 성공 확률: 기본 45% + 주자의 스피드 보정
            int successThresh = 45 + (batter.Speed / 3);

            if (buntRoll < successThresh) return Outcome.Bunt;

            // 실패 시 → 땅볼 60% / 삼진 40%
            return (rng.Next(100) < 60) ? Outcome.GroundOut : Outcome.StrikeOut;
        }

        
        // 타자 vs 투수 능력치 차이 계산
        // (랜덤 보정 포함)
        private static int CalculateDiff(Player batter, Player pitcher)
        {
            int bat = batter.Hitting
                    + batter.Power / 2
                    + rng.Next(-12, 13)   // 랜덤 요소
                    + batter.Eye / 3;

            int pit = pitcher.Pitching + rng.Next(-12, 13);

            // 차이를 일정 범위(-30 ~ 30)로 제한
            return Clamp(bat - pit, -MaxDiff, MaxDiff);
        }

        // Outcome 별 확률 테이블 계산
        private static Dictionary<Outcome, int> CalculateChances(Player batter, Player pitcher, int diff)
        {
            return new Dictionary<Outcome, int>
        {
            { Outcome.Walk, Math.Max(2, batter.Eye / 5 + 5 - (pitcher.Pitching / 25)) },
            { Outcome.StrikeOut, Math.Max(6, 14 + (pitcher.Pitching - batter.Hitting) / 12) },
            { Outcome.HomeRun, Math.Max(1, batter.Power / 12) + Math.Max(-2, diff / 8) },
            { Outcome.Double, Math.Max(2, (batter.Hitting + batter.Power) / 30 + diff / 25) },
            { Outcome.Triple, Math.Max(1, batter.Speed / 35) },
            { Outcome.Single, Math.Max(10, batter.Hitting / 9 + diff / 15) },
            { Outcome.Error, Math.Max(1, 5 - (pitcher.Pitching / 30)) }
        };
        }

        // 누적 확률 기반으로 실제 Outcome 결정
        private static Outcome RollOutcome(Dictionary<Outcome, int> chances)
        {
            int roll = rng.Next(100);
            int cumulative = 0;

            foreach (var kvp in chances)
            {
                cumulative += kvp.Value;
                if (roll < cumulative)
                    return kvp.Key;
            }

            // 확률에 포함되지 않은 경우는 "아웃" 처리
            return (rng.Next(100) < 55) ? Outcome.GroundOut : Outcome.FlyOut;
        }
    }
}


