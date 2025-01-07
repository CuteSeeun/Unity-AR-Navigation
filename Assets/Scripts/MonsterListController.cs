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

        // Resources폴더 안MonsterData안의 모든 리소스 로드하여 리스트에 추가 
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


        #region 사용자 선택에 응답
/*
 * 사용자가 캐릭터를 선택하면 초상화, 성명과 클래스가 오른쪽 화면에 표시되어야 한다.
 * 또한, 캐릭터가 선택되면 선택 버튼이 활성화되어야 한다.
 * 선택된 캐릭터가 없다면 버튼이 다시 비활성화되어야 한다. 
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
