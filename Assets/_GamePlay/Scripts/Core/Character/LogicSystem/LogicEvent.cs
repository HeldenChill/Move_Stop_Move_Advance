using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoveStopMove.Core.Character.LogicSystem
{
    using System;
    public class LogicEvent : ScriptableObject
    {
        public Action<Vector3> SetVelocity;
        public Action<GameConst.Type,Quaternion> SetRotation;
        public Action<GameConst.Type, Vector3> SetSmoothRotation;

        public Action<string, bool> SetBool_Anim;
        public Action<string, float> SetFloat_Anim;
        public Action<string, int> SetInt_Anim;
        public Action<Vector3, float> DealDamage;
        public Action<Vector3, float, bool> SpecialDealDamage;
        public Action<bool> SetActive;
        public Action<BaseCharacter, bool> SetTargetIndicatorPosition;


        public Action<Collider> CollideGift;
        public Action EndGiftBonus;
    }
}