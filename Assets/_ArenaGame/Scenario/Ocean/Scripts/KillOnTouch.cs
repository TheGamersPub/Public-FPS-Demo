using UnityEngine;

public class KillOnTouch : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        if (_collider == null) _collider = GetComponentInChildren<Collider>();
        if (_collider == null) Debug.LogWarning($"Nenhum Collider encontrado no GameObject {gameObject.name} ou em seus filhos.");
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Player player = hit.gameObject.GetComponent<Player>();
        Debug.Log($"Collisionwith: {hit.gameObject.name}");
        if (player != null)
        {
            player.TakeDamage(999);
        }
    }
}
