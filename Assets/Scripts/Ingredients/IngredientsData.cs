using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds a collection of available ingredients for the sandwich puzzle game.
/// Allows for easy management and configuration of ingredients in the Unity editor.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ingredients", order = 1)]
public class IngredientsData : ScriptableObject
{
    // Ingredient collection
    [SerializeField] private List<Ingredients> ingredients;  // List of all available ingredients

    /// <summary>
    /// Gets or sets the list of ingredients available in this data asset.
    /// </summary>
    internal List<Ingredients> Ingredients { get => ingredients; set => ingredients = value; }
}

/// <summary>
/// Serializable structure that represents a single ingredient in the game.
/// Contains the prefab reference for instantiating the ingredient in the scene.
/// </summary>
[System.Serializable]
struct Ingredients
{
    public IngredientHandler ingredientPrefab;  // Prefab reference for the ingredient
}