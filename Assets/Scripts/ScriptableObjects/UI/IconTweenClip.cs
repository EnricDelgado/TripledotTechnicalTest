using UnityEngine;

[CreateAssetMenu(menuName = "UI/Icon Tween Clip")]
public class IconAnimClip : ScriptableObject
{
    [Header("Scale")]
    public bool useScale = true;
    public Vector3 scaleFrom = Vector3.one;
    public Vector3 scaleTo   = Vector3.one * 1.2f;
    public float scaleDuration = 0.18f;
    public LeanTweenType scaleEase = LeanTweenType.easeInOutCubic;

    [Header("Move (local)")]
    public bool useMove = true;
    public Vector3 moveFrom = Vector3.zero;
    public Vector3 moveTo   = new Vector3(0f, 10f, 0f);
    public float moveDuration = 0.18f;
    public LeanTweenType moveEase = LeanTweenType.easeInOutCubic;

    [Header("Alpha")]
    public bool useAlpha = false;
    public float alphaFrom = 0f;
    public float alphaTo   = 1f;
    public float alphaDuration = 0.18f;
    public LeanTweenType alphaEase = LeanTweenType.easeInOutCubic;
}