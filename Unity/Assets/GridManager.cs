using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public GameObject imagePrefab;
    public Transform container;



    void Start()
    {
        GridLayoutGroup gridLayout = container.GetComponent<GridLayoutGroup>();

        int width = Screen.width;
        int height = Screen.height;

        float minCellSize = (float)((width + height) * 0.01 * 2);

        int cellSize = getCellSize(width,height,minCellSize);
        int gridX = width / cellSize;
        int gridY = height / cellSize;

        GridTools.Instance.RowCount = gridY;
        GridTools.Instance.ColumnCount = gridX;

        gridLayout.cellSize = new Vector2(cellSize, cellSize);

        for (int j = 1; j <= gridY; j++)
        {
            for(int i = 1; i <= gridX; i++)
            {
                GameObject image = Instantiate(imagePrefab, container);
                image.GetComponent<UnityEngine.UI.Image>().color = Color.black;
                image.AddComponent<Grid>();
                image.GetComponent<Grid>().set(i, j);
                GridTools.Instance.RegisteredGrids.Add(image);
            }
        }
        GridTools.Instance.MapReady();
    }

    public int getCellSize(int a, int b, float min)
    {
        List<int> results  = new List<int>() { 1 };
        int ABmin = Mathf.Min(a, b);

        for (int i = 2; i <= ABmin; i++)
        {
            if (a % i == 0 && b % i == 0)
            {
                results.Add(i);
            }
        }

        int endSize = results[results.Count-1];
        foreach (var result in results)
        {
            if (result >= min)
            {
                endSize = result;
                break;
            }
        }

        return endSize;
    }
}
    