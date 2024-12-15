using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset pcInputActions;
        [SerializeField] private InputActionAsset xrInputActions;
        [SerializeField] private bool isXR = true;
        [SerializeField] private float speed = 1.0f;
        
        private InputActionMap XRILeftHandMap;
        private InputActionMap XRILeftHandInteractionMap;
        private InputActionMap XRIRightHandMap;
        private InputActionMap XRIRightHandInteractionMap;
        private InputActionMap PCMap;

        private void Awake()
        {
            if (isXR)
            {
                XRILeftHandMap = xrInputActions.FindActionMap("XRI Left");
                XRILeftHandInteractionMap = xrInputActions.FindActionMap("XRI Left Interaction");
                XRIRightHandMap = xrInputActions.FindActionMap("XRI Right");
                XRIRightHandInteractionMap = xrInputActions.FindActionMap("XRI Right Interaction");
                
            }
            else
            {
                PCMap = pcInputActions.FindActionMap("Player");
            }
        }

        private void OnEnable()
        {
            if (isXR)
            {
                XRILeftHandMap.Enable();
                XRIRightHandMap.Enable();
            }
            else
            {
                PCMap.Enable();
            }
        }

        private void OnDisable()
        {
            if (isXR)
            {
                XRILeftHandMap.Disable();
                XRIRightHandMap.Disable();
            }
            else
            {
                PCMap.Disable();
            }
        }

        private void Update()
        {
            if (isXR)
            {
                Vector2 leftStick = XRILeftHandMap["Thumbstick"].ReadValue<Vector2>();
                Vector2 rightStick = XRIRightHandMap["Thumbstick"].ReadValue<Vector2>();
                Rotate(leftStick, rightStick);
                float brakeVal = XRILeftHandInteractionMap["Select Value"].ReadValue<float>();
                float gasVal = XRIRightHandInteractionMap["Select Value"].ReadValue<float>();
                Move(gasVal - brakeVal);
            }
            else
            {
            }
        }
        
        private void Move(float direction)
        {
            float moveSpeed = speed * direction * Time.deltaTime;
            transform.position += transform.forward * moveSpeed;
        }
        
        private void Rotate(Vector2 leftStick, Vector2 rightStick)
        {
            // Rotate the player based on the left stick
            float pitch = -(leftStick.y + rightStick.y);
            float yaw = rightStick.x;
            float roll = -leftStick.x;
            Vector3 rotation = new Vector3(pitch, yaw, roll);
            transform.Rotate(rotation);
        }

    }
}