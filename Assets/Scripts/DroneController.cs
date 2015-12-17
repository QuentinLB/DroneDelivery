using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneController_new : MonoBehaviour
{
	internal FlockController controller;

    enum DroneStates
    {
        Delivery,
        Return,
        Searching,
        Waiting
    }

    IEnumerator Start()
	{
		while (true)
		{
			if (controller)
			{
				GetComponent<Rigidbody>().velocity += steer() * Time.deltaTime;

				// enforce minimum and maximum speeds for the boids
				float speed = GetComponent<Rigidbody>().velocity.magnitude;
				if (speed > controller.maxVelocity)
				{
					GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * controller.maxVelocity;
				}
				else if (speed < controller.minVelocity)
				{
					GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * controller.minVelocity;
				}
			}
            float waitTime = 0.3f;// Random.Range(0.3f, 0.5f);
			yield return new WaitForSeconds(waitTime);
		}
	}

	Vector3 steer()
	{
		Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);
		randomize.Normalize();
        randomize *= 1;// controller.randomness;

		Vector3 center = controller.flockCenter - transform.localPosition;
		Vector3 velocity = controller.flockVelocity - GetComponent<Rigidbody>().velocity;
		Vector3 follow = controller.target.localPosition - transform.localPosition;
        return controller.target.localPosition - transform.localPosition; 
        //return (center + velocity + follow * 10 + randomize);
    }
}