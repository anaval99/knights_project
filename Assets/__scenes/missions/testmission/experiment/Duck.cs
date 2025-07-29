using UnityEngine;

[ExecuteAlways]
public class Duck : MonoBehaviour
{
    [SerializeField]
    GameObject duckfood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.duckfood)
        {
            this.transform.LookAt(duckfood.transform);
        }
    }
}
