using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElectricityPuzzlePrefabs", menuName = "Scriptable Objects/ElectricityPuzzlePrefabs")]
public class ElectricityPuzzlePrefab : ScriptableObject
{
    [SerializeField] public int[] TileTypeOrder;
}
