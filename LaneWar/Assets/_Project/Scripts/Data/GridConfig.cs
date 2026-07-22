using UnityEngine;

namespace LaneWar.Data
{
    // 배치 그리드의 행/열 수를 정의하는 ScriptableObject.
    // 칸 크기와 위치(원점)는 여기서 지정하지 않는다 — GridManager가 PathSystem+EnemyFlowConfig로부터 계산한
    // 배치 가능 영역(LaneLayout.PlacementBounds)에 행/열 수만큼 정확히 맞춰 자동으로 계산한다.
    // 그래야 통로 폭이나 경로 좌표가 바뀌어도 그리드가 항상 배치 가능 영역에 딱 맞게 따라온다
    [CreateAssetMenu(fileName = "GridConfig", menuName = "LaneWar/Grid Config")]
    public class GridConfig : ScriptableObject
    {
        [Header("격자 칸 수 (칸 크기는 배치 가능 영역 크기에서 자동 역산된다)")]
        [SerializeField] private int rows = 12;
        [SerializeField] private int columns = 12;

        [Header("배치 평면의 월드 Y좌표 (MapFloor 표면 높이와 일치해야 한다)")]
        [SerializeField] private float floorHeight = 0f;

        public int Rows => rows;
        public int Columns => columns;
        public float FloorHeight => floorHeight;
    }
}
