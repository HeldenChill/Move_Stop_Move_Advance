using System.Collections.Generic;
using UnityEngine;

namespace MoveStopMove.Manager
{
    using Utilitys;

    public enum VisualEffect
    {
        VFX_Hit = 0,
        VFX_AddStatus = 1,
    }
    [DefaultExecutionOrder(-1)]
    public class VisualEffectManager : Singleton<VisualEffectManager>
    {


        [SerializeField]
        GameObject hitEffect;
        [SerializeField]
        GameObject addStatusEffect;

        [SerializeField]
        GameObject pool;
        Dictionary<VisualEffect, Pool> visualEffectData = new Dictionary<VisualEffect, Pool>();
        protected override void Awake()
        {
            base.Awake();
            CreatePool(hitEffect, VisualEffect.VFX_Hit);
            CreatePool(addStatusEffect, VisualEffect.VFX_AddStatus);
        }


        public void CreatePool(GameObject obj, VisualEffect nameEffect, Quaternion quaternion = default, int numObj = 10)
        {
            if (!visualEffectData.ContainsKey(nameEffect))
            {
                GameObject newPool = Instantiate(pool, Vector3.zero, Quaternion.identity);
                Pool poolScript = newPool.GetComponent<Pool>();
                newPool.name = nameEffect.ToString();
                poolScript.Initialize(obj, quaternion, numObj);
                visualEffectData.Add(nameEffect, poolScript);
            }
        }

        public void PushToPool(GameObject obj, VisualEffect nameEffect, bool checkContain = true)
        {
            if (!visualEffectData.ContainsKey(nameEffect))
            {
                CreatePool(obj, nameEffect);
            }

            visualEffectData[nameEffect].Push(obj, checkContain);
        }

        public GameObject PopFromPool(VisualEffect nameEffect, GameObject obj = null)
        {
            if (!visualEffectData.ContainsKey(nameEffect))
            {
                if (obj == null)
                {
                    Debug.LogError("No pool name " + nameEffect + " was found!!!");
                    return null;
                }
            }

            return visualEffectData[nameEffect].Pop();
        }
    }
}