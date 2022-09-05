using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.ContentCreation.Weapon
{
    using Manager;
    public class Knife1Weapon : BaseWeapon
    {
        public override void DealDamage(BaseAttackInfo data)
        {
            base.DealDamage(data);
            if (WeaponType == WeaponType.Normal)
            {
                GameObject bullet = PrefabManager.Inst.PopFromPool(BulletPoolName);
                bullet.transform.position = firePoint.position;
                bullet.transform.localScale = Vector3.one * data.scale;

                BaseBullet bulletScript = Cache.GetBaseBullet(bullet);
                bulletScript.OnFire(data,Character);
            }
        }

    }
}