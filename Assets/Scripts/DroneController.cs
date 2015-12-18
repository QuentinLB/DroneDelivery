using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DroneController : MonoBehaviour {

    public enum DroneStates
    {
        Delivery,
        Return,
        Searching,
        Waiting,
        End
    }

    public int id;
    public DroneStates state;
    public float maxSpeed;
    public float minSpeed;
    public Rigidbody rb;
    public int maxWeight;
    public bool isInTeam = false;

    public FlockController flock;
    public DeliveryController deliveryController;
    public Vector3 deliveryPosition;
    public StockController stockController;
    public Vector3 stockPosition;

    public int packageTargetId = -1;
    public GameObject packageTarget;

    public List<int> helpNeeded = new List<int>();

    private float distMin = 4;

  //  public GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
	
    // Use this for initialization
	void Start () {
        stockPosition = stockController.GetComponent<Rigidbody>().transform.position;
        deliveryPosition = deliveryController.GetComponent<Rigidbody>().transform.position;
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
                MoveTo(packageTarget.transform.position);
                break;
            case DroneStates.Delivery:
                Delivery();
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
        Vector3 dist = packageTarget.transform.position - transform.position;
        if (dist.magnitude < distMin)
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
        Vector3 dist = stockPosition - transform.position;
        if (dist.magnitude < distMin)
        {
            state = DroneStates.Searching;

            //rb.velocity = Vector3.zero;
            bool res = flock.BroadcastMessage(id, "ArrivedInStock");
            if (res)
            {
                if(helpNeeded.Count>0)
                {
                    //helpNeeded.Sort((a, b) => a.CompareTo(b));
                    packageTargetId = helpNeeded[UnityEngine.Random.Range(0,helpNeeded.Count-1)];
                    packageTarget = stockController.GetPackage(packageTargetId);
                    flock.JoinDeliveryTeam(id, packageTargetId);
                    isInTeam = true;
                }
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
        {
            MoveTo(stockPosition);
        }

    }

   

    void Delivery()
    {
        Vector3 dist = deliveryPosition - packageTarget.GetComponent<Rigidbody>().transform.position;
        if (dist.magnitude < distMin)
        {
            stockController.RemovePackage(packageTargetId);
            rb.velocity = Vector3.zero;
            packageTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
            state = DroneStates.Return;
        }
        else
        {
            MoveWithBox(deliveryPosition, packageTarget);
        }
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 force = direction.normalized * maxSpeed * ((direction.magnitude < distMin) ? -1 : 1);
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 2);
        rb.velocity = force;
        Vector3 pos = rb.transform.position;
        pos.y = 3;
        rb.transform.position = pos;
    }

    void MoveWithBox(Vector3 target, GameObject box)
    {
        Rigidbody box_rb = box.GetComponent<Rigidbody>();
        box_rb.mass = rb.mass;
        Vector3 direction = target - transform.position;
        Vector3 force = direction.normalized * maxSpeed * ((direction.magnitude < distMin) ? -1 : 1);
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 2);
        rb.velocity = force;
        box_rb.velocity = force;

        Vector3 pos = rb.transform.position;
        pos.y = 3;
        rb.transform.position = pos;

        pos = box_rb.transform.position;
        pos.y = 2;
        box_rb.transform.position = pos;
    }

    public bool GetMessage(int from, string message)
    {
        if (message == "ArrivedInStock" && state != DroneStates.Waiting)
        {
            return false;
        }
        else if(message == "ArrivedInStock" && state == DroneStates.Waiting)
        {
            flock.SendMessage(id, from, "RequestingHelp;" + packageTargetId);
            return true;
        }
        else if(message.StartsWith("RequestingHelp", StringComparison.InvariantCulture) && state == DroneStates.Searching)
        {
            int boxId = int.Parse(message.Split(';')[1]);
            if (!helpNeeded.Contains(boxId))
                helpNeeded.Add(boxId);
            return true;
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
