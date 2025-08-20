using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ScreenActor : MonoBehaviour
{
    [Header("Actor Elements")]
    [SerializeField] private GameObject _actor;
    [SerializeField] private ParticleSystem _actorVFX;

    [Header("Animation Timings")]
    [SerializeField] private float _actorRotationSpeed = .12f;
    [SerializeField] private float _bounceDuration = .18f;
    [SerializeField] private float _overshootScale = 1.2f;
    [SerializeField] private LeanTweenType _bounceEase = LeanTweenType.linear;

    private int _tweenID = -1;
    private Quaternion _actorInitialRotation;
    private Vector3 _actorInitialScale;


    private void Awake()
    {
        _actorInitialRotation =  _actor.transform.rotation;
        _actorInitialScale = _actor.transform.localScale;
    }

    public void ShowActor()
    {
        _actor.SetActive(true);
        BounceElement(_actor);
        RotateElement(_actor);
        PlayPS(_actorVFX).Forget();
    }

    public void HideActor()
    {
        _actor.SetActive(false);

        if (_tweenID >= 0)
        {
            LeanTween.cancel(_tweenID);
            _actor.transform.rotation = _actorInitialRotation;
        }
    }
    
    private void RotateElement(GameObject element)
    {
        _tweenID = LeanTween.rotateAround(element, Vector3.up, 360f, _actorRotationSpeed).setLoopClamp().id;
    }

    private void BounceElement(GameObject element)
    {
        float overshootRange = _overshootScale * 0.33f; 
        float oversoot = Random.Range(_overshootScale -= overshootRange, _overshootScale += overshootRange);
        
        LeanTween.value(gameObject, 0f, 1f, _bounceDuration)
            .setEase(_bounceEase)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float t) =>
            {
                float squash = Mathf.Sin(t * Mathf.PI);
                float stretch = Mathf.Cos(t * Mathf.PI);

                float scaleX = Mathf.Lerp(_actorInitialScale.x, _actorInitialScale.x * oversoot, squash);
                float scaleY = Mathf.Lerp(_actorInitialScale.y, _actorInitialScale.y / oversoot, stretch);

                element.transform.localScale = new Vector3(scaleX, scaleY, _actorInitialScale.z);
            })
            .setOnComplete(() =>
            {
                element.transform.localScale = _actorInitialScale;
            });
    }

    private async UniTask PlayPS(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
    
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();

        await UniTask.WaitUntil(() => !ps.IsAlive());
        
        ps.Stop();
        ps.gameObject.SetActive(false);
    }
}