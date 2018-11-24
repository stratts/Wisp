using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Wisp
{
    public interface IKeyService {
        bool IsHeld(VirtualKeys key);
        bool IsPressed(VirtualKeys key);
        float GetHeldTime(VirtualKeys key);
    }

    public enum VirtualKeys { ZoomIn, ZoomOut, Close, MoveUp, MoveDown, MoveLeft, MoveRight, Delete, Escape, Confirm, Hub}

    public class VirtualKeyManager : IKeyService
    {
        private Dictionary<VirtualKeys, VirtualKey> keys;

        public VirtualKeyManager()
        {
            keys = new Dictionary<VirtualKeys, VirtualKey>();
        }

        public bool IsPressed(VirtualKeys key)
        {
            return keys[key].Pressed;
        }

        public bool IsHeld(VirtualKeys key)
        {
            return keys[key].Held;
        }

        public float GetHeldTime(VirtualKeys key)
        {
            return keys[key].HeldTime;
        }

        public VirtualKey AddKey(VirtualKeys key, Keys[] trigger = null)
        {
            var vKey = new VirtualKey();
            vKey.AddTrigger(trigger);
            keys.Add(key, vKey);

            return vKey;
        }

        public VirtualKey GetKey(VirtualKeys key)
        {
            return keys[key];
        }

        public void Update(KeyboardState kstate, float elapsedTime)
        {
            foreach (var key in keys.Values)
            {
                key.SetKeyboardState(kstate, elapsedTime);
            }
        }
    }

    public class VirtualKey
    {
        List<Keys[]> triggers = new List<Keys[]>();
        KeyboardState prevstate, kstate;
        float elapsed;
        private float heldtime; 
        public float HeldTime
        {
            get
            {
                if (Held) heldtime += elapsed;
                else heldtime = 0;

                return heldtime;
            }
        }

        public bool Held
        {
            get {
                return (KeysDown(kstate) && KeysDown(prevstate));
            }
        }
        public bool Pressed
        {
            get
            {
                return (KeysDown(kstate) && !KeysDown(prevstate));
            }
        }

        public void AddTrigger(Keys[] keys)
        {
            if (keys != null) triggers.Add(keys);
        }

        public void SetKeyboardState(KeyboardState currentState, float elapsedTime)
        {
            prevstate = kstate;
            kstate = currentState;
            elapsed = elapsedTime;
        }

        private bool KeysDown(KeyboardState kstate)
        {
            foreach (Keys[] keys in triggers)
            {
                foreach (Keys key in keys)
                {
                    if (kstate.IsKeyUp(key)) return false;
                }
            }

            return true;
        }
    }
}
