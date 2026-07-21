using System.Collections.Generic;
using UnityEngine;
using LaneWar.Core;
using LaneWar.Data;

namespace LaneWar.Enemies
{
    // 등록된 모든 적 에이전트 사이의 분리력(회피)을 계산하고 각자의 이동을 갱신하는 순수 로직.
    // Boids의 separation 개념을 물리 충돌 없이 위치 계산만으로 구현한다. MonoBehaviour에 의존하지 않는다
    public class EnemyFlowSystem
    {
        private readonly PathSystem _pathSystem;
        private readonly EnemyFlowConfig _config;
        private readonly List<EnemyFlowAgent> _agents = new List<EnemyFlowAgent>();

        public EnemyFlowSystem(PathSystem pathSystem, EnemyFlowConfig config)
        {
            _pathSystem = pathSystem;
            _config = config;
        }

        public EnemyFlowAgent CreateAgent(float moveSpeed)
        {
            var agent = new EnemyFlowAgent(_pathSystem, _config, moveSpeed);
            _agents.Add(agent);
            return agent;
        }

        public void RemoveAgent(EnemyFlowAgent agent)
        {
            _agents.Remove(agent);
        }

        public void Tick(float deltaTime)
        {
            ApplySeparationForces();

            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].Tick(deltaTime);
            }
        }

        // 회피 감지 반경 안의 모든 쌍에 대해 거리 기반 분리력을 계산해 각자의 좌우 오프셋에 누적시킨다 (단순 거리 비교, 물리 충돌 없음)
        private void ApplySeparationForces()
        {
            float radius = _config.AvoidanceRadius;

            for (int i = 0; i < _agents.Count; i++)
            {
                EnemyFlowAgent a = _agents[i];

                for (int j = i + 1; j < _agents.Count; j++)
                {
                    EnemyFlowAgent b = _agents[j];

                    Vector3 delta = a.Position - b.Position;
                    float distance = delta.magnitude;
                    if (distance >= radius || distance <= Mathf.Epsilon)
                    {
                        continue;
                    }

                    Vector3 pushDirection = delta / distance;
                    float strength = _config.AvoidanceStrength * (1f - distance / radius);

                    a.AddSeparationForce(Vector3.Dot(pushDirection, a.Right) * strength);
                    b.AddSeparationForce(Vector3.Dot(-pushDirection, b.Right) * strength);
                }
            }
        }
    }
}
