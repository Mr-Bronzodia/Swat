using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private string _name;
    [SerializeField]
    [Tooltip("Shoots per seconds")]
    private int _fireRate;
    [SerializeField]
    private float _damagePerShot;
    [SerializeField]
    [Tooltip("Range in meters")]
    private float _effectiveRange;
    [SerializeField]
    [Range(0f, 1f)]
    private float _recoil;
    [SerializeField]
    private int _magazineSize;
    [SerializeField]
    private Transform _barrel;

    private int _remainingBullets;

    public float Recoil { get => _recoil;  }
    public float EffectiveRange { get => _effectiveRange; }
    public float DamagePerShot { get => _damagePerShot; }
    public int FireRate { get => _fireRate; }
    public string Name { get => _name; }
    public int MagazineSize { get => _magazineSize; }
    public int RemainingBullets { get => _remainingBullets; }

    private void Awake()
    {
        _remainingBullets = _magazineSize;
    }

    public void Shoot(Vector3 initialDirection, float accuracy)
    {
        RaycastHit hit;
        
        float directionModifier = Recoil / accuracy;
        Vector3 recoilVector = new Vector3(Random.Range(-directionModifier / 2, directionModifier / 2), Random.Range(-directionModifier, directionModifier), 0f);

        Vector3 direction = (initialDirection + recoilVector).normalized;

        _remainingBullets--;

        Debug.DrawRay(_barrel.position, direction * 100f, Color.red, 3);
        if (Physics.Raycast(_barrel.position, direction, out hit, Mathf.Infinity))
        {
            IDamageable damageable;
            if (!hit.collider.gameObject.TryGetComponent<IDamageable>(out damageable)) return;

            damageable.ReceiveDamage(DamagePerShot);
        }
    }

    public void Reload()
    {
        _remainingBullets = _magazineSize;
    }
}
