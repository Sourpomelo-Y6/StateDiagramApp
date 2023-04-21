using StateDiagramApp.Model;
using StateDiagramApp.View;
using StateDiagramApp.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StateDiagramApp.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private StateDiagram stateDiagram;
        private State selectedState;

        private Point startPoint;
        private bool isDragging;

        public ObservableCollection<State> States { get; set; }

        public ObservableCollection<NodeViewModel> NodeViewModels;

        public ObservableCollection<object> Shapes { get; set; }

        public enum ControlMode 
        { 
            ClickMode,
            LineMode
        }
 
        public ControlMode NowMode = ControlMode.ClickMode;
        private Brush clickModeBrush;
        public Brush ClickModeBrush
        { 
            get { return clickModeBrush; }
            set 
            {
                clickModeBrush = value;
                OnPropertyChanged("ClickModeBrush");
            }
        }
        private Brush lineModeBrush;
        public Brush LineModeBrush
        { 
            get { return lineModeBrush; }
            set 
            {
                lineModeBrush = value;
                OnPropertyChanged("LineModeBrush");
            }
        }

        public ICommand AddStateCommand { get; private set; }
        public ICommand AddTransitionCommand { get; private set; }
        public ICommand DeleteStateCommand { get; private set; }
        public ICommand DeleteTransitionCommand { get; private set; }
        public ICommand SelectStateCommand { get; private set; }
        public ICommand MoveStateCommand { get; private set; }

        public ICommand ChangeModeClickCommand { get; }
        public ICommand ChangeModeLineCommand { get; }


        public ICommand MouseDownCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ICommand MouseUpCommand { get; }

        public ICommand MouseMoveCommand2 { get; }

        public MainViewModel()
        {
            ClickModeBrush = Brushes.Red;
            LineModeBrush = Brushes.Transparent;

            stateDiagram = new StateDiagram();
            States = new ObservableCollection<State>();
            
            AddStateCommand = new RelayCommand(AddState);
            AddTransitionCommand = new RelayCommand(AddTransition);
            DeleteStateCommand = new RelayCommand(DeleteState);
            DeleteTransitionCommand = new RelayCommand(DeleteTransition);
            SelectStateCommand = new RelayCommand<State>(SelectState);
            MoveStateCommand = new RelayCommand<State>(MoveState);

            MouseDownCommand = new RelayCommand<object>(MouseDown);
            MouseMoveCommand = new RelayCommand<object>(MouseMove);
            MouseUpCommand = new RelayCommand<object>(MouseUp);

            MouseMoveCommand2 = new RelayCommand<object>(MouseMove2);

            ChangeModeClickCommand = new RelayCommand(ChangeModeClick);
            ChangeModeLineCommand = new RelayCommand(ChangeModeLine);

            var workState1 = new State("1", new Point(0, 0));
            var workState2 = new State("2", new Point(100, 100)) { };
            var workState3 = new State("3", new Point(0, 100)) { };
            var workState4 = new State("4", new Point(200, 200)) { };

            workState1.Transitions = new List<StateTransition>()
            {
                new StateTransition(workState2),
                new StateTransition(workState3)
            };

            workState2.Transitions = new List<StateTransition>()
            {
                new StateTransition(workState4)
            };

            workState3.Transitions = new List<StateTransition>()
            {
                new StateTransition(workState4)
            };

            States = new ObservableCollection<State>();
            States.Add(workState1);
            States.Add(workState2);
            States.Add(workState3);
            States.Add(workState4);

            Shapes = new ObservableCollection<object>();
            
            NodeViewModels = new ObservableCollection<NodeViewModel>();
            foreach (var State in States)
            {
                var newNodeViewModel = new NodeViewModel(State);
                //Shapes.Add(newNodeViewModel);
                newNodeViewModel.transitionViewModels = new List<TransitionViewModel>();
                NodeViewModels.Add(newNodeViewModel);
            }

            foreach (var nodeViewModel in NodeViewModels) 
            {
                 foreach (var transition in nodeViewModel.NodeState.Transitions)
                {
                    var target_state = transition.ToState;
                    NodeViewModel toNodeViewModel = null;
                    foreach (var nodeViewModel2 in NodeViewModels) 
                    {
                        if (target_state == nodeViewModel2.NodeState) 
                        {
                            toNodeViewModel = nodeViewModel2;
                        }
                    } 

                    var newTransitionViewModel = new TransitionViewModel(nodeViewModel, toNodeViewModel, transition);
                    Shapes.Add(newTransitionViewModel);
                    nodeViewModel.transitionViewModels.Add(newTransitionViewModel);
                    toNodeViewModel.transitionViewModels.Add(newTransitionViewModel);
                }
                Shapes.Add(nodeViewModel);
            }
        }

        private void ChangeModeLine()
        {
            NowMode = ControlMode.LineMode;

            ClickModeBrush = Brushes.Transparent;
            LineModeBrush = Brushes.Red;
        }

        private void ChangeModeClick()
        {
            NowMode = ControlMode.ClickMode;
            
            ClickModeBrush = Brushes.Red;
            LineModeBrush = Brushes.Transparent;
        }

        private void AddState()
        {
            State state = new State("State " + (States.Count + 1), new Point(100, 100));
            stateDiagram.States.Add(state);
            States.Add(state);
        }

        private void AddTransition()
        {
            if (selectedState == null)
            {
                MessageBox.Show("Please select a state to add a transition from.");
                return;
            }

            StateTransitionWindow transitionWindow = new StateTransitionWindow(States.ToList());
            if (transitionWindow.ShowDialog() == true)
            {
                State toState = transitionWindow.SelectedState;
                if (stateDiagram.AddTransition(selectedState, toState))
                {
                    // Add the transition line to the canvas.
                    //Line transitionLine = new Line();
                    //transitionLine.Stroke = Brushes.Black;
                    //transitionLine.StrokeThickness = 2;
                    //transitionLine.X1 = selectedState.Position.X + selectedState.Radius;
                    //transitionLine.Y1 = selectedState.Position.Y + selectedState.Radius;
                    //transitionLine.X2 = toState.Position.X + toState.Radius;
                    //transitionLine.Y2 = toState.Position.Y + toState.Radius;
                    //canvas.Children.Add(transitionLine);

                    UpdateShape();
                }
            }
        }

        //あまり使わない方向で
        private void UpdateShape()
        {
            var newShapes = new ObservableCollection<object>();
            foreach (var State in States)
            {
                newShapes.Add(new NodeViewModel(State));
                foreach (var transition in State.Transitions)
                {
                    //newShapes.Add(new TransitionViewModel() { StartPoint = State.Position, EndPoint = transition.ToState.Position });
                }
            }
            Shapes = newShapes;
        }

        private void DeleteState()
        {
            if (selectedState == null)
            {
                MessageBox.Show("Please select a state to delete.");
                return;
            }

            stateDiagram.DeleteState(selectedState);
            States.Remove(selectedState);
        }

        private void DeleteTransition()
        {
            if (selectedState == null)
            {
                MessageBox.Show("Please select a state to delete a transition from.");
                return;
            }

            StateTransitionWindow transitionWindow = new StateTransitionWindow(selectedState.Transitions.Select(t => t.ToState).ToList());
            if (transitionWindow.ShowDialog() == true)
            {
                State toState = transitionWindow.SelectedState;
                stateDiagram.DeleteTransition(selectedState, toState);

                // Remove the transition line from the canvas.
                //IEnumerable<UIElement> linesToDelete = canvas.Children.Cast<UIElement>().Where(e => e is Line && e.Tag != null && e.Tag.ToString() == $"{selectedState.Name} - {toState.Name}");
                //foreach (UIElement line in linesToDelete)
                //{
                //    canvas.Children.Remove(line);
                //}

                UpdateShape();
            }
        }

        private void SelectState(State state)
        {
            if (state != null)
            {
                selectedState = state;
            }
        }

        private void MoveState(State state)
        {
            if (state != null)
            {
                stateDiagram.MoveState(state, state.Position);
                
            }
        }

        private NodeViewModel selectedNode;
        public NodeViewModel SelectedNode 
        {
            get { return selectedNode; }
            set 
            {
                if (selectedNode != null && selectedNode != value) 
                {
                    selectedNode.Selected = false;
                }

                selectedNode = value;

                if (selectedNode != null)
                {
                    selectedNode.Selected = true;
                }
                OnPropertyChanged("SelectedNode");
            }
        }

        private NodeViewModel selectedNode2;
        public NodeViewModel SelectedNode2
        {
            get { return selectedNode2; }
            set
            {
                if (selectedNode2 != null && selectedNode2 != value)
                {
                    selectedNode2.Selected2 = false;
                }

                selectedNode2 = value;

                if (selectedNode2 != null)
                {
                    selectedNode2.Selected2 = true;
                }
                OnPropertyChanged("SelectedNode2");
            }
        }

        private void MouseDown(object parameter)
        {
            if (NowMode == ControlMode.ClickMode)
            {
                if (parameter is NodeViewModel Node)
                {
                    SelectedNode = Node;
                    startPoint = Mouse.GetPosition(null);
                    isDragging = true;
                }
            }
            else if (NowMode == ControlMode.LineMode) 
            {
                if (parameter is NodeViewModel Node)
                {
                    SelectedNode = Node;
                    isDragging = true;
                }
            }
        }
        
        bool flag_free = false;

        private void MouseMove(object parameter)
        {
            if (NowMode == ControlMode.ClickMode)
            {
                if (isDragging)
                {
                    var currentPosition = Mouse.GetPosition(null);
                    var deltaX = currentPosition.X - startPoint.X;
                    var deltaY = currentPosition.Y - startPoint.Y;
                    var nowPosition = SelectedNode.Position;
                    SelectedNode.Position = new Point(nowPosition.X + deltaX, nowPosition.Y + deltaY);

                    startPoint = currentPosition;
                }
            }
            else if (NowMode == ControlMode.LineMode)
            {
                if (isDragging)
                {
                    if (flag_free)
                    {
                        SelectedNode2 = null;
                    }
                    else 
                    {
                        flag_free = true;
                    }
                }
            }
        }

        private void MouseMove2(object parameter)
        {
            if (NowMode == ControlMode.LineMode)
            {
                NodeViewModel work = null;
                if (isDragging)
                {
                    if (parameter is NodeViewModel Node)
                    {
                        if (SelectedNode != Node)
                        {
                            work = Node;
                        }
                    }

                    if (work == null)
                    {
                        flag_free = true;
                    }
                    else flag_free = false;
                    SelectedNode2 = work;
                }
            }
        }

        private void MouseUp(object parameter)
        {
            if (NowMode == ControlMode.ClickMode)
            {
                isDragging = false;
                SelectedNode = null;
            }
            else if (NowMode == ControlMode.LineMode) 
            {
                AddTransition(SelectedNode,SelectedNode2);
                isDragging = false;
                SelectedNode = null;
                SelectedNode2 = null;
            }
        }

        private void AddTransition(NodeViewModel selectedNode, NodeViewModel selectedNode2)
        {
            var transition = new StateTransition(selectedNode2.NodeState);
            selectedNode.NodeState.Transitions.Add(transition);
            var newTransitionViewModel = new TransitionViewModel(selectedNode,selectedNode2,transition);
            selectedNode.transitionViewModels.Add(newTransitionViewModel);
            selectedNode2.transitionViewModels.Add(newTransitionViewModel);
            Shapes.Add(newTransitionViewModel);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            isDragging = true;

            // If a state is selected, start drawing a transition line.
            if (selectedState != null)
            {
                //Line transitionLine = new Line();

                ////Copy code
                //transitionLine.Stroke = Brushes.Black;
                //transitionLine.StrokeThickness = 2;
                //transitionLine.X1 = selectedState.Position.X + selectedState.Radius;
                //transitionLine.Y1 = selectedState.Position.Y + selectedState.Radius;
                //transitionLine.X2 = startPoint.X;
                //transitionLine.Y2 = startPoint.Y;
                //transitionLine.Tag = $"{selectedState.Name} - ";
                //canvas.Children.Add(transitionLine);

                UpdateShape();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point position = e.GetPosition(null);

                // If a state is selected, update the transition line.
                if (selectedState != null)
                {
                    //IEnumerable<UIElement> transitionLines = canvas.Children.Cast<UIElement>().Where(e => e is Line && e.Tag != null && e.Tag.ToString().StartsWith($"{selectedState.Name} - "));
                    //foreach (UIElement line in transitionLines)
                    //{
                    //    ((Line)line).X2 = position.X;
                    //    ((Line)line).Y2 = position.Y;
                    //}


                }
                // Otherwise, move the canvas.
                else
                {
                    Vector offset = startPoint - position;

                    //foreach (UIElement item in canvas.Children)
                    //{
                    //    if (item is Node)
                    //    {
                    //        Node Node = item as Node;
                    //        Point point = new Point(Canvas.GetLeft(Node), Canvas.GetTop(Node));
                    //        Canvas.SetLeft(Node, point.X - offset.X);
                    //        Canvas.SetTop(Node, point.Y - offset.Y);
                    //    }
                    //    else if (item is Line)
                    //    {
                    //        Line line = item as Line;
                    //        line.X1 -= offset.X;
                    //        line.Y1 -= offset.Y;
                    //        line.X2 -= offset.X;
                    //        line.Y2 -= offset.Y;
                    //    }
                    //}

                    startPoint = position;
                }
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;

            //// If a state is selected, add a new state or transition.
            //if (selectedState != null)
            //{
            //    Point position = e.GetPosition(canvas);

            //    // If the mouse is still over the selected state, add a new transition.
            //    if (position.X >= selectedState.Position.X && position.X <= selectedState.Position.X + selectedState.Radius * 2 &&
            //        position.Y >= selectedState.Position.Y && position.Y <= selectedState.Position.Y + selectedState.Radius * 2)
            //    {
            //        AddTransition();
            //    }
            //    // Otherwise, add a new state.
            //    else
            //    {
            //        State state = new State("State " + (States.Count + 1), position);
            //        stateDiagram.States.Add(state);
            //        States.Add(state);

            //        // Add the new state to the canvas.
            //        Node Node = new Node();
            //        Node.Fill = Brushes.LightBlue;
            //        Node.Width = 50;
            //        Node.Height = 50;
            //        Canvas.SetLeft(Node, position.X);
            //        Canvas.SetTop(Node, position.Y);
            //        canvas.Children.Add(Node);
            //    }

            //    // Remove the transition lines.
            //    IEnumerable<UIElement> transitionLines = canvas.Children.Cast<UIElement>().Where(e => e is Line && e.Tag != null && e.Tag.ToString().StartsWith($"{selectedState.Name} - "));
            //    foreach (UIElement line in transitionLines)
            //    {
            //        canvas.Children.Remove(line);
            //    }
            //}

            selectedState = null;
        }


    }
}
