using System.Collections.Generic;
using System.Windows;

namespace StateDiagramApp.Model
{
    public class State
    {
        public string Name { get; set; }
        public Point Position { get; set; }
        public double Radius { get; set; }
        public List<StateTransition> Transitions { get; set; }

        public State(string name, Point position)
        {
            Name = name;
            Position = position;
            Radius = 25;
            Transitions = new List<StateTransition>();
        }

        public State(){}
    }
}