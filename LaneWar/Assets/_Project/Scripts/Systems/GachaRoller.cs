using System;
using LaneWar.Data;

namespace LaneWar.Systems
{
    // GachaConfig의 풀에서 균등 확률로 유닛 1종을 뽑는 순수 로직. MonoBehaviour에 의존하지 않는다
    public class GachaRoller
    {
        private readonly GachaConfig _config;
        private readonly Random _random;

        public GachaRoller(GachaConfig config, Random random = null)
        {
            _config = config;
            _random = random ?? new Random();
        }

        public int Cost => _config.GachaCost;

        // 풀에서 균등 확률로 1종을 뽑는다
        public UnitData RollUnit()
        {
            var pool = _config.Pool;
            int index = _random.Next(pool.Count);
            return pool[index];
        }
    }
}
