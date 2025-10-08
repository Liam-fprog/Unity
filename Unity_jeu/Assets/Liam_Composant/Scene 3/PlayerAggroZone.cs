using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAggroZone : MonoBehaviour
{
    [Header("Filtrage (optionnel)")]
    [Tooltip("Laissez 0 pour détecter tous les layers")]
    public LayerMask monstersMask;

    [Header("Debug")]
    public bool debugLogs = true;
    [Tooltip("Temps minimal entre deux logs STAY pour le même collider")]
    public float stayLogInterval = 0.5f;

    SphereCollider _sphere;
    Rigidbody _rb;
    readonly Dictionary<Collider, float> _lastStayLogTime = new Dictionary<Collider, float>();

    void Reset()
    {
        _sphere = GetComponent<SphereCollider>();
        _sphere.isTrigger = true;

        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    void Awake()
    {
        _sphere = GetComponent<SphereCollider>();
        _rb = GetComponent<Rigidbody>();

        if (!_sphere.isTrigger)
        {
            _sphere.isTrigger = true;
            if (debugLogs) Debug.LogWarning($"[PlayerAggroZone] {_Pretty(this)}: SphereCollider.isTrigger forcé à TRUE.");
        }
        if (!_rb.isKinematic || _rb.useGravity)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
            if (debugLogs) Debug.LogWarning($"[PlayerAggroZone] {_Pretty(this)}: Rigidbody configuré (isKinematic=TRUE, useGravity=FALSE).");
        }

        if (debugLogs)
        {
            Debug.Log($"[PlayerAggroZone] {_Pretty(this)}: READY " +
                      $"(layer={LayerMask.LayerToName(gameObject.layer)}, radius={_sphere.radius:F2}, center(local)={_sphere.center})");
        }
    }

    Vector3 SphereWorldCenter()
    {
        // centre du SphereCollider en monde
        return transform.TransformPoint(_sphere.center);
    }

    bool PassesMask(GameObject go)
    {
        if (monstersMask.value == 0) return true;
        return (monstersMask.value & (1 << go.layer)) != 0;
    }

    // ------------------------- TRIGGER EVENTS -------------------------

    void OnTriggerEnter(Collider other)
    {
        if (!PassesMask(other.gameObject)) return;

        var monster = other.GetComponentInParent<MonsterAggro>();
        string layerName = LayerMask.LayerToName(other.gameObject.layer);
        Vector3 c = SphereWorldCenter();
        Vector3 p = other.ClosestPoint(c);
        float dist = Vector3.Distance(c, p);

        if (debugLogs)
            Debug.Log($"[PlayerAggroZone] ENTER time={Time.time:F2} " +
                      $"zone={_Pretty(this)} other={_Pretty(other)} layer={layerName} " +
                      $"center={c} closest={p} dist={dist:F2} radius={_sphere.radius:F2} isTrigger={_sphere.isTrigger}");

        if (monster != null)
        {
            monster.OnPlayerTriggerEnter();
        }
        else if (debugLogs)
        {
            Debug.LogWarning($"[PlayerAggroZone] ENTER -> Pas de MonsterAggro trouvé sur/parent de: {_Pretty(other)}");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!PassesMask(other.gameObject)) return;

        float last;
        if (!_lastStayLogTime.TryGetValue(other, out last)) last = -999f;
        if (Time.time - last >= stayLogInterval)
        {
            _lastStayLogTime[other] = Time.time;

            Vector3 c = SphereWorldCenter();
            Vector3 p = other.ClosestPoint(c);
            float dist = Vector3.Distance(c, p);

            if (debugLogs)
                Debug.Log($"[PlayerAggroZone] STAY  time={Time.time:F2} other={_Pretty(other)} dist={dist:F2} <= radius={_sphere.radius:F2}");
        }

        var monster = other.GetComponentInParent<MonsterAggro>();
        if (monster != null)
        {
            monster.OnPlayerTriggerStay();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!PassesMask(other.gameObject)) return;

        var monster = other.GetComponentInParent<MonsterAggro>();
        Vector3 c = SphereWorldCenter();
        Vector3 p = other.ClosestPoint(c);
        float dist = Vector3.Distance(c, p);

        if (debugLogs)
            Debug.Log($"[PlayerAggroZone] EXIT  time={Time.time:F2} other={_Pretty(other)} lastDist={dist:F2}");

        if (monster != null)
        {
            monster.OnPlayerTriggerExit();
        }

        _lastStayLogTime.Remove(other);
    }

    // ------------------------- Helpers -------------------------

    string _Pretty(Object o)
    {
        if (o == null) return "(null)";
        var comp = o as Component;
        if (comp != null) return $"{comp.name}";
        return o.name;
    }
}
