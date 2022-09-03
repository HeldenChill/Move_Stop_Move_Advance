using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using MoveStopMove.Manager;
using MoveStopMove.Core.Data;

public class CanvasMainMenu : UICanvas
{
    bool isDirty = false;
    
    [SerializeField]
    TMP_Text descriptionPlayText;
    [SerializeField]
    TMP_Text cash;


    GameData Data;
    private const string ZONE = "Zone:";
    private const string BEST = " - Best:#";

    public void Awake()
    {
        Data = GameManager.Inst.GameData;
    }
    public void PlayGameButton()
    {
        UIManager.Inst.OpenUI(UIID.UICGamePlay);
        GameManager.Inst.StartGame();
        SoundManager.Inst.PlaySound(SoundManager.Sound.Button_Click);
        Close();
    }

    public void ShopSkinButton()
    {
        UIManager.Inst.OpenUI(UIID.UICShopSkin);
        SoundManager.Inst.PlaySound(SoundManager.Sound.Button_Click);
        Close();
    }

    public void ShopWeaponButton()
    {
        UIManager.Inst.ShopWeaponCanvas.Open();
        SoundManager.Inst.PlaySound(SoundManager.Sound.Button_Click);
        Close();
    }

    public override void Open()
    {
        base.Open();       
        if (isDirty)
        {
            //GameplayManager.Inst.PlayerScript.Reset();
            GameManager.Inst.StopGame();
            LevelManager.Inst.OpenLevel(Data.CurrentRegion);
            SoundManager.Inst.PlaySound(SoundManager.Sound.Button_Click);            
        }
        else
        {
            isDirty = true;
        }
        GameplayManager.Inst.SetCameraPosition(CameraPosition.MainMenu);
        UpdateData();
    }

    public void UpdateData()
    {
        string des = ZONE + (Data.CurrentRegion + 1).ToString() + BEST + Data.HighestRank.ToString();
        descriptionPlayText.text = des;
        cash.text = Data.Cash.ToString();
    }

}
