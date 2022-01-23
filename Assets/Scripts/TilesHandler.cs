using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesHandler : MonoBehaviour
{
    [SerializeField] private Transform tilesParent;
    [SerializeField] private IngredientsData ingredientsData;
    [SerializeField] private CellPrefabHandler cell;
    [SerializeField] private List<CellPrefabHandler> instantiatedCells;
    [SerializeField] private int gridX;
    [SerializeField] private int gridZ;

    [SerializeField] private float spacing = 0;
    private Vector3 gridOrigin = Vector3.zero;
    private CellPrefabHandler fromCell;
    private CellPrefabHandler toCell;
    private void Start()
    {
        SpawnGrid();
        PickAndSpawnIngerdient();
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
                toCell.TranslateFrom(fromCell);
                toCell = null;
                fromCell = null;
            }
        }
    }

    private bool IsValidMove(CellPrefabHandler from, CellPrefabHandler to) {
        bool isValid = fromCell != null && toCell != null;
        if (!isValid) return false;
        isValid &= fromCell != toCell;
        int fromIndex = instantiatedCells.IndexOf(from);
        int toIndex = instantiatedCells.IndexOf(to);
        int diff= Mathf.Abs(fromIndex - toIndex);
        isValid &= (diff == 1) || (diff == 4);
        isValid &= toCell.IsValidMove();
        return isValid;
    }
    private void SpawnGrid()
    {
        int randomGridX = Random.Range(2, gridX);
        int randomGridZ = Random.Range(2, gridZ);

       // int ii = Random.Range(0, gridX);
        //int kk = Random.Range(0, gridZ);

        print(randomGridX);
        print(randomGridZ);
       // print(ii);
       // print(kk);

        for (int i = 0; i < gridX; i++)
            for (int k = 0; k < gridZ; k++)
            {
                Vector3 spawnPosition = new Vector3(i * spacing, 0, k * spacing) + gridOrigin;

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
      
        var locatios = GetRandomLocation();
        int counter = 0;
        foreach (var item in locatios)
        {
           
            int randomIndex = Random.Range(1, ingredientsData.Ingredients.Count);
            Transform obj = null;
            if (counter == 0 || counter == locatios.Count-1)
            {
                obj = ingredientsData.Ingredients[0].ingredientPrefab;
             
               
            }
            else {
                obj = ingredientsData.Ingredients[randomIndex].ingredientPrefab;
             
            }
            var index = item.GetIndex(gridX);
            var t = Instantiate(obj, instantiatedCells[index].transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            instantiatedCells[index].Ingredients.Add(t);
            Debug.Log($"{obj.name}, pos { index} - counter {counter}");
            counter++;
        }

    }

    public HashSet<CellInfo> GetRandomLocation() {
        HashSet<CellInfo> cells = new HashSet<CellInfo>();
        int temp = gridX * gridZ;
        CellInfo current = null;
        int cellsNum = 2 + Random.Range(2, temp - 2);
        do {
            if (current == null)
            {
                int x = Random.Range(0, gridX);
                int z = Random.Range(0, gridZ);
                var info = new CellInfo(x, z);
                current = info;
                cells.Add(current);
            }
            else {
                current = CellInfo.GetRandomValidCell(ref current,gridX,gridZ);
                cells.Add(current);
            }
        } while (cells.Count < cellsNum);
        return cells;
    }
}
[System.Serializable]
public class CellInfo {
    int posX;
    int posZ;
    public CellInfo(int _posX = 0, int _posZ = 0) {
        posX = _posX;
        posZ = _posZ;
    }

    public CellInfo(ref CellInfo cell) {
        posX = cell.posX;
        posZ = cell.posZ;
    }
    public override int GetHashCode()
    {
        int hash = 3;
        hash = 97 * hash + posX;
        hash = 97 * hash + posZ;
        return hash;
    }
    public override bool Equals(object obj)
    {
        var other = obj as CellInfo;
        if (other == null)
        {
            return false;
        }
        return posX == other.posX && posZ == other.posZ;
    }
    public bool IsValidGridPos(int maxRow, int maxCol) {
        bool isValidRow = posX >= 0 && posX < maxRow;
        bool isValidCol = posZ >= 0 && posZ < maxCol;
        return isValidRow && isValidCol;
    }
    public static CellInfo GetRandomValidCell(ref CellInfo center, int maxRow, int maxCol) {
        CellInfo tempCell = null;
        do {

            int tempX = 0;
            int tempZ = 0;
            if (Random.Range(0, 2) == 0) tempX = Random.Range(-1, 2);
            else tempZ = Random.Range(-1, 2);
            tempCell = new CellInfo(ref center);
            tempCell.posX += tempX;
            tempCell.posZ += tempZ;
        } while (!tempCell.IsValidGridPos(maxRow,maxCol));
        return tempCell;
    }
    public int GetIndex(int rowLength) => (posX * rowLength) + posZ;
}