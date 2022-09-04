using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys;
using MoveStopMove.Manager;

public class CanvasCreateWeapon : UICanvas
{
    [SerializeField]
    DrawMesh drawMesh;
    [SerializeField]
    Camera screenCamera;
    private int oldWeapon = 0;

    public void SaveWeaponButton()
    {
        drawMesh.CreateWeaponAndBullet();
        if(drawMesh.WeaponObject.GetInstanceID() != oldWeapon)
        {
            oldWeapon = drawMesh.WeaponObject.GetInstanceID();
            PrefabManager.Inst.CreatePool(drawMesh.WeaponObject, PoolID.Weapon_Player, Quaternion.Euler(0, 0, 0));
            PrefabManager.Inst.CreatePool(drawMesh.BulletObject, PoolID.Bullet_Player, Quaternion.Euler(0, 0, 0));
            drawMesh.ResetData();
            Close();
        }
    }

    public void ClearButton()
    {
        drawMesh.ResetData();
    }
    public void CreateWeaponButton()
    {
        drawMesh.CreateWeaponAndBullet();
        if (drawMesh.WeaponObject.GetInstanceID() != oldWeapon)
        {
            oldWeapon = drawMesh.WeaponObject.GetInstanceID();
            SaveManager.Inst.SaveAsPrefab(drawMesh.WeaponObject);
            SaveManager.Inst.SaveAsPrefab(drawMesh.BulletObject);
            drawMesh.ResetData();           
        }
        
    }

    public override void Open()
    {
        base.Open();
        screenCamera.gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        screenCamera.gameObject.SetActive(false);
    }
}
