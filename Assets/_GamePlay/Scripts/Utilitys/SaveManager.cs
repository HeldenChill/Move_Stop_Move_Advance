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
        public void SaveAsPrefab(GameObject gameObject)
        {
            if (gameObject == null) return;
            string localPath = "Assets/_GamePlay/Prefabs/Create/" + gameObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
        }
    }
}