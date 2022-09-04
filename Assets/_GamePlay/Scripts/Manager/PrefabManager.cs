using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolID
{
    Character = 0,
    #region Bullet
    Bullet_Axe1 = 1,
    Bullet_Knife1 = 2,
    Bullet_Axe2 = 3,
    Bullet_Arrow = 4,
    Bullet_1 = 5,
    Bullet_2 = 6,
    Bullet_3 = 7,
    Bullet_4 = 8,
    Bullet_5 = 9,
    Bullet_Player = 99,
    #endregion

    #region Weapon
    Weapon_Axe1 = 100,
    Weapon_Knife1 = 101,
    Weapon_Axe2 = 102,
    Weapon_Arrow = 103,
    Weapon_1 = 104,
    Weapon_2 = 105,
    Weapon_3 = 106,
    Weapon_4 = 107,
    Weapon_5 = 108,
    Weapon_Player = 199,
    #endregion

    #region Skins
    Hair_Arrow = 1000,
    Hair_Cowboy = 1001,
    Hair_Headphone = 1002,
    Hair_Ear = 1003,
    Hair_Crown = 1004,
    Hair_Horn = 1005,
    Hair_Beard = 1006,
    #endregion

    None = 10000,
    UIItem = 10001,
    UITargetIndicator = 10002,
    Obstance = 10003,
    Gift = 10004,
    BaseWeapon = 10005,
    ObjectCreateWeapon = 10006

    
}
namespace MoveStopMove.Manager
{
    using Utilitys;

    [DefaultExecutionOrder(-1)]
    public class PrefabManager : Singleton<PrefabManager>
    {

        //NOTE:Specific for game,remove to reuse
        private GameObject PrefabPool;
        [SerializeField]
        GameObject Character;
        #region Bullet
        [SerializeField]
        GameObject Bullet_Axe1;
        [SerializeField]
        GameObject Bullet_Knife1;
        [SerializeField]
        GameObject Bullet_Axe2;
        [SerializeField]
        GameObject Bullet_Arrow;
        [SerializeField]
        GameObject Bullet_1;
        [SerializeField]
        GameObject Bullet_2;
        [SerializeField]
        GameObject Bullet_3;
        [SerializeField]
        GameObject Bullet_4;
        [SerializeField]
        GameObject Bullet_5;       
        #endregion
        #region Weapon
        [SerializeField]
        GameObject Weapon_Axe1;
        [SerializeField]
        GameObject Weapon_Knife1;
        [SerializeField]
        GameObject Weapon_Axe2;
        [SerializeField]
        GameObject Weapon_Arrow;
        [SerializeField]
        GameObject Weapon_1;
        [SerializeField]
        GameObject Weapon_2;
        [SerializeField]
        GameObject Weapon_3;
        [SerializeField]
        GameObject Weapon_4;
        [SerializeField]
        GameObject Weapon_5;
        #endregion
        #region Hair
        [SerializeField]
        GameObject Hair_Arrow;
        [SerializeField]
        GameObject Hair_Cowboy;
        [SerializeField]
        GameObject Hair_Headphone;
        [SerializeField]
        GameObject Hair_Ear;
        [SerializeField]
        GameObject Hair_Crown;
        [SerializeField]
        GameObject Hair_Horn;
        [SerializeField]
        GameObject Hair_Beard;
        #endregion
        [SerializeField]
        GameObject Obstance;
        
        [SerializeField]
        GameObject UIItem;
        [SerializeField]
        GameObject UIIndicator;
        [SerializeField]
        GameObject Gift;
        [SerializeField]
        GameObject BaseWeapon;
        [SerializeField]
        GameObject ObjectCreateWeapon;
        //-----
        [SerializeField]
        GameObject pool;
        

        Dictionary<PoolID, Pool> poolData = new Dictionary<PoolID, Pool>();
        protected override void Awake()
        {
            base.Awake();

            PrefabPool = Instantiate(pool);
            PrefabPool.name = "PrefabPool";

            CreatePool(Character, PoolID.Character, Quaternion.Euler(0, 0, 0), 15);

            CreatePool(Bullet_Axe1, PoolID.Bullet_Axe1, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_Knife1, PoolID.Bullet_Knife1, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_Axe2, PoolID.Bullet_Axe2, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_Arrow, PoolID.Bullet_Arrow, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_1, PoolID.Bullet_1, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_2, PoolID.Bullet_2, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_3, PoolID.Bullet_3, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_4, PoolID.Bullet_4, Quaternion.Euler(0, 0, 0));
            CreatePool(Bullet_5, PoolID.Bullet_5, Quaternion.Euler(0, 0, 0));

            CreatePool(Weapon_Axe1, PoolID.Weapon_Axe1, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_Knife1, PoolID.Weapon_Knife1, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_Axe2, PoolID.Weapon_Axe2, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_Arrow, PoolID.Weapon_Arrow, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_1, PoolID.Weapon_1, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_2, PoolID.Weapon_2, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_3, PoolID.Weapon_3, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_4, PoolID.Weapon_4, Quaternion.Euler(0, 0, 0));
            CreatePool(Weapon_5, PoolID.Weapon_5, Quaternion.Euler(0, 0, 0));

            CreatePool(Hair_Arrow, PoolID.Hair_Arrow);
            CreatePool(Hair_Cowboy, PoolID.Hair_Cowboy);
            CreatePool(Hair_Headphone, PoolID.Hair_Headphone);
            CreatePool(Hair_Ear, PoolID.Hair_Ear);
            CreatePool(Hair_Crown, PoolID.Hair_Crown);
            CreatePool(Hair_Horn, PoolID.Hair_Horn);
            CreatePool(Hair_Beard, PoolID.Hair_Beard);

            CreatePool(UIItem, PoolID.UIItem);
            CreatePool(UIIndicator, PoolID.UITargetIndicator);
            CreatePool(Obstance, PoolID.Obstance);
            CreatePool(Gift, PoolID.Gift);
            CreatePool(BaseWeapon, PoolID.BaseWeapon, Quaternion.identity, 5);
            CreatePool(ObjectCreateWeapon, PoolID.ObjectCreateWeapon, Quaternion.identity, 50);
        }


        public void CreatePool(GameObject obj, PoolID namePool, Quaternion quaternion = default, int numObj = 10)
        {
            GameObject newPool = Instantiate(pool, Vector3.zero, Quaternion.identity);
            newPool.transform.parent = PrefabPool.transform;
            Pool poolScript = newPool.GetComponent<Pool>();
            newPool.name = namePool.ToString();
            poolScript.Initialize(obj, quaternion, numObj);

            if (!poolData.ContainsKey(namePool))
            {               
                poolData.Add(namePool, poolScript);
            }
            else
            {
                Destroy(poolData[namePool].gameObject);
                poolData[namePool] = poolScript;
            }
        }

        public void PushToPool(GameObject obj, PoolID namePool, bool checkContain = true)
        {
            if (!poolData.ContainsKey(namePool))
            {
                CreatePool(obj, namePool);
            }

            poolData[namePool].Push(obj, checkContain);
        }

        public GameObject PopFromPool(PoolID namePool, GameObject obj = null)
        {
            if (!poolData.ContainsKey(namePool))
            {
                if (obj == null)
                {
                    Debug.LogError("No pool name " + namePool + " was found!!!");
                    return null;
                }
            }

            return poolData[namePool].Pop();
        }

    }
}