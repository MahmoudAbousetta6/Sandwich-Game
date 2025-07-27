using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration defining the difficulty levels for the sandwich puzzle game.
/// Easy: Fewer ingredients and simpler puzzles
/// Hard: More ingredients and complex puzzles
/// </summary>
[System.Serializable]
public enum LevelDifficulty {Easy,Hard }

/// <summary>
/// ScriptableObject that holds configuration data for game levels.
/// Contains grid dimensions, spacing, difficulty settings, and ingredient data.
/// This allows for easy level creation and modification in the Unity editor.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    // Configuration fields
    [SerializeField] private IngredientsData ingredientData;  // Reference to available ingredients
    [SerializeField] private LevelDifficulty difficulty;      // Current level difficulty
    [SerializeField] private int gridX;                       // Number of cells in X direction
    [SerializeField] private int gridZ;                       // Number of cells in Z direction
    [SerializeField] private float spacing;                   // Distance between grid cells

    // Public properties for accessing configuration data
    /// <summary>
    /// Gets or sets the ingredient data containing all available ingredients for this level.
    /// </summary>
    public IngredientsData IngredientData { get => ingredientData; set => ingredientData = value; }
    
    /// <summary>
    /// Gets or sets the difficulty level which affects puzzle complexity.
    /// </summary>
    public LevelDifficulty Difficulty { get => difficulty; set => difficulty = value; }
    
    /// <summary>
    /// Gets or sets the grid width (number of cells in X direction).
    /// </summary>
    public int GridX { get => gridX; set => gridX = value; }
    
    /// <summary>
    /// Gets or sets the grid depth (number of cells in Z direction).
    /// </summary>
    public int GridZ { get => gridZ; set => gridZ = value; }
    
    /// <summary>
    /// Gets or sets the spacing between grid cells in world units.
    /// </summary>
    public float Spacing { get => spacing; set => spacing = value; }
}
