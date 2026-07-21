using System.Collections.Generic;
using UnityEngine;
using LaneWar.Enemies;

namespace LaneWar.Units
{
    // 사거리 내 적 1명에게 즉시 데미지를 입히는 단일 타겟 공격. 근접(SingleMelee)/원거리(SingleRanged)는
    // 연출 거리(range)만 다를 뿐 타겟팅/타격 로직은 동일하므로 이 전략을 공유한다
    public class SingleTargetAttackBehavior : IAttackBehavior
    {
        public void Attack(UnitAttackContext context)
        {
            if (!IsValidTarget(context.CurrentTarget, context))
            {
                context.CurrentTarget = FindNearestEnemyInRange(context);
            }

            context.CurrentTarget?.TakeDamage(context.Damage);
        }

        private static bool IsValidTarget(Enemy target, UnitAttackContext context)
        {
            if (target == null || target.IsDead)
            {
                return false;
            }

            return Vector3.Distance(context.Position, target.transform.position) <= context.Range;
        }

        // 가장 가까운 적을 선택한다 (구현이 쉬운 방식. 경로상 가장 앞선 적을 고르려면 각 적의 진행도 비교가 추가로 필요해 더 복잡하다)
        private static Enemy FindNearestEnemyInRange(UnitAttackContext context)
        {
            Enemy nearest = null;
            float nearestDistance = float.MaxValue;

            IReadOnlyList<Enemy> enemies = context.AllEnemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy candidate = enemies[i];
                if (candidate.IsDead)
                {
                    continue;
                }

                float distance = Vector3.Distance(context.Position, candidate.transform.position);
                if (distance <= context.Range && distance < nearestDistance)
                {
                    nearest = candidate;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }
    }
}
