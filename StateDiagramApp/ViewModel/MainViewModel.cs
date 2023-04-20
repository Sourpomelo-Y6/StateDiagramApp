using StateDiagramApp.Model;
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
        public State SelectedState;

        public ICommand AddStateCommand { get; private set; }
        public ICommand AddTransitionCommand { get; private set; }
        public ICommand DeleteStateCommand { get; private set; }
        public ICommand DeleteTransitionCommand { get; private set; }
        public ICommand SelectStateCommand { get; private set; }
        public ICommand MoveStateCommand { get; private set; }

        public MainViewModel()
        {
            stateDiagram = new StateDiagram();
            States = new ObservableCollection<State>();
            AddStateCommand = new RelayCommand(AddState);
            AddTransitionCommand = new RelayCommand(AddTransition);
            DeleteStateCommand = new RelayCommand(DeleteState);
            DeleteTransitionCommand = new RelayCommand(DeleteTransition);
            SelectStateCommand = new RelayCommand<State>(SelectState);
            MoveStateCommand = new RelayCommand<State>(MoveState);
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
                    Line transitionLine = new Line();
                    transitionLine.Stroke = Brushes.Black;
                    transitionLine.StrokeThickness = 2;
                    transitionLine.X1 = selectedState.Position.X + selectedState.Radius;
                    transitionLine.Y1 = selectedState.Position.Y + selectedState.Radius;
                    transitionLine.X2 = toState.Position.X + toState.Radius;
                    transitionLine.Y2 = toState.Position.Y + toState.Radius;
                    canvas.Children.Add(transitionLine);
                }
            }
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
                IEnumerable<UIElement> linesToDelete = canvas.Children.Cast<UIElement>().Where(e => e is Line && e.Tag != null && e.Tag.ToString() == $"{selectedState.Name} - {toState.Name}");
                foreach (UIElement line in linesToDelete)
                {
                    canvas.Children.Remove(line);
                }
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

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            isDragging = true;

            // If a state is selected, start drawing a transition line.
            if (selectedState != null)
            {
                Line transitionLine = new Line();

                //Copy code
                transitionLine.Stroke = Brushes.Black;
                transitionLine.StrokeThickness = 2;
                transitionLine.X1 = selectedState.Position.X + selectedState.Radius;
                transitionLine.Y1 = selectedState.Position.Y + selectedState.Radius;
                transitionLine.X2 = startPoint.X;
                transitionLine.Y2 = startPoint.Y;
                transitionLine.Tag = $"{selectedState.Name} - ";
                canvas.Children.Add(transitionLine);
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
                    IEnumerable<UIElement> transitionLines = canvas.Children.Cast<UIElement>().Where(e => e is Line && e.Tag != null && e.Tag.ToString().StartsWith($"{selectedState.Name} - "));
                    foreach (UIElement line in transitionLines)
                    {
                        ((Line)line).X2 = position.X;
                        ((Line)line).Y2 = position.Y;
                    }
                }
                // Otherwise, move the canvas.
                else
                {
                    Vector offset = startPoint - position;

                    foreach (UIElement item in canvas.Children)
                    {
                        if (item is Ellipse)
                        {
                            Ellipse ellipse = item as Ellipse;
                            Point point = new Point(Canvas.GetLeft(ellipse), Canvas.GetTop(ellipse));
                            Canvas.SetLeft(ellipse, point.X - offset.X);
                            Canvas.SetTop(ellipse, point.Y - offset.Y);
                        }
                        else if (item is Line)
                        {
                            Line line = item as Line;
                            line.X1 -= offset.X;
                            line.Y1 -= offset.Y;
                            line.X2 -= offset.X;
                            line.Y2 -= offset.Y;
                        }
                    }

                    startPoint = position;
                }
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;

            // If a state is selected, add a new state or transition.
            if (selectedState != null)
            {
                Point position = e.GetPosition(canvas);

                // If the mouse is still over the selected state, add a new transition.
                if (position.X >= selectedState.Position.X && position.X <= selectedState.Position.X + selectedState.Radius * 2 &&
                    position.Y >= selectedState.Position.Y && position.Y <= selectedState.Position.Y + selectedState.Radius * 2)
                {
                    AddTransition();
                }
                // Otherwise, add a new state.
                else
                {
                    State state = new State("State " + (States.Count + 1), position);
                    stateDiagram.States.Add(state);
                    States.Add(state);

                    // Add the new state to the canvas.
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = Brushes.LightBlue;
                    ellipse.Width = 50;
                    ellipse.Height = 50;
                    Canvas.SetLeft(ellipse, position.X);
                    Canvas.SetTop(ellipse, position.Y);
                    canvas.Children.Add(ellipse);
                }

                // Remove the transition lines.
                IEnumerable<UIElement> transitionLines = canvas.Children.Cast<UIElement>().Where(e => e is Line && e.Tag != null && e.Tag.ToString().StartsWith($"{selectedState.Name} - "));
                foreach (UIElement line in transitionLines)
                {
                    canvas.Children.Remove(line);
                }
            }

            selectedState = null;
        }

    }
}
