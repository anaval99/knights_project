using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeScreen : MonoBehaviour
{
    [SerializeField]
    GameObject TapToStartForm;
    [SerializeField]
    UnityEngine.UI.Button BtnTapToStart;
    [SerializeField]
    GameObject LoginForm;
    [SerializeField]
    TMPro.TMP_InputField EmailField;
    [SerializeField]
    TMPro.TMP_InputField PasswordField;
    [SerializeField]
    UnityEngine.UI.Button BtnLogin;
    [SerializeField]
    Alerts alerts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.TapToStartForm.SetActive(true);
        this.LoginForm.SetActive(false);
        System.Console.WriteLine("Welcome to the game!");
        this.BtnTapToStart.onClick.AddListener(OnTapToStart);
        this.BtnLogin.onClick.AddListener(OnClickLogin);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        this.BtnTapToStart.onClick.RemoveListener(OnTapToStart);
        this.BtnLogin.onClick.RemoveListener(OnClickLogin);
    }

    void OnTapToStart()
    {
        Debug.Log("Tap to start the game!");
        this.TapToStartForm.SetActive(false);
        this.LoginForm.SetActive(true);
    }

    async void OnClickLogin()
    {
        string email = this.EmailField.text;
        string password = this.PasswordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            this.alerts.Error("Email and password cannot be empty.");
            return;
        }
        await this.ExecuteLogin(email, password);
    }

    async Task ExecuteLogin(string email, string password)
    {
        this.BtnLogin.interactable = false; // Disable the button to prevent multiple clicks
        try
        {
            await FirebaseService.Instance.Login(email, password);
            Debug.Log("Login successful!");
            // Get avatar
            var avatar = await FirebaseService.Instance.GetSingle<CharAvatar>(FirebasePaths.Avatars);
            if (avatar == null)
            {
                // Proceed to the next screen or game state
                SceneManager.LoadScene("__scenes/character_creation/character_creation");
            }
            else
            {
                // Load the main game scene
                SceneManager.LoadScene("__scenes/dashboard/dashboard");
            }
        }
        catch (Exception ex)
        {
            this.alerts.Error($"Login failed: {ex.Message}");
            this.BtnLogin.interactable = true; // Re-enable the button for retry
        }
    }
}
