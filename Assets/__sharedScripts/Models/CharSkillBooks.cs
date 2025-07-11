
using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
public class CharSkillBooks: IFirebaseDoc
{
  [FirestoreProperty]
  public List<SkillBook> SkillBooks { get; set; } = new List<SkillBook>()
  {
    new() { SkillBookSOId = "normal_sword", Quantity = 1 },
  };
  [FirestoreProperty]
  public string UserId { get; set; } = string.Empty;
}
