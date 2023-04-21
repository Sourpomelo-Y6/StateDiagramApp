using StateDiagramApp.Model;
using System.Collections.Generic;
using System.Windows;

namespace StateDiagramApp.ViewModel
{
    public class NodeViewModel :ObservableObject
    {
        internal List<TransitionViewModel> transitionViewModels;

        private State nodeState;
        public State NodeState 
        {
            get { return nodeState; }
        }
        
        private double left;
        public double Left 
        { 
            get { return left; }
            //set 
            //{
            //    left = value;
            //    OnPropertyChanged("Left");
            //} 
        }

        private double top;
        public double Top 
        {
            get { return top; }
            //set 
            //{
            //    top = value;
            //    OnPropertyChanged("Top");
            //}
        }

        public Point Position 
        {
            get { return nodeState.Position; }
            set 
            {
                nodeState.Position = value;
                left = nodeState.Position.X;
                top = nodeState.Position.Y;

                foreach (var transition in transitionViewModels) 
                {
                    transition.MovePosition();
                }

                OnPropertyChanged("Position");
                OnPropertyChanged("Left");
                OnPropertyChanged("Top");
            }
        } 

        public NodeViewModel(State state)
        {
            this.nodeState = state;
            left = state.Position.X;
            top = state.Position.Y;
        }
    }


}
