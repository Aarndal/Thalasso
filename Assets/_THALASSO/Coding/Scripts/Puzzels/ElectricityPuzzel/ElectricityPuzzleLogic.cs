using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ElectricityPuzzleLogic : MonoBehaviour
{
    [SerializeField] GameObject[] tileFieldInput;
    [SerializeField] Button[] tileFieldButtonsInput;
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
        OnFieldGotUpdate(tileField[0, 0]);
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
                    tileFieldButtonsInput[index].onClick.AddListener(tileFieldInput[index].GetComponent<PuzzleTileRotator>().OnRotateClick);
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

    private List<Vector2Int> activeTiles = new List<Vector2Int>();
    private List<Vector2Int> updatedInThisLoop = new List<Vector2Int>();

    private GameObject originTile;
    public void OnFieldGotUpdate(GameObject _updatedTile)
    {
        if (originTile == _updatedTile)
            return;

        int updatedTileCurRotation = _updatedTile.GetComponent<PuzzleTileRotator>().curRotation;

        GameObject startTile = tileField[0, 0];
        GameObject endTile = tileField[4, 2];

        if (_updatedTile == startTile)
        {
            int rotSteps = GetRotationSteps(updatedTileCurRotation);
            directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            if (relativeDirections.Contains(directions.U))
            {
                _updatedTile.transform.localPosition = new Vector3(0.03f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);                  //temp visuals

                if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Add(ObjToPos(_updatedTile));
                    updatedInThisLoop.Add(ObjToPos(_updatedTile));
                }

                originTile = _updatedTile;

                List<GameObject> neighbours = GetAllNeighbours(_updatedTile);
                foreach (GameObject neighbour in neighbours)
                    if (!updatedInThisLoop.Contains(ObjToPos(neighbour)))
                        OnFieldGotUpdate(neighbour);
            }
            else
            {
                _updatedTile.transform.localPosition = new Vector3(0.01029964f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);            //temp visuals

                if (activeTiles.Contains(ObjToPos(_updatedTile)))
                    activeTiles.Remove(ObjToPos(_updatedTile));

                updatedInThisLoop.Add(ObjToPos(_updatedTile));
            }
        }
        else if (_updatedTile == endTile)
        {
            int rotSteps = GetRotationSteps(updatedTileCurRotation);
            directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            List<GameObject> neighbours = GetAllNeighbours(_updatedTile);

            foreach (GameObject neighbour in neighbours)
            {
                if (activeTiles.Contains(ObjToPos(neighbour)))
                {

                    if (OriginHasConnectionToTarget(neighbour, _updatedTile) && OriginHasConnectionToTarget(_updatedTile, neighbour) && relativeDirections.Contains(directions.D))
                    {
                        _updatedTile.transform.localPosition = new Vector3(0.03f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);          //temp visuals

                        if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                        {
                            activeTiles.Add(ObjToPos(_updatedTile));
                            updatedInThisLoop.Add(ObjToPos(_updatedTile));
                        }

                        originTile = _updatedTile;

                        foreach (GameObject neighbour2 in neighbours)
                            if (!updatedInThisLoop.Contains(ObjToPos(neighbour2)))
                                OnFieldGotUpdate(neighbour2);

                        break;
                    }

                    else
                    {
                        _updatedTile.transform.localPosition = new Vector3(0.01029964f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);            //temp visuals

                        if (activeTiles.Contains(ObjToPos(_updatedTile)))
                            activeTiles.Remove(ObjToPos(_updatedTile));

                        updatedInThisLoop.Add(ObjToPos(_updatedTile));
                    }
                }
            }
        }
        else
        {
            List<GameObject> neighbours = GetAllNeighbours(_updatedTile);

            foreach (GameObject neighbour in neighbours)
            {
                if (activeTiles.Contains(ObjToPos(neighbour)))
                {

                    if (OriginHasConnectionToTarget(neighbour, _updatedTile) && OriginHasConnectionToTarget(_updatedTile, neighbour))
                    {
                        _updatedTile.transform.localPosition = new Vector3(0.03f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);          //temp visuals

                        if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                        {
                            updatedInThisLoop.Add(ObjToPos(_updatedTile));
                            activeTiles.Add(ObjToPos(_updatedTile));
                        }


                        originTile = _updatedTile;

                        foreach (GameObject neighbour2 in neighbours)
                            if (!updatedInThisLoop.Contains(ObjToPos(neighbour2)))
                                OnFieldGotUpdate(neighbour2);

                        break;
                    }
                    else
                    {
                        _updatedTile.transform.localPosition = new Vector3(0.01029964f, _updatedTile.transform.localPosition.y, _updatedTile.transform.localPosition.z);    //temp visuals

                        if (activeTiles.Contains(ObjToPos(_updatedTile)))
                            activeTiles.Remove(ObjToPos(_updatedTile));

                        updatedInThisLoop.Add(ObjToPos(_updatedTile));
                    }
                }
            }
        }
        if (originTile == _updatedTile)
            updatedInThisLoop.Clear();

        if (activeTiles.Contains(ObjToPos(endTile)))
        {
            PuzzleSolved();
        }
    }


    #region Neighbours
    private List<GameObject> GetAllNeighbours(GameObject _updatedTile)
    {
        List<GameObject> neighbours = new List<GameObject>();

        Vector2Int pos = ObjToPos(_updatedTile);

        bool isOddY = pos.y % 2 == 0;

        CheckAndAddNeighbour(pos.x - 1, pos.y, neighbours);                             // up

        CheckAndAddNeighbour(isOddY ? pos.x : pos.x - 1, pos.y + 1, neighbours);        // right up
        CheckAndAddNeighbour(isOddY ? pos.x + 1 : pos.x, pos.y + 1, neighbours);    // right down

        CheckAndAddNeighbour(pos.x + 1, pos.y, neighbours);                             // down

        CheckAndAddNeighbour(isOddY ? pos.x + 1 : pos.x, pos.y - 1, neighbours);          // left down
        CheckAndAddNeighbour(isOddY ? pos.x : pos.x - 1, pos.y - 1, neighbours);         // left up

        return neighbours;
    }

    private void CheckAndAddNeighbour(float _neighborRow, float _neighborCol, List<GameObject> _neighbours)
    {
        if (_neighborRow >= 0 && _neighborRow < tileField.GetLength(0) &&
            _neighborCol >= 0 && _neighborCol < tileField.GetLength(1))
        {
            GameObject neighborTile = tileField[(int)_neighborRow, (int)_neighborCol];
            if (neighborTile != null)
            {
                _neighbours.Add(neighborTile);
            }
        }
    }
    private directions GetConnectionDirection(GameObject _originTile, GameObject _targetTile)
    {
        Vector2Int posOrigin = ObjToPos(_originTile);
        Vector2Int posTarget = ObjToPos(_targetTile);


        bool isOddY = posOrigin.y % 2 == 0;

        if (posOrigin.x - 1 == posTarget.x && posOrigin.y == posTarget.y)                               // Up
            return directions.U;
        if (posOrigin.x == (isOddY ? posTarget.x : posTarget.x + 1) && posOrigin.y == posTarget.y - 1) // Right Up
            return directions.RU;
        if (posOrigin.x == (isOddY ? posTarget.x - 1 : posTarget.x) && posOrigin.y == posTarget.y - 1) // Right Down
            return directions.RD;
        if (posOrigin.x + 1 == posTarget.x && posOrigin.y == posTarget.y)                              // Down
            return directions.D;
        if (posOrigin.x == (isOddY ? posTarget.x - 1 : posTarget.x) && posOrigin.y == posTarget.y + 1) // Left Down
            return directions.LD;
        if (posOrigin.x == (isOddY ? posTarget.x : posTarget.x + 1) && posOrigin.y == posTarget.y + 1) // Left Up
            return directions.LU;

        throw new System.Exception("Target Tile is not a valid neighbor of the Origin Tile what should't be possible but yeah");
    }
    private bool OriginHasConnectionToTarget(GameObject _origin, GameObject _target)
    {
        directions neededConnectionDirection = GetConnectionDirection(_origin, _target);
        directions[] relativeDirections = GetRelativeDirections(_origin);

        return relativeDirections.Contains(neededConnectionDirection);
    }

    private directions[] GetRelativeDirections(GameObject _tile)
    {
        int curRotation = _tile.GetComponent<PuzzleTileRotator>().curRotation;

        int rotSteps = GetRotationSteps(curRotation);
        directions[] absolutTypeDirections = GetConnectionsForTileType(_tile);
        directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(absolutTypeDirections, rotSteps);
        return relativeDirections;
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

    #region DataConvertion
    private GameObject PosInObj(Vector2Int _pos)
    {
        return tileField[(int)_pos.x, (int)_pos.y];
    }
    private Vector2Int ObjToPos(GameObject _obj)
    {
        Vector2Int pos = Vector2Int.zero;

        for (int x = 0; x < tileField.GetLength(0); x++)
        {
            for (int y = 0; y < tileField.GetLength(1); y++)
            {
                if (tileField[x, y].Equals(_obj))
                {
                    pos.x = x;
                    pos.y = y;
                    return pos;
                }
            }
        }

        return pos;
    }
    #endregion

    private void PuzzleSolved()
    {
        Debug.Log("Puzzle Gelöst!");
    }
}
