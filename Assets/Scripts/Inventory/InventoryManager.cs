using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    PlayerControl player;

    public InventorySlot[] inventorySlots;
    public GameObject InventoryItemPrefab;

    static GameObject handlingItem;
    public GameObject[] storeItem;

    public GameObject storage;
    public List<GameObject> temp;

    int selectedSlot = -1;
    int preSlot = -1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
        storage = GameObject.Find("Storage");
        temp = new List<GameObject>();
        storeItem = new GameObject[inventorySlots.Length];
        ChangedSelectedSlot(0);
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 4)
            {
                ChangedSelectedSlot(number - 1);
            }
        }
    }

    void ChangedSelectedSlot(int newValue)
    {
        if (selectedSlot == newValue)
            return;

        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        preSlot = selectedSlot;
        selectedSlot = newValue;

        
        if (temp.Count > 0) return;

        if (storeItem[selectedSlot] != null)
        {
            if (handlingItem != null)
            {
                storeItem[preSlot] = handlingItem;
                storeItem[preSlot].transform.SetParent(storage.transform);
                storage.SetActive(false);
            }

            GameObject temp = storeItem[selectedSlot];
            temp.transform.SetParent(player.transform);
            player.CarriedPosition(temp);
            player.SetCarredItem(temp);
            handlingItem = temp;
        }
        else
        {
            if (handlingItem == null) return;

            storeItem[preSlot] = handlingItem;
            storeItem[preSlot].transform.SetParent(storage.transform);
            player.SetCarredItem(null);
            storage.SetActive(false);
            handlingItem = null;
            return;
        }
    }

    public bool AddItem(GameObject itemData)
    {
        handlingItem = itemData;
        
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSLot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSLot == null)
        {
            SpawnItemData(itemData, slot);
            return true;
        }

        return false;
    }

    void SpawnItemData(GameObject itemData, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(InventoryItemPrefab, slot.transform);
        newItemGo.name = InventoryItemPrefab.name;
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitailiseItemData(itemData);
    }

    public GameObject GetSelectedItem(GameObject itemData)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSLot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSLot != null)
        {
            if (itemData == null)
            {
                storeItem[selectedSlot] = null;
                Destroy(itemInSLot.gameObject);
                handlingItem = null;
            }
            
            return itemInSLot.itemData;
        }
        return null;
    }

    public PlayerControl GetPlayer()
    {
        return player;
    }

    public void ReSetStoreObj()
    {
        for (int i = 0; i < storeItem.Length; i++)
        {
            storeItem[i] = null;
        }
        player.SetCarredItem(null);
    }
}
