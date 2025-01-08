using System;
using Entities;
using UI;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset pcInputActions;
        [SerializeField] private InputActionAsset xrInputActions;
        [SerializeField] private bool isXR = true;
        [SerializeField] private float acceleration = 0.1f;
        [SerializeField] private float maxSpeed = 10.0f;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;
        [SerializeField] private MeshCollider windowCollider;
        
        private float _currentSpeed = 0.0f;
        
        private InputActionMap XRILeftHandMap;
        private InputActionMap XRILeftHandInteractionMap;
        private InputActionMap XRIRightHandMap;
        private InputActionMap XRIRightHandInteractionMap;
        private InputActionMap PCMap;
        
        [SerializeField] private float interactionDistance = 1000.0f;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private GameObject menuPanel;
        private Renderer _leftPokePointRenderer;
        private Renderer _rightPokePointRenderer;
        private Renderer _leftLineVisualRenderer;
        private Renderer _rightLineVisualRenderer;
        private Color _defaultColor;

        private GameObject _currentInfoPanel;
        private LocalizedString _localizedEntities = new LocalizedString();
        private LocalizedString _localizedMenu = new LocalizedString();
        

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
            
            _localizedEntities.TableReference = "Entities";
            _localizedMenu.TableReference = "UI";
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
                string interactableName = string.Empty;
                interactableName = CheckForInteractable(leftHand.transform, _leftPokePointRenderer, _leftLineVisualRenderer);
                if (interactableName.IsNotNullOrEmpty() && XRILeftHandInteractionMap["Activate"].triggered)
                {
                    // TODO Interact with object
                    Interact(interactableName, leftHand);
                }
                interactableName = CheckForInteractable(rightHand.transform, _rightPokePointRenderer, _rightLineVisualRenderer);
                if (interactableName.IsNotNullOrEmpty()  && XRIRightHandInteractionMap["Activate"].triggered)
                {
                    Interact(interactableName, rightHand);
                }
            }
            else
            {
            }
        }
        
        private void Move(float direction)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed + direction * acceleration, 0f, maxSpeed);
            transform.position += transform.forward * _currentSpeed;
        }
        
        private void Rotate(Vector2 leftStick, Vector2 rightStick)
        {
            float pitch = -rightStick.y; // left and right
            float yaw = rightStick.x; // up and down
            float roll = -leftStick.x; // rotation
            Vector3 rotation = new Vector3(pitch, yaw, roll);
            transform.Rotate(rotation);
        }
        
        private string CheckForInteractable(Transform hand, Renderer pokePointRenderer, Renderer lineVisualRenderer)
        {

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                pokePointRenderer.material.SetColor("_RimColor", _defaultColor);
                lineVisualRenderer.material.SetColor("_TintColor", _defaultColor);
                return string.Empty;
            }

            RaycastHit windowHit;
            if (Physics.Raycast(hand.position, hand.forward, out windowHit, interactionDistance))
            {
                if (windowHit.collider == windowCollider)
                {
                    Vector3 windowHitPoint = windowHit.point;
                    RaycastHit interactableHit;
                    if (Physics.Raycast(windowHitPoint, hand.forward, out interactableHit, interactionDistance))
                    {
                        InteractableBase interactable = interactableHit.collider.GetComponent<InteractableBase>();
                        if (interactable)
                        {
                            pokePointRenderer.material.SetColor("_RimColor", Color.green);
                            lineVisualRenderer.material.SetColor("_TintColor", Color.green);
                            return interactable.GetName();
                        }
                    }
                }
            }
            
            pokePointRenderer.material.SetColor("_RimColor", _defaultColor);
            lineVisualRenderer.material.SetColor("_TintColor", _defaultColor);
            return string.Empty;
        }
        
        private void Interact(string hit, Transform hand)
        {
            if(_currentInfoPanel)
                Destroy(_currentInfoPanel);
            GameObject infoPanelRef = Instantiate(infoPanel, transform);
            infoPanelRef.transform.position = hand.position + hand.forward * 0.2f;
            infoPanelRef.transform.LookAt(hand);
            infoPanelRef.transform.Rotate(0, 180, 0);
            StepManager stepManagerRef = infoPanelRef.transform.Find("CoachingCardRoot").GetComponent<StepManager>();
            _localizedEntities.TableEntryReference = hit + ".name";
            stepManagerRef.AddStep(_localizedEntities.GetLocalizedString());
            stepManagerRef.AddStep("");
            
            _currentInfoPanel = infoPanelRef;
        }

    }
    
}