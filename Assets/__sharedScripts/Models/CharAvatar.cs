using System;
using Firebase.Firestore;
using UnityEngine;

[FirestoreData]
public class CharAvatar : IFirebaseDoc
{
  [FirestoreProperty]
  public string HairColor { get; set; } = "Default"; // Default hair color
  [FirestoreProperty]
  public string Hair { get; set; } = "Hair01";
  [FirestoreProperty]
  public string Eye { get; set; } = "Eye01";
  [FirestoreProperty]
  public string Mouth { get; set; } = "Mouth01";
  [FirestoreProperty]
  public string CharacterName { get; set; } = "";
  [FirestoreProperty]
  public string UserId { get; set; }
  [FirestoreProperty]
  public string AvatarId { get; set; } = Guid.NewGuid().ToString();
}
