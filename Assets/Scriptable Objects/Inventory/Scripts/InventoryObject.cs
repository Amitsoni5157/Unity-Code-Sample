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

/*    private void OnEnable()
    {
#if UNITY_EDITOR
        database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Database.asset", typeof(ItemDatabaseObject));
#else
        database = Resources.Load<ItemDatabaseObject>("Database");
#endif
    }*/

    public void AddItem(Item _item, int _amount)
    {
        if (_item.buffs.Length > 0)
        {
            SetEmptySlot(_item, _amount);
            return;
        }

        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].Id == _item.Id)
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        SetEmptySlot(_item, _amount);
    }

    public InventorySlots SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item.Id,_item,_amount);
                return Container.Items[i];
            }
        }
        //Setup functionality for full inventory
        return null;
    }

    public void MoveItem(InventorySlots item1, InventorySlots item2)
    {
        InventorySlots temp = new InventorySlots(item2.Id, item2.item, item2.amount);
        item2.UpdateSlot(item1.Id, item1.item, item1.amount);
        item1.UpdateSlot(temp.Id,temp.item, temp.amount);
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
            }
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
                Container.Items[i].UpdateSlot(newContainer.Items[i].Id, newContainer.Items[i].item, newContainer.Items[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }

/*    public void OnAfterDeserialize()
    {
        for (int i = 0; i < Container.Items.Count; i++)
        {
            Container.Items[i].item = database.GetItem[Container.Items[i].Id];
        }
    }*/

  
}

[System.Serializable]
public class Inventory
{
    public InventorySlots[] Items = new InventorySlots[28];
}

[System.Serializable]
public class InventorySlots
{
    public int Id = -1;
    public Item item;
    public int amount;
    public InventorySlots()
    {
        Id = -1;
        item = null;
        amount = 0;
    }
    public InventorySlots(int _id, Item _item, int _amount)
    {
        Id = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        Id = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}
