using Cysharp.Threading.Tasks;
using UnityEngine;

public class GrowingIcon : MonoBehaviour
{
    [SerializeField] private RectTransform _foregroundIcon;
    [SerializeField] private ParticleSystem _endParticles;
    [SerializeField] private float _growthDuration;
    [SerializeField] private LeanTweenType _growthEaseType;

    private Vector2 _foregroundIconInitialScale;

    public async UniTask GrowIcon()
    {
        await ScaleIcon();
        PlayPS(_endParticles).Forget();
    }
    
    private async UniTask ScaleIcon()
    {
        var completionSource = new UniTaskCompletionSource();

        LeanTween.value(gameObject, 0, 1, _growthDuration)
            .setEase(_growthEaseType)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float val) =>
            {
                _foregroundIcon.localScale = Vector3.one * val;
            })
            .setOnComplete(() => completionSource.TrySetResult());
        
        await completionSource.Task;
    }

    private async UniTask PlayPS(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        particle.Play();
        
        await UniTask.WaitUntil(() => !particle.IsAlive());
        
        particle.Stop();
        particle.gameObject.SetActive(false);
    }

    public void ResetIcon()
    {
        _foregroundIcon.localScale = _foregroundIconInitialScale;
    }
}
