using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public GameObject workTable;

    private void Awake()
    {
        Deselect();
    }

    private void Start()
    {
        workTable = GameObject.Find("WorkTable");
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

            if (inventoryItem.parentAfterDrag.parent.name == transform.parent.name)
            {
                inventoryItem.parentAfterDrag = transform;
                return;
            }

            inventoryItem.parentAfterDrag = transform;
            
            if (inventoryItem.parentAfterDrag.parent.name == "MakingSlot")
            {
                GameObject obj = InventoryManager.instance.GetPlayer().gameObject.transform.GetChild(1).gameObject;
                InventoryManager.instance.temp.Add(obj);
            
                InventoryManager.instance.GetPlayer().gameObject.transform.GetChild(1).transform.SetParent(workTable.transform);
                return;
            }

            if (inventoryItem.parentAfterDrag.parent.name == "ItemSlot")
            {
                for (int i = 0; i < workTable.transform.childCount; i++)
                {
                    if (InventoryManager.instance.temp[i].name == workTable.transform.GetChild(0).name)
                    {                        
                        workTable.transform.Find(InventoryManager.instance.temp[i].name).SetParent(InventoryManager.instance.GetPlayer().gameObject.transform);
                        InventoryManager.instance.temp.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

}
