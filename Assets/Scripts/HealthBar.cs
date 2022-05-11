using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Canvas Canvas;
    public Slider Slider;
    public Gradient Gradient;
    public Image Health;

    private Transform MainCamera;

    public void SetMaxHealth(int maxHealth)
    {
        Slider.maxValue = maxHealth;
        Slider.value = maxHealth;

        Health.color = Gradient.Evaluate(1f);
    }

    public void UpdateHealth(int newHealth)
    {
        Slider.value = newHealth;

        Health.color = Gradient.Evaluate(Slider.normalizedValue);
    }

    private void Start()
    {
        MainCamera = GameManager.Instance.MainCamera.transform;
    }

    private void LateUpdate()
    {
        if (Canvas.renderMode == RenderMode.WorldSpace)
        {
            transform.LookAt(transform.position + MainCamera.forward);
        }
    }
}
