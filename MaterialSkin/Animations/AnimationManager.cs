using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSkin.Animations
{
    class AnimationManager
    {
        public bool InterruptAnimation { get; set; }
        public double Increment { get; set; }
        public AnimationType AnimationType { get; set; }
        public bool Singular { get; set; }

        public delegate void AnimationFinished(object sender);
        public event AnimationFinished OnAnimationFinished;

        public delegate void AnimationProgress(object sender);
        public event AnimationProgress OnAnimationProgress;

        private List<double> animationProgresses;
        private List<Point> animationSources;
        private List<AnimationDirection> animationDirections;
        private const double MIN_VALUE = 0.00;
        private const double MAX_VALUE = 1.00;

        private readonly Timer animationTimer = new Timer { Interval = 5, Enabled = false };

        public AnimationManager(bool singular = true)
        {
            animationProgresses = new List<double>();
            animationSources = new List<Point>();
            animationDirections = new List<AnimationDirection>();

            Increment = 0.03;
            AnimationType = AnimationType.Linear;
            InterruptAnimation = true;
            Singular = singular;

            if (Singular)
            {
                animationProgresses.Add(0);
                animationSources.Add(new Point(0,0));
                animationDirections.Add(AnimationDirection.Out);
            }

            animationTimer.Tick += AnimationTimerOnTick;
        }

        private void AnimationTimerOnTick(object sender, EventArgs eventArgs)
        {
            for (int i = 0; i < animationProgresses.Count; i++)
            {
                UpdateProgress(i);

                if (!Singular)
                {
                    if ((animationDirections[i] == AnimationDirection.In && animationProgresses[i] == MAX_VALUE) ||
                        (animationDirections[i] == AnimationDirection.Out && animationProgresses[i] == MIN_VALUE))
                    {
                        animationProgresses.RemoveAt(i);
                        animationSources.RemoveAt(i);
                        animationDirections.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (OnAnimationProgress != null)
            {
                OnAnimationProgress(this);
            }
        }

        public bool IsAnimating()
        {
            return animationTimer.Enabled;
        }

        public void StartNewAnimation(AnimationDirection animationDirection)
        {
            StartNewAnimation(animationDirection, new Point(0,0));
        }

        public void StartNewAnimation(AnimationDirection animationDirection, Point animationSource)
        {
            if (!IsAnimating() || InterruptAnimation)
            {
                if (Singular && animationDirections.Count > 0)
                {
                    this.animationDirections[0] = animationDirection;
                }
                else
                {
                    this.animationDirections.Add(animationDirection);
                }

                if (Singular && animationSources.Count > 0)
                {
                    this.animationSources[0] = animationSource;
                }
                else
                {
                    this.animationSources.Add(animationSource);
                }

                if (!(Singular && animationProgresses.Count > 0))
                {
                    switch (this.animationDirections[this.animationDirections.Count - 1])
                    {
                        case AnimationDirection.In:
                            animationProgresses.Add(MIN_VALUE);
                            break;
                        case AnimationDirection.Out:
                            animationProgresses.Add(MAX_VALUE);
                            break;
                        default:
                            throw new Exception("Invalid AnimationDirection");
                    }
                }
            }

            animationTimer.Start();
        }

        public void UpdateProgress(int index)
        {
            switch (animationDirections[index])
            {
                case AnimationDirection.In:
                    IncrementProgress(index);
                    break;
                case AnimationDirection.Out:
                    DecrementProgress(index);
                    break;
                default:
                    throw new Exception("No AnimationDirection has been set");
            }
        }

        private void IncrementProgress(int index)
        {
            animationProgresses[index] += Increment;
            if (animationProgresses[index] > MAX_VALUE)
            {
                animationProgresses[index] = MAX_VALUE;

                for (int i = 0; i < GetRippleCount(); i++)
                {
                    if (animationDirections[i] == AnimationDirection.In && animationProgresses[i] != MAX_VALUE) return;
                }

                animationTimer.Stop();
                if (OnAnimationFinished != null) OnAnimationFinished(this);
            }
        }

        private void DecrementProgress(int index)
        {
            animationProgresses[index] -= Increment;
            if (animationProgresses[index] < MIN_VALUE)
            {
                animationProgresses[index] = MIN_VALUE;

                for (int i = 0; i < GetRippleCount(); i++)
                {
                    if (animationDirections[i] == AnimationDirection.Out && animationProgresses[i] != MIN_VALUE) return;
                }

                animationTimer.Stop();
                if (OnAnimationFinished != null) OnAnimationFinished(this);
            }
        }

        public double GetProgress()
        {
            if(!Singular)
                throw new Exception("Animation is not set to Singular.");

            if (animationProgresses.Count == 0)
                throw new Exception("Invalid animation");

            return GetProgress(0);
        }

        public double GetProgress(int index)
        {
            if (!(index < GetRippleCount()))
                throw new IndexOutOfRangeException("Invalid animation index");

            switch (AnimationType)
            {
                case AnimationType.Linear:
                    return AnimationLinear.CalculateProgress(animationProgresses[index]);
                case AnimationType.EaseInOut:
                    return AnimationEaseInOut.CalculateProgress(animationProgresses[index]);
            }

            throw new NotImplementedException("The given AnimationType is not implemented");
        }

        public Point GetSource(int index)
        {
            if (!(index < GetRippleCount()))
                throw new IndexOutOfRangeException("Invalid animation index");

            return animationSources[index];
        }

        public Point GetSource()
        {
            if (!Singular)
                throw new Exception("Animation is not set to Singular.");

            if (animationSources.Count == 0)
                throw new Exception("Invalid animation");

            return animationSources[0];
        }

        public int GetRippleCount()
        {
            return animationProgresses.Count;
        }

        public void SetProgress(int progress)
        {
            if (!Singular)
                throw new Exception("Animation is not set to Singular.");

            if (animationProgresses.Count == 0)
                throw new Exception("Invalid animation");

            animationProgresses[0] = progress;
        }
    }
}
