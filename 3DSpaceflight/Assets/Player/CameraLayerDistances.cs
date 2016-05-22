using UnityEngine;
using System.Collections;

public class CameraLayerDistances : MonoBehaviour {

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[22] = 40; //dust-culling layer
        camera.layerCullDistances = distances;
        this.enabled = false;
    }
}
