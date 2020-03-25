using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public LayerMask clickableLayers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    Vector3 lastClick = new Vector3(100.0f, 100.0f, 100.0f); //test
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lastClick, 0.5f);
    }

    float holdTimerMouseButton0 = 0.0f;
    public float minClickHoldTimeToMove = 0.025f;
    public float maxClickHoldTimeToMove = 0.5f;
    void ProcessInput()
    {
        holdTimerMouseButton0 += Time.deltaTime;

        if(Input.GetMouseButtonDown(0))
        {
           holdTimerMouseButton0 = 0.0f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (holdTimerMouseButton0 >= minClickHoldTimeToMove && holdTimerMouseButton0 <= maxClickHoldTimeToMove)
            {
                Vector3 modMousePos = Input.mousePosition;
                modMousePos.z = 1.0f;
                Vector3 rawClickPos = Camera.main.ScreenToWorldPoint(modMousePos);
            
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 clickDir = rawClickPos - cameraPos;
                Ray ray = new Ray (cameraPos, clickDir);
                RaycastHit hit;
            
                if (Physics.Raycast(ray, out hit, 1000.0f, clickableLayers))
                {
                    //test
                    print ("Hit Name: " + hit.collider.gameObject.name);
                    if (hit.collider.gameObject.GetComponent<Triggerable>() != null && !hit.collider.gameObject.GetComponent<Triggerable>().isTriggered)
                        Player.player.SetTask(hit.collider.gameObject.GetComponent<Triggerable>());
                    else
                        Player.player.SetTask(null);

                    Player.player.PlanAndExecuteMovement(hit.point);
                    //end test
                }
            }
        }
    }

}
