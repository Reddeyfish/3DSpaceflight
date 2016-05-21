using UnityEngine;
using System.Collections;

public class AddToSimulate : MonoBehaviour {

    void Awake()
    {
        ProceduralDuplication.AddToSimulate(this.gameObject);
        Destroy(this);
    }
}
