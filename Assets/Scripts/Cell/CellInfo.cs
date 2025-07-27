using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class holds the data about each cell and which ingredients during initialization.
/// Represents a position in the 2D grid and provides utility methods for grid navigation.
/// </summary>
[System.Serializable]
public class CellInfo
{
    // Grid position coordinates
    private int posX;                    // X coordinate in the grid
    private int posZ;                    // Z coordinate in the grid
    private int instantiatedIndex;       // Index of the ingredient assigned to this cell

    /// <summary>
    /// Gets or sets the index of the ingredient that was instantiated in this cell.
    /// </summary>
    public int InstantiatedIndex { get => instantiatedIndex; set => instantiatedIndex = value; }

    /// <summary>
    /// Constructor to create a new cell at specified grid coordinates.
    /// </summary>
    /// <param name="_posX">X coordinate in the grid</param>
    /// <param name="_posZ">Z coordinate in the grid</param>
    public CellInfo(int _posX = 0, int _posZ = 0)
    {
        posX = _posX;
        posZ = _posZ;
    }

    /// <summary>
    /// Copy constructor that creates a new cell with the same position as the reference cell.
    /// </summary>
    /// <param name="cell">Reference cell to copy position from</param>
    public CellInfo(ref CellInfo cell)
    {
        posX = cell.posX;
        posZ = cell.posZ;
    }

    /// <summary>
    /// Generates a hash code for this cell based on its position.
    /// Used for HashSet operations to ensure unique cell identification.
    /// </summary>
    /// <returns>Hash code based on X and Z coordinates</returns>
    public override int GetHashCode()
    {
        int hash = 3;
        hash = 97 * hash + posX;
        hash = 97 * hash + posZ;
        return hash;
    }

    /// <summary>
    /// Compares this cell with another object to check if they represent the same grid position.
    /// </summary>
    /// <param name="obj">Object to compare with</param>
    /// <returns>True if both cells have the same X and Z coordinates</returns>
    public override bool Equals(object obj)
    {
        var other = obj as CellInfo;
        if (other == null)
        {
            return false;
        }
        return posX == other.posX && posZ == other.posZ;
    }

    /// <summary>
    /// Checks if this cell's position is within the valid grid boundaries.
    /// </summary>
    /// <param name="maxRow">Maximum number of rows (X dimension)</param>
    /// <param name="maxCol">Maximum number of columns (Z dimension)</param>
    /// <returns>True if the cell position is within grid bounds</returns>
    public bool IsValidGridPos(int maxRow, int maxCol)
    {
        bool isValidRow = posX >= 0 && posX < maxRow;
        bool isValidCol = posZ >= 0 && posZ < maxCol;
        return isValidRow && isValidCol;
    }

    /// <summary>
    /// Generates a random valid cell position adjacent to the center cell.
    /// Ensures the generated position is within grid boundaries.
    /// </summary>
    /// <param name="center">Reference cell to generate adjacent position from</param>
    /// <param name="maxRow">Maximum number of rows in the grid</param>
    /// <param name="maxCol">Maximum number of columns in the grid</param>
    /// <returns>New cell position adjacent to the center cell</returns>
    public static CellInfo GetRandomValidCell(ref CellInfo center, int maxRow, int maxCol)
    {
        CellInfo tempCell = null;
        do
        {
            int tempX = 0;
            int tempZ = 0;
            
            // Randomly choose to modify X or Z coordinate
            if (Random.Range(0, 2) == 0) 
                tempX = Random.Range(-1, 2);  // -1, 0, or 1
            else 
                tempZ = Random.Range(-1, 2);  // -1, 0, or 1
                
            // Create new cell based on center position
            tempCell = new CellInfo(ref center);
            tempCell.posX += tempX;
            tempCell.posZ += tempZ;
        } while (!tempCell.IsValidGridPos(maxRow, maxCol)); // Keep trying until valid position found
        
        return tempCell;
    }

    /// <summary>
    /// Converts the 2D grid position to a 1D array index.
    /// Used to map grid coordinates to the linear list of instantiated cells.
    /// </summary>
    /// <param name="rowLength">Number of cells in each row (X dimension)</param>
    /// <returns>1D array index corresponding to this grid position</returns>
    public int GetIndex(int rowLength) => (posX * rowLength) + posZ;
}