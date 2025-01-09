using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Player
{
    public class CompassController : MonoBehaviour
    {
        [SerializeField] float compassAngle = 120f;
        private List<PlanetBody> _planets = new List<PlanetBody>();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameObject starSystem = GameObject.FindWithTag("StarSystem");
            if (starSystem != null)
            {
                foreach (Transform child in starSystem.transform)
                {
                    if(child.GetComponent<InteractableBase>())
                        _planets.Add(new PlanetBody(child.gameObject, transform));
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (PlanetBody planet in _planets)
                planet.CompassUpdate(transform, compassAngle);
        }
    }
}