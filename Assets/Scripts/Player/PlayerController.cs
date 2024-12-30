using System;
using Entities;
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
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;
        [SerializeField] private MeshCollider windowCollider;
        
        private InputActionMap XRILeftHandMap;
        private InputActionMap XRILeftHandInteractionMap;
        private InputActionMap XRIRightHandMap;
        private InputActionMap XRIRightHandInteractionMap;
        private InputActionMap PCMap;
        
        [SerializeField] private float interactionDistance = 1000.0f;
        private Renderer _leftPokePointRenderer;
        private Renderer _rightPokePointRenderer;
        private Renderer _leftLineVisualRenderer;
        private Renderer _rightLineVisualRenderer;
        private Color _defaultColor;
        

        private void Awake()
        {
            if (isXR)
            {
                XRILeftHandMap = xrInputActions.FindActionMap("XRI Left");
                XRILeftHandInteractionMap = xrInputActions.FindActionMap("XRI Left Interaction");
                XRIRightHandMap = xrInputActions.FindActionMap("XRI Right");
                XRIRightHandInteractionMap = xrInputActions.FindActionMap("XRI Right Interaction");
                
                Transform leftPokePoint = leftHand.Find("Poke Interactor/Poke Point/Pinch_Pointer_LOD0");
                if (leftPokePoint != null)
                    _leftPokePointRenderer = leftPokePoint.GetComponent<Renderer>();
                else
                    Debug.LogError("Left PokePoint not found");
                
                Transform rightPokePoint = rightHand.Find("Poke Interactor/Poke Point/Pinch_Pointer_LOD0");
                if (rightPokePoint != null)
                    _rightPokePointRenderer = rightPokePoint.GetComponent<Renderer>();
                else
                    Debug.LogError("Right PokePoint not found");
                
                Transform leftLineVisual = leftHand.Find("Near-Far Interactor/LineVisual");
                if (leftLineVisual != null)
                    _leftLineVisualRenderer = leftLineVisual.GetComponent<Renderer>();
                else
                    Debug.LogError("Left LineVisual not found");
                
                Transform rightLineVisual = rightHand.Find("Near-Far Interactor/LineVisual");
                if (rightLineVisual != null)
                    _rightLineVisualRenderer = rightLineVisual.GetComponent<Renderer>();
                else
                    Debug.LogError("Right LineVisual not found");
                
                if(leftLineVisual && rightLineVisual)
                    _defaultColor = _leftLineVisualRenderer.material.GetColor("_TintColor");
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
                // * Movement and Rotation
                Vector2 leftStick = XRILeftHandMap["Thumbstick"].ReadValue<Vector2>();
                Vector2 rightStick = XRIRightHandMap["Thumbstick"].ReadValue<Vector2>();
                Rotate(leftStick, rightStick);
                float brakeVal = XRILeftHandInteractionMap["Select Value"].ReadValue<float>();
                float gasVal = XRIRightHandInteractionMap["Select Value"].ReadValue<float>();
                Move(gasVal - brakeVal);
                
                // * Interaction
                if (CheckForInteractable(leftHand.transform, _leftPokePointRenderer, _leftLineVisualRenderer) && XRILeftHandInteractionMap["Activate"].triggered)
                {
                    // TODO Interact with object
                    Debug.Log("Left Hand Interacting");
                }
                if (CheckForInteractable(rightHand.transform, _rightPokePointRenderer, _rightLineVisualRenderer) && XRIRightHandInteractionMap["Activate"].triggered)
                {
                    // TODO Interact with object
                    Debug.Log("Right Hand Interacting");
                }
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
        
        private bool CheckForInteractable(Transform hand, Renderer pokePointRenderer, Renderer lineVisualRenderer)
        {
            RaycastHit windowHit;
            if (Physics.Raycast(hand.position, hand.forward, out windowHit, interactionDistance))
            {
                if (windowHit.collider == windowCollider)
                {
                    Vector3 windowHitPoint = windowHit.point;
                    RaycastHit interactableHit;
                    if (Physics.Raycast(windowHitPoint, hand.forward, out interactableHit, interactionDistance))
                    {
                        if (interactableHit.collider.GetComponent<InteractableBase>())
                        {
                            pokePointRenderer.material.SetColor("_RimColor", Color.green);
                            lineVisualRenderer.material.SetColor("_TintColor", Color.green);
                            return true;
                        }
                    }
                }
            }
            
            pokePointRenderer.material.SetColor("_RimColor", _defaultColor);
            lineVisualRenderer.material.SetColor("_TintColor", _defaultColor);
            return false;
        }
        
        private void Interact(RaycastHit hit)
        {
            Debug.Log("Interacting with " + hit.collider.name);
        }

    }
    
}