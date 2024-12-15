using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace Player
{
    public class XRCameraController : MonoBehaviour
    {
        [Tooltip("Distance from the initial position to cancel xr movement")]
        float distanceToCancel = 2;

        private XROrigin xrOrigin;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        
        void Start()
        {
            GameObject xRRig = transform.parent.transform.parent.gameObject;
            if(xRRig == null)
                return;
            xrOrigin = xRRig.GetComponent<XROrigin>();
            initialPosition = transform.position;
        }
        
        void Update()
        {
            if(xrOrigin == null)
                return;
            // Distance from initial position
            float distance = Vector3.Distance(initialPosition, transform.position);
            // If the distance is greater than 2 cancel xr movement
            if (distance > distanceToCancel)
            {
                Vector3 direction = (transform.position - initialPosition).normalized;
                Vector3 new_location = initialPosition + direction * distanceToCancel;
                xrOrigin.MoveCameraToWorldLocation(new_location);
            }   
            
        }
        
        public void ResetView()
        {
            xrOrigin.MoveCameraToWorldLocation(initialPosition);
        }
    }
}

