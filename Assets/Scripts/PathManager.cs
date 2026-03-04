using UnityEngine;

public class PathManager : MonoBehaviour
{
 
    [SerializeField] public Transform[] waypoints;

    // Рисуем линию в редакторе, чтобы ты видел путь глазами, а не гадал
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }

}