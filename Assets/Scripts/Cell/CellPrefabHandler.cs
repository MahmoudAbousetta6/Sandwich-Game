using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPrefabHandler : MonoBehaviour
{
    [SerializeField] private List<IngredientHandler> ingredients;
    [SerializeField] private Transform pivot;
    [SerializeField] private float flippingSpeed = 3f;

    private float spacing;
    private float height = 0.2f;

    public List<IngredientHandler> Ingredients { get => ingredients; set => ingredients = value; }
    public float Spacing { get => spacing; set => spacing = value; }
    public Transform Pivot { get => pivot; set => pivot = value; }

    public bool IsValidMove() => ingredients.Count > 0;

    /// <summary>
    /// Move cell which hold ingredient to the current cell.
    /// </summary>
    public void TranslateFrom(CellPrefabHandler fromCell, Vector3 dir)
    {
        for (int i = fromCell.ingredients.Count - 1; i >= 0; i--)
            MoveIngredien(fromCell.GetIngredients(i), dir, fromCell.ingredients.Count - 1 - i);
        fromCell.ingredients.Clear();
        GameManager.Instance.PlayFlipAudio();
    }

    /// <summary>
    /// Animate ingredient movement from cell to current cell.
    /// </summary>
    private void MoveIngredien(IngredientHandler trans, Vector3 dir, int index)
    {
        trans.transform.parent = transform;
        var pos = Pivot.localPosition;
        pos.y += height;
        Pivot.localPosition = pos;
        var point = trans.GetIngredientFlipPoint(spacing, dir);
        point.y = Pivot.position.y;
        trans.FlipTo(point, flippingSpeed);
        ingredients.Add(trans);
    }

    /// <summary>
    /// Get and remove ingredient from current cell.
    /// </summary>
    public IngredientHandler GetIngredients(int index) {
        var pos = Pivot.localPosition;
        pos.y -= height;
        Pivot.localPosition = pos;
        return ingredients[index];
    }

    /// <summary>
    /// Winning condition checker based on top and base of sandwich and ingredients count.
    /// </summary>
    public bool IsWin(int tolalIngredientsCount)
    {
        if (ingredients.Count == 0) return false;
        bool isDone = tolalIngredientsCount == ingredients.Count;
        isDone &= ingredients[0].IsBread;
        isDone &= ingredients[ingredients.Count-1].IsBread;
        return isDone;
    }
}
