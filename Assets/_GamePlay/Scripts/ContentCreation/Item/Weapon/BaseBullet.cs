using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.ContentCreation.Weapon
{
    using Core;
    using Manager;
    public enum BulletType
    {
        Normal = 0,
        HorizontalRotation = 1
    }
    public class BaseBullet : MonoBehaviour
    {
        private const float AMPLIFY_PARAMETER = 60;
        private const float SPECIAL_MAX_SCALE = 3f;

        float range;
        BaseCharacter parentCharacter;

        [SerializeField]
        LayerMask characterLayer;
        [SerializeField]
        LayerMask obstanceLayer;
        [SerializeField]
        BulletType Type;
        [SerializeField]
        PoolID poolName;
        [SerializeField]
        BoxCollider boxCollider;

        [SerializeField]
        float rotationSpeed = 30f;
        [SerializeField]
        float speed = 0.1f;
        

        Vector3 direction = Vector3.zero;
        bool isSpecial;
        bool isStop = false;

        Vector3 specialScale;       
        float currentSpeed;
        float specialSpeed;
        float lastSpeed => currentSpeed * Time.fixedDeltaTime * AMPLIFY_PARAMETER;
        [HideInInspector]
        public Collider SelfCharacterCollider;
        private void OnEnable()
        {
            isStop = false;
        }
        private void Awake()
        {
            currentSpeed = speed;
        }
        private void FixedUpdate()
        {
            if (range < 0)
            {
                PrefabManager.Inst.PushToPool(this.gameObject, poolName, false);
            }
            else
            {
                range -= lastSpeed;
            }

            if (isStop)
                return;
            //Code When Special
            if (isSpecial)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, specialScale, 0.08f);
                currentSpeed = Mathf.Lerp(currentSpeed, specialSpeed, 0.1f);
            }

            if(Type == BulletType.HorizontalRotation)
            {
                transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime * AMPLIFY_PARAMETER,Space.Self);
            }
            if(direction.sqrMagnitude > 0.001)
            {
                transform.Translate(direction * lastSpeed,Space.World);
            }                      
        }

        public void SetSizeBoxCollider(Vector3 size)
        {
            boxCollider.size = size;
        }

        public void OnHit(BaseCharacter character)
        {
            if(character != parentCharacter)
            {
                if (!character.IsDie)
                {
                    PrefabManager.Inst.PushToPool(this.gameObject, poolName, false);
                    character.TakeDamage(1);
                    parentCharacter.AddStatus(); 
                }               
            }
        }

        public void OnFire(BaseAttackInfo data,BaseCharacter parentCharacter)
        {
            data.direction.y = 0;
            this.direction = data.direction.normalized;          
            this.range = data.range - lastSpeed * 6; 
            this.parentCharacter = parentCharacter;
            this.isSpecial = data.isSpecial;

            if (isSpecial)
            {
                specialScale = transform.localScale * SPECIAL_MAX_SCALE;
                specialSpeed = speed * BaseCharacter.GIFT_BONUS;
            }
            else
            {
                currentSpeed = speed * data.speedRatio;
            }

            if(Type == BulletType.Normal)
            {
                direction.y = 1;
                transform.localRotation = Quaternion.LookRotation(Vector3.up,-direction);
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if(Mathf.Pow(2, col.gameObject.layer) == characterLayer)
            {
                OnHit(Cache.GetBaseCharacter(col));
            }
            else if(Mathf.Pow(2, col.gameObject.layer) == obstanceLayer)
            {
                isStop = true;
            }
        }
    }
}