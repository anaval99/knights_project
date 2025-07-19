using R3;
using UnityEngine;
using UnityEngine.UI;

public class FriendActionsUI : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    TMPro.TextMeshProUGUI friendNameText;
    [SerializeField]
    Button party1Button;
    [SerializeField]
    Button party2Button;

    public BehaviorSubject<CharAvatar> SelectedFriendObs = new(null);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        this.SelectedFriendObs
            .Subscribe(avatar =>
            {
                this.friendNameText.text = string.Empty;
                this.friendNameText.text = avatar.CharacterName;
            })
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        this.SelectedFriendObs.OnNext(null);
    }
}
