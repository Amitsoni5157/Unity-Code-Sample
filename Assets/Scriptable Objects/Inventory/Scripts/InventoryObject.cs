using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

public enum InterfaceType
{
    Inventory,
    Equipment,
    Chest

}

[CreateAssetMenu(fileName = "new Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string SavePath;
    public ItemDatabaseObject database;
    public InterfaceType type;
    public Inventory Container;
    public InventorySlots[] GetSlots { get { return Container.Slots; } }

    public bool AddItem(Item _item, int _amount)
    {
        if(EmptySlotCount <= 0)
            return false;
        InventorySlots slot = FindItemOnInventory(_item);
        if(!database.itemsObjects[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id <= -1)
                    counter++;
            }
            return counter;
        }
    }

    public InventorySlots FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id == _item.Id)
            {
                return GetSlots[i];
            }
        }
        return null;
    }
    public InventorySlots SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id <= -1)
            {
                GetSlots[i].UpdateSlot(_item,_amount);
                return GetSlots[i];
            }
        }
        //Setup functionality for full inventory
        return null;
    }

    public void swapItem(InventorySlots item1, InventorySlots item2)
    {
        if(item2.CanPlaceInSlot(item1.Itemobject) && item1.CanPlaceInSlot(item2.Itemobject))
        {
            InventorySlots temp = new InventorySlots(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);

        }
    }

  
    //Amit
    [ContextMenu("Save")]
    public void Save()
    {
        /* string saveData = JsonUtility.ToJson(this,true);
         BinaryFormatter bf = new BinaryFormatter();
         FileStream file = File.Create(string.Concat(Application.persistentDataPath, SavePath));
         bf.Serialize(file, saveData);
         file.Close();*/

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, SavePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
       if(File.Exists(string.Concat(Application.persistentDataPath,SavePath)))
        {
            /* BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, SavePath),FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(),this);
            file.Close();*/

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, SavePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlots[] Slots = new InventorySlots[28];
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlots _slot);

[System.Serializable]
public class InventorySlots
{
    public TypeInterface[] AllowedItems = new TypeInterface[0];
    [System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;

    public Item item = new Item();
    public int amount;

    public ItemObject Itemobject
    {
        get
        {
            if(item.Id >= 0)
            {
                return parent.inventory.database.itemsObjects[item.Id];
            }
            return null;
        }
    }
    public InventorySlots()
    {
        UpdateSlot(new Item(), 0);
    }
    public InventorySlots(Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }
    public void UpdateSlot(Item _item, int _amount)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
        item = _item;
        amount = _amount;
        if (OnAfterUpdate != null)
            OnAfterUpdate.Invoke(this);
    }

    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }

    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
        
    }
    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
            return true;
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
                return true;
        }
        return false;
    }
}
