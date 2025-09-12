using Firebase.Firestore;

[FirestoreData]
public class CharMailAttachment
{
    [FirestoreProperty]
    public string ItemSOId { get; set; }
    [FirestoreProperty]
    public string SkillBookSOId { get; set; }
    [FirestoreProperty]
    public int Qty { get; set; }
}

[FirestoreData]
public class CharMail : IFirebaseDoc
{
    /// <summary>
    /// This will be the sender
    /// </summary>
    [FirestoreProperty]
    public string UserId { get; set; }
    [FirestoreProperty]
    public string Subject { get; set; }
    [FirestoreProperty]
    public string Message { get; set; }
}
