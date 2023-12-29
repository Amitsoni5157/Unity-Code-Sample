using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Helmet,
    Weapon,
    Shield,
    Boots,
    Chest,
    Default
}

public enum Attributes
{
    Agility,
    Intellect,
    Stemina,
    Strngth
}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item/item")]
public class ItemObject : ScriptableObject
{
    public Sprite uiDisplay;
    public bool stackable;
    public ItemType type;
    [TextArea(15,20)]
    public string Description;
    public Item data = new Item();  

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public Item()
    {
        name = "";
        Id = -1;
    }
    public Item(ItemObject item)
    {
        name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max);
            buffs[i].attributes = item.data.buffs[i].attributes;
        }
    }
}
[System.Serializable]
public class ItemBuff
{
    public Attributes attributes;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min,max);
    }
}
