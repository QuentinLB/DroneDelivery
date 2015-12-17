using UnityEngine;
using System.Collections;

public class DroneSpawner : MonoBehaviour {
    public Rigidbody prefab;
    public int drone_number;

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
        Rigidbody droneClone = (Rigidbody)Instantiate(prefab, transform.position, transform.rotation);
        droneClone.velocity = transform.forward;

        // You can also acccess other components / scripts of the clone
        //droneClone.GetComponent<droneController>().DoSomething();
    }
}
