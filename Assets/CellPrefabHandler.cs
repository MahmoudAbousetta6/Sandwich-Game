using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CellPrefabHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> ingredients;

    public List<Transform> Ingredients { get => ingredients; set => ingredients = value; }

    public bool IsValidMove() => ingredients.Count > 0;

    public void TranslateFrom(CellPrefabHandler fromCell) {
        for (int i = 0; i < fromCell.ingredients.Count; i++)
        {
            MoveIngredien(fromCell.ingredients[i]);
        }
        fromCell.ingredients.Clear();
    }

    private void MoveIngredien(Transform trans) {
        trans.parent = transform;
        
        trans.localPosition = Vector3.zero;
        ingredients.Add(trans);
    }
}
