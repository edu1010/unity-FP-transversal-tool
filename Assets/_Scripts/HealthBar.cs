using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider _slider;
    public Gradient ColorGradient;
    public Image FillImage;

    // Start is called before the first frame update
    
    private void OnEnable()
    {
        HealthSystem.OnPlayerRecieveDamage += SetValue;
    }

    private void OnDisable()
    {
        HealthSystem.OnPlayerRecieveDamage -= SetValue;

    }
    void Awake()
    {
        _slider = GetComponent<Slider>();
        
    }

    public void SetValue(float f)
    {
        _slider.value = f;
        FillImage.color = ColorGradient.Evaluate(f);
    }
}
