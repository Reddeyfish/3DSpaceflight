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

    public static void AddToSimulate(GameObject toSimulate)
    {
        simulate.Add(toSimulate);
    }

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
                    duplicated.transform.position = toDuplicate.transform.position + ProceduralGeneration.scale * new Vector3(ProceduralGeneration.width * x, ProceduralGeneration.height * y, ProceduralGeneration.length * z);
                }
            }
        }
    }

    static void simulateGameObject(GameObject toSimulate)
    {
        toSimulate.transform.root.AddComponent<PositionWrapping>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) //the position of the original
                        continue;
                    GameObject duplicated = Instantiate(toSimulate);
                    Vector3 offset = ProceduralGeneration.scale * new Vector3(ProceduralGeneration.width * x, ProceduralGeneration.height * y, ProceduralGeneration.length * z);
                    duplicated.transform.position = toSimulate.transform.position + offset;
                    SimulateWithOffset simulator = duplicated.AddComponent<SimulateWithOffset>();
                    simulator.offset = offset;
                    simulator.target = toSimulate.transform;
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
