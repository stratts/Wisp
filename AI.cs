using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Components
{
    public class AI : Component
    {
        public IStateMachine stateMachine;

        public override void Update(Scene scene)
        {
            stateMachine.ChooseState(Parent, scene);
            stateMachine.Run(Parent, scene);
            base.Update(scene);
        }
    }
}

namespace Wisp
{

    public interface ILogic
    {
        void Run(Node node, Scene scene);
    }

    public interface IStateMachine {
        void ChooseState(Node node, Scene scene);
        void Run(Node node, Scene scene);
    }

    public abstract class AIStateMachine<TEnum> : IStateMachine
    {
        public TEnum currentState { get; protected set; }
        public ILogic BackgroundLogic = null;
        ILogic currentStateLogic;

        Dictionary<TEnum, ILogic> states;

        public AIStateMachine()
        {
            states = new Dictionary<TEnum, ILogic>();
        }

        public void AddState(TEnum state, ILogic logic)
        {
            states.Add(state, logic);
        }

        public void RemoveState(TEnum state)
        {
            states.Remove(state);
        }

        public void EditState(TEnum state, ILogic logic)
        {
            states[state] = logic;
        }

        public void SetState(TEnum state)
        {
            try
            {
                currentState = state;
                currentStateLogic = states[state];
            }
            catch (KeyNotFoundException e)
            {
                throw new ArgumentException(
                    "Given state has not been added to the state machine.", e);
            }

        }

        public virtual void ChooseState(Node node, Scene scene)
        {

        }

        public virtual void Run(Node node, Scene scene)
        {
            if (BackgroundLogic != null) BackgroundLogic.Run(node, scene);
        
            currentStateLogic.Run(node, scene);
        }
    }
}
