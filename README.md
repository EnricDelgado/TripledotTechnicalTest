# Technical Dot Tecnical Test
## Overview
### Summary 
I haven't been able to use all of the provided time, but I managed to implement a UI system that is widely scalable with plenty of reusable components

To cut on time consumption, multiple plugins have been added to this project - they will be listed down below. The most notable of mention are:
    - LeanTween: my tweening library of choice due to it being lightweight and quite versatile
    - Cysharp: awaitable alternative to Coroutines
    - ParticleEffectForUGUI: plugin that allows for particles to be displayed over canvas when they're set to ScreenSpace - Overlay

### Plugins Used
- [EpicToon FX](https://assetstore.unity.com/packages/vfx/particles/epic-toon-fx-57772)
- [LeanTween](https://assetstore.unity.com/packages/tools/animation/leantween-3595)
- [Lowpoly Handpainted House](https://assetstore.unity.com/packages/3d/props/exterior/lowpoly-handpainted-house-286891)
- [Cute Supermarket Lite](https://assetstore.unity.com/packages/3d/props/food/cute-supermarket-lite-302941)
- [CySharp](https://github.com/Cysharp/UniTask)
- [ParticleEffectForUGUI](https://github.com/mob-sakai/ParticleEffectForUGUI)
- [Froggy Travel Pack](https://sketchfab.com/3d-models/froggy-travel-pack-739561a9936349b3a9c02ec81cc032b0)

## Features
### Tween Clips
Instead of reusing tween chunks of code, a reusable ScriptableObject-based solution has been provided. This allows the user to generate common clip movements that can be reusable amongst objects

### Tab System
A tab system recreating the provided GIF has been created. It uses the TweenClips feature, as well as a prefab structure that allows for scalability

### Modal System
A reusable/easily iterable modal system has been added. The prefab structure allows for general wide changes, as well as for niche changes. Source files for the asset generations have also been provided; gradient maps have been used for quickly generating multiple color choices

### Localisation System
Texts have been localised in two languages: English and Spanish. It follows a simple key driven approach using a .JSON file that can be found at `Assets/Resources`

## Known Issues
### Popup System
- When quickly pressing between locked tabs, the error popup doesn't translate correctly. Better use of the cancellation token is required

## What would I've done if I had more time
### Cleaner Code
Due to having to rush through the Technical Test, the code has been deteriorating and generating big code smells, specially when it comes to violating the Single Responsiblity principle. With more time, I'd stick to cleaner code

### Implement Currency Additions
- Add buttons to increase currencies
- Add Attractor effect to the desired currency (Particle System Force Field)

### Keep my system strict
I spend quite a lot of time generating the reusable and scalable TweenClip system, but quickly discarding it for being on a rush - I started generating not-so carefully crafted code instead of sticking to the good architecture.

### Add more elements to modals
Right now, modals simply show; I'd rather have them do more actions instead of just exemplifying their reusability

### Add sound feedback
There is no sound system implemented, so there is a big part of the feedback missing. I'd like to add a simple sound system, and back it with options in the modal