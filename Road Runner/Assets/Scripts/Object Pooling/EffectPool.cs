using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectPool : MonoBehaviour
{
    protected PooledEffect[] pooledEffects;
    [SerializeField] protected PooledEffect effectToPool;
    [SerializeField] protected int poolSize;

    private int _currentIndex;

    private void Start()
    {
        pooledEffects = new PooledEffect[poolSize];
        PooledEffect container;

        for (int i = 0; i < poolSize; i++)
        {
            container = Instantiate(effectToPool, transform.position, transform.rotation, transform);
            container.gameObject.SetActive(false);
            pooledEffects[i] = container;
        }
    }

    protected PooledEffect GetNextPooledEffect()
    {
        if (_currentIndex >= poolSize)
            _currentIndex = 0;

        PooledEffect effect = pooledEffects[_currentIndex];
        effect.gameObject.SetActive(true);
        _currentIndex++;

        return effect;
    }

    public void PlaceEffect(Vector3 position, Quaternion rotation)
    {
        PooledEffect effect = GetNextPooledEffect();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.PlayEffects();
    }

    public void PlayEffect()
    {
        PooledEffect effect = GetNextPooledEffect();
        effect.PlayEffects();
    }

    /*    protected PooledEffect GetPooledEffect()
        {
            for (int i = 0; i < poolSize; i++)
            {
                if (!pooledEffects[i].gameObject.activeInHierarchy)
                {
                    return pooledEffects[i];
                }
            }
            return null;
        }*/
}
