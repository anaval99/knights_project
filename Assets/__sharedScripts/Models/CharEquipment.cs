using Firebase.Firestore;

[FirestoreData]
public class CharEquipment: IFirebaseDoc
{
  [FirestoreProperty]
  public Item Weapon { get; set; }
  [FirestoreProperty]
  public Item Armor { get; set; }
  [FirestoreProperty]
  public string UserId { get; set; } = string.Empty;
}
