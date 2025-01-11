using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace Player
{
    public class XRCameraController : MonoBehaviour
    {
        [Tooltip("")] 
        [SerializeField] private GameObject BoundarieSphere;

        private XROrigin xrOrigin;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private float distanceToCancel = 1;

        void Start()
        {
            GameObject xRRig = transform.parent.transform.parent.gameObject;
            if (xRRig == null)
                return;
            xrOrigin = xRRig.GetComponent<XROrigin>();
            initialPosition = transform.position;
            if (BoundarieSphere)
                distanceToCancel = BoundarieSphere.GetComponent<SphereCollider>().radius * BoundarieSphere.transform.parent.transform.localScale.x;
            else
                Debug.LogWarning("BoundarieSphere not set");
        }
        
        void Update()
        {
            if(xrOrigin == null || BoundarieSphere == null)
                return;
            // Distance from initial position
            initialPosition = BoundarieSphere.transform.position;
            float distance = Vector3.Distance(initialPosition, transform.position);
            // If the distance is greater than 2 cancel xr movement
            if (distance > distanceToCancel)
            {
                Vector3 direction = (transform.position - initialPosition).normalized;
                Vector3 newLocation = initialPosition + direction * distanceToCancel;
                xrOrigin.MoveCameraToWorldLocation(newLocation);
            }   
            
        }
        
        public void ResetView()
        {
            xrOrigin.MoveCameraToWorldLocation(initialPosition);
        }
    }
}

