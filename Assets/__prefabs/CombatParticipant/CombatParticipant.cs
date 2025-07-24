using R3;
using UnityEngine;
using UnityEngine.UI;

public class CombatParticipant : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public int MaxHealth = 100;
    [SerializeField]
    public int CurrentHealth = 100;
    [SerializeField]
    public int Defense = 10;
    [SerializeField]
    public int Damage = 20;
    [SerializeField]
    public int AP = 0;

    [SerializeField]
    public Slider HealthSlider;
    [SerializeField]
    Image FillColor;
    [SerializeField]
    TMPro.TextMeshProUGUI HealthNumber;

    private CombatController combatController;
    public BehaviorSubject<bool> MyTurnStartObs = new BehaviorSubject<bool>(false);

    void Start()
    {
        this.combatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
        this.combatController.CombatStateObs
            .Where(state => state != null)
            .Select(state => state.Phase == CombatPhase.TurnStart && state.ShuffledParticipants[state.TurnIndex] == this)
            .Subscribe(isMyTurn => this.MyTurnStartObs.OnNext(isMyTurn))
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (HealthSlider != null)
        {
            HealthSlider.value = (float)CurrentHealth / MaxHealth;
        }
        // 23, 149, 149 by default, yellow if less than 66%, red if less than 33%
        Color color = new Color(0.090f, 0.584f, 0.584f);
        if (HealthSlider.value < 0.33f)
        {
            color = Color.red;
        }
        else if (HealthSlider.value < 0.66f)
        {
            color = Color.yellow;
        }

        if (FillColor != null)
        {
            FillColor.color = color;
        }

        if (HealthNumber != null)
        {
            HealthNumber.text = $"{CurrentHealth}/{MaxHealth}";
            HealthNumber.color = color;
        }  
    }
}
