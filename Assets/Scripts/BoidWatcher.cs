using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidWatcher : MonoBehaviour
{
	public FlockController boidController;

	void LateUpdate()
	{
		if (boidController)
		{
			transform.LookAt(boidController.flockCenter + boidController.transform.position);
		}
	}
}