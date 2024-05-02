using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockRespawn : MonoBehaviour
{
    public GameObject RockPrefab = null;
    public int RESPAWN_TIME_ROCK = 8; // 돌 출현 시간 상수.
    private float respawn_timer_rock = 0.0f; // 돌의 출현 시간. 

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
            this.respawnRock(); // 철광석을 출현시킨다.
        }
    }

    public void respawnRock()
    {
        // 철광석 프리팹을 인스턴스화.
        GameObject go = GameObject.Instantiate(this.RockPrefab) as GameObject;
        // 철광석의 출현 포인트를 취득.
        Transform trans = this.transform;
        Vector3 pos = trans.position;
        // 출현 위치를 조정.
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // 철광석의 위치를 이동.
        go.transform.position = pos;
        go.name = RockPrefab.name;
        go.transform.SetParent(trans);
    }
}
