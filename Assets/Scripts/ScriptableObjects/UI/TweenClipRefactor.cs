/*using UnityEngine;

public enum FromMode { Current, Value }
public enum ToModePosition { Absolute, ByOffset }
public enum ToModeScale    { Absolute, MultiplyBy, ByOffset }
public enum EffectAxis{ X, Y }

public interface ITweenClip { }

public class TweenClip : ScriptableObject, ITweenClip { }

[CreateAssetMenu(menuName = "UI/Tween Clip")]
public class TweenIconClip : ScriptableObject
{
    [Header("Scale Clip")]
    public bool useScale = true;
    public bool separateAxis = false;
    public EffectAxis effectAxis = EffectAxis.X;
    public FromMode scaleFrom = FromMode.Current;
    public ToModeScale scaleToMode = ToModeScale.Absolute;
    [Tooltip("Use this scale with the \"Absolute\" Scale Mode")]
    public Vector3 scaleTo = Vector3.one * 1.2f;
    [Tooltip("Use this scale with the \"By Offset\" Scale Mode")]
    public Vector3 scaleDelta = Vector3.zero;
    [Tooltip("Use this scale with the \"Multiply By\" Scale Mode")]
    public float   scaleMultiply = 1.0f;
    public float   scaleDuration = 0.18f;
    public LeanTweenType scaleEase = LeanTweenType.easeInOutCubic;

    [Header("Move (Local) Clip")]
    public bool useMove = true;
    public FromMode moveFrom = FromMode.Current;
    public ToModePosition moveToMode = ToModePosition.ByOffset;
    [Tooltip("Use this scale with the \"Absolute\" Mode Mode")]
    public Vector3 moveTo = Vector3.zero;
    [Tooltip("Use this scale with the \"By Offset\" Move Mode")]
    public Vector3 moveOffset = new Vector3(0f, 10f, 0f);
    public float   moveDuration = 0.18f;
    public LeanTweenType moveEase = LeanTweenType.easeInOutCubic;

    [Header("Alpha Clip")]
    public bool useAlpha = false;
    public FromMode alphaFrom = FromMode.Current;
    [Tooltip("Use this scale with the \"Value\" Alpha From")]
    public float initalAlpha = 1f;
    public float alphaTo = 1f;
    public float alphaDuration = 0.18f;
    public LeanTweenType alphaEase = LeanTweenType.easeInOutCubic;
}

[CreateAssetMenu(menuName = "UI/Tween Canvas Group Clip")]
public class TweenCanvasGroupClip : TweenClip
{
    [Header("Canvas Group Visibility")] 
    public bool useCanvasGroupVisibility = false;
}

[CreateAssetMenu(menuName = "UI/Tween Scale Clip")]
public class TweenScaleClip : TweenClip
{
    [Header("Scale Clip")]
    public bool useScale = true;
    public bool separateAxis = false;
    public EffectAxis effectAxis = EffectAxis.X;
    public FromMode scaleFrom = FromMode.Current;
    public ToModeScale scaleToMode = ToModeScale.Absolute;
    [Tooltip("Use this scale with the \"Absolute\" Scale Mode")]
    public Vector3 scaleTo = Vector3.one * 1.2f;
    [Tooltip("Use this scale with the \"By Offset\" Scale Mode")]
    public Vector3 scaleDelta = Vector3.zero;
    [Tooltip("Use this scale with the \"Multiply By\" Scale Mode")]
    public float   scaleMultiply = 1.0f;
    public float   scaleDuration = 0.18f;
    public LeanTweenType scaleEase = LeanTweenType.easeInOutCubic;
}

[CreateAssetMenu(menuName = "UI/Tween Position Clip")]
public class TweenPositionClip : TweenClip
{
    [Header("Tween Position Clip")]
    public bool useMove = true;
    public FromMode moveFrom = FromMode.Current;
    public ToModePosition moveToMode = ToModePosition.ByOffset;
    [Tooltip("Use this scale with the \"Absolute\" Mode Mode")]
    public Vector3 moveTo = Vector3.zero;
    [Tooltip("Use this scale with the \"By Offset\" Move Mode")]
    public Vector3 moveOffset = new Vector3(0f, 10f, 0f);
    public float   moveDuration = 0.18f;
    public LeanTweenType moveEase = LeanTweenType.easeInOutCubic;
}

[CreateAssetMenu(menuName = "UI/Tween Alpha Clip")]
public class TweenAlphaClip : TweenClip
{
    [Header("Alpha Clip")]
    public bool useAlpha = false;
    public FromMode alphaFrom = FromMode.Current;
    [Tooltip("Use this scale with the \"Value\" Alpha From")]
    public float initalAlpha = 1f;
    public float alphaTo = 1f;
    public float alphaDuration = 0.18f;
    public LeanTweenType alphaEase = LeanTweenType.easeInOutCubic;
}

[CreateAssetMenu(menuName = "UI/Tween Shake Clip")]
public class TweenShakeClip : TweenClip
{
    [Header("Shake Clip")] 
    public bool useShake = false;
    public float shakeDuration = 0.18f;
    public float shakeMagnitude = 8f;
    public float shakeFrequency = 25f;
    public LeanTweenType shakeEase = LeanTweenType.linear;
}*/