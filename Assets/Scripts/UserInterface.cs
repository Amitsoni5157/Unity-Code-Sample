using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public abstract class UserInterface : MonoBehaviour
{
    
    public InventoryObject inventory;


    public Dictionary<GameObject, InventorySlots> slotsOnInterface = new Dictionary<GameObject, InventorySlots>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();//  CreateDisplay();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    // Update is called once per frame
    void Update()
    {
        slotsOnInterface.UpdateSlotDisplay();// UpdateDisplay();
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

    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoverdOver = obj;
    }
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoverdOver = null;
    }
    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();   
    }
    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }
    public void OnDeagStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempUItem(obj);
    }
    public GameObject CreateTempUItem(GameObject obj)
    {
        GameObject tempItem = null;
        if(slotsOnInterface[obj].item.Id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(60, 60);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].Itemobject.uiDisplay/*inventory.database.GetItem[slotsOnInterface[obj].item.Id].uiDisplay*/;
            img.raycastTarget = false;
        }
        return tempItem;
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if(MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoverdOver)
        {
            InventorySlots mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoverdOver];
            inventory.swapItem(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }
    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
        {
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}
public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoverdOver;
}

public static class ExtensionMethod
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlots> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlots> _slot in _slotsOnInterface)
        {
            if (_slot.Value.item.Id >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.Itemobject.uiDisplay;/*inventory.database.GetItem[_slot.Value.item.Id].uiDisplay*/;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
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
}