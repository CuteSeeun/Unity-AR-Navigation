using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterListEntryController
{
    Label m_NameLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        m_NameLabel = visualElement.Q<Label>("MonsterName");
    }

    public void SetMonsterData(MonsterData monsterData)
    {
        m_NameLabel.text = monsterData.m_MonsterName;
    }

}
