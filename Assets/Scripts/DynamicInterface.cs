using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : UserInterface
{

    public GameObject inventoryPrefab;
    public int X_Start;
    public int Y_Start;
    public int X_Space_Between_Items;
    public int Number_Of_Coloumn;
    public int Y_Space_Between_Items;


    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlots>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDeagStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            slotsOnInterface.Add(obj, inventory.Container.Items[i]);
        }
    }

    private Vector3 GetPosition(int i)
    {
        return new Vector3(X_Start + (X_Space_Between_Items * (i % Number_Of_Coloumn)), Y_Start + (-Y_Space_Between_Items * (i / Number_Of_Coloumn)), 0f);
    }
}
