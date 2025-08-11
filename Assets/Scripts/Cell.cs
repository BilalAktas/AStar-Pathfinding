using UnityEngine;

namespace Portfolio.Astar.Core
{
    /// <summary>
    /// Visual representation of a single node in the grid.
    /// Handles color changes based on pathfinding state.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        public Node Node;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        /// <summary>
        /// Marks the cell as part of the path by changing its color to green.
        /// </summary>
        public void InPath()
        {
            if(!Node.Obstacle) _spriteRenderer.color = Color.green;
        }
        
        /// <summary>
        /// Marks the cell as not in the path by changing its color to red.
        /// </summary>
        public void OutPath()
        {
            if(!Node.Obstacle) _spriteRenderer.color = Color.red;
        }
        
        /// <summary>
        /// Sets the cell's color based on whether it's an obstacle or a normal cell.
        /// </summary>
        public void SetDefault()
        {
            _spriteRenderer.color = Node.Obstacle ? Color.black : Color.white;
        }
        
        /// <summary>
        /// Initializes the cell's color if it's an obstacle (sets it to black).
        /// </summary>
        public void Init()
        {
            if (Node.Obstacle)
                _spriteRenderer.color = Color.black;
        }
    }
}

