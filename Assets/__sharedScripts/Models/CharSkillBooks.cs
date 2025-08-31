
using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
public class CharSkillBooks: IFirebaseDoc
{
  [FirestoreProperty]
  public List<SkillBook> SkillBooks { get; set; } = new List<SkillBook>()
  {
    new() { SkillBookSOId = "beginner_slash", Quantity = 1 },
    new() { SkillBookSOId = "beginner_shot", Quantity = 1 },
    new() { SkillBookSOId = "beginner_bolt", Quantity = 1 },
    new() { SkillBookSOId = "beginner_hp_potion", Quantity = 30 },
    new() { SkillBookSOId = "beginner_mp_potion", Quantity = 30 },
  };
  [FirestoreProperty]
  public string UserId { get; set; } = string.Empty;
}
