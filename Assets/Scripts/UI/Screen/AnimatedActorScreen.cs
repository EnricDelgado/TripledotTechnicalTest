using System;
using UnityEngine;  

public class AnimatedActorScreen : BaseScreen
{
    [Header("Animated Actor Screen Elements")] 
    [SerializeField] private ScreenActor _actor;
    
    
    public override void EnterScreen()
    {
        base.EnterScreen();
        _actor.ShowActor();
    }

    public override void ExitScreen()
    {
        base.ExitScreen();
        _actor.HideActor();
    }
}
