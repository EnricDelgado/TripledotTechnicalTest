using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TweenedElement : MonoBehaviour
{
    private Dictionary<TweenChannel, int> _tweens = new ();
    
    private void OnDisable() => CancelAllChannels();

    public virtual async UniTask PlayClip(TweenClip clip, CancellationToken ct)
    {
        await UniTask.Yield();
    }
    
    public virtual async UniTask PlayClip(CanvasTweenClip clip, CancellationToken ct)
    {
        await UniTask.Yield();
    }

    protected void FillChannel(TweenChannel channel, int id) => _tweens[channel] = id;
    
    protected void ClearChannel(TweenChannel channel) => _tweens[channel] = -1;

    protected void CancelChannel(TweenChannel channel)
    {
        if (_tweens.TryGetValue(channel, out var id) && id >= 0)
        {
            LeanTween.cancel(id);
            ClearChannel(channel);
        }
    }

    protected void CancelAllChannels()
    {
        foreach (var element in _tweens)
        {
            if(element.Value >= 0) LeanTween.cancel(element.Value);
        }
        _tweens.Clear();
    }

    protected void RegisterChannelCancellationToken(CancellationToken token, TweenChannel channel, int id, UniTaskCompletionSource completionSource)
    {
        token.Register(() =>
        {
            if (_tweens.TryGetValue(channel, out var currentID) && id == currentID)
            {
                LeanTween.cancel(currentID);
                ClearChannel(channel);
                completionSource.TrySetCanceled();
            }
        });
    }
}
