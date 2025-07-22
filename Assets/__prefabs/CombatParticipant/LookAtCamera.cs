using R3;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject mainCamera;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            Observable.EveryUpdate()
                .Take(1)
                .Subscribe(_ => UpdateLook())
                .AddTo(this);
        }
    }

    void UpdateLook()
    {
       this.transform.rotation = this.mainCamera.transform.rotation;
    }
}
