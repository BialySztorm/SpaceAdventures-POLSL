using System.Collections.Generic;
using Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlanetBody
    {
        private GameObject mapPlanet { get; set; }
        private List<GameObject> mapSatellites { get; set; }
        private Transform originalPlanetTransform { get; set; }
        private Dictionary<string, Transform> originalSatelliteTransforms { get; set; }

        private float _compassSize = 0.1f;
        
        private void AddMesh(GameObject item, GameObject mapItem)
        {
            MeshFilter meshFilter = item.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();

            if (meshFilter && meshRenderer)
            {
                MeshFilter mapMeshFilter = mapItem.AddComponent<MeshFilter>();
                MeshRenderer mapMeshRenderer = mapItem.AddComponent<MeshRenderer>();
                
                mapMeshFilter.mesh = meshFilter.mesh;
                mapMeshRenderer.materials = meshRenderer.materials;
            }
            else
            {
                Debug.LogError($"Planet {item.name} does not have a MeshFilter or MeshRenderer component.");
            }
        }

        public PlanetBody(GameObject planet, Transform parent)
        {
            _compassSize = parent.localScale.x*0.5f;
            
            mapPlanet = new GameObject(planet.name);
            mapPlanet.transform.SetParent(parent);
            AddMesh(planet, mapPlanet);
            originalPlanetTransform = planet.transform;
            mapPlanet.transform.localPosition = Vector3.zero;
            mapPlanet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            mapSatellites = new List<GameObject>();
            originalSatelliteTransforms= new Dictionary<string, Transform>();

            foreach (Transform child in planet.transform)
            {
                Debug.Log(child.gameObject.name);
                if (child.GetComponent<InteractableBase>())
                {
                    GameObject mapSatellite = new GameObject(child.gameObject.name);
                    mapSatellite.transform.SetParent(parent);
                    AddMesh(child.gameObject, mapSatellite);
                    mapSatellite.SetActive(false);
                    mapSatellite.transform.localPosition = Vector3.zero;
                    mapSatellite.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    mapSatellites.Add(mapSatellite);
                    originalSatelliteTransforms[child.gameObject.name] = child.transform;
                }
            }
        }

        public void CompassUpdate(Transform playerTransform,float compassAngle = 180f, float activationDistance = 50f)
        {
            // TODO Planet Position on compass
            Vector3 directionToPlanet = originalPlanetTransform.position - playerTransform.position;
            
            Vector3 localDirection = playerTransform.InverseTransformDirection(directionToPlanet.normalized);
            float yaw = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg; // Kąt poziomy
            float pitch = Mathf.Asin(localDirection.y) * Mathf.Rad2Deg;                  // Kąt pionowy
            
            if(mapPlanet.name == "Sun")
                Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
            
            float maxPitch = compassAngle / 2f;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
            yaw = Mathf.Clamp(yaw, -maxPitch, maxPitch);

            float pitchRadians = pitch * Mathf.Deg2Rad;
            float yawRadians = yaw * Mathf.Deg2Rad;

            float x = _compassSize * Mathf.Cos(pitchRadians) * Mathf.Sin(yawRadians);
            float y = _compassSize * Mathf.Cos(pitchRadians) * Mathf.Cos(yawRadians);
            float z = _compassSize * Mathf.Sin(pitchRadians);

            mapPlanet.transform.localPosition = new Vector3(x, y, z);

            if(Vector3.Distance(playerTransform.position, originalPlanetTransform.position) <= activationDistance)
            {
                foreach (var satellite in mapSatellites)
                {
                    satellite.SetActive(true);
                    // TODO Satellite Position on compass
                }
            }
            else
            {
                foreach (var satellite in mapSatellites)
                    satellite.SetActive(false);
            }
        }
    }
}