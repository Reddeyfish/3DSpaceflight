using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class InputToAction : MonoBehaviour {

    [SerializeField]
    public float maxVerticalDegreesPerSec;
    [SerializeField]
    public float maxHorizontalDegreesPerSec;
    [SerializeField]
    public float maxRotationalDegreesPerSec;

    [SerializeField]
    protected float minSpeed;
    [SerializeField]
    protected float maxSpeed;
    [SerializeField]
    protected float initialSpeed;
    [SerializeField]
    protected float maxAcceleration;

    public float verticalAxis { get; set; }
    public float horizontalAxis { get; set; }
    public float rotationalAxis { get; set; }
    public float throttleAxis { get; set; }

    public float verticalAxisScaled { set { verticalAxis = maxVerticalDegreesPerSec * value; } }
    public float horizontalAxisScaled { set { horizontalAxis = maxHorizontalDegreesPerSec * value; } }
    public float rotationalAxisScaled { set { rotationalAxis = maxRotationalDegreesPerSec * value; } }
    public float throttleAxisScaled { set { throttleAxis = maxAcceleration * value; } }

    float speed;
    public Rigidbody rigid { get; set; }

    void Awake()
    {
        //Cursor.visible = false;
        speed = initialSpeed;
        rigid = GetComponent<Rigidbody>();
        ApplySpeed();
    }

    public Quaternion appliedRotation(float verticalAxis, float horizontalAxis, float rotationalAxis)
    {
        //clamp to maximum values
        float verticalRotation = Mathf.Clamp(verticalAxis, -maxVerticalDegreesPerSec * Time.deltaTime, maxVerticalDegreesPerSec * Time.deltaTime);

        float horizontalRotation = Mathf.Clamp(horizontalAxis, -maxHorizontalDegreesPerSec * Time.deltaTime, maxHorizontalDegreesPerSec * Time.deltaTime);

        float rotationalRotation = Mathf.Clamp(rotationalAxis, -maxRotationalDegreesPerSec * Time.deltaTime, maxRotationalDegreesPerSec * Time.deltaTime);

        //cannot be done in one line, must be done in 2 steps to avoid resets
        Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, rotationalRotation);
        return rigid.rotation * rotation;
    }

    public Quaternion appliedRotationScaled(float verticalAxis, float horizontalAxis, float rotationalAxis)
    {
        return appliedRotation(maxVerticalDegreesPerSec * verticalAxis, maxHorizontalDegreesPerSec * horizontalAxis, maxRotationalDegreesPerSec * rotationalAxis);
    }

    // Update is called once per frame
    void Update()
    {
        rigid.rotation = appliedRotation(verticalAxis, horizontalAxis, rotationalAxis);

        float throttle = Mathf.Clamp(throttleAxis, -maxAcceleration * Time.deltaTime, maxAcceleration * Time.deltaTime);
        speed = Mathf.Clamp(speed + throttle, minSpeed, maxSpeed);
        ApplySpeed();
    }

    void ApplySpeed()
    {
        rigid.velocity = speed * transform.forward;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("DING: " + collision.impulse.magnitude);
    }
}