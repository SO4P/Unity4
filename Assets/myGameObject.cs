using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myGameObject : MonoBehaviour, ISceneController, IUserAction
{
    public SSActionManager actionManager { get; set; }
    SSDirector director;
    public DiskFactory disk;
    public int count;
    public int level;
    public int number;

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
