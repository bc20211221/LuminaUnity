using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Utility;
using DG.Tweening;
//新增
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Process
{
    public class ComboPresenter : SingletonMonoBehaviour<ComboPresenter>
    {
        [Header("Game Settings")]
        public float maxGameTime = 300f;  // 游戏最大时间，单位：秒（例如 300秒 = 5分钟）
        private float currentGameTime = 0f; // 当前游戏时间
        [Header("Victory Settings")]
        public int requiredScoreToWin = 1000; // 胜利所需得分
        [Header("Completed Panel")]
        public GameObject resultMenu;
        public GameObject victoryPanel;  // 胜利面板
        public GameObject defeatPanel;   // 失败面板
        [Header("UI References")]
        public Text clickText, comboText, EnergyText;
        public Image Light_Energy;
        //public GameObject resultMenu;
        public Text missText, perfectText, greatText, goodText, badText;
        public Text maxComboText;
        public Text ratingText; // 新增：评级文本
        public Text enemyKillText; // 新增：击杀敌人数量文本
        //新增Buff提示文本
        public Text buffText;
        public GameObject buffTextPanel;

        [Header("Game Data")]
        public int MusicScore = 0;
        public int Energy = 0;
        public int miss, perfect, great, good, bad;
        public int combo, maxCombo;
        public int enemyKillCount = 0; // 新增：击杀敌人数量

        //上次触发buff的能量值
        private int lastBuffEnergy = 0;
        //Buff触发阈值
        public int buffTriggerThreshold = 100;
        //Buff显示时间
        public float buffShowTime = 1.5f;
        //剩余Buff显示时间
        private float buffLeftTime = 0;

        [Header("Timing Settings")]
        public float showTime = 0.2f;
        private float leftTime = 0;

        public const int MISS = -1, PERFECT = 0, GREAT = 1, GOOD = 2, BAD = 3;

        //补充
        private HashSet<int> selectedIdSet = new HashSet<int>();
        //Buff类型枚举
        public enum BuffType
        {
            PurityBoost,
            BloodBoost,
            KillEnemyPurityBoost,
            OneAttackBoost,
            TwoAttackBoost,
            ThreeAttackBoost,
            FourAttackBoost,
            FiveAttackBoost,
            SixAttackBoost,
            EnergyEffBoost,
            BiologyDeathBoost
        }
        // 新增：Buff图片存储列表
        private List<Sprite> triggeredBuffImages = new List<Sprite>();
        // 新增：存储触发的Buff类型列表（最多保留最近5次）
        private const int MaxStoredBuffRecords = 20;
        private List<BuffType> triggeredBuffTypes = new List<BuffType>();

        [System.Serializable]
        public struct BuffConfig
        {
            public BuffType buffType;
            public float probability;
            public int maxCount;
            public bool isSelected; // 是否被选中（自动计算）
            public Sprite buffImage; // 新增：Buff对应的图片
        }
        [Header("Buff配置")]
        public List<BuffConfig> buffConfigs;
        private Dictionary<BuffType, int> remainingBuffCounts;
        private HashSet<int> selectedCharacterIds = new HashSet<int>(); // 存储选中的角色ID
        // 新增：选中的Buff集合（用于快速判断）
        private Dictionary<BuffType, bool> selectedBuffStates = new Dictionary<BuffType, bool>();

        // 新增：用于跟踪Miss次数的计数器
        private int missCounter = 0;
        private TowerPlacer towerPlacer;

        // 放大和渐消效果的参数
        public float scaleDuration = 0.8f; // 放大动画的持续时间
        public float fadeDuration = 0.3f;  // 渐消动画的持续时间
        public float scaleAmount = 1.3f;   // 放大的倍数

        // 数字变化的动画参数
        public float scrollDuration = 0.5f; // 数字滚动的持续时间
        public float numberScaleAmount = 1.2f; // 数字变化时的缩放比例

        // 存储原始缩放比例
        private Vector3 originalComboScale;
        private Vector3 originalEnergyScale;

        private bool isGameOver = false;
        //public static ComboPresenter Instance;

        /*private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 防止重复
                return;
            }

            Instance = this;
            // 不设置 DontDestroyOnLoad(gameObject); 保证场景切换重置
            isGameOver = false;
            resultMenu.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }*/


        void Start()
        {
            isGameOver = false;
            resultMenu.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
            towerPlacer = FindObjectOfType<TowerPlacer>();
            currentGameTime = maxGameTime;  // 初始化游戏时间
            TowerDataManager.OnCharacterIdsLoaded += OnCharacterIdsReady; // 订阅事件

            // 记录原始缩放比例
            originalComboScale = comboText.transform.localScale;
            originalEnergyScale = EnergyText.transform.localScale;

            //使用当前时间作为随机数种子
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            //初始化Buff配置
            remainingBuffCounts = new Dictionary<BuffType, int>();
            for (int i = 0; i < buffConfigs.Count; i++)
            {
                var config = buffConfigs[i];
                remainingBuffCounts[config.buffType] = config.maxCount;
                config.probability = config.probability;
            }
            float totalProbability = buffConfigs.Sum(c => c.probability);
            if (Mathf.Abs(totalProbability - 1f) > 0.01f)
            {
                Debug.LogError($"Buff概率总和错误！当前总和：{totalProbability}，请调整配置");
            }
            // 初始化选中状态字典
            foreach (var config in buffConfigs)
            {
                selectedBuffStates[config.buffType] = false;
            }
        }
        //获取被选中的角色ID数组
        private void OnCharacterIdsReady()
        {
            int[] selectedIds = TowerDataManager.Instance.SelectedCharacterIds;
            if (selectedIds != null && selectedIds.Length > 0)
            {
                string idsStr = "[" + string.Join(",", selectedIds) + "]";
                Debug.Log($"已选角色ID数组：{idsStr}");
                selectedCharacterIds = new HashSet<int>(selectedIds);
                UpdateBuffSelectionStates(); // 更新Buff选中状态
                NormalizeBuffProbabilities(); // 归一化概率
            }
            TowerDataManager.OnCharacterIdsLoaded -= OnCharacterIdsReady; // 取消订阅
        }
        // 关键修正：正确匹配角色ID与Buff的选中状态
        private void UpdateBuffSelectionStates()
        {
            foreach (var config in buffConfigs.ToList())
            {
                int index = buffConfigs.IndexOf(config);
                BuffType buffType = config.buffType;

                // 判断是否是六个特定关联ID的Buff
                bool isSpecialBuff = buffType switch
                {
                    BuffType.OneAttackBoost or
                    BuffType.TwoAttackBoost or
                    BuffType.ThreeAttackBoost or
                    BuffType.FourAttackBoost or
                    BuffType.FiveAttackBoost or
                    BuffType.SixAttackBoost => true,
                    _ => false
                };

                if (isSpecialBuff)
                {
                    // 获取该Buff对应的目标ID
                    int targetId = GetCorrespondingIdFromBuffType(buffType);
                    // 选中状态：角色ID数组包含目标ID时为true
                    buffConfigs[index] = new BuffConfig
                    {
                        buffType = buffType,
                        probability = config.probability,
                        maxCount = config.maxCount,
                        isSelected = selectedCharacterIds.Contains(targetId)
                    };
                }
                else
                {
                    // 非特定Buff默认选中
                    buffConfigs[index] = new BuffConfig
                    {
                        buffType = buffType,
                        probability = config.probability,
                        maxCount = config.maxCount,
                        isSelected = true
                    };
                }
            }
        }

        // 关键修正：仅对选中的Buff进行概率归一化
        private void NormalizeBuffProbabilities()
        {
            // 筛选出所有被选中的Buff（包括特定Buff中被选中的和非特定Buff）
            var selectedBuffs = buffConfigs
                .Where(c => c.isSelected)
                .ToList();

            float totalProbability = selectedBuffs.Sum(c => c.probability);

            // 处理总概率为0的边界情况（自动均匀分布）
            if (totalProbability <= Mathf.Epsilon)
            {
                Debug.LogWarning("选中的Buff总概率为0，自动设置为均匀分布");
                if (selectedBuffs.Count > 0)
                {
                    float uniformProb = 1f / selectedBuffs.Count;
                    for (int i = 0; i < selectedBuffs.Count; i++)
                    {
                        int index = buffConfigs.FindIndex(c => c.buffType == selectedBuffs[i].buffType);
                        buffConfigs[index] = new BuffConfig
                        {
                            buffType = selectedBuffs[i].buffType,
                            probability = uniformProb,
                            maxCount = selectedBuffs[i].maxCount,
                            isSelected = true
                        };
                    }
                }
                return;
            }

            // 等比缩放概率，保证总和为1
            float scaleFactor = 1f / totalProbability;
            foreach (var buff in selectedBuffs)
            {
                int index = buffConfigs.FindIndex(c => c.buffType == buff.buffType);
                if (index != -1)
                {
                    buffConfigs[index] = new BuffConfig
                    {
                        buffType = buff.buffType,
                        probability = buff.probability * scaleFactor,
                        maxCount = buff.maxCount,
                        isSelected = true
                    };
                }
            }
        }

        // 新增：获取Buff对应的ID
        private int GetCorrespondingIdFromBuffType(BuffType buffType)
        {
            return buffType switch
            {
                BuffType.OneAttackBoost => 0,
                BuffType.TwoAttackBoost => 1,
                BuffType.ThreeAttackBoost => 2,
                BuffType.FourAttackBoost => 5,
                BuffType.FiveAttackBoost => 3,
                BuffType.SixAttackBoost => 4,
                _ => -1 // 其他类型不关联ID
            };
        }


        private bool lastState = false;
        void Update()
        {
            if (isGameOver) return;
            // 这里可以每帧减少游戏时间
            if (currentGameTime > 0)
            {
                currentGameTime -= Time.deltaTime;
            }

            // 调用 CheckGameOver() 检查游戏是否结束
            CheckGameOver();
            // 处理点击文本显示时间
            if (leftTime <= 0)
            {
                clickText.text = "";
            }
            else
            {
                leftTime -= Time.deltaTime;
            }

            // 实时更新UI信息
            UpdateUI();
            //处理Buff文本显示时间
            if (buffLeftTime > 0)
            {
                buffLeftTime -= Time.deltaTime;
                if (buffLeftTime <= 0)
                {
                    buffText.text = "";
                    if (buffTextPanel != null) buffTextPanel.SetActive(false);
                }
            }
            comboText.text = "" + combo;
            //敌人死亡时纯净值增加的buff事件
            if (BuffAttribute.Thiskillbuff && BuffAttribute.Thiskill && !lastState)
            {
                //纯净值+1
                BuffAttribute.Purity = Math.Min(BuffAttribute.Purity + 1, 50);
                //Debug.Log($"成功触发buff:每次击杀敌人回复1纯净值，当前值：{BuffAttribute.Purity}");
                BuffAttribute.Thiskill = false;
            }
            lastState = BuffAttribute.Thiskill;
            // 检查游戏是否结束
            /*if (IsGameOver())
            {
                GameOver();
            }*/

        }

        private void UpdateUI()
        {
            
            comboText.text = "" + combo;
            EnergyText.text = " " + Energy;
            missText.text = "Miss: " + miss;
            perfectText.text = "Perfect: " + perfect;
            greatText.text = "Great: " + great;
            goodText.text = "Good: " + good;
            badText.text = "Bad: " + bad;
            maxComboText.text = "Max Combo: " + maxCombo;
            ratingText.text = "" + GetRating();
            enemyKillText.text = "" + enemyKillCount;
        }

        public void ShowClick(string text, Color color)
        {
            clickText.text = text;
            clickText.color = color;
            leftTime = showTime;

            // 重置字体大小和透明度
            clickText.transform.localScale = Vector3.one;
            clickText.color = new Color(clickText.color.r, clickText.color.g, clickText.color.b, 1f);

            // 播放放大动画
            LeanTween.scale(clickText.gameObject, Vector3.one * scaleAmount, scaleDuration).setEaseOutBack();

            // 播放渐消动画
            LeanTween.value(clickText.gameObject, 1f, 0f, fadeDuration).setOnUpdate((float alpha) =>
            {
                Color newColor = clickText.color;
                newColor.a = alpha;
                clickText.color = newColor;
            }).setDelay(scaleDuration);
        }

        /*public void ShowResult()
        {
            if (resultMenu.activeSelf)
            {
                resultMenu.SetActive(false);
                return;
            }

            // 更新最大连击数
            if (maxCombo < combo) maxCombo = combo;

            clickText.gameObject.SetActive(false);
            resultMenu.SetActive(true);

            // 更新结果界面文本
            UpdateUI();
        }*/

        //记录触发次数
        int triggerCount = 0;
        public void Combo(int rating)
        {
            int oldCombo = combo;
            int oldMusicScore = MusicScore;
            int oldEnergy = Energy;
            //记录判定前的能量值
            int prevEnergy = Energy;

            switch (rating)
            {
                case MISS:
                    miss++;
                    missCounter++; // 增加Miss计数器
                    ShowClick("MISS", new Color(0.643f, 0.643f, 0.643f));
                    EndCombo();
                    // 检查Miss计数器是否达到5
                    if (missCounter >= 5)
                    {
                        //KillOneTower();
                        missCounter = 0; // 重置Miss计数器
                    }
                    break;
                case PERFECT:
                    perfect++;
                    ShowClick("PERFECT", new Color(1f, 0.957f, 0.682f));
                    combo++;
                    MusicScore += 3;
                    break;
                case GREAT:
                    great++;
                    ShowClick("GREAT", new Color(0.725f, 0.533f, 0.843f));
                    combo++;
                    MusicScore += 2;
                    break;
                case GOOD:
                    good++;
                    ShowClick("GOOD", new Color(0.619f, 0.819f, 0.960f));
                    MusicScore += 1;
                    EndCombo();
                    break;
                case BAD:
                    bad++;
                    ShowClick("BAD", Color.white);
                    EndCombo();
                    break;
            }

            // 更新能量值
            //Energy = MusicScore;
            // 更新能量值和透明度，增加获取速率属性
            Energy = Mathf.RoundToInt(MusicScore * BuffAttribute.EnergyEff) - triggerCount * buffTriggerThreshold;
            //检查能量增加是否触发buff
            BoolTrigger();
            //CheckAndTriggerBuff();
            //UpdateEnergyAlpha();
            //EnergyText.text = "Energy\n" + Energy;
            BattleScoreBoard.Instance.AddMusicScore(MusicScore*10);
            //ScoreText.text = ""+ MusicScore;

            // 确保所有UI元素恢复到原始大小
            comboText.transform.localScale = originalComboScale;
            EnergyText.transform.localScale = originalEnergyScale;

            // 播放Combo、MusicScore和Energy的动画
            PlayNumberChangeAnimation(comboText, oldCombo, combo);
            //PlayNumberChangeAnimation(EnergyText, oldMusicScore, MusicScore);
            PlayNumberChangeAnimation(EnergyText, oldEnergy, Energy);
            UpdateEnergyAlpha();

            // 延迟更新EnergyText的显示，确保数字动画完成
            DOVirtual.DelayedCall(scrollDuration, () => {
                EnergyText.text = "" + Energy;
            });

            BattleScoreBoard.Instance.AddMusicScore(MusicScore*10);
        }
        private void BoolTrigger()
        {
            while (Energy >= buffTriggerThreshold)
            {
                TriggerRandomBuff();
                triggerCount++;
                Energy -= buffTriggerThreshold;
            }
            Debug.Log($"{triggerCount}");
            //EnergyText.text = "Energy\n" + Energy;
            //UpdateEnergyAlpha();
        }


        //新增能量判断是否触发Buff
        private void CheckAndTriggerBuff(int prevEnergy)
        {
            //计算能量增加值
            int energyIncrease = Energy - prevEnergy;
            if (energyIncrease < 0) return;
            //如果能量增加且达到阈值整数倍，则触发buff
            if (energyIncrease > 0 && (Energy / buffTriggerThreshold) > (prevEnergy / buffTriggerThreshold))
            {
                TriggerRandomBuff();
            }
        }


        //Buff随机触发
        private void TriggerRandomBuff()
        {
            Debug.Log("触发Buff检查：进入TriggerRandomBuff方法");
            // 过滤掉已达最大次数的Buff（排除maxCount≠-1且剩余次数≤0的）
            var availableBuffs = buffConfigs
                .Where(config => config.isSelected)
                .Where(config => remainingBuffCounts[config.buffType] == -1 || remainingBuffCounts[config.buffType] > 0)
                .Where(config => config.probability > Mathf.Epsilon)
                .ToList();

            Debug.Log($"触发Buff检查：可用Buff数量={availableBuffs.Count}");

            if (availableBuffs.Count == 0)
            {
                //显示提示信息
                ShowBuffMessage("已获得全部Buff!");
                Debug.LogWarning("所有Buff已达最大触发次数！");
                return;
            }

            // 概率加权随机选择
            float randomValue = UnityEngine.Random.value;
            float cumulativeProb = 0f;
            BuffType selectedBuff = availableBuffs[0].buffType;  // 默认第一个（防止空列表）

            foreach (var buff in availableBuffs)
            {
                cumulativeProb += buff.probability;
                if (randomValue <= cumulativeProb)
                {
                    selectedBuff = buff.buffType;
                    break;
                }
            }

            // 更新剩余次数（仅当maxCount≠-1时）
            if (remainingBuffCounts[selectedBuff] != -1)
            {
                remainingBuffCounts[selectedBuff]--;
            }
            //if (triggeredBuffTypes.Contains(selectedBuff))
            //{
            //triggeredBuffTypes.Remove(selectedBuff); // 移除旧记录
            //}
            // 添加到触发记录（保持最新记录在前，限制数量）
            triggeredBuffTypes.Insert(0, selectedBuff); // 最新触发放在列表开头
            if (triggeredBuffTypes.Count > MaxStoredBuffRecords)
            {
                triggeredBuffTypes.RemoveAt(MaxStoredBuffRecords); // 超过限制则移除最早的记录
            }

            // 新增：当列表变化时输出内容
            PrintTriggeredBuffTypes();


            // 应用Buff效果
            string buffName = GetBuffName(selectedBuff);
            ShowBuffMessage($"{buffName}");
            //buffLeftTime = buffShowTime;
            ApplyBuffEffect(selectedBuff);
            Debug.Log($"触发Buff：{buffName}（剩余次数：{remainingBuffCounts[selectedBuff]}）");

            Debug.Log($"触发Buff检查：选中的Buff类型={selectedBuff}");

        }
        // 新增：打印列表内容的方法
        private void PrintTriggeredBuffTypes()
        {
            string buffList = string.Join(", ", triggeredBuffTypes.Select(t => t.ToString()));
            Debug.Log($"Triggered Buff Types: [{buffList}]");
        }
        // 保持GetTriggeredBuffTypes方法不变，仍返回列表副本：
        public List<BuffType> GetTriggeredBuffTypes()
        {
            return triggeredBuffTypes.ToList(); // 返回当前不重复的有序列表副本
        }

        private void ShowBuffMessage(string message)
        {
            if (buffText == null) return;
            buffText.text = message;
            buffLeftTime = buffShowTime;
            if (buffTextPanel != null)
            {
                buffTextPanel.SetActive(true);

            }

        }

        //获取buff名称
        private string GetBuffName(BuffType buffType)
        {
            switch (buffType)
            {
                case BuffType.PurityBoost: return "【净化】纯净值增加5";
                case BuffType.BloodBoost: return "【削弱】Boss血量减少200";
                case BuffType.KillEnemyPurityBoost: return "【汲取】每次击杀敌人回复1纯净值";
                case BuffType.OneAttackBoost: return "【荧水母】攻击*1.2";
                case BuffType.TwoAttackBoost: return "【执灯鱼】攻击*1.2";
                case BuffType.ThreeAttackBoost: return "【萤火虫】攻击*1.2";
                case BuffType.FourAttackBoost: return "【藻灵】攻击*1.2";
                case BuffType.FiveAttackBoost: return "【雾光菇】攻击*1.2";
                case BuffType.SixAttackBoost: return "【蓝海萤】攻击*1.2";
                case BuffType.EnergyEffBoost: return "【光合】光核能量获取速率*1.2";
                case BuffType.BiologyDeathBoost: return "【顽强】生物死亡条件提升至10 MISS";
                default: return "未知Buff";
            }
        }
        //应用buff效果
        private void ApplyBuffEffect(BuffType buffType)
        {
            BuffImageShow buffManager = GetComponent<BuffImageShow>();
            switch (buffType)
            {
                case BuffType.PurityBoost:
                    //纯净值立刻增加20,但不超过上限200
                    BuffAttribute.Purity = Math.Min(BuffAttribute.Purity + 5, 50);
                    Debug.Log($"纯净值增加20，当前值：{BuffAttribute.Purity}");
                    // 假设已获取到 BuffImageShow 组件的引用（例如通过 GetComponent）
                    //BuffImageShow buffManager = GetComponent<BuffImageShow>();
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost); // 显示图标
                    break;
                case BuffType.BloodBoost:
                    //Boss血量立刻增加2000
                    BuffAttribute.BossBlood -= 300;
                    Debug.Log("Boss血量增加20");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.KillEnemyPurityBoost:
                    BuffAttribute.Thiskillbuff = true;
                    Debug.Log($"随机到buff:每次击杀敌人回复1纯净值，当前值：{BuffAttribute.Purity}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.OneAttackBoost:
                    Debug.Log($"荧水母攻击值：{BuffAttribute.OneAttack}");
                    BuffAttribute.OneAttack = Mathf.RoundToInt((float)BuffAttribute.OneAttack * 1.2f);
                    Debug.Log($"荧水母攻击*1.5,攻击值：{BuffAttribute.OneAttack}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.TwoAttackBoost:
                    Debug.Log($"执灯鱼攻击值：{BuffAttribute.OneAttack}");
                    BuffAttribute.TwoAttack = Mathf.RoundToInt((float)BuffAttribute.TwoAttack * 1.2f);
                    Debug.Log($"执灯鱼攻击*1.5,攻击值：{BuffAttribute.TwoAttack}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.ThreeAttackBoost:
                    Debug.Log($"萤火虫攻击值：{BuffAttribute.ThreeAttack}");
                    BuffAttribute.ThreeAttack = Mathf.RoundToInt((float)BuffAttribute.ThreeAttack * 1.2f);
                    Debug.Log($"萤火虫攻击*1.5,攻击值：{BuffAttribute.ThreeAttack}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.FourAttackBoost:
                    Debug.Log($"藻灵攻击值：{BuffAttribute.FourAttack}");
                    BuffAttribute.FourAttack = Mathf.RoundToInt((float)BuffAttribute.FourAttack * 1.2f);
                    Debug.Log($"藻灵攻击*1.5,攻击值：{BuffAttribute.FourAttack}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.FiveAttackBoost:
                    Debug.Log($"雾光菇攻击值：{BuffAttribute.FiveAttack}");
                    BuffAttribute.FiveAttack = Mathf.RoundToInt((float)BuffAttribute.FiveAttack * 1.2f);
                    Debug.Log($"雾光菇攻击*1.5,攻击值：{BuffAttribute.FiveAttack}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.SixAttackBoost:
                    Debug.Log($"蓝海萤攻击值：{BuffAttribute.SixAttack}");
                    BuffAttribute.SixAttack = Mathf.RoundToInt((float)BuffAttribute.SixAttack * 1.2f);
                    Debug.Log($"蓝海萤攻击*1.5,攻击值：{BuffAttribute.SixAttack}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.EnergyEffBoost:
                    BuffAttribute.EnergyEff = 1.2f;
                    Debug.Log($"当前光核能量获取速率：{BuffAttribute.EnergyEff}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;
                case BuffType.BiologyDeathBoost:
                    BuffAttribute.BiologyDeath = 10;
                    Debug.Log($"当前死亡条件：{BuffAttribute.BiologyDeath}");
                    //buffManager.AddTriggeredBuff(BuffTypeShow.PurityBoost);
                    break;


            }
        }

        private void PlayNumberChangeAnimation(Text textComponent, int oldValue, int newValue, string prefix = "")
        {
            // 先恢复到原始大小
            textComponent.transform.localScale = textComponent == comboText ? originalComboScale : originalEnergyScale;

            // 数字变化动画
            DOTween.To(() => oldValue, x => textComponent.text = prefix + x.ToString(), newValue, scrollDuration)
                .OnStart(() => {
                    // 开始时放大
                    textComponent.transform.DOScale(textComponent == comboText ?
                        originalComboScale * numberScaleAmount :
                        originalEnergyScale * numberScaleAmount, scrollDuration / 2);
                })
                .OnComplete(() => {
                    // 结束时恢复原始大小
                    textComponent.transform.DOScale(textComponent == comboText ?
                        originalComboScale :
                        originalEnergyScale, scrollDuration / 2);
                });
        }

        private void EndCombo()
        {
            if (combo > maxCombo) maxCombo = combo;
            combo = 0;
        }

        private void UpdateEnergyAlpha()
        {
            if (Light_Energy != null)
            {
                float alpha = Mathf.Clamp01(Energy / 500f);

                Color newColor = Light_Energy.color;
                newColor.a = alpha;
                Light_Energy.color = newColor;
            }
        }


        // 新增：获取评级
        private string GetRating()
        {
            string rating = "";
            Color ratingColor = Color.white;

            if (perfect > 0 && miss == 0 && great == 0 && good == 0 && bad == 0)
            {
                rating = "SSS";
                ratingColor = new Color(1f, 0.957f, 0.682f); // 金色
            }
            else if (miss < 5 && (good + great) < 30)
            {
                rating = "SS";
                ratingColor = new Color(1f, 0.957f, 0.682f); // 金色
            }
            else if (miss < 10 && (good + great) < 50)
            {
                rating = "S";
                ratingColor = new Color(1f, 0.957f, 0.682f); 
            }
            else if (IsDefeat())
            {
                rating = "B";
                ratingColor = Color.blue;
            }
            else
            {
                rating = "A";
                ratingColor = new Color(0.725f, 0.533f, 0.843f);
            }

            ratingText.color = ratingColor;
            return rating;
        }

        // 新增：检查是否游戏结束
        private bool IsDefeat()
        {
            // 这里可以根据实际情况添加游戏结束的条件
            // 例如：总部生命值为0
            if (BuffAttribute.Purity <= 0)
            {
                return true;
            }
            return false;

        }

        // 新增：增加击杀敌人数量
        public void AddEnemyKill()
        {
            enemyKillCount++;
        }

        /*private void GameOver()
        {
            ShowResult();
            Time.timeScale = 0; // 停止游戏

            // 确保成功界面不显示
            Headquarters hq = FindObjectOfType<Headquarters>();
            if (hq != null && hq.victoryPanel != null)
            {
                hq.victoryPanel.SetActive(false);
            }
        }*/

        public void CheckGameOver()
        {
            //if (isGameOver) return;
            // 如果纯净值为零，则游戏失败
            if (IsDefeat())
            {
                ShowResult();
            }
            else
            {
                // 如果时间结束，判断是否胜利
                if (Time.time >= maxGameTime) // 假设 maxGameTime 为设定的最大游戏时间
                {
                    ShowResult();
                }
            }
        }

        // 显示胜利面板
        public void ShowVictory()
        {
            resultMenu.SetActive(true);
            victoryPanel.SetActive(true);
            defeatPanel.SetActive(false);
            /*LevelCompleteManager.Instance.ShowVictoryPanel();
            LevelCompleteManager.Instance.NextLevelButton();
            LevelCompleteManager.Instance.UnlockNextLevel();*/
        }

        // 显示失败面板
        public void ShowFailure()
        {
            resultMenu.SetActive(true);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(true);
        }

        // 这个方法会在游戏结束时调用
        public void ShowResult()
        {
            if (isGameOver) return;
            int totalScore = BattleScoreBoard.Instance.GetFinalScore();
            if (BuffAttribute.Purity <= 0)
            {
                ShowFailure();
            }
            else
            {
                if (totalScore >= requiredScoreToWin) // 假设 requiredScoreToWin 为胜利所需的得分
                {
                    ShowVictory();
                    
                }
                else
                {
                    ShowVictory();
                }
            }
            
            UpdateUI();
            isGameOver = true;

        }
    }
}