using UnityEngine;
using System.Collections;

public class HighBar : MonoBehaviour
{
    [Header("���������˳���Ӧ Q, W, E, R��")]
    public GameObject[] targets = new GameObject[4];

    [Header("ÿ�������������ʱ�䣨��λ���룩")]
    public float[] activeDurations = new float[4] { 1f, 1f, 1f, 1f };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ActivateTarget(0);
        if (Input.GetKeyDown(KeyCode.W))
            ActivateTarget(1);
        if (Input.GetKeyDown(KeyCode.E))
            ActivateTarget(2);
        if (Input.GetKeyDown(KeyCode.R))
            ActivateTarget(3);
    }

    void ActivateTarget(int index)
    {
        if (index < 0 || index >= targets.Length || targets[index] == null)
            return;

        StartCoroutine(ActivateCoroutine(index));
    }

    IEnumerator ActivateCoroutine(int index)
    {
        targets[index].SetActive(true);
        yield return new WaitForSeconds(activeDurations[index]);
        targets[index].SetActive(false);
    }
}
