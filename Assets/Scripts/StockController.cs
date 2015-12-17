using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StockController : MonoBehaviour {
    
    public int NbPackage;
    public int MaxStockWeight;
    public List<int> Packages;   // Liste de paquet et poid correspondant

    static int countCube = 0;


    // Use this for initialization
    void Start()
    {
        int RestWeight = MaxStockWeight;
        Packages = new List<int>(NbPackage);

        int MaxPackageWeight = 50;
        for (int i = 0; i < Packages.Count - 1; i++)   //Instanciation des poids de chaque paquet
        {
            if (RestWeight <= Packages.Count - 1 - i)
            {
                RestWeight = 0;
                Packages[i] = 1;
                CreateCube(1);
            }
            else
            {
                Packages[i] = Random.Range(1, 2*(MaxPackageWeight/NbPackage));
                RestWeight -= Packages[i];
                CreateCube(Packages[i]);
            }

        }
        if (RestWeight != 0)
        {
            //Packages[Packages.Count - 1] = RestWeight;
            CreateCube(RestWeight);
        }
        else
        {
            Packages[Packages.Count - 1] = 1;
            CreateCube(1);
        }


    }


    // Update is called once per frame
    void Update()
    {

    }

    void CreateCube(int Poid)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>();
       // cube.GetComponent<Rigidbody>().useGravity = false;
        cube.GetComponent<Rigidbody>().mass = Poid;
        if(countCube % 3 == 0)
        {
            cube.GetComponent<Rigidbody>().position = new Vector3(countCube, 0, 0);
        }
        if(countCube % 3 == 1)
        {
            cube.GetComponent<Rigidbody>().position = new Vector3(0, countCube, 0);
        }
        if(countCube % 3 == 2)
        {
            cube.GetComponent<Rigidbody>().position = new Vector3(countCube, countCube, 0);
        }
      
        countCube++;
    }
}

