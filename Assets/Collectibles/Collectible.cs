using System.Runtime.CompilerServices;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    private float floatAmplitude = .2f;
    private float floatSpeed = .5f;
    private float rotationSpeed = 30f;

    private Vector3 startPosition;

    [SerializeField] private bool _animate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _audioManager.PlayOneShot("RetroCoin");
            Destroy(this.gameObject);
        }
    }

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (_animate) 
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        }
    }
}
