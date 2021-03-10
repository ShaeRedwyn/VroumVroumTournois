using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Requirement : MonoBehaviour, IComparable<Requirement>
{
    // Start is called before the first frame update

    public List<int> listOfInt;
    public int[] arrayOfInt;

    public int[][] jaggedArray2dOfInt;

    public int[][][] jaggedArray3dOfInt;

    //permet de comparer les "value".
    public int value;

    //permet de comparer les ints "value" entre elles. Ce qui permet de trier, ici du plus grand au plus petit.
    public int CompareTo(Requirement other) 
    {
        if(value < other.value)
        {
            return 1;
        }

        if(value > other.value)
        {
            return -1;
        }

        return 0;

    }
    

    void Start()
    {
        //TestList();
        //TestArray();
        //TestJaggedArray2D();
        //TestJaggedArray3D();

        TestRaycast();
    }
    void TestList()
    {
        //Reset une liste vierge;
        listOfInt = new List<int>();

        //Permet d'ajouter une variable à la liste.
        listOfInt.Add(18);

        //Créer une int et l'ajouter.
        int myInt = 123;
        listOfInt.Add(myInt);

        //Enlever une int à l'index 0. Les autres vont rétrograder.
        listOfInt.RemoveAt(0);

        listOfInt = new List<int>();

        listOfInt.Add(4);
        listOfInt.Add(2);
        listOfInt.Add(1);
        listOfInt.Add(3);


        listOfInt.Sort();

        for (int i = 0; i < listOfInt.Count; i++)
        {
            Debug.Log(listOfInt[i]);
        }
        //Debug la derniere valeur du tableau
        Debug.Log(listOfInt.Count - 1);

        
    }
   
    private void TestArray()
    {
        //Préciser la taille quand on créer l'array.

        arrayOfInt = new int[4];
        /*
        arrayOfInt[0] = 3;
        arrayOfInt[1] = 0;
        arrayOfInt[2] = 2;
        arrayOfInt[3] = 1;
        */
        arrayOfInt = new int[] { 3, 0, 2, 1 };

        for (int i = 0; i < arrayOfInt.Length; i++)
        {
            Debug.Log(arrayOfInt[i]);
        }
    }

    private void TestJaggedArray2D()
    {
        //Déclarer le nombre de colonnes (on gère chaque chose à la fois).
        jaggedArray2dOfInt = new int[4][];

        //déclarer le nombre de cases dans chaque tableau (ici la colonne 0 à 3 cases);
        jaggedArray2dOfInt[0] = new int[4];

        //Je continue de développer mon tableau.
        jaggedArray2dOfInt[1] = new int[2];
        jaggedArray2dOfInt[2] = new int[4];
        jaggedArray2dOfInt[3] = new int[2];

        jaggedArray2dOfInt = new int[4][];

        int count = 1;

        //Dans une jaggedArray, le Lenght est égal au nombre de Colonnes.
        for (int x = 0; x < jaggedArray2dOfInt.Length; x++)
        {
            jaggedArray2dOfInt[x] = new int[x + 1];

            //Dans ma colonne x, j'assigne à chaque case, la valeur y+1
            for (int y = 0; y < jaggedArray2dOfInt[x].Length; y++)
            {
                jaggedArray2dOfInt[x][y] = count;
                count++;
                Debug.Log(jaggedArray2dOfInt[x][y]);
            }

        }

    }

    private void TestJaggedArray3D()
    {
        //Créer un jagged Array à 3 Dimensions à 4 colonnes.. 
        jaggedArray3dOfInt = new int[4][][];

        //pour chaque colonne (X), j'attribue 4 cases. (2 Dimensions)  (Colonnes)
        for (int x = 0; x < jaggedArray3dOfInt.Length; x++)  
        {
            jaggedArray3dOfInt[x] = new int[4][];

            //Pour chaque Colonne (X), j'attribue 4 cases (Y). (3 Dimensions) (Cases) 
            for (int y = 0; y < jaggedArray3dOfInt[x].Length; y++)
            {
                jaggedArray3dOfInt[x][y] = new int[4];

                //J'attribue à chaque cases la valeur 3.(Valeur dans chaque cases)
                for (int z = 0; z < jaggedArray3dOfInt[x][y].Length; z++)
                {
                    jaggedArray3dOfInt[x][y][z] = 3;
                }
            }
        }
    }

    //Permet de dire au raycast de toucher qu'un seul type de raycast.
    public LayerMask layerMask;

    private void TestRaycast()
    { 
        //Paramètres du Raycast.
        Vector3 origine = Vector3.zero;
        Vector3 direction = Vector3.up;
        float length = 2f;

        //Quest ce que je touche.
        RaycastHit hit;

        //Test pour voir si mon raycast touche.
        if (Physics.Raycast(origine,direction,out hit, length,layerMask))
        {
            //Hit.distance permet de stoper le raycast quand il touche pour ne pas qu'il traverse.
            Debug.DrawRay(origine, direction*hit.distance,Color.green);
        }
        else //Je ne touche pas.
        {
            //Afficher un raycast pour le débug (MANDATORY).
            Debug.DrawRay(origine, direction * length, Color.red);
        }
        



    }
}
