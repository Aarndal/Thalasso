using UnityEngine;

public enum directions
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
    public directions[] possibleConnectionsType1;
    [Header("TileType2")]
    public directions[] possibleConnectionsType2;
    [Header("TileType3")]
    public directions[] possibleConnectionsType3;
    [Header("TileType4")]
    public directions[] possibleConnectionsType4;
    [Header("TileType5")]
    public directions[] possibleConnectionsType5;

}
