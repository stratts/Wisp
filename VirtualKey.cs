using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Wisp
{
    public static class KeyManager 
    {
        static IKeyManager manager = null;

        public static VirtualKeyManager<TEnum> Get<TEnum>()
        {
            if (manager == null) 
                manager = new VirtualKeyManager<TEnum>();
            return (VirtualKeyManager<TEnum>)manager;
        }

        public static void Update(KeyboardState kstate, float elapsedTime) 
        {
            if (manager == null) return;
            manager.Update(kstate, elapsedTime);
        }
    }

    interface IKeyManager 
    {
        void Update(KeyboardState kstate, float elapsedTime);
    }

    public class VirtualKeyManager<TEnum> : IKeyManager
    {
        private Dictionary<TEnum, VirtualKey> keys;

        public VirtualKeyManager()
        {
            keys = new Dictionary<TEnum, VirtualKey>();
        }

        public bool IsPressed(TEnum key)
        {
            keys.TryGetValue(key, out var vKey);
            return vKey != null && vKey.Pressed;
        }

        public bool IsHeld(TEnum key)
        {
            keys.TryGetValue(key, out var vKey);
            return vKey != null && vKey.Held;
        }

        public float GetHeldTime(TEnum key)
        {
            keys.TryGetValue(key, out var vKey);
            return vKey == null ? 0 : vKey.HeldTime;
        }

        public VirtualKey AddKey(TEnum key, Keys[] trigger = null)
        {
            var vKey = new VirtualKey();
            vKey.AddTrigger(trigger);
            keys.Add(key, vKey);

            return vKey;
        }

        public void AddKeys(IEnumerable<(TEnum key, Keys[] trigger)> keys) 
        {
            foreach (var key in keys) AddKey(key.key, key.trigger);
        }

        public void AddKeys(IEnumerable<(TEnum key, Keys trigger)> keys) {
            foreach (var key in keys) AddKey(key.key, new[] { key.trigger });
        }

        public VirtualKey GetKey(TEnum key)
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
