using UnityEngine;

namespace RosSharp.Control
{
    public class AGVController : MonoBehaviour
    {
        public GameObject wheel1;
        public GameObject wheel2;
        public GameObject wheel3;
        public GameObject wheel4;
        
        private HingeJoint leftFrontWheelJoint;
        private HingeJoint rightFrontWheelJoint;
        private HingeJoint leftBackWheelJoint;
        private HingeJoint rightBackWheelJoint;

        public float maxLinearSpeed = 2; //  m/s
        public float maxRotationalSpeed = 1;//
        public float wheelRadius = 0.033f; //meters
        public float trackWidth = 0.288f; // meters Distance between tyres
        public float forceLimit = 10;
        public float damping = 10;

        public float ROSTimeout = 0.5f;
        private float lastCmdReceived = 0f;

        private float rosLinear = 0f;
        private float rosAngular = 0f;

        void Start()
        {
            leftFrontWheelJoint = wheel1.GetComponent<HingeJoint>();
            rightFrontWheelJoint = wheel2.GetComponent<HingeJoint>();
            leftBackWheelJoint = wheel3.GetComponent<HingeJoint>();
            rightBackWheelJoint = wheel4.GetComponent<HingeJoint>();
            
            SetParameters(leftFrontWheelJoint);
            SetParameters(rightFrontWheelJoint);
            SetParameters(leftBackWheelJoint);
            SetParameters(rightBackWheelJoint);
        }

        void FixedUpdate()
        {
            KeyBoardUpdate();
        }

        private void SetParameters(HingeJoint joint)
        {
            var motor = joint.motor;
            motor.force = forceLimit;
            joint.motor = motor;
            joint.useMotor = true;

            var spring = joint.spring;
            spring.damper = damping;
            joint.spring = spring;
            joint.useSpring = true;
        }

        private void SetSpeed(HingeJoint joint, float wheelSpeed = float.NaN)
        {
            var motor = joint.motor;
            if (float.IsNaN(wheelSpeed))
            {
                motor.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg;   
            }
            else
            {
                motor.targetVelocity = wheelSpeed;
            }
            joint.motor = motor;
        }

        private void KeyBoardUpdate()
        {
            float moveDirection = Input.GetAxis("Vertical");
            float inputSpeed;
            float inputRotationSpeed;
            if (moveDirection > 0)
            {
                inputSpeed = maxLinearSpeed;
            }
            else if (moveDirection < 0)
            {
                inputSpeed = maxLinearSpeed * -1;
            }
            else
            {
                inputSpeed = 0;
            }

            float turnDirction = Input.GetAxis("Horizontal");
            if (turnDirction > 0)
            {
                inputRotationSpeed = maxRotationalSpeed;
            }
            else if (turnDirction < 0)
            {
                inputRotationSpeed = maxRotationalSpeed * -1;
            }
            else
            {
                inputRotationSpeed = 0;
            }
            RobotInput(inputSpeed, inputRotationSpeed);
        }

        private void RobotInput(float speed, float rotSpeed) // m/s and rad/s
        {
            if (speed > maxLinearSpeed)
            {
                speed = maxLinearSpeed;
            }
            if (rotSpeed > maxRotationalSpeed)
            {
                rotSpeed = maxRotationalSpeed;
            }
            float wheel1Rotation = (speed / wheelRadius);
            float wheel2Rotation = wheel1Rotation;
            float wheelSpeedDiff = ((rotSpeed * trackWidth) / wheelRadius);
            if (rotSpeed != 0)
            {
                wheel1Rotation = (wheel1Rotation + (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
                wheel2Rotation = (wheel2Rotation - (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
            }
            else
            {
                wheel1Rotation *= Mathf.Rad2Deg;
                wheel2Rotation *= Mathf.Rad2Deg;
            }
            SetSpeed(leftFrontWheelJoint, wheel1Rotation);
            SetSpeed(rightFrontWheelJoint, wheel2Rotation);
            SetSpeed(leftBackWheelJoint, wheel1Rotation);
            SetSpeed(rightBackWheelJoint, wheel2Rotation);
        }
    }
}
