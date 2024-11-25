using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricityPuzzelLogic : MonoBehaviour
{
    [SerializeField] GameObject[] tileFieldInput;
    private GameObject[,] tileField = new GameObject[5, 3];
    [SerializeField] private GameObject[] differentTileTypes;
    [SerializeField] private ElectricityPuzzelTileTypeConnections differentTileTypeConnections;


    private void Awake()
    {
        PuzzleTileRotator.tileWasUpdated += OnFieldGotUpdate;
    }
    private void OnDestroy()
    {
        PuzzleTileRotator.tileWasUpdated -= OnFieldGotUpdate;
    }

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
                if (index < tileFieldInput.Length)
                {
                    tileField[row, col] = tileFieldInput[index];
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
            string newName;

            int rndValue = rnd.Next(differentTileTypes.Length);

            newRandomMesh = differentTileTypes[rndValue].GetComponent<MeshFilter>().sharedMesh;
            newName = differentTileTypes[rndValue].name;

            if (tile == tileField[4, 2])
            {
                newRandomMesh = differentTileTypes[4].GetComponent<MeshFilter>().sharedMesh;
                newName = differentTileTypes[4].name;
            }

            tile.GetComponent<MeshFilter>().mesh = newRandomMesh;
            tile.name = newName;
        }
    }

    private List<GameObject> activeTiles = new List<GameObject>();

    public void OnFieldGotUpdate(GameObject _updatedTile, int _curRotation)
    {
        GameObject startTile = tileField[0, 0];
        GameObject endTile = tileField[4, 2];
        if (_updatedTile == startTile)
        {
            int rotSteps = GetRotationSteps(_curRotation);
            directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            if (relativeDirections.Contains(directions.U))
            {
                _updatedTile.transform.localPosition = new Vector3(0.03f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);
                activeTiles.Add(_updatedTile);
            }
            else
            {
                _updatedTile.transform.localPosition = new Vector3(0.01029964f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);
                activeTiles.Remove(_updatedTile);
            }
        }
        else if (_updatedTile == endTile)
        {
            int rotSteps = GetRotationSteps(_curRotation);
        }
        else
        {
            List<GameObject> neighbours = GetAllNeighbours(_updatedTile);

            foreach(GameObject neighbour in neighbours)
            {
                if(activeTiles.Contains(neighbour))
                {
                    //liegt output von neighbour an _updatedTile?
                    //Überprüfung wie bei 0,0 an neighbour zu _updatedTile
                    //Überprüfung wie bei 0,0 an _updatedTile zu neighbour
                    //wenn ja setz auf activeTiles
                }
            }
        }
    }
    #region Neighbours
    private List<GameObject> GetAllNeighbours(GameObject updatedTile)
    {
        List<GameObject> neighbours = new List<GameObject>();

        int row = -1, col = -1;

        for (int r = 0; r < tileField.GetLength(0); r++)
        {
            for (int c = 0; c < tileField.GetLength(1); c++)
            {
                if (tileField[r, c] == updatedTile)
                {
                    row = r;
                    col = c;
                    break;
                }
            }
        }

        if (row == -1 || col == -1)
            return neighbours;

        bool isOddCol = col % 2 != 0;

        CheckAndAddNeighbour(row - 1, col, neighbours);  
        
        CheckAndAddNeighbour(isOddCol ? row-1 : row ,  col +1 , neighbours);
        CheckAndAddNeighbour(isOddCol ? row  : row+ 1,   col  +1  , neighbours);  
        
        CheckAndAddNeighbour(row + 1, col, neighbours);  
        
        CheckAndAddNeighbour(isOddCol ? row  : row+ 1, col-1, neighbours);              
        CheckAndAddNeighbour(isOddCol ? row - 1: row , col -1, neighbours);

        return neighbours;
    }

    private void CheckAndAddNeighbour(int neighborRow, int neighborCol, List<GameObject> neighbours)
    {
        if (neighborRow >= 0 && neighborRow < tileField.GetLength(0) &&
            neighborCol >= 0 && neighborCol < tileField.GetLength(1))
        {
            GameObject neighborTile = tileField[neighborRow, neighborCol];
            if (neighborTile != null)
            {
                neighbours.Add(neighborTile);
            }
        }
    }
    #endregion

    #region Directions&Rotations
    private directions[] ConvertDefaultConnectionIntoRotationRelative(directions[] _directions, int _rotSteps)
    {
        directions[] relativeDirections = new directions[_directions.Length];

        int arrIndex = 0;
        foreach (directions direction in _directions)
        {
            int curIndex = (int)direction;
            int newIndex = curIndex + _rotSteps;

            if (newIndex > 5)
                newIndex -= 6;

            relativeDirections[arrIndex] = (directions)newIndex;
            arrIndex++;
        }

        return relativeDirections;
    }

    private int GetRotationSteps(float _rotation)
    {
        int steps = 0;
        steps = (int)(_rotation / 60);
        return steps;
    }
    private directions[] GetConnectionsForTileType(GameObject _tile)
    {
        string tileType = _tile.name;
        switch (tileType)
        {
            case "TileType1":
                return differentTileTypeConnections.possibleConnectionsType1;
            case "TileType2":
                return differentTileTypeConnections.possibleConnectionsType2;
            case "TileType3":
                return differentTileTypeConnections.possibleConnectionsType3;
            case "TileType4":
                return differentTileTypeConnections.possibleConnectionsType4;
            case "TileType5":
                return differentTileTypeConnections.possibleConnectionsType5;
            default:
                return null;
        }
    }
    #endregion
    private void PuzzleSolved()
    {
        Debug.Log("Puzzle Gelöst!");
    }

    #region old code
    private void CheckFromTile(GameObject _tileToCheckFrom)
    {
        int row = -1, col = -1;

        // Suche nach der Position im Array
        for (int r = 0; r < tileField.GetLength(0); r++)
        {
            for (int c = 0; c < tileField.GetLength(1); c++)
            {
                if (tileField[r, c] == _tileToCheckFrom)
                {
                    row = r;
                    col = c;
                    break;
                }
            }
        }

        if (row == -1 || col == -1) return; // Tile nicht gefunden

        // Füge das aktuelle Tile zur aktiven Liste hinzu
        if (!activeTiles.Contains(_tileToCheckFrom))
            activeTiles.Add(_tileToCheckFrom);

        // Prüfe Nachbarn für Hexagone
        bool isOddRow = row % 2 != 0;

        CheckNeighbor(row - 1, col, _tileToCheckFrom, directions.D, directions.U);              // U (oben)
        CheckNeighbor(row - 1, isOddRow ? col + 1 : col - 1, _tileToCheckFrom, directions.RD, directions.LU); // RU
        CheckNeighbor(row, col + 1, _tileToCheckFrom, directions.LD, directions.RU);           // Rechts
        CheckNeighbor(row + 1, isOddRow ? col + 1 : col - 1, _tileToCheckFrom, directions.LU, directions.RD); // RD
        CheckNeighbor(row + 1, col, _tileToCheckFrom, directions.U, directions.D);             // Unten
        CheckNeighbor(row, col - 1, _tileToCheckFrom, directions.RU, directions.LD);           // Links
    }

    private void CheckNeighbor(int neighborRow, int neighborCol, GameObject currentTile, directions myDirection, directions neighborDirection)
    {
        if (neighborRow < 0 || neighborRow >= tileField.GetLength(0) || neighborCol < 0 || neighborCol >= tileField.GetLength(1))
            return;

        GameObject neighborTile = tileField[neighborRow, neighborCol];

        // Hole die TileType-Namen aus den GameObject-Namen
        string currentTileType = currentTile.name;
        string neighborTileType = neighborTile.name;

    }

    // Methode, um die Verbindungen eines TileTypes aus dem ScriptableObject zu holen

    #endregion


}
