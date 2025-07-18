using System;
using System.Collections.Generic;
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

  [FirestoreProperty]
  public string Party1AvatarId { get; set; } = "";
  [FirestoreProperty]
  public string Party2AvatarId { get; set; } = "";
  [FirestoreProperty]
  public List<string> FriendAvatarIds { get; set; } = new ();
}
