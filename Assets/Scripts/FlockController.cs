using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockController : MonoBehaviour {
    public DroneController prefab;
    public int droneNumber;
    public float maxSpeed;
    public int maxWeight;

    public Transform stockPosition;

    List<DroneController> drones = new List<DroneController>();

    List<DroneController> dronesInStock = new List<DroneController>();

    Dictionary<int, List<int>> deliveryTeams = new Dictionary<int, List<int>>();

    // Use this for initialization
    void Start () {
        for (int i = 0; i < droneNumber; i++)
        {
            SpawnDrone(i);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnDrone(int i)
    {
        DroneController drone = (DroneController)Instantiate(prefab, transform.position, transform.rotation);
       	List<DroneController> drones = new List<DroneController>();
        drone.transform.parent = transform;
        drone.transform.localPosition = new Vector3(
                        Random.value * GetComponent<Collider>().bounds.size.x,
                        Random.value * GetComponent<Collider>().bounds.size.y,
                        Random.value * GetComponent<Collider>().bounds.size.z) - GetComponent<Collider>().bounds.extents;
        drone.flock = this;
        drone.stockPosition = stockPosition;
        drone.id = i;
        drone.maxSpeed = maxSpeed;
        drone.maxWeight = maxWeight;
        drones.Add(drone);
    }

    public bool BroadcastMessage(int id, string message)
    {
        bool result = false;
        foreach(DroneController drone in drones)
        {
            if (drone.id != id)
            {
                if(drone.GetMessage(id, message))
                {
                    result = true;
                }
            }
        }
        return result;
    }


    public bool SendMessage(int from, int to, string message)
    {
        return drones[to].GetMessage(from, message);
    }

    public void NotifyArrivalInStockArea(DroneController drone)
    {
        dronesInStock.Add(drone);
    }

    public void CreateNewDeliveryTeam(int droneId, int packageId)
    {
        deliveryTeams.Add(packageId, new List<int>(){ droneId});
    }

    public void JoinDeliveryTeam(int droneId, int packageId)
    {
        deliveryTeams[packageId].Add(droneId);
    }

    public List<DroneController> GetDeliveryTeam(int packageId)
    {
        return drones.FindAll(x => deliveryTeams[packageId].Contains(x.id));
    }
}
