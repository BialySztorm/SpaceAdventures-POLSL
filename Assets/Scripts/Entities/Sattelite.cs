using System;
using Unity.Mathematics;
using UnityEngine;

namespace Entities
{
    public class Sattelite : InteractableBase
    {
        private const double Deg2Rad = math.PI / 180.0;
        private const double EpochJ2000 = 2451545.0; // Reference epoch

        // Default orbital parameters for the Moon
        [SerializeField, Tooltip("Semi-major axis in km to power of 6")]
        private double _semiMajorAxis = 0.3844;
        [SerializeField, Tooltip("Orbital eccentricity")]
        private double _eccentricity = 0.0549;
        [SerializeField, Tooltip("Orbital inclination in degrees")]
        private double _inclination = 5.145;
        [SerializeField, Tooltip("Longitude of ascending node in degrees")]
        private double _longitudeOfAscendingNode = 0.0;
        [SerializeField, Tooltip("Argument of perihelion in degrees")]
        private double _argumentOfPerihelion = 0.0;
        [SerializeField, Tooltip("Mean longitude in degrees")]
        private double _meanLongitude = 0.0;
        

        // Rotation parameters
        [SerializeField, Tooltip("Rotation period in days")]
        private double _rotationPeriod = 27.3217;
        [SerializeField, Tooltip("Rotation axis inclination in degrees")]
        private double _rotationAxisInclination = 6.68;
        
        private Planet _planet;
        private Vector3 _inSpacePosition;
        private Quaternion _inSpaceRotation;
        

        private void Awake()
        {
            _planet = GetComponentInParent<Planet>();
            // Transform default values
            _semiMajorAxis = _semiMajorAxis * math.pow(10.0, 6.0); // Convert to km
            _inclination = _inclination * Deg2Rad; // Convert to radians
            _longitudeOfAscendingNode = _longitudeOfAscendingNode * Deg2Rad; // Convert to radians
            _argumentOfPerihelion = _argumentOfPerihelion * Deg2Rad; // Convert to radians
            _meanLongitude = _meanLongitude * Deg2Rad; // Convert to radians
        }

        private void LateUpdate()
        {
            transform.position = _inSpacePosition + _planet.transform.position;
            transform.rotation = _inSpaceRotation;
        }
        
        public void SetInSpaceTransform(Vector3 position, Quaternion rotation)
        {
            _inSpacePosition = position;
            _inSpaceRotation = rotation;
        }

        public Vector3 GetCurrentPosition(double julianDate)
        {
            double T = (julianDate - EpochJ2000) / 36525.0; // Julian centuries from J2000.0
            double currentMeanLongitude = _meanLongitude + (T * 360.0 * Deg2Rad / 36525.0); // Mean longitude
            double M = currentMeanLongitude - _argumentOfPerihelion; // Mean anomaly
            double E = M; // Initial guess for Eccentric Anomaly

            // Solve Kepler's equation iteratively
            for (int j = 0; j < 10; j++)
            {
                E = M + _eccentricity * math.sin(E);
            }

            // Calculate true anomaly and heliocentric distance
            double nu = 2.0 * math.atan2(math.sqrt(1 + _eccentricity) * math.sin(E / 2.0), math.sqrt(1 - _eccentricity) * math.cos(E / 2.0));
            double r = _semiMajorAxis * (1 - _eccentricity * math.cos(E));

            // Calculate heliocentric coordinates
            double x = r * (math.cos(_longitudeOfAscendingNode) * math.cos(nu + _argumentOfPerihelion) - math.sin(_longitudeOfAscendingNode) * math.sin(nu + _argumentOfPerihelion) * math.cos(_inclination));
            double z = r * (math.sin(_longitudeOfAscendingNode) * math.cos(nu + _argumentOfPerihelion) + math.cos(_longitudeOfAscendingNode) * math.sin(nu + _argumentOfPerihelion) * math.cos(_inclination));
            double y = r * (math.sin(nu + _argumentOfPerihelion) * math.sin(_inclination));

            return new Vector3((float)x, (float)y, (float)z);
        }

        public Quaternion GetCurrentRotation(double julianDate)
        {
            double rotationAngle = (julianDate - EpochJ2000) / _rotationPeriod * 360.0; // Rotation angle in degrees
            double RA = _rotationAxisInclination; // Right Ascension of rotation axis
            double Dec = 90.0 - _rotationAxisInclination; // Declination of rotation axis

            Quaternion rotation = Quaternion.Euler((float)Dec, (float)RA, (float)rotationAngle);
            return rotation;
        }
        
    }
}