﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wisp.Nodes;
using Wisp.Components;

namespace Wisp
{
    public class TextTools
    {
        public static string WrapText(string text, SpriteFont font, int width)
        {
            var words = GetWords(text);
            var lines = new List<string>();

            string buffer = "";
            string result = "";

            foreach (var word in words)
            {
                int length = (int)font.MeasureString(buffer + word).X;

                if (length > width)
                {
                    result += buffer + '\n';
                    buffer = "";
                }
                if (word == "\n")
                {
                    result += buffer + '\n';
                    buffer = "";    
                }
                else buffer += word;
            }

            result += buffer + '\n';
            return result;
        }

        private static List<string> GetWords(string text)
        {
            var words = new List<string>();
            string buffer = "";

            foreach (var c in text)
            {
                if (c == ' ')
                {
                    words.Add(buffer + ' ');
                    buffer = "";
                }
                else if (c == '\n')
                {
                    words.Add(buffer);
                    words.Add("\n");
                    buffer = "";
                }
                else
                {
                    buffer += c;
                }
            }

            words.Add(buffer);

            return words;
        }
    }

    public class PosTools
    {
        public static bool WithinRange(Vector2 pos1, Vector2 pos2, float range)
        {
            return Vector2.DistanceSquared(pos1, pos2) < Math.Pow(range, 2);
        }

        public static Vector2 GetDirectionVector(Vector2 source, Vector2 target)
        {
            Vector2 dirVector = (target - source);
            dirVector.Normalize();

            return dirVector;
        }

        public static Direction GetDirection(Vector2 dirVector)
        {
            var absX = Math.Abs(dirVector.X);
            var absY = Math.Abs(dirVector.Y);

            if (dirVector.X < 0 && -absY > dirVector.X)
            {
                return Direction.Left;
            }

            if (dirVector.X > 0 && absY < dirVector.X)
            {
                return Direction.Right;
            }

            if (dirVector.Y < 0 && -absX > dirVector.Y)
            {
                return Direction.Up;
            }

            if (dirVector.Y > 0 && absX < dirVector.Y)
            {
                return Direction.Down;
            }

            return Direction.Left;
        }

        public static Direction GetDirection(Vector2 source, Vector2 target)
        {
            return GetDirection(GetDirectionVector(source, target));
        }
    }

    public static class NodeTools
    {
        public static Node FindClosestNode(Node node, IEnumerable<Node> targets, float maxDist)
        {
            Node closest = null;

            float maxDistSquared = (float)Math.Pow(maxDist, 2);
            float lowestDistSquared = -1;

            foreach (Node target in targets)
            {
                var distSquared = Vector2.DistanceSquared(node.CentrePos, target.CentrePos);

                if (distSquared < maxDistSquared)
                {
                    if (lowestDistSquared < 0 || distSquared < lowestDistSquared)
                    {
                        lowestDistSquared = distSquared;
                        closest = target;
                    }
                }
            }

            return closest;
        }

        public static IEnumerable<Node> FilterNodesByComponent<T>(IEnumerable<Node> nodes) where T : Component
        {
            foreach (var node in nodes)
            {
                if (node.HasComponent<T>()) yield return node;
            }
        }
    }

    public static class AnimTools
    {
        public static void FrameAnimation(AnimationGroup group, string name, int start, int end, float interval, bool loop = true)
        {
            var track = group.AddAnimation(name).AddTrack(AnimationProperty.Frame);
            track.Length = (end - start + 1) * interval;
            track.Ease = EaseType.Linear;
            track.Loop = loop;
            track.AddFrame(0, start);
            track.AddFrame((end - start) * interval, end);
        }

        public static void BasicAnimation(AnimationGroup group, string name, AnimationProperty property, float start, float end,
            float length, EaseType ease = EaseType.Linear, bool loop = false, bool pingPong = false)
        {
            var anim = group.AddAnimation(name);
            BasicTrack(anim, property, start, end, length, ease, loop, pingPong);
        }

        public static void BasicTrack(Animation animation, AnimationProperty property, float start, float end,
            float length, EaseType ease = EaseType.Linear, bool loop = false, bool pingPong = false)
        {
            var track = animation.AddTrack(property);
            track.Loop = loop;
            track.Ease = ease;
            track.AddFrame(0, start);
            track.AddFrame(length, end);

            if (pingPong)
            {
                track.AddFrame(length * 2, start);
                track.Length = length * 2;
            }
            else track.Length = length;
        }
    }
}
