using UnityEngine;

public class DashboardController : MonoBehaviour
{
    [SerializeField]
    BodyPartRenderer bodyPartRenderer;
    [SerializeField]
    EquipmentRenderer equipmentRenderer;
    [SerializeField]
    CharDataCenter charDataCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        // Ensure Firebase is initialized before loading the avatar
        this.LoadAvatar();
    }
    
    async void LoadAvatar()
    {
        // Get the Firebase UID from the FirebaseService
        string firebaseUid = FirebaseService.Instance.GetUserId();
        if (string.IsNullOrEmpty(firebaseUid))
        {
            Debug.LogError("Firebase UID is null or empty. User may not be logged in.");
            return;
        }

        // Load character data asynchronously
        await charDataCenter.LoadCharDataAsync(firebaseUid);
    }
}
