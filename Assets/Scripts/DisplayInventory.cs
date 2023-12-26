using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    public GameObject inventoryPrefab;
    public InventoryObject inventory;
    public int X_Start;
    public int Y_Start;
    public int X_Space_Between_Items;
    public int Number_Of_Coloumn;
    public int Y_Space_Between_Items;

    Dictionary<InventorySlots, GameObject> itemDisplayed = new Dictionary<InventorySlots, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlots slot = inventory.Container.Items[i];

            if (itemDisplayed.ContainsKey(inventory.Container.Items[i]))
            {
                itemDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                TextMeshProUGUI textMeshPro = obj.GetComponentInChildren<TextMeshProUGUI>();
                if (textMeshPro != null && inventory != null && inventory.Container != null && i < inventory.Container.Items.Count && slot != null)
                {
                    textMeshPro.text = slot.amount.ToString("n0");
                    itemDisplayed.Add(slot, obj);
                }
                else
                {
                    // Handle the case where something is null
                    Debug.LogError("One of the objects or components is null.");
                }
            }
        }
    }

    public void CreateDisplay()
    {
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlots slot = inventory.Container.Items[i];

            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            TextMeshProUGUI textMeshPro = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshPro != null && inventory != null && inventory.Container != null && i < inventory.Container.Items.Count && slot != null)
            {
                textMeshPro.text = slot.amount.ToString("n0");
                itemDisplayed.Add(slot, obj);
            }
            else
            {
                // Handle the case where something is null
                Debug.LogError("One of the objects or components is null.");
            }
        }
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_Start + (X_Space_Between_Items * (i % Number_Of_Coloumn)), Y_Start + (-Y_Space_Between_Items * (i / Number_Of_Coloumn)), 0f);
    }
}
