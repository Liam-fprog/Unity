using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public int keyCount = 0;
    public bool hasPowerItem = false;
    public TMP_Text keyText;
    public Image powerIcon;

    private void Start()
    {
        UpdateUI();
    }

    public void AddItem(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Key:
                keyCount++;
                break;
            case CollectibleType.PowerItem:
                hasPowerItem = true;
                break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (keyText != null)
            keyText.text = "Keys: " + keyCount;

        if (powerIcon != null)
            powerIcon.enabled = hasPowerItem;
    }
}
