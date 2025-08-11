using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Portfolio.Astar.Core
{
    /// <summary>
    /// Manages the grid system and pathfinding logic for the game.
    /// </summary>
    public class Grid : Singleton<Grid>
    {
        public Node[,] _Grid { get; private set; }
        [SerializeField] private LevelConfig _currentLevelConfig;
        [SerializeField] private Vector2Int _nodeSize;
        [SerializeField] private GameObject _nodePrefab;
        [SerializeField] private GameObject _obstaclePrefab;
        [SerializeField] private GameObject _candyPrefab;
        public Candy Candy { get; private set; }
        
        private void Start()
        {
            CreateGrid();

            Candy.OnMoveCompleted += SetAllCellsDefault;
        }
        
        /// <summary>
        /// Creates the grid of nodes and places obstacles and cells.
        /// </summary>
        private void CreateGrid()
        {
            _Grid = new Node[_currentLevelConfig.GridSize.x, _currentLevelConfig.GridSize.y];
            var origin = Vector2Int.zero;
            var totalSize = new Vector2(_currentLevelConfig.GridSize.x * _nodeSize.x, _currentLevelConfig.GridSize.y * _nodeSize.y);
            var bottomLeft = origin - new Vector2(totalSize.x, totalSize.y) / 2f +
                             new Vector2(_nodeSize.x, _nodeSize.y) / 2f;

            for (var x = 0; x < _currentLevelConfig.GridSize.x; x++)
            {
                for (var y = 0; y < _currentLevelConfig.GridSize.y; y++)
                {
                    var worldPos = bottomLeft + new Vector2(x * _nodeSize.x, y * _nodeSize.y);
                    var obstacle = _currentLevelConfig.ObstaclePoses.Contains(new Vector2Int(x, y));
                    
                    GameObject clone = null;
                    if (!obstacle)
                        clone = Instantiate(_nodePrefab);
                    else
                        clone = Instantiate(_obstaclePrefab);

                    var cell = !obstacle ? clone.GetComponent<Cell>() : null;
                    var node = new Node(true, new Vector2Int(x, y), worldPos, clone.GetComponent<Cell>(), obstacle);
                    
                    
                    clone.transform.position = worldPos;

                    _Grid[x, y] = node;
                }
            }
            
            SpawnCandy();
        }
        
        /// <summary>
        /// Spawns the candy object at its defined position in the grid.
        /// </summary>
        private void SpawnCandy()
        {
            var node = _Grid[_currentLevelConfig.CandyPosition.x, _currentLevelConfig.CandyPosition.y];
            var clone = Instantiate(_candyPrefab).GetComponent<Candy>();
            clone.transform.position = node.WorldPosition;
            node.Candy = clone;
            clone.SetNode(node);

            Candy = clone;
        }
        
        /// <summary>
        /// Checks if a given position is within the grid boundaries.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <returns>True if the position is valid; otherwise, false.</returns>
         private bool IsPositionInBounds(Vector2Int pos)
        {
            var checkX = pos.x >= 0 && pos.x < _currentLevelConfig.GridSize.x;
            var checkY = pos.y >= 0 && pos.y < _currentLevelConfig.GridSize.y;

            return checkX && checkY;
        }

        /// <summary>
        /// Returns a list of valid neighboring nodes for a given position.
        /// </summary>
        /// <param name="pos">The position to find neighbors for.</param>
        /// <returns>List of neighboring nodes.</returns>
        private List<Node> GetNeighbors(Vector2Int pos)
        {
            var _neighbors = new List<Node>();
            foreach (var dir in new Vector2Int[]
                     {
                         Vector2Int.right,
                         Vector2Int.left,
                         Vector2Int.up,
                         Vector2Int.down
                     })
            {
                var cPos = pos + dir;
                if(IsPositionInBounds(cPos))
                    _neighbors.Add(_Grid[cPos.x, cPos.y]);
            }

            return _neighbors;
        }

        /// <summary>
        /// Calculates the Manhattan distance between two grid positions.
        /// </summary>
        /// <param name="currentPos">Start position.</param>
        /// <param name="targetPos">Target position.</param>
        /// <returns>Manhattan distance as an integer.</returns>
        private int ManhattanDistance(Vector2Int currentPos, Vector2Int targetPos) {
            return (Mathf.Abs(currentPos.x - targetPos.x)) + (Mathf.Abs(currentPos.y - targetPos.y));
        }

        /// <summary>
        /// Finds the shortest path between two positions using the A* algorithm.
        /// </summary>
        /// <param name="startPos">Start grid position.</param>
        /// <param name="targetPos">Target grid position.</param>
        /// <returns>List of nodes representing the path.</returns>
        public List<Node> GetPath(Vector2Int startPos, Vector2Int targetPos)
        {
            var openList = new List<Node>();
            var closedList = new HashSet<Node>();
            
            var startNode = new Node(true, startPos, Vector2Int.zero, _Grid[startPos.x, startPos.y].Cell, _Grid[startPos.x, startPos.y].Obstacle);
            startNode.GCost = 0;
            startNode.HCost = ManhattanDistance(startNode.GridPosition, targetPos);
            
            openList.Add(startNode);

            while (openList.Count>0)
            {
                //check currentNode
                var currentNode = openList.FirstOrDefault();
                foreach (var _node in openList)
                {
                    if (_node.FCost < currentNode.FCost ||
                        (_node.FCost == currentNode.FCost &&
                         ManhattanDistance(_node.GridPosition,targetPos) < currentNode.HCost))
                    {
                        currentNode = _node;
                    }
                }
                
                //check final
                if (currentNode.GridPosition == targetPos)
                {
                    var _path = new List<Node>();
                    var _node = currentNode;
        
                    while (_node.parent != null)
                    {
                        _path.Add(_node.parent);
                        _node = _node.parent;
                    }

                    _path.Reverse();
                    _path.Add(currentNode);
                    
                    return _path;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                //check neighbors
                foreach (var _node in GetNeighbors(currentNode.GridPosition))
                {
                    if(closedList.Contains(_node) || _node.Obstacle)
                        continue;

                    var _g = currentNode.GCost + 1;
                    if (_g < _node.GCost)
                    {
                        var node = new Node(true, _node.GridPosition, Vector2Int.one, _node.Cell, _node.Obstacle);
                        node.GCost = _g;
                        node.HCost = ManhattanDistance(node.GridPosition, targetPos);
                        node.parent = currentNode;
                        openList.Add(node);
                    }
                }
            }

            return new List<Node>();
        }

        /// <summary>
        /// When move completed, change cells.
        /// </summary>
        private void SetAllCellsDefault()
        {
            foreach (var _node in Grid.Instance._Grid)
                _node.Cell.SetDefault();
        }
    }  
    
}

