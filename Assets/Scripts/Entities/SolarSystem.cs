using System;
using Player;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Entities
{
    public class SolarSystem : MonoBehaviour
    {
        private Planet[] planets;
        
        [SerializeField]
        private float positionScale = 10000.0f;
        
        
        private void Start()
        {
            planets = GetComponentsInChildren<Planet>();
            double currentDateTime = DateTime.UtcNow.ToOADate();
            foreach (Planet planet in planets)
            {
                Vector3 position = planet.GetCurrentPosition(currentDateTime);
                Quaternion rotation = planet.GetCurrentRotation(currentDateTime);
                planet.transform.position = position/positionScale;
                planet.transform.rotation = rotation;
            }
            
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && planets.Length > 2)
            {
                Vector3 tmpPos = planets[2].transform.position;
                tmpPos.x += 100.0f*transform.localScale.x;
                player.transform.position = tmpPos;
                XROrigin playerRef = player.GetComponentInChildren<XROrigin>();
                if (playerRef != null)
                {
                    Vector3 tmpPos2 = player.transform.position;
                    tmpPos2.y += 0.675f;
                    tmpPos2.z += 0.25f;
                    playerRef.MoveCameraToWorldLocation(tmpPos2);
                }
            }
            
        }
        
        private void Update()
        {
            double currentDateTime = DateTime.UtcNow.ToOADate();
            foreach (Planet planet in planets)
            {
                Quaternion rotation = planet.GetCurrentRotation(currentDateTime);
                planet.transform.rotation = rotation;
            }
        }
        
        public float GetPositionScale()
        {
            return positionScale;
        }
    }
}