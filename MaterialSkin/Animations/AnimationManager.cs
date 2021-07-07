namespace MaterialSkin.Animations
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="AnimationManager" />
    /// </summary>
    internal class AnimationManager
    {
        /// <summary>
        /// Gets or sets a value indicating whether InterruptAnimation
        /// </summary>
        public bool InterruptAnimation { get; set; }

        /// <summary>
        /// Gets or sets the Increment
        /// </summary>
        public double Increment { get; set; }

        /// <summary>
        /// Gets or sets the SecondaryIncrement
        /// </summary>
        public double SecondaryIncrement { get; set; }

        /// <summary>
        /// Gets or sets the AnimationType
        /// </summary>
        public AnimationType AnimationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Singular
        /// </summary>
        public bool Singular { get; set; }

        /// <summary>
        /// The AnimationFinished
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        public delegate void AnimationFinished(object sender);

        /// <summary>
        /// Defines the OnAnimationFinished
        /// </summary>
        public event AnimationFinished OnAnimationFinished;

        /// <summary>
        /// The AnimationProgress
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        public delegate void AnimationProgress(object sender);

        /// <summary>
        /// Defines the OnAnimationProgress
        /// </summary>
        public event AnimationProgress OnAnimationProgress;

        /// <summary>
        /// Defines the _animationProgresses
        /// </summary>
        private readonly List<double> _animationProgresses;

        /// <summary>
        /// Defines the _animationSources
        /// </summary>
        private readonly List<Point> _animationSources;

        /// <summary>
        /// Defines the _animationDirections
        /// </summary>
        private readonly List<AnimationDirection> _animationDirections;

        /// <summary>
        /// Defines the _animationDatas
        /// </summary>
        private readonly List<object[]> _animationDatas;

        /// <summary>
        /// Defines the MIN_VALUE
        /// </summary>
        private const double MIN_VALUE = 0.00;

        /// <summary>
        /// Defines the MAX_VALUE
        /// </summary>
        private const double MAX_VALUE = 1.00;

        /// <summary>
        /// Defines the _animationTimer
        /// </summary>
        private readonly Timer _animationTimer = new Timer { Interval = 5, Enabled = false };

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationManager"/> class.
        /// </summary>
        /// <param name="singular">If true, only one animation is supported. The current animation will be replaced with the new one. If false, a new animation is added to the list.</param>
        public AnimationManager(bool singular = true)
        {
            _animationProgresses = new List<double>();
            _animationSources = new List<Point>();
            _animationDirections = new List<AnimationDirection>();
            _animationDatas = new List<object[]>();

            Increment = 0.03;
            SecondaryIncrement = 0.03;
            AnimationType = AnimationType.Linear;
            InterruptAnimation = true;
            Singular = singular;

            if (Singular)
            {
                _animationProgresses.Add(0);
                _animationSources.Add(new Point(0, 0));
                _animationDirections.Add(AnimationDirection.In);
            }

            _animationTimer.Tick += AnimationTimerOnTick;
        }

        /// <summary>
        /// The AnimationTimerOnTick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="eventArgs">The eventArgs<see cref="EventArgs"/></param>
        private void AnimationTimerOnTick(object sender, EventArgs eventArgs)
        {
            for (var i = 0; i < _animationProgresses.Count; i++)
            {
                UpdateProgress(i);

                if (!Singular)
                {
                    if ((_animationDirections[i] == AnimationDirection.InOutIn && _animationProgresses[i] == MAX_VALUE))
                    {
                        _animationDirections[i] = AnimationDirection.InOutOut;
                    }
                    else if ((_animationDirections[i] == AnimationDirection.InOutRepeatingIn && _animationProgresses[i] == MIN_VALUE))
                    {
                        _animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                    }
                    else if ((_animationDirections[i] == AnimationDirection.InOutRepeatingOut && _animationProgresses[i] == MIN_VALUE))
                    {
                        _animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                    }
                    else if (
                        (_animationDirections[i] == AnimationDirection.In && _animationProgresses[i] == MAX_VALUE) ||
                        (_animationDirections[i] == AnimationDirection.Out && _animationProgresses[i] == MIN_VALUE) ||
                        (_animationDirections[i] == AnimationDirection.InOutOut && _animationProgresses[i] == MIN_VALUE))
                    {
                        _animationProgresses.RemoveAt(i);
                        _animationSources.RemoveAt(i);
                        _animationDirections.RemoveAt(i);
                        _animationDatas.RemoveAt(i);
                    }
                }
                else
                {
                    if ((_animationDirections[i] == AnimationDirection.InOutIn && _animationProgresses[i] == MAX_VALUE))
                    {
                        _animationDirections[i] = AnimationDirection.InOutOut;
                    }
                    else if ((_animationDirections[i] == AnimationDirection.InOutRepeatingIn && _animationProgresses[i] == MAX_VALUE))
                    {
                        _animationDirections[i] = AnimationDirection.InOutRepeatingOut;
                    }
                    else if ((_animationDirections[i] == AnimationDirection.InOutRepeatingOut && _animationProgresses[i] == MIN_VALUE))
                    {
                        _animationDirections[i] = AnimationDirection.InOutRepeatingIn;
                    }
                }
            }

            OnAnimationProgress?.Invoke(this);
        }

        /// <summary>
        /// The IsAnimating
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool IsAnimating()
        {
            return _animationTimer.Enabled;
        }

        /// <summary>
        /// The StartNewAnimation
        /// </summary>
        /// <param name="animationDirection">The animationDirection<see cref="AnimationDirection"/></param>
        /// <param name="data">The data<see cref="object[]"/></param>
        public void StartNewAnimation(AnimationDirection animationDirection, object[] data = null)
        {
            StartNewAnimation(animationDirection, new Point(0, 0), data);
        }

        /// <summary>
        /// The StartNewAnimation
        /// </summary>
        /// <param name="animationDirection">The animationDirection<see cref="AnimationDirection"/></param>
        /// <param name="animationSource">The animationSource<see cref="Point"/></param>
        /// <param name="data">The data<see cref="object[]"/></param>
        public void StartNewAnimation(AnimationDirection animationDirection, Point animationSource, object[] data = null)
        {
            if (!IsAnimating() || InterruptAnimation)
            {
                if (Singular && _animationDirections.Count > 0)
                {
                    _animationDirections[0] = animationDirection;
                }
                else
                {
                    _animationDirections.Add(animationDirection);
                }

                if (Singular && _animationSources.Count > 0)
                {
                    _animationSources[0] = animationSource;
                }
                else
                {
                    _animationSources.Add(animationSource);
                }

                if (!(Singular && _animationProgresses.Count > 0))
                {
                    switch (_animationDirections[_animationDirections.Count - 1])
                    {
                        case AnimationDirection.InOutRepeatingIn:
                        case AnimationDirection.InOutIn:
                        case AnimationDirection.In:
                            _animationProgresses.Add(MIN_VALUE);
                            break;

                        case AnimationDirection.InOutRepeatingOut:
                        case AnimationDirection.InOutOut:
                        case AnimationDirection.Out:
                            _animationProgresses.Add(MAX_VALUE);
                            break;

                        default:
                            throw new Exception("Invalid AnimationDirection");
                    }
                }

                if (Singular && _animationDatas.Count > 0)
                {
                    _animationDatas[0] = data ?? new object[] { };
                }
                else
                {
                    _animationDatas.Add(data ?? new object[] { });
                }
            }

            _animationTimer.Start();
        }

        /// <summary>
        /// The UpdateProgress
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        public void UpdateProgress(int index)
        {
            switch (_animationDirections[index])
            {
                case AnimationDirection.InOutRepeatingIn:
                case AnimationDirection.InOutIn:
                case AnimationDirection.In:
                    IncrementProgress(index);
                    break;

                case AnimationDirection.InOutRepeatingOut:
                case AnimationDirection.InOutOut:
                case AnimationDirection.Out:
                    DecrementProgress(index);
                    break;

                default:
                    throw new Exception("No AnimationDirection has been set");
            }
        }

        /// <summary>
        /// The IncrementProgress
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        private void IncrementProgress(int index)
        {
            _animationProgresses[index] += Increment;
            if (_animationProgresses[index] > MAX_VALUE)
            {
                _animationProgresses[index] = MAX_VALUE;

                for (int i = 0; i < GetAnimationCount(); i++)
                {
                    if (_animationDirections[i] == AnimationDirection.InOutIn)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.InOutRepeatingIn)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.InOutRepeatingOut)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.InOutOut && _animationProgresses[i] != MAX_VALUE)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.In && _animationProgresses[i] != MAX_VALUE)
                    {
                        return;
                    }
                }

                _animationTimer.Stop();
                OnAnimationFinished?.Invoke(this);
            }
        }

        /// <summary>
        /// The DecrementProgress
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        private void DecrementProgress(int index)
        {
            _animationProgresses[index] -= (_animationDirections[index] == AnimationDirection.InOutOut || _animationDirections[index] == AnimationDirection.InOutRepeatingOut) ? SecondaryIncrement : Increment;
            if (_animationProgresses[index] < MIN_VALUE)
            {
                _animationProgresses[index] = MIN_VALUE;

                for (var i = 0; i < GetAnimationCount(); i++)
                {
                    if (_animationDirections[i] == AnimationDirection.InOutIn)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.InOutRepeatingIn)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.InOutRepeatingOut)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.InOutOut && _animationProgresses[i] != MIN_VALUE)
                    {
                        return;
                    }

                    if (_animationDirections[i] == AnimationDirection.Out && _animationProgresses[i] != MIN_VALUE)
                    {
                        return;
                    }
                }

                _animationTimer.Stop();
                OnAnimationFinished?.Invoke(this);
            }
        }

        /// <summary>
        /// The GetProgress
        /// </summary>
        /// <returns>The <see cref="double"/></returns>
        public double GetProgress()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationProgresses.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return GetProgress(0);
        }

        /// <summary>
        /// The GetProgress
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="double"/></returns>
        public double GetProgress(int index)
        {
            if (!(index < GetAnimationCount()))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            switch (AnimationType)
            {
                case AnimationType.Linear:
                    return AnimationLinear.CalculateProgress(_animationProgresses[index]);

                case AnimationType.EaseInOut:
                    return AnimationEaseInOut.CalculateProgress(_animationProgresses[index]);

                case AnimationType.EaseOut:
                    return AnimationEaseOut.CalculateProgress(_animationProgresses[index]);

                case AnimationType.CustomQuadratic:
                    return AnimationCustomQuadratic.CalculateProgress(_animationProgresses[index]);

                default:
                    throw new NotImplementedException("The given AnimationType is not implemented");
            }
        }

        /// <summary>
        /// The GetSource
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="Point"/></returns>
        public Point GetSource(int index)
        {
            if (!(index < GetAnimationCount()))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            return _animationSources[index];
        }

        /// <summary>
        /// The GetSource
        /// </summary>
        /// <returns>The <see cref="Point"/></returns>
        public Point GetSource()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationSources.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return _animationSources[0];
        }

        /// <summary>
        /// The GetDirection
        /// </summary>
        /// <returns>The <see cref="AnimationDirection"/></returns>
        public AnimationDirection GetDirection()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationDirections.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return _animationDirections[0];
        }

        /// <summary>
        /// The GetDirection
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="AnimationDirection"/></returns>
        public AnimationDirection GetDirection(int index)
        {
            if (!(index < _animationDirections.Count))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            return _animationDirections[index];
        }

        /// <summary>
        /// The GetData
        /// </summary>
        /// <returns>The <see cref="object[]"/></returns>
        public object[] GetData()
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationDatas.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            return _animationDatas[0];
        }

        /// <summary>
        /// The GetData
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="object[]"/></returns>
        public object[] GetData(int index)
        {
            if (!(index < _animationDatas.Count))
            {
                throw new IndexOutOfRangeException("Invalid animation index");
            }

            return _animationDatas[index];
        }

        /// <summary>
        /// The GetAnimationCount
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int GetAnimationCount()
        {
            return _animationProgresses.Count;
        }

        /// <summary>
        /// The SetProgress
        /// </summary>
        /// <param name="progress">The progress<see cref="double"/></param>
        public void SetProgress(double progress)
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationProgresses.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            _animationProgresses[0] = progress;
        }

        /// <summary>
        /// The SetDirection
        /// </summary>
        /// <param name="direction">The direction<see cref="AnimationDirection"/></param>
        public void SetDirection(AnimationDirection direction)
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationProgresses.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            _animationDirections[0] = direction;
        }

        /// <summary>
        /// The SetData
        /// </summary>
        /// <param name="data">The data<see cref="object[]"/></param>
        public void SetData(object[] data)
        {
            if (!Singular)
            {
                throw new Exception("Animation is not set to Singular.");
            }

            if (_animationDatas.Count == 0)
            {
                throw new Exception("Invalid animation");
            }

            _animationDatas[0] = data;
        }
    }
}