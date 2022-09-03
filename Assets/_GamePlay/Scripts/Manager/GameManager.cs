using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoveStopMove.Manager
{
    using System;
    using Utilitys;
    using MoveStopMove.Core.Data;
    using MoveStopMove.Core;
    using System.Linq;
    [DefaultExecutionOrder(-10)]
    public class GameManager : Singleton<GameManager>
    {
        //[SerializeField] UserData userData;
        //[SerializeField] CSVData csv;
        //private static GameState gameState = GameState.MainMenu;

        // Start is called before the first frame update
        public event Action OnStartGame;
        public event Action OnStopGame;
        bool gameIsRun = false;
        public bool GameIsRun => gameIsRun;

        private List<IPersistentData> persistentDataObjects;
        [SerializeField]
        public GameData GameData;
        protected override void Awake()
        {
            base.Awake();
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            int maxScreenHeight = 1280;
            float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
            if (Screen.currentResolution.height > maxScreenHeight)
            {
                Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
            }

            //csv.OnInit();
            //userData?.OnInitData();

            //ChangeState(GameState.MainMenu);
            LoadGame();
            UIManager.Inst.OpenUI(UIID.UICMainMenu);
            
        }

        private void Start()
        {
               
        }

        public void StartGame()
        {
            gameIsRun = true;
            Time.timeScale = 1;
            OnStartGame?.Invoke();
        }

        public void StopGame()
        {
            gameIsRun = false;
            OnStopGame?.Invoke();
        }


        public void NewGame()
        {
            
        }
        public void LoadGame()
        {
            GameData.OnInitData();
        }
        public void SaveGame()
        {
            
        }



        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}