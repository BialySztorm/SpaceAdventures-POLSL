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

        public PlanetBody(GameObject planet, Transform parent, float compassSize)
        {
            _compassSize = compassSize;
            float planetScale = 0.1f / parent.transform.localScale.x;
            
            mapPlanet = new GameObject(planet.name);
            mapPlanet.transform.SetParent(parent);
            AddMesh(planet, mapPlanet);
            originalPlanetTransform = planet.transform;
            mapPlanet.transform.localPosition = Vector3.zero;
            mapPlanet.transform.localScale = new Vector3(planetScale, planetScale, planetScale);
            
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
                    mapSatellite.transform.localScale = new Vector3(planetScale, planetScale, planetScale);
                    mapSatellites.Add(mapSatellite);
                    originalSatelliteTransforms[child.gameObject.name] = child.transform;
                }
            }
        }

        public void CompassUpdate(Transform playerTransform,float compassAngle = 180f, float activationDistance = 50f)
        {
            Vector3 directionToPlanet = originalPlanetTransform.position - playerTransform.position;
            
            Vector3 localDirection = playerTransform.InverseTransformDirection(directionToPlanet.normalized);
            float yaw = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
            float pitch = -Mathf.Asin(localDirection.y) * Mathf.Rad2Deg;
            
            // if(mapPlanet.name == "Sun")
            //     Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
            
            float maxPitch = compassAngle / 2f;
            yaw = Mathf.Clamp(yaw, -maxPitch, maxPitch);
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

            float yawRadians = yaw * Mathf.Deg2Rad;
            float pitchRadians = pitch * Mathf.Deg2Rad;

            float x = _compassSize * Mathf.Cos(yawRadians) * Mathf.Sin(pitchRadians);
            float y = _compassSize * Mathf.Cos(yawRadians) * Mathf.Cos(pitchRadians);
            float z = _compassSize * Mathf.Sin(yawRadians);

            mapPlanet.transform.localPosition = new Vector3(x, y, z);

            if(Vector3.Distance(playerTransform.position, originalPlanetTransform.position) <= activationDistance)
            {
                foreach (var satellite in mapSatellites)
                {
                    satellite.SetActive(true);
                    Vector3 directionToSatellite = originalSatelliteTransforms[satellite.name].position - playerTransform.position;
                    
                    localDirection = playerTransform.InverseTransformDirection(directionToSatellite.normalized);
                    float satellitePitch = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
                    float satelliteYaw = -Mathf.Asin(localDirection.y) * Mathf.Rad2Deg;
                    
                    satellitePitch = Mathf.Clamp(satellitePitch, -maxPitch, maxPitch);
                    satelliteYaw = Mathf.Clamp(satelliteYaw, -maxPitch, maxPitch);
                    
                    float satellitePitchRadians = satellitePitch * Mathf.Deg2Rad;
                    float satelliteYawRadians = satelliteYaw * Mathf.Deg2Rad;
                    
                    x = _compassSize * Mathf.Cos(satellitePitchRadians) * Mathf.Sin(satelliteYawRadians);
                    y = _compassSize * Mathf.Cos(satellitePitchRadians) * Mathf.Cos(satelliteYawRadians);
                    z = _compassSize * Mathf.Sin(satellitePitchRadians);
                    
                    satellite.transform.localPosition = new Vector3(x, y, z);
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