using UnityEngine;

namespace Portfolio.Astar.Helpers
{
    public class Helper : MonoBehaviour
    {
        public static Vector2Int Vector3ToVector2Int(Vector3 pos) => new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        public static Vector3 Vector2IntToVector3(Vector2Int pos) => new Vector3(pos.x, pos.y);
    }
}

