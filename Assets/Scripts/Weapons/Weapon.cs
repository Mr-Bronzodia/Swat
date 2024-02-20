using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

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
    [SerializeField]
    private VisualEffect _impactEffect;   
    [SerializeField]
    private VisualEffect _muzzleEffect;
    [SerializeField]
    private TrailRenderer _trailEffect;

    private int _remainingBullets;
    private ObjectPool<VisualEffect> _impactPool;
    private ObjectPool<VisualEffect> _flashPool;
    private ObjectPool<TrailRenderer> _trailPool;

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
        int defaultPoolCap = Mathf.RoundToInt((1f / _fireRate) * MagazineSize);
        _impactPool = new ObjectPool<VisualEffect>(CreateImpactVFX, OnTakeVFXFromPool, OnReturnVFXFromPool, OnDestroyVFX, true, defaultPoolCap, defaultPoolCap * 2);

        for (int i = 0; i < defaultPoolCap; i++)
        {
            VisualEffect instance = Instantiate(_impactEffect, _barrel);
            _impactPool.Release(instance);
        }

        _flashPool = new ObjectPool<VisualEffect>(CreateMuzzleVFX, OnTakeVFXFromPool, OnReturnVFXFromPool, OnDestroyVFX, true, defaultPoolCap, defaultPoolCap * 2);

        for (int i = 0; i < defaultPoolCap; i++)
        {
            VisualEffect instance = Instantiate(_muzzleEffect, _barrel);
            _flashPool.Release(instance);
        }

        _trailPool = new ObjectPool<TrailRenderer>(CreateTrailVFX, OnTakeTrialFromPool, OnReturnTrailFromPool, OnDestroyTrail, true, defaultPoolCap, defaultPoolCap * 2);

        for (int i = 0; i < defaultPoolCap; i++)
        {
            TrailRenderer instance = Instantiate(_trailEffect, _barrel);
            _trailPool.Release(instance);
        }
    }

    private VisualEffect CreateImpactVFX()
    {
        VisualEffect instance = Instantiate(_impactEffect, _barrel, transform);
        

        return instance;
    }

    private TrailRenderer CreateTrailVFX()
    {
        TrailRenderer instance = Instantiate(_trailEffect, _barrel, transform);

        return instance;
    }


    private VisualEffect CreateMuzzleVFX()
    {
        VisualEffect instance = Instantiate(_muzzleEffect, _barrel, transform);


        return instance;
    }

    private void OnTakeVFXFromPool(VisualEffect visualEffect)
    {
        visualEffect.gameObject.SetActive(true);
    }

    private void OnTakeTrialFromPool(TrailRenderer trail)
    {
        trail.gameObject.SetActive(true);
    }

    private void OnReturnVFXFromPool(VisualEffect visualEffect)
    {
        visualEffect.gameObject.SetActive(false);
    }

    private void OnReturnTrailFromPool(TrailRenderer trail)
    {
        trail.gameObject.SetActive(false);
    }

    private void OnDestroyVFX(VisualEffect visualEffect)
    {
        Destroy(visualEffect.gameObject);
    }

    private void OnDestroyTrail(TrailRenderer trail)
    {
        Destroy(trail.gameObject);
    }

    private IEnumerator ReleaseVFX(float time, VisualEffect toReturn, bool isImpact)
    {
        yield return new WaitForSeconds(time);

        if (isImpact) _impactPool.Release(toReturn);
        else _flashPool.Release(toReturn);

    }

    private IEnumerator ReleaseTrail(float time, TrailRenderer toReturn)
    {
        yield return new WaitForSeconds(time);
        _trailPool.Release(toReturn);
    }

    public void Shoot(Vector3 initialDirection, float accuracy)
    {
        RaycastHit hit;
        
        float directionModifier = Recoil / accuracy;
        Vector3 recoilVector = new Vector3(Random.Range(-directionModifier / 2, directionModifier / 2), Random.Range(-directionModifier, directionModifier), 0f);

        Vector3 direction = (initialDirection + recoilVector).normalized;

        VisualEffect muzzleFlash = _flashPool.Get();
        muzzleFlash.transform.position = _barrel.position;
        muzzleFlash.Play();

        TrailRenderer trailRenderer = _trailPool.Get();
        trailRenderer.transform.position = _barrel.position;
        trailRenderer.AddPosition(_barrel.position);

        StartCoroutine(ReleaseVFX(0.1f, muzzleFlash, false));

        _remainingBullets--;

        Debug.DrawRay(_barrel.position, direction * 100f, Color.red, 10f);
        if (Physics.Raycast(_barrel.position, direction, out hit, Mathf.Infinity))
        {
            VisualEffect impact = _impactPool.Get();
            impact.transform.position = hit.point;
            impact.Play();
            StartCoroutine(ReleaseVFX( 0.1f, impact, true));

            trailRenderer.transform.position = hit.point;
            StartCoroutine(ReleaseTrail(.1f, trailRenderer));

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
