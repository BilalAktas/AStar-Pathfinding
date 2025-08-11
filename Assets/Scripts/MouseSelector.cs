using System;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio.Astar.Core
{
    /// <summary>
    /// Handles mouse input to select grid cells and move the candy.
    /// </summary>
    public class MouseSelector : MonoBehaviour
    {
        private Camera _cam;
        private void Start() =>  _cam = Camera.main;
    
        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
                HandleTouchMove(Input.mousePosition);
        }
        
        /// <summary>
        /// Processes mouse position, finds the clicked cell, calculates path, and moves the candy.
        /// Also updates cell colors to show the path.
        /// </summary>
        /// <param name="mousePosition">The mouse position in screen coordinates.</param>
        private async void HandleTouchMove(Vector3 mousePosition)
        {
            try
            {
                if (Grid.Instance.Candy.CanMove)
                {
                    var _pos = _cam.ScreenToWorldPoint(mousePosition);
                    var _collider = Physics2D.OverlapPoint(_pos);
                    if (_collider && _collider.TryGetComponent(out NodeVisual nodeVisual))
                    {
                        if (!nodeVisual.Node.Obstacle)
                        {
                            var _path = Grid.Instance.GetPath(Grid.Instance.Candy.Node.GridPosition,
                                nodeVisual.Node.GridPosition);
                            var _poses = new HashSet<Vector2Int>();
                            foreach (var _s in _path)
                            {
                                _poses.Add(_s.GridPosition);
                            }
    
                            foreach (var _currentNode in Grid.Instance._Grid)
                            {
                                //Debug.Log(_currentNode.gridPosition);
                                if (_poses.Contains(_currentNode.GridPosition))
                                    _currentNode.NodeVisual.InPath();
                                else
                                    _currentNode.NodeVisual.OutPath();
                            }
    
                            await Grid.Instance.Candy.Move(_path);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
    }

}
