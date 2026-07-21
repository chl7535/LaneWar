using System.Collections.Generic;
using UnityEngine;
using LaneWar.Enemies;

namespace LaneWar.Units
{
    // 공격 전략(IAttackBehavior)에 전달되는 컨텍스트. 단일 타겟 스타일은 CurrentTarget에 상태를 유지해 사거리 이탈/사망 시에만 재탐색한다
    public class UnitAttackContext
    {
        public Vector3 Position;
        public float Range;
        public float Damage;
        public IReadOnlyList<Enemy> AllEnemies;
        public Enemy CurrentTarget;
    }
}
