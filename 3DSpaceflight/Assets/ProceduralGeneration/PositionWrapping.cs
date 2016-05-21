using UnityEngine;
using System.Collections;

public class PositionWrapping : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        position.x = (position.x + ProceduralGeneration.scale * ProceduralGeneration.width) % (ProceduralGeneration.scale * ProceduralGeneration.width);
        position.y = (position.y + ProceduralGeneration.scale * ProceduralGeneration.height) % (ProceduralGeneration.scale * ProceduralGeneration.height);
        position.z = (position.z + ProceduralGeneration.scale * ProceduralGeneration.length) % (ProceduralGeneration.scale * ProceduralGeneration.length);
        if (position != transform.position)
        {
            Debug.Log("WRAP!");
            transform.position = position;
        }
	}
}
