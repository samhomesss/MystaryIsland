using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Misson : MonoBehaviour
{
    private TextMeshProUGUI missonText;
    private string initText;
    

    private void Start()
    {
        missonText = GameObject.Find("Canvas").transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();

        initText = missonText.text;
        missonText.text = initText + "°î±ªÀÌ ¸¸µé±â";
    }

    private void Update()
    {
        
    }

    public void ChangeText(string text)
    {
        missonText.text = initText + text;
    }
}
