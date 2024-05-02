using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{  
    [Header("UI")]
    public Image image;

    [HideInInspector] public GameObject itemData;
    [HideInInspector] public Transform parentAfterDrag;

    public Sprite[] sprites;

    public void InitailiseItemData(GameObject itemData)
    {
        this.itemData = itemData;
        image.sprite = FindImage(itemData.name);

    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    image.raycastTarget = false;
    //    parentAfterDrag = transform.parent;
    //    transform.SetParent(transform.root);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    transform.position = Input.mousePosition;
    //}


    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    image.raycastTarget = true;
    //    transform.SetParent(parentAfterDrag);
    //}

    private Sprite FindImage(string name)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == name)
                return sprites[i];
        }

        Debug.Log("이미지 없음");
        return null;
    }
}
