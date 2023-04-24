using StateDiagramApp.Model;
using System;
using System.Windows;

namespace StateDiagramApp.ViewModel
{
    public class TransitionViewModel : ObservableObject
    {
        private StateTransition transition;
        public StateTransition GetTransition()
        {
            return transition; 
        }

        private NodeViewModel fromNodeViewModel;
        public NodeViewModel FromNodeViewModel
        {
            get { return fromNodeViewModel; }
        }

        private NodeViewModel toNodeViewModel;
        public NodeViewModel ToNodeViewModel 
        {
            get { return toNodeViewModel; }
        }

        public TransitionViewModel(NodeViewModel fromNodeViewModel, NodeViewModel toNodeViewModel,StateTransition transition)
        {
            this.fromNodeViewModel = fromNodeViewModel;
            this.toNodeViewModel = toNodeViewModel;
            this.transition = transition;
        }

        public Point StartPoint
        {
            get { return FromNodeViewModel.Position; }
            set
            {
                fromNodeViewModel.Position = value;
                OnPropertyChanged("StartPoint");
            }
        }

        public Point EndPoint 
        {
            get { return ToNodeViewModel.Position; }
            set 
            {
                toNodeViewModel.Position = value;
                OnPropertyChanged("EndPoint");
            }
        }

        internal void MovePosition()
        {
            OnPropertyChanged("StartPoint");
            OnPropertyChanged("EndPoint");
            OnPropertyChanged("CommentMargin");
        }

        public string Comment
        {
            get { return transition.Comment; }
            set
            {
                transition.Comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public Thickness CommentMargin
        {
            get 
            {
                return new Thickness(
                    (FromNodeViewModel.Position.X + ToNodeViewModel.Position.X) /2,
                    (FromNodeViewModel.Position.Y + ToNodeViewModel.Position.Y) /2,
                    0, 0);              
            }

        }

        public string FromName 
        {
            get { return FromNodeViewModel.NodeState.Name; }
        }

        public string ToName
        {
            get { return ToNodeViewModel.NodeState.Name; }
        }

    }
}
