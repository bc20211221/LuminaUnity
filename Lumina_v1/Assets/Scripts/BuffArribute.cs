using System;
using UnityEngine;
public static class BuffAttribute 
{
    //����ֵ������ؼ�����ֵ
	private static int _purity = 50;
    //������ֵ�仯ʱ����
    public static event System.Action<int> OnPurityChanged;
    public static int Purity
    {
        get { return _purity; }
        set
        {
            if (_purity != value)
            {
                _purity = value;
                // �����¼���֪ͨ���м�����
                OnPurityChanged?.Invoke(_purity);
            }
        }
    }
    //Thiskillbuff���ж�buff�Ƿ��������
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
    //�ж��Ƿ�����������
    private static bool _thiskill = false;
    public static bool Thiskill
    {
        get => _thiskill;
        set 
        {
            if (_thiskill != value)
            {
                Debug.Log($"��{_thiskill}��Ϊ{value}");
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
            // ���������ӽ�ұ仯ʱ�Ļص�
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
            // ���������ӽ�ұ仯ʱ�Ļص�
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
            // ���������ӽ�ұ仯ʱ�Ļص�
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
            // ���������ӽ�ұ仯ʱ�Ļص�
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
            // ���������ӽ�ұ仯ʱ�Ļص�
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
            // ���������ӽ�ұ仯ʱ�Ļص�
            OnSixAttackChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnSixAttackChanged;
    //������������ֵ��ȡ����
    private static float _energyeff = 1;
    public static float EnergyEff
    {
        get => _energyeff;
        set 
        {
            _energyeff = value;
            // ���������ӽ�ұ仯ʱ�Ļص�
            OnEnergyEffChanged?.Invoke(value);
        }
    }
    public static event System.Action<float> OnEnergyEffChanged;
    //���������������
    private static int _biologydeath = 5;
    public static int BiologyDeath
    {
        get => _biologydeath;
        set 
        {
            _biologydeath = value;
            // ���������ӽ�ұ仯ʱ�Ļص�
            OnBiologyDeathChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnBiologyDeathChanged;
    //BossѪ��
    private static int _bossblood = 2500;
    public static int BossBlood
    {
        get => _bossblood;
        set 
        {
            _bossblood = value;
            // ���������ӽ�ұ仯ʱ�Ļص�
            OnBossBloodChanged?.Invoke(value);
        }
    }
    public static event System.Action<int> OnBossBloodChanged;
}
