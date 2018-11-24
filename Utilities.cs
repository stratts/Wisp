using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wisp.Nodes;

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
        public static Vector2 GetDirectionVector(Vector2 source, Vector2 target)
        {
            Vector2 dirVector = (target - source);
            dirVector.Normalize();

            return dirVector;
        }

        public static int GetDirection(Vector2 dirVector)
        {
            var absX = Math.Abs(dirVector.X);
            var absY = Math.Abs(dirVector.Y);

            if (dirVector.X < 0 && -absY > dirVector.X)
            {
                return (int)Direction.Left;
            }

            if (dirVector.X > 0 && absY < dirVector.X)
            {
                return (int)Direction.Right;
            }

            if (dirVector.Y < 0 && -absX > dirVector.Y)
            {
                return (int)Direction.Up;
            }

            if (dirVector.Y > 0 && absX < dirVector.Y)
            {
                return (int)Direction.Down;
            }

            return (int)Direction.Left;
        }
    }
}
