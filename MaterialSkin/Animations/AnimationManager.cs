using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSkin.Animations
{
    class AnimationManager
    {
        public double Increment { get; set; }
        public AnimationType AnimationType { get; set; }

        public delegate void AnimationFinished(object sender);
        public event AnimationFinished OnAnimationFinished;

        public delegate void AnimationProgress(object sender);
        public event AnimationProgress OnAnimationProgress;

        private double progress;
        private AnimationDirection animationDirection;
        private const double MIN_VALUE = 0.00;
        private const double MAX_VALUE = 1.00;

        private readonly Timer animationTimer = new Timer { Interval = 5, Enabled = false };

        public AnimationManager()
        {
            Increment = 0.03;
            AnimationType = AnimationType.Linear;

            animationTimer.Tick += AnimationTimerOnTick;
        }

        private void AnimationTimerOnTick(object sender, EventArgs eventArgs)
        {
            UpdateProgress();

            if (OnAnimationProgress != null)
            {
                OnAnimationProgress(this);
            }
        }

        public bool IsAnimating()
        {
            return progress >= Increment;
        }

        public void StartNewAnimation(AnimationDirection animationDirection)
        {
            this.animationDirection = animationDirection;
            switch (this.animationDirection)
            {
                case AnimationDirection.In:
                    progress = MIN_VALUE;
                    break;
                case AnimationDirection.Out:
                    progress = MAX_VALUE;
                    break;
                default:
                    throw new Exception("Invalid AnimationDirection");
            }

            animationTimer.Start();
        }

        public void UpdateProgress()
        {
            switch (animationDirection)
            {
                case AnimationDirection.In:
                    IncrementProgress();
                    break;
                case AnimationDirection.Out:
                    DecrementProgress();
                    break;
                default:
                    throw new Exception("No AnimationDirection has been set");
            }
        }

        private void IncrementProgress()
        {
            progress += Increment;
            if (progress > MAX_VALUE)
            {
                progress = MAX_VALUE;
                animationTimer.Stop();
                if (OnAnimationFinished != null) OnAnimationFinished(this);
            }
        }

        private void DecrementProgress()
        {
            progress -= Increment;
            if (progress < MIN_VALUE)
            {
                progress = MIN_VALUE;
                animationTimer.Stop();
                if (OnAnimationFinished != null) OnAnimationFinished(this);
            }
        }

        public double GetProgress()
        {
            switch (AnimationType)
            {
                case AnimationType.Linear:
                    return AnimationLinear.CalculateProgress(progress);
                case AnimationType.EaseInOut:
                    return AnimationEaseInOut.CalculateProgress(progress);
            }
            
            throw new NotImplementedException("The given AnimationType is not implemented");
        }
    }
}
