using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Entities
{
    public class SolarSystem : MonoBehaviour
    {
        private Planet[] planets;
        
        
        private void Start()
        {
            planets = GetComponentsInChildren<Planet>();
            double currentDateTime = DateTime.UtcNow.ToOADate();
            foreach (Planet planet in planets)
            {
                Vector3 position = planet.GetCurrentPosition(currentDateTime);
                Quaternion rotation = planet.GetCurrentRotation(currentDateTime);
                planet.transform.position = position;
                planet.transform.rotation = rotation;
            }
            
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && planets.Length > 2)
            {
                Vector3 tmpPos = planets[2].transform.position;
                tmpPos.x += 100.0f;
                player.transform.position = tmpPos;
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
    }
}