using UnityEngine;

public class Hero : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attacking,
        Moving,
        TakingDamage,
        Dying
    }

    public int HealthPoints;
    public HealthBar HealthBar;
    public Transform WeaponParent;
    public InputManager InputManager;
    public Animator Animator;

    public State CurrentState { get; private set; }

    private Weapon EquipedWeapon;

    public void EquipWeapon(Weapon weapon)
    {
        EquipedWeapon = weapon;
        Animator.SetFloat("AttackSpeed", EquipedWeapon.AttackSpeed);
    }

    public void OnTakeDamageEnd()
    {
        ChangeState(State.Idle, "Stop");
    }

    public void EnableAttack(int enable)
    {
        EquipedWeapon.Collider.enabled = enable != 0;
    }

    public void OnAttackEnded()
    {
        ChangeState(State.Idle, "Stop");
    }

    public void OnDieEnd()
    {
        GameManager.Instance.GameOver();
    }

    private void Start()
    {
        CurrentState = State.Idle;
        HealthBar.SetMaxHealth(HealthPoints);
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

    void Update()
    {
        switch (CurrentState)
        {
            case State.Idle:
                CheckForMovement();
                CheckForAttack();
                break;
            case State.Attacking:
                CheckForMovement();
                break;
            case State.Moving:
                CheckForMovement();
                CheckForStop();
                break;
            case State.TakingDamage:
                break;
            case State.Dying:
                break;
            default:
                break;
        }
    }

    private void CheckForStop()
    {
        if (InputManager.Direction.x == 0 && InputManager.Direction.y == 0)
        {
            ChangeState(State.Idle, "Stop");
        }
    }

    private void CheckForAttack()
    {
        if (CurrentState == State.Attacking)
        {
            return;
        }
        
        if (GameManager.Instance.Enemies.Count > 0)
        {
            transform.LookAt(GameManager.Instance.ClosestEnemyPosition(transform.position));
            ChangeState(State.Attacking, "Attack");
        }
    }

    private void CheckForMovement()
    {
        if (InputManager.Direction.x != 0 || InputManager.Direction.y != 0)
        {
            var direction = new Vector3(-InputManager.Direction.x, 0, -InputManager.Direction.y);
            transform.LookAt(transform.position + direction);
            transform.Translate(Vector3.forward * Time.deltaTime * EquipedWeapon.MovementSpeed);
            ChangeState(State.Moving, "Move");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile" && CanTakeDamage())
        {
            var projectile = other.gameObject.GetComponent<Projectile>();
            HealthPoints -= projectile.AttackPower;
            HealthBar.UpdateHealth(HealthPoints);
            Destroy(other);
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
        return HealthPoints > 0 && CurrentState != State.TakingDamage;
    }
}
