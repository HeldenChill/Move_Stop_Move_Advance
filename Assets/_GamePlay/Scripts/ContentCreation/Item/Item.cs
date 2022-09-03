using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.ContentCreation
{
    using Manager;
    public class Item : MonoBehaviour, IDespawn
    {
        [SerializeField]
        protected ItemData data;
        public void SetTranformData()
        {
            if (data == null) return;
            transform.localPosition = data.position;
            transform.localRotation = Quaternion.Euler(data.rotation);
            transform.localScale = data.scale;
        }


        public void OnDespawn()
        {
            PrefabManager.Inst.PushToPool(this.gameObject, data.poolID, false);
        }
    }
}