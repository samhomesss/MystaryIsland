using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS : MonoBehaviour
{
    GameObject player;

    public float distance = 9.0f;   // currentZoom���� ��Ȯ�� �̸����� ����
    public float minZoom = 5.0f;
    public float maxZoom = 10.0f;
    public float sensitivity = 100f; // ���콺 ����

    float x;
    float y;

    GameObject transparentObj;
    Renderer ObstacleRenderer;  // ������Ʈ�� �������ϰ� ������ִ� ������
    List<GameObject> Obstacles;

    void Start()
    {
        // �÷��̾� �±׸� ���� ���ӿ�����Ʈ(=�÷��̾�)�� ã�Ƽ� �ֱ�
        player = GameObject.FindGameObjectWithTag("Player");
        Obstacles = new List<GameObject>(); // �� ����Ʈ ����
    }

    // Update is called once per frame
    void Update()
    {
        RotateAround();
        //CalculateZoom();
        FadeOut();

    }

    // ī�޶� Ȯ���� ���
    void CalculateZoom()
    {
        // ���콺 �� ��/�ƿ�
        distance -= Input.GetAxis("Mouse ScrollWheel");

        // �� �ּ�/�ִ� ����
        // Clamp�Լ� : �ִ�/�ּҰ��� �������ְ� ����
        distance = Mathf.Clamp(distance, minZoom, maxZoom);
    }

    void RotateAround()
    {
        // ���콺�� ��ġ�� �޾ƿ���
        x += Input.GetAxis("Mouse X") * sensitivity * 0.01f; // ���콺 �¿� ������ ����
        y -= Input.GetAxis("Mouse Y") * sensitivity * 0.01f; // ���콺 ���� ������ ����

        // ī�޶� ���̰�(������������) ����
        if (y < 10)  // �ٴ��� ���� �ʰ�
            y = 10;
        if (y > 50) // Top View(�������� ��������)�� �ϰ� �ʹٸ� 90���� �ٲٱ�
            y = 50;

        // player.transform�� ���� ����Ұǵ� �ʹ� �� ġȯ => target
        Transform target = player.transform;

        // ī�޶� ȸ���� ������ �̵��� ��ġ ���
        Quaternion angle = Quaternion.Euler(y, x, 0);

        Vector3 destination =(Vector3.back * distance) + target.position + Vector3.zero + new Vector3(0, 4, 0);

        //transform.rotation = angle;             // ī�޶� ���� ����
        transform.position = destination;   // ī�޶� ��ġ ����
    }

    void FadeOut()
    {
        // Raycast�� �̿��Ͽ� �÷��̾�� ī�޶� ���̿� �ִ� ������Ʈ ����
        // ������Ʈ�� �������� �������� Layer�� Ignor Raycast�� �ٲ���ƾ� ��
        // Ignore Raycast: Player, Terrain, Particles(Steam, DustStorm)
        float distance = Vector3.Distance(transform.position, player.transform.position) - 1;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        RaycastHit[] hits;

        // ī�޶󿡼� �÷��̾ ���� �������� ����� �� ���� ������Ʈ�� �ִٸ�
        hits = Physics.RaycastAll(transform.position, direction, distance);

        bool remove = true;
        if (Obstacles.Count != 0 && hits != null)
        {
            for (int i = 0; i < Obstacles.Count; i++)
            {
                foreach (var hit in hits)
                {
                    // hit�� ������Ʈ�� ����Ʈ�� ������� �ʾ��� ���̸� ��� Ž��
                    if (Obstacles[i] != hit.collider.gameObject)
                        continue;
                    // ����� ������Ʈ�� ����
                    else
                    {
                        remove = false;
                        break;
                    }
                }

                // ���� ����̸�
                if (remove == true)
                {
                    ObstacleRenderer = Obstacles[i].GetComponent<MeshRenderer>();
                    RestoreMaterial();

                    Obstacles.Remove(Obstacles[i]);
                }
            }
        }

        if (hits.Length > 0)
        {
            // �̹� ����� ������Ʈ���� Ȯ��
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.DrawRay(transform.position, direction * distance, Color.red);

                transparentObj = hits[i].collider.gameObject;

                // �̹� ����� ������Ʈ�̸� ���� ������Ʈ �˻�
                if (Obstacles != null && Obstacles.Contains(transparentObj))
                    continue;

                // ������� ���� ������Ʈ�� ����ȭ �� ����Ʈ�� �߰�
                if (transparentObj.layer == 9)
                    ObstacleRenderer = transparentObj.GetComponent<Renderer>();
                if (ObstacleRenderer != null && transparentObj != null)
                {
                    // ������Ʈ�� �������ϰ� �������Ѵ�
                    Material material = ObstacleRenderer.material;
                    Color matColor = material.color;
                    matColor.a = 0.5f;
                    material.color = matColor;

                    // ����Ʈ�� �߰�
                    Obstacles.Add(transparentObj);
                    ObstacleRenderer = null;
                    transparentObj = null;
                }
            }
        }
    }

    // ���� ����ȭ�� ������Ʈ�� ���󺹱� �ϴ� �޼ҵ�
    void RestoreMaterial()
    {
        Material material = ObstacleRenderer.material;
        Color matColor = material.color;
        matColor.a = 1f;    // ���İ� 1:������(���󺹱�)
        material.color = matColor;

        ObstacleRenderer = null;
    }
}
