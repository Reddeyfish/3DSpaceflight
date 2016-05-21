using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputToAction))]
public abstract class PlayerInput : MonoBehaviour {

    protected InputToAction controller;

    protected virtual void Awake()
    {
        controller = GetComponent<InputToAction>();
    }
}

[System.Serializable]
public struct InputConfiguration
{
    public float verticalSensitivity;
    public string verticalAxis;

    public float horizontalSensitivity;
    public string horizontalAxis;

    public float rotationalSensitivity;
    public string rotationalAxis;

    public float throttleSensitivity;
    public string throttleAxis;

    public bool invertedVerticalControls;
    public bool invertedRotationalControls;
}