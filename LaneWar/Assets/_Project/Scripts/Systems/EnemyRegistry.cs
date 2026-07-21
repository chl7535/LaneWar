using System.Collections.Generic;
using UnityEngine;
using LaneWar.Enemies;

namespace LaneWar.Systems
{
    // 현재 살아있는 Enemy 목록을 유지해 유닛의 타겟팅 조회에 사용되는 레지스트리
    public class EnemyRegistry : MonoBehaviour
    {
        private readonly List<Enemy> _activeEnemies = new List<Enemy>();

        public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;

        public void Register(Enemy enemy)
        {
            _activeEnemies.Add(enemy);
        }

        public void Unregister(Enemy enemy)
        {
            _activeEnemies.Remove(enemy);
        }
    }
}
