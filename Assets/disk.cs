using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector3 position;
    public Color color;
    public float speed;
    public Vector3 direction;
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

    private void refresh()
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
