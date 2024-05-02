using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaButton : MonoBehaviour
{
    public float iAlphaThreshhold = 0.1f;

    static int currentPage = 0;
    private int totalPage = 4;
    private Button btn;
    private GameObject OptionUI;

    private void Start()
    {
        OptionUI = GameObject.Find("OptionsUI");

        this.GetComponent<Image>().alphaHitTestMinimumThreshold = this.iAlphaThreshhold;
        btn = GetComponent<Button>();

        if (btn.name == "Left")
            this.btn.onClick.AddListener(() => { this.PrevPage(); });
        else
            this.btn.onClick.AddListener(() => { this.NextPage(); });
    }

    public void NextPage()
    {
        OptionUI.transform.GetChild(currentPage).gameObject.SetActive(false);

        currentPage++;

        if (currentPage >= this.totalPage)
        {
            currentPage = currentPage % totalPage;
        }
        Debug.Log(currentPage);

        OptionUI.transform.GetChild(currentPage).gameObject.SetActive(true);

    }

    public void PrevPage()
    {
        OptionUI.transform.GetChild(currentPage).gameObject.SetActive(false);
        currentPage--;
        
        if (currentPage < 0)
        {
            currentPage = totalPage - 1;
        }
        Debug.Log(currentPage);
        OptionUI.transform.GetChild(currentPage).gameObject.SetActive(true);

    }
}
