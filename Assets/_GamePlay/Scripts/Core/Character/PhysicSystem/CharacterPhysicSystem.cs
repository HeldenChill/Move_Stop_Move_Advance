using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoveStopMove.Core.Character.PhysicSystem
{
    using System;
    using WorldInterfaceSystem;
    public class CharacterPhysicSystem : AbstractCharacterSystem<AbstractPhysicModule,PhysicData,PhysicParameter>
    {
        public CharacterPhysicSystem(AbstractPhysicModule module)
        {
            Data = ScriptableObject.CreateInstance(typeof(PhysicData)) as PhysicData;
            Parameter = ScriptableObject.CreateInstance(typeof(PhysicParameter)) as PhysicParameter;
            this.module = module;
            module.Initialize(Data,Parameter);
        }

        public void SetVelocity(Vector3 velocity)
        {
            module.SetVelocity(velocity);
        }

        public void SetRotation(GameConst.Type type,Quaternion rotation)
        {
            module.SetRotation(type,rotation);
        }

        public void SetSmoothRotation(GameConst.Type type, Vector3 direction)
        {
            module.SetSmoothRotation(type, direction);
        }

        public void SetScale(GameConst.Type type,Vector3 scale)
        {
            module.SetScale(type, scale);
        }

        public void SetScale(GameConst.Type type, float ratio)
        {
            module.SetScale(type, ratio);
        }
    }
}