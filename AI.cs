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
        public AIStateMachine stateMachine;

        public void Run(Node node, Scene scene)
        {
            stateMachine.ChooseState(node, scene);
            stateMachine.Run(node, scene);
        }
    }
}

namespace Wisp
{

    public interface ILogic
    {
        void Run(Node node, Scene scene);
    }

    public enum AIState { Idle, Chase, Jump, Test };

    public abstract class AIStateMachine
    {
        public AIState currentState { get; protected set; }
        public ILogic BackgroundLogic = null;
        ILogic currentStateLogic;

        Dictionary<AIState, ILogic> states;

        public AIStateMachine()
        {
            states = new Dictionary<AIState, ILogic>();
        }

        public void AddState(AIState state, ILogic logic)
        {
            states.Add(state, logic);
        }

        public void RemoveState(AIState state)
        {
            states.Remove(state);
        }

        public void EditState(AIState state, ILogic logic)
        {
            states[state] = logic;
        }

        public void SetState(AIState state)
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
