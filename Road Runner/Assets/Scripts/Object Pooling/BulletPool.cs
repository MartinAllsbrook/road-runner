using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : ObjectPool
{
    public static BulletPool Instance;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void FireBullet(Vector3 velocity, Vector3 position, float damage)
    {
        GameObject bullet = GetPooledObject();
        
        if (bullet == null)
            Debug.Log("Holy Fuck");

        bullet.transform.position = position;
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().FireBullet(velocity, damage);
    }
}
