using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.Core.Character.PhysicSystem {
    public class CCPhysicModule : AbstractPhysicModule
    {
        public CharacterController controller;
        [SerializeField]
        GameObject charModel;
        [SerializeField]
        GameObject charSensor;     
        [SerializeField]
        bool active;
        [SerializeField]
        bool useGravity;
        [SerializeField]
        float rotateSpeed = 10f;
        

        Quaternion rotGoal;
        private void Awake()
        {
            SetActive(active);
        }
        public override void SetVelocity(Vector3 velocity)
        {
            controller.Move(velocity * Time.deltaTime);
            Data.Velocity = velocity;
        }
        public override void OnInit()
        {
            SetActive(true);
        }

        public override void SetRotation(GameConst.Type type,Quaternion rotation)
        {
            controller.enabled = false;
            if(type == GameConst.Type.Character)
            {
                gameObject.transform.rotation = rotation;
            }
            else if(type == GameConst.Type.Model)
            {
                charModel.transform.rotation = rotation;
            }
            else if(type == GameConst.Type.Sensor)
            {
                charSensor.transform.rotation = rotation;
            }
            controller.enabled = true;
        }

        public override void SetSmoothRotation(GameConst.Type type, Vector3 direction)
        {
            if (type == GameConst.Type.Character)
            {
                rotGoal = Quaternion.LookRotation(direction);
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rotGoal, rotateSpeed * Time.deltaTime);
            }
            else if (type == GameConst.Type.Model)
            {
                rotGoal = Quaternion.LookRotation(direction);
                charModel.transform.rotation = Quaternion.Slerp(charModel.transform.rotation, rotGoal, rotateSpeed * Time.deltaTime);
            }
            else if (type == GameConst.Type.Sensor)
            {
                rotGoal = Quaternion.LookRotation(direction);
                charSensor.transform.rotation = Quaternion.Slerp(charSensor.transform.rotation, rotGoal, rotateSpeed * Time.deltaTime);
            }

        }

        public override void SetScale(GameConst.Type type, Vector3 scale)
        {
            if (type == GameConst.Type.Character)
            {
                gameObject.transform.localScale = scale;
            }
            else if (type == GameConst.Type.Model)
            {
                charModel.transform.localScale = scale;
            }
            else if (type == GameConst.Type.Sensor)
            {
                charSensor.transform.localScale = scale;
            }
        }

        public override void SetScale(GameConst.Type type, float ratio)
        {
            if (type == GameConst.Type.Character)
            {
                gameObject.transform.localScale = gameObject.transform.localScale * ratio;
            }
            else if (type == GameConst.Type.Model)
            {
                charModel.transform.localScale = charModel.transform.localScale = gameObject.transform.localScale * ratio;

            }
            else if (type == GameConst.Type.Sensor)
            {
                charSensor.transform.localScale = charSensor.transform.localScale = gameObject.transform.localScale * ratio;
            }
        }
        public override void SetActive(bool value)
        {
            active = value;
            controller.enabled = value;
            controller.detectCollisions = value;
        }
        public override void UpdateData()
        {
            if (useGravity)
            {
                if (Parameter.GravityParameter < 0.001f) return;
                else
                {
                    Data.Velocity.y += Parameter.GRAVITY * Parameter.GravityParameter * Time.deltaTime;
                }
            }
            
        }

        
    }
}
