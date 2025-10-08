using UnityEngine;

[RequireComponent(typeof(Collider))] // Le monstre doit avoir un collider (NON trigger)
public class MonsterAggro : MonoBehaviour
{
    [Header("R�f�rences")]
    public Unit unit;                 // Ton script de d�placement
    public Transform player;          // Capsule du joueur
    public Transform randomTarget;    // Cible al�atoire (ex: GO avec TargetWander)
    public Transform playerLight;     // Spot Light du joueur (Transform)

    [Header("D�tection par lampe (c�ne + LOS)")]
    public bool useSpotLightValues = true; // true -> Light.spotAngle & Light.range
    public float coneAngle = 45f;          // utilis� si useSpotLightValues = false
    public float coneRange = 12f;          // utilis� si useSpotLightValues = false
    public LayerMask obstacleMask;         // murs/obstacles qui bloquent la vue

    [Header("Persistance d�aggro")]
    public float aggroPersistSeconds = 5f;

    [Header("Debug")]
    public bool debugLogs = true;

    // --- �tat interne ---
    Transform _currentTarget;
    bool _sphereContact = false;   // -> mis � jour par OnTriggerEnter/Stay/Exit (c�t� PlayerAggroZone)
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

        bool lightContact = IsInPlayerLightConeWithLOS(); // d�tection via lampe du joueur
        bool shouldAggro = _sphereContact || lightContact;

        if (shouldAggro)
        {
            if (_currentTarget != player)
            {
                if (debugLogs) Debug.Log($"[MonsterAggro] {name}: AGGRO -> Player (sphere={_sphereContact}, light={lightContact})");
                SetTarget(player, true);
            }
            _hasAggro = true;
            _lostTimer = 0f; // reset du timer tant qu'on a Enter/Stay ou lumi�re active
        }
        else
        {
            if (_hasAggro)
            {
                _lostTimer += Time.deltaTime;
                if (_lostTimer >= aggroPersistSeconds)
                {
                    if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Perte d'aggro apr�s {aggroPersistSeconds:F1}s -> RandomTarget");
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

    // ===================== APPEL�ES par le trigger du joueur =====================

    // OnTriggerEnter -> aggro imm�diate
    public void OnPlayerTriggerEnter()
    {
        _sphereContact = true;
        if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Trigger ENTER re�u -> Aggro Player");
    }

    // OnTriggerStay -> maintenir la poursuite (�vite le timer)
    public void OnPlayerTriggerStay()
    {
        if (!_sphereContact && debugLogs) Debug.Log($"[MonsterAggro] {name}: Trigger STAY re�u (transition false->true)");
        _sphereContact = true;
    }

    // OnTriggerExit -> lance le d�lai de 5s (g�r� dans Update)
    public void OnPlayerTriggerExit()
    {
        _sphereContact = false;
        if (debugLogs) Debug.Log($"[MonsterAggro] {name}: Trigger EXIT re�u -> D�marre le timer de perte ({aggroPersistSeconds}s)");
    }

    // =========================== D�tection par LAMPE =============================

    bool IsInPlayerLightConeWithLOS()
    {
        if (!playerLight) return false;

        // Lampe OFF => pas de d�tection
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
