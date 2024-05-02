using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public static float MOVE_AREA_RADIUS = 25.0f; // ���� ������.
    public static float MOVE_SPEED = 7.0f; // �̵� �ӵ�. 

    private struct Key
    { // Ű ���� ���� ����ü.
        public bool up; // ��.
        public bool down; // ��.
        public bool right; // ��.
        public bool left; // ��.
        public bool pick; // �ݴ´٣�������.
        public bool action; // �Դ´� / �����Ѵ�.
        public bool action2;  // ���� �츰�� / ����� �ɴ´�.
    };

    private Key key; // Ű ���� ������ �����ϴ� ����.

    public enum STEP
    { // �÷��̾��� ���¸� ��Ÿ���� ����ü.
        NONE = -1, // ���� ���� ����.
        MOVE = 0, // �̵� ��.
        REPAIRING, // ���� ��.
        EATING, // �Ļ� ��.
        MAKINGFIRE, // �� �츮����
        PLANTING,  // ���� �ɴ���
        MINING,
        CASTING,   // ������
        FELLING,   // ������
        TABLING,
        NUM, // ���°� �� ���� �ִ��� ��Ÿ����(=3).
    };

    public STEP step = STEP.NONE; // ���� ����.
    public STEP next_step = STEP.NONE; // ���� ����.
    public float step_timer = 0.0f; // Ÿ�̸�.

    private GameObject mainCamera;

    private GameObject closest_item = null; // �÷��̾��� ���鿡 �ִ� GameObject.
    private GameObject carried_item = null; // �÷��̾ ���ø� GameObject.
    private ItemRoot item_root = null; // ItemRoot ��ũ��Ʈ�� ����.
    public GUIStyle guistyle; // ��Ʈ ��Ÿ��.

    private GameObject closest_event = null;// �ָ��ϰ� �ִ� �̺�Ʈ�� ����.
    private EventRoot event_root = null;    // EventRoot Ŭ������ ����ϱ� ���� ����.
    private GameObject rocket_model = null; // ���ּ��� ���� ����ϱ� ���� ����.
    private GameObject bonfire = null; // ��ں��� ���� ����ϱ� ���� ����.

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

        this.step = STEP.NONE; // �� �ܰ� ���¸� �ʱ�ȭ.
        this.next_step = STEP.MOVE; // ���� �ܰ� ���¸� �ʱ�ȭ.	

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
        this.get_input(); // �Է� ���� ���. 

        this.step_timer += Time.deltaTime;
        float eat_time = 0.5f; // ����� 2�ʿ� ���� �Դ´�.
        float repair_time = 0.5f; // ������ �ɸ��� �ð��� 2��.
        float makeFire_time = 0.5f; // ���� �ִ� �ð��� 2��.
        float plante_time = 0.5f; // ���� �ִ� �ð��� 2��.
        float mining_time = 0.5f;
        float casting_time = 0.5f;
        float felling_time = 0.5f;
        //float tabling_time = 0.5f;

        // ���¸� ��ȭ��Ų��---------------------.
        if (this.next_step == STEP.NONE)
        { // ���� ������ ������.
            switch (this.step)
            {
                case STEP.MOVE: // '�̵� ��' ������ ó��.
                    do
                    {
                        if (!this.key.action && !this.key.action2)
                        { // �׼� Ű�� �������� �ʴ�.
                            break; // ���� Ż��.
                        }

                        // �ָ��ϴ� �̺�Ʈ�� ���� ��.
                        if (this.closest_event != null)
                        {
                            //Debug.Log(this.is_event_ignitable());
                            if (!this.is_event_ignitable())
                            { // �̺�Ʈ�� ������ �� ������.
                                break; // �ƹ� �͵� ���� �ʴ´�.
                            }
                            // �̺�Ʈ ������ �����´�.
                            Event.TYPE ignitable_event = this.event_root.getEventType(this.closest_event);

                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    // �̺�Ʈ�� ������ ROCKET�̸�.
                                    // REPAIRING(����) ���·� ����.
                                    if (!this.key.action && this.key.action2) break; // ���� Ż��.
                                    this.next_step = STEP.REPAIRING;
                                    break;
                                case Event.TYPE.BONFIRE:
                                    if (this.key.action && !this.key.action2) break; // ���� Ż��.
                                    this.next_step = STEP.MAKINGFIRE;
                                    break;
                                case Event.TYPE.MINING:
                                    if (!this.key.action && this.key.action2) break; // ���� Ż��.
                                    this.next_step = STEP.MINING;
                                    break;
                                case Event.TYPE.CASTING:  // ö ����
                                    if (this.key.action && !this.key.action2) break; // ���� Ż��.
                                    this.next_step = STEP.CASTING;
                                    break;
                                case Event.TYPE.FELLING:  // ����
                                    if (this.key.action && !this.key.action2) break; // ���� Ż��.
                                    this.next_step = STEP.FELLING;
                                    break;
                            }
                            break;
                        }

                        if (this.carried_item != null)
                        {
                            // ������ �ִ� ������ �Ǻ�.
                            Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);

                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE: // ������.
                                    if (this.key.action && !this.key.action2)
                                    {
                                        this.next_step = STEP.EATING;
                                        break; // ���� Ż��.
                                    }
                                    if (!this.key.action && this.key.action2)
                                    {
                                        this.next_step = STEP.PLANTING;
                                        break; // ���� Ż��.
                                    }
                                    break;
                                case Item.TYPE.PLANT: // �Ĺ��̶��.
                                    // '�Ļ� ��' ���·� ����.
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

                case STEP.EATING: // '�Ļ� ��' ������ ó��.
                    if (this.step_timer > eat_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;

                case STEP.REPAIRING: // '���� ��' ������ ó��.
                    if (this.step_timer > repair_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
                case STEP.MAKINGFIRE: // �� �츮���� 
                    if (this.step_timer > makeFire_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
                case STEP.PLANTING: // ���� �ɴ��� 
                    if (this.step_timer > plante_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
                case STEP.MINING: // ���� ĳ����
                    if (this.step_timer > mining_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
                case STEP.CASTING: // ���� ĳ����
                    if (this.step_timer > casting_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
                case STEP.FELLING: // ������
                    if (this.step_timer > felling_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
            }
        }
        // ���°� ��ȭ���� ��------------.
        while (this.next_step != STEP.NONE)
        { // ���°� NONE�̿� = ���°� ��ȭ�ߴ�.
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.MOVE:
                    break;

                case STEP.EATING: // '�Ļ� ��' ������ ó��.
                    if (this.carried_item != null)
                    {
                        // ��� �ִ� �������� 'ü�� ȸ�� ����'�� �����ͼ� ����.
                        this.game_status.addSatiety(this.item_root.getRegainSatiety(this.carried_item));

                        // ������ �ִ� �������� ���.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;

                case STEP.REPAIRING: // '���� ��'�� �Ǹ�.
                    if (this.carried_item != null)
                    {
                        if(carried_item.gameObject.CompareTag("Hammer"))
                        {
                            this.game_status.isSetHammer();
                        }
                        else
                        // ��� �ִ� �������� '���� ��ô ����'�� �����ͼ� ����.
                            this.game_status.addRepairment(this.item_root.getGainRepairment(this.carried_item));

                        // ������ �ִ� ������ ����.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;
                case STEP.MAKINGFIRE:  // ���� �ִ����̸�.
                    if (this.carried_item != null)
                    {
                        // ��� �ִ� �������� '��ں� hp ����'�� �����ͼ� ����.
                        this.game_status.addTemperature(this.item_root.getRegainTemperature(this.carried_item));

                        // ������ �ִ� ������ ����.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;
                case STEP.PLANTING:  // �ɴ����̸�.
                    if (this.carried_item != null)
                    {
                        // ���� ����
                        this.item_root.CreatTree(carried_item);

                        // ������ �ִ� ������ ����.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                    }
                    break;
                case STEP.MINING: // ���� ĳ������ �Ǹ�
                    bGMControl.PlayBGM("MiningSound");
                    bGMControl.GetComponent<AudioSource>().volume = 0.1f;
                    bGMControl.GetComponent<AudioSource>().Play();
                    this.item_root.creatIron();
                    break;
                case STEP.CASTING: // ȭ�� ������� �Ǹ�
                    if (this.carried_item != null)
                    {
                        // ������ �ִ� ������ ����.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        InventoryManager.instance.GetSelectedItem(carried_item);
                        this.item_root.creatSmeltedIron();
                    }
                    break;
                case STEP.FELLING: // �������� �Ǹ�
                    if(closest_event != null)
                        this.item_root.getTree().fellingTree(closest_event);
                    break;

            }
            this.step_timer = 0.0f;
        }

        // �� ��Ȳ���� �ݺ��� ��----------.
        switch (this.step)
        {
            case STEP.MOVE:
                this.move_control();
                this.pick_or_drop_control();
                this.game_status.alwaysSatiety();  // �̵� ������ ���� �׻� �谡 ��������.
                this.game_status.alwaysBodyTemperature();
                this.bonfireDIstane();
                break;
            case STEP.REPAIRING:
                // ���ּ��� ȸ����Ų��.
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
        // ��Ű�� �������� true�� ����.
        this.key.up |= Input.GetKey(KeyCode.UpArrow);
        this.key.up |= Input.GetKey(KeyCode.Keypad8);
        // ��Ű�� �������� true�� ����.
        this.key.down |= Input.GetKey(KeyCode.DownArrow);
        this.key.down |= Input.GetKey(KeyCode.Keypad2);
        // ��Ű�� �������� true�� ����.
        this.key.right |= Input.GetKey(KeyCode.RightArrow);
        this.key.right |= Input.GetKey(KeyCode.Keypad6);
        // ��Ű�� �������� true�� ����..
        this.key.left |= Input.GetKey(KeyCode.LeftArrow);
        this.key.left |= Input.GetKey(KeyCode.Keypad4);
        // Z Ű�� �������� true�� ����.

        this.key.pick = Input.GetKeyDown(KeyCode.Q);
        // X Ű�� �������� true�� ����.
        this.key.action = Input.GetKeyDown(KeyCode.W);
        // C Ű�� �������� true�� ����.
        this.key.action2 = Input.GetKeyDown(KeyCode.E);
    }

    private void move_control()
    {
        Vector3 move_vector = Vector3.zero; // �̵��� ����.
        Vector3 position = this.transform.position; // ���� ��ġ�� ����.
        bool is_moved = false;

        if (this.key.right)
        { // ��Ű�� ��������.
            move_vector += Vector3.right; // �̵��� ���͸� ���������� ���Ѵ�.
            is_moved = true; // '�̵� ��' �÷���. 
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

        move_vector.Normalize(); // ���̸� 1��.
        move_vector *= MOVE_SPEED * Time.deltaTime; // �ӵ����ð����Ÿ�.
        position += move_vector; // ��ġ�� �̵�.
        position.y = 0.0f; // ���̸� 0���� �Ѵ�.

        // ������ �߾ӿ��� ������ ��ġ������ �Ÿ��� ���� ���������� ũ��.
        if (position.magnitude > MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= MOVE_AREA_RADIUS; // ��ġ�� ���� ���ڶ��� �ӹ��� �Ѵ�.
        }

        // ���� ���� ��ġ(position)�� ���̸� ���� ���̷� �ǵ�����.
        position.y = this.transform.position.y;
        // ���� ��ġ�� ���� ���� ��ġ�� �����Ѵ�.
        this.transform.position = position;
        // �̵� ������ ���̰� 0.01���� ū ���.
        // =��� ���� �̻��� �̵��� ���.

        if (move_vector.magnitude > 0.01f)
        {
            // ĳ������ ������ õõ�� �ٲ۴�.
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

        // Ʈ������ GameObject ���̾� ������ Item�̶��.
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            // �ƹ� �͵� �ָ��ϰ� ���� ������.
            if (this.closest_item == null)
            {
                if (this.is_other_in_view(other_go))
                { // ���鿡 ������.
                    this.closest_item = other_go; // �ָ��Ѵ�.                    
                }
                // ���� �ָ��ϰ� ������.
            }
            else if (this.closest_item == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // ���鿡 ������.
                    this.closest_item = null; // �ָ��� �׸��д�.
                }
            }
        }
        // Ʈ������ GameObject�� ���̾� ������ Event���.
        else if (other_go.layer == LayerMask.NameToLayer("Event"))
        {
            // �ƹ��͵� �ָ��ϰ� ���� ������.
            if (this.closest_event == null)
            {
                if (this.is_other_in_view(other_go))
                {  // ���鿡 ������.
                    this.closest_event = other_go;      // �ָ��Ѵ�.
                }
                // ������ �ָ��ϰ� ������.
            }
            else if (this.closest_event == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // ���鿡 ������.
                    this.closest_event = null;          // �ָ��� �׸��д�.
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null; // �ָ��� �׸��д�.
        }
        if (this.closest_event == other.gameObject)
        {
            this.closest_event = null; // �ָ��� �׸��д�.
        }
    }

    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            { // '�ݱ�/������'Ű�� ������ �ʾ�����.
                break; // �ƹ��͵� ���� �ʰ� �޼ҵ� ����.
            }
            if (this.carried_item == null)
            { // ��� �ִ� �������� ����.
                if (this.closest_item == null)
                { // �ָ� ���� �������� ������.
                    break; // �ƹ��͵� ���� �ʰ� �޼ҵ� ����.
                }
                // �ָ� ���� �������� ���ø���.
                this.carried_item = this.closest_item;
                // ��� �ִ� �������� �ڽ��� �ڽ����� ����.
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
            { // ��� �ִ� �������� ���� ���.
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
                    this.carried_item.transform.localPosition = Vector3.up * 0.3f + Vector3.forward * 1.0f;  // ��� �ִ� �������� �ణ(1.0f) ������ �̵����Ѽ�.
                }
                else
                    this.carried_item.transform.localPosition = Vector3.forward * 1.0f;  // ��� �ִ� �������� �ణ(1.0f) ������ �̵����Ѽ�.

                this.carried_item.transform.parent = null; // �ڽ� ������ ����.
                this.carried_item = null; // ��� �ִ� �������� ���ش�.
                InventoryManager.instance.GetSelectedItem(carried_item);
            }
        } while (false);
    }

    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = this.transform.TransformDirection(Vector3.forward);  // �ڽ��� ���� ���ϰ� �ִ� ������ ����.
            Vector3 to_other = other.transform.position - this.transform.position;  // �ڽ� �ʿ��� �� �������� ������ ����.
            heading.y = 0.0f;
            to_other.y = 0.0f;
            heading.Normalize(); // ���̸� 1�� �ϰ� ���⸸ ���ͷ�.
            to_other.Normalize(); // ���̸� 1�� �ϰ� ���⸸ ���ͷ�.
            float dp = Vector3.Dot(heading, to_other); // ���� ������ ������ ���.
            if (dp < Mathf.Cos(30.0f))
            { // ������ 45���� �ڻ��� �� �̸��̸�.
                break; // ������ ����������.
            }
            ret = true; // ������ 45���� �ڻ��� �� �̻��̸� ���鿡 �ִ�.
        } while (false);
        return (ret);
    }

    private bool is_event_ignitable()
    {
        bool ret = false;

        if (this.closest_event == null)
        { // �ָ� �̺�Ʈ�� ������.
            return ret; // false�� ��ȯ�Ѵ�.
        }
        //Debug.Log(carried_item);

        // ��� �ִ� ������ ������ �����´�.
        Item.TYPE carried_item_type = this.item_root.getItemType(this.carried_item);

       

        //Debug.Log(carried_item_type);
        // ��� �ִ� ������ ������ �ָ��ϴ� �̺�Ʈ�� ��������.
        // �̺�Ʈ�� �������� �����ϰ�, �̺�Ʈ �Ұ���� false�� ��ȯ�Ѵ�.
        if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
        {
            return ret;
        }

        if (carried_item_type == Item.TYPE.HAMMER && game_status.GetRepairment() < 1.0f)
        {
            return ret;
        }

        ret = true; // ������� ���� �̺�Ʈ�� ������ �� �ִٰ� ����!.

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
            GUI.Label(new Rect(x, y, fontWidth, fontHeight), "Q:������", guistyle);
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
                GUI.Label(new Rect(x + 170.0f, y, fontWidth, fontHeight), "W:�Դ´�", guistyle);
                if (item_root.getItemType(this.carried_item) == Item.TYPE.APPLE)
                {
                    x += 170;
                    GUI.Label(new Rect(x + 170.0f, y, fontWidth, fontHeight), "E:���ڳ��� �ɴ´�", guistyle);
                }

            } while (false);
        }
        else
        {
            if (this.closest_item != null)
            {
                GUI.Label(new Rect(x, y, fontWidth, fontHeight), "Q:�ݴ´�", guistyle);
            }
        }

        switch (this.step)
        {
            case STEP.EATING:
                GUI.Label(new Rect(Screen.width / 2 - 80.0f, y, fontWidth, fontHeight), "��ƿ�ƿ칰�칰����", guistyle);
                break;
            case STEP.REPAIRING:
                GUI.Label(new Rect(Screen.width / 2 - 20.0f, y, fontWidth, fontHeight), "������", guistyle);
                break;
            case STEP.MAKINGFIRE:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "�����ִ���...", guistyle);
                break;
            case STEP.PLANTING:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "�����ɴ���...", guistyle);
                break;
            case STEP.MINING:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "����ĳ����...", guistyle);
                break;
            case STEP.CASTING:
                GUI.Label(new Rect(Screen.width / 2 - 50.0f, y, fontWidth, fontHeight), "ö�� �������..", guistyle);
                break;
        }
        Debug.Log(this.is_event_ignitable());

        if (this.is_event_ignitable())
        { // �̺�Ʈ�� ���� ������ ���.
            // �̺�Ʈ�� �޽����� ���.
            string message = this.event_root.getIgnitableMessage(this.closest_event);

            if (message == "�����Ѵ�" || message == "������ ĵ��")
                GUI.Label(new Rect(x + 170.0f, y, fontWidth, fontHeight), "W:" + message, guistyle);
            if (message == "���� �츰��" || message == "ö�� �����Ѵ�" || message == "�����Ѵ�")
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
            obj.transform.localPosition = Vector3.up * 2.4f;  // 2.0f ���� ��ġ(�Ӹ� ���� �̵�).
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
