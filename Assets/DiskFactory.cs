using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
