using UnityEngine;
using System.Collections;

public class ElevatorPlatform : MonoBehaviour
{
    public float HauteurFinale;
    public float VitesseMontee;
    public int NbKeysRequired;
    private bool isRising = false;
    private bool hasActivated = false;
    private bool playerOnPlatform = false;
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * HauteurFinale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasActivated) return; // évite double déclenchement
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.keyCount == NbKeysRequired)
            {
                Debug.Log("Ascenseur activé");
                StartCoroutine(RiseElevator(collision.transform));
                hasActivated = true;
            }
            else
            {
                Debug.Log("Pas assez de clés");
            }
        }
    }

    private IEnumerator RiseElevator(Transform player)
    {
        isRising = true;
        float elapsed = 0f;
        float duration = HauteurFinale / VitesseMontee;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, progress);
            float deltaY = newPos.y - transform.position.y;
            transform.position = newPos;
            if (player != null)
            {
                player.position += new Vector3(0, deltaY, 0);
            }
            yield return null;
        }

        Debug.Log("Ascenseur arrivé en haut !");
        isRising = false;
    }
}