using UnityEngine;
using System.Linq;

public class TargetWander : MonoBehaviour
{
    [Header("Références")]
    public Grid grid;              // ton composant Grid (le même que pour le pathfinding)
    public Transform seeker;       // (optionnel) pour déclencher un changement à l'arrivée

    [Header("Paramètres de déplacement")]
    public float changeEverySeconds = 4f;   // change la position à intervalle régulier
    public float arriveDistance = 1.5f;     // ou quand le seeker est proche
    public float minMoveDistance = 4f;      // éviter de re-piocher quasi la même case
    public float yOffset = 0.5f;            // hauteur du target

    [Header("Qualité des points")]
    public int minFreeNeighbours = 2;       // évite les cul-de-sac trop serrés
    public int maxTries = 60;               // essais max pour trouver une bonne case
    public int penaltyMax = 999999;         // tolérance au "penalty" (plus petit = évite bords/murs)

    float _timer;

    void Reset()
    {
        // petit confort si on pose le script sur le Target directement
        grid = Object.FindFirstObjectByType<Grid>();
    }

    void OnEnable()
    {
        _timer = changeEverySeconds;
        MoveTargetToRandomWalkable(forceFar: true);
    }

    void Update()
    {
        if (grid == null) return;

        _timer -= Time.deltaTime;

        // 1) changement périodique
        if (_timer <= 0f)
        {
            MoveTargetToRandomWalkable();
            _timer = changeEverySeconds;
        }

        // 2) ou quand le seeker arrive au point
        if (seeker != null)
        {
            if ((seeker.position - transform.position).sqrMagnitude <= arriveDistance * arriveDistance)
            {
                MoveTargetToRandomWalkable();
                _timer = changeEverySeconds; // on redémarre le timer
            }
        }
    }

    void MoveTargetToRandomWalkable(bool forceFar = false)
    {
        Vector2 size = grid.gridWorldSize;
        Vector3 center = grid.transform.position;

        Vector3 startPos = transform.position;
        for (int i = 0; i < maxTries; i++)
        {
            float rx = Random.Range(-size.x * 0.5f, size.x * 0.5f);
            float rz = Random.Range(-size.y * 0.5f, size.y * 0.5f);
            Vector3 candidate = new Vector3(center.x + rx, startPos.y, center.z + rz);

            Node n = grid.NodeFromWorldPoint(candidate);
            if (!n.walkable) continue;
            if (n.movementPenalty > penaltyMax) continue; // (facultatif) éviter les bords si tu mets des gros penalties

            // éviter de rester collé / cul-de-sac : au moins X voisins walkable
            int free = grid.GetNeighbours(n).Count(nei => nei.walkable);
            if (free < minFreeNeighbours) continue;

            // garder une distance minimale pour varier la balade
            if (forceFar || minMoveDistance > 0f)
            {
                float dSqr = (n.worldPosition - startPos).sqrMagnitude;
                if (dSqr < minMoveDistance * minMoveDistance) continue;
            }

            transform.position = n.worldPosition + Vector3.up * yOffset;
            return;
        }

        // fallback : poser quand même sur la meilleure estimation locale
        Node fallback = grid.NodeFromWorldPoint(startPos);
        transform.position = fallback.worldPosition + Vector3.up * yOffset;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, arriveDistance);
    }
}

