using UnityEngine;

public class Alerts : MonoBehaviour
{
    [SerializeField]
    float alertDuration = 2f;
    [SerializeField]
    GameObject errrorPrefab;
    [SerializeField]
    TMPro.TextMeshProUGUI errorText;

    private float alertTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.errrorPrefab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.alertTimer > 0f)
        {
            this.alertTimer -= Time.deltaTime;
            if (this.alertTimer <= 0f)
            {
                this.errrorPrefab.gameObject.SetActive(false);
                this.alertTimer = 0f;
            }
        }
    }

    void Show(string message, GameObject prefab, TMPro.TextMeshProUGUI text)
    {
        prefab.SetActive(true);
        text.text = message;
        this.alertTimer = this.alertDuration;
    }
    
    public void Error(string message)
    {
        Show(message, this.errrorPrefab, this.errorText);
    }
}
