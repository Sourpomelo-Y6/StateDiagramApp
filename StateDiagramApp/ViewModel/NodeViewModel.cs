using StateDiagramApp.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace StateDiagramApp.ViewModel
{
    public class NodeViewModel : ObservableObject
    {
        internal List<TransitionViewModel> transitionViewModels;

        private State nodeState;
        public State NodeState 
        {
            get { return nodeState; }
        }

        public string NodeStateName
        {
            get { return nodeState.Name; }
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

        private bool selected = false;
        public bool Selected 
        {
            get { return selected; }
            set 
            {
                selected = value;
                if (selected)
                {
                    SelectMark = Brushes.Red;
                }
                else 
                {
                    SelectMark = Brushes.Black;
                }
                OnPropertyChanged("Selected");
            } 
        }

        private bool selected2 = false;
        public bool Selected2
        {
            get { return selected2; }
            set
            {
                selected2 = value;
                if (selected2)
                {
                    SelectMark = Brushes.Blue;
                }
                else
                {
                    SelectMark = Brushes.Black;
                }
                OnPropertyChanged("Selected2");
            }
        }

        private Brush selectMark = Brushes.Black;
        public Brush SelectMark
        {
            get { return selectMark; }
            set
            {
                selectMark = value;
                OnPropertyChanged("SelectMark");
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
