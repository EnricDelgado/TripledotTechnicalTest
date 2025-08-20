using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasePopup : MonoBehaviour
{
    //[Header("Popup Base Elements")]
    //[SerializeField] protected CanvasGroup _canvasGroup;

    [Header("Popup Base Timings")]
    [SerializeField] private float _popupScreenTime = 1f;

    [Header("Popup Placement")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _popupRect;
    [SerializeField] private float _yOffset = 24f;
    
    bool _showing = false;

    
    // This is for triggering the popup through UnityEvent
    public void LaunchPopupEvent() => EnterPopup().Forget();

    protected virtual async UniTask EnterPopup()
    {
        if (_showing) return;
        
        _showing = true;
        PlacePopupAtLastPointer();
        ShowPopup();
        await UniTask.Delay(TimeSpan.FromSeconds(_popupScreenTime));
        HidePopup();
        _showing = false;
    }

    protected virtual void ShowPopup() { }
    protected virtual void HidePopup() { }
    
    protected virtual void PlacePopupAtLastPointer()
    {
        Vector2 screenPos = GetLastPointerScreenPosition();
        
        var pivotX = screenPos.x / Screen.width;
        var pivotY = screenPos.y / Screen.height;
        
        _popupRect.pivot = new Vector2(pivotX, pivotY);
        
        PlacePopup(screenPos);
    }
    
    protected virtual void PlacePopup(Vector2 screenPosition)
    {
        var canvasRect = _canvas.transform as RectTransform;
        var uiCam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, uiCam, out var localPoint);

        localPoint += new Vector2(0f, _yOffset);

        _popupRect.anchoredPosition = localPoint;
    }
    
    private Vector2 GetLastPointerScreenPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;

        return Input.mousePosition;
    }

    protected UniTask FadeElement(GameObject element, float from, float to, float duration, LeanTweenType ease, Action<float> updateCallback)
    {
        var completionSource = new UniTaskCompletionSource();

        int actionID = LeanTween.value(gameObject, from, to, duration)
            .setEase(ease)
            .setIgnoreTimeScale(true)
            .setOnUpdate(updateCallback)
            .setOnComplete(() => completionSource.TrySetResult())
            .id;

        return completionSource.Task;
    }
}
