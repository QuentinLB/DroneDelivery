using UnityEngine;
using System.Collections;

public class DroneController_old : MonoBehaviour {

    enum DroneStates
    {
        Delivery,
        Return,
        Searching,
        Waiting
    }

    public DroneSpawner flock;

    private DroneStates state;

    public Transform stockPosition;

    public float maxSpeed;

    public Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        state = DroneStates.Return;    
	}
	
	void FixedUpdate () {
        switch (state)
        {
            case DroneStates.Return:
                MoveTowards(stockPosition.position);
                break;
            default:
                break;
        }
	}

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 force = direction.normalized * maxSpeed;
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 2);
        rb.velocity = force;
    }
}
