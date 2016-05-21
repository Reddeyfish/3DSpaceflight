using UnityEngine;
using System.Collections;

public class OffsetCamera : MonoBehaviour {

    [SerializeField]
    public Vector3 offset;

    [SerializeField]
    public Camera target;

    Transform targetTransform;
    void Start()
    {
        targetTransform = target.transform;
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = targetTransform.position + offset;
        this.transform.rotation = targetTransform.rotation;
	}
}
