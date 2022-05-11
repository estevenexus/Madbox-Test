using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public float LifeTime;

    public int AttackPower { get; set; }

    void Update()
    {
        if (LifeTime > 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * Speed);
            LifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
