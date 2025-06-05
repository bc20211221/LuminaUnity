using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterLibraryManager : MonoBehaviour
{
    public static CharacterLibraryManager Instance { get; private set; }

    [Header("����")]
    public RectTransform contentParent;
    public GameObject characterCardPrefab;
    public List<CharacterData> allCharacters = new List<CharacterData>();

    [Header("��������")]
    public float snapSpeed = 5f;
    public int cardsPerPage = 3;

    private ScrollRect scrollRect;
    private float[] pagePositions;
    private int currentPage = 0;
    private bool isSnapping = false;

    // ��ɫ��Ƭ�б�
    private List<CharacterCard> characterCards = new List<CharacterCard>();

    // ��ɫѡ���¼�
    public System.Action<CharacterData> onCharacterSelected;

    void Awake()
    {
        // ������ʼ��
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("CharacterLibraryManager������ʼ���ɹ�");
        }
        else
        {
            Debug.LogWarning("CharacterLibraryManager�����Ѵ��ڣ����ٵ�ǰʵ��");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();

        // ����Ҫ���
        if (contentParent == null)
        {
            Debug.LogError("ContentParent����Ϊ�գ�����Inspector������");
            return;
        }

        if (characterCardPrefab == null)
        {
            Debug.LogError("CharacterCardPrefab����Ϊ�գ�����Inspector������");
            return;
        }

        GenerateCharacterCards();
        CalculatePagePositions();

        // ע���λ�������Ľ�ɫ�Ƴ��¼�
        if (SlotManager.Instance != null)
        {
            SlotManager.Instance.onCharacterRemoved += OnCharacterRemoved;
            Debug.Log("��ע��SlotManager�Ľ�ɫ�Ƴ��¼�");
        }
        else
        {
            Debug.LogError("�޷�ע���ɫ�Ƴ��¼���SlotManagerʵ��Ϊ��");
        }
    }

    // ���ɽ�ɫ��Ƭ
    void GenerateCharacterCards()
    {
        // ������п�Ƭ
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        characterCards.Clear();

        // �����¿�Ƭ
        foreach (var character in allCharacters)
        {
            if (character == null)
            {
                Debug.LogError("��ɫ����Ϊ�գ�����������Ƭ");
                continue;
            }

            GameObject card = Instantiate(characterCardPrefab, contentParent);
            Debug.Log($"ʵ������ɫ��Ƭ: {character.characterName}");

            // ȷ����Ƭ����Ϸ�м���
            if (!card.activeSelf)
            {
                Debug.LogWarning($"��ɫ��Ƭ {card.name} δ����Զ�����");
                card.SetActive(true);
            }

            // ��ȡ��Ƭ��CharacterCard���
            CharacterCard cardComponent = card.GetComponent<CharacterCard>();

            if (cardComponent == null)
            {
                Debug.LogError($"Ԥ������δ�ҵ�CharacterCard�������ɫ����: {character.characterName}");
                continue;
            }

            // ȷ�����������
            if (!cardComponent.enabled)
            {
                Debug.LogWarning($"CharacterCard����� {card.name} ��δ���ã��Զ�����");
                cardComponent.enabled = true;
            }

            // ��ȡ��Ƭ��Button���
            Button cardButton = card.GetComponent<Button>();

            if (cardButton == null)
            {
                Debug.LogError($"Ԥ������δ�ҵ�Button�������ɫ����: {character.characterName}");
                continue;
            }

            // ȷ��Button������
            if (!cardButton.enabled)
            {
                Debug.LogWarning($"Button����� {card.name} ��δ���ã��Զ�����");
                cardButton.enabled = true;
            }

            // ��ʼ����Ƭ
            cardComponent.Initialize(character);
            cardComponent.OnCharacterClicked += OnCharacterCardClicked;
            characterCards.Add(cardComponent);

            Debug.Log($"�ɹ���ʼ����ɫ��Ƭ: {character.characterName}");
        }

        Debug.Log($"������ɣ��� {characterCards.Count} �Ž�ɫ��Ƭ");
    }

    // ��Ƭ����¼�
    void OnCharacterCardClicked(CharacterData character)
    {
        Debug.Log($"CharacterLibraryManager�յ�����¼�����ɫ: {character.characterName}");

        // ��ȡ��һ�����ò�λ����
        int nextSlotIndex = SlotManager.Instance.GetNextAvailableSlotIndex();
        Debug.Log($"��һ�����ò�λ����: {nextSlotIndex}");

        // ����п��ò�λ����ӽ�ɫ
        if (nextSlotIndex >= 0)
        {
            // ֪ͨ��λ��������ӽ�ɫ��ָ����λ
            if (SlotManager.Instance.AddCharacterToSlot(character, nextSlotIndex))
            {
                Debug.Log($"�ɹ�����ɫ {character.characterName} ��ӵ���λ {nextSlotIndex}");

                // ���½�ɫ��Ƭ״̬
                UpdateCharacterSelection(character, true);

                // ������ɫѡ���¼�
                if (onCharacterSelected != null)
                {
                    onCharacterSelected(character);
                    Debug.Log($"������ɫѡ���¼�: {character.characterName}");
                }
            }
            else
            {
                Debug.LogWarning($"�޷�����ɫ {character.characterName} ��ӵ���λ {nextSlotIndex}");
            }
        }
        else
        {
            Debug.Log("û�п��ò�λ");
            // ����û�п��ò�λ��������������ʾ�߼�
        }
    }

    // ��ɫ�Ӳ�λ�Ƴ��¼�
    void OnCharacterRemoved(CharacterData character)
    {
        Debug.Log($"�յ���ɫ�Ƴ��¼�: {character.characterName}");
        UpdateCharacterSelection(character, false);
        UnlockCharacterCard(character);
    }

    // ���½�ɫѡ��״̬
    public void UpdateCharacterSelection(CharacterData character, bool isSelected)
    {
        foreach (var card in characterCards)
        {
            if (card.GetCharacter() == character)
            {
                Debug.Log($"���½�ɫ״̬: {character.characterName}, ѡ��״̬: {isSelected}");
                card.SetSelected(isSelected);
                break;
            }
        }
    }

    // ������ɫ��Ƭ
    public void UnlockCharacterCard(CharacterData character)
    {
        foreach (var card in characterCards)
        {
            if (card.GetCharacter() == character)
            {
                card.Unlock();
                break;
            }
        }
    }

    // ����ҳ��λ��
    void CalculatePagePositions()
    {
        int totalPages = Mathf.Max(1, Mathf.CeilToInt((allCharacters.Count - cardsPerPage) / (float)cardsPerPage) + 1);
        pagePositions = new float[totalPages];

        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = i / (float)(totalPages - 1);
            if (totalPages == 1) pagePositions[i] = 0f;  // ���ֻ��һҳ������Ϊ0
        }

        Debug.Log($"����ҳ��λ�����: �� {totalPages} ҳ");
    }

    // ҳ�����
    void Update()
    {
        if (isSnapping)
        {
            float targetPosition = pagePositions[currentPage];
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                snapSpeed * Time.deltaTime
            );

            // ����Ƿ񵽴�Ŀ��λ��
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
            {
                isSnapping = false;
                Debug.Log($"�ѹ�����ҳ�� {currentPage}");
            }
        }
    }

    // �л���һҳ
/*    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            isSnapping = true;
            Debug.Log($"�л�����һҳ: {currentPage}");
        }
        else
        {
            Debug.Log("�Ѿ��ǵ�һҳ");
        }
    }

    // �л���һҳ
    public void NextPage()
    {
        if (currentPage < pagePositions.Length - 1)
        {
            currentPage++;
            isSnapping = true;
            Debug.Log($"�л�����һҳ: {currentPage}");
        }
        else
        {
            Debug.Log("�Ѿ������һҳ");
        }
    }*/

    // ��������ʱ���ã����Զ����ã�
/*    public void OnScrollEnd()
    {
        if (!isSnapping)
        {
            float minDistance = float.MaxValue;
            int closestPage = currentPage;

            // �ҵ������ҳ��
            for (int i = 0; i < pagePositions.Length; i++)
            {
                float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - pagePositions[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPage = i;
                }
            }

            // �����Ҫ�л�ҳ��
            if (closestPage != currentPage)
            {
                currentPage = closestPage;
                isSnapping = true;
                Debug.Log($"�Զ�������ҳ��: {currentPage}");
            }
        }
    }*/
}