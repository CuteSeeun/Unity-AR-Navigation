using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMonsterClass
{
    DogMonsterA,
    DogMonsterB,
    CubeMonster,
    TriMonster,
    RollMonster,
    RingMonster,
    SuminMonster,
    SeeunMonser,
 }

[CreateAssetMenu]
public class MonsterData : ScriptableObject
{
    public string m_MonsterName;
    public EMonsterClass m_Class;
    public Sprite m_PortraitImage;

}
