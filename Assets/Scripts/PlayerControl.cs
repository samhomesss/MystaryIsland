using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public static float MOVE_AREA_RADIUS = 25.0f; // 섬의 반지름.
    public static float MOVE_SPEED = 7.0f; // 이동 속도. 

    private struct Key
    { // 키 조작 정보 구조체.
        public bool up; // ↑.
        public bool down; // ↓.
        public bool right; // →.
        public bool left; // ←.
        public bool pick; // 줍는다／버린다.
        public bool action; // 먹는다 / 수리한다.
        public bool action2;  // 불을 살린다 / 사과를 심는다.
    };

    private Key key; // 키 조작 정보를 보관하는 변수.

    public enum STEP
    { // 플레이어의 상태를 나타내는 열거체.
        NONE = -1, // 상태 정보 없음.
        MOVE = 0, // 이동 중.
        REPAIRING, // 수리 중.
        EATING, // 식사 중.
        MAKINGFIRE, // 불 살리는중
        PLANTING,  // 나무 심는중
        MINING,
        CASTING,   // 주조중
        FELLING,   // 벌목중
        TABLING,
        NUM, // 상태가 몇 종류 있는지 나타낸다(=3).
    };

    public STEP step = STEP.NONE; // 현재 상태.
    public STEP next_step = STEP.NONE; // 다음 상태.
    public float step_timer = 0.0f; // 타이머.

    private GameObject mainCamera;

    private GameObject closest_item = null; // 플레이어의 정면에 있는 GameObject.
    private GameObject carried_item = null; // 플레이어가 들어올린 GameObject.
    private ItemRoot item_root = null; // ItemRoot 스크립트를 가짐.
    public GUIStyle guistyle; // 폰트 스타일.

    private GameObject closest_event = null;// 주목하고 있는 이벤트를 저장.
    private EventRoot event_root = null;    // EventRoot 클래스를 사용하기 위한 변수.
    private GameObject rocket_model = null; // 우주선의 모델을 사용하기 위한 변수.
    private GameObject bonfire = null; // 모닥불의 모델을 사용하기 위한 변수.

    private GameStatus game_status = null;
    public float bonfireDistance;

    private Weather game_status_weather = null;
    private MakingTable makingTable;
    private GameObject workTable;
    private bool usingWorkTable = false;
    bool tableAction = false;

    ParticleSystem rain;
    AudioSource audioSource;
    BGMControl bGMControl;
    Animator animator;
    bool s = true;
    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0.5f;
        bGMControl = GameObject.Find("SoundManager").GetComponent<BGMControl>();
        bGMControl.GetComponent<AudioSource>().playOnAwake = false;
        bGMControl.GetComponent<AudioSource>().loop = false;
        animator = GetComponentInChildren<Animator>();
        rain = GameObject.Find("Rain").GetComponent<ParticleSystem>();

        this.step = STEP.NONE; // 현 단계 상태를 초기화.
        this.next_step = STEP.MOVE; // 다음 단계 상태를 초기화.	

        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.guistyle.fontSize = 32;
        this.event_root = GameObject.Find("GameRoot").GetComponent<EventRoot>();
        this.rocket_model = GameObject.Find("Ship").transform.Find("Ship_model").gameObject;
        this.game_status = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        this.bonfire = GameObject.Find("Bonfire");
        this.game_status_weather = game_status.GetWeather();
        workTable = GameObject.Find("workTable");
        makingTable = GameObject.FindObjectOfType<MakingTable>().GetComponent<MakingTable>();
    }

    // Update is called once per frame
    void Update()
    {
        tableAction = makingTable.TableAction();
        this.get_input(); // 입력 정보 취득. 

        this.step_timer += Time.deltaTime;
        float eat_time = 0.5f; // 사과는 2초에 걸쳐 먹는다.
        float repair_time = 0.5f; // 수리에 걸리는 시간도 2초.
        float makeFire_time = 0.5f; // 땔감 넣는 시간도 2초.
        float plante_time = 0.5f; // 땔감 넣는 시간도 2초.
        float mining_time = 0.5f;
        float casting_time = 0.5f;
        float felling_time = 0.5f;
        //float tabling_time = 0.5f;

        // 상태를 변화시킨다---------------------.
        if (this.next_step == STEP.NONE)
        { // 다음 예정이 없으면.
            switch (this.step)
            {
                case STEP.MOVE: // '이동 중' 상태의 처리.
                    do
                    {
                        if (!this.key.action && !this.key.action2)
                        { // 액션 키가 눌려있지 않다.
                            break; // 루프 탈출.
                        }

                        // 주목하는 이벤트가 있을 때.
                        if (this.closest_event != null)
                        {
                            //Debug.Log(this.is_event_ignitable());
                            if (!this.is_event_ignitable())
                            { // 이벤트를 시작할 수 없으면.
                                break; // 아무 것도 하지 않는다.
                            }
                            // 이벤트 종류를 가져온다.
                            Event.TYPE ignitable_event = this.event_root.getEventType(this.closest_event);

                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    // 이벤트의 종류가 ROCKET이면.
                                    // REPAIRING(수리) 상태로 이행.
                                    if (!this.key.action && this.key.action2) break; // 루프 탈출.
                                    this.next_step = STEP.REPAIRING;
                                    break;
                                case Event.TYPE.BONFIRE:
                                    if (this.key.action && !this.key.action2) break; // 루프 탈출.
                                    this.next_step = STEP.MAKINGFIRE;
                                    break;
                                case Event.TYPE.MINING:
                                    if (!this.key.action && this.key.action2) break; // 루프 탈출.
                                    this.next_step = STEP.MINING;
                                    break;
                                case Event.TYPE.CASTING:  // 철 주조
                                    if (this.key.action && !this.key.action2) break; // 루프 탈출.
                                    this.next_step = STEP.CASTING;
                                    break;
                                case Event.TYPE.FELLING:  // 벌목
                                    if (this.key.action && !this.key.action2) break; // 루프 탈출.
                                    this.next_step = STEP.FELLING;
                                    break;
                            }
                            break;
                        }

                        if (this.carried_item != null)
                        {
                            // 가지고 있는 아이템 판별.
                            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);

                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE: // 사과라면.
                                    if (this.key.action && !this.key.action2)
                                    {
                                        this.next_step = STEP.EATING;
                                        break; // 루프 탈출.
                                    }
                                    if (!this.key.action && this.key.action2)
                                    {
                                        this.next_step = STEP.PLANTING;
                                        break; // 루프 탈출.
                                    }
                                    break;
                                case Item.TYPE.PLANT: // 식물이라면.
                                    // '식사 중' 상태로 이행.
                                    if (this.key.action && !this.key.action2)
                                    {
                                        this.next_step = STEP.EATING;
                                        break;
                                    }
                                    if (!this.key.action && this.key.action2)
                                    {
                                        this.next_step = STEP.MAKINGFIRE;
                                        break;
                                    }
                                    break;
                                case Item.TYPE.HANDSAW:
                                    if (this.key.action && !this.key.action2) break;
                                    this.next_step = STEP.FELLING;
                                    break;
                                case Item.TYPE.IRON:
                                    if (!this.key.action && this.key.action2) break;
                                    this.next_step = STEP.CASTING;
                                    break;
                                case Item.TYPE.LUMBER:
                                    if (this.key.action && !this.key.action2) break;
                                    this.next_step = STEP.REPAIRING;
                                    break;
                                case Item.TYPE.PICKAXE:
                                    if (this.key.action && !this.key.action2) break;
                                    this.next_step = STEP.MINING;
                                    break;
                                case Item.TYPE.SMELTEDIRON:
                                    if (this.key.action && !this.key.action2) break;
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING: // '식사 중' 상태의 처리.
                    if (this.step_timer > eat_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;

                case STEP.REPAIRING: // '수리 중' 상태의 처리.
                    if (this.step_timer > repair_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
                case STEP.MAKINGFIRE: // 불 살리는중 
                    if (this.step_timer > makeFire_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
                case STEP.PLANTING: // 나무 심는중 
                    if (this.step_timer > plante_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
                case STEP.MINING: // 광석 캐는중
                    if (this.step_timer > mining_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
                case STEP.CASTING: // 광석 캐는중
                    if (this.step_timer > casting_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
                case STEP.FELLING: // 벌목중
                    if (this.step_timer > felling_time)
                    { // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
            }
        }
        // 상태가 변화했을 때------------.
        while (this.next_step != STEP.NONE)
        { // 상태가 NONE이외 = 상태가 변화했다.
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.MOVE:
                    break;

                case STEP.EATING: // '식사 중' 상태의 처리.
                    if (this.carried_item != null)
                    {
                        // 들고 있는 아이템의 '체력 회복 정도'를 가져와서 설정.
                        this.game_status.addSatiety(this.item_root.getRegainSatiety(this.carried_item));

                        // 가지고 있던 아이템을 폐기.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;

                case STEP.REPAIRING: // '수리 중'이 되면.
                    if (this.carried_item != null)
                    {
                        if(carried_item.gameObject.CompareTag("Hammer"))
                        {
                            this.game_status.isSetHammer();
                        }
                        else
                        // 들고 있는 아이템의 '수리 진척 상태'를 가져와서 설정.
                            this.game_status.addRepairment(this.item_root.getGainRepairment(this.carried_item));

                        // 가지고 있는 아이템 삭제.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;
                case STEP.MAKINGFIRE:  // 땔감 넣는중이면.
                    if (this.carried_item != null)
                    {
                        // 들고 있는 아이템의 '모닥불 hp 상태'를 가져와서 설정.
                        this.game_status.addTemperature(this.item_root.getRegainTemperature(this.carried_item));

                        // 가지고 있는 아이템 삭제.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;
                case STEP.PLANTING:  // 심는중이면.
                    if (this.carried_item != null)
                    {
                        // 나무 생성
                        this.item_root.CreatTree(carried_item);

                        // 가지고 있는 아이템 삭제.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;
                case STEP.MINING: // 광석 캐는중이 되면
                    bGMControl.PlayBGM("MiningSound");
                    bGMControl.GetComponent<AudioSource>().volume = 0.1f;
                    bGMControl.GetComponent<AudioSource>().Play();
                    this.item_root.creatIron();
                    break;
                case STEP.CASTING: // 화로 사용중이 되면
                    if (this.carried_item != null)
                    {
                        // 가지고 있는 아이템 삭제.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                        this.item_root.creatSmeltedIron();
                    }
                    break;
                case STEP.FELLING: // 벌목중이 되면
                    if(closest_event != null)
                        this.item_root.getTree().fellingTree(closest_event);
                    break;

            }
            this.step_timer = 0.0f;
        }

        // 각 상황에서 반복할 것----------.
        switch (this.step)
        {
            case STEP.MOVE:
                this.move_control();
                this.pick_or_drop_control();
                this.game_status.alwaysSatiety();  // 이동 가능한 경우는 항상 배가 고파진다.
                this.game_status.alwaysBodyTemperature();
                this.bonfireDIstane();
                break;
            case STEP.REPAIRING:
                // 우주선을 회전시킨다.
               this.rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);               
         
                break;
        }

        switch (game_status_weather.GetWeatherState())
        {
            case Weather.WeatherState.SUNNY:
            case Weather.WeatherState.CLOUDY:
                rain.Stop();
                MOVE_SPEED = 7.0f;
                break;
            case Weather.WeatherState.RAINY:
                MOVE_SPEED = 4.0f;
                if (rain.isPlaying == false)
                    rain.Play();
                break;
            default:
                break;
        }

        game_status.regulateBonfire();
    }

    private void get_input()
    {
        this.key.up = false;
        this.key.down = false;
        this.key.right = false;
        this.key.left = false;
        // ↑키가 눌렸으면 true를 대입.
        this.key.up |= Input.GetKey(KeyCode.UpArrow);
        this.key.up |= Input.GetKey(KeyCode.Keypad8);
        // ↓키가 눌렸으면 true를 대입.
        this.key.down |= Input.GetKey(KeyCode.DownArrow);
        this.key.down |= Input.GetKey(KeyCode.Keypad2);
        // →키가 눌렸으면 true를 대입.
        this.key.right |= Input.GetKey(KeyCode.RightArrow);
        this.key.right |= Input.GetKey(KeyCode.Keypad6);
        // ←키가 눌렸으면 true를 대입..
        this.key.left |= Input.GetKey(KeyCode.LeftArrow);
        this.key.left |= Input.GetKey(KeyCode.Keypad4);
        // Z 키가 눌렸으면 true를 대입.

        this.key.pick = Input.GetKeyDown(KeyCode.Q);
        // X 키가 눌렸으면 true를 대입.
        this.key.action = Input.GetKeyDown(KeyCode.W);
        // C 키가 눌렸으면 true를 대입.
        this.key.action2 = Input.GetKeyDown(KeyCode.E);
    }

    private void move_control()
    {
        Vector3 move_vector = Vector3.zero; // 이동용 벡터.
        Vector3 position = this.transform.position; // 현재 위치를 보관.
        bool is_moved = false;

        if (this.key.right)
        { // →키가 눌렸으면.
            move_vector += Vector3.right; // 이동용 벡터를 오른쪽으로 향한다.
            is_moved = true; // '이동 중' 플래그. 
        }

        if (this.key.left)
        {
            move_vector += Vector3.left;
            is_moved = true;
        }

        if (this.key.up)
        {
            move_vector += Vector3.forward;
            is_moved = true;
        }

        if (this.key.down)
        {
            move_vector += Vector3.back;
            is_moved = true;
        }

        move_vector.Normalize(); // 길이를 1로.
        move_vector *= MOVE_SPEED * Time.deltaTime; // 속도×시간＝거리.
        position += move_vector; // 위치를 이동.
        position.y = 0.0f; // 높이를 0으로 한다.

        // 세계의 중앙에서 갱신한 위치까지의 거리가 섬의 반지름보다 크면.
        if (position.magnitude > MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= MOVE_AREA_RADIUS; // 위치를 섬의 끝자락에 머물게 한다.
        }

        // 새로 구한 위치(position)의 높이를 현재 높이로 되돌린다.
        position.y = this.transform.position.y;
        // 실제 위치를 새로 구한 위치로 변경한다.
        this.transform.position = position;
        // 이동 벡터의 길이가 0.01보다 큰 경우.
        // =어느 정도 이상의 이동한 경우.

        if (move_vector.magnitude > 0.01f)
        {
            // 캐릭터의 방향을 천천히 바꾼다.
            Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.2f);
        }

        if (is_moved)
        {
            animator.SetBool("Walking", true);
            audioSource.volume = 0.35f;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            animator.SetBool("Walking", false);
            audioSource.Stop();
        }
        
    }

    void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;

        // 트리거의 GameObject 레이어 설정이 Item이라면.
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            // 아무 것도 주목하고 있지 않으면.
            if (this.closest_item == null)
            {
                if (this.is_other_in_view(other_go))
                { // 정면에 있으면.
                    this.closest_item = other_go; // 주목한다.                    
                }
                // 뭔가 주목하고 있으면.
            }
            else if (this.closest_item == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // 정면에 없으면.
                    this.closest_item = null; // 주목을 그만둔다.
                }
            }
        }
        // 트리거의 GameObject의 레이어 설정이 Event라면.
        else if (other_go.layer == LayerMask.NameToLayer("Event"))
        {
            // 아무것도 주목하고 있지 않으면.
            if (this.closest_event == null)
            {
                if (this.is_other_in_view(other_go))
                {  // 정면에 있으면.
                    this.closest_event = other_go;      // 주목한다.
                }
                // 뭔가에 주목하고 있으면.
            }
            else if (this.closest_event == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // 정면에 없으면.
                    this.closest_event = null;          // 주목을 그만둔다.
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null; // 주목을 그만둔다.
        }
        if (this.closest_event == other.gameObject)
        {
            this.closest_event = null; // 주목을 그만둔다.
        }
    }

    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            { // '줍기/버리기'키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.carried_item == null)
            { // 들고 있는 아이템이 없고.
                if (this.closest_item == null)
                { // 주목 중인 아이템이 없으면.
                    break; // 아무것도 하지 않고 메소드 종료.
                }
                // 주목 중인 아이템을 들어올린다.
                this.carried_item = this.closest_item;
                // 들고 있는 아이템을 자신의 자식으로 설정.
                this.carried_item.transform.parent = this.transform;

                CarriedPosition(this.carried_item);

                
                bGMControl.PlayBGM("ItemSound");
                bGMControl.GetComponent<AudioSource>().volume = 0.8f;
                bGMControl.GetComponent<AudioSource>().Play();

                bool addItem = InventoryManager.instance.AddItem(carried_item);
                if (addItem)
                {
                    //this.carried_item = null;
                }

                this.closest_item = null;
            }
            else
            { // 들고 있는 아이템이 있을 경우.
                if (this.carried_item.CompareTag("HandSaw") || this.carried_item.CompareTag("PickAxe"))
                {
                    Vector3 pos = transform.position;
                    this.carried_item.transform.localPosition = Vector3.up * 0.1f + Vector3.forward * 1.2f;
                    this.carried_item.transform.localEulerAngles = new Vector3(0, 90, 0);
                }
                else if(this.carried_item.CompareTag("Hammer"))
                {
                    Vector3 pos = transform.position;
                    this.carried_item.transform.localPosition = Vector3.up * 0.1f + Vector3.forward * 1.2f;
                    this.carried_item.transform.localEulerAngles = new Vector3(90, 90, 0);
                }
                else if(this.carried_item.CompareTag("Plant"))
                {
                    this.carried_item.transform.localPosition = Vector3.up * 0.3f + Vector3.forward * 1.0f;  // 들고 있는 아이템을 약간(1.0f) 앞으로 이동시켜서.
                }
                else
                    this.carried_item.transform.localPosition = Vector3.forward * 1.0f;  // 들고 있는 아이템을 약간(1.0f) 앞으로 이동시켜서.

                this.carried_item.transform.parent = null; // 자식 설정을 해제.
                this.carried_item = null; // 들고 있던 아이템을 없앤다.
                InventoryManager.instance.GetSelectedItem(carried_item);
            }
        } while (false);
    }

    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = this.transform.TransformDirection(Vector3.forward);  // 자신이 현재 향하고 있는 방향을 보관.
            Vector3 to_other = other.transform.position - this.transform.position;  // 자신 쪽에서 본 아이템의 방향을 보관.
            heading.y = 0.0f;
            to_other.y = 0.0f;
            heading.Normalize(); // 길이를 1로 하고 방향만 벡터로.
            to_other.Normalize(); // 길이를 1로 하고 방향만 벡터로.
            float dp = Vector3.Dot(heading, to_other); // 양쪽 벡터의 내적을 취득.
            if (dp < Mathf.Cos(30.0f))
            { // 내적이 45도인 코사인 값 미만이면.
                break; // 루프를 빠져나간다.
            }
            ret = true; // 내적이 45도인 코사인 값 이상이면 정면에 있다.
        } while (false);
        return (ret);
    }

    private bool is_event_ignitable()
    {
        bool ret = false;

        if (this.closest_event == null)
        { // 주목 이벤트가 없으면.
            return ret; // false를 반환한다.
        }
        //Debug.Log(carried_item);

        // 들고 있는 아이템 종류를 가져온다.
        Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);

       

        //Debug.Log(carried_item_type);
        // 들고 있는 아이템 종류와 주목하는 이벤트의 종류에서.
        // 이벤트가 가능한지 판정하고, 이벤트 불가라면 false를 반환한다.
        if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
        {
            return ret;
        }

        if (carried_item_type == Item.TYPE.HAMMER && game_status.GetRepairment() < 1.0f)
        {
            return ret;
        }

        ret = true; // 여기까지 오면 이벤트를 시작할 수 있다고 판정!.

        return ret;
    }

    void OnGUI()
    {
        float x = 20.0f;
        float y = Screen.height - 100.0f;

        float fontWidth = 200.0f;
        float fontHeight = 20.0f;

        if (this.carried_item != null && !tableAction)
        {
            GUI.Label(new Rect(x, y, fontWidth, fontHeight), "Q:버린다", guistyle);
            do
            {
                if (this.is_event_ignitable())
                {
                    break;
                }
                if (item_root.getItemType(this.carried_item) == Item.TYPE.ROCK || item_root.getItemType(this.carried_item) == Item.TYPE.HAMMER
                    || item_root.getItemType(this.carried_item) == Item.TYPE.PICKAXE || item_root.getItemType(this.carried_item) == Item.TYPE.HANDSAW
                    || item_root.getItemType(this.carried_item) == Item.TYPE.IRON || item_root.getItemType(this.carried_item) == Item.TYPE.SMELTEDIRON
                    || item_root.getItemType(this.carried_item) == Item.TYPE.LUMBER)
                {
                    break;
                }
                GUI.Label(new Rect(x + 170.0f, y, fontWidth, fontHeight), "W:먹는다", guistyle);
                if (item_root.getItemType(this.carried_item) == Item.TYPE.APPLE)
                {
                    x += 170;
                    GUI.Label(new Rect(x + 170.0f, y, fontWidth, fontHeight), "E:코코넛을 심는다", guistyle);
                }

            } while (false);
        }
        else
        {
            if (this.closest_item != null)
            {
                GUI.Label(new Rect(x, y, fontWidth, fontHeight), "Q:줍는다", guistyle);
            }
        }

        switch (this.step)
        {
            case STEP.EATING:
                GUI.Label(new Rect(Screen.width / 2 - 80.0f, y, fontWidth, fontHeight), "우걱우걱우물우물……", guistyle);
                break;
            case STEP.REPAIRING:
                GUI.Label(new Rect(Screen.width / 2 - 20.0f, y, fontWidth, fontHeight), "수리중", guistyle);
                break;
            case STEP.MAKINGFIRE:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "땔감넣는중...", guistyle);
                break;
            case STEP.PLANTING:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "나무심는중...", guistyle);
                break;
            case STEP.MINING:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "광석캐는중...", guistyle);
                break;
            case STEP.CASTING:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "철을 만드는중..", guistyle);
                break;
        }
        Debug.Log(this.is_event_ignitable());

        if (this.is_event_ignitable())
        { // 이벤트가 시작 가능한 경우.
            // 이벤트용 메시지를 취득.
            string message = this.event_root.getIgnitableMessage(this.closest_event);

            if (message == "수리한다" || message == "광석을 캔다")
                GUI.Label(new Rect(x + 170.0f, y, fontWidth, fontHeight), "W:" + message, guistyle);
            if (message == "불을 살린다" || message == "철을 제련한다" || message == "벌목한다")
                GUI.Label(new Rect(x + 270.0f, y, fontWidth, fontHeight), "E:" + message, guistyle);
        }
    }

    private void bonfireDIstane()
    {
        bonfireDistance = Vector3.Distance(transform.position, bonfire.transform.position);
        float temperature = game_status.GetTemperature();
        if (bonfireDistance < 5.0f && temperature > 0) game_status.addBodyTemperature();
    }

    public void CarriedPosition(GameObject obj)
    {
        if (obj.CompareTag("Hammer") || obj.CompareTag("HandSaw") || obj.CompareTag("PickAxe"))
        {
            obj.transform.localPosition = new Vector3(0.26f, 0.35f, 0.12f);
            obj.transform.localRotation = Quaternion.Euler(54.6f, -2.5f, -83f);
        }
        else
        {
            obj.transform.localPosition = Vector3.up * 2.4f;  // 2.0f 위에 배치(머리 위로 이동).
        }
            
    }

    public void SetCarredItem(GameObject item)
    {
        carried_item = item;
    }

    public GameObject GetCarredItem()
    {
        return carried_item;
    }

}
