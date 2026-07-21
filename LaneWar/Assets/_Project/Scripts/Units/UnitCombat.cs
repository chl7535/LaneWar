using System.Collections.Generic;
using UnityEngine;
using LaneWar.Data;
using LaneWar.Enemies;

namespace LaneWar.Units
{
    // 유닛의 자동 전투(공속 간격 타이밍 + 공격 스타일 위임)를 담당하는 순수 로직. MonoBehaviour에 의존하지 않는다
    public class UnitCombat
    {
        private readonly UnitData _data;
        private readonly IAttackBehavior _attackBehavior;
        private readonly UnitAttackContext _context;

        private float _attackTimer;

        public UnitCombat(UnitData data, Vector3 position)
        {
            _data = data;
            _attackBehavior = AttackBehaviorFactory.Create(data.AttackStyle);
            _context = new UnitAttackContext
            {
                Position = position,
                Range = data.Range,
                Damage = data.Damage
            };
        }

        public Vector3 Position
        {
            get => _context.Position;
            set => _context.Position = value;
        }

        public void Tick(float deltaTime, IReadOnlyList<Enemy> allEnemies)
        {
            _context.AllEnemies = allEnemies;

            float attackInterval = 1f / _data.AttacksPerSecond;
            _attackTimer += deltaTime;

            if (_attackTimer < attackInterval)
            {
                return;
            }

            _attackTimer -= attackInterval;
            _attackBehavior.Attack(_context);
        }
    }
}
