using UnityEngine;
using System.Collections;

public class DuplicateRotation : MonoBehaviour {

    public Transform target;

    void Start()
    {
        UnityEngine.Assertions.Assert.IsTrue(target != null);
    }

	void Update () 
    {
        transform.rotation = target.rotation;
	}
}
