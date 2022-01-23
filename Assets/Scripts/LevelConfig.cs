using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum LevelDifficulty {Easy,Hard }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private IngredientsData ingredientData;
    [SerializeField] private LevelDifficulty difficulty;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;
    [SerializeField] private float spacing;
    public IngredientsData IngredientData { get => ingredientData; set => ingredientData = value; }
    public LevelDifficulty Difficulty { get => difficulty; set => difficulty = value; }
    public int GridX { get => gridX; set => gridX = value; }
    public int GridZ { get => gridZ; set => gridZ = value; }
    public float Spacing { get => spacing; set => spacing = value; }
}
