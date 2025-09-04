using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootedItem
{
    [SerializeField]
    public ItemSO ItemSO;
    [SerializeField]
    public SkillBookSO SkillBookSO;
    [SerializeField]
    public int Qty;
}

[CreateAssetMenu(fileName = "LootSO", menuName = "Scriptable Objects/LootSO")]
public class LootSO : ScriptableObject
{
    [SerializeField]
    public List<LootedItem> LootedItems = new();
}
