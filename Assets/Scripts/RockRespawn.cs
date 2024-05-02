using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRespawn : MonoBehaviour
{
    public GameObject RockPrefab = null;
    public int RESPAWN_TIME_ROCK = 8; // �� ���� �ð� ���.
    private float respawn_timer_rock = 0.0f; // ���� ���� �ð�. 

    // Start is called before the first frame update
    void Start()
    {
        RockPrefab = Resources.Load<GameObject>("Prefab/Rock");
        GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        respawn_timer_rock += Time.deltaTime;

        if (respawn_timer_rock > RESPAWN_TIME_ROCK)
        {
            respawn_timer_rock = 0.0f;
            this.respawnRock(); // ö������ ������Ų��.
        }
    }

    public void respawnRock()
    {
        // ö���� �������� �ν��Ͻ�ȭ.
        GameObject go = GameObject.Instantiate(this.RockPrefab) as GameObject;
        // ö������ ���� ����Ʈ�� ���.
        Transform trans = this.transform;
        Vector3 pos = trans.position;
        // ���� ��ġ�� ����.
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // ö������ ��ġ�� �̵�.
        go.transform.position = pos;
        go.name = RockPrefab.name;
        go.transform.SetParent(trans);
    }
}
