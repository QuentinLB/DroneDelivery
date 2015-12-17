using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneSpawner : MonoBehaviour {
    public DroneController_old prefab;
    public int drone_number;
    public Transform stockPosition;

    List<DroneController_old> drones = new List<DroneController_old>();

    // Use this for initialization
    void Start () {
        for (int i = 0; i < drone_number; i++)
        {
            SpawnDrone();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnDrone()
    {
        DroneController_old drone = (DroneController_old)Instantiate(prefab, transform.position, transform.rotation);
       	List<DroneController_old> drones = new List<DroneController_old>();
        drone.transform.parent = transform;
        drone.transform.localPosition = new Vector3(
                        Random.value * GetComponent<Collider>().bounds.size.x,
                        Random.value * GetComponent<Collider>().bounds.size.y,
                        Random.value * GetComponent<Collider>().bounds.size.z) - GetComponent<Collider>().bounds.extents;
        drone.flock = this;
        drone.stockPosition = stockPosition;
        drones.Add(drone);
        // You can also acccess other components / scripts of the clone
        //droneClone.GetComponent<droneController>().DoSomething();
    }
}
