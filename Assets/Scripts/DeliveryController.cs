using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeliveryController : MonoBehaviour {

    public List<int> DPackages;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnCollisionEnter (Collision col)
    {
        //DPackage.Add(col.gameObject.GetComponent<Rigidbody>().mass);
    }
}
