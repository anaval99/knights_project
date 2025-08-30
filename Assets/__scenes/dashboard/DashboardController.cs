using System.Collections.Generic;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{
    [SerializeField]
    BodyPartRenderer bodyPartRenderer;
    [SerializeField]
    EquipmentRenderer equipmentRenderer;
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    Button logoutButton;
    [SerializeField]
    List<GameObject> stuffToHide;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.logoutButton.OnClickAsObservable()
            .Subscribe(this.Logout)
            .AddTo(this);
        this.stuffToHide.ForEach(x => x.SetActive(false));
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

    void Logout(Unit unit)
    {
        FirebaseService.Instance.Logout();
        SceneManager.LoadScene("__scenes/home/home");
    }
}
