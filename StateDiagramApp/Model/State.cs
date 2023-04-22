using System.Collections.Generic;
using System.Windows;

namespace StateDiagramApp.Model
{
    public class State
    {
        public static uint idCounter { get; set; }
        public uint IDNo;
        public string Name { get; set; }
        public Point Position { get; set; }
        public double Radius { get; set; }
        public List<StateTransition> Transitions { get; set; }

        public State(string name, Point position)
        {
            IDNo = idCounter++;
            Name = name;
            Position = position;
            Radius = 25;
            Transitions = new List<StateTransition>();
        }

        public State(){}
    }
}