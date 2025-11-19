using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GridManager : SingletonMonoBehaviour<GridManager>
    {
        #region Private Variables

        private Dictionary<Vector3, bool> _busyCells = new Dictionary<Vector3, bool>();

        [Tooltip("Number of cells in width")]
        [SerializeField] private int _gridWidth = 10;

        [Tooltip("Number of cells in height")]
        [SerializeField] private int _gridHeight = 5;

        [Tooltip("Size of each cell")]
        [SerializeField] private float _cellSize = 1f;

        [Tooltip("Line thickness")]
        [SerializeField] private float _lineWidth = 0.05f;

        [Tooltip("Layer for the LineRenderer lines")]
        [SerializeField] private int _lineOrderLayer = 1;

        private GameObject _gridContainer;

        #endregion

        #region Properties

        public float CellSize => _cellSize;

        #endregion

        #region Monobehaviour Functions

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            EventManager.Instance.Subscribe(EventName.CardDrag, OnCardDrag);
            EventManager.Instance.Subscribe(EventName.CardEndDrag, OnCardEndDrag);
            DrawGrid();
            _gridContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.Instance.Unsubscribe(EventName.CardDrag, OnCardDrag);
            EventManager.Instance.Unsubscribe(EventName.CardEndDrag, OnCardEndDrag);
        }

        #endregion

        #region Private Functions

        private void OnCardDrag(params object[] parameters)
        {
            _gridContainer.SetActive(true);
        }

        private void OnCardEndDrag(params object[] parameters)
        {
            _gridContainer.SetActive(false);
        }

        /// <summary>
        /// Draws the grid using LineRenderers.
        /// </summary>
        private void DrawGrid()
        {
            _gridContainer = new GameObject("GridLines") { transform = { position = transform.position } };

            for (int x = 0; x <= _gridWidth; x++)
                CreateLine(GetWorldPosition(x, 0), GetWorldPosition(x, _gridHeight));

            for (int y = 0; y <= _gridHeight; y++)
                CreateLine(GetWorldPosition(0, y), GetWorldPosition(_gridWidth, y));
        }

        /// <summary>
        /// Creates a line between two points using LineRenderer.
        /// </summary>
        private void CreateLine(Vector3 start, Vector3 end)
        {
            GameObject line = new GameObject("GridLine");
            line.transform.parent = _gridContainer.transform;

            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.startWidth = lr.endWidth = _lineWidth;
            lr.positionCount = 2;
            lr.SetPositions(new Vector3[] { start, end });
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = Color.white;
            lr.sortingOrder = _lineOrderLayer;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the nearest cell center to a given position.
        /// </summary>
        public Vector3 GetNearestCell(Vector3 position)
        {
            int cellX = Mathf.FloorToInt((position.x - (transform.position.x - (_gridWidth * _cellSize) / 2f)) / _cellSize);
            int cellY = Mathf.FloorToInt((position.y - transform.position.y) / _cellSize);
            return GetCellCenter(cellX, cellY);
        }

        /// <summary>
        /// Calculates the world position for a specific grid cell.
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            float pivotX = transform.position.x - (_gridWidth * _cellSize) / 2f;
            float pivotY = transform.position.y;
            return new Vector3(pivotX + (x * _cellSize), pivotY + (y * _cellSize), 0);
        }

        /// <summary>
        /// Returns the center position of a cell in the grid.
        /// </summary>
        public Vector3 GetCellCenter(int x, int y)
        {
            return GetWorldPosition(x, y) + new Vector3(_cellSize / 2f, _cellSize / 2f, 0);
        }

        /// <summary>
        /// Returns a random point index within the grid width.
        /// </summary>
        public int GetRandomPointIndex()
        {
            return Random.Range(0, _gridWidth);
        }

        /// <summary>
        /// Determines which grid cell is under the mouse cursor.
        /// </summary>
        public Vector3 GetGridCellUnderMouse()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;

            int cellX = Mathf.FloorToInt((mouseWorldPosition.x - (transform.position.x - (_cellSize * _gridWidth) / 2f)) / _cellSize);
            int cellY = Mathf.FloorToInt((mouseWorldPosition.y - transform.position.y) / _cellSize);

            if (cellX < 0 || cellX >= _gridWidth || cellY < 0 || cellY >= _gridHeight)
                return Vector3.zero;

            return GetCellCenter(cellX, cellY);
        }

        /// <summary>
        /// Checks if a grid cell is occupied.
        /// </summary>
        public bool CheckCellBusy(Vector3 cellPosition)
        {
            return _busyCells.ContainsKey(cellPosition) && _busyCells[cellPosition];
        }

        /// <summary>
        /// Marks a grid cell as occupied.
        /// </summary>
        public void SetCellBusy(Vector3 cellPosition, bool occupied)
        {
            _busyCells[cellPosition] = occupied;
        }

        #endregion

        #region Gizmos Methods

#if UNITY_EDITOR
        /// <summary>
        /// Draws the grid in editor mode.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            DrawGridGizmos();
        }
        private void DrawGridGizmos()
        {
            Gizmos.color = Color.white;
            for (int x = 0; x <= _gridWidth; x++)
                Gizmos.DrawLine(GetWorldPosition(x, 0), GetWorldPosition(x, _gridHeight));
            for (int y = 0; y <= _gridHeight; y++)
                Gizmos.DrawLine(GetWorldPosition(0, y), GetWorldPosition(_gridWidth, y));

            Gizmos.color = Color.red;
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    Gizmos.DrawWireSphere(GetCellCenter(x, y), _cellSize * 0.2f);
                }
            }
        }
#endif
        #endregion
    }
}