using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Title : MonoBehaviour
{
    Button btn;
    bool isClick;
    GameObject ui;
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Title")
            ui = GameObject.Find("Canvas").transform.GetChild(4).gameObject;

        btn = GetComponent<Button>();

        btn.onClick.AddListener(IsWhat);
    }

    void IsWhat()
    {
        switch (this.gameObject.name)
        {
            case "Start":
                SceneManager.LoadScene("PlaplaPlanet");
                break;
            case "Options":
                ui.SetActive(true); 
                break;
            case "Return":
                ui.SetActive(false);
                break;
            case "ReStart":
                SceneManager.LoadScene("PlaplaPlanet");
                break;
            case "Title":
                SceneManager.LoadScene("Title");
                break;
            default:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                break;
        }
    }
}
