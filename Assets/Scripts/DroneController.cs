using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneController : MonoBehaviour {

    enum DroneStates
    {
        Delivery,
        Return,
        Searching,
        Waiting
    }

    public FlockController flock;

    private DroneStates state;

    public Transform stockPosition;

    public float maxSpeed;

    public Rigidbody rb;

    public StockController stockController;

    public int id;

    public int maxWeight;

    public GameObject packageTarget;


    public GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        state = DroneStates.Return;  
        
   	}
	
	void FixedUpdate () {
        switch (state)
        {
            case DroneStates.Return:
                ReturnToStock();
                break;
            case DroneStates.Searching:
                SearchPackage();
                break;
            default:
                break;
        }
    }

    void SearchPackage()
    {
        if ((packageTarget.transform.position - transform.position).magnitude < 2)
        {

        }
        else
        {
            MoveTo(packageTarget.transform.position);
        }
    }

    void ReturnToStock()
    {
        if((stockPosition.position - transform.position).magnitude < 2)
        {
            //rb.velocity = Vector3.zero;
            state = DroneStates.Searching;
            if(flock.BroadcastMessage(id,"ArrivedInStock"))
            {

            }
            else
            {
                packageTarget = stockController.SelectPackage(maxWeight);
            }
        }
        else
            MoveTo(stockPosition.position);
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 force = direction.normalized * maxSpeed;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 2);
        rb.velocity = force;
    }

    public void SoloTransport(Vector3 target, GameObject cube)
    {
        Vector3 direction = target - transform.position;
        Vector3 force = direction.normalized * maxSpeed;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 2);
        rb.velocity = force;
        cube.GetComponent<Rigidbody>().velocity = force;
        
    }


    public bool GetMessage(int from, string message)
    {
        if (message == "ArrivedInStock" && state != DroneStates.Waiting)
        {
            return false;
        }
        else if(message == "ArrivedInStock" && state == DroneStates.Waiting)
        {
            flock.SendMessage(id, from, "RequestingHelp");
            return true;
        }
        else if(message == "RequestingHelp" && state == DroneStates.Searching)
        {

        }
        return false;
    }
}
