using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionBoard : MonoBehaviour
{
    [SerializeField]
    public Button TestMissionButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.TestMissionButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                SceneManager.LoadScene("__scenes/missions/testmission/testmission");
            })
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
