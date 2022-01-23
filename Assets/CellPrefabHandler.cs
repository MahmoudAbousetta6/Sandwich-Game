using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CellPrefabHandler : MonoBehaviour
{
    [SerializeField] private List<IngredientHandler> ingredients;
    [SerializeField] private Transform pivot;

    [SerializeField] private IngredientsData x;

    private float height = 0.2f;
    public List<IngredientHandler> Ingredients { get => ingredients; set => ingredients = value; }

    public bool IsValidMove() => ingredients.Count > 0;

    public void TranslateFrom(CellPrefabHandler fromCell, Vector3 dir) {
        for (int i = fromCell.ingredients.Count-1; i >= 0; i--)
        {
            MoveIngredien(fromCell.GetIngredients(i),dir, fromCell.ingredients.Count-1-i);
        }
        fromCell.ingredients.Clear();
    }

    private void MoveIngredien(IngredientHandler trans,Vector3 dir,int index) {
        trans.transform.parent = transform;
        var pos = pivot.localPosition;
        pos.y += height;
        pivot.localPosition = pos;
        trans.transform.DOLocalJump (pivot.localPosition,2 + (index * 0.5f), 1, 2+(index * 0.35f));
        trans.transform.DORotate(dir*180,0.7f + (index * 0.2f));
        trans.transform.localPosition = Vector3.zero;
        ingredients.Add(trans);
    }
    public IngredientHandler GetIngredients(int index) {
        var pos = pivot.localPosition;
        pos.y -= height;
        pivot.localPosition = pos;
        return ingredients[index];
    }

    public bool IsWin(int tolalIngredientsCount)
    {
        if (ingredients.Count == 0) return false;
        bool isDone = tolalIngredientsCount == ingredients.Count;
        isDone &= ingredients[0].IsBread;
        isDone &= ingredients[ingredients.Count-1].IsBread;
        return isDone;
    }
}
