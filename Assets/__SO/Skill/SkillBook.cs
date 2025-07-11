using Firebase.Firestore;

[FirestoreData]
public class SkillBook
{
	[FirestoreProperty]
	public string SkillBookSOId { get; set; } = string.Empty;
	[FirestoreProperty]
	public int Quantity { get; set; } = 1;
}