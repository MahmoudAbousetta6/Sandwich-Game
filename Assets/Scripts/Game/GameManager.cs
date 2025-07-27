using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Main game controller that manages the sandwich puzzle game.
/// Handles grid creation, ingredient spawning, player input, win conditions, and UI interactions.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Grid and level configuration references
    [SerializeField] private Transform cellParent;           // Parent transform for all grid cells
    [SerializeField] private LevelConfig config;             // Level configuration data
    [SerializeField] private CellPrefabHandler cell;         // Cell prefab template
    
    // Camera and UI references
    [SerializeField] private CinemachineVirtualCamera cam;   // Virtual camera for dynamic camera control
    [SerializeField] private TMP_Text scoreText;             // Text component for displaying score
    [SerializeField] private Button resetGameButton;         // Button to reset current level
    [SerializeField] private Button winningGameResetGameButton; // Button to reset game from win screen
    [SerializeField] private Button generateLevelButton;     // Button to generate new level
    [SerializeField] private Button nextLevelButton;         // Button to proceed to next level
    [SerializeField] private GameObject inGamePanel;         // Main game UI panel
    [SerializeField] private GameObject winningPanel;        // Win screen UI panel

    // Game state variables
    private float stratTime;                                 // Start time for calculating score
    private float endTime;                                   // End time for calculating score
    private List<CellPrefabHandler> instantiatedCells = new List<CellPrefabHandler>(); // All created grid cells

    // Grid and movement variables
    private Vector3 gridOrigin = Vector3.zero;               // Origin point of the grid
    private CellPrefabHandler fromCell;                      // Cell where drag started
    private CellPrefabHandler toCell;                        // Cell where drag ended
    private HashSet<CellInfo> gameLocation;                  // Valid positions for ingredients

    // Win condition variables
    private Transform winningPivotTransform;                 // Transform to focus camera on win

    /// <summary>
    /// Initialize the game on start. Set up UI buttons, spawn grid, and start timer.
    /// </summary>
    private void Start()
    {
        // Hide win panel initially
        winningPanel.SetActive(false);
        
        // Set up button click listeners
        resetGameButton.onClick.AddListener(ResetGameButtonClick);
        winningGameResetGameButton.onClick.AddListener(ResetGameButtonClick);
        generateLevelButton.onClick.AddListener(GenerateLevelButtonClick);
        nextLevelButton.onClick.AddListener(GenerateLevelButtonClick);
        
        // Initialize game elements
        SpawnGrid();                    // Create the game grid
        PickAndSpawnIngerdient();       // Place ingredients randomly
        stratTime = System.DateTime.UtcNow.Second; // Start timer
    }
   
    /// <summary>
    /// Handle player input for dragging ingredients between cells.
    /// Detects mouse down/up events and validates moves.
    /// </summary>
    private void Update()
    {
        // Handle mouse button down - start of drag operation
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if we clicked on a cell
            if (Physics.Raycast(ray, out hit))
                fromCell = hit.collider.gameObject.GetComponent<CellPrefabHandler>();
            else
                fromCell = null;
        }

        // Handle mouse button up - end of drag operation
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if we released on a cell
            if (Physics.Raycast(ray, out hit))
                toCell = hit.collider.gameObject.GetComponent<CellPrefabHandler>();
            else
                toCell = null;

            // Validate and execute the move if valid
            if (IsValidMove(fromCell, toCell))
            {
                var dir = GetDircation(fromCell, toCell);
                toCell.TranslateFrom(fromCell, dir);
            }

            // Check for win condition
            if (toCell && toCell.IsWin(gameLocation.Count))
            {
                winningPivotTransform = toCell.Pivot;
                Invoke(nameof(Win), 1f); // Trigger win sequence after 1 second
                endTime = System.DateTime.UtcNow.Second; // Record end time
            }

            // Reset drag state
            toCell = null;
            fromCell = null;
        }
    }

    /// <summary>
    /// Reset the same level including same locations and ingredients.
    /// Clears all cells and respawns them with the same configuration.
    /// </summary>
    private void ResetGameButtonClick()
    {
        // Reset camera priority
        cam.Priority = 01;
        
        // Reset timer and UI
        stratTime = System.DateTime.UtcNow.Second;
        winningPanel.SetActive(false);
        inGamePanel.SetActive(true);
        
        // Destroy all existing cells
        for (int i = 0; i < instantiatedCells.Count; i++)
            Destroy(instantiatedCells[i].gameObject);

        instantiatedCells.Clear();
        
        // Recreate grid and ingredients
        SpawnGrid();

        // Respawn ingredients in their original positions
        foreach (var item in gameLocation)
        {
            var obj = config.IngredientData.Ingredients[item.InstantiatedIndex].ingredientPrefab;
            var index = item.GetIndex(config.GridX);
            var t = Instantiate(obj, instantiatedCells[index].transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
            instantiatedCells[index].Ingredients.Add(t);
        }
    }

    /// <summary>
    /// Generate a completely new level by reloading the scene.
    /// </summary>
    private void GenerateLevelButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Handle win sequence - switch camera focus and show win panel.
    /// </summary>
    private void Win()
    {
        inGamePanel.SetActive(false);
        
        // Focus camera on the winning sandwich
        cam.LookAt = winningPivotTransform;
        cam.Follow = winningPivotTransform;
        cam.Priority = 100;
        
        // Show win panel after camera transition
        Invoke(nameof(ShowWinningPanel), 2.5f);
    }

    /// <summary>
    /// Display the winning panel with calculated score.
    /// Score is based on completion time - faster completion = higher score.
    /// </summary>
    private void ShowWinningPanel()
    {
        var time = endTime - stratTime;
        var score = (1 / time) * 100 + 10; // Calculate score based on time
        scoreText.text = score.ToString("#.##");
        winningPanel.SetActive(true);
    }

    /// <summary>
    /// Get directions between two cells based on the movement between current and next cell.
    /// Calculates the direction vector for ingredient movement animation.
    /// </summary>
    /// <param name="from">Source cell</param>
    /// <param name="to">Destination cell</param>
    /// <returns>Direction vector for movement</returns>
    private Vector3 GetDircation(CellPrefabHandler from, CellPrefabHandler to)
    {
        Vector3 dir = Vector3.zero;

        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff = (fromIndex - toIndex);

        // Determine direction based on index difference
        if (diff == 1)           // Moving left
            dir.x = -1;
        else if (diff == -1)     // Moving right
            dir.x = 1;
        else if (diff == config.GridX)  // Moving down
            dir.z = 1;
        else if (diff == -config.GridX) // Moving up
            dir.z = -1;
        return dir;
    }

    /// <summary>
    /// Check if the cell is valid to move.
    /// Prevents moving out of the grid.
    /// Prevents moving empty cell.
    /// Ensures cells are adjacent and destination is valid.
    /// </summary>
    /// <param name="from">Source cell</param>
    /// <param name="to">Destination cell</param>
    /// <returns>True if move is valid, false otherwise</returns>
    private bool IsValidMove(CellPrefabHandler from, CellPrefabHandler to)
    {
        // Basic validation - both cells must exist
        bool isValid = fromCell != null && toCell != null;
        if (!isValid) return false;

        // Cells must be different
        isValid &= fromCell != toCell;
        
        // Calculate if cells are adjacent
        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff = Mathf.Abs(fromIndex - toIndex);
        isValid &= (diff == 1) || (diff == config.GridX); // Must be adjacent horizontally or vertically
        
        // Destination cell must be valid for movement
        isValid &= toCell.IsValidMove();

        return isValid;
    }

    /// <summary>
    /// Create grid based on size assigned in configuration.
    /// Spawns cells in a rectangular grid pattern.
    /// </summary>
    private void SpawnGrid()
    {
        // Create grid cells in X and Z dimensions
        for (int i = 0; i < config.GridX; i++)
            for (int k = 0; k < config.GridZ; k++)
            {
                Vector3 spawnPosition = new Vector3(i * config.Spacing, 0, k * config.Spacing) + gridOrigin;
                CreateCells(spawnPosition, Quaternion.identity);
            }
    }

    /// <summary>
    /// Instantiate individual cells at specified position.
    /// </summary>
    /// <param name="positionToSpawn">World position for the cell</param>
    /// <param name="rotation">Rotation for the cell</param>
    private void CreateCells(Vector3 positionToSpawn, Quaternion rotation)
    {
        CellPrefabHandler cells = Instantiate(cell, positionToSpawn, rotation, cellParent);
        cells.Spacing = config.Spacing;
        instantiatedCells.Add(cells);
    }

    /// <summary>
    /// Instantiate ingredients in random cells location.
    /// Creates a connected path of ingredients for the puzzle.
    /// </summary>
    private void PickAndSpawnIngerdient()
    {
        // Get random connected locations for ingredients
        gameLocation = GetRandomLocation();

        int counter = 0;
        int levelIndex = 1;

        // Spawn ingredients at each location
        foreach (var item in gameLocation)
        {
            int randomIndex = Random.Range(1, config.IngredientData.Ingredients.Count);
            IngredientHandler obj = null;

            // Ensure first and last ingredients are bread (sandwich structure)
            if (counter == 0 || counter == levelIndex)
                randomIndex = 0;

            item.InstantiatedIndex = randomIndex;
            obj = config.IngredientData.Ingredients[randomIndex].ingredientPrefab;
            var index = item.GetIndex(config.GridX);
            var t = Instantiate(obj, instantiatedCells[index].transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
            instantiatedCells[index].Ingredients.Add(t);
            counter++;
        }
    }

    /// <summary>
    /// Generate random locations from the created grid cells for ingredients.
    /// The generated random locations are connected to each other to form a solvable puzzle.
    /// </summary>
    /// <returns>Set of connected cell positions for ingredients</returns>
    private HashSet<CellInfo> GetRandomLocation()
    {
        HashSet<CellInfo> cells = new HashSet<CellInfo>();
        
        // Calculate maximum number of cells based on difficulty
        int temp = config.GridX * config.GridZ;
        temp /= config.Difficulty == LevelDifficulty.Easy ? 2 : 1;
        
        CellInfo current = null;
        int cellsNum = 2 + Random.Range(2, temp - 2); // Random number of cells (minimum 4)
        
        // Generate connected path of cells
        do
        {
            if (current == null)
            {
                // Start with random position
                int x = Random.Range(0, config.GridX);
                int z = Random.Range(0, config.GridZ);
                var info = new CellInfo(x, z);
                current = info;
                cells.Add(current);
            }
            else
            {
                // Get random adjacent cell
                current = CellInfo.GetRandomValidCell(ref current, config.GridX, config.GridZ);
                cells.Add(current);
            }
        } while (cells.Count < cellsNum);
        
        return cells;
    }
}