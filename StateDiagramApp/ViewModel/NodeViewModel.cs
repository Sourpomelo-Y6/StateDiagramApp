using StateDiagramApp.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StateDiagramApp.ViewModel
{
    public class NodeViewModel : ObservableObject
    {
        private ObservableCollection<TransitionViewModel> transitionViewModels;
        public ObservableCollection<TransitionViewModel> TransitionViewModels 
        {
            get { return transitionViewModels; }
            set 
            {
                transitionViewModels = value;
                OnPropertyChanged("TransitionViewModels");
            } 
        }

        private State nodeState;
        public State NodeState 
        {
            get { return nodeState; }
        }

        public string NodeStateName
        {
            get { return nodeState.Name; }
            set 
            {
                nodeState.Name = value;
                OnPropertyChanged("NodeStateName");
            }
        }

        public uint NodeStateID
        {
            get { return nodeState.IDNo; }
            set
            {
                nodeState.IDNo = value;
                OnPropertyChanged("NodeStateID");
            }
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

            DeleteTransitionCommand = new RelayCommand<object>(DeleteTransition);
        }


        private TransitionViewModel selectedTransition;
        public TransitionViewModel SelectedTransition 
        {
            get { return selectedTransition; }
            set 
            {
                selectedTransition = value;
                OnPropertyChanged("SelectedTransition");
            }
        
        }

        public ICommand DeleteTransitionCommand { get; }

        public void DeleteTransition(object parameter)
        {
            if (selectedTransition != null)
            {
                nodeState.Transitions.Remove(selectedTransition.GetTransition());
                transitionViewModels.Remove(selectedTransition);
                //selectedTransition.Delete();
                //foreach (var transition in nodeState.Transitions) 
                //{
                //    if (transition.isDelete) 
                //    {
                //        nodeState.Transitions.Remove(transition);
                //        break;
                //    }
                //}
            }
        }
    }


}
