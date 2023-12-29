using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "new Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string SavePath;
    public ItemDatabaseObject database;
    public Inventory Container;

    public bool AddItem(Item _item, int _amount)
    {
        if(EmptySlotCount <= 0)
            return false;
        InventorySlots slot = FindItemOnInventory(_item);
        if(!database.items[_item.Id].stackable || slot == null)
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
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.Id <= -1)
                    counter++;
            }
            return counter;
        }
    }

    public InventorySlots FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item.Id == _item.Id)
            {
                return Container.Items[i];
            }
        }
        return null;
    }
    public InventorySlots SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item.Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item,_amount);
                return Container.Items[i];
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
            Inventory  newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].item, newContainer.Items[i].amount);
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
    public InventorySlots[] Items = new InventorySlots[28];
    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RemoveItem();
        }
    }
}

[System.Serializable]
public class InventorySlots
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UserInterface parent;
    public Item item = new Item();
    public int amount;

    public ItemObject Itemobject
    {
        get
        {
            if(item.Id >= 0)
            {
                return parent.inventory.database.items[item.Id];
            }
            return null;
        }
    }
    public InventorySlots()
    {
        item = new Item();
        amount = 0;
    }
    public InventorySlots(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public void RemoveItem()
    {
        item = new Item();
        amount = 0;
    }

    public void AddAmount(int value)
    {
        amount += value;
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
