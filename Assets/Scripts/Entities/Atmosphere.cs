using UnityEngine;


namespace Entities
{
    public class Atmosphere : MonoBehaviour
    {
        private Vector3 windDirection;

        private void Start()
        {
            // Initialize wind direction with random values
            windDirection = new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f)
            );
        }

        private void Update()
        {
            // Apply small random changes to the wind direction
            windDirection.x += Random.Range(-0.001f, 0.001f);
            windDirection.y += Random.Range(-0.001f, 0.001f);
            windDirection.z += Random.Range(-0.001f, 0.001f);

            // Normalize the wind direction to keep it consistent
            windDirection.Normalize();

            // Apply the rotations to the atmosphere's transform
            transform.Rotate(windDirection * (0.5f * Time.deltaTime));
        }
    }
}