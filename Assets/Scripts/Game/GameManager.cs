using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private Transform cellParent;
    [SerializeField] private LevelConfig config;
    [SerializeField] private CellPrefabHandler cell;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button resetGameButton;
    [SerializeField] private Button winningGameResetGameButton;
    [SerializeField] private Button generateLevelButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject winningPanel;

    [SerializeField] private AudioSource flipAudioSource;

    private float stratTime;
    private float endTime;
    private List<CellPrefabHandler> instantiatedCells = new List<CellPrefabHandler>();


    private Vector3 gridOrigin = Vector3.zero;
    private CellPrefabHandler fromCell;
    private CellPrefabHandler toCell;
    private HashSet<CellInfo> gameLocation;

    private Transform winningPivotTransform;

    public static GameManager Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        instance ??= this;
    }

    private void Start()
    {
        winningPanel.SetActive(false);
        resetGameButton.onClick.AddListener(ResetGameButtonClick);
        winningGameResetGameButton.onClick.AddListener(ResetGameButtonClick);
        generateLevelButton.onClick.AddListener(GenerateLevelButtonClick);
        nextLevelButton.onClick.AddListener(GenerateLevelButtonClick);
        SpawnGrid();
        PickAndSpawnIngerdient();
        stratTime = System.DateTime.UtcNow.Second;
    }
   
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
                fromCell = hit.collider.gameObject.GetComponent<CellPrefabHandler>();
            else
                fromCell = null;
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
                toCell = hit.collider.gameObject.GetComponent<CellPrefabHandler>();
            else
                toCell = null;

            if (IsValidMove(fromCell, toCell))
            {
                var dir = GetDircation(fromCell, toCell);
                toCell.TranslateFrom(fromCell, dir);
            }

            if (toCell && toCell.IsWin(gameLocation.Count))
            {
                winningPivotTransform = toCell.Pivot;
                Invoke(nameof(Win), 1f);
                endTime = System.DateTime.UtcNow.Second;

            }

            toCell = null;
            fromCell = null;
        }
    }

    /// <summary>
    /// Play flip audio on flip ingredient.
    /// </summary>
    public void PlayFlipAudio()
    {
        flipAudioSource.Play();
    }

    /// <summary>
    /// Reset the same level including same locations and ingredients.
    /// </summary>
    private void ResetGameButtonClick()
    {
        cam.Priority = 01;
        stratTime = System.DateTime.UtcNow.Second;
        winningPanel.SetActive(false);
        inGamePanel.SetActive(true);
        for (int i = 0; i < instantiatedCells.Count; i++)
            Destroy(instantiatedCells[i].gameObject);

        instantiatedCells.Clear();
        SpawnGrid();

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

    private void GenerateLevelButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Win()
    {
        inGamePanel.SetActive(false);
        cam.LookAt = winningPivotTransform;
        cam.Follow = winningPivotTransform;
        cam.Priority = 100;
        Invoke(nameof(ShowWinningPanel), 2.5f);
    }

    private void ShowWinningPanel()
    {
        var time = endTime - stratTime;
        var score = (1 / time) * 100 + 10;
        scoreText.text = score.ToString("#.##");
        winningPanel.SetActive(true);
    }

    /// <summary>
    /// Get directions between two cells based on the movement between current and next cell.
    /// </summary>
    private Vector3 GetDircation(CellPrefabHandler from, CellPrefabHandler to)
    {
        Vector3 dir = Vector3.zero;

        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff = (fromIndex - toIndex);

        if (diff == 1)
            dir.x = -1;
        else if (diff == -1)
            dir.x = 1;
        else if (diff == config.GridX)
            dir.z = 1;
        else if (diff == -config.GridX)
            dir.z = -1;
        return dir;
    }


    /// <summary>
    /// Check if the cell is valid to move.
    /// Prevent moving out of the grid.
    /// Prevent moving empty cell.
    /// </summary>
    private bool IsValidMove(CellPrefabHandler from, CellPrefabHandler to)
    {
        bool isValid = fromCell != null && toCell != null;
        if (!isValid) return false;

        isValid &= fromCell != toCell;
        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff = Mathf.Abs(fromIndex - toIndex);
        isValid &= (diff == 1) || (diff == config.GridX);
        isValid &= toCell.IsValidMove();

        return isValid;
    }

    /// <summary>
    /// Create grid based on size assigned in configuration.
    /// </summary>
    private void SpawnGrid()
    {
        for (int i = 0; i < config.GridX; i++)
            for (int k = 0; k < config.GridZ; k++)
            {
                Vector3 spawnPosition = new Vector3(i * config.Spacing, 0, k * config.Spacing) + gridOrigin;
                CreateCells(spawnPosition, Quaternion.identity);
            }
    }

    /// <summary>
    /// Instantiate cells.
    /// </summary>
    private void CreateCells(Vector3 positionToSpawn, Quaternion rotation)
    {
        CellPrefabHandler cells = Instantiate(cell, positionToSpawn, rotation, cellParent);
        cells.Spacing = config.Spacing;
        instantiatedCells.Add(cells);
    }

    /// <summary>
    /// Instantiate ingredients in random cells location.
    /// </summary>
    private void PickAndSpawnIngerdient()
    {
        gameLocation = GetRandomLocation();

        int counter = 0;
        int levelIndex = 1;

        foreach (var item in gameLocation)
        {
            int randomIndex = Random.Range(1, config.IngredientData.Ingredients.Count);
            IngredientHandler obj = null;

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
    /// Generate random locations from the created grid cells for ingredients,
    /// The generated random locations are connected to each other.
    /// </summary>
    /// <returns></returns>
    private HashSet<CellInfo> GetRandomLocation()
    {
        HashSet<CellInfo> cells = new HashSet<CellInfo>();
        int temp = config.GridX * config.GridZ;
        temp /= config.Difficulty == LevelDifficulty.Easy ? 2 : 1;
        CellInfo current = null;
        int cellsNum = 2 + Random.Range(2, temp - 2);
        do
        {
            if (current == null)
            {
                int x = Random.Range(0, config.GridX);
                int z = Random.Range(0, config.GridZ);
                var info = new CellInfo(x, z);
                current = info;
                cells.Add(current);
            }
            else
            {
                current = CellInfo.GetRandomValidCell(ref current, config.GridX, config.GridZ);
                cells.Add(current);
            }
        } while (cells.Count < cellsNum);
        return cells;
    }
}