using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;

namespace Portfolio.Astar.Core
{
    /// <summary>
    /// Controls the candy object movement and its current grid node.
    /// </summary>
    public class Candy : MonoBehaviour
    {
        public Node Node { get; private set; }
        public void SetNode(Node node) => Node = node;
        public bool CanMove { get; private set; }
        private void Start() => CanMove = true;

        public event System.Action OnMoveCompleted;
        
        /// <summary>
        /// Moves the candy along the given path and updates its node at the end.
        /// </summary>
        /// <param name="path">List of nodes to follow.</param>
        public async Task Move(List<Node> path)
        {
            CanMove = false;
            List<Vector3> pathPoses = new();
            path.RemoveAt(0);
            foreach (var _p in path) {
                pathPoses.Add( Grid.Instance._Grid[_p.GridPosition.x
                    , _p.GridPosition.y].WorldPosition);
            }
            
            await transform.DOPath(pathPoses.ToArray(), path.Count / 5f).SetEase(Ease.Linear).AsyncWaitForCompletion();

            var newNode = Grid.Instance._Grid[path.LastOrDefault().GridPosition.x, path.LastOrDefault().GridPosition.y];
            Node.Candy = null;
            Node = newNode;
            newNode.Candy = this;

            CanMove = true;
            OnMoveCompleted?.Invoke();
        }
    }    
}


