using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI skillDescription;

    public void SetText(string text)
    {
        skillDescription.text = text;
    }
}
