using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the behavior of individual grid cells in the sandwich puzzle game.
/// Manages ingredient storage, movement animations, and win condition validation.
/// </summary>
public class CellPrefabHandler : MonoBehaviour
{
    // Component references and configuration
    [SerializeField] private List<IngredientHandler> ingredients;  // List of ingredients in this cell
    [SerializeField] private Transform pivot;                      // Pivot point for ingredient stacking
    [SerializeField] private float flippingSpeed = 3f;             // Speed of ingredient flip animation

    // Audio and positioning variables
    private AudioSource flipAudioSource;                           // Audio source for flip sound effects
    private float spacing;                                         // Grid spacing for movement calculations
    private float height = 0.2f;                                   // Height offset for ingredient stacking

    // Public properties for external access
    /// <summary>
    /// Gets or sets the list of ingredients currently in this cell.
    /// </summary>
    public List<IngredientHandler> Ingredients { get => ingredients; set => ingredients = value; }
    
    /// <summary>
    /// Gets or sets the grid spacing used for movement calculations.
    /// </summary>
    public float Spacing { get => spacing; set => spacing = value; }
    
    /// <summary>
    /// Gets or sets the pivot transform used for ingredient positioning.
    /// </summary>
    public Transform Pivot { get => pivot; set => pivot = value; }

    /// <summary>
    /// Checks if this cell can be moved to (has ingredients to move).
    /// </summary>
    /// <returns>True if the cell contains ingredients</returns>
    public bool IsValidMove() => ingredients.Count > 0;

    /// <summary>
    /// Initialize audio source component on awake.
    /// </summary>
    private void Awake()
    {
        flipAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Move all ingredients from the source cell to this cell.
    /// Handles the visual animation and audio feedback for the movement.
    /// </summary>
    /// <param name="fromCell">Source cell containing ingredients to move</param>
    /// <param name="dir">Direction vector for movement animation</param>
    public void TranslateFrom(CellPrefabHandler fromCell, Vector3 dir)
    {
        // Move ingredients from the source cell to this cell
        for (int i = fromCell.ingredients.Count - 1; i >= 0; i--)
            MoveIngredien(fromCell.GetIngredients(i), dir, fromCell.ingredients.Count - 1 - i);
        
        // Clear the source cell's ingredient list
        fromCell.ingredients.Clear();
        
        // Play flip sound effect
        flipAudioSource.Play();
    }

    /// <summary>
    /// Animate ingredient movement from source cell to current cell.
    /// Creates a flipping animation and adjusts pivot height for stacking.
    /// </summary>
    /// <param name="trans">Ingredient to move</param>
    /// <param name="dir">Direction vector for movement</param>
    /// <param name="index">Index of ingredient in the stack</param>
    private void MoveIngredien(IngredientHandler trans, Vector3 dir, int index)
    {
        // Reparent ingredient to this cell
        trans.transform.parent = transform;
        
        // Adjust pivot height for proper stacking
        var pos = Pivot.localPosition;
        pos.y += height;
        Pivot.localPosition = pos;
        
        // Calculate flip point and animate movement
        var point = trans.GetIngredientFlipPoint(spacing, dir);
        point.y = Pivot.position.y;
        trans.FlipTo(point, flippingSpeed);
        
        // Add ingredient to this cell's list
        ingredients.Add(trans);
    }

    /// <summary>
    /// Get and remove ingredient from current cell.
    /// Adjusts pivot height when removing ingredients from the stack.
    /// </summary>
    /// <param name="index">Index of ingredient to retrieve</param>
    /// <returns>The ingredient at the specified index</returns>
    public IngredientHandler GetIngredients(int index) {
        // Lower pivot height when removing ingredients
        var pos = Pivot.localPosition;
        pos.y -= height;
        Pivot.localPosition = pos;
        
        return ingredients[index];
    }

    /// <summary>
    /// Winning condition checker based on top and base of sandwich and ingredients count.
    /// Validates that all ingredients are collected and properly arranged as a sandwich.
    /// </summary>
    /// <param name="tolalIngredientsCount">Total number of ingredients that should be in the sandwich</param>
    /// <returns>True if the sandwich is complete and properly structured</returns>
    public bool IsWin(int tolalIngredientsCount)
    {
        // Check if cell has any ingredients
        if (ingredients.Count == 0) return false;
        
        // Check if all ingredients are collected
        bool isDone = tolalIngredientsCount == ingredients.Count;
        
        // Check if first ingredient is bread (bottom of sandwich)
        isDone &= ingredients[0].IsBread;
        
        // Check if last ingredient is bread (top of sandwich)
        isDone &= ingredients[ingredients.Count-1].IsBread;
        
        return isDone;
    }
}
