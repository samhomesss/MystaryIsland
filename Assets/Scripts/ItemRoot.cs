using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum TYPE
    { // ������ ����.
        NONE = -1, IRON = 0, APPLE, PLANT, LUMBER, ROCK, PICKAXE, HAMMER, HANDSAW, SMELTEDIRON, // ����, ö����, ���, �Ĺ�, ����, ������, ���
        NUM,
    }; // �������� �� �����ΰ� ��Ÿ����(=3).
};

public class ItemRoot : MonoBehaviour
{
    public GameObject RockPrefab = null; // Prefab 'ROCK'
    public GameObject plantPrefab = null; // Prefab 'Plant'
    public GameObject ironPrefab = null; // Prefab 'Iron'
    public GameObject appleTreePrefab = null; // Prefab 'Tree'
    public GameObject smeltedIronPrefab = null; // Prefab 'SmeltedIron'

    Apple apple;
    Tree tree;
    Misson missonText;
    protected List<Transform> respawn_points; // ���� ���� List.
    protected List<Transform> respawn_points_Rock; // ���� ���� List.

    public float step_timer = 0.0f;
    //public static float RESPAWN_TIME_APPLE = 5.0f; // ��� ���� �ð� ���.
    public static float RESPAWN_TIME_ROCK = 7.0f; // �� ���� �ð� ���.
    public static float RESPAWN_TIME_PLANT = 8.0f; // �Ĺ� ���� �ð� ���.
    

    //private float respawn_timer_apple = 0.0f; // ����� ���� �ð�.
    private float respawn_timer_rock = 0.0f; // ���� ���� �ð�. 
    private float respawn_timer_plant = 0.0f; // �Ĺ��� ���� �ð�.

    // �ʱ�ȭ �۾��� �����Ѵ�.
    void Start()
    {
        // �޸� ���� Ȯ��.
        this.respawn_points = new List<Transform>();
        this.respawn_points_Rock = new List<Transform>();
        // "PlantRespawn" �±װ� ���� ��� ������Ʈ�� �迭�� ����.
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("PlantRespawn");
        GameObject[] respawns_Rock = GameObject.FindGameObjectsWithTag("RockRespawn");
        apple = FindObjectOfType<Apple>().GetComponent<Apple>();
        tree = FindObjectOfType<Tree>().GetComponent<Tree>();
        RockPrefab = Resources.Load<GameObject>("Prefab/Rock");
        plantPrefab = Resources.Load<GameObject>("Prefab/Plant");
        appleTreePrefab = Resources.Load<GameObject>("Prefab/Tree");
        ironPrefab = Resources.Load<GameObject>("Prefab/Iron");
        smeltedIronPrefab = Resources.Load<GameObject>("Prefab/SmeltedIron");
        missonText = GameObject.FindObjectOfType<Misson>().GetComponent<Misson>();
        // �迭 respawns ���� ������ GameObject�� �������� ó���Ѵ�.
        foreach (GameObject go in respawns)
        {
            // ������ ȹ��.
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            { // �������� �����ϸ�.
                renderer.enabled = false; // �� �������� ������ �ʰ�.
            }
            // ���� ����Ʈ List�� ��ġ ������ �߰�.
            this.respawn_points.Add(go.transform);
        }
        foreach (GameObject go in respawns_Rock)  // ö������ ���� ����Ʈ�� ����ϰ�, �������� ������ �ʰ�.
        {
            // ������ ȹ��.
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            { // �������� �����ϸ�.
                renderer.enabled = false; // �� �������� ������ �ʰ�.
            }
            // ���� ����Ʈ List�� ��ġ ������ �߰�.
            this.respawn_points_Rock.Add(go.transform);
        }
               
        this.respawnPlant();

        this.respawnPlant();
        this.respawnPlant();
    }

    // �� �������� Ÿ�̸� ���� ���� �ð��� �ʰ��ϸ� �ش� �������� ����.
    void Update()
    {
        respawn_timer_rock += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;
  
        if (respawn_timer_rock > RESPAWN_TIME_ROCK)
        {
            respawn_timer_rock = 0.0f;
            this.respawnRock(); // ö������ ������Ų��.
        }
        if (respawn_timer_plant > RESPAWN_TIME_PLANT)
        {
            respawn_timer_plant = 0.0f;
            this.respawnPlant(); // �Ĺ��� ������Ų��.
        }
    }

    // �������� ������ Item.TYPE������ ��ȯ�ϴ� �޼ҵ�.
    public Item.TYPE getItemType(GameObject item_go)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)
        { // �μ��� ���� GameObject�� ������� ������.
            switch (item_go.tag)
            { // �±׷� �б�.
                case "Rock": type = Item.TYPE.ROCK; break;
                case "Apple": type = Item.TYPE.APPLE; break;
                case "Plant": type = Item.TYPE.PLANT; break;
                case "Iron": type = Item.TYPE.IRON; break;
                case "Lumber": type = Item.TYPE.LUMBER; break;
                case "PickAxe": type = Item.TYPE.PICKAXE; break;
                case "Hammer": type = Item.TYPE.HAMMER; break;
                case "HandSaw": type = Item.TYPE.HANDSAW; break;
                case "SmeltedIron": type = Item.TYPE.SMELTEDIRON; break;
            }
        }
        return (type);
    }

    // ö������ ������Ų��.
    public void respawnRock()
    {
        if (this.respawn_points_Rock.Count > 0)
        { // List�� ������� ������.
          // �Ĺ� �������� �ν��Ͻ�ȭ.
            GameObject go = GameObject.Instantiate(this.RockPrefab) as GameObject;
            // �Ĺ��� ���� ����Ʈ�� �����ϰ� ���.
            int n = Random.Range(0, this.respawn_points_Rock.Count);
            Transform trans = this.respawn_points_Rock[n];
            Vector3 pos = trans.position;
            // ���� ��ġ�� ����.
            pos.y = 0.0f;
            pos.x += Random.Range(-1.0f, 1.0f);
            pos.z += Random.Range(-1.0f, 1.0f);
            // ö������ ��ġ�� �̵�.
            go.transform.position = pos;
            go.name = RockPrefab.name;
            go.transform.SetParent(trans);
        }
    }

    public void creatIron()
    {
        //--------------------------
        GameObject go = GameObject.Instantiate(this.ironPrefab) as GameObject;
        // ö������ ���� ����Ʈ�� ���.
        Transform trans = GameObject.Find("IronRespawn").transform;
        Vector3 pos = trans.position;
        // ���� ��ġ�� ����.
        pos.y = 0.0f;
        pos.x = trans.position.x + Random.Range(-2.0f, 2.0f);
        pos.z = trans.position.z +Random.Range(-2.0f, 2.0f);
        // ö������ ��ġ�� �̵�.
        go.transform.position = pos;
        go.name = ironPrefab.name;
        go.transform.SetParent(trans);
    }

    public void creatSmeltedIron()
    {
        Transform trans = GameObject.Find("Brazier").transform;
        Vector3 pos = trans.position;
        // ���� ��ġ�� ����.
        // ö������ ��ġ�� �̵�.
        pos.y = 0.5f;

        GameObject go = GameObject.Instantiate(this.smeltedIronPrefab, pos + new Vector3(1.0f, 0, -1.7f), Quaternion.Euler(0, 150, 0)) as GameObject;
        // ö������ ���� ����Ʈ�� ���.
        go.name = smeltedIronPrefab.name;
        go.transform.SetParent(trans);

        bool used = false;
        if(!used)
        {
            used = true;
            missonText.ChangeText("�踦 ���� Ż���ϱ�");
        }
    }


    // �Ĺ��� ������Ų��.
    public void respawnPlant()
    {
        if (this.respawn_points.Count > 0)
        { // List�� ������� ������.
          // �Ĺ� �������� �ν��Ͻ�ȭ.
            GameObject go = GameObject.Instantiate(this.plantPrefab) as GameObject;
            // �Ĺ��� ���� ����Ʈ�� �����ϰ� ���.
            int n = Random.Range(0, this.respawn_points.Count);
            Transform trans = this.respawn_points[n];
            Vector3 pos = trans.position;
            // ���� ��ġ�� ����.
            pos.y = 0.4f;
            pos.x += Random.Range(-1.0f, 1.0f);
            pos.z += Random.Range(-1.0f, 1.0f);
            // �Ĺ��� ��ġ�� �̵�.
            go.transform.position = pos;
            go.name = plantPrefab.name;
            go.transform.SetParent(trans);
        }
    }

    public void CreatTree(GameObject apple)
    {
        GameObject go = GameObject.Instantiate(this.appleTreePrefab) as GameObject;

        go.transform.position = new Vector3(apple.transform.position.x, 0f, apple.transform.position.z);
        go.gameObject.name = appleTreePrefab.name;

    }
    

    // ��� �ִ� �����ۿ� ���� ������ ��ô ���¡��� ��ȯ
    public float getGainRepairment(GameObject item_go)
    {
        float gain = 0.0f;
        if (item_go == null)
        {
            gain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.SMELTEDIRON:
                    gain = GameStatus.GAIN_REPAIRMENT_SMELTEDIRON; break;
                case Item.TYPE.LUMBER:
                    gain = GameStatus.GAIN_REPAIRMENT_LUMBER; break;
            }
        }
        return (gain);
    }
    // ��� �ִ� �����ۿ� ���� ��ü�� ���� ���¡��� ��ȯ
    public float getConsumeSatiety(GameObject item_go)
    {
        float consume = 0.0f;
        if (item_go == null)
        {
            consume = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.ROCK:
                    consume = GameStatus.CONSUME_SATIETY_ROCK; break;
                case Item.TYPE.APPLE:
                    consume = GameStatus.CONSUME_SATIETY_APPLE; break;
                case Item.TYPE.PLANT:
                    consume = GameStatus.CONSUME_SATIETY_PLANT; break;
            }
        }
        return (consume);
    }
    // ��� �ִ� �����ۿ� ���� ��ü�� ȸ�� ���¡��� ��ȯ
    public float getRegainSatiety(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.APPLE:
                    regain = GameStatus.REGAIN_SATIETY_APPLE; break;
                case Item.TYPE.PLANT:
                    regain = GameStatus.REGAIN_SATIETY_PLANT; break;
            }
        }
        return (regain);
    }

    public float getRegainTemperature(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.APPLE:
                    regain = GameStatus.REGAIN_TEMPERATURE_APPLE; break;
                case Item.TYPE.PLANT:
                    regain = GameStatus.REGAIN_TEMPERATURE_PLANT; break;
            }
        }
        return (regain);
    }

    public Tree getTree()
    {
        return tree;
    }
}
