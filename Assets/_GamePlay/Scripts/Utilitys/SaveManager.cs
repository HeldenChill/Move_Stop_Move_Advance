using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Utilitys
{
    public class SaveManager : Singleton<SaveManager>
    {
        public void SaveAsPrefab(GameObject gameObject,bool isNew = false)
        {
            if (gameObject == null) return;
            string localPath = "Assets/_GamePlay/Resources/Weapon/" + gameObject.name + ".prefab";

            if (isNew)
            {               
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            }           
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
        }

    }
}