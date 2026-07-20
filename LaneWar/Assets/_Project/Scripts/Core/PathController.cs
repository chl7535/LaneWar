using System.Collections.Generic;
using UnityEngine;

namespace LaneWar.Core
{
    // 씬에 배치된 웨이포인트 Transform들을 관리하며 PathSystem을 제공하고, 에디터에서 경로를 Gizmo로 시각화한다
    public class PathController : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private float gizmoPointRadius = 0.3f;

        private PathSystem _pathSystem;

        public PathSystem PathSystem
        {
            get
            {
                if (_pathSystem == null)
                {
                    _pathSystem = BuildPathSystem();
                }
                return _pathSystem;
            }
        }

        private void Awake()
        {
            _pathSystem = BuildPathSystem();
        }

        private PathSystem BuildPathSystem()
        {
            var positions = new List<Vector3>(waypoints.Length);
            foreach (var waypoint in waypoints)
            {
                positions.Add(waypoint.position);
            }
            return new PathSystem(positions);
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length < 2)
            {
                return;
            }

            Gizmos.color = gizmoColor;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Transform current = waypoints[i];
                Transform next = waypoints[(i + 1) % waypoints.Length];
                if (current == null || next == null)
                {
                    continue;
                }

                Gizmos.DrawLine(current.position, next.position);
                Gizmos.DrawSphere(current.position, gizmoPointRadius);
            }
        }
    }
}
