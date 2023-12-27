using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   
    public InventoryObject inventory;

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if(item)
        {
            inventory.AddItem(new Item(item.item),1);
            Destroy(other.gameObject);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            inventory.Save();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Load");
            inventory.Load();
        }

    }

    private void OnApplicationQuit()
    {
        inventory.Container.Items = new InventorySlots[28];
    }
}
