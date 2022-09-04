using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys;
using MoveStopMove.Manager;
using MoveStopMove.ContentCreation;
using MoveStopMove.ContentCreation.Weapon;

public class CanvasCreateWeapon : UICanvas
{
    private readonly Vector3 BOX_COLLIDER_SIZE_BULLET = new Vector3(0.7f, 0.7f, 0.5f);
    [SerializeField]
    DrawMesh drawMesh;
    [SerializeField]
    Camera screenCamera;
    [SerializeField] 
    ItemData weaponData;

    private int oldWeapon = 0;

    public void SaveWeaponButton()
    {
        drawMesh.CreateWeaponAndBullet();
        if(drawMesh.WeaponObject.GetInstanceID() != oldWeapon)
        {
            oldWeapon = drawMesh.WeaponObject.GetInstanceID();

            BaseWeapon weaponScript = Cache.GetBaseWeapon(drawMesh.WeaponObject);
            weaponScript.SetData(PoolID.Bullet_Player, WeaponType.Normal, weaponData);
            Cache.GetBaseBullet(drawMesh.BulletObject).SetSizeBoxCollider(BOX_COLLIDER_SIZE_BULLET);

            PrefabManager.Inst.CreatePool(drawMesh.WeaponObject, PoolID.Weapon_Player, Quaternion.Euler(0, 0, 0));
            PrefabManager.Inst.CreatePool(drawMesh.BulletObject, PoolID.Bullet_Player, Quaternion.Euler(0, 0, 0));
            PrefabManager.Inst.SaveAsPrefab(drawMesh.WeaponObject);
            PrefabManager.Inst.SaveAsPrefab(drawMesh.BulletObject);

            GameplayManager.Inst.PlayerScript.ChangeWeapon(weaponScript);

            drawMesh.ResetData();
            CloseButton();
        }
    }

    public void ClearWeaponButton()
    {
        drawMesh.ResetData();
    }

    public void CloseButton()
    {
        UIManager.Inst.OpenUI(UIID.UICMainMenu);
        GameplayManager.Inst.SetCameraPosition(CameraPosition.MainMenu);
        SoundManager.Inst.PlaySound(SoundManager.Sound.Button_Click);
        Close();
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
