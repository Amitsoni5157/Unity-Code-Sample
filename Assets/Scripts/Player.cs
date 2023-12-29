using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public MouseData mouseItem = new MouseData();
    public InventoryObject inventory;
    public InventoryObject equipment;

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

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        equipment.Container.Clear();
    }
}
