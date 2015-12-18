using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DroneController : MonoBehaviour {

    enum DroneStates
    {
        Delivery,
        Return,
        Searching,
        Waiting,
        End
    }

    public FlockController flock;

    private DroneStates state;

    public Transform stockPosition;

    public float maxSpeed;

    public Rigidbody rb;

    public StockController stockController;

    public int id;

    public int maxWeight;

    public int packageTargetId = -1;
    public GameObject packageTarget;

    public bool isInTeam = false;

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
            case DroneStates.Waiting:
                // get wake up on NewArrival broadcast
                break;
            case DroneStates.Delivery:
                //do nothing
                break;
            case DroneStates.End:
                //do nothing
                break;
            default:
                break;
        }
	}

    void SearchPackage()
    {
        if ((packageTarget.transform.position - transform.position).magnitude < 2)
        {
            if(isInTeam)
            {
                int DeliveryTeamCarryingCapacity = GetDeliveryTeamCarryingCapacity();
                float toCarry = packageTarget.GetComponent<Rigidbody>().mass;
                if (toCarry <= DeliveryTeamCarryingCapacity)
                {
                    state = DroneStates.Delivery;
                    flock.GetDeliveryTeam(packageTargetId).ForEach((x) => x.GetMessage(id,"Departure"));
                }
                else
                {
                    state = DroneStates.Waiting;
                }
            }
            else
            {
                if (packageTarget.GetComponent<Rigidbody>().mass <= maxWeight)
                {
                    state = DroneStates.Delivery;
                }
                else
                {
                    flock.CreateNewDeliveryTeam(id, packageTargetId);
                    state = DroneStates.Waiting;
                }
            }
        }
        else
        {
            MoveTo(packageTarget.transform.position);
        }
    }

    void ReturnToStock()
    {
        //reset
        isInTeam = false;
        packageTargetId = -1;
        packageTarget = null;

        if ((stockPosition.position - transform.position).magnitude < 2)
        {
            state = DroneStates.Searching;

            //rb.velocity = Vector3.zero;
            if (flock.BroadcastMessage(id, "ArrivedInStock"))
            {
                packageTarget = stockController.GetPackage(packageTargetId);
                flock.JoinDeliveryTeam(id, packageTargetId);
            } 
            else
            {
                packageTargetId = stockController.SelectPackage(maxWeight);
                if (packageTargetId == -1)
                    state = DroneStates.End;
                else
                {
                    packageTarget = stockController.GetPackage(packageTargetId);
                }
            }
            
        }
        else
            MoveTo(stockPosition.position);
    }

    void MoveTo(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 force = direction.normalized * maxSpeed;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 2);
        rb.velocity = force;
    }

    public bool GetMessage(int from, string message)
    {
        if (message == "ArrivedInStock" && state != DroneStates.Waiting)
        {
            return false;
        }
        else if(message == "ArrivedInStock" && state == DroneStates.Waiting)
        {
            if (flock.SendMessage(id, from, "RequestingHelp;" + packageTargetId))
                return true;
            else
                return false;
        }
        else if(message.StartsWith("RequestingHelp", StringComparison.InvariantCulture) && state == DroneStates.Searching)
        {
            if (!isInTeam)
            {
                packageTargetId = int.Parse(message.Split(';')[1]);
                isInTeam = true;
                return true;
            }
            return false;
        }
        else if (message == "HelpGranted" && state == DroneStates.Waiting)
        {
            if (packageTarget.GetComponent<Rigidbody>().mass <= GetDeliveryTeamCarryingCapacity())
                return false;
            else
                return true;
        }
        else if(message == "Departure" && state == DroneStates.Waiting)
        {
            state = DroneStates.Delivery;
            return true;
        }
        return false;
    }

    int GetDeliveryTeamCarryingCapacity()
    {
        int teamMaxWeight = 0;
        flock.GetDeliveryTeam(packageTargetId).ForEach((x) => teamMaxWeight += x.maxWeight);
        return teamMaxWeight;
    }    

    
}
