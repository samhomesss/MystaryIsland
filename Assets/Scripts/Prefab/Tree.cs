using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    public GameObject fellingTreePrefab = null; // Prefab 'lumber'

    private void Start()
    {
        fellingTreePrefab = Resources.Load<GameObject>("Prefab/Lumber");
    }

    public void fellingTree(GameObject obj)
    {
        // 출현 위치를 조정.
        Vector3 pos = obj.transform.position;
        pos.y = 0.3f;
        
        GameObject go = GameObject.Instantiate(this.fellingTreePrefab, pos + new Vector3(1.0f, 0, -1.7f), Quaternion.identity) as GameObject;
        // 철광석의 위치를 이동.
        go.name = fellingTreePrefab.name;
        Destroy(obj);
    }
}
