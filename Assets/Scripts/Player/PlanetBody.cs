using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Player
{
    public class PlanetBody
    {
        private GameObject mapPlanet { get; set; }
        private List<GameObject> mapSatellites { get; set; }
        private Vector3 OriginalPlanetLocation { get; set; }
        private Dictionary<string, Vector3> OriginalSatelliteLocations { get; set; }

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
            mapPlanet = new GameObject(planet.name);
            mapPlanet.transform.SetParent(parent);
            AddMesh(planet, mapPlanet);
            OriginalPlanetLocation = planet.transform.position;
            mapPlanet.transform.localPosition = Vector3.zero;
            mapPlanet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            mapSatellites = new List<GameObject>();
            OriginalSatelliteLocations = new Dictionary<string, Vector3>();

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
                    OriginalSatelliteLocations[child.gameObject.name] = child.transform.position;
                }
            }
        }

        public void UpdatePosition(Vector3 newPlanetPosition, float activationDistance)
        {
            mapPlanet.transform.position = newPlanetPosition;

            foreach (var satellite in mapSatellites)
            {
                float distanceToOriginal = Vector3.Distance(newPlanetPosition, OriginalSatelliteLocations[satellite.name]);
                if (distanceToOriginal <= activationDistance)
                {
                    satellite.SetActive(true);
                    satellite.transform.position = OriginalSatelliteLocations[satellite.name] + (newPlanetPosition - OriginalPlanetLocation);
                }
                else
                {
                    satellite.SetActive(false);
                }
            }
        }

        public void CompassUpdate(Transform playerTransform,float compassAngle = 180f, float activationDistance = 50f)
        {
            
        }
    }
}