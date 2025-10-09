using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MonsterCollision : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Le collider doit être NON-trigger pour détecter les collisions physiques")]
    public bool debugLogs = true;

    private bool hasTriggeredDefeat = false;

    void OnCollisionEnter(Collision collision)
    {
        // Éviter de déclencher plusieurs fois
        if (hasTriggeredDefeat) return;

        // Vérifier si c'est le joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            if (debugLogs)
                Debug.Log($"[MonsterCollision] Le monstre a touché le joueur ! GAME OVER");

            hasTriggeredDefeat = true;

            // Déclencher la défaite
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerDefeat();
            }
        }
    }

    void OnValidate()
    {
        // Vérifier que le collider n'est PAS en trigger
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Debug.LogWarning($"[MonsterCollision] ATTENTION ! Le collider de {gameObject.name} est en TRIGGER. Mets-le en NON-TRIGGER pour détecter les collisions avec le joueur !");
        }
    }
}