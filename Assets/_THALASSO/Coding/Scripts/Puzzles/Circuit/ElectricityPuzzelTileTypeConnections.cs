using UnityEngine;

public enum Directions
{
    U = 0,
    RU = 1,
    RD = 2,
    D = 3,
    LD = 4,
    LU = 5
}
[CreateAssetMenu(fileName = "ElectricityPuzzelTileOptions", menuName = "Scriptable Objects/ElectricityPuzzelTileOptions")]
public class ElectricityPuzzelTileTypeConnections : ScriptableObject
{
    [Header("TileType1")]
    public Directions[] possibleConnectionsType1;
    [Header("TileType2")]
    public Directions[] possibleConnectionsType2;
    [Header("TileType3")]
    public Directions[] possibleConnectionsType3;
    [Header("TileType4")]
    public Directions[] possibleConnectionsType4;
    [Header("TileType5")]
    public Directions[] possibleConnectionsType5;

}
