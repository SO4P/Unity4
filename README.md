# 编写一个简单的鼠标打飞碟（Hit UFO）游戏
+ 规则：
    + 每个回合得分超过10即为胜利，超过5分可能会出现多个飞碟
    + 游戏共三种难度，每种难度可能出现的飞碟种类不同
    + 飞碟分三种颜色：
        + 绿色，1分
        + 黄色，2分
        + 红色，3分
+ 游戏说明：
    + 使用了带缓存的工厂模式管理飞碟的生产与回收，该工厂采用单例模式
    + 采用动作管理模式
+ UML类图：
![UML](https://github.com/SO4P/Unity4/blob/master/4.1.png)
+ 代码：
    + 动作的基类和事件回调接口：
  
            public enum SSActionEventType : int { Started, Competeted }

            public interface ISSActionCallback
            {
                void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
                int intParam = 0, string strParam = null, Object objectParam = null);
            }

            public class SSAction : ScriptableObject
            {
                public bool enable = true;
                public bool destroy = false;

                public GameObject gameobject { get; set; }
                public Transform transform { get; set; }
                public ISSActionCallback callback { get; set; }

                public virtual void Start()
                {
                    throw new System.NotImplementedException();
                }

                public virtual void Update()
                {
                    throw new System.NotImplementedException();
                }
            }
    + 具体动作：
  
            public class CCFly : SSAction {
                public myGameObject sceneController;
                private bool move = true;
                public disk Disk;

                public static CCFly GetSSAction()
                {
                    CCFly action = ScriptableObject.CreateInstance<CCFly>();
                    return action;
                }
	            
                // Use this for initialization
	            public override void Start () {
                    sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
                    if (this.gameobject != null)
                    Disk = sceneController.disk.findDisk(this.transform.name);
	            }
	
	            // Update is called once per frame
	            public override void Update () {
                    if (move)
                    {
                        if (this.gameobject == null)
                            move = false;
                        else if (this.transform.position != Disk.direction)
                            this.transform.position = Vector3.MoveTowards(this.transform.position, Disk.direction, Disk.speed * Time.deltaTime);
                        else
                            move = false;
                    }
                    if (!move)
                    {
                        if(this.gameobject != null)
                            sceneController.disk.destroy(this.transform.name);
                        this.destroy = true;
                        this.callback.SSActionEvent(this);
                    }
	            }
            }
    + 动作管理者基类

            public class SSActionManager : MonoBehaviour
            {
                private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
                private List<SSAction> waitingAdd = new List<SSAction>();
                private List<int> waitingDelete = new List<int>();

                // Use this for initialization
                void Start()
                {

                }

                // Update is called once per frame
                protected void Update()
                {
                    foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
                    waitingAdd.Clear();

                    foreach (KeyValuePair<int, SSAction> kv in actions)
                    {
                        SSAction ac = kv.Value;
                        if (ac.destroy)
                        {
                            waitingDelete.Add(ac.GetInstanceID());
                        }
                        else if (ac.enable)
                        {
                            ac.Update();
                        }
                    }

                    foreach (int key in waitingDelete)
                    {
                        SSAction ac = actions[key]; actions.Remove(key); DestroyObject(ac);
                    }
                    waitingDelete.Clear();
                }

                public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
                {
                    action.gameobject = gameobject;
                    action.transform = gameobject.transform;
                    action.callback = manager;
                    waitingAdd.Add(action);
                    action.Start();
                }
            }
    + 管理具体动作派生类

            public class CCActionManager : SSActionManager, ISSActionCallback
            {
                public myGameObject sceneController;
                public CCFly fly;
                private float timer = 5;
                // Use this for initialization
                void Start () {
                    sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
                    sceneController.actionManager = this;
                }
	
	            // Update is called once per frame
	            protected new void Update () {
                    if (sceneController.count < 10)
                    {
                        if (timer != 0)
                        {
                            timer -= Time.deltaTime;
                            if (timer < 0)
                            {
                                timer = Random.Range(3 - sceneController.level,6 - sceneController.level);
                                if (sceneController.count < 5)
                                {
                                    fly = CCFly.GetSSAction();
                                    sceneController.number++;
                                    this.RunAction(sceneController.disk.GetDisk().Disk, fly, this);
                                }
                                else
                                {
                                    int number = Random.Range(1, 3);
                                    for(;number > 0; number--)
                                    {
                                        fly = CCFly.GetSSAction();
                                        sceneController.number++;
                                        this.RunAction(sceneController.disk.GetDisk().Disk, fly, this);
                                    }
                                }
                            }
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform.tag == "Disk")
                                {
                                    sceneController.score(hit.transform.name);
                                    sceneController.disk.destroy(hit.transform.name);
                                }
                            }
                        }
                        base.Update();
                    }
                }

                public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,int intParam = 0, string strParam = null, Object objectParam = null)
                {
                    //  
                }
            }
    + GameObject管理类

            public class myGameObject : MonoBehaviour, ISceneController, IUserAction
            {
                public SSActionManager actionManager { get; set; }
                SSDirector director;
                public DiskFactory disk;  //飞碟工厂
                public int count;  //击中飞碟数
                public int level; //当前关卡
                public int number; //射出飞碟数

                public void loadResources()
                {
                    disk = DiskFactory.getInstance();
                    count = 0;
                    level = 0;
                    number = 0;
                }

                public void reStart()
                {
                    count = 0;
                    number = 0;
                    disk.round = level;
                    disk.reStart();
                }

                void Awake()
                {
                    director = SSDirector.getInstance();
                    director.currentSceneController = this;
                    loadResources();
                }
                void OnGUI()
                {
                    GUI.Label(new Rect(Screen.width - 150, 0, 100, 50), "current level:");
                    GUI.Label(new Rect(Screen.width - 50, 0, 100, 50), (level + 1).ToString());
                    GUI.Label(new Rect(Screen.width - 150, 60, 100, 50),"disk counter:");
                    GUI.Label(new Rect(Screen.width - 50, 60, 100, 50), number.ToString());
                    GUI.Label(new Rect(Screen.width - 150, 120, 100, 50), "current counter:");
                    GUI.Label(new Rect(Screen.width - 50, 120, 100, 50), count.ToString());
                    if (count >= 10)
                    {
                        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 30, 100, 50), "You Win!");
                        if (level < 2)
                        {
                            if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 50), "Next level"))
                            {
                                level++;
				reStart();
                            }
                        }
                        disk.reStart();
                    }
                    if (GUI.Button(new Rect(Screen.width - 100, 180, 60, 60), "Restart"))
                    {
                        reStart();
                    }
                }
		
		public void score(string name)
    		{
        		if (disk.findDisk(name).color == Color.green)
            			count++;
        		else if (disk.findDisk(name).color == Color.yellow)
            			count += 2;
        		else if (disk.findDisk(name).color == Color.red)
            			count += 3;
    		}
            }
    + 飞碟

            public class disk{
                private float[] Speed = { 5f, 10f, 15f };

                private Vector3[] start = {new Vector3(-10,-7.5f,0),
                              new Vector3(10,-7.5f,0),
                              new Vector3(-5,-7.5f,0),
                              new Vector3(5,-7.5f,0)};
                private Vector3[] end = {new Vector3(-10,7.5f,0),
                            new Vector3(10,7.5f,0),
                            new Vector3(-5,7.5f,0),
                            new Vector3(5,7.5f,0)};

                public Vector3 position;  //初始位置
                public Color color;  //飞碟颜色
                public float speed;  //飞行速度
                public Vector3 direction;  //目标位置
                public GameObject Disk;
		public string name;

                public disk(Color color)   //green,yellow,red
                {
                    this.color = color;
                    if (color == Color.green)
                        speed = Speed[0];
                    else if (color == Color.yellow)
                        speed = Speed[1];
                    else
                        speed = Speed[2];
                    create();
                }

                private void refresh()  //更新飞碟飞行路径
                {
                    int side1 = Random.Range(0, 4);
                    bool equal1 = false;
                    if (position == start[side1])
                        equal1 = true;
                    position = start[side1];
                    int side2 = side1;
                    while (side2 == side1 && !(equal1 && direction == end[side2]))
                    {
                        side2 = Random.Range(0, 4);
                    }
                    direction = end[side2];
                }

                public void create()
                {
                    refresh();
                    Disk = GameObject.Instantiate(Resources.Load("Perfabs/Disk", typeof(GameObject)), position, Quaternion.identity, null) as GameObject;
                    Disk.GetComponent<MeshRenderer>().material.color = this.color;
		    Disk.name = name;
                }
            }
    + 飞碟工厂

            public class DiskFactory{

                private static DiskFactory instance;
                public static DiskFactory getInstance()
                {
                    if(instance == null)
                    {
                        instance = new DiskFactory();
                    }
                    return instance;
                }

                private Color[] colors = { Color.green, Color.yellow, Color.red };

                public List<disk> used = new List<disk>();  //正在使用的飞碟
                public List<disk> free = new List<disk>();  //空闲飞碟
                public int round = 0;
                public disk selectedDisk;
                private disk usedDisk;
                private int no;  //飞碟编号

                private DiskFactory()
                {
                    round = 0;
                    no = 0;
                }

	            public disk GetDisk()
                {
                    int color = Random.Range(0, round + 1);
                    if(free.Count == 0)
                    {
                        if (used.Count == 0)
                        {
                            disk temp = new disk(colors[0]);
                            used.Add(temp);
                        }
                        else
                        {
                            disk temp = new disk(colors[color]);
                            used.Add(temp);
                        }
                    }
                    else
                    {
                        for(int i = 0;i < free.Count; i++)
                        {
                            if (free[i].color == colors[color])
                            {
                                use(i);
                                return usedDisk;
                            }
                        }
                        disk temp = new disk(colors[color]);
                        used.Add(temp);
                    }
                    used[used.Count - 1].name = no.ToString();
                    no++;
                    selectedDisk = used[used.Count - 1];
                    return used[used.Count - 1];
                }

                public disk findDisk(string name)
                {
                    for(int i = 0;i < used.Count; i++)
                    {
                        if (used[i].Disk.name == name)
                            return used[i];
                    }
                    return null;
                }

                public void destroy(string name)
                {
                    for(int i = 0;i < used.Count; i++)
                    {
                        if (used[i].Disk.name == name)
                            selectedDisk = used[i];
                    }
                    store();
                    GameObject.Destroy(selectedDisk.Disk);
                }

                public void reStart()
                {
                    for(int i = 0;i < used.Count; i++)
                    {
                        destroy(used[i].Disk.name);
                    }
                }

                private void use(int i)
                {
                    free[i].create();
                    used.Add(free[i]);
                    usedDisk = free[i];
                    free.Remove(free[i]);
                }

                private void store()
                {
                    free.Add(selectedDisk);
                    used.Remove(selectedDisk);
                }
            }
