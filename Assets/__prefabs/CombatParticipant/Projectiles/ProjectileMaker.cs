using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class ProjectileMaker : MonoBehaviour
{
    [SerializeField]
    List<GameObject> templates;
    [SerializeField]
    Projectile projectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.templates.ForEach(x => x.SetActive(false));
        this.projectile.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        // Set the color to orange
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f); // Red = 1, Green = 0.5, Blue = 0, which is orange

        // Draw a sphere gizmo with a radius of 0.5 at the transform's position
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

    public Observable<int> CreateProjectile(string projectileName, CombatParticipant target)
    {
        // make copies
        var projectileToFire = GameObject.Instantiate(this.projectile, this.transform);
        var projectileTemplate = this.templates.FirstOrDefault(x => x.name == projectileName);
        var templateCopy = GameObject.Instantiate(projectileTemplate, projectileToFire.transform);

        // set parents
        return Observable.Defer(() =>
        {
            templateCopy.SetActive(true);
            projectileToFire.gameObject.SetActive(true);
            return projectileToFire.Fire(target, templateCopy);
        });
    }
}
