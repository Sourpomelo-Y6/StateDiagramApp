using StateDiagramApp.Model;
using System;
using System.Windows;

namespace StateDiagramApp.ViewModel
{
    class TransitionViewModel : ObservableObject
    {
        private StateTransition transition;
        private NodeViewModel FromNodeViewModel;
        private NodeViewModel ToNodeViewModel;

        public TransitionViewModel(NodeViewModel fromNodeViewModel, NodeViewModel toNodeViewModel,StateTransition transition)
        {
            this.FromNodeViewModel = fromNodeViewModel;
            this.ToNodeViewModel = toNodeViewModel;
            this.transition = transition;
        }

        public Point StartPoint
        {
            get { return FromNodeViewModel.Position; }
            set
            {
                FromNodeViewModel.Position = value;
                OnPropertyChanged("StartPoint");
            }
        }

        public Point EndPoint 
        {
            get { return ToNodeViewModel.Position; }
            set 
            {
                ToNodeViewModel.Position = value;
                OnPropertyChanged("EndPoint");
            }
        }

        internal void MovePosition()
        {
            OnPropertyChanged("StartPoint");
            OnPropertyChanged("EndPoint");
        }
    }
}
