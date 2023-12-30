using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] itemsObjects;
   // public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();

    [ContextMenu("Update ID's")]
    public void UpdateID()
    {
        for (int i = 0; i < itemsObjects.Length; i++)
        {
            if (itemsObjects[i].data.Id != i)
                itemsObjects[i].data.Id = i;
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateID();
    }

    public void OnBeforeSerialize()
    {

    }
}
