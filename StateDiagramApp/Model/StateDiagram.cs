using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StateDiagramApp.Model
{
    internal class StateDiagram
    {
        public List<State> States { get; set; }

        public StateDiagram()
        {
            States = new List<State>();
        }

        public bool AddTransition(State fromState, State toState)
        {
            if (!fromState.Transitions.Any(t => t.ToState == toState))
            {
                fromState.Transitions.Add(new StateTransition(toState));
                return true;
            }

            return false;
        }

        public void DeleteState(State state)
        {
            foreach (State s in States)
            {
                s.Transitions.RemoveAll(t => t.ToState == state);
            }

            States.Remove(state);
        }

        public void DeleteTransition(State fromState, State toState)
        {
            fromState.Transitions.RemoveAll(t => t.ToState == toState);
        }

        public void MoveState(State state, Point newPosition)
        {
            state.Position = newPosition;
        }
    }
}