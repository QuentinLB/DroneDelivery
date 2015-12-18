using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class StockController : MonoBehaviour {
    
    public int NbPackage;
    public int MaxStockWeight;
    public List<int> Packages;   // Liste de paquet et poid correspondant
    public List<GameObject> Cubes = new List<GameObject>();   // Liste de paquet et poid correspondant

    static int countCube = 0;

    Vector3 v;

    // Use this for initialization
    void Start()
    {
        int RestWeight = MaxStockWeight;
        Packages = new List<int>(NbPackage);

        int MaxPackageWeight = 50;

        for (int i = 0; i < NbPackage - 1; i++)   //Instanciation des poids de chaque paquet
        {
            if (RestWeight <= NbPackage - 1 - i)
            {
                RestWeight = 0;
                Packages.Add(1);
                CreateCube(1);
            }
            else
            {
                
                Packages.Add(Random.Range(1, MaxPackageWeight)) ;
                RestWeight -= Packages[i];
                CreateCube(Packages[i]);
            }

        }
        if (RestWeight != 0)
        {
            if(RestWeight>30)
            {
                Packages.Add(30);
                CreateCube(30);
            }
            else
            {
                Packages.Add(RestWeight);
                CreateCube(RestWeight);
            }
        }
        else
        {
            Packages.Add(1);
            CreateCube(1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateCube(int Poids)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Rigidbody>().mass = Poids;

        v = new Vector3(Random.Range(-15, 15), Random.Range(1,12), Random.Range(-15, 15));
        cube.GetComponent<Rigidbody>().position = v;
        Cubes.Add(cube);
    }

    public int SelectPackage(int maxWeight)
    {
        // on retourne le paquet au poids le plus faible
        if (Packages.Count < 1)
            return -1;
        return Packages.IndexOf(Packages.Min());
    }

    public GameObject GetPackage(int i)
    {
        return Cubes[i];
    }

    public void RemovePackage(int i)
    {
        Packages.RemoveAt(i);
        Cubes.RemoveAt(i);
    }
}

