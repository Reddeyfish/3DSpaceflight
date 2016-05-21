using UnityEngine;
using System.Collections;

public class HumanInput : PlayerInput {

    [SerializeField]
    public InputConfiguration inputConfig;

	// Update is called once per frame
	void Update () {
        controller.verticalAxis = inputConfig.verticalSensitivity * (inputConfig.invertedVerticalControls ? -Input.GetAxis(inputConfig.verticalAxis) : Input.GetAxis(inputConfig.verticalAxis));

        controller.horizontalAxis = inputConfig.horizontalSensitivity * Input.GetAxis(inputConfig.horizontalAxis);

        controller.rotationalAxis = inputConfig.rotationalSensitivity * (inputConfig.invertedRotationalControls ? -Input.GetAxis(inputConfig.rotationalAxis) : Input.GetAxis(inputConfig.rotationalAxis));

        controller.throttleAxis = inputConfig.throttleSensitivity * Input.GetAxis(inputConfig.throttleAxis);
	}
}
