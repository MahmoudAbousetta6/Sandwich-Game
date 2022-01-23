using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ingredients", order = 1)]
public class IngredientsData : ScriptableObject
{
    [SerializeField] private List<Ingredients> ingredients;

    internal List<Ingredients> Ingredients { get => ingredients; set => ingredients = value; }
}

[System.Serializable]
struct Ingredients
{
    public IngredientHandler ingredientPrefab;
}