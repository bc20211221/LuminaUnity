using UnityEngine;
using System.Collections;

public class HitAnimationManager : MonoBehaviour
{
    [Header("轨道动画配置")]
    public GameObject[] laneHitAnimations; // 使用GameObject数组，对应四个轨道的动画预制体
    public float animationDuration = 0.2f; // 动画持续时间（需与实际动画时长匹配）

    private void Start()
    {
        // 初始化隐藏所有动画
        for (int i = 0; i < laneHitAnimations.Length; i++)
        {
            if (laneHitAnimations[i])
                laneHitAnimations[i].SetActive(false); // 直接操作GameObject的激活状态
        }
    }

    /// <summary>
    /// 播放指定轨道的打击动画
    /// </summary>
    /// <param name="laneIndex">轨道索引（0-3）</param>
    public void PlayAnimation(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= laneHitAnimations.Length)
        {
            Debug.LogError($"无效轨道索引 {laneIndex}，必须为0-{laneHitAnimations.Length - 1}！");
            return;
        }

        GameObject animationGO = laneHitAnimations[laneIndex];
        if (!animationGO)
        {
            Debug.LogError($"轨道 {laneIndex} 的动画GameObject未赋值！");
            return;
        }

        // 显示动画并开始协程控制隐藏
        animationGO.SetActive(false);
        animationGO.SetActive(true);
        StartCoroutine(HideAnimationAfterDelay(laneIndex));
    }

    /// <summary>
    /// 延迟后隐藏指定轨道的动画
    /// </summary>
    private IEnumerator HideAnimationAfterDelay(int laneIndex)
    {
        yield return new WaitForSeconds(animationDuration);

        if (laneIndex >= 0 && laneIndex < laneHitAnimations.Length)
        {
            laneHitAnimations[laneIndex].SetActive(false); // 隐藏动画GameObject
        }
    }
}