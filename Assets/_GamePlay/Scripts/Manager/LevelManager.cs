using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MoveStopMove.Manager
{
    using Core;
    using Core.Data;
    using ContentCreation;
    using Utilitys;
    using Utilitys.Timer;
    public class LevelManager : Singleton<LevelManager>,IInit
    {
        public event Action OnWinLevel;
        public event Action OnLoseLevel;
        public const int MARGIN = 2;
        public const int NUM_GIFT_MAX = 6;
        public const float RANDOM_GIFT_POSITION = 2f;
        public const float RANDOM_GIFT_TIME = 5f;
        public const float GROUNG_HEIGHT_PARAMETER = 0.42f;
        

        public Transform Level;
        public Transform StaticEnvironment;
        private List<Obstance> obstances = new List<Obstance>();
        private List<BaseCharacter> characters = new List<BaseCharacter>();

        [SerializeField]
        int difficulty = 3;
        [SerializeField]
        List<LevelData> levelDatas;
        
        [SerializeField]
        private GameObject Ground;
        [SerializeField]             
        private Vector3 position = Vector3.zero;
        [SerializeField]
        private LayerMask groundLayer;

        LevelData currentLevelData;
        private Vector3 groundSize;  
        
        private List<Gift> gifts = new List<Gift>();
        private STimer giftTimer = new STimer();

        private GameData GameData;

        private int numOfSpawnPlayers;
        private int numOfRemainingPlayers;
        private int currentLevel = 0;

        private int NumOfRemainingPlayers
        {
            get => numOfRemainingPlayers;
            set
            {
                numOfRemainingPlayers = value;
                gameplay.SetRemainingPlayerNumber(value + 1);
            }
        }

        CanvasGameplay gameplay;

        protected override void Awake()
        {
            base.Awake();          
            gameplay = UIManager.Inst.GetUI(UIID.UICGamePlay) as CanvasGameplay;
            gameplay.Close();
            GameData = GameManager.Inst.GameData;
        }

        private void Start()
        {
            GameplayManager.Inst.PlayerScript = Cache.GetBaseCharacter(GameplayManager.Inst.Player);
            GameplayManager.Inst.PlayerScript.OnDie += OnPlayerDie;
            GameManager.Inst.OnStartGame += RunLevel;
            OpenLevel(GameManager.Inst.GameData.CurrentRegion);          
        }

        private void OnEnable()
        {
            giftTimer.TimeOut1 += TimerEvent;
            GameManager.Inst.OnStartGame += StartSpawnGift;
        }

        private void OnDisable()
        {
            giftTimer.TimeOut1 -= TimerEvent;
            GameManager.Inst.OnStartGame -= StartSpawnGift;
        }

        private void FixedUpdate()
        {
            TransparentObstance();
        }
        public void OnInit()
        {
            characters.Clear();
            obstances.Clear();
            NumOfRemainingPlayers = currentLevelData.numOfPlayers;
            numOfSpawnPlayers = currentLevelData.numOfPlayers;
            
            for (int i = 0; i < 10; i++)
            {
                //NOTE: UI Target Indicator
                gameplay.SubscribeTarget(SpawnCharacter());
            }
            GameplayManager.Inst.PlayerScript.OnInit();
            ConstructLevel();

            
        }
        
        public void OpenLevel(int level)
        {
            //TODO: Set Data Level
            if(level >= levelDatas.Count)
            {
                currentLevel = levelDatas.Count - 1;
            }
            else if(level < 0)
            {
                currentLevel = 0;
            }
            else
            {
                currentLevel = level;
            }
            
            currentLevelData = levelDatas[currentLevel];
            DestructLevel();
            GameplayManager.Inst.PlayerScript.Reset();
            OnInit();
        }

        public void RunLevel()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].Run();
            }
        }

        public void ConstructLevel()
        {
            groundSize = Vector3.one * currentLevelData.Size * 2;
            Ground.transform.localScale = groundSize;
            Ground.transform.localPosition = -Vector3.up * currentLevelData.Size * 2 * GROUNG_HEIGHT_PARAMETER;
            for (int i = 0; i < currentLevelData.ObstancePositions.Count; i++)
            {
                GameObject obstance = PrefabManager.Inst.PopFromPool(PoolID.Obstance);
                Obstance obstanceScript = Cache.GetObstance(obstance);
                obstance.transform.parent = StaticEnvironment;

                float value = UnityEngine.Random.Range(3, 6f);
                Vector3 scale = new Vector3(value, value, value);
                obstance.transform.localScale = scale;

                Vector3 pos = currentLevelData.ObstancePositions[i] * currentLevelData.Size;
                pos.y = 0.5f;
                obstance.transform.localPosition = pos;
                obstance.transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

                
                obstances.Add(obstanceScript);
            }
        }

        public void DestructLevel()
        {
            numOfSpawnPlayers = 0;
            oldObstance = null;
            giftTimer.Stop();
            for (int i = 0; i < obstances.Count; i++)
            {
                PrefabManager.Inst.PushToPool(obstances[i].gameObject, PoolID.Obstance);
                obstances[i].SetTransparent(false);
            }

            for(int i = 0; i < gifts.Count; i++)
            {
                gifts[i].OnGiftDespawn -= OnGiftDespawn;
                gifts[i].OnDespawn();
            }

            while(characters.Count > 0)
            {          
                characters[0].OnDespawn();
                RemoveCharacter(characters[0]);
            }
        }
        
        private void OnPlayerDie(BaseCharacter player)
        {
            CanvasFail fail = ((CanvasFail)UIManager.Inst.OpenUI(UIID.UICFail));
            fail.SetRank(NumOfRemainingPlayers + 1);

            int bonusCash = GameplayManager.Inst.PlayerScript.Level * 3 + UnityEngine.Random.Range(10, 20);
            GameData.SetIntData(Player.P_CASH, ref GameData.Cash, GameData.Cash + bonusCash);
            fail.SetCash(bonusCash);

            UIManager.Inst.CloseUI(UIID.UICGamePlay);            
            OnLoseLevel?.Invoke();          
        }
        private void OnEnemyDie(BaseCharacter character)
        {
            RemoveCharacter(character);
            NumOfRemainingPlayers -= 1;

            //NOTE: UI Target Indicator
            if(NumOfRemainingPlayers > 0)
            {
                gameplay.SubscribeTarget(SpawnCharacter());
            }
            else
            {
                OnWinLevel?.Invoke();
                CanvasVictory victory = UIManager.Inst.OpenUI(UIID.UICVictory) as CanvasVictory;

                int bonusCash = GameplayManager.Inst.PlayerScript.Level * 3 + UnityEngine.Random.Range(10, 50);
                GameData.SetIntData(Player.P_CASH, ref GameData.Cash, GameData.Cash + bonusCash);

                victory.SetCash(bonusCash);
                victory.SetCurrentLevel(currentLevel);
                currentLevel += 1;
                UIManager.Inst.CloseUI(UIID.UICGamePlay);
            }
        }

        private void RemoveCharacter(BaseCharacter character)
        {
            character.OnDie -= OnEnemyDie;
            characters.Remove(character);
            gameplay.UnsubcribeTarget(character);
        }
        private BaseCharacter SpawnCharacter()
        {
            if(numOfSpawnPlayers <= 0)
            {
                return null;
            }

            numOfSpawnPlayers -= 1;
            GameObject character = PrefabManager.Inst.PopFromPool(PoolID.Character);
            character.transform.parent = Level;           

            BaseCharacter characterScript = Cache.GetBaseCharacter(character);
            
            Vector3 randomPos;
            do
            {
                randomPos = GetRandomPositionCharacter();
            } while ((randomPos - GameplayManager.Inst.Player.transform.position).sqrMagnitude < 2 * GameplayManager.Inst.PlayerScript.AttackRange);
            
            
            characterScript.SetPosition(randomPos);

            int level;
            if(GameplayManager.Inst.PlayerScript.Level <= difficulty)
            {
                level = UnityEngine.Random.Range(1, GameplayManager.Inst.PlayerScript.Level + difficulty);
            }
            else
            {
                level = UnityEngine.Random.Range(GameplayManager.Inst.PlayerScript.Level - difficulty, GameplayManager.Inst.PlayerScript.Level + difficulty);
            }

            characterScript.SetLevel(level);
            characterScript.OnInit();
            if (GameManager.Inst.GameIsRun)
            {
                characterScript.Run();
            }
            else
            {
                characterScript.Stop();
            }
            characterScript.ChangeWeapon(GameplayManager.Inst.GetRandomWeapon());
            characterScript.OnDie += OnEnemyDie;

            

            GameColor color = GameplayManager.Inst.GetRandomColor();
            characterScript.ChangeColor(color);
            PantSkin pantName = GameplayManager.Inst.GetRandomPantSkin();
            characterScript.ChangePant(pantName);
            PoolID hairname = GameplayManager.Inst.GetRandomHair();
            characterScript.ChangeHair(hairname);

            characters.Add(characterScript);
            return characterScript;
                     
        }

        private Vector3 GetRandomPositionCharacter()
        {
            int value = UnityEngine.Random.Range(0, 4);
            float vecX;
            float vecZ;
            if (value == 0)
            {
                vecX = currentLevelData.Size - MARGIN;
                vecZ = UnityEngine.Random.Range(-(currentLevelData.Size - MARGIN) + position.z, currentLevelData.Size - MARGIN + position.z);
            }
            else if(value == 1)
            {
                vecX = -(currentLevelData.Size - MARGIN);
                vecZ = UnityEngine.Random.Range(-(currentLevelData.Size - MARGIN) + position.z, currentLevelData.Size - MARGIN + position.z);
            }
            else if(value == 2)
            {
                vecZ = currentLevelData.Size - MARGIN;
                vecX = UnityEngine.Random.Range(-(currentLevelData.Size - MARGIN) + position.x, currentLevelData.Size - MARGIN + position.x);
            }
            else
            {
                vecZ = -(currentLevelData.Size - MARGIN);
                vecX = UnityEngine.Random.Range(-(currentLevelData.Size - MARGIN) + position.x, currentLevelData.Size - MARGIN + position.x);
            }
            return new Vector3(vecX, GameConst.INIT_CHARACTER_HEIGHT, vecZ);
        }


        #region Spawn Gift
        private void StartSpawnGift()
        {
            //Spawn Gift
            giftTimer.Start(1f);
        }
        private void SpawnGift()
        {
            Vector3 pos;
            float sizeScale = GameplayManager.Inst.PlayerScript.Size;
            if (currentLevelData.GiftPositions.Count == 0)
            {
                pos = new Vector3();
                pos.x = UnityEngine.Random.Range(-0.9f, 0.9f) * currentLevelData.Size;
                pos.z = UnityEngine.Random.Range(-0.9f, 0.9f) * currentLevelData.Size;
                pos.y = GameConst.INIT_CHARACTER_HEIGHT + sizeScale/2;
            }
            else
            {
                int index = UnityEngine.Random.Range(0, currentLevelData.GiftPositions.Count);
                pos = currentLevelData.GiftPositions[index];
                pos.x = pos.x * currentLevelData.Size + UnityEngine.Random.Range(-1f, 1f) * RANDOM_GIFT_POSITION;
                pos.z = pos.z * currentLevelData.Size + UnityEngine.Random.Range(-1f, 1f) * RANDOM_GIFT_POSITION;
                pos.y = GameConst.INIT_CHARACTER_HEIGHT + sizeScale/2;
            }
            GameObject gift = PrefabManager.Inst.PopFromPool(PoolID.Gift);            
            gift.transform.parent = Level;
            gift.transform.localPosition = pos;
            gift.transform.localScale = Vector3.one * sizeScale;

            Gift giftScript = Cache.GetGift(gift);
            giftScript.OnGiftDespawn += OnGiftDespawn;
            gifts.Add(giftScript);
        }

        private void OnGiftDespawn(Gift gift)
        {
            gift.OnGiftDespawn -= OnGiftDespawn;
            gifts.Remove(gift);
            
            if(gifts.Count < NUM_GIFT_MAX)
            {
                float time = UnityEngine.Random.Range(4f, 4f + RANDOM_GIFT_TIME);
                giftTimer.Start(time);
            }
        }

        private void TimerEvent(int value)
        {
            SpawnGift();
            if (gifts.Count < NUM_GIFT_MAX)
            {
                float time = UnityEngine.Random.Range(4f, 4f + RANDOM_GIFT_TIME);
                giftTimer.Start(time);
            }
        }
        #endregion

        #region Check Obstance
        Ray ray = new Ray();
        Obstance oldObstance;
        private void TransparentObstance()
        {
            Vector3 pos1 = GameplayManager.Inst.PlayerCamera.transform.position;
            Vector3 pos2 = GameplayManager.Inst.Player.transform.position + Vector3.up * 0.5f;
            Vector3 direction = pos2 - pos1;

            ray.origin = pos1;
            ray.direction = direction;

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, direction.magnitude, groundLayer))
            {
                Obstance obstance = Cache.GetObstance(hitInfo.collider.gameObject);
                if(obstance != null)
                {
                    oldObstance = obstance;
                    obstance.SetTransparent(true);
                }
                else
                {
                    Debug.Log(hitInfo.collider.gameObject.name);
                }
                
            }
            else
            {
                if(oldObstance != null)
                {
                    oldObstance.SetTransparent(false);
                }
            }

        }
        #endregion
    }
}