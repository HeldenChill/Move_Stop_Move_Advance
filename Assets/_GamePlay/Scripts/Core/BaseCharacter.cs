using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.Core
{
    using Manager;
    using Utilitys;
    using Utilitys.Timer;
    using MoveStopMove.Core.Data;
    using MoveStopMove.Core.Character.WorldInterfaceSystem;
    using MoveStopMove.Core.Character.NavigationSystem;
    using MoveStopMove.Core.Character.PhysicSystem;
    using MoveStopMove.Core.Character.LogicSystem;
    using MoveStopMove.ContentCreation;
    using ContentCreation.Weapon;
    using System;
    public enum CharacterType
    {
        Player = 0,
        Enemy = 1
    }

    public class BaseCharacter : MonoBehaviour,IInit,IDespawn
    {
        public event Action<BaseCharacter> OnDie;
        public const float GIFT_BONUS = 1.6f;

        protected VisualEffectController VFX_Hit;
        protected VisualEffectController VFX_AddStatus;

        [SerializeField]
        protected SkinnedMeshRenderer meshCharacter;
        [SerializeField]
        protected SkinnedMeshRenderer meshPant;
        [SerializeField]
        protected GameObject hair;
        [SerializeField]
        protected Transform SensorTF;
        [SerializeField]
        protected Transform ContainWeaponTF;
        [SerializeField]
        protected Transform ContainHairTF;
        protected CharacterData Data;

        [SerializeField]
        protected WorldInterfaceModule WorldInterfaceModule;
        [SerializeField]
        protected AbstractNavigationModule NavigationModule;
        [SerializeField]
        protected AbstractLogicModule LogicModule;
        [SerializeField]
        protected AbstractPhysicModule PhysicModule;
        [SerializeField]
        protected AnimationModule AnimModule;


        protected CharacterWorldInterfaceSystem WorldInterfaceSystem;
        protected CharacterNavigationSystem NavigationSystem;
        protected CharacterLogicSystem LogicSystem;
        protected CharacterPhysicSystem PhysicSystem;
        protected STimer timerDie = new STimer();

        public BaseWeapon Weapon;
        public bool IsDie
        {
            get
            {
                if (Data.Hp > 0) return false;
                else return true;
            }
        }
        public float Size => Data.Size;
        public float AttackRange => Data.AttackRange;
        public int Level => Data.Level;
        public GameColor Color => (GameColor)Data.Color;

        public bool IsRun => ((CharacterAI)NavigationModule).StateMachine.IsStarted;


        protected virtual void Awake()
        {
            WorldInterfaceSystem = new CharacterWorldInterfaceSystem(WorldInterfaceModule);
            NavigationSystem = new CharacterNavigationSystem(NavigationModule);
            LogicSystem = new CharacterLogicSystem(LogicModule);
            PhysicSystem = new CharacterPhysicSystem(PhysicModule);

                       
            NavigationSystem.SetCharacterInformation(transform, SensorTF, GetInstanceID());
            //NOTE: When change wepon need to set this line of code
            
        }


        public virtual void OnInit()
        {
            PhysicModule.SetActive(true);
            transform.localScale = Vector3.one * Data.Size;
            PhysicModule.SetRotation(GameConst.Type.Model, Quaternion.Euler(0, 0, 0));
            PhysicModule.SetRotation(GameConst.Type.Sensor, Quaternion.Euler(0, 0, 0));          
            Data.AttackCount = 0;
            OnEnable();
            ((CharacterLogicModule)LogicModule).StartStateMachine();            
        }
        

        public virtual void OnDespawn()
        {
        }

        public virtual void Run()
        {        
        }

        public virtual void Stop()
        {
        }
        protected virtual void OnEnable()
        {
            if (LogicSystem.Event.SetVelocity != null) return;
            #region Update Data Event
            WorldInterfaceSystem.OnUpdateData += NavigationSystem.ReceiveInformation;
            WorldInterfaceSystem.OnUpdateData += LogicSystem.ReceiveInformation;

            NavigationSystem.OnUpdateData += LogicSystem.ReceiveInformation;
            PhysicSystem.OnUpdateData += LogicSystem.ReceiveInformation;

            LogicSystem.Event.SetVelocity += PhysicSystem.SetVelocity;
            LogicSystem.Event.SetRotation += PhysicSystem.SetRotation;
            LogicSystem.Event.SetActive += PhysicModule.SetActive;

            LogicSystem.Event.SetSmoothRotation += PhysicSystem.SetSmoothRotation;
            LogicSystem.Event.SetBool_Anim += AnimModule.SetBool;
            LogicSystem.Event.SetFloat_Anim += AnimModule.SetFloat;
            LogicSystem.Event.SetInt_Anim += AnimModule.SetInt;
            LogicSystem.Event.DealDamage += DealDamage;
            LogicSystem.Event.SpecialDealDamage += DealDamage;
            LogicSystem.Event.CollideGift += OnCollideGift;
            LogicSystem.Event.EndGiftBonus += OnEndGiftBonus;

            AnimModule.UpdateEventAnimationState += LogicSystem.ReceiveInformation;
            #endregion           
            
            timerDie.TimeOut1 += TimerEvent;
        }

        protected virtual void OnDisable()
        {
            #region Update Data Event
            WorldInterfaceSystem.OnUpdateData -= NavigationSystem.ReceiveInformation;
            WorldInterfaceSystem.OnUpdateData -= LogicSystem.ReceiveInformation;

            NavigationSystem.OnUpdateData -= LogicSystem.ReceiveInformation;
            PhysicSystem.OnUpdateData -= LogicSystem.ReceiveInformation;

            LogicSystem.Event.SetVelocity -= PhysicSystem.SetVelocity;
            LogicSystem.Event.SetRotation -= PhysicSystem.SetRotation;
            LogicSystem.Event.SetActive -= PhysicModule.SetActive;

            LogicSystem.Event.SetSmoothRotation -= PhysicSystem.SetSmoothRotation;
            LogicSystem.Event.SetBool_Anim -= AnimModule.SetBool;
            LogicSystem.Event.SetFloat_Anim -= AnimModule.SetFloat;
            LogicSystem.Event.SetInt_Anim -= AnimModule.SetInt;
            LogicSystem.Event.DealDamage -= DealDamage;
            LogicSystem.Event.SpecialDealDamage -= DealDamage;
            LogicSystem.Event.CollideGift -= OnCollideGift;
            LogicSystem.Event.EndGiftBonus -= OnEndGiftBonus;

            AnimModule.UpdateEventAnimationState -= LogicSystem.ReceiveInformation;
            #endregion
            
            timerDie.TimeOut1 -= TimerEvent;
        }
        public void Reset()
        {
            SetLevel(1);
            SetPosition(new Vector3(0, GameConst.INIT_CHARACTER_HEIGHT, 0));
            OnInit();
        }

        public void SetPosition(Vector3 position)
        {
            PhysicModule.SetActive(false);
            transform.localPosition = position;
            PhysicModule.SetActive(true);
        }

        public void SetLevel(int level)
        {
            Data.Level = level;
            transform.localScale = Vector3.one * Data.Size;
        }
        protected virtual void Update()
        {
            WorldInterfaceSystem.Run();
            NavigationSystem.Run();
            LogicSystem.Run();
            PhysicSystem.Run();
        }

        protected virtual void FixedUpdate()
        {
            LogicSystem.FixedUpdateData();
        }

        protected virtual void DealDamage(Vector3 direction, float range)
        {
            Weapon.DealDamage(direction, range ,Data.Size);
        }

        protected virtual void DealDamage(Vector3 direction, float range, bool isSpecial)
        {
            Weapon.DealDamage(direction, range, Data.Size, isSpecial);
        }

        public virtual void ChangeColor(GameColor color)
        {
            Material mat = GameplayManager.Inst.GetMaterial(color);
            meshCharacter.material = mat;
            Data.Color = (int)color;
            VFX_Hit.SetColor(GameplayManager.Inst.GetColor(Color));
        }

        public virtual void ChangePant(PantSkin name)
        {
            Material mat = GameplayManager.Inst.GetMaterial(name);
            meshPant.material = mat;
            Data.Pant = (int)name;
        }
        public virtual void ChangeHair(PoolID hair)
        {
            if(hair != PoolID.None)
            {
                GameObject hairObject = PrefabManager.Inst.PopFromPool(hair);
                hairObject.transform.parent = ContainHairTF;
                Cache.GetItem(hairObject).SetTranformData();

                if(this.hair != null)
                {
                    Cache.GetItem(this.hair).OnDespawn();
                }
                this.hair = hairObject;
            }
            Data.Hair = (int)hair;
        }
        public virtual void ChangeWeapon(BaseWeapon weapon)
        {
            if(weapon != null)
            {
                if(Weapon != null)
                {
                    Weapon.OnDespawn();
                }
                
                Weapon = weapon;
                Weapon.Character = this;
                Weapon.gameObject.transform.parent = ContainWeaponTF;
                Weapon.SetTranformData();
                Data.Weapon = (int)Weapon.Name;
            }          
        }
            
        public void TakeDamage(int damage)
        {
            Data.Hp -= damage;
            VFX_Hit.Play();
            SoundManager.Inst.PlaySound(SoundManager.Sound.Character_Hit,transform.position);
            if(Data.Hp <= 0)
            {
                SoundManager.Inst.PlaySound(SoundManager.Inst.GetRandomDieSound(), transform.position);
                timerDie.Start(GameConst.ANIM_IS_DEAD_TIME, 0);
              
            }
        }

        //TODO: Combat Function(Covert to a system)
        public virtual void AddStatus()
        {
            Data.Level += 1;

            //TODO: Increase Size of character
            //TODO: Increase Size of Attack Range Indicator
            SoundManager.Inst.PlaySound(SoundManager.Sound.Character_SizeUp, transform.position);           
            VFX_AddStatus.Play();
            
            if(Data.Level < 10)
            {
                PhysicModule.SetScale(GameConst.Type.Character, 1.1f);
            }
            else if(Data.Level < 20)
            {
                PhysicModule.SetScale(GameConst.Type.Character, 1.05f);
            }
            else
            {
                PhysicModule.SetScale(GameConst.Type.Character, 1.02f);
            }

        }

        protected virtual void OnCollideGift(Collider col)
        {
            if (col)
            {
                Data.BaseAttackRange = CharacterData.BASE_ATTACK_RANGE * GIFT_BONUS;
                Cache.GetGift(col.gameObject).OnDespawn();
            }
        }

        protected virtual void OnEndGiftBonus()
        {
            Data.BaseAttackRange = CharacterData.BASE_ATTACK_RANGE;
        }
        private void Die()
        {
            OnDie?.Invoke(this);
            timerDie.Start(2f, 1);
        }

        private void TimerEvent(int code)
        {
            if(code == 0)
            {
                Die();
            }
            else if(code == 1)
            {
                OnDespawn();             
            }
        }
        
    }
}
