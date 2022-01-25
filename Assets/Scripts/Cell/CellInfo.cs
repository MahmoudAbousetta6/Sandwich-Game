using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class hold the data about each cell and which ingredients during intialization.
/// </summary>
[System.Serializable]
public class CellInfo
{
    private int posX;
    private int posZ;
    private int instantiatedIndex;

    public int InstantiatedIndex { get => instantiatedIndex; set => instantiatedIndex = value; }

    public CellInfo(int _posX = 0, int _posZ = 0)
    {
        posX = _posX;
        posZ = _posZ;
    }

    public CellInfo(ref CellInfo cell)
    {
        posX = cell.posX;
        posZ = cell.posZ;
    }

    public override int GetHashCode()
    {
        int hash = 3;
        hash = 97 * hash + posX;
        hash = 97 * hash + posZ;
        return hash;
    }

    public override bool Equals(object obj)
    {
        var other = obj as CellInfo;
        if (other == null)
        {
            return false;
        }
        return posX == other.posX && posZ == other.posZ;
    }

    public bool IsValidGridPos(int maxRow, int maxCol)
    {
        bool isValidRow = posX >= 0 && posX < maxRow;
        bool isValidCol = posZ >= 0 && posZ < maxCol;
        return isValidRow && isValidCol;
    }

    public static CellInfo GetRandomValidCell(ref CellInfo center, int maxRow, int maxCol)
    {
        CellInfo tempCell = null;
        do
        {

            int tempX = 0;
            int tempZ = 0;
            if (Random.Range(0, 2) == 0) tempX = Random.Range(-1, 2);
            else tempZ = Random.Range(-1, 2);
            tempCell = new CellInfo(ref center);
            tempCell.posX += tempX;
            tempCell.posZ += tempZ;
        } while (!tempCell.IsValidGridPos(maxRow, maxCol));
        return tempCell;
    }

    public int GetIndex(int rowLength) => (posX * rowLength) + posZ;
}