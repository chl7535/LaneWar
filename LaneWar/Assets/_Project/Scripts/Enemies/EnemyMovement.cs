using UnityEngine;
using LaneWar.Core;

namespace LaneWar.Enemies
{
    // PathController가 제공하는 경로를 따라 이동하고, 마지막 지점에서 처음으로 순환하는 적 이동 뷰 컴포넌트
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private PathController pathController;
        [SerializeField] private float moveSpeed = 3f;

        private EnemyPathMover _mover;

        private void Start()
        {
            if (pathController != null)
            {
                InitializeMover();
            }
        }

        // 스포너 등 외부에서 생성 직후 경로를 주입할 때 사용한다
        public void Initialize(PathController controller)
        {
            pathController = controller;
            InitializeMover();
        }

        private void InitializeMover()
        {
            _mover = new EnemyPathMover(pathController.PathSystem);
            transform.position = _mover.CurrentPosition;
        }

        private void Update()
        {
            if (_mover == null)
            {
                return;
            }

            transform.position = _mover.Advance(moveSpeed, Time.deltaTime);
        }
    }
}
