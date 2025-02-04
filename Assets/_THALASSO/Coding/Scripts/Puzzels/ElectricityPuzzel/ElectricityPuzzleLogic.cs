using ProgressionTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ElectricityPuzzleLogic : SolvableObjectBase
{
    [SerializeField] private int puzzleID = 0;
    [SerializeField] private GameObject[] tileFieldInput;
    private GameObject[,] tileField = new GameObject[5, 3];
    [SerializeField] private GameObject[] differentTileTypes;
    [SerializeField] private ElectricityPuzzelTileTypeConnections differentTileTypeConnections;

    [SerializeField] private GameObject doorLockLid;
    [SerializeField] private Transform doorLockLidRotationPoint;
    [SerializeField] private float transitionduration = 0.5f;
    [SerializeField] private AnimationCurve animationSpeedCurve;

    private GameObject startTile;
    private GameObject endTile;
    private GameObject buttonUICanvas;
    private Button[] tileFieldButtonsInput;

    private System.Random rnd = new System.Random();


    private void Awake()
    {
        PuzzleUIReferencesSender.puzzleUIReferenceLogger += GetUIReference;
        PuzzleTileRotator.tileWasUpdated += OnFieldGotUpdate;
    }

    private void GetUIReference(GameObject reference, int ID)
    {
        if (ID == puzzleID)
        {
            buttonUICanvas = reference;
            buttonUICanvas.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        PuzzleTileRotator.tileWasUpdated -= OnFieldGotUpdate;
        PuzzleUIReferencesSender.puzzleUIReferenceLogger -= GetUIReference;
    }

    private void Start()
    {
        SetupTileInput();

        startTile = tileField[0, 0];
        endTile = tileField[4, 2];

        GenerateNewLayout();
        OnFieldGotUpdate(tileField[0, 0]);
    }
    public void StartPuzzle()
    {
        TransformTransitionSystem.Instance.TransitionRot(doorLockLid, doorLockLidRotationPoint, transitionduration, animationSpeedCurve, null, null);
    }

    private void SetupTileInput()
    {
        if (buttonUICanvas == null)
        {
            Debug.LogError("buttonUICanvas is null. Make sure it is assigned correctly.");
            return;
        }

        Button[] allChildWithButtons = buttonUICanvas.transform.GetComponentsInChildren<Button>();
        tileFieldButtonsInput = new Button[allChildWithButtons.Length];
        for (int i = 0; i < allChildWithButtons.Length; i++)
        {
            tileFieldButtonsInput[i] = allChildWithButtons[i];
        }

        if (tileFieldInput.Length < tileField.GetLength(0) * tileField.GetLength(1))
        {
            Debug.LogError("tileFieldInput does not have enough elements.");
            return;
        }

        int index = 0;
        for (int row = 0; row < tileField.GetLength(0); row++)
        {
            for (int col = 0; col < tileField.GetLength(1); col++)
            {
                if (index < tileFieldInput.Length)
                {
                    tileField[row, col] = tileFieldInput[index];
                    if (tileFieldInput[index] != null)
                    {
                        PuzzleTileRotator rotator = tileFieldInput[index].GetComponent<PuzzleTileRotator>();
                        if (rotator != null)
                        {
                            tileFieldButtonsInput[index].onClick.AddListener(rotator.OnRotateClick);
                        }
                        else
                        {
                            Debug.LogError($"PuzzleTileRotator component not found on tileFieldInput[{index}].");
                        }
                    }
                    else
                    {
                        Debug.LogError($"tileFieldInput[{index}] is null.");
                    }
                    index++;
                }
            }
        }
    }

    private List<Vector2Int> connectiongTiles = new List<Vector2Int>();
    int failBuffer = 0;

    private void GenerateNewLayout()
    {
        connectiongTiles.Clear();
        connectiongTiles.Add(ObjToPos(startTile));
        failBuffer = 0;

        ChooseNextTile(startTile);
        BuildPathWithTiles();
        CompleteRestOfTiles();
    }


    private void ChooseNextTile(GameObject curTile)
    {
        if (connectiongTiles.Count == 0 || failBuffer > 50)
        {
            GenerateNewLayout();
            return;
        }

        List<GameObject> neighboursTiles = GetAllNeighbours(curTile).FindAll(tile => !connectiongTiles.Contains(ObjToPos(tile)));

        if (neighboursTiles.Count == 0)
        {
            if (curTile == startTile)
            {
                GenerateNewLayout();
                return;
            }
            failBuffer++;
            connectiongTiles.RemoveAt(connectiongTiles.Count - 1);
            ChooseNextTile(PosToObj(connectiongTiles[connectiongTiles.Count - 1]));
            return;
        }

        GameObject newTile = neighboursTiles[rnd.Next(neighboursTiles.Count)];
        connectiongTiles.Add(ObjToPos(newTile));

        if (newTile == endTile)
        {
            return;
        }

        ChooseNextTile(newTile);
    }

    private void BuildPathWithTiles()
    {
        for (int i = 0; i < connectiongTiles.Count; i++)
        {
            List<directions> neededDirection = new List<directions>();
            if (PosToObj(connectiongTiles[i]) == startTile)
            {
                neededDirection.Add(directions.U);
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i + 1])));
            }
            else if (PosToObj(connectiongTiles[i]) == endTile)
            {
                neededDirection.Add(directions.D);
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i - 1])));
            }
            else
            {
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i + 1])));
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i - 1])));
            }

            ChooseTileBasedOnRequirements(PosToObj(connectiongTiles[i]), neededDirection.ToArray());
        }
    }

    private void ChooseTileBasedOnRequirements(GameObject _tileObject, directions[] _requiredDirections)
    {
        int directionOneValue = (int)_requiredDirections[0];
        int directionTwoValue = (int)_requiredDirections[1];
        int tileTypeValue;
        if (Math.Abs(directionOneValue - directionTwoValue) > 3)
        {
            tileTypeValue = Math.Abs(3 - (Math.Abs(directionOneValue - directionTwoValue)));
            Debug.Log("over" + tileTypeValue);
        }
        else
        {
            tileTypeValue = Math.Abs(directionOneValue - directionTwoValue);
            Debug.Log("under" + tileTypeValue);
        }
        switch (tileTypeValue)
        {
            case 1:
                {
                    SetNewTileType(_tileObject, new int[] { 2, 3 });
                    break;
                }
            case 2:
                {
                    SetNewTileType(_tileObject, new int[] { 1, 4 });
                    break;
                }
            case 3:
                {
                    SetNewTileType(_tileObject, new int[] { 0 });
                    break;
                }
        }
    }

    private void SetNewTileType(GameObject _tileObject, int[] _tileTypeIndices)
    {
        int randomSelection = rnd.Next(_tileTypeIndices.Length);

        GameObject newTileType = differentTileTypes[_tileTypeIndices[randomSelection]];

        _tileObject.GetComponent<MeshFilter>().mesh = newTileType.GetComponent<MeshFilter>().sharedMesh;
        _tileObject.name = newTileType.name;
    }
    private void CompleteRestOfTiles()
    {
        for (int row = 0; row < tileField.GetLength(0); row++)
        {
            for (int col = 0; col < tileField.GetLength(1); col++)
            {
                Vector2Int pos = new Vector2Int(row, col);
                if (!connectiongTiles.Contains(pos))
                {
                    GameObject tile = tileField[row, col];
                    int randomIndex = rnd.Next(differentTileTypes.Length);
                    GameObject newTileType = differentTileTypes[randomIndex];

                    tile.GetComponent<MeshFilter>().mesh = newTileType.GetComponent<MeshFilter>().sharedMesh;
                    tile.name = newTileType.name;
                }
            }
        }
    }

    private List<Vector2Int> activeTiles = new List<Vector2Int>();
    private List<Vector2Int> updatedInThisLoop = new List<Vector2Int>();

    public void OnFieldGotUpdate(GameObject _updatedTile)
    {
        if (updatedInThisLoop.Contains(ObjToPos(_updatedTile)))
            return;

        int updatedTileCurRotation = Math.Abs(_updatedTile.GetComponent<PuzzleTileRotator>().curRotation);


        if (_updatedTile == startTile)
        {
            int rotSteps = GetRotationSteps(updatedTileCurRotation);
            directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            if (relativeDirections.Contains(directions.U))
            {
                _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green;                 //temp visuals

                if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Add(ObjToPos(_updatedTile));
                }


                updatedInThisLoop.Add(ObjToPos(_updatedTile));
                UpdateNeighbour(_updatedTile);
            }
            else
            {
                _updatedTile.GetComponent<MeshRenderer>().material.color = Color.red;                 //temp visuals

                if (activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Remove(ObjToPos(_updatedTile));
                    UpdateNeighbour(_updatedTile);
                }
            }

            if (updatedInThisLoop.Contains(ObjToPos(_updatedTile)))
                updatedInThisLoop.Add(ObjToPos(_updatedTile));
        }
        else if (_updatedTile == endTile)
        {
            int rotSteps = GetRotationSteps(updatedTileCurRotation);
            directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            List<GameObject> neighbours = GetAllNeighbours(_updatedTile);

            bool hasActiveConnection = false;

            foreach (GameObject neighbour in neighbours)
            {
                if (activeTiles.Contains(ObjToPos(neighbour)))
                {

                    if (OriginHasConnectionToTarget(neighbour, _updatedTile) && OriginHasConnectionToTarget(_updatedTile, neighbour) && relativeDirections.Contains(directions.D))
                    {
                        _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green;               //temp visuals

                        if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                        {
                            activeTiles.Add(ObjToPos(_updatedTile));
                        }


                        updatedInThisLoop.Add(ObjToPos(_updatedTile));
                        UpdateNeighbour(_updatedTile);

                        hasActiveConnection = true;
                        break;
                    }
                }
            }

            if (!hasActiveConnection)
            {
                _updatedTile.GetComponent<MeshRenderer>().material.color = Color.red;    //temp visuals

                if (activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Remove(ObjToPos(_updatedTile));
                    UpdateNeighbour(_updatedTile);
                }
            }

            if (updatedInThisLoop.Contains(ObjToPos(_updatedTile)))
                updatedInThisLoop.Add(ObjToPos(_updatedTile));
        }
        else
        {
            List<GameObject> neighbours = GetAllNeighbours(_updatedTile);

            bool hasActiveConnection = false;

            foreach (GameObject neighbour in neighbours)
            {
                if (activeTiles.Contains(ObjToPos(neighbour)))
                {

                    if (OriginHasConnectionToTarget(neighbour, _updatedTile) && OriginHasConnectionToTarget(_updatedTile, neighbour))
                    {
                        _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green;               //temp visuals

                        if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                        {
                            activeTiles.Add(ObjToPos(_updatedTile));
                        }

                        updatedInThisLoop.Add(ObjToPos(_updatedTile));
                        UpdateNeighbour(_updatedTile);

                        hasActiveConnection = true;
                        break;
                    }
                }
            }

            if (!hasActiveConnection)
            {
                _updatedTile.GetComponent<MeshRenderer>().material.color = Color.red;         //temp visuals

                if (activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Remove(ObjToPos(_updatedTile));
                    UpdateNeighbour(_updatedTile);
                }
            }

            if (updatedInThisLoop.Contains(ObjToPos(_updatedTile)))
                updatedInThisLoop.Add(ObjToPos(_updatedTile));
        }

        if (updatedInThisLoop.Count != 0)
            if (updatedInThisLoop[0] == ObjToPos(_updatedTile))
            {
                updatedInThisLoop.Clear();

        if (activeTiles.Contains(ObjToPos(endTile)))
        {
            Solve();
        }
    }

    private void UpdateNeighbour(GameObject _updatedTile)
    {
        List<GameObject> neighbours = GetAllNeighbours(_updatedTile);
        foreach (GameObject neighbour in neighbours)
            if (!updatedInThisLoop.Contains(ObjToPos(neighbour)))
                OnFieldGotUpdate(neighbour);
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
        int curRotation = Math.Abs(_tile.GetComponent<PuzzleTileRotator>().curRotation);

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
    private GameObject PosToObj(Vector2Int _pos)
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

    public override bool Solve()
    {
        Debug.Log("Puzzle Gelöst!");

        return IsSolved = true;
    }
}
