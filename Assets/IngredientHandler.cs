using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHandler : MonoBehaviour
{
    [SerializeField] private bool isBread;

    public bool IsBread { get => isBread; set => isBread = value; }
}