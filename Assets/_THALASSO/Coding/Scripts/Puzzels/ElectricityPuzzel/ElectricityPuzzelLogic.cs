using System;
using UnityEngine;

public class ElectricityPuzzelLogic : MonoBehaviour
{
    [SerializeField] GameObject[] tileFieltInput;
    private GameObject[,] tileField = new GameObject[5,3];
    [SerializeField] private GameObject[] differentTileTypes;

    private void Start()
    {
        SortTileInput();
        RandomizeTiles();
    }

    private void SortTileInput()
    {
        int index = 0;
        for (int row = 0; row < tileField.GetLength(0); row++)
        {
            for (int col = 0; col < tileField.GetLength(1); col++) 
            {
                if (index < tileFieltInput.Length)
                {
                    tileField[row, col] = tileFieltInput[index];
                    index++;
                }
            }
        }
    }

    private void RandomizeTiles()
    {
        System.Random rnd = new System.Random();
        foreach (GameObject tile in tileField)
        {
            Mesh newRandomMesh = new Mesh();
            int rndValue = rnd.Next(5);

            newRandomMesh = differentTileTypes[rndValue].GetComponent<MeshFilter>().sharedMesh;

            tile.GetComponent<MeshFilter>().mesh = newRandomMesh;
        }
    }
}
