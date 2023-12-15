using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectPool : MonoBehaviour
{
    protected CustomEffect[] pooledEffects;
    [SerializeField] protected CustomEffect effectToPool;
    [SerializeField] protected int poolSize;

    private int _currentIndex;

    private void Start()
    {
        pooledEffects = new CustomEffect[poolSize];
        CustomEffect container;

        for (int i = 0; i < poolSize; i++)
        {
            container = Instantiate(effectToPool, transform.position, transform.rotation, transform);
            container.gameObject.SetActive(false);
            pooledEffects[i] = container;
        }
    }

    protected CustomEffect GetNextCustomEffect()
    {
        if (_currentIndex >= poolSize)
            _currentIndex = 0;

        CustomEffect effect = pooledEffects[_currentIndex];
        effect.gameObject.SetActive(true);
        _currentIndex++;

        return effect;
    }

    public void PlaceEffect(Vector3 position, Quaternion rotation)
    {
        CustomEffect effect = GetNextCustomEffect();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.PlayEffects();
    }

    public void PlayEffect()
    {
        CustomEffect effect = GetNextCustomEffect();
        effect.PlayEffects();
    }

    /*    protected CustomEffect GetCustomEffect()
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
