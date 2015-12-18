using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockController : MonoBehaviour {
    public DroneController prefab;
    public int drone_number;
    public Transform stockPosition;

    List<DroneController> drones = new List<DroneController>();

    List<DroneController> dronesInStock = new List<DroneController>();

    // Use this for initialization
    void Start () {
        for (int i = 0; i < drone_number; i++)
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
        drones.Add(drone);
        // You can also acccess other components / scripts of the clone
        //droneClone.GetComponent<droneController>().DoSomething();
    }

    public bool BroadcastMessage(int id, string message)
    {
        foreach(DroneController drone in drones)
        {
            if (drone.id != id)
            {
                return drone.GetMessage(id, message);
            }
        }
        return true;
    }


    public void SendMessage(int from, int to, string message)
    {
        drones[to].GetMessage(from, message);
    }

    public void NotifyArrivalInStockArea(DroneController drone)
    {
        dronesInStock.Add(drone);
    }
}
