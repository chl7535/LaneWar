using UnityEngine;

namespace LaneWar.Enemies
{
    // 적 모델 위쪽에 HP 게이지를 빌보드(항상 카메라를 향함)로 표시하는 뷰.
    // Enemy의 CurrentHealth/MaxHealth를 매 프레임 읽어 채움 비율과 색(초록→노랑→빨강)을 즉시 갱신한다
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("크기/위치")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.4f, 0f);
        [SerializeField] private float barWidth = 1.4f;
        [SerializeField] private float barHeight = 0.22f;
        [SerializeField] private float borderThickness = 0.04f;

        [Header("색상")]
        [SerializeField] private Color borderColor = Color.black;
        [SerializeField] private Color backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.95f);
        [SerializeField] private Color highHealthColor = new Color(0.2f, 0.9f, 0.25f, 1f);
        [SerializeField] private Color midHealthColor = new Color(0.95f, 0.85f, 0.15f, 1f);
        [SerializeField] private Color lowHealthColor = new Color(0.9f, 0.15f, 0.15f, 1f);

        private static Mesh _centeredQuadMesh;
        private static Mesh _leftAnchoredQuadMesh;

        private Enemy _enemy;
        private Transform _barRoot;
        private Transform _fill;
        private MeshRenderer _fillRenderer;
        private Camera _mainCamera;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            BuildVisual();
        }

        private void LateUpdate()
        {
            if (_enemy == null)
            {
                return;
            }

            _barRoot.position = transform.position + offset;

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            if (_mainCamera != null)
            {
                _barRoot.rotation = _mainCamera.transform.rotation;
            }

            float ratio = _enemy.MaxHealth > 0f ? Mathf.Clamp01(_enemy.CurrentHealth / _enemy.MaxHealth) : 0f;
            _fill.localScale = new Vector3(barWidth * ratio, barHeight, 1f);
            _fillRenderer.sharedMaterial.color = EvaluateHealthColor(ratio);
        }

        private void OnDestroy()
        {
            if (_barRoot != null)
            {
                Destroy(_barRoot.gameObject);
            }
        }

        private Color EvaluateHealthColor(float ratio)
        {
            return ratio >= 0.5f
                ? Color.Lerp(midHealthColor, highHealthColor, (ratio - 0.5f) * 2f)
                : Color.Lerp(lowHealthColor, midHealthColor, ratio * 2f);
        }

        private void BuildVisual()
        {
            var root = new GameObject("HealthBar");
            _barRoot = root.transform;

            float borderWidth = barWidth + borderThickness * 2f;
            float borderHeight = barHeight + borderThickness * 2f;

            var border = new GameObject("Border");
            border.transform.SetParent(_barRoot, false);
            border.transform.localPosition = new Vector3(0f, 0f, 0.002f);
            border.transform.localScale = new Vector3(borderWidth, borderHeight, 1f);
            AddQuadRenderer(border, GetCenteredQuadMesh(), borderColor, 0);

            var background = new GameObject("Background");
            background.transform.SetParent(_barRoot, false);
            background.transform.localPosition = new Vector3(0f, 0f, 0.001f);
            background.transform.localScale = new Vector3(barWidth, barHeight, 1f);
            AddQuadRenderer(background, GetCenteredQuadMesh(), backgroundColor, 1);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(_barRoot, false);
            fill.transform.localPosition = new Vector3(-barWidth * 0.5f, 0f, 0f);
            fill.transform.localScale = new Vector3(barWidth, barHeight, 1f);
            _fill = fill.transform;
            _fillRenderer = AddQuadRenderer(fill, GetLeftAnchoredQuadMesh(), highHealthColor, 2);
        }

        private static MeshRenderer AddQuadRenderer(GameObject target, Mesh mesh, Color color, int sortingOrder)
        {
            MeshFilter filter = target.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer renderer = target.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = new Material(Shader.Find("Sprites/Default")) { color = color };
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static Mesh GetCenteredQuadMesh()
        {
            if (_centeredQuadMesh == null)
            {
                _centeredQuadMesh = BuildQuad(-0.5f, 0.5f);
                _centeredQuadMesh.name = "HealthBarCenteredQuad";
            }
            return _centeredQuadMesh;
        }

        private static Mesh GetLeftAnchoredQuadMesh()
        {
            if (_leftAnchoredQuadMesh == null)
            {
                _leftAnchoredQuadMesh = BuildQuad(0f, 1f);
                _leftAnchoredQuadMesh.name = "HealthBarFillQuad";
            }
            return _leftAnchoredQuadMesh;
        }

        // xMin~xMax 범위로 폭을 지정한 화면 정렬(로컬 XY 평면) 쿼드. 채움 바는 왼쪽 끝(0)을 고정점으로 스케일한다
        private static Mesh BuildQuad(float xMin, float xMax)
        {
            var mesh = new Mesh();
            mesh.vertices = new[]
            {
                new Vector3(xMin, -0.5f, 0f),
                new Vector3(xMin, 0.5f, 0f),
                new Vector3(xMax, 0.5f, 0f),
                new Vector3(xMax, -0.5f, 0f)
            };
            mesh.uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
