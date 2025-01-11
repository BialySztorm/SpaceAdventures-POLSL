using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Player
{
    public class CompassController : MonoBehaviour
    {
        [SerializeField] float compassAngle = 120f;
        [SerializeField] bool invertCompass = false;
        [SerializeField] float compassSize = 0.5f;
        private List<PlanetBody> _planets = new List<PlanetBody>();
        private Transform _playerTransform;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (invertCompass)
            {
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                transform.GetComponent<MeshRenderer>().enabled = false;
            }
            GameObject starSystem = GameObject.FindWithTag("StarSystem");
            if (starSystem != null)
            {
                foreach (Transform child in starSystem.transform)
                {
                    if(child.GetComponent<InteractableBase>())
                        _planets.Add(new PlanetBody(child.gameObject, transform, compassSize));
                }
            }
            
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
            foreach (PlanetBody planet in _planets)
                planet.CompassUpdate(_playerTransform, compassAngle);
        }
    }
}