using UnityEngine;
using System.Collections;

public class PositionWrapping : MonoBehaviour {
	// Update is called once per frame

    public Transform target;

    void Start()
    {
        if (target == null)
        {
            target = this.transform; //wrap our own position
        }
    }

	void Update () 
    {
        transform.position = wrapPosition(target.position);
	}

    public static Vector3 wrapPosition(Vector3 unwrappedPosition)
    {
        unwrappedPosition.x = (unwrappedPosition.x + ProceduralGeneration.width) % (ProceduralGeneration.width);
        unwrappedPosition.y = (unwrappedPosition.y + ProceduralGeneration.height) % (ProceduralGeneration.height);
        unwrappedPosition.z = (unwrappedPosition.z + ProceduralGeneration.length) % (ProceduralGeneration.length);
        return unwrappedPosition;
    }
}
