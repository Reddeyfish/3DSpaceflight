using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralDuplication : MonoBehaviour {

    static List<GameObject> duplicate = new List<GameObject>(); //objects to duplicate and make static

    public static void AddToDuplicate(GameObject toDuplicate)
    {
        duplicate.Add(toDuplicate);
    }

    public static void AddToDuplicate(GameObject toDuplicate, bool enableAfterStart)
    {
        if (started && enableAfterStart)
            duplicateGameObject(toDuplicate);
        else
            duplicate.Add(toDuplicate);
    }

    static List<GameObject> simulate = new List<GameObject>(); //objects to duplicate and update

    /// <summary>
    /// Note: Do NOT resimulate gameplay logic; only visuals/audio/etc.
    /// </summary>
    /// <param name="toSimulate"></param>
    public static void AddToSimulate(GameObject toSimulate)
    {
        simulate.Add(toSimulate);
    }

    /// <summary>
    /// Note: Do NOT resimulate gameplay logic; only visuals/audio/etc.
    /// </summary>
    /// <param name="toSimulate"></param>
    public static void AddToSimulate(GameObject toSimulate, bool enableAfterStart)
    {
        if (started && enableAfterStart)
            simulateGameObject(toSimulate);
        else
            simulate.Add(toSimulate);
    }

    static bool started = false;

    static void duplicateGameObject(GameObject toDuplicate)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) //the position of the original
                        continue;
                    GameObject duplicated = Instantiate(toDuplicate);
                    duplicated.transform.position = toDuplicate.transform.position + new Vector3(ProceduralGeneration.width * x, ProceduralGeneration.height * y, ProceduralGeneration.length * z);
                }
            }
        }
    }

    static void simulateGameObject(GameObject toSimulate)
    {
        GameObject root = new GameObject(toSimulate.name + " Root");
        root.transform.SetParent(toSimulate.transform.parent);
        root.transform.localPosition = toSimulate.transform.localPosition;
        root.transform.rotation = toSimulate.transform.rotation;

        GameObject arrayBase = new GameObject(toSimulate.name + " ArrayBase");

        arrayBase.AddComponent<PositionWrapping>().target = root.transform;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3 offset = new Vector3(ProceduralGeneration.width * x, ProceduralGeneration.height * y, ProceduralGeneration.length * z);
                    GameObject duplicated;
                    if (x == 0 && y == 0 && z == 0) //the position of the original
                    {
                        duplicated = toSimulate;
                    }
                    else
                    {
                        duplicated = Instantiate(toSimulate);
                    }
                    duplicated.transform.SetParent(arrayBase.transform);
                    duplicated.transform.localPosition = offset;
                    duplicated.AddComponent<DuplicateRotation>().target = root.transform;
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
        //duplicate duplicates
        int numDuplicates = duplicate.Count; //ensure it isn't changed
        for (int i = 0; i < numDuplicates; i++)
        {
            duplicateGameObject(duplicate[i]);
        }
        duplicate.Clear();

        //duplicate simulates and attach simulation
        int numSimulates = simulate.Count; //ensure it isn't changed
        for (int i = 0; i < numSimulates; i++)
        {
            simulateGameObject(simulate[i]);
        }
        simulate.Clear();
        this.enabled = false;

        //started = true; only duplicate/simulate at start, for now
	}
}
