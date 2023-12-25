using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   
    public InventoryObject inventory;

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if(item)
        {
            inventory.AddItem(item.item,1);
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
        inventory.Container.Clear();
    }
}
