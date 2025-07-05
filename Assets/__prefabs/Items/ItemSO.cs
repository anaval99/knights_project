using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
  public int Level = 0;
  public string ItemId;
  public string ItemName;
  public string ItemDescription;
  public Sprite ItemIcon;
  public bool IsConsumable;
  public bool IsEquippable;
  public bool IsStackable;
  public string GameObjectName;

  public WeaponClass WeaponClass = WeaponClass.None;
  public int Damage = 0;

  public ArmorPart ArmorPart = ArmorPart.None;
  public int Defense = 0;
  public int Health = 0;
  /// <summary>
  /// Note: Crit chance formula is [PlayerCritPoints / (EnemyLevel * 100)]
  /// </summary>
  public int Critical = 0;


  public Rarity Rarity = Rarity.Common;
}
