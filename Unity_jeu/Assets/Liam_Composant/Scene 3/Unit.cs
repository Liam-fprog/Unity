using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = .2f;
    const float pathUpadateMoveThreshold = .5f;

    [Header("Cible / Vitesse")]
    public Transform target;
    public float speed = 20;
    public float TurnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    [Header("Height lock")]
    [Tooltip("Verrouille la hauteur du monstre à lockedY")]
    public bool lockY = true;
    [Tooltip("Hauteur fixe du monstre")]
    public float lockedY = 1f;

    Path path;

    void Start()
    {
        // Place immédiatement le monstre à la hauteur fixée (si demandé)
        if (lockY)
        {
            var p = transform.position;
            transform.position = new Vector3(p.x, lockedY, p.z);
        }

        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
            yield return new WaitForSeconds(.3f);

        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        float sqrMoveThreshold = pathUpadateMoveThreshold * pathUpadateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);

            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    public void ForceRepath()
    {
        PathRequestManager.RequestPath(
            new PathRequest(transform.position, target.position, OnPathFound)
        );
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        // --- REGARD A PLAT (ignore Y) ---
        if (path != null && path.lookPoints.Length > 0)
        {
            Vector3 firstFlat = new Vector3(path.lookPoints[0].x, transform.position.y, path.lookPoints[0].z);
            transform.LookAt(firstFlat);
        }

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (speedPercent < 0.01f)
                        followingPath = false;
                }

                // --- ROTATION A PLAT (on garde notre Y actuel) ---
                Vector3 nextLook = new Vector3(path.lookPoints[pathIndex].x, transform.position.y, path.lookPoints[pathIndex].z);
                Quaternion targetRotation = Quaternion.LookRotation(nextLook - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);

                // --- AVANCE A PLAT ---
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);

                // --- VERROU Y ---
                if (lockY)
                {
                    var p = transform.position;
                    transform.position = new Vector3(p.x, lockedY, p.z);
                }
            }

            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
            path.DrawWithGizmos();
    }
}
