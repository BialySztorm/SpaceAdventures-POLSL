using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities
{
    public class Planet : InteractableBase
    {
        private const double AU = 149597870.7; // Astronomical Unit in km
        private const double Deg2Rad = math.PI / 180.0;
        private const double EpochJ2000 = 2451545.0; // Reference epoch
        
        [SerializeField]
        private float positionScale = 100000.0f;


        // Default variables for Earth
        [SerializeField, Tooltip("Semimajor axis in AU")]
        private double semimajorAxis = 1.00000011; // a
        [SerializeField, Tooltip("Orbital eccentricity")]
        private double eccentricity = 0.01671022; // e
        [SerializeField, Tooltip("Orbital inclination in degrees")]
        private double inclination = 0.00005; // i
        [SerializeField, Tooltip("Longitude of ascending node in degrees")]
        private double longitudeOfAscendingNode = -11.26064; // Ω
        [SerializeField, Tooltip("Argument of perihelion in degrees")]
        private double argumentOfPerihelion = 102.94719; // ω
        [SerializeField, Tooltip("Mean longitude in degrees")]
        private double meanLongitude = 100.46435; // L
        
        // North Pole Rotation
        [SerializeField, Tooltip("Right Ascension of North Pole (first value)")]
        private double rightAscension = 281.010;
        [SerializeField, Tooltip("Right Ascension rate per Julian century (second value)")]
        private double rightAscensionRate = 0.033;
        [SerializeField, Tooltip("Declination of North Pole (first value)")]
        private double declination = 61.414;
        [SerializeField, Tooltip("Declination rate per Julian century (second value)")]
        private double declinationRate = 0.005;
        
        private void Awake()
        {
            // Transform default values
            semimajorAxis = semimajorAxis * AU; // Convert to km
            inclination = inclination * Deg2Rad; // Convert to radians
            longitudeOfAscendingNode = longitudeOfAscendingNode * Deg2Rad; // Convert to radians
            argumentOfPerihelion = argumentOfPerihelion * Deg2Rad; // Convert to radians
            meanLongitude = meanLongitude * Deg2Rad; // Convert to radians
            rightAscension = rightAscension * Deg2Rad; // Convert to radians
            declination = declination * Deg2Rad; // Convert to radians
        }

        private void Start()
        {
            // Get all child elements with Sattelite class
            Sattelite[] sattelites = GetComponentsInChildren<Sattelite>();
            double currentDateTime = DateTime.UtcNow.ToOADate();
            foreach (Sattelite sattelite in sattelites)
            {
                Vector3 position = sattelite.GetCurrentPosition(currentDateTime);
                Quaternion rotation = sattelite.GetCurrentRotation(currentDateTime);
                sattelite.transform.position = position;
                sattelite.transform.rotation = rotation;
            }
        }


        public Vector3 GetCurrentPosition(double julianDate)
        {
            double T = (julianDate - EpochJ2000) / 36525.0; // Julian centuries from J2000.0
            double currentMeanLongitude = meanLongitude + (T * 360.0 * Deg2Rad / 36525.0); // Mean longitude
            double M = currentMeanLongitude - argumentOfPerihelion; // Mean anomaly
            double E = M; // Initial guess for Eccentric Anomaly

            // Solve Kepler's equation iteratively
            for (int j = 0; j < 10; j++)
            {
                E = M + eccentricity * math.sin(E);
            }
            
            // Calculate true anomaly and heliocentric distance
            double nu = 2.0 * math.atan2(math.sqrt(1 + eccentricity) * math.sin(E / 2.0), math.sqrt(1 - eccentricity) * math.cos(E / 2.0));
            double r = semimajorAxis * (1 - eccentricity * math.cos(E));

            // Calculate heliocentric coordinates
            double x = r * (math.cos(longitudeOfAscendingNode) * math.cos(nu + argumentOfPerihelion) - math.sin(longitudeOfAscendingNode) * math.sin(nu + argumentOfPerihelion) * math.cos(inclination));
            double y = r * (math.sin(longitudeOfAscendingNode) * math.cos(nu + argumentOfPerihelion) + math.cos(longitudeOfAscendingNode) * math.sin(nu + argumentOfPerihelion) * math.cos(inclination));
            double z = r * (math.sin(nu + argumentOfPerihelion) * math.sin(inclination));

            return new Vector3((float)x / positionScale, (float)y / positionScale, (float)z / positionScale);
        }

        public Quaternion GetCurrentRotation(double julianDate)
        {
            double T = (julianDate - EpochJ2000) / 36525.0; // Julian centuries from J2000.0
            double RA = rightAscension - rightAscensionRate * T; // Right Ascension of North Pole
            double Dec = declination - declinationRate * T; // Declination of North Pole

            Quaternion rotation = Quaternion.Euler((float)Dec, (float)RA, 0);
            return rotation;
        }
        
    }
}