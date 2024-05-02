using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public GameObject applePrefab = null; // Prefab 'Apple'

    public static float RESPAWN_TIME_APPLE = 5.0f;
    private float respawn_timer_apple = 0.0f; // 사과의 출현 시간.
    
    private int num = 0;
    private void Start()
    {
        applePrefab = Resources.Load<GameObject>("Prefab/Apple");
    }

    private void Update()
    {
        respawn_timer_apple += Time.deltaTime;

        if (respawn_timer_apple > RESPAWN_TIME_APPLE)
        {
            respawn_timer_apple = 0.0f;

            if(num <= 4)
                respawnApple();
        }
    }

    public void respawnApple()
    {
        num++;

        // 사과 프리팹을 인스턴스화.
        GameObject go = GameObject.Instantiate(this.applePrefab) as GameObject;
        // 사과의 출현 포인트를 취득.
        Vector3 pos = transform.parent.position;
        // 출현 위치를 조정.
        pos.y = 0.2f;
        float radius = transform.parent.GetComponent<SphereCollider>().radius;
        pos.x += Random.Range(-radius, radius);
        pos.z += Random.Range(-radius, radius);
        // 사과의 위치를 이동.
        go.transform.position = pos;
        go.gameObject.name = applePrefab.name;
    }
}
