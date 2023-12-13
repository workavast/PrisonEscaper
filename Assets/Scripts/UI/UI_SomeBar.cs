using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_SomeBar
{
    private MonoBehaviour _parentBehaviour;
    private readonly Slider _slider;
    private bool _coroutineIsActive;
    private float _targetValue;

    public UI_SomeBar(MonoBehaviour parentBehaviour, Slider slider, float startValue)
    {
        _parentBehaviour = parentBehaviour;
        _slider = slider;
        _slider.value = startValue;
    }
    
    public void SetValue(float value)
    {
        _slider.value = value;
        _targetValue = value;
    }
    
    public void SetTargetValue(float targetValue)
    {
        _targetValue = targetValue;
        if (!_coroutineIsActive)
            _parentBehaviour.StartCoroutine(ChangeHealthBarValue());
    }
    
    private IEnumerator ChangeHealthBarValue()
    {
        _coroutineIsActive = true;
    
        while (Mathf.Abs(_slider.value - _targetValue) > 0.001f)
        {
            _slider.value = Mathf.Lerp(_slider.value,_targetValue, 2.5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    
        _slider.value = _targetValue;
        _coroutineIsActive = false;
    }
}
