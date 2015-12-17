using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// these define the flock's behavior
/// </summary>
public class FlockController : MonoBehaviour
{
	public float minVelocity = 5;
	public float maxVelocity = 20;
	public float randomness = 1;
	public int flockSize = 20;
	public DroneController_new prefab;
	public Transform target;

	internal Vector3 flockCenter;
	internal Vector3 flockVelocity;

	List<DroneController_new> drones = new List<DroneController_new>();

	void Start()
	{
		for (int i = 0; i < flockSize; i++)
		{
			DroneController_new drone = Instantiate(prefab, transform.position, transform.rotation) as DroneController_new;
			drone.transform.parent = transform;
			drone.transform.localPosition = new Vector3(
							Random.value * GetComponent<Collider>().bounds.size.x,
							Random.value * GetComponent<Collider>().bounds.size.y,
							Random.value * GetComponent<Collider>().bounds.size.z) - GetComponent<Collider>().bounds.extents;
			drone.controller = this;
			drones.Add(drone);
		}
	}

	void Update()
	{
		Vector3 center = Vector3.zero;
		Vector3 velocity = Vector3.zero;
		foreach (DroneController_new drone in drones)
		{
			center += drone.transform.localPosition;
			velocity += drone.GetComponent<Rigidbody>().velocity;
		}
		flockCenter = center / flockSize;
		flockVelocity = velocity / flockSize;
	}
}