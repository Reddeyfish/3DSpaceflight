using UnityEngine;
using System.Collections;

namespace DerekEdrich
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SoundFX : MonoBehaviour
    {
        [SerializeField]
        protected float radius;
        Camera mainCamera;
        Material mat;
        // Use this for initialization
        void Awake()
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();
            mat = rend.material = Instantiate(rend.material);

            mat.SetFloat("_Radius", radius);

            mainCamera = Camera.main;
        }

        void Update()
        {
            Matrix4x4 matrix = mainCamera.cameraToWorldMatrix;
            mat.SetMatrix("_InverseView", matrix);
        }
    }
}