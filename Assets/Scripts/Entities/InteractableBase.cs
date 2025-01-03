using UnityEngine;

namespace Entities
{
    public class InteractableBase : MonoBehaviour
    {
        [SerializeField, Tooltip("Name of the interactable object")]
        private string objectName = "Interactable Object";
        [SerializeField, Tooltip("Description of the interactable object")]
        private string objectDescription = "This is an interactable object";
        
    }
}