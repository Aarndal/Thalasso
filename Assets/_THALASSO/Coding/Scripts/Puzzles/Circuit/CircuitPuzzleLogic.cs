using ProgressionTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CircuitPuzzleLogic : SolvableObject, IAmPuzzle
{
    [SerializeField] private int puzzleID = 0;
    [SerializeField] private GameObject[] tileFieldInput;
    [SerializeField] private GameObject[] differentTileTypes;
    [SerializeField] private ElectricityPuzzelTileTypeConnections differentTileTypeConnections;
    [SerializeField] private ElectricityPuzzlePrefab[] layoutPrefabs;

    //[SerializeField] private GameObject doorLockLid;
    //[SerializeField] private Transform doorLockLidRotationPoint;
    //[SerializeField] private float transitionduration = 0.5f;
    //[SerializeField] private AnimationCurve animationSpeedCurve;

#if WWISE_2024_OR_LATER
    [Header("Wwise Audio Settings")]
    [SerializeField]
    private AK.Wwise.Event clickSound;
    [SerializeField]
    private AK.Wwise.Event correctSound;
    [SerializeField]
    private AK.Wwise.Event wrongSound;
    [SerializeField]
    private AK.Wwise.Event completeSound;
    //[SerializeField]
    //private AK.Wwise.Event openDoorSound;
#endif

    private bool isSceneStart = true;
    private GameObject nextTile;
    private GameObject startTile;
    private GameObject endTile;
    private GameObject buttonUICanvas;
    private ElectricityPuzzlePrefab ativeLayoutPrefab;
    private readonly System.Random rnd = new();

    private Button[] tileFieldButtonsInput;
    private readonly GameObject[,] tileField = new GameObject[5, 3];
    private readonly List<Vector2Int> activeTiles = new();

    #region Unity Lifecycle Methods
    private void Awake()
    {
        isSceneStart = true;
    }

    private void OnEnable()
    {
        PuzzleUIReferencesSender.PuzzleUIReferenceLogger += GetUIReference;
        PuzzleTileRotator.TileWasUpdated += ProcessTileFieldUpdate;
    }

    private void Start()
    {
        RandomizeActiveLayoutPrefab();
        SetupTileInput();

        startTile = tileField[0, 0];
        endTile = tileField[4, 2];

        ProcessTileFieldUpdate(tileField[0, 0]);

        isSceneStart = false;
    }

    private void OnDisable()
    {
        PuzzleTileRotator.TileWasUpdated -= ProcessTileFieldUpdate;
        PuzzleUIReferencesSender.PuzzleUIReferenceLogger -= GetUIReference;
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
        //StartCoroutine(TransformTransitionSystem.Instance.TransitionRot(doorLockLid, doorLockLidRotationPoint.rotation, transitionduration, animationSpeedCurve, null
//#if WWISE_2024_OR_LATER
//            , () => openDoorSound.Post(gameObject)
//#endif
//            ));
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
                            for (int i = 0; i < rnd.Next(5); i++)
                            {
                                rotator.InstantRotateForSetup();
                            }
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
        _tileObject.GetComponent<MeshRenderer>().material = newTileType.GetComponent<MeshRenderer>().sharedMaterial;
        _tileObject.name = newTileType.name;
    }

    public void ProcessTileFieldUpdate(GameObject _updatedTile)
    {
        if (!activeTiles.Contains(ObjToPos(_updatedTile)) && _updatedTile != nextTile && _updatedTile != startTile)
        {
            return;
        }

#if WWISE_2024_OR_LATER
        if (!isSceneStart)
            clickSound.Post(gameObject);
#endif

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
                    _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green * 2.2f - Color.black * 1.2f; // temp visuals

#if WWISE_2024_OR_LATER
                    if (!isSceneStart)
                        correctSound.Post(gameObject);
#endif
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


            if (activeTiles.Contains(ObjToPos(_updatedTile)) && activeTiles.Last() != ObjToPos(_updatedTile) || activeTiles.Last() == ObjToPos(_updatedTile))
            {
                previousTile = PosToObj(activeTiles[activeTiles.IndexOf(ObjToPos(_updatedTile)) - 1]);
                DeactivateTile(PosToObj(activeTiles[activeTiles.IndexOf(ObjToPos(_updatedTile))]));
            }

            if (OriginHasConnectionToTarget(_updatedTile, previousTile))
            {
                if (!activeTiles.Contains(ObjToPos(_updatedTile)))
                {
                    activeTiles.Add(ObjToPos(_updatedTile));
                    _updatedTile.GetComponent<MeshRenderer>().material.color = Color.green * 2.2f - Color.black * 1.2f; // temp visuals
#if WWISE_2024_OR_LATER
                    if (!isSceneStart)
                        correctSound.Post(gameObject);
#endif
                }

                nextTile = GetNextTileInCircuit(_updatedTile, relativeDirections, previousTile);

                if (nextTile != null && !activeTiles.Contains(ObjToPos(nextTile)))
                    ProcessTileFieldUpdate(nextTile);

                if (relativeDirections.Contains(Directions.D) && _updatedTile == endTile)
                {
                    Solve();
                }

                return;
            }
            if (activeTiles.Contains(ObjToPos(_updatedTile)) && _updatedTile == nextTile)
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

            _tileToDeactivate.GetComponent<MeshRenderer>().material.color = Color.red * 2.2f - Color.black * 1.2f;
#if WWISE_2024_OR_LATER
            wrongSound.Post(gameObject);
#endif
            activeTiles.RemoveAt(index);

            nextTile = _tileToDeactivate;
        }
    }

    private GameObject GetNextTileInCircuit(GameObject _updatedTile, Directions[] _relativeDirections, GameObject _previousTile)
    {
        Directions targetDirection = Directions.U;

        Directions connectionDirection = GetConnectionDirection(_updatedTile, _previousTile);

        switch (_relativeDirections.Length)
        {
            case 2:
                {
                    foreach (var direction in _relativeDirections)
                    {
                        if (connectionDirection != direction)
                        {
                            targetDirection = direction;
                            break;
                        }
                    }
                    break;
                }
            case 4:
                {
                    if (_relativeDirections[0] == connectionDirection || _relativeDirections[1] == connectionDirection)
                    {
                        targetDirection = _relativeDirections[0] == connectionDirection ? _relativeDirections[1] : _relativeDirections[0];
                    }
                    else if (_relativeDirections[2] == connectionDirection || _relativeDirections[3] == connectionDirection)
                    {
                        targetDirection = _relativeDirections[2] == connectionDirection ? _relativeDirections[3] : _relativeDirections[2];
                    }
                    break;
                }
        }

        GameObject nextTile = GetTileInDirection(_updatedTile, targetDirection);
        if (nextTile == _updatedTile)
        {
            return null;
        }
        return nextTile;
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

    private Directions GetConnectionDirection(GameObject _originTile, GameObject _targetTile)
    {
        Vector2Int posOrigin = ObjToPos(_originTile);
        Vector2Int posTarget = ObjToPos(_targetTile);

        bool isOddY = posOrigin.y % 2 == 0;

        if (posOrigin.x - 1 == posTarget.x && posOrigin.y == posTarget.y) // Up
            return Directions.U;
        if (posOrigin.x == (isOddY ? posTarget.x : posTarget.x + 1) && posOrigin.y == posTarget.y - 1) // Right Up
            return Directions.RU;
        if (posOrigin.x == (isOddY ? posTarget.x - 1 : posTarget.x) && posOrigin.y == posTarget.y - 1) // Right Down
            return Directions.RD;
        if (posOrigin.x + 1 == posTarget.x && posOrigin.y == posTarget.y) // Down
            return Directions.D;
        if (posOrigin.x == (isOddY ? posTarget.x - 1 : posTarget.x) && posOrigin.y == posTarget.y + 1) // Left Down
            return Directions.LD;
        if (posOrigin.x == (isOddY ? posTarget.x : posTarget.x + 1) && posOrigin.y == posTarget.y + 1) // Left Up
            return Directions.LU;

        throw new System.Exception("Target Tile is not a valid neighbor of the Origin Tile what shouldn't be possible but yeah");
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
            case "DoorLock_Hex1":
                return differentTileTypeConnections.possibleConnectionsType1;
            case "DoorLock_Hex2":
                return differentTileTypeConnections.possibleConnectionsType2;
            case "DoorLock_Hex3":
                return differentTileTypeConnections.possibleConnectionsType3;
            case "DoorLock_Hex4":
                return differentTileTypeConnections.possibleConnectionsType4;
            case "DoorLock_Hex5":
                return differentTileTypeConnections.possibleConnectionsType5;
            default:
                return null;
        }
    }

    private GameObject PosToObj(Vector2Int _pos)
    {
        return tileField[(int)_pos.x, (int)_pos.y];
    }

    private Vector2Int ObjToPos(GameObject _obj)
    {
        for (int x = 0; x < tileField.GetLength(0); x++)
        {
            for (int y = 0; y < tileField.GetLength(1); y++)
            {
                if (tileField[x, y].Equals(_obj))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return Vector2Int.zero;
    }

    public override bool Solve()
    {
        Debug.Log("Puzzle Gel�st!");
#if WWISE_2024_OR_LATER
        completeSound.Post(gameObject);
#endif
        return IsSolved = true;
    }
}
