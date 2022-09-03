using MoveStopMove.Core;
using MoveStopMove.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys.Input;
using TMPro;

public class CanvasGameplay : UICanvas
{
    private readonly Vector3 TARGET_INDICATOR_UP = Vector3.up * 1.5f;
    private const float SENSITIVITY = 0.1f;
    float minX = UITargetIndicator.WIDTH / 2;
    float maxX = Screen.width - UITargetIndicator.WIDTH / 2;

    float minY = UITargetIndicator.HEIGHT / 2;
    float maxY = Screen.height - UITargetIndicator.HEIGHT / 2;

    public JoyStick joyStick;
    [SerializeField]
    Transform canvasIndicatorTF;
    [SerializeField]
    TMP_Text remainingPlayersNum;
    Dictionary<BaseCharacter, UITargetIndicator> indicators = new Dictionary<BaseCharacter, UITargetIndicator>();
    List<BaseCharacter> characters = new List<BaseCharacter>();
    Camera playerCamera;

    private void Start()
    {
        playerCamera = GameplayManager.Inst.PlayerCamera;
        SubscribeTarget(GameplayManager.Inst.PlayerScript);
    }
    public void FixedUpdate()
    {
        for(int i = 0; i < characters.Count; i++)
        {
            indicators[characters[i]].SetLevel(characters[i].Level);
            Vector3 pos = playerCamera.WorldToScreenPoint(characters[i].transform.position + TARGET_INDICATOR_UP * characters[i].Size);

            //NOTE: Because Clippane camera do not stick exactly to camera(0.01 forward)
            //=> Indicator error when character bettween the clippane camera plane and the actual camera plane
            //The situation happens when distance between character and actual camera plane are < distance between character and clippane camera plane
            if (pos.z < 0)
            {
                pos *= -1;
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.z = 0;
            indicators[characters[i]].transform.position = pos;           
            indicators[characters[i]].SetDirection();

        }
    }
    public void SetRemainingPlayerNumber(int num)
    {
        remainingPlayersNum.text = num.ToString();
    }
    public void SubscribeTarget(BaseCharacter character)
    {
        if (character == null)
            return;

        GameObject uiIndicator = PrefabManager.Inst.PopFromPool(PoolID.UITargetIndicator);
        UITargetIndicator indicatorScript = Cache.GetUIIndicator(uiIndicator);

        //indicatorScript.SetColor(new UnityEngine.Color(1f, 107f/255, 107f/255, 1f));
        indicatorScript.SetColor(GameplayManager.Inst.GetColor(character.Color));
        uiIndicator.transform.SetParent(canvasIndicatorTF);
        indicators.Add(character, indicatorScript);
        characters.Add(character);
    }

    public void UnsubcribeTarget(BaseCharacter character)
    {
        PrefabManager.Inst.PushToPool(indicators[character].gameObject, PoolID.UITargetIndicator);
        indicators.Remove(character);
        characters.Remove(character);
    }
    public void SettingButton()
    {
        UIManager.Inst.OpenUI(UIID.UICSetting);
    }

    public override void Open()
    {
        base.Open();
        GameplayManager.Inst.SetCameraPosition(CameraPosition.Gameplay);
    }
}
