using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientHandler : MonoBehaviour
{
    [SerializeField] private bool isBread;
    private bool isFlipping = false;
    private Bounds bound;

    public bool IsBread { get => isBread; set => isBread = value; }

    private void Start()
    {
        bound = GetComponent<BoxCollider>().bounds;
    }

    public void FlipTo(Vector3 positionToRotation, float rotationSpeed)
    {
        if (isFlipping) return;
        isFlipping = true;
        StartCoroutine(FlipIngredient(positionToRotation, rotationSpeed));
    }

    /// <summary>
    /// Flip ingredient transition.
    /// </summary>
    private IEnumerator FlipIngredient(Vector3 positionToRotation, float rotationSpeed)
    {
        float angle = 0;
        Vector3 point = transform.position + positionToRotation;
        Vector3 axis = Vector3.Cross(Vector3.up, positionToRotation).normalized;

        while (angle < 180f)
        {
            float angleSpeed = Time.deltaTime + rotationSpeed;
            transform.RotateAround(point, axis, angleSpeed);
            angle += angleSpeed;
            yield return null;
        }
        
        transform.RotateAround(point, axis, 180 - angle);
        transform.position = new Vector3(transform.position.x, positionToRotation.y, transform.position.z);

        isFlipping = false;
    }

    /// <summary>
    /// Get the ingredient flip points based on direction.
    /// </summary>
    public Vector3 GetIngredientFlipPoint(float spacing, Vector3 dir)
    {
        Vector3 point = Vector3.zero;
        var spaceH = spacing - (bound.size.x);
        var spaceV = spacing - (bound.size.z);
        if (dir.z == 1) { point = new Vector3((-bound.size.x / 2) - (spaceH / 2), -bound.size.y / 2, 0); }
        else if (dir.z == -1) { point = new Vector3(bound.size.x / 2 + (spaceH / 2), -bound.size.y / 2, 0); }
        else if (dir.x == 1) { point = new Vector3(0, -bound.size.y / 2, bound.size.z / 2 + (spaceV / 2)); }
        else if (dir.x == -1) { point = new Vector3(0, -bound.size.y / 2, -bound.size.z / 2 - (spaceV / 2)); }
        return point;
    }
}