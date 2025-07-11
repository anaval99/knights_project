using Firebase.Firestore;

[FirestoreData]
public class CharSkillBar : IFirebaseDoc
{
  [FirestoreProperty]
  public string[] SkillBar { get; set; } = new string[4] { null, null, null, null };
  [FirestoreProperty]
  public string LifePotionId { get; set; } = null;
  [FirestoreProperty]
  public string ManaPotionId { get; set; } = null;
  [FirestoreProperty]
  public string UserId { get; set; } = string.Empty;
}
