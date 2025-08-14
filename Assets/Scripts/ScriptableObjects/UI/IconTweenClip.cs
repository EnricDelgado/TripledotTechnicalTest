using UnityEngine;

public enum FromMode { Current, Initial }

public enum ToModePosition { Absolute, ByOffset }
public enum ToModeScale    { Absolute, MultiplyBy, ByOffset }
public enum ToModeAlpha    { Absolute, ByOffset }

[CreateAssetMenu(menuName = "UI/Icon Tween Clip")]
public class IconTweenClip : ScriptableObject
{
    [Header("Scale Track")]
    public bool useScale = true;
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

    [Header("Move (Local) Track")]
    public bool useMove = true;
    public FromMode moveFrom = FromMode.Current;
    public ToModePosition moveToMode = ToModePosition.ByOffset;
    [Tooltip("Use this scale with the \"Absolute\" Mode Mode")]
    public Vector3 moveTo = Vector3.zero;
    [Tooltip("Use this scale with the \"By Offset\" Move Mode")]
    public Vector3 moveOffset = new Vector3(0f, 10f, 0f);
    public float   moveDuration = 0.18f;
    public LeanTweenType moveEase = LeanTweenType.easeInOutCubic;

    [Header("Alpha Track")]
    public bool useAlpha = false;
    public FromMode alphaFrom = FromMode.Current;
    public ToModeAlpha alphaToMode = ToModeAlpha.Absolute;

    [Tooltip("Use this scale with the \"Absolute\" Alpha Mode")]
    public float alphaTo = 1f;
    [Tooltip("Use this scale with the \"By Offset\" Alpha Mode")]
    public float alphaDelta = 0f;
    public float alphaDuration = 0.18f;
    public LeanTweenType alphaEase = LeanTweenType.easeInOutCubic;
}