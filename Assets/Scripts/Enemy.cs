using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attacking,
        TakingDamage,
        Dying
    }

    public int HealthPoints;
    public int AttackPower;
    public HealthBar HealthBar;
    public GameObject Projectile;
    public Animator Animator;

    private State CurrentState;
    private Hero Player;
    private Vector3 PlayerOffsetFromGround;

    public void Initinitialize(Hero player, float difficulty)
    {
        Player = player;
        HealthPoints = (int)(HealthPoints * difficulty);
        AttackPower = (int)(AttackPower * difficulty);
        PlayerOffsetFromGround = new Vector3(0, 0.3f, 0);
        HealthBar.SetMaxHealth(HealthPoints);
    }

    public void OnTakeDamageEnd()
    {
        ChangeState(State.Idle, "Stop");
    }

    public void OnAttackEnded()
    {
        ChangeState(State.Idle, "Stop");
    }

    public void OnDieEnd()
    {
        GameManager.Instance.RemoveEnemy(GetInstanceID);
        Destroy(transform.parent.gameObject);
    }

    private void ResetAllTriggers()
    {
        foreach (var param in Animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                Animator.ResetTrigger(param.name);
            }
        }
    }

    private void ChangeState(State newState, string newStateTrigger)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            ResetAllTriggers();
            Animator.SetTrigger(newStateTrigger);
        }
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Idle:
                CheckForAttack();
                break;
            case State.Attacking:
                break;
            case State.TakingDamage:
                break;
            case State.Dying:
                break;
            default:
                break;
        }
    }

    private void CheckForAttack()
    {
        if (CurrentState == State.Attacking)
        {
            return;
        }

        transform.LookAt(Player.transform.position + PlayerOffsetFromGround);
        var projectile = Instantiate(Projectile, transform).GetComponent<Projectile>();
        projectile.transform.parent = null;
        projectile.AttackPower = AttackPower;
        ChangeState(State.Attacking, "Attack");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon" && CanTakeDamage())
        {
            HealthPoints -= other.gameObject.GetComponent<Weapon>().AttackPower;
            transform.LookAt(Player.transform.position + PlayerOffsetFromGround);
            HealthBar.UpdateHealth(HealthPoints);
            if (HealthPoints <= 0)
            {
                ChangeState(State.Dying, "Die");
            }
            else
            {
                ChangeState(State.TakingDamage, "TakeDamage");
            }
        }
    }

    private bool CanTakeDamage()
    {
        return HealthPoints > 0 && Player.CurrentState == Hero.State.Attacking && CurrentState != State.TakingDamage;
    }
}
