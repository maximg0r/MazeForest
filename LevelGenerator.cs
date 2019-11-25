using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public static class FOV
    {
        public static float x = 1;
        public static float y = 0.75f;
        public static float step = 0.25f;
    }

    private static Transform tileRoot;
    private static Dictionary<float, List<GameObject>> columnMap;
    private static Dictionary<float, List<GameObject>> rowMap;
    private static int timesRotated = 0; // unused

    public float worldBorder = 12.5f;
    public GameObject[] grassTiles;
    public GameObject[] treeTiles;
    public GameObject finish;

    private List<GameObject> AddItem(List<GameObject> list, GameObject item)
    {
        list.Add(item);
        return list;
    }

    private void SetActiveItems(List<GameObject> list, bool state)
    {
        foreach (GameObject item in list)
        {
            if (item.activeSelf == state)
                continue;
            item.SetActive(state);
        }
    }

    private Dictionary<float, List<GameObject>> OppositeKeys(Dictionary<float, List<GameObject>> map)
    {
        Dictionary<float, List<GameObject>> temp = new Dictionary<float, List<GameObject>>();
        foreach (float key in map.Keys)
        {
            temp.Add(-key, map[key]);
        }
        return temp;
    }

    /// <summary>Do not use: doesn't make sense anymore</summary>
    private void SwapDictionaries()
    {
        // Was used to fix local-global coordinate systems
        Dictionary<float, List<GameObject>> temp = columnMap;

        if (timesRotated % 2 == 0)
        {
            columnMap = OppositeKeys(rowMap);
            rowMap = temp;
        }
        else
        {
            columnMap = rowMap;
            rowMap = OppositeKeys(temp);
        }
    }

    #region DictionaryGetters
    float GetMinColumn()
    {
        float min = float.MaxValue;
        foreach (float i in columnMap.Keys)
            if (i < min)
                min = i;

        return min;
    }

    float GetMaxColumn()
    {
        float max = float.MinValue;
        foreach (float i in columnMap.Keys)
            if (i > max)
                max = i;

        return max;
    }

    float GetMinRow()
    {
        float min = float.MaxValue;
        foreach (float i in rowMap.Keys)
            if (i < min)
                min = i;

        return min;
    }

    float GetMaxRow()
    {
        float max = float.MinValue;
        foreach (float i in rowMap.Keys)
            if (i > max)
                max = i;

        return max;
    }
    #endregion

    private void GenerateInstance(GameObject[] tiles, float x, float y)
    {
        if (Random.Range(0, 4) == 0)
            tiles = treeTiles;

        GameObject instance = Instantiate(tiles[Random.Range(0, tiles.Length)]) as GameObject; // Tiles are chosen randomly from a preset. Tune probability in the inspector.
        instance.transform.SetParent(tileRoot);
        instance.transform.localPosition = new Vector3(x, y, 0);

        if (Mathf.Abs(x) == worldBorder || Mathf.Abs(y) == worldBorder) // set world border
        {
            instance.AddComponent<BoxCollider2D>().isTrigger = true;
            instance.tag = "Border";
        }

        #region Dictionaries
        // add record of the instance into the dictionaries
        if (columnMap.ContainsKey(x))
        {
            List<GameObject> list = AddItem(columnMap[x], instance);
            columnMap.Remove(x);
            columnMap.Add(x, list);
        }
        else
        {
            columnMap.Add(x, AddItem(new List<GameObject>(), instance));
        }

        if (rowMap.ContainsKey(y))
        {
            List<GameObject> list = AddItem(rowMap[y], instance);
            rowMap.Remove(y);
            rowMap.Add(y, list);
        }
        else
        {
            rowMap.Add(y, AddItem(new List<GameObject>(), instance));
        }
        #endregion
    }

    private void GenerateSpawnArea()
    {
        for (float x = -FOV.x; x <= FOV.x; x += FOV.step)
            for (float y = -FOV.y; y <= FOV.y; y += FOV.step)
                GenerateInstance(grassTiles, x, y);
    }

    private void GenerateFinish()
    {
        GameObject instance;
        float range = Random.Range(-worldBorder + FOV.step, worldBorder - FOV.step);
        switch (Random.Range(0, 4)) // Directions (NSEW)
        {
            case 0:
                instance = Instantiate(finish, new Vector3(range, worldBorder - FOV.step, 0f), Quaternion.identity) as GameObject;
                instance.transform.Rotate(new Vector3(0, 0, 90f));
                break;
            case 1:
                instance = Instantiate(finish, new Vector3(range, -worldBorder + FOV.step, 0f), Quaternion.identity) as GameObject;
                instance.transform.Rotate(new Vector3(0, 0, -90f));
                break;
            case 2:
                instance = Instantiate(finish, new Vector3(worldBorder - FOV.step, range, 0f), Quaternion.identity) as GameObject;
                break;
            case 3:
                instance = Instantiate(finish, new Vector3(-worldBorder + FOV.step, range, 0f), Quaternion.identity) as GameObject;
                instance.transform.Rotate(new Vector3(0, 0, 180f));
                break;
        }
    }

    private void ExpandArea(float minColumn, float maxColumn, float minRow, float maxRow)
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.localPosition;

        if (Mathf.Abs(playerPos.x - minColumn) < FOV.x) // expand left border of the map
            for (float y = minRow; y <= maxRow; y += FOV.step)
            {
                GenerateInstance(grassTiles, minColumn - FOV.step, y);
            }
        else if (Mathf.Abs(playerPos.x - maxColumn) < FOV.x) // expand right border of the map
            for (float y = minRow; y <= maxRow; y += FOV.step)
            {
                GenerateInstance(grassTiles, maxColumn + FOV.step, y);
            }

        if (Mathf.Abs(playerPos.y - minRow) < FOV.y) // expand down border of the map
            for (float x = minColumn; x <= maxColumn; x += FOV.step)
            {
                GenerateInstance(grassTiles, x, minRow - FOV.step);
            }
        else if (Mathf.Abs(playerPos.y - maxRow) < FOV.y) // expand up border of the map
            for (float x = minColumn; x <= maxColumn; x += FOV.step)
            {
                GenerateInstance(grassTiles, x, maxRow + FOV.step);
            }
    }

    void Awake()
    {
        tileRoot = new GameObject("Tile Root").transform;
        columnMap = new Dictionary<float, List<GameObject>>();
        rowMap = new Dictionary<float, List<GameObject>>();
        GenerateSpawnArea();
        GenerateFinish();
        GameObject.FindGameObjectWithTag("Player").transform.SetParent(tileRoot); // TODO: Fix bug
        GameObject.FindGameObjectWithTag("Finish").transform.SetParent(tileRoot);
    }

    void LateUpdate()
    {
        ExpandArea(GetMinColumn(), GetMaxColumn(), GetMinRow(), GetMaxRow());

        // (De)activate distant tiles
        /*
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        foreach (int i in columnMap.Keys)
            SetActiveItems(columnMap[i], !(Vector2.Distance(new Vector2(playerPos.x, 0), new Vector2(i, 0)) > FOV.x));
        foreach (int i in rowMap.Keys)
            SetActiveItems(rowMap[i], !(Vector2.Distance(new Vector2(0, playerPos.y), new Vector2(0, i)) > FOV.y));
        */
    }

    /// <param name="degree">Only use multiples of 90f as an argument.</param>
    private void WorldRotate(float degree)
    {
        //timesRotated++;
        //Debug.Log("Rotating world by " + degree);

        tileRoot.Rotate(0, 0, degree);
        foreach (SpriteRenderer r in tileRoot.GetComponentsInChildren<SpriteRenderer>())
        {
            if (r.gameObject.CompareTag("Finish"))
                continue;
            r.transform.Rotate(0, 0, -degree);
        }
    }

    public void RandomWorldRotate()
    {
        int i = Random.Range(0, 3);
        switch (i)
        {
            case 0:
                WorldRotate(90f);
                break;
            case 1:
                WorldRotate(-90f);
                break;
            case 2:
                WorldRotate(180f);
                break;
        }
    }

}
