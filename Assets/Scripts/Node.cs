using UnityEngine;

namespace Portfolio.Astar.Core
{
    /// <summary>
    /// Represents a single node (cell) in the grid for pathfinding.
    /// Holds information about position, costs, obstacles, and references.
    /// </summary>
    public class Node
    {
        public bool IsEmpty { get; }
        public Vector2Int GridPosition { get; }
        public Vector2 WorldPosition { get; }
        public bool Obstacle { get; }
        public NodeVisual NodeVisual { get; }
        private Candy _candy;
        public Candy Candy
        {
            get => _candy;
            set { _candy = value; }
        }
        
        public float GCost;
        public float HCost;
        public float FCost => GCost + HCost;
        public Node parent;
        
        public Node(bool isEmpty, Vector2Int gridPosition, Vector2 worldPosition, NodeVisual nodeVisual, bool obstacle)
        {
            this.IsEmpty = isEmpty;
            this.GridPosition = gridPosition;
            this.WorldPosition = worldPosition;

            this.GCost = float.MaxValue;
            this.HCost = 0;
            this.parent = null;
            
            this.NodeVisual = nodeVisual;
            NodeVisual.Node = this;
            
            this.Obstacle = obstacle;
            
        }
        
    }    
}

