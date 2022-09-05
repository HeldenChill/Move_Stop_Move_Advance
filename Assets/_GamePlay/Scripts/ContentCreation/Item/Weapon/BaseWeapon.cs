using MoveStopMove.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType
{
    Normal  = 0,
    Has3Ray = 1
}
namespace MoveStopMove.ContentCreation.Weapon
{
    using Manager;
    public struct BaseAttackInfo
    {
        public Vector3 direction;
        public float range;
        public float scale;
        public bool isSpecial;
        public float speedRatio;
    }
    public abstract class BaseWeapon : Item
    {
        float Range;
        [SerializeField]
        protected PoolID BulletPoolName;
        [SerializeField]
        protected WeaponType WeaponType;
        [SerializeField]
        protected Transform firePoint;
        
        [HideInInspector]
        public BaseCharacter Character;
        public PoolID Name => data.poolID;
             
        public virtual void DealDamage(BaseAttackInfo data)
        {
            SoundManager.Inst.PlaySound(SoundManager.Sound.Weapon_Throw,gameObject.transform.position);
        }

        public void SetData(PoolID BulletPoolName, WeaponType WeaponType,ItemData data)
        {
            this.BulletPoolName = BulletPoolName;
            this.WeaponType = WeaponType;
            this.data = data;
            firePoint = transform;
        }
    }
}