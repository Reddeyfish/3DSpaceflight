using UnityEngine;
using System.Collections;

public class AvoidanceAIInput : PlayerInput
{
    [SerializeField]
    protected float maxDistance;

    int wallsLayerMask;

    protected override void Awake()
    {
        base.Awake();
        controller.throttleAxis = 1;
        wallsLayerMask = LayerMask.GetMask("Wall");
    }

    void handleRotation()
    {
        RaycastHit rotationHit;
        if (Physics.Raycast(this.transform.position, Vector3.up, out rotationHit, maxDistance, wallsLayerMask))
        {
            Vector3 targetUp = Vector3.ProjectOnPlane(rotationHit.normal, transform.forward);
            controller.rotationalAxisScaled = Mathf.Sign(Vector3.Dot(transform.forward, Vector3.Cross(targetUp, transform.up)));
        }
        else
        {
            controller.rotationalAxisScaled = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.BoxCast(this.transform.position, new Vector3(1.5f, 0.375f, 1.5f), this.transform.forward, out hit, transform.rotation, maxDistance, wallsLayerMask))
        {
            float verticalComponent = -Mathf.Sign(Vector3.Dot(transform.up, hit.normal));
            float horizontalComponent = -Mathf.Sign(Vector3.Dot(transform.right, hit.normal));
            Quaternion rotation = controller.appliedRotationScaled(verticalComponent, horizontalComponent, 0);

            RaycastHit newHit;
            if (Physics.BoxCast(this.transform.position, new Vector3(1.5f, 0.375f, 1.5f), rotation * Vector3.forward, out newHit, rotation, maxDistance, wallsLayerMask))
            {
                if (newHit.distance > hit.distance)
                {
                    //good change, move in direction
                    controller.verticalAxisScaled = verticalComponent;
                    controller.horizontalAxisScaled = horizontalComponent;

                    handleRotation();
                    return;
                }
                else
                {
                    //look through all actions
                    RaycastHit bestDistance = hit;
                    float bestVerticalComponent = 0;
                    float bestHorizontalComponent = 0;
                    Quaternion bestRotation = transform.rotation;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x != 0 && y != 0)
                            {
                                verticalComponent = y;
                                horizontalComponent = x;
                                rotation = controller.appliedRotationScaled(verticalComponent, horizontalComponent, 0);
                                if (Physics.BoxCast(this.transform.position, new Vector3(1f, 0.25f, 1f), rotation * Vector3.forward, out hit, rotation, maxDistance, wallsLayerMask))
                                {
                                    if (hit.distance < bestDistance.distance)
                                    {
                                        bestDistance = hit;
                                        bestVerticalComponent = verticalComponent;
                                        bestHorizontalComponent = horizontalComponent;
                                        bestRotation = rotation;
                                    }
                                }
                                else //nothing hit, keep flying
                                {
                                    controller.verticalAxisScaled = verticalComponent;
                                    controller.horizontalAxisScaled = horizontalComponent;
                                    controller.rotationalAxisScaled = 0;

                                    handleRotation();

                                    return;
                                }
                            }
                        }
                    }

                    //good change, move in direction
                    controller.verticalAxisScaled = bestVerticalComponent;
                    controller.horizontalAxisScaled = bestHorizontalComponent;

                    handleRotation();
                    return;
                }
            }
            else //nothing hit, keep flying
            {
                controller.verticalAxisScaled = verticalComponent;
                controller.horizontalAxisScaled = horizontalComponent;
                controller.rotationalAxisScaled = 0;

                handleRotation();

                return;
            }
        }
        else //nothing hit, keep flying forward
        {
            controller.verticalAxisScaled = 0;
            controller.horizontalAxisScaled = 0;
            controller.rotationalAxisScaled = 0;

            handleRotation();

            return;
        }
    }
}
