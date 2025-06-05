using System;
using UnityEngine;
public static class BuffAttribute 
{
    //纯净值增加相关即健康值
	private static int _purity = 50;
    //当纯净值变化时触发
    public static event System.Action<int> OnPurityChanged;
    public static int Purity
    {
        get { return _purity; }
        set
        {
            if (_purity != value)
            {
                _purity = value;
                // 触发事件，通知所有监听者
                OnPurityChanged?.Invoke(_purity);
            }
        }
    }
    //Thiskillbuff来判断buff是否随机到了
    private static bool _thiskillbuff = false;
    public static bool Thiskillbuff
    {
        get => _thiskillbuff;
        set 
        {
            if (_thiskillbuff != value)
            {
                _thiskillbuff = value;
                OnThiskillbuffChanged?.Invoke(value);
            }
        }
    }
    public static event System.Action<bool> OnThiskillbuffChanged;
    //判断是否有生物死亡
    private static bool _thiskill = false;
    public static bool Thiskill
    {
        get => _thiskill;
        set 
        {
            if (_thiskill != value)
            {
                Debug.Log($"从{_thiskill}变为{value}");
                _thiskill = value;
                OnThiskillChanged?.Invoke(value);
            }
        }

    }
    public static event System.Action<bool> OnThiskillChanged;

    //
	private static int _oneAttack = 5;
    public static int OneAttack
    {
        get => _oneAttack;
        set 
        {
            _oneAttack = value;
            // 这里可以添加金币变化时的回调
            OnOneAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnOneAttackChanged;
    //
    private static int _twoAttack = 5;
    public static int TwoAttack
    {
        get => _twoAttack;
        set 
        {
            _twoAttack = value;
            // 这里可以添加金币变化时的回调
            OnTwoAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnTwoAttackChanged;
    //
    private static int _threeAttack = 5;
    public static int ThreeAttack
    {
        get => _threeAttack;
        set 
        {
            _threeAttack = value;
            // 这里可以添加金币变化时的回调
            OnThreeAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnThreeAttackChanged;
    //
    private static int _fourAttack = 5;
    public static int FourAttack
    {
        get => _fourAttack;
        set 
        {
            _fourAttack = value;
            // 这里可以添加金币变化时的回调
            OnFourAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnFourAttackChanged;
    //
    private static int _fiveAttack = 5;
    public static int FiveAttack
    {
        get => _fiveAttack;
        set 
        {
            _fiveAttack = value;
            // 这里可以添加金币变化时的回调
            OnFiveAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnFiveAttackChanged;
    //
    private static int _sixAttack = 5;
    public static int SixAttack
    {
        get => _sixAttack;
        set 
        {
            _sixAttack = value;
            // 这里可以添加金币变化时的回调
            OnSixAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnSixAttackChanged;
    //光核能量即金币值获取速率
    private static float _energyeff = 1;
    public static float EnergyEff
    {
        get => _energyeff;
        set 
        {
            _energyeff = value;
            // 这里可以添加金币变化时的回调
            OnEnergyEffChanged?.Invoke(value);
        }
    }
    public static event System.Action<float> OnEnergyEffChanged;
    //随机生物死亡条件
    private static int _biologydeath = 5;
    public static int BiologyDeath
    {
        get => _biologydeath;
        set 
        {
            _biologydeath = value;
            // 这里可以添加金币变化时的回调
            OnBiologyDeathChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnBiologyDeathChanged;
    //Boss血量
    private static int _bossblood = 2500;
    public static int BossBlood
    {
        get => _bossblood;
        set 
        {
            _bossblood = value;
            // 这里可以添加金币变化时的回调
            OnBossBloodChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnBossBloodChanged;
}
