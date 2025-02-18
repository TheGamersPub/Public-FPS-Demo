using System.Security.Cryptography;
using UnityEngine;

public class Player : FirstPersonMovement
{
    [Header("References")]
    [SerializeField] private GameObject _headCollision;
    [SerializeField] private CanvasGroup _gameOverPanel;
    [SerializeField] private GameObject _body;

    private int _health = 100;
    private Vector3 _initialPos;

    override protected void Awake()
    {
        base.Awake();
        _initialPos = transform.position;
    }

    public void TakeDamage(int dmg)
    {
        _health -= dmg;
        if (_health <= 0) Die();
    }

    private void Die()
    {
        _headCollision.SetActive(true);
        _body.SetActive(false);
        Invoke(nameof(Respawn), 3f);
    }

    private void Respawn()
    {
        transform.position = _initialPos;
    }
}
