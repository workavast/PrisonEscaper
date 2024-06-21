using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_SomeBar
{
    private readonly MonoBehaviour _parentBehaviour;
    private readonly Slider _slider;
    private bool _coroutineIsActive;
    private float _targetValue;
    private Coroutine _coroutine;

    public UI_SomeBar(MonoBehaviour parentBehaviour, Slider slider, float startValue)
    {
        _parentBehaviour = parentBehaviour;
        _slider = slider;
        _slider.value = startValue;
    }
    
    public void SetValue(float value)
    {
        if (_coroutine != null)
        {
            _parentBehaviour.StopCoroutine(_coroutine);
            _coroutineIsActive = false;
        }
        _slider.value = value;
        _targetValue = value;
    }
    
    public void SetTargetValue(float targetValue)
    {
        _targetValue = targetValue;
        if (!_coroutineIsActive)
            _coroutine = _parentBehaviour.StartCoroutine(ChangeBarValue());
    }

    public void OnParentDisabled()
    {
        if (_coroutine != null)
        {
            _slider.value = _targetValue;
            _parentBehaviour.StopCoroutine(_coroutine);
            _coroutineIsActive = false;
        }
    }
    
    private IEnumerator ChangeBarValue()
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
