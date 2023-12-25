using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
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
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if(itemDisplayed.ContainsKey(inventory.Container[i]))
            {
                itemDisplayed[inventory.Container[i]].GetComponentInChildren<TextMeshPro>().text = inventory.Container[i].amount.ToString("n0");
            }
            else
            {
                var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                TextMeshPro textMeshPro = obj.GetComponentInChildren<TextMeshPro>();
                if (textMeshPro != null && inventory != null && inventory.Container != null && i < inventory.Container.Count && inventory.Container[i] != null)
                {
                    textMeshPro.text = inventory.Container[i].amount.ToString("n0");
                    itemDisplayed.Add(inventory.Container[i], obj);
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
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            var obj = Instantiate(inventory.Container[i].item.prefab, Vector3.zero, Quaternion.identity, transform);            
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
            TextMeshPro textMeshPro = obj.GetComponentInChildren<TextMeshPro>();
            if (textMeshPro != null && inventory != null && inventory.Container != null && i < inventory.Container.Count && inventory.Container[i] != null)
            {
                textMeshPro.text = inventory.Container[i].amount.ToString("n0");
                itemDisplayed.Add(inventory.Container[i], obj);
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
