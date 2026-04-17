using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;

namespace DefenderUI.Helpers;

/// <summary>
/// Reusable animation utilities built on top of the WinUI Composition API.
/// Provides entrance, emphasis and continuous animations that can be applied
/// to any <see cref="UIElement"/>.
/// </summary>
public static class AnimationHelper
{
    /// <summary>
    /// Fades an element in while sliding it up from 24px below.
    /// </summary>
    public static void AnimateEntrance(
        UIElement element,
        double delayMs = 0,
        double durationMs = 500,
        float offsetY = 24f)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        // Set starting state
        visual.Opacity = 0f;
        visual.Offset = new Vector3(0, offsetY, 0);

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.1f, 0.9f),
            new Vector2(0.2f, 1f));

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(0f, 0f);
        opacityAnim.InsertKeyFrame(1f, 1f, easing);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        opacityAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        opacityAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        var offsetAnim = compositor.CreateVector3KeyFrameAnimation();
        offsetAnim.InsertKeyFrame(0f, new Vector3(0, offsetY, 0));
        offsetAnim.InsertKeyFrame(1f, Vector3.Zero, easing);
        offsetAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        offsetAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        offsetAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation("Opacity", opacityAnim);
        visual.StartAnimation("Offset", offsetAnim);
    }

    /// <summary>
    /// Slides an element in from a given horizontal offset while fading in.
    /// </summary>
    public static void AnimateSlideInHorizontal(
        UIElement element,
        float offsetX = 40f,
        double delayMs = 0,
        double durationMs = 500)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        visual.Opacity = 0f;
        visual.Offset = new Vector3(offsetX, 0, 0);

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.1f, 0.9f),
            new Vector2(0.2f, 1f));

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(0f, 0f);
        opacityAnim.InsertKeyFrame(1f, 1f, easing);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        opacityAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        opacityAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        var offsetAnim = compositor.CreateVector3KeyFrameAnimation();
        offsetAnim.InsertKeyFrame(0f, new Vector3(offsetX, 0, 0));
        offsetAnim.InsertKeyFrame(1f, Vector3.Zero, easing);
        offsetAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        offsetAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        offsetAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation("Opacity", opacityAnim);
        visual.StartAnimation("Offset", offsetAnim);
    }

    /// <summary>
    /// Animates a set of elements in sequence, each with an incremental delay.
    /// </summary>
    public static void AnimateStaggered(
        IEnumerable<UIElement> elements,
        double staggerMs = 80,
        double initialDelayMs = 0,
        double durationMs = 500,
        float offsetY = 24f)
    {
        if (elements is null)
        {
            return;
        }

        double delay = initialDelayMs;
        foreach (var element in elements)
        {
            AnimateEntrance(element, delay, durationMs, offsetY);
            delay += staggerMs;
        }
    }

    /// <summary>
    /// Animates a set of elements in sequence with a horizontal slide.
    /// </summary>
    public static void AnimateStaggeredHorizontal(
        IEnumerable<UIElement> elements,
        double staggerMs = 90,
        double initialDelayMs = 0,
        double durationMs = 500,
        float offsetX = 40f)
    {
        if (elements is null)
        {
            return;
        }

        double delay = initialDelayMs;
        foreach (var element in elements)
        {
            AnimateSlideInHorizontal(element, offsetX, delay, durationMs);
            delay += staggerMs;
        }
    }

    /// <summary>
    /// Scales an element in from 0.85 to 1 while fading in.
    /// </summary>
    public static void AnimateScaleIn(
        UIElement element,
        double delayMs = 0,
        double durationMs = 500,
        float initialScale = 0.85f)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        CenterAnchor(element, visual);

        visual.Opacity = 0f;
        visual.Scale = new Vector3(initialScale, initialScale, 1f);

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.1f, 0.9f),
            new Vector2(0.2f, 1f));

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(0f, 0f);
        opacityAnim.InsertKeyFrame(1f, 1f, easing);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        opacityAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        opacityAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        var scaleAnim = compositor.CreateVector3KeyFrameAnimation();
        scaleAnim.InsertKeyFrame(0f, new Vector3(initialScale, initialScale, 1f));
        scaleAnim.InsertKeyFrame(1f, Vector3.One, easing);
        scaleAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        scaleAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        scaleAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation("Opacity", opacityAnim);
        visual.StartAnimation("Scale", scaleAnim);
    }

    /// <summary>
    /// Starts a continuous pulse (scale breathing) effect.
    /// </summary>
    public static void StartPulse(
        UIElement element,
        float minScale = 1.0f,
        float maxScale = 1.06f,
        double durationMs = 1600)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        CenterAnchor(element, visual);

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.4f, 0f),
            new Vector2(0.6f, 1f));

        var anim = compositor.CreateVector3KeyFrameAnimation();
        anim.InsertKeyFrame(0f, new Vector3(minScale, minScale, 1f));
        anim.InsertKeyFrame(0.5f, new Vector3(maxScale, maxScale, 1f), easing);
        anim.InsertKeyFrame(1f, new Vector3(minScale, minScale, 1f), easing);
        anim.Duration = TimeSpan.FromMilliseconds(durationMs);
        anim.IterationBehavior = AnimationIterationBehavior.Forever;

        visual.StartAnimation("Scale", anim);
    }

    /// <summary>
    /// Starts a continuous opacity pulse effect (nice for dots / indicators).
    /// </summary>
    public static void StartOpacityPulse(
        UIElement element,
        float minOpacity = 0.3f,
        float maxOpacity = 1.0f,
        double durationMs = 1800)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.4f, 0f),
            new Vector2(0.6f, 1f));

        var anim = compositor.CreateScalarKeyFrameAnimation();
        anim.InsertKeyFrame(0f, minOpacity);
        anim.InsertKeyFrame(0.5f, maxOpacity, easing);
        anim.InsertKeyFrame(1f, minOpacity, easing);
        anim.Duration = TimeSpan.FromMilliseconds(durationMs);
        anim.IterationBehavior = AnimationIterationBehavior.Forever;

        visual.StartAnimation("Opacity", anim);
    }

    /// <summary>
    /// Shakes an element horizontally - useful for error feedback.
    /// </summary>
    public static void Shake(UIElement element, float intensity = 8f, double durationMs = 500)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        var anim = compositor.CreateVector3KeyFrameAnimation();
        anim.InsertKeyFrame(0f, Vector3.Zero);
        anim.InsertKeyFrame(0.1f, new Vector3(-intensity, 0, 0));
        anim.InsertKeyFrame(0.3f, new Vector3(intensity, 0, 0));
        anim.InsertKeyFrame(0.5f, new Vector3(-intensity * 0.6f, 0, 0));
        anim.InsertKeyFrame(0.7f, new Vector3(intensity * 0.6f, 0, 0));
        anim.InsertKeyFrame(0.9f, new Vector3(-intensity * 0.3f, 0, 0));
        anim.InsertKeyFrame(1f, Vector3.Zero);
        anim.Duration = TimeSpan.FromMilliseconds(durationMs);

        visual.StartAnimation("Offset", anim);
    }

    /// <summary>
    /// Bounces an element into view (scale overshoots then settles).
    /// </summary>
    public static void AnimateBounce(UIElement element, double delayMs = 0, double durationMs = 700)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        CenterAnchor(element, visual);

        visual.Opacity = 0f;
        visual.Scale = new Vector3(0.3f, 0.3f, 1f);

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.34f, 1.56f),
            new Vector2(0.64f, 1f));

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(0f, 0f);
        opacityAnim.InsertKeyFrame(0.4f, 1f);
        opacityAnim.InsertKeyFrame(1f, 1f);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        opacityAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        opacityAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        var scaleAnim = compositor.CreateVector3KeyFrameAnimation();
        scaleAnim.InsertKeyFrame(0f, new Vector3(0.3f, 0.3f, 1f));
        scaleAnim.InsertKeyFrame(1f, Vector3.One, easing);
        scaleAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        scaleAnim.DelayTime = TimeSpan.FromMilliseconds(delayMs);
        scaleAnim.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation("Opacity", opacityAnim);
        visual.StartAnimation("Scale", scaleAnim);
    }

    /// <summary>
    /// Starts a continuous rotation on the element.
    /// </summary>
    public static void StartRotation(UIElement element, double durationSec = 2.0)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        CenterAnchor(element, visual);

        var anim = compositor.CreateScalarKeyFrameAnimation();
        anim.InsertKeyFrame(0f, 0f);
        anim.InsertKeyFrame(1f, 360f);
        anim.Duration = TimeSpan.FromSeconds(durationSec);
        anim.IterationBehavior = AnimationIterationBehavior.Forever;

        visual.StartAnimation("RotationAngleInDegrees", anim);
    }

    /// <summary>
    /// Stops a running animation on a given property.
    /// </summary>
    public static void StopAnimation(UIElement element, string propertyName)
    {
        if (element is null || string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        visual.StopAnimation(propertyName);
    }

    /// <summary>
    /// Flashes a container's background with an accent color briefly, and
    /// adds a short shake for emphasis. Great for threat-detected feedback.
    /// </summary>
    public static void FlashAccentColor(FrameworkElement element, Color color, double durationMs = 600)
    {
        if (element is null)
        {
            return;
        }

        Brush? originalBrush = GetBackground(element);
        var flashBrush = new SolidColorBrush(color);
        ApplyBackground(element, flashBrush);

        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(durationMs / 2),
        };
        int tick = 0;
        timer.Tick += (_, _) =>
        {
            tick++;
            if (tick == 1)
            {
                var fadedColor = Color.FromArgb((byte)(color.A / 2), color.R, color.G, color.B);
                ApplyBackground(element, new SolidColorBrush(fadedColor));
            }
            else
            {
                ApplyBackground(element, originalBrush);
                timer.Stop();
            }
        };
        timer.Start();

        Shake(element, intensity: 4f, durationMs: durationMs);
    }

    private static Brush? GetBackground(FrameworkElement element)
    {
        return element switch
        {
            Border b => b.Background,
            Panel p => p.Background,
            Control c => c.Background,
            _ => null,
        };
    }

    private static void ApplyBackground(FrameworkElement element, Brush? brush)
    {
        switch (element)
        {
            case Border b:
                b.Background = brush;
                break;
            case Panel p:
                p.Background = brush;
                break;
            case Control c:
                c.Background = brush;
                break;
        }
    }

    /// <summary>
    /// Starts a continuous opacity shimmer (0.65 -> 1.0 -> 0.65) intended to
    /// be applied to a ProgressBar or its overlay while work is in progress.
    /// </summary>
    public static void AnimateProgressShimmer(FrameworkElement element, double durationMs = 1400)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.4f, 0f),
            new Vector2(0.6f, 1f));

        var opacityAnim = compositor.CreateScalarKeyFrameAnimation();
        opacityAnim.InsertKeyFrame(0f, 0.65f);
        opacityAnim.InsertKeyFrame(0.5f, 1.0f, easing);
        opacityAnim.InsertKeyFrame(1f, 0.65f, easing);
        opacityAnim.Duration = TimeSpan.FromMilliseconds(durationMs);
        opacityAnim.IterationBehavior = AnimationIterationBehavior.Forever;

        visual.StartAnimation("Opacity", opacityAnim);
    }

    /// <summary>
    /// Smoothly animates a <see cref="ProgressBar"/> from 0 to the target
    /// value over the specified duration using an ease-out cubic tween.
    /// </summary>
    public static void AnimateProgressBar(
        ProgressBar bar,
        double targetValue,
        double delayMs = 0,
        double durationMs = 800)
    {
        if (bar is null)
        {
            return;
        }

        bar.Value = 0;

        void StartTween()
        {
            var startTime = DateTime.UtcNow;
            var tweenTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16),
            };
            tweenTimer.Tick += (_, _) =>
            {
                var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                var progress = Math.Clamp(elapsed / durationMs, 0, 1);
                var eased = 1 - Math.Pow(1 - progress, 3);
                bar.Value = targetValue * eased;

                if (progress >= 1)
                {
                    bar.Value = targetValue;
                    tweenTimer.Stop();
                }
            };
            tweenTimer.Start();
        }

        if (delayMs > 0)
        {
            var delayTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(delayMs),
            };
            delayTimer.Tick += (_, _) =>
            {
                delayTimer.Stop();
                StartTween();
            };
            delayTimer.Start();
        }
        else
        {
            StartTween();
        }
    }

    /// <summary>
    /// Performs a short scale pulse ("check" effect) for toggle feedback.
    /// </summary>
    public static void AnimateCheckPulse(FrameworkElement element, double durationMs = 380)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        CenterAnchor(element, visual);

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.34f, 1.56f),
            new Vector2(0.64f, 1f));

        var anim = compositor.CreateVector3KeyFrameAnimation();
        anim.InsertKeyFrame(0f, Vector3.One);
        anim.InsertKeyFrame(0.5f, new Vector3(1.04f, 1.04f, 1f), easing);
        anim.InsertKeyFrame(1f, Vector3.One, easing);
        anim.Duration = TimeSpan.FromMilliseconds(durationMs);

        visual.StartAnimation("Scale", anim);
    }

    /// <summary>
    /// Fades an element's opacity from one value to another.
    /// </summary>
    public static void FadeOpacity(
        UIElement element,
        float from,
        float to,
        double durationMs = 200)
    {
        if (element is null)
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;

        var easing = compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.1f, 0.9f),
            new Vector2(0.2f, 1f));

        var anim = compositor.CreateScalarKeyFrameAnimation();
        anim.InsertKeyFrame(0f, from);
        anim.InsertKeyFrame(1f, to, easing);
        anim.Duration = TimeSpan.FromMilliseconds(durationMs);

        visual.StartAnimation("Opacity", anim);
    }

    /// <summary>
    /// Anchors the composition transform origin to the element's center so
    /// scale and rotation animations rotate around the middle.
    /// </summary>
    private static void CenterAnchor(UIElement element, Visual visual)
    {
        if (element is FrameworkElement fe)
        {
            if (fe.ActualWidth > 0 && fe.ActualHeight > 0)
            {
                visual.CenterPoint = new Vector3(
                    (float)(fe.ActualWidth / 2),
                    (float)(fe.ActualHeight / 2),
                    0f);
            }

            fe.SizeChanged += (_, args) =>
            {
                visual.CenterPoint = new Vector3(
                    (float)(args.NewSize.Width / 2),
                    (float)(args.NewSize.Height / 2),
                    0f);
            };
        }
    }
}