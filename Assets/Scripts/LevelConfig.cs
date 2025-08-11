using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private Vector2Int _gridSize;
    public Vector2Int GridSize => _gridSize;

    [SerializeField] private List<Vector2Int> _obstaclePoses;
    public List<Vector2Int> ObstaclePoses => _obstaclePoses;

    [SerializeField] private Vector2Int _candyPosition;
    public Vector2Int CandyPosition => _candyPosition;
}
