using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.ContentCreation.Weapon
{
    using Manager;

    public class Axe1Weapon : BaseWeapon
    {
        public void Start()
        {
            SetTranformData();
        }
        
        public override void DealDamage(BaseAttackInfo data)
        {
            base.DealDamage(data);
            if(WeaponType == WeaponType.Normal)
            {
                GameObject bullet = PrefabManager.Inst.PopFromPool(BulletPoolName);
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = Quaternion.Euler(-90, 0, transform.rotation.eulerAngles.z);
                bullet.transform.localScale = Vector3.one * data.scale;

                BaseBullet bulletScript = Cache.GetBaseBullet(bullet);
                bulletScript.OnFire(data, Character);
            }
        }
    }
}