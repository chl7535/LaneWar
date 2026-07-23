using System;
using LaneWar.Data;

namespace LaneWar.Core
{
    // 라운드 번호에 따른 스탯 배율을 계산하는 순수 로직. 복리(제곱)/단리(비례) 두 방식을 지원한다
    public static class DifficultyScaler
    {
        public static float GetMultiplier(int round, float baseMultiplier, float growthRatePerRound, GrowthMode growthMode)
        {
            int roundsElapsed = round > 1 ? round - 1 : 0;

            float growthFactor = growthMode == GrowthMode.Compound
                ? (float)Math.Pow(1f + growthRatePerRound, roundsElapsed)
                : 1f + growthRatePerRound * roundsElapsed;

            return baseMultiplier * growthFactor;
        }
    }
}
