using UnityEngine;
using System.Collections;

public class InputToAction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        //transform.localRotation = Quaternion.LookRotation(Quaternion.AngleAxis(Input.GetAxis("Vertical"), transform.right) * transform.forward);
        //transform.position += transform.forward *= Time.deltaTime * 5;
        Quaternion rotation = Quaternion.Euler(-Input.GetAxis("VerticalMouse"), Input.GetAxis("HorizontalMouse"), Input.GetAxis("Horizontal"));
        transform.rotation *= rotation;

        //transform.position += Input.GetAxis("Vertical") * transform.forward;
        transform.position += 0.1f * transform.forward;
    }
}
