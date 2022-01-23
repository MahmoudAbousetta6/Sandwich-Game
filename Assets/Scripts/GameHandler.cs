using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Transform tilesParent;
    [SerializeField] private LevelConfig config;
    [SerializeField] private CellPrefabHandler cell;
     private List<CellPrefabHandler> instantiatedCells = new List<CellPrefabHandler>();

    
    private Vector3 gridOrigin = Vector3.zero;
    private CellPrefabHandler fromCell;
    private CellPrefabHandler toCell;
    private HashSet<CellInfo> gameLocation;
    private void Start()
    {
        SpawnGrid();
        PickAndSpawnIngerdient();
    }

    public Vector3 GetDircation(CellPrefabHandler from, CellPrefabHandler to) {
        Vector3 dir = Vector3.zero;
        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff = (fromIndex - toIndex);
        if (diff == 1) dir.x = -1;
        else if (diff == -1) dir.x = 1;
        else if (diff == config.GridX) dir.z = 1;
        else if (diff == -config.GridX) dir.z = -1;
        return dir;
    }


    [ContextMenu("ResetGame")]
    public void ResetGame()
    {
        for (int i = 0; i < instantiatedCells.Count; i++)
        {
            Destroy(instantiatedCells[i].gameObject);
        }
        instantiatedCells.Clear();
        SpawnGrid();
        int counter = 0;
        foreach (var item in gameLocation)
        {
            var obj = config.IngredientData.Ingredients[item.InstantiatedIndex].ingredientPrefab;
            var index = item.GetIndex(config.GridX);
            var t = Instantiate(obj, instantiatedCells[index].transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
            instantiatedCells[index].Ingredients.Add(t);
            Debug.Log($"{obj.name}, pos { index} - counter {counter}");

        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                fromCell = hit.collider.gameObject.GetComponent<CellPrefabHandler>();
            }
            else fromCell = null;
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                toCell = hit.collider.gameObject.GetComponent<CellPrefabHandler>();
            }
            else toCell = null;

            if (IsValidMove(fromCell,toCell)) {
                var dir = GetDircation(fromCell, toCell);
                toCell.TranslateFrom(fromCell,dir);
               
            }
            if (toCell && toCell.IsWin(gameLocation.Count)) { Debug.LogError("Win"); }
            toCell = null;
            fromCell = null;
        }
    }

    private bool IsValidMove(CellPrefabHandler from, CellPrefabHandler to) {
        bool isValid = fromCell != null && toCell != null;
        if (!isValid) return false;
        isValid &= fromCell != toCell;
        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff= Mathf.Abs(fromIndex - toIndex);
        isValid &= (diff == 1) || (diff == config.GridX);
        isValid &= toCell.IsValidMove();
        return isValid;
    }
    private void SpawnGrid()
    {
        int randomGridX = Random.Range(2, config.GridX);
        int randomGridZ = Random.Range(2, config.GridZ);

        for (int i = 0; i < config.GridX; i++)
            for (int k = 0; k < config.GridZ; k++)
            {
                Vector3 spawnPosition = new Vector3(i * config.Spacing, 0, k * config.Spacing) + gridOrigin;

                CreateCells(spawnPosition, Quaternion.identity);
            }
    }

    private void CreateCells(Vector3 positionToSpawn, Quaternion rotation)
    {        
        CellPrefabHandler cells = Instantiate(cell, positionToSpawn, rotation, tilesParent);
        instantiatedCells.Add(cells);
    }

    private void PickAndSpawnIngerdient()
    {

        gameLocation = GetRandomLocation();
        int counter = 0;
        int levelIndex = config.Difficulty == LevelDifficulty.Easy ? 1 : gameLocation.Count - 1;
        foreach (var item in gameLocation)
        {
           
            int randomIndex = Random.Range(1, config.IngredientData.Ingredients.Count);
            IngredientHandler obj = null;
            if (counter == 0 || counter == levelIndex)
            {
                randomIndex = 0;
            }
            item.InstantiatedIndex = randomIndex;
            obj = config.IngredientData.Ingredients[randomIndex].ingredientPrefab;
            var index = item.GetIndex(config.GridX);
            var t = Instantiate(obj, instantiatedCells[index].transform);
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
            instantiatedCells[index].Ingredients.Add(t);
            Debug.Log($"{obj.name}, pos { index} - counter {counter}");
            counter++;
        }

    }

    public HashSet<CellInfo> GetRandomLocation() {
        HashSet<CellInfo> cells = new HashSet<CellInfo>();
        int temp = config.GridX * config.GridZ;
        temp /= config.Difficulty == LevelDifficulty.Easy ? 2 : 1;
        CellInfo current = null;
        int cellsNum = 2 + Random.Range(2, temp - 2);
        do {
            if (current == null)
            {
                int x = Random.Range(0, config.GridX);
                int z = Random.Range(0, config.GridZ);
                var info = new CellInfo(x, z);
                current = info;
                cells.Add(current);
            }
            else {
                current = CellInfo.GetRandomValidCell(ref current, config.GridX, config.GridZ);
                cells.Add(current);
            }
        } while (cells.Count < cellsNum);
        return cells;
    }
}
