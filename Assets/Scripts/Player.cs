using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public MouseData mouseItem = new MouseData();
    public InventoryObject inventory;
    public InventoryObject equipment;

    public Attribute[] attributes;

    private Transform _boots;
    private Transform _chest;
    private Transform _helmet;
    private Transform _offhand;
    private Transform _sword;

    public Transform weaponTransform;
    public Transform offhandShieldTransform;
    public Transform offhandHandTransform;

    private BoneCombiner boneCombiner;


    private void Start()
    {
        boneCombiner = new BoneCombiner(gameObject);
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    public void OnRemoveItem(InventorySlots _slot)
    {
        if(_slot.Itemobject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Removed ",_slot.Itemobject," on ", _slot.parent.inventory.type, ", Allowed Item: ", string.Join(", ",_slot.AllowedItems)));
                
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attributes)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }

                if (_slot.Itemobject.CharacterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case TypeInterface.Helmet:
                            Destroy(_helmet.gameObject);
                            break;
                        case TypeInterface.Weapon:
                             Destroy(_sword.gameObject);
                            break;
                        case TypeInterface.Shield:
                            Destroy(_offhand.gameObject);
                            break;
                        case TypeInterface.Boots:
                            Destroy(_boots.gameObject);
                            break;
                        case TypeInterface.Chest:
                            Destroy(_chest.gameObject);
                            break;
                    }
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
        Debug.Log("Before Update");
    }

    public void OnAddItem(InventorySlots _slot)
    {
        if (_slot.Itemobject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Placed ", _slot.Itemobject, " on ", _slot.parent.inventory.type, ", Allowed Item: ", string.Join(", ", _slot.AllowedItems)));
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attributes)
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                    }
                }

                if(_slot.Itemobject.CharacterDisplay != null)
                {
                    switch (_slot.AllowedItems[0])
                    {
                        case TypeInterface.Helmet:
                            _helmet = boneCombiner.AddLimb(_slot.Itemobject.CharacterDisplay,_slot.Itemobject.boneNames);
                            break;
                        case TypeInterface.Weapon:
                            _sword = Instantiate(_slot.Itemobject.CharacterDisplay, weaponTransform).transform;
                            break;
                        case TypeInterface.Shield:
                            switch (_slot.Itemobject.type)
                            {   
                                case TypeInterface.Weapon:
                                    _offhand = Instantiate(_slot.Itemobject.CharacterDisplay, offhandHandTransform).transform;
                                    break;
                                case TypeInterface.Shield:
                                    _offhand = Instantiate(_slot.Itemobject.CharacterDisplay, offhandShieldTransform).transform;
                                    break;                                
                            }
                            break;
                        case TypeInterface.Boots:
                            _boots = boneCombiner.AddLimb(_slot.Itemobject.CharacterDisplay, _slot.Itemobject.boneNames);
                            break;
                        case TypeInterface.Chest:
                            _chest = boneCombiner.AddLimb(_slot.Itemobject.CharacterDisplay, _slot.Itemobject.boneNames);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                            break;
                    }
                }

                break;
            case InterfaceType.Chest:
                break;
        }
        Debug.Log("After Update");
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if(item)
        {
            Item _item = new Item(item.item);
            if (inventory.AddItem(_item, 1))
            { 
                Destroy(other.gameObject);
            }
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            inventory.Save();
            equipment.Save();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Load");
            inventory.Load();
            equipment.Load();
        }

    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }


    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public Player parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(Player _parent)
    {//Amit 
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
