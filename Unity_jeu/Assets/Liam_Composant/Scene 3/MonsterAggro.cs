using UnityEngine;

[RequireComponent(typeof(Collider))] // Le monstre doit avoir un collider (NON trigger)
public class MonsterAggro : MonoBehaviour
{
    [Header("Références")]
    public Unit unit;                 // Ton script de déplacement
    public Transform player;          // Capsule du joueur
    public Transform randomTarget;    // Cible aléatoire (ex: GO avec TargetWander)
    public Transform playerLight;     // Spot Light du joueur (Transform)

    [Header("Détection par lampe (cône + LOS)")]
    public bool useSpotLightValues = true; // true -> Light.spotAngle & Light.range
    public float coneAngle = 45f;          // utilisé si useSpotLightValues = false
    public float coneRange = 12f;          // utilisé si useSpotLightValues = false
    public LayerMask obstacleMask;         // murs/obstacles qui bloquent la vue

    [Header("Persistance d’aggro")]
    public float aggroPersistSeconds = 5f;

    [Header("Debug")]
    public bool debugLogs = true;

    // --- État interne ---
    Transform _currentTarget;
    bool _sphereContact = false;   // -> mis à jour par OnTriggerEnter/Stay/Exit (côté PlayerAggroZone)
    float _lostTimer = 0f;
    bool _hasAggro = false;
    Light _playerSpot;

    void Awake()
    {
        if (!unit) unit = GetComponent<Unit>();
        if (playerLight) _playerSpot = playerLight.GetComponent<Light>();

        var col = GetComponent<Collider>();
        if (col && col.isTrigger)
            Debug.LogWarning($"[MonsterAggro] {name}: Le collider du monstre est en Trigger. Mets-le en NON-Trigger pour recevoir les OnTrigger* de la zone du joueur.");
    }

    void OnEnable()
    {
        SetTarget(randomTarget, true);
        _sphereContact = false;
        _hasAggro = false;
        _lostTimer = 0f;

        if (debugLogs) Debug.Log($"[MonsterAggro] {name}: INIT -> target=randomTarget={randomTarget?.name}");
    }

    void Update()
    {
        if (!unit || !player || !randomTarget) return;

        bool lightContact = IsInPlayerLightConeWithLOS(); // détection via lampe du joueur
        bool shouldAggro = _sphereContact || lightContact;

        if (shouldAggro)
        {
            if (_currentTarget != player)
            {
                if (debugLogs) Debug.Log($"[MonsterAggro] {name}: AGGRO -> Player (sphere={_sphereContact}, light={lightContact})");
                SetTarget(player, true);
            }
            _hasAggro = true;
            _lostTimer = 0f; // reset du timer tant qu'on a Enter/Stay ou lumière active
        }
        else
        {
            if (_hasAggro)
            {
                _lostTimer += Time.deltaTime;
                if (_lostTimer >= aggroPersistSeconds)
                {
                    if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Perte d'aggro après {aggroPersistSeconds:F1}s -> RandomTarget");
                    _hasAggro = false;
                    _lostTimer = 0f;
                    if (_currentTarget != randomTarget)
                        SetTarget(randomTarget, true);
                }
                else
                {
                    if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Timer perte en cours ({_lostTimer:F2}/{aggroPersistSeconds:F2}s)...");
                }
            }
            else
            {
                if (_currentTarget != randomTarget)
                    SetTarget(randomTarget, false);
            }
        }
    }

    // ===================== APPELÉES par le trigger du joueur =====================

    // OnTriggerEnter -> aggro immédiate
    public void OnPlayerTriggerEnter()
    {
        _sphereContact = true;
        if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Trigger ENTER reçu -> Aggro Player");
    }

    // OnTriggerStay -> maintenir la poursuite (évite le timer)
    public void OnPlayerTriggerStay()
    {
        if (!_sphereContact && debugLogs) Debug.Log($"[MonsterAggro] {name}: Trigger STAY reçu (transition false->true)");
        _sphereContact = true;
    }

    // OnTriggerExit -> lance le délai de 5s (géré dans Update)
    public void OnPlayerTriggerExit()
    {
        _sphereContact = false;
        if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Trigger EXIT reçu -> Démarre le timer de perte ({aggroPersistSeconds}s)");
    }

    // =========================== Détection par LAMPE =============================

    bool IsInPlayerLightConeWithLOS()
    {
        if (!playerLight) return false;

        // Lampe OFF => pas de détection
        if (!playerLight.gameObject.activeInHierarchy) return false;

        float angleTotal = coneAngle;
        float range = coneRange;

        if (_playerSpot)
        {
            if (!_playerSpot.enabled) return false;
            if (_playerSpot.intensity <= 0f) return false;
            if (_playerSpot.range <= 0f) return false;

            if (useSpotLightValues)
            {
                angleTotal = _playerSpot.spotAngle;
                range = _playerSpot.range;
            }
        }

        Vector3 origin = playerLight.position;
        Vector3 toMonster = transform.position - origin;
        float dist = toMonster.magnitude;

        if (dist > range) return false;

        float angle = Vector3.Angle(playerLight.forward, toMonster);
        if (angle > (angleTotal * 0.5f)) return false;

        // Line-of-sight : s'il y a un mur on annule
        if (Physics.Raycast(origin, toMonster.normalized, out RaycastHit hit, dist, obstacleMask))
            return false;

        return true;
    }

    // ============================== Cible / Path =================================

    void SetTarget(Transform t, bool forceRepath)
    {
        _currentTarget = t;
        unit.target = t;
        if (forceRepath) unit.ForceRepath();

        if (debugLogs) Debug.Log($"[MonsterAggro] {name}: SetTarget -> {t?.name} (forceRepath={forceRepath})");
    }

    // =============================== Gizmos ======================================

    void OnDrawGizmosSelected()
    {
        if (!playerLight) return;

        float angleTotal = (_playerSpot && useSpotLightValues) ? _playerSpot.spotAngle : coneAngle;
        float range = (_playerSpot && useSpotLightValues) ? _playerSpot.range : coneRange;

        Gizmos.color = Color.cyan;
        Vector3 fwd = playerLight.forward;
        Vector3 pos = playerLight.position;
        Vector3 left = Quaternion.AngleAxis(-angleTotal * 0.5f, Vector3.up) * fwd;
        Vector3 right = Quaternion.AngleAxis(+angleTotal * 0.5f, Vector3.up) * fwd;
        Gizmos.DrawRay(pos, fwd.normalized * range);
        Gizmos.DrawRay(pos, left.normalized * range);
        Gizmos.DrawRay(pos, right.normalized * range);
    }
}
