using UnityEngine;
using System.Collections;

public class MonsterCollisionHandler : MonoBehaviour
{
    public float ForceProjection;
    public float Duree;
    public Rigidbody rb;
    private bool isProjete = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProjete) return; //�vite double d�clenchement
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
                    Debug.Log("Le monstre a gagn�");
                }
            }
        }
    }

    private IEnumerator RepousseMonster()
    {
        isProjete = true;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * ForceProjection, ForceMode.Impulse);
        Debug.Log("Monstre repousse");
        yield return new WaitForSeconds(Duree);
        yield return new WaitUntil(() => Mathf.Abs(rb.linearVelocity.y) < 0.1f && IsGrounded());
        isProjete = false;
        Debug.Log("Monstre repart");
    }

    private bool IsGrounded()
    {
        //raycast pour savoir si monstre au sol
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
