using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class CharInventory : IFirebaseDoc
{
  [FirestoreProperty]
  public List<Item> Items { get; set; } = new List<Item>()
  {
    // add beginner sword wand bow and armor
    new Item() { ItemSOId = "beginner_sword", Quantity = 1 },
    new Item() { ItemSOId = "beginner_wand", Quantity = 1 },
    new Item() { ItemSOId = "beginner_bow", Quantity = 1 },
    new Item() { ItemSOId = "beginner_armor", Quantity = 1 },
    new Item() { ItemSOId = "beginner_robe", Quantity = 1 },
    new Item() { ItemSOId = "beginner_garbs", Quantity = 1 },
  };

  [FirestoreProperty]
  public string UserId { get; set; } = string.Empty;
}