using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys;

public class CanvasCreateWeapon : UICanvas
{
    [SerializeField]
    DrawMesh drawMesh;
    private int oldWeapon = 0;
    public void CreateWeaponButton()
    {
        drawMesh.CreateWeaponAndBullet();
        if (drawMesh.WeaponObject.GetInstanceID() != oldWeapon)
        {            
            SaveManager.Inst.SaveAsPrefab(drawMesh.WeaponObject);
            SaveManager.Inst.SaveAsPrefab(drawMesh.BulletObject);
            drawMesh.ResetData();
            oldWeapon = drawMesh.WeaponObject.GetInstanceID();
        }
        
    }
}
