using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the behavior of individual ingredients in the sandwich puzzle game.
/// Manages flipping animations, movement calculations, and ingredient type identification.
/// </summary>
public class IngredientHandler : MonoBehaviour
{
    // Ingredient properties
    [SerializeField] private bool isBread;                    // Flag indicating if this ingredient is bread
    
    // Animation state
    private bool isFlipping = false;                          // Flag to prevent multiple simultaneous flips
    
    // Collision bounds for movement calculations
    private Bounds bound;                                     // Bounding box of the ingredient

    /// <summary>
    /// Gets or sets whether this ingredient is bread (used for sandwich structure validation).
    /// </summary>
    public bool IsBread { get => isBread; set => isBread = value; }

    /// <summary>
    /// Initialize the ingredient's bounding box on start.
    /// </summary>
    private void Start()
    {
        bound = GetComponent<BoxCollider>().bounds;
    }

    /// <summary>
    /// Initiates a flipping animation to the specified position.
    /// Prevents multiple simultaneous flip animations.
    /// </summary>
    /// <param name="positionToRotation">Target position for the flip animation</param>
    /// <param name="rotationSpeed">Speed of the rotation animation</param>
    public void FlipTo(Vector3 positionToRotation, float rotationSpeed)
    {
        // Prevent multiple simultaneous flips
        if (isFlipping) return;
        
        isFlipping = true;
        StartCoroutine(FlipIngredient(positionToRotation, rotationSpeed));
    }

    /// <summary>
    /// Coroutine that handles the flipping animation of the ingredient.
    /// Creates a smooth 180-degree rotation around a calculated axis.
    /// </summary>
    /// <param name="positionToRotation">Target position for the flip</param>
    /// <param name="rotationSpeed">Speed of the rotation</param>
    /// <returns>IEnumerator for coroutine execution</returns>
    private IEnumerator FlipIngredient(Vector3 positionToRotation, float rotationSpeed)
    {
        float angle = 0;
        
        // Calculate the rotation point and axis
        Vector3 point = transform.position + positionToRotation;
        Vector3 axis = Vector3.Cross(Vector3.up, positionToRotation).normalized;

        // Perform 180-degree rotation
        while (angle < 180f)
        {
            float angleSpeed = Time.deltaTime + rotationSpeed;
            transform.RotateAround(point, axis, angleSpeed);
            angle += angleSpeed;
            yield return null;
        }
        
        // Ensure exact 180-degree rotation and set final position
        transform.RotateAround(point, axis, 180 - angle);
        transform.position = new Vector3(transform.position.x, positionToRotation.y, transform.position.z);

        // Reset flipping state
        isFlipping = false;
    }

    /// <summary>
    /// Get the ingredient flip points based on direction and grid spacing.
    /// Calculates the appropriate rotation point for smooth flipping animation.
    /// </summary>
    /// <param name="spacing">Grid spacing between cells</param>
    /// <param name="dir">Direction vector for movement</param>
    /// <returns>Vector3 representing the flip point relative to the ingredient</returns>
    public Vector3 GetIngredientFlipPoint(float spacing, Vector3 dir)
    {
        Vector3 point = Vector3.zero;
        
        // Calculate available space for movement
        var spaceH = spacing - (bound.size.x);  // Horizontal space
        var spaceV = spacing - (bound.size.z);  // Vertical space
        
        // Determine flip point based on movement direction
        if (dir.z == 1) 
        { 
            // Moving forward (up in grid)
            point = new Vector3((-bound.size.x / 2) - (spaceH / 2), -bound.size.y / 2, 0); 
        }
        else if (dir.z == -1) 
        { 
            // Moving backward (down in grid)
            point = new Vector3(bound.size.x / 2 + (spaceH / 2), -bound.size.y / 2, 0); 
        }
        else if (dir.x == 1) 
        { 
            // Moving right
            point = new Vector3(0, -bound.size.y / 2, bound.size.z / 2 + (spaceV / 2)); 
        }
        else if (dir.x == -1) 
        { 
            // Moving left
            point = new Vector3(0, -bound.size.y / 2, -bound.size.z / 2 - (spaceV / 2)); 
        }
        
        return point;
    }
}