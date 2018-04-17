using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
