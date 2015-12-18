using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class StockController : MonoBehaviour {
    
    public int NbPackage = 50;
    public int MaxStockWeight = 200;
    int MaxPackageWeight = 50;

    public Dictionary<int,int> Packages;   // Liste de paquet et poid correspondant
    public Dictionary<int, GameObject> Cubes;   // Liste de paquet et poid correspondant

    static int countCube = 0;

    Vector3 v;

    // Use this for initialization
    void Start()
    {
        int RestWeight = MaxStockWeight;
        Packages = new Dictionary<int, int>();
        Cubes = new Dictionary<int, GameObject>();

        for (int i = 0; i < NbPackage - 1; i++)   //Instanciation des poids de chaque paquet
        {
            if (RestWeight <= NbPackage - 1 - i)
            {
                RestWeight = 0;
                Packages.Add(i,1);
                CreateCube(i,1);
            }
            else
            {
                
                Packages.Add(i,Random.Range(1, MaxPackageWeight)) ;
                RestWeight -= Packages[i];
                CreateCube(i, Packages[i]);
            }

        }
        if (RestWeight != 0)
        {
            if(RestWeight>30)
            {
                Packages.Add(NbPackage,30);
                CreateCube(NbPackage, 30);
            }
            else
            {
                Packages.Add(NbPackage, RestWeight);
                CreateCube(NbPackage, RestWeight);
            }
        }
        else
        {
            Packages.Add(NbPackage, 1);
            CreateCube(NbPackage, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateCube(int i, int Poids)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>();
        cube.GetComponent<Rigidbody>().mass = Poids;

        v = new Vector3(Random.Range(-15, 15), Random.Range(1,12), Random.Range(-15, 15));
        cube.GetComponent<Rigidbody>().position = v;
        Cubes.Add(i,cube);
    }

    public int SelectPackage(int maxWeight)
    {
        // on retourne le paquet au poids le plus faible
        if (countCube>Packages.Count)
            return -1;
        return countCube++;
    }

    public GameObject GetPackage(int i)
    {
        return Cubes[i];
    }

    public void RemovePackage(int i)
    {
        Packages.Remove(i);
        Cubes.Remove(i);
    }
}

