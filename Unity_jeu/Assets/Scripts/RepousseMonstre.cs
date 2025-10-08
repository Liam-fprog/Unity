using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterCollision : MonoBehaviour
{
    public float ForceProjection;
    public float Duree;
    public NavMeshAgent agent;
    public Rigidbody rb;
    private bool isProjete = false;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProjete) return; //évite double déclenchement
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                if (inventory.hasPowerItem)
                {
                    inventory.hasPowerItem = false;
                    StartCoroutine(RepousseMonster());
                }
                else
                {
                    Debug.Log("Le monstre a gagné");
                }
            }
        }
    }

    private IEnumerator RepousseMonster()
    {
        isProjete = true;
        agent.enabled = false;   //arrêt pathfinding
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * ForceProjection, ForceMode.Impulse);
        Debug.Log("Monstre repoussé");
        yield return new WaitForSeconds(Duree);
        Debug.Log("Monstre retombe");
        yield return new WaitUntil(() => Mathf.Abs(rb.linearVelocity.y) < 0.1f);
        rb.isKinematic = true;
        agent.enabled = true;
        Debug.Log("Monstre repart");
        isProjete = false;
    }
}
