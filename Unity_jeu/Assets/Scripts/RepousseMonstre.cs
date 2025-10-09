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
                    GameChrono.Instance.GameOver();
                }
            }
        }
    }

    private IEnumerator RepousseMonster()
    {
        isProjete = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * ForceProjection, ForceMode.Impulse);
        Debug.Log("Monstre repouss�");
        yield return new WaitForSeconds(Duree);
        Debug.Log("Monstre retombe");
        yield return new WaitUntil(() => Mathf.Abs(rb.linearVelocity.y) < 0.1f);
        rb.isKinematic = true;
        Debug.Log("Monstre repart");
        isProjete = false;
    }
}
