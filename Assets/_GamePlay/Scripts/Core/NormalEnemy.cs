using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.Core
{
    using MoveStopMove.Manager;
    using MoveStopMove.Core.Character.LogicSystem;
    using MoveStopMove.Core.Character.NavigationSystem;
    using MoveStopMove.Core.Data;

    public class NormalEnemy : BaseCharacter
    {
        protected override void Awake()
        {           
            base.Awake();
            Data = ScriptableObject.CreateInstance(typeof(CharacterData)) as CharacterData;
            LogicSystem.SetCharacterInformation(Data, gameObject.transform);
            WorldInterfaceSystem.SetCharacterInformation(Data);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VFX_Hit = Cache.GetVisualEffectController(VisualEffectManager.Inst.PopFromPool(VisualEffect.VFX_Hit));
            VFX_AddStatus = Cache.GetVisualEffectController(VisualEffectManager.Inst.PopFromPool(VisualEffect.VFX_AddStatus));
            VFX_Hit.Init(transform, Vector3.up * 0.5f, Quaternion.Euler(Vector3.zero), Vector3.one * 0.3f);
            VFX_AddStatus.Init(transform, Vector3.up * -0.5f, Quaternion.Euler(-90, 0, 0), Vector3.one);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VisualEffectManager.Inst.PushToPool(VFX_Hit.gameObject, VisualEffect.VFX_Hit);
        }

        public override void OnInit()
        {
            base.OnInit();
            Data.Hp = 1;
        }
        public override void OnDespawn()
        {
            base.OnDespawn();
            ((CharacterLogicModule)LogicModule).StopStateMachine();
            PrefabManager.Inst.PushToPool(this.gameObject, PoolID.Character);
        }

        
        public override void Run()
        {          
            ((CharacterAI)NavigationModule).StartStateMachine();           
        }

        public override void Stop()
        {
            ((CharacterAI)NavigationModule).StopStateMachine();
        }
    }
}