using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class InputToAction : MonoBehaviour {

    [SerializeField]
    public InputConfiguration inputConfig;
    [SerializeField]
    protected float maxVerticalDegreesPerSec;
    [SerializeField]
    protected float maxHorizontalDegreesPerSec;
    [SerializeField]
    protected float maxRotationalDegreesPerSec;

    [SerializeField]
    protected float minSpeed;
    [SerializeField]
    protected float maxSpeed;
    [SerializeField]
    protected float initialSpeed;
    [SerializeField]
    protected float maxAcceleration;

    float speed;
    Rigidbody rigidbody;

    void Awake()
    {
        //Cursor.visible = false;
        speed = initialSpeed;
        rigidbody = GetComponent<Rigidbody>();
        ApplySpeed();
    }

    // Update is called once per frame
    void Update()
    {
        //apply inversion
        float verticalRotation = inputConfig.invertedVerticalControls ? -Input.GetAxis(inputConfig.verticalAxis) : Input.GetAxis(inputConfig.verticalAxis);
        //scale and clamp to maximum values
        verticalRotation = Mathf.Clamp(inputConfig.verticalSensitivity * verticalRotation, -maxVerticalDegreesPerSec * Time.deltaTime, maxVerticalDegreesPerSec * Time.deltaTime);

        float horizontalRotation = Mathf.Clamp(inputConfig.horizontalSensitivity * Input.GetAxis(inputConfig.horizontalAxis), -maxHorizontalDegreesPerSec * Time.deltaTime, maxHorizontalDegreesPerSec * Time.deltaTime);

        float rotationalRotation = inputConfig.invertedRotationalControls ? -Input.GetAxis(inputConfig.rotationalAxis) : Input.GetAxis(inputConfig.rotationalAxis);
        rotationalRotation = Mathf.Clamp(inputConfig.rotationalSensitivity * rotationalRotation, -maxRotationalDegreesPerSec * Time.deltaTime, maxRotationalDegreesPerSec * Time.deltaTime);

        //cannot be done in one line, must be done in 2 steps to avoid resets
        Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, rotationalRotation);
        rigidbody.rotation *= rotation;

        float throttle = Mathf.Clamp(inputConfig.throttleSensitivity * Input.GetAxis(inputConfig.throttleAxis), -maxAcceleration * Time.deltaTime, maxAcceleration * Time.deltaTime);
        speed = Mathf.Clamp(speed + throttle, minSpeed, maxSpeed);
        ApplySpeed();
    }

    void ApplySpeed()
    {
        rigidbody.velocity = speed * transform.forward;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("DING!");
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