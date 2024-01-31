using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridTools : MonoBehaviour
{
    private static GridTools _instance;
    public static GridTools Instance { get { return _instance; } }

    public float PathMapRatio = 0.18f;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(_instance);
        }
        else
        {
            _instance = this;
        }
        RegisteredGrids = new List<GameObject>();
    }

    public void TileClicked(GameObject element)
    {
    } // triggered when a tile is clicked



    public void MapReady()
    {
        PathTileCount = (int)(RegisteredGrids.Count * PathMapRatio);
        GenerateRandomMap();
    }

    public IEnumerator MapGenerationReady()
    {
        generationResult.result[1].GetComponent<Image>().color = Color.red;
        for(int i = 2; i < generationResult.result.Count; i++)
        {
            generationResult.result[i].GetComponent<Image>().color = Color.red;
            generationResult.result[i-1].GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void GenerateRandomMap()
    {
        StartCoroutine(GenerateRandomMapCoroutine());
    }

    public IEnumerator GenerateRandomMapCoroutine()
    {
        var start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        List<GameObject> pathAttempt = new List<GameObject>();

        while (pathAttempt.Count<PathTileCount)
        {
            foreach (GameObject g in RegisteredGrids)
            {
                g.GetComponent<Image>().color = Color.black;
                g.tag = "Untagged";
            }

            pathAttempt.Clear();

            int index = 0;

            GameObject starter = getTile(Random.Range((int)((int)ColumnCount * 0.1), (int)((int)ColumnCount * 0.9)), Random.Range((int)((int)RowCount * 0.1), (int)((int)RowCount * 0.9)));
            pathAttempt.Add(starter);   
            starter.tag = "map_starter";    
            starter.GetComponent<Image>().color = Color.green;

            var current = getSurroundingTiles(starter);
            GameObject next = null;
            while (index < PathTileCount && current.Count > 0)
            {
                index++;
                next = current[Random.Range(0, current.Count)];
                next.tag = "map_tile";
                pathAttempt.Add(next);
                next.GetComponent<Image>().color = Color.white;
                current = getSurroundingTiles(next);
            }

            next.GetComponent<Image>().color = Color.red;

            yield return new WaitForSeconds(0.15f);
        }
        generationResult = new MapGenerationResult(pathAttempt);
        StartCoroutine(MapGenerationReady());
    }

    public List<GameObject> getSurroundingTiles(GameObject tile)
    {
        Grid g = tile.GetComponent<Grid>();
        int x = (int)g.get().x;
        int y = (int)g.get().y;

        List<List<GameObject>> tiles = new List<List<GameObject>>
        {
            new List<GameObject>() { getTile(x+1,y+1), getTile(x-1,y+1), getTile(x+1,y+2), getTile(x-1,y+2), getTile(x,y+2), getTile(x,y+1) },
            new List<GameObject>() { getTile(x + 1, y - 1), getTile(x - 1, y - 1), getTile(x+1,y-2), getTile(x-1,y-2), getTile(x,y-2), getTile(x, y - 1), },
            new List<GameObject>() { getTile(x - 1, y + 1), getTile(x - 1, y - 1), getTile(x-2,y+1), getTile(x-2,y-1), getTile(x-2,y), getTile(x-1, y) },
            new List<GameObject>() { getTile(x + 1, y + 1), getTile(x + 1, y - 1), getTile(x+2,y+1), getTile(x+2,y-1), getTile(x+2,y), getTile(x+1, y) },
        };

        List<GameObject> result = new List<GameObject>();
        foreach(List<GameObject> entry in tiles)
        {
            bool match = true;
            for(int i = 0; i < entry.Count; i++)
            {
                var go = entry[i];
                if(!(go!=null && go.tag!="map_tile" && go.tag != "map_starter"))
                {
                    match = false;
                }
            }

            if(match)
            {
                result.Add(entry[entry.Count-1]);
            }
        }
        return result;
    }

    public GameObject getTile(int x, int y)
    {
        Vector2 vec = new Vector2(x, y);
        foreach(GameObject obj in RegisteredGrids){
            Grid g = obj.GetComponent<Grid>();
            if(g.get() == vec)
            {
                return obj;
            }
        }
        return null;
    }

    public int PathTileCount = -1;


    public MapGenerationResult generationResult;
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public List<GameObject> RegisteredGrids { get; set; }

}

public class MapGenerationResult
{
    public List<GameObject> result { get; set; }

    MapGenerationResult() { }
    public MapGenerationResult(List<GameObject> result)
    {
        this.result = result;
    }
}