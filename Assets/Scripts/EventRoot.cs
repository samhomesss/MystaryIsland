using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 처음에 이벤트의 종류를 나타내는 class.
public class Event
{ // 이벤트 종류.
    public enum TYPE
    {
        NONE = -1, // 없음.
        ROCKET = 0, // 우주선 수리.
        MINING,
        CASTING,  // 주조
        BONFIRE,
        FELLING,
        TABLING,
        NUM, // 이벤트가 몇 종류 있는지 나타낸다(=1).
    };
};
public class EventRoot : MonoBehaviour
{
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        { // 인수의 GameObject가 비어있지 않으면.
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
    // 철광석이나 식물을 든 상태에서 우주선에 접촉했는지 확인
    public bool isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
    {
        bool ret = false;
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        {
            type = this.getEventType(event_go); // 이벤트 타입을 구한다.
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:
                // 가지고 있는 것이 철광석이라면.
                if (carried_item == Item.TYPE.SMELTEDIRON)  // 수정해야됨
                {
                    ret = true; // '이벤트할 수 있어요！'라고 응답한다.
                    break;
                }
                if (carried_item == Item.TYPE.LUMBER)  // 수정해야됨
                {
                    ret = true; // '이벤트할 수 있어요！'라고 응답한다.
                    break;
                }
                if (carried_item == Item.TYPE.HAMMER)  
                {
                    ret = true; // '이벤트할 수 있어요！'라고 응답한다.
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
    // 지정된 게임 오브젝트의 이벤트 타입 반환
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
                message = "수리한다";
                break;
            case Event.TYPE.BONFIRE:
                message = "불을 살린다";
                break;
            case Event.TYPE.MINING:
                message = "광석을 캔다";
                break;
            case Event.TYPE.CASTING:
                message = "철을 제련한다";
                break;
            case Event.TYPE.FELLING:
                message = "벌목한다";
                break;
        }
        return (message);
    }
}