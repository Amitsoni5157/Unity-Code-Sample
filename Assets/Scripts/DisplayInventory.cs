using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DisplayInventory : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();

    public GameObject inventoryPrefab;
    public InventoryObject inventory;
    public int X_Start;
    public int Y_Start;
    public int X_Space_Between_Items;
    public int Number_Of_Coloumn;
    public int Y_Space_Between_Items;

    Dictionary<GameObject,InventorySlots> itemDisplayed = new Dictionary<GameObject, InventorySlots>();

    // Start is called before the first frame update
    void Start()
    {
        CreateSlots();//  CreateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSlots();// UpdateDisplay();
    }

    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlots> _slot in itemDisplayed)
        {
            if (_slot.Value.Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1,1,1,1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    /*public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
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
    }*/

    public void CreateSlots()//CreateDisplay()
    {
        itemDisplayed = new Dictionary<GameObject, InventorySlots>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter,delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDeagStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemDisplayed.Add(obj, inventory.Container.Items[i]);
        }




       /* for (int i = 0; i < inventory.Container.Items.Count; i++)
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
        }*/
    }

    private void AddEvent(GameObject obj, EventTriggerType type,UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if(itemDisplayed.ContainsKey(obj))
        {
            mouseItem.hoverItem = itemDisplayed[obj];
        }
    }
    public void OnExit(GameObject obj)
    {
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null;
    }
    public void OnDeagStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60,60);
        mouseObject.transform.SetParent(transform.parent);
        if(itemDisplayed[obj].Id >= 0)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemDisplayed[obj].Id].uiDisplay;
            img.raycastTarget = false;
        }
        mouseItem.obj = mouseObject;
        mouseItem.item = itemDisplayed[obj];
    }
    public void OnDragEnd(GameObject obj)
    {
        if(mouseItem.hoverObj)
        {
            inventory.MoveItem(itemDisplayed[obj],itemDisplayed[mouseItem.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemDisplayed[obj].item);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if(mouseItem.obj != null)
        {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
    


    public Vector3 GetPosition(int i)
    {
        return new Vector3(X_Start + (X_Space_Between_Items * (i % Number_Of_Coloumn)), Y_Start + (-Y_Space_Between_Items * (i / Number_Of_Coloumn)), 0f);
    }
}

public class MouseItem
{
    public GameObject obj;
    public InventorySlots item;
    public InventorySlots hoverItem;
    public GameObject hoverObj;
}
