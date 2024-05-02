using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public GameObject applePrefab = null; // Prefab 'Apple'

    public static float RESPAWN_TIME_APPLE = 5.0f;
    private float respawn_timer_apple = 0.0f; // ����� ���� �ð�.
    
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

        // ��� �������� �ν��Ͻ�ȭ.
        GameObject go = GameObject.Instantiate(this.applePrefab) as GameObject;
        // ����� ���� ����Ʈ�� ���.
        Vector3 pos = transform.parent.position;
        // ���� ��ġ�� ����.
        pos.y = 0.2f;
        float radius = transform.parent.GetComponent<SphereCollider>().radius;
        pos.x += Random.Range(-radius, radius);
        pos.z += Random.Range(-radius, radius);
        // ����� ��ġ�� �̵�.
        go.transform.position = pos;
        go.gameObject.name = applePrefab.name;
    }
}
