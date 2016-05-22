using UnityEngine;
using System.Collections;

public class PositionWrapping : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        position.x = (position.x + ProceduralGeneration.width) % (ProceduralGeneration.width);
        position.y = (position.y + ProceduralGeneration.height) % (ProceduralGeneration.height);
        position.z = (position.z + ProceduralGeneration.length) % (ProceduralGeneration.length);
        if (position != transform.position)
        {
            Debug.Log("WRAP!");
            transform.position = position;
        }
	}
}
