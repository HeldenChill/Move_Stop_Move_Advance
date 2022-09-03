using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.ContentCreation.Weapon
{
    using Manager;
    public class HorizontalWeapon : BaseWeapon
    {
        public void Start()
        {
            SetTranformData();
        }

        public override void DealDamage(Vector3 direction, float range, float size, bool isSpecial = false)
        {
            base.DealDamage(direction, range, size, isSpecial);
            if (WeaponType == WeaponType.Normal)
            {
                GameObject bullet = PrefabManager.Inst.PopFromPool(BulletPoolName);
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = Quaternion.Euler(-90, 0, transform.rotation.eulerAngles.z);
                bullet.transform.localScale = Vector3.one * size;

                BaseBullet bulletScript = Cache.GetBaseBullet(bullet);
                bulletScript.OnFire(direction, range, Character, isSpecial);
            }
        }
    }
}