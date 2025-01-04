using System;
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