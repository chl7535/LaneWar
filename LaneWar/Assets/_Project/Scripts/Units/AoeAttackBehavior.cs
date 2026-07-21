using System.Collections.Generic;
using UnityEngine;
using LaneWar.Enemies;

namespace LaneWar.Units
{
    // 사거리 내 모든 적에게 동시에 데미지를 입히는 광역 공격
    public class AoeAttackBehavior : IAttackBehavior
    {
        public void Attack(UnitAttackContext context)
        {
            IReadOnlyList<Enemy> enemies = context.AllEnemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy candidate = enemies[i];
                if (candidate.IsDead)
                {
                    continue;
                }

                if (Vector3.Distance(context.Position, candidate.transform.position) <= context.Range)
                {
                    candidate.TakeDamage(context.Damage);
                }
            }
        }
    }
}
