using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterListController : MonsterListEntryController
{
    void FillMonsterList()
    {
        m_MonsterList.makeItem = () =>
        {
            var newListEntry = m_ListEntryTemplate.Instantiate();

            var newListEntryLogic = new MonsterListController();

            newListEntry.userData = newListEntryLogic;

            newListEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };

        m_MonsterList.bindItem = (item, index) =>
        {
            (item.userData as MonsterListEntryController).SetMonsterData(m_AllMonsters[index]);
        };

        m_MonsterList.fixedItemHeight = 25;
        m_MonsterList.itemsSource = m_AllMonsters;
    }

    List<MonsterData> m_AllMonsters;
    void EnumerateAllMonster()
    {
        m_AllMonsters = new List<MonsterData>();

        // Resources���� ��MonsterData���� ��� ���ҽ� �ε��Ͽ� ����Ʈ�� �߰� 
        m_AllMonsters.AddRange(Resources.LoadAll<MonsterData>("Monsters"));
    }

    VisualTreeAsset m_ListEntryTemplate;

    ListView m_MonsterList;
    Label m_MonsterClassLabel;
    Label m_MonsterNameLabel;
    VisualElement m_MonsterPortrait;
    Button m_SelectMonsterButton;
    public void InitializeCharacterList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        EnumerateAllMonster();

        m_ListEntryTemplate = listElementTemplate;
        m_MonsterList = root.Q<ListView>("MonsterList");
        m_MonsterClassLabel = root.Q<Label>("MonsterClass");
        m_MonsterNameLabel = root.Q<Label>("MonsterName");
        m_MonsterPortrait = root.Q<VisualElement>("MonsterPortrait");

        m_SelectMonsterButton = root.Q<Button>("SelectMonsterButton");


        #region ����� ���ÿ� ����
/*
 * ����ڰ� ĳ���͸� �����ϸ� �ʻ�ȭ, ����� Ŭ������ ������ ȭ�鿡 ǥ�õǾ�� �Ѵ�.
 * ����, ĳ���Ͱ� ���õǸ� ���� ��ư�� Ȱ��ȭ�Ǿ�� �Ѵ�.
 * ���õ� ĳ���Ͱ� ���ٸ� ��ư�� �ٽ� ��Ȱ��ȭ�Ǿ�� �Ѵ�. 
 */
        #endregion
        m_MonsterList.onSelectionChange += OnMonsterSelected;

        FillMonsterList();
    }

    void OnMonsterSelected(IEnumerable<object> selectedItems)
    {
        var selectedMonster = m_MonsterList.selectedItem as MonsterData;

        if (selectedMonster == null)
        {
            m_MonsterClassLabel.text = "";
            m_MonsterNameLabel.text = "";
            m_MonsterPortrait.style.backgroundImage = null;

            m_SelectMonsterButton.SetEnabled(false);

            return;
        }

        m_MonsterClassLabel.text = selectedMonster.m_Class.ToString();
        m_MonsterNameLabel.text = selectedMonster.m_MonsterName;
        m_MonsterPortrait.style.backgroundImage = new StyleBackground(selectedMonster.m_PortraitImage);

        m_SelectMonsterButton.SetEnabled(true);
    }
}
