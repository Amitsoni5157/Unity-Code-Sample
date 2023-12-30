using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticInterface : UserInterface
{
    public GameObject[] slots;

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlots>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = slots[i];

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDeagStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            inventory.GetSlots[i].slotDisplay = obj;
            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
        }
    }

    
}
