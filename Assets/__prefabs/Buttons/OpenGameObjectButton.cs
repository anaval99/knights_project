using System.Collections.Generic;
using R3;
using UnityEngine;

[System.Serializable]
public class ButtonGameObjectPair
{
    [SerializeField]
    public UnityEngine.UI.Button Button;
    [SerializeField]
    public GameObject TargetObject;
}

public class OpenGameObjectButton : MonoBehaviour
{
    [SerializeField]
    public ButtonGameObjectPair[] openButtons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var pair in openButtons)
        {
            pair.Button.OnClickAsObservable()
                .Subscribe(_ => pair.TargetObject.SetActive(true))
                .AddTo(this);
        }
    }
}
