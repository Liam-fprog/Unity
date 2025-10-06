using UnityEngine;

public enum CollectibleType
{
    Key,
    PowerItem
}

public class CollectibleItem : MonoBehaviour
{
    public CollectibleType itemType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddItem(itemType);
                Debug.Log("Objet collecté par : " + other.name);
                Destroy(gameObject);
            }
        }
    }
}
