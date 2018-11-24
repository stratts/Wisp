using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisp
{
    public enum AnimationProperty { PosX, PosY, Frame, Scale, Rotation, Opacity }

    public class AnimationGroup
    {
        Dictionary<string, Animation> animations =
            new Dictionary<string, Animation>();

        [JsonIgnore]
        public Animation CurrentAnimation { get; private set; } = null;
        public IReadOnlyDictionary<string, Animation> Animations => animations;

        public AnimationGroup() { }

        private void AddAnimation(Animation animation)
        {
            animations.Add(animation.Name, animation);
        }

        public Animation AddAnimation(string name)
        {
            var anim = new Animation(name);
            AddAnimation(anim);
            if (CurrentAnimation == null) SetAnimation(name);
            return anim;
        }

        public void SetAnimation(string name)
        {
            if (CurrentAnimation != null && name == CurrentAnimation.Name) return;
            CurrentAnimation = animations[name];
            CurrentAnimation.Reset();
        }
    }

    public class Animation
    {
        [JsonProperty]
        Dictionary<AnimationProperty, AnimationTrack> tracks =
            new Dictionary<AnimationProperty, AnimationTrack>();
        [JsonIgnore]
        float time;

        [JsonIgnore]
        public IEnumerable<AnimationTrack> Tracks { get { return tracks.Values; } }
        public String Name { get; set; }

        public Animation(string name)
        {
            Name = name;
        }

        private void AddTrack(AnimationTrack track)
        {
            tracks.Add(track.Property, track);
        }

        public AnimationTrack AddTrack(AnimationProperty property)
        {
            var track = new AnimationTrack(property);
            AddTrack(track);
            return track;
        }

        public void Update(float elapsedTime)
        {
            time += elapsedTime;
            foreach (var track in Tracks) track.SetCurrentValue(time);
        }

        public void Reset()
        {
            time = 0;
            Update(time);
        }

        [JsonIgnore]
        public bool Finished
        {
            get
            {
                foreach (var track in Tracks) if (!track.Finished) return false;
                return true;
            }
        }
    }

    public class AnimationTrack
    {
        List<AnimationFrame> frames = new List<AnimationFrame>();
        public EaseType Ease { get; set; } = EaseType.None;
        public AnimationProperty Property { get; private set; }
        public float Length { get; set; }
        public bool Loop { get; set; }
        [JsonIgnore]
        public float CurrentValue { get; private set; }
        [JsonIgnore]
        public bool Finished { get; private set; } = false;
        public IReadOnlyList<AnimationFrame> Frames => frames;

        public AnimationTrack() { }

        public AnimationTrack(AnimationProperty property)
        {
            Property = property;
            Ease = EaseType.None;
        }

        public AnimationTrack(AnimationProperty property, EaseType tweenType)
        {
            Property = property;
            Ease = tweenType;
        }

        public void AddFrame(float time, float value)
        {
            var frame = new AnimationFrame { Time = time, Value = value };
            frames.Add(frame);
            frames.Sort();
        }

        public void SetCurrentValue(float time)
        {
            CurrentValue = GetCurrentValue(time);
        }

        private float GetCurrentValue(float time)
        {
            if (frames.Count == 0) return 0;
            else if (frames.Count == 1) return frames[0].Value;

            if (time > Length)
            {
                Finished = true;
                if (Loop) return GetCurrentValue(time % Length);
                else return frames[frames.Count - 1].Value;
            }
            else Finished = false;

            AnimationFrame current = null;
            AnimationFrame next = null;

            for (int i = 1; i < frames.Count; i++)
            {
                var frame = frames[i];

                if (frame.Time >= time || i == frames.Count - 1)
                {
                    next = frame;
                    current = frames[i - 1];
                    break;
                }
            }

            return Interpolate(current, next, time);
        }

        private float Interpolate(AnimationFrame a, AnimationFrame b, float time)
        {
            if (time >= b.Time) return b.Value;
            if (Ease == EaseType.None) return a.Value;

            float valuePos = (time - a.Time) / (b.Time - a.Time);

            valuePos = Easings.Interpolate(valuePos, Ease);

            return a.Value + (b.Value - a.Value) * valuePos;
        }
    }

    public class AnimationFrame : IComparable<AnimationFrame>
    {
        public float Time { get; set; }
        public float Value { get; set; }

        public int CompareTo(AnimationFrame other)
        {
            return (int)(Time * 100 - other.Time * 100);
        }
    }
}