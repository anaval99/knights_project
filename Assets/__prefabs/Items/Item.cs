using System.Runtime.CompilerServices;
using Firebase.Firestore;
public enum WeaponClass
{
  None = 0,
  Sword = 1,
  Wand = 2,
  Bow = 3,
}

public enum ArmorPart
{
  None = 0,
  Helmet = 1,
  Body = 2,
}

public enum Rarity
{
  None = 0,
  Common = 1,
  Rare = 2,
  Epic = 3,
  Legendary = 4,
}

[FirestoreData]
public class Item
{
  [FirestoreProperty]
  public string InstanceId { get; set; } = System.Guid.NewGuid().ToString();
  [FirestoreProperty]
  public string ItemSOId { get; set; }
  [FirestoreProperty]
  public int Quantity { get; set; }
  [FirestoreProperty]
  public int EnchantmentLevel { get; set; } = 0;
}
