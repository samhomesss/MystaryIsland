using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ó���� �̺�Ʈ�� ������ ��Ÿ���� class.
public class Event
{ // �̺�Ʈ ����.
    public enum TYPE
    {
        NONE = -1, // ����.
        ROCKET = 0, // ���ּ� ����.
        MINING,
        CASTING,  // ����
        BONFIRE,
        FELLING,
        TABLING,
        NUM, // �̺�Ʈ�� �� ���� �ִ��� ��Ÿ����(=1).
    };
};
public class EventRoot : MonoBehaviour
{
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        { // �μ��� GameObject�� ������� ������.
            switch (event_go.tag)
            {
                case "Rocket":
                    type = Event.TYPE.ROCKET;
                    break;
                case "Bonfire":
                    type = Event.TYPE.BONFIRE;
                    break;
                case "Iron":
                    type = Event.TYPE.MINING;
                    break;
                case "Brazier":
                    type = Event.TYPE.CASTING;
                    break;
                case "Tree":
                    type = Event.TYPE.FELLING;
                    break;
                case "Table":
                    type = Event.TYPE.TABLING;
                    break;
                default:
                    break;
            }
        }
        return (type);
    }
    // ö�����̳� �Ĺ��� �� ���¿��� ���ּ��� �����ߴ��� Ȯ��
    public bool isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
    {
        bool ret = false;
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        {
            type = this.getEventType(event_go); // �̺�Ʈ Ÿ���� ���Ѵ�.
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:
                // ������ �ִ� ���� ö�����̶��.
                if (carried_item == Item.TYPE.SMELTEDIRON)  // �����ؾߵ�
                {
                    ret = true; // '�̺�Ʈ�� �� �־�䣡'��� �����Ѵ�.
                    break;
                }
                if (carried_item == Item.TYPE.LUMBER)  // �����ؾߵ�
                {
                    ret = true; // '�̺�Ʈ�� �� �־�䣡'��� �����Ѵ�.
                    break;
                }
                if (carried_item == Item.TYPE.HAMMER)  
                {
                    ret = true; // '�̺�Ʈ�� �� �־�䣡'��� �����Ѵ�.
                    break;
                }
                break;
            case Event.TYPE.BONFIRE:
                if(carried_item == Item.TYPE.APPLE)
                {
                    ret = true;
                    break;
                }
                if(carried_item == Item.TYPE.PLANT)
                {
                    ret = true;
                    break;
                }
                break;
            case Event.TYPE.MINING:
                if (carried_item == Item.TYPE.PICKAXE)
                {
                    ret = true;
                    break;
                }
                break;
            case Event.TYPE.CASTING:
                if (carried_item == Item.TYPE.IRON)
                {
                    ret = true;
                    break;
                }
                break;
            case Event.TYPE.FELLING:
                if (carried_item == Item.TYPE.HANDSAW)
                {
                    ret = true;
                    break;
                }
                break;
        }
        return (ret);
    }
    // ������ ���� ������Ʈ�� �̺�Ʈ Ÿ�� ��ȯ
    public string getIgnitableMessage(GameObject event_go)
    {
        string message = "";
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        {
            type = this.getEventType(event_go);
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:
                message = "�����Ѵ�";
                break;
            case Event.TYPE.BONFIRE:
                message = "���� �츰��";
                break;
            case Event.TYPE.MINING:
                message = "������ ĵ��";
                break;
            case Event.TYPE.CASTING:
                message = "ö�� �����Ѵ�";
                break;
            case Event.TYPE.FELLING:
                message = "�����Ѵ�";
                break;
        }
        return (message);
    }
}