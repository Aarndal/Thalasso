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
    [SerializeField] private GameObject[] differentTileTypes;
    [SerializeField] private ElectricityPuzzelTileTypeConnections differentTileTypeConnections;
    [SerializeField] private ElectricityPuzzlePrefab[] layoutPrefabs;

    [SerializeField] private GameObject doorLockLid;
    [SerializeField] private Transform doorLockLidRotationPoint;
    [SerializeField] private float transitionduration = 0.5f;
    [SerializeField] private AnimationCurve animationSpeedCurve;

    private GameObject nextTile;
    private GameObject startTile;
    private GameObject endTile;
    private GameObject buttonUICanvas;
    private int failBuffer = 0;
    private System.Random rnd = new();
    private ElectricityPuzzlePrefab ativeLayoutPrefab;

    private GameObject[,] tileField = new GameObject[5, 3];
    private Button[] tileFieldButtonsInput;
    private List<Vector2Int> activeTiles = new();
    private List<Vector2Int> updatedInThisLoop = new();
    private List<Vector2Int> connectiongTiles = new();

    #region Unity Lifecycle Methods
    private void Awake()
    {
        PuzzleUIReferencesSender.puzzleUIReferenceLogger += GetUIReference;
        PuzzleTileRotator.tileWasUpdated += ProcessTileFieldUpdate;
    }

    private void Start()
    {
        RandomizeActiveLayoutPrefab();
        SetupTileInput();

        startTile = tileField[0, 0];
        endTile = tileField[4, 2];

        ProcessTileFieldUpdate(tileField[0, 0]);
    }


    private void OnDestroy()
    {
        PuzzleTileRotator.tileWasUpdated -= ProcessTileFieldUpdate;
        PuzzleUIReferencesSender.puzzleUIReferenceLogger -= GetUIReference;
    }
    #endregion

    private void GetUIReference(GameObject reference, int ID)
    {
        if (ID == puzzleID)
        {
            buttonUICanvas = reference;
            buttonUICanvas.SetActive(false);
        }
    }

    public void StartPuzzle()
    {
        StartCoroutine(TransformTransitionSystem.Instance.TransitionRot(doorLockLid, doorLockLidRotationPoint.rotation, transitionduration, animationSpeedCurve, null, null));
    }

    private void RandomizeActiveLayoutPrefab()
    {
        ativeLayoutPrefab = layoutPrefabs[rnd.Next(layoutPrefabs.Length)];
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
                    SetNewTileType(tileField[row, col], ativeLayoutPrefab.TileTypeOrder[index]);
                    if (tileFieldInput[index] != null)
                    {
                        if (tileFieldInput[index].TryGetComponent<PuzzleTileRotator>(out var rotator))
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
    private void SetNewTileType(GameObject _tileObject, int _tileTypeIndice)
    {

        GameObject newTileType = differentTileTypes[_tileTypeIndice];

        _tileObject.GetComponent<MeshFilter>().mesh = newTileType.GetComponent<MeshFilter>().sharedMesh;
        _tileObject.name = newTileType.name;
    }

    #region OldLogicWithBugs
    private void GenerateNewLayoutOld()
    {
        connectiongTiles.Clear();
        connectiongTiles.Add(ObjToPos(startTile));
        failBuffer = 0;

        ChooseNextTileOld(startTile);
        BuildPathWithTilesOld();
        CompleteRestOfTilesOld();
    }

    private void ChooseNextTileOld(GameObject curTile)
    {
        if (connectiongTiles.Count == 0 || failBuffer > 50)
        {
            GenerateNewLayoutOld();
            return;
        }

        List<GameObject> neighboursTiles = GetAllNeighbours(curTile).FindAll(tile => !connectiongTiles.Contains(ObjToPos(tile)));

        if (neighboursTiles.Count == 0)
        {
            if (curTile == startTile)
            {
                GenerateNewLayoutOld();
                return;
            }
            failBuffer++;
            connectiongTiles.RemoveAt(connectiongTiles.Count - 1);
            ChooseNextTileOld(PosToObj(connectiongTiles[connectiongTiles.Count - 1]));
            return;
        }

        GameObject newTile = neighboursTiles[rnd.Next(neighboursTiles.Count)];
        connectiongTiles.Add(ObjToPos(newTile));

        if (newTile == endTile)
        {
            return;
        }

        ChooseNextTileOld(newTile);
    }

    private void BuildPathWithTilesOld()
    {
        for (int i = 0; i < connectiongTiles.Count; i++)
        {
            List<Directions> neededDirection = new();
            if (PosToObj(connectiongTiles[i]) == startTile)
            {
                neededDirection.Add(Directions.U);
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i + 1])));
            }
            else if (PosToObj(connectiongTiles[i]) == endTile)
            {
                neededDirection.Add(Directions.D);
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i - 1])));
            }
            else
            {
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i + 1])));
                neededDirection.Add(GetConnectionDirection(PosToObj(connectiongTiles[i]), PosToObj(connectiongTiles[i - 1])));
            }

            ChooseTileBasedOnRequirementsOld(PosToObj(connectiongTiles[i]), neededDirection.ToArray());
        }
    }

    private void ChooseTileBasedOnRequirementsOld(GameObject _tileObject, Directions[] _requiredDirections)
    {
        int directionOneValue = (int)_requiredDirections[0];
        int directionTwoValue = (int)_requiredDirections[1];
        int tileTypeValue;
        if (Math.Abs(directionOneValue - directionTwoValue) > 3)
        {
            tileTypeValue = Math.Abs(3 - (Math.Abs(directionOneValue - directionTwoValue)));
            Debug.LogWarning("over" + tileTypeValue);
        }
        else
        {
            tileTypeValue = Math.Abs(directionOneValue - directionTwoValue);
            Debug.LogWarning("under" + tileTypeValue);
        }
        switch (tileTypeValue)
        {
            case 1:
                {
                    SetNewTileTypeOld(_tileObject, new int[] { 2, 3 });
                    break;
                }
            case 2:
                {
                    SetNewTileTypeOld(_tileObject, new int[] { 1, 4 });
                    break;
                }
            case 3:
                {
                    SetNewTileTypeOld(_tileObject, new int[] { 0 });
                    break;
                }
        }
    }

    private void SetNewTileTypeOld(GameObject _tileObject, int[] _tileTypeIndices)
    {
        int randomSelection = rnd.Next(_tileTypeIndices.Length);

        GameObject newTileType = differentTileTypes[_tileTypeIndices[randomSelection]];

        _tileObject.GetComponent<MeshFilter>().mesh = newTileType.GetComponent<MeshFilter>().sharedMesh;
        _tileObject.name = newTileType.name;
    }

    private void CompleteRestOfTilesOld()
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
    #endregion

    public void ProcessTileFieldUpdate(GameObject _updatedTile)
    {
        if (!activeTiles.Contains(ObjToPos(_updatedTile)) && _updatedTile != nextTile && _updatedTile != startTile)
        {
            return;
        }

        int updatedTileCurRotation = Math.Abs(_updatedTile.GetComponent<PuzzleTileRotator>().curRotation);
        int rotSteps = GetRotationSteps(updatedTileCurRotation);
        Directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
        Directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);


        if (_updatedTile == startTile)
        {
            if (relativeDirections.Contains(Directions.U))
            {
                if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Add(ObjToPos(_updatedTile));
                    _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green;                 //temp visuals
                }

                Directions neededDirection = Directions.U;
                foreach (var direction in relativeDirections)
                {
                    if (Directions.U != direction)
                    {
                        neededDirection = direction;
                        break;
                    }
                }

                nextTile = GetTileInDirection(_updatedTile, neededDirection);

                if (nextTile != null)
                    ProcessTileFieldUpdate(nextTile);
            }
            else
            {
                DeactivateTile(_updatedTile);
            }
        }
        else
        {
            GameObject previousTile = PosToObj(activeTiles.Last());
            if ((!activeTiles.Contains(ObjToPos(_updatedTile)) && _updatedTile == nextTile) || previousTile == _updatedTile)
            {
                if (previousTile == _updatedTile)
                    previousTile = PosToObj(activeTiles[activeTiles.Count - 2]);

                if (OriginHasConnectionToTarget(_updatedTile, previousTile))
                {
                    if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                    {
                        activeTiles.Add(ObjToPos(_updatedTile));
                        _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green;                 //temp visuals
                    }


                    nextTile = GetNextTileInCircuit(_updatedTile, relativeDirections, previousTile);

                    if (nextTile != null)
                        ProcessTileFieldUpdate(nextTile);

                    if (relativeDirections.Contains(Directions.D) && _updatedTile == endTile)
                    {
                        Solve();
                    }

                    return;
                }
            }
            else if (activeTiles.Contains(ObjToPos(_updatedTile)) && _updatedTile == nextTile)
            {
                nextTile = null;
                return;
            }

            DeactivateTile(_updatedTile);

        }


    }

    private void DeactivateTile(GameObject _tileToDeactivate)
    {
        if (_tileToDeactivate == null)
        {
            return;
        }
        if (activeTiles.Contains(ObjToPos(_tileToDeactivate)))
        {
            int index = activeTiles.IndexOf(ObjToPos(_tileToDeactivate));
            if (index + 1 < activeTiles.Count)
            {
                DeactivateTile(PosToObj(activeTiles[index + 1]));
            }

            _tileToDeactivate.GetComponent<MeshRenderer>().material.color = Color.red;
            activeTiles.RemoveAt(index);

            nextTile = _tileToDeactivate;
        }
    }

    private GameObject GetNextTileInCircuit(GameObject _updatedTile, Directions[] _relativeDirections, GameObject _previousTile)
    {
        Directions neededDirection = Directions.U;

        Directions connectionDirection = GetConnectionDirection(_updatedTile, _previousTile);

        switch (_relativeDirections.Length)
        {
            case 2:
                {
                    foreach (var direction in _relativeDirections)
                    {
                        if (connectionDirection != direction)
                        {
                            neededDirection = direction;
                            break;
                        }
                    }
                    break;
                }
            case 4:
                {
                    if (_relativeDirections[0] == connectionDirection || _relativeDirections[1] == connectionDirection)
                    {
                        neededDirection = _relativeDirections[0] == connectionDirection ? _relativeDirections[1] : _relativeDirections[0];
                    }
                    else if (_relativeDirections[2] == connectionDirection || _relativeDirections[3] == connectionDirection)
                    {
                        neededDirection = _relativeDirections[2] == connectionDirection ? _relativeDirections[3] : _relativeDirections[2];
                    }
                    break;
                }
        }

        return GetTileInDirection(_updatedTile, neededDirection);
    }

    private GameObject GetTileInDirection(GameObject updatedTile, Directions neededDirection)
    {
        Vector2Int pos = ObjToPos(updatedTile);
        bool isOddY = pos.y % 2 == 0;
        Vector2Int newPos = pos;

        switch (neededDirection)
        {
            case Directions.U:
                newPos = new Vector2Int(pos.x - 1, pos.y);
                break;
            case Directions.RU:
                newPos = new Vector2Int(isOddY ? pos.x : pos.x - 1, pos.y + 1);
                break;
            case Directions.RD:
                newPos = new Vector2Int(isOddY ? pos.x + 1 : pos.x, pos.y + 1);
                break;
            case Directions.D:
                newPos = new Vector2Int(pos.x + 1, pos.y);
                break;
            case Directions.LD:
                newPos = new Vector2Int(isOddY ? pos.x + 1 : pos.x, pos.y - 1);
                break;
            case Directions.LU:
                newPos = new Vector2Int(isOddY ? pos.x : pos.x - 1, pos.y - 1);
                break;
            default:
                return null;
        }

        if (newPos.x >= 0 && newPos.x < tileField.GetLength(0) && newPos.y >= 0 && newPos.y < tileField.GetLength(1))
        {
            return tileField[newPos.x, newPos.y];
        }
        else
        {
            return null;
        }
    }
    #region oldCode
    public void OnFieldGotUpdateOld(GameObject _updatedTile)
    {
        if (updatedInThisLoop.Contains(ObjToPos(_updatedTile)))
        {
            return;
        }
        else
        {
            updatedInThisLoop.Add(ObjToPos(_updatedTile));
        }

        int updatedTileCurRotation = Math.Abs(_updatedTile.GetComponent<PuzzleTileRotator>().curRotation);


        if (_updatedTile == startTile)
        {
            int rotSteps = GetRotationSteps(updatedTileCurRotation);
            Directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            Directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            if (relativeDirections.Contains(Directions.U))
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
        }
        else if (_updatedTile == endTile)
        {
            int rotSteps = GetRotationSteps(updatedTileCurRotation);
            Directions[] typeDirections = GetConnectionsForTileType(_updatedTile);
            Directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(typeDirections, rotSteps);

            List<GameObject> neighbours = GetAllNeighbours(_updatedTile);

            bool hasActiveConnection = false;

            foreach (GameObject neighbour in neighbours)
            {
                if (activeTiles.Contains(ObjToPos(neighbour)))
                {

                    if (OriginHasConnectionToTarget(neighbour, _updatedTile) && OriginHasConnectionToTarget(_updatedTile, neighbour) && relativeDirections.Contains(Directions.D))
                    {
                        if (activeTiles.Contains(ObjToPos(_updatedTile)))
                            if (activeTiles.IndexOf(ObjToPos(neighbour)) > activeTiles.IndexOf(ObjToPos(_updatedTile)))
                            {
                                _updatedTile.GetComponent<MeshRenderer>().material.color = Color.red;    //temp visuals
                                activeTiles.Remove(ObjToPos(_updatedTile));
                                UpdateNeighbour(_updatedTile);
                                return;
                            }
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
                        if (activeTiles.Contains(ObjToPos(_updatedTile)))
                            if (activeTiles.IndexOf(ObjToPos(neighbour)) > activeTiles.IndexOf(ObjToPos(_updatedTile)))
                            {
                                _updatedTile.GetComponent<MeshRenderer>().material.color = Color.red;    //temp visuals
                                activeTiles.Remove(ObjToPos(_updatedTile));
                                UpdateNeighbour(_updatedTile);
                                return;
                            }
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
    }
    #endregion

    private void UpdateNeighbour(GameObject _updatedTile)
    {
        List<GameObject> neighbours = GetAllNeighbours(_updatedTile);
        foreach (GameObject neighbour in neighbours)
            if (!updatedInThisLoop.Contains(ObjToPos(neighbour)))
                ProcessTileFieldUpdate(neighbour);
    }

    #region Neighbours
    private List<GameObject> GetAllNeighbours(GameObject _updatedTile)
    {
        List<GameObject> neighbours = new();

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
    private Directions GetConnectionDirection(GameObject _originTile, GameObject _targetTile)
    {
        Vector2Int posOrigin = ObjToPos(_originTile);
        Vector2Int posTarget = ObjToPos(_targetTile);


        bool isOddY = posOrigin.y % 2 == 0;

        if (posOrigin.x - 1 == posTarget.x && posOrigin.y == posTarget.y)                               // Up
            return Directions.U;
        if (posOrigin.x == (isOddY ? posTarget.x : posTarget.x + 1) && posOrigin.y == posTarget.y - 1) // Right Up
            return Directions.RU;
        if (posOrigin.x == (isOddY ? posTarget.x - 1 : posTarget.x) && posOrigin.y == posTarget.y - 1) // Right Down
            return Directions.RD;
        if (posOrigin.x + 1 == posTarget.x && posOrigin.y == posTarget.y)                              // Down
            return Directions.D;
        if (posOrigin.x == (isOddY ? posTarget.x - 1 : posTarget.x) && posOrigin.y == posTarget.y + 1) // Left Down
            return Directions.LD;
        if (posOrigin.x == (isOddY ? posTarget.x : posTarget.x + 1) && posOrigin.y == posTarget.y + 1) // Left Up
            return Directions.LU;

        throw new System.Exception("Target Tile is not a valid neighbor of the Origin Tile what should't be possible but yeah");
    }
    private bool OriginHasConnectionToTarget(GameObject _origin, GameObject _target)
    {
        Directions neededConnectionDirection = GetConnectionDirection(_origin, _target);
        Directions[] relativeDirections = GetRelativeDirections(_origin);

        if (relativeDirections.Length == 2)
        {
            return relativeDirections.Contains(neededConnectionDirection);
        }
        else if (relativeDirections.Length == 4)
        {
            // Check if the target direction is connected to one of the two pairs
            bool firstPairConnected = relativeDirections[0] == neededConnectionDirection || relativeDirections[1] == neededConnectionDirection;
            bool secondPairConnected = relativeDirections[2] == neededConnectionDirection || relativeDirections[3] == neededConnectionDirection;
            return firstPairConnected || secondPairConnected;
        }

        return false;
    }

    private Directions[] GetRelativeDirections(GameObject _tile)
    {
        int curRotation = Math.Abs(_tile.GetComponent<PuzzleTileRotator>().curRotation);

        int rotSteps = GetRotationSteps(curRotation);
        Directions[] absolutTypeDirections = GetConnectionsForTileType(_tile);
        Directions[] relativeDirections = ConvertDefaultConnectionIntoRotationRelative(absolutTypeDirections, rotSteps);
        return relativeDirections;
    }
    #endregion

    #region Directions&Rotations
    private Directions[] ConvertDefaultConnectionIntoRotationRelative(Directions[] _directions, int _rotSteps)
    {
        Directions[] relativeDirections = new Directions[_directions.Length];

        int arrIndex = 0;
        foreach (Directions direction in _directions)
        {
            int curIndex = (int)direction;
            int newIndex = curIndex + _rotSteps;

            if (newIndex > 5)
                newIndex -= 6;

            relativeDirections[arrIndex] = (Directions)newIndex;
            arrIndex++;
        }

        return relativeDirections;
    }

    private int GetRotationSteps(float _rotation)
    {
        return (int)(_rotation / 60);
    }

    private Directions[] GetConnectionsForTileType(GameObject _tile)
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
