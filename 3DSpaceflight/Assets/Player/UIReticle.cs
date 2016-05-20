using UnityEngine;
using System.Collections;

public class UIReticle : MonoBehaviour {

    [SerializeField]
    protected Camera cam;
    [SerializeField]
    protected Transform shooter;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = (RectTransform)transform;
    }

	// Update is called once per frame
	void Update () {
        rectTransform.position = cam.WorldToScreenPoint(shooter.position + 100 * shooter.forward);
	}
}
