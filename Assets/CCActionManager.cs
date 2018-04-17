using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        //  
    }
}
