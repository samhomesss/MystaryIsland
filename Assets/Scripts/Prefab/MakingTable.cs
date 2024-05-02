using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MakingTable : MonoBehaviour
{
    GameObject player;
    GameObject makingTable;
    public GameObject[] prefabs;
    public GameObject itemSlot;
    public GUIStyle guistyle;

    Misson missonText;

    int rockNum = 0;
    int plantNum = 0;
    int lumberNum = 0;
    int smeltedIron = 0;
    int iron = 0;

    bool action = false;
    bool loop = true;
    bool onTable = false;

    private void Start()
    {
        makingTable = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        prefabs = Resources.LoadAll<GameObject>("Tool");
        itemSlot = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        missonText = GameObject.FindObjectOfType<Misson>().GetComponent<Misson>();
        this.guistyle.fontSize = 32;

        for (int i = 0; i < 4; i++)
        {
            Button btn = makingTable.transform.GetChild(i + 2).GetComponent<Button>();
            btn.interactable = false;
        }
    }

    private void Update()
    {
        float dis = Vector3.Distance(transform.position, player.transform.position);

        if (Input.GetKeyDown(KeyCode.D) && makingTable.activeSelf == true || Input.GetKeyDown(KeyCode.Escape) && !onTable || dis > 3.0f || Input.GetKeyDown(KeyCode.D) && !onTable)
        {
            onTable = false;
            makingTable.SetActive(false);
           
            init();
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) && onTable)
        {
            makingTable.SetActive(true);
            action = true;
            Checking();
            BtnInteractive();
        }
    }

    void Checking()
    {
        bool result = childCount();
        if (!result) return;

        if (loop)
        {
            loop = false;
            for (int i = 0; i < itemSlot.transform.childCount; i++)
            {
                string name = itemSlot.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite.name;
                switch (name)
                {
                    case "Rock":
                        rockNum++;
                        break;
                    case "Plant":
                        plantNum++;
                        break;
                    case "Lumber":
                        lumberNum++;
                        break;
                    case "SmeltedIron":
                        smeltedIron++;
                        break;
                    case "Iron":
                        iron++;
                        break;
                    default:
                        break;
                }
            }

        }


        //Debug.Log("돌: " + rockNum);
        //Debug.Log("식물: " + plantNum);
        //Debug.Log("목재: " + lumberNum);
        //Debug.Log("정제된 철: " + smeltedIron);
        //Debug.Log("철: " + iron);
    }

    public void CreatTool(GameObject tool)
    {
        int lastIndex = tool.name.LastIndexOf('(');
        string name = $"{tool.name.Substring(0, lastIndex)}";

        GameObject go = GameObject.Instantiate(Findprefabs(name)) as GameObject;

        Vector3 pos = transform.position;
        pos.y = 0.1f;

        bool used = false;
        if (name == "Brazier")
        {
            go.transform.position = pos + new Vector3(-5.5f, 0, -1.0f);
            if (!used)
            {
                used = true;
                missonText.ChangeText("철광석으로 철 만들기");
            }
        }
        else
            go.transform.position = pos + new Vector3(0, 0, -2.0f);


        go.gameObject.name = name;

        DestoryItem();
        init();

        bool used2 = false;
        if (name == "PickAxe" && !used2)
        {
            used2 = true;
            missonText.ChangeText("화로 만들기");
        }
        Debug.Log(transform.gameObject.name);
        makingTable.SetActive(false);
    }

    private GameObject Findprefabs(string name)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].name == name)
                return prefabs[i];
        }

        Debug.Log("프리팹 없음");
        return null;
    }



    private void DestoryItem()
    {
        for (int i = 0; i < itemSlot.transform.childCount; i++)
        {
            Destroy(itemSlot.transform.GetChild(i).transform.GetChild(0).gameObject);
        }
        Destroy(InventoryManager.instance.GetPlayer().transform.GetChild(1).gameObject);
        InventoryManager.instance.ReSetStoreObj();
    }

    private void BtnInteractive()
    {
        if (rockNum == 2 && plantNum == 1) makingTable.transform.GetChild(2).GetComponent<Button>().interactable = true;
        if (plantNum == 2 && smeltedIron == 1) makingTable.transform.GetChild(3).GetComponent<Button>().interactable = true;
        if (smeltedIron == 2 && lumberNum == 1) makingTable.transform.GetChild(4).GetComponent<Button>().interactable = true;
        if (rockNum == 2 && iron == 1) makingTable.transform.GetChild(5).GetComponent<Button>().interactable = true;
    }

    bool childCount()
    {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;

        for (int i = 0; i < itemSlot.transform.childCount; i++)
        {
            num1 = itemSlot.transform.GetChild(0).transform.childCount;
            num2 = itemSlot.transform.GetChild(1).transform.childCount;
            num3 = itemSlot.transform.GetChild(2).transform.childCount;
        }
        if (num1 <= 0 || num2 <= 0 || num3 <= 0)
            return false;

        return true;
    }

    private void init()
    {
        rockNum = 0;
        plantNum = 0;
        lumberNum = 0;
        smeltedIron = 0;

        for (int i = 0; i < 4; i++)
        {
            makingTable.transform.GetChild(i+2).GetComponent<Button>().interactable = false;
        }
        loop = true;
        action = false;
    }

    public bool TableAction()
    {
        return action;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            onTable = true;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.name == "Player")
    //    {
    //        onTable = true;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (!makingTable.activeSelf) return;

            onTable = false;

            makingTable.SetActive(false);
            init();
        }

    }

    private void OnGUI()
    {
        float x = 360.0f;
        float y = Screen.height - 50.0f;

        if(!action && onTable)
        {
            GUI.Label(new Rect(x, y, 200.0f, 20.0f), "D:제작대를 연다", guistyle);
        }
        else if(action && onTable)
        {
            GUI.Label(new Rect(x, y, 200.0f, 20.0f), "D:제작대를 닫는다", guistyle);
        }
    }
}
