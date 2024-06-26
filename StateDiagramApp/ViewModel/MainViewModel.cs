﻿using StateDiagramApp.Model;
using StateDiagramApp.Utillity;
using StateDiagramApp.View;
using StateDiagramApp.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private Point startPoint;
        private bool isDragging;

        public ObservableCollection<State> States { get; set; }

        public ObservableCollection<NodeViewModel> NodeViewModels;

        public ObservableCollection<object> Shapes { get; set; }

        public enum ControlMode 
        { 
            None,
            ClickMode,
            LineMode,
            NewMode,
            PropertyMode,
            DeleteMode
        }
 
        public ControlMode NowMode = ControlMode.None;

        public ICommand FileSaveCommand { get; }
        public ICommand FileLoadCommand { get; }

        public ICommand ItemMouseDownCommand { get; }
        public ICommand ItemMouseMoveCommand { get; }
        public ICommand ItemMouseUpCommand { get; }

        public ICommand WindowMouseDownCommand { get; }
        public ICommand WindowMouseMoveCommand { get; }
        public ICommand WindowMouseUpCommand { get; }

        public MainViewModel()
        {
            State.idCounter = 0;
            Shapes = new ObservableCollection<object>();
            NodeViewModels = new ObservableCollection<NodeViewModel>();

            IsClickRadioButtonSelected = true;

            //stateDiagram = new StateDiagram();
            States = new ObservableCollection<State>();

            FileSaveCommand = new RelayCommand(FileSave);
            FileLoadCommand = new RelayCommand(FileLoad);

            ItemMouseDownCommand = new RelayCommand<object>(ItemMouseDown);
            ItemMouseMoveCommand = new RelayCommand<object>(ItemMouseMove);
            ItemMouseUpCommand = new RelayCommand<object>(ItemMouseUp);

            WindowMouseDownCommand = new RelayCommand<object>(WindowMouseDown);
            WindowMouseMoveCommand = new RelayCommand<object>(WindowMouseMove);
            WindowMouseUpCommand = new RelayCommand<object>(WindowMouseUp);

            //var xml = new XmlFile();

            //SettingTestState();

            //ObservableCollection<State> work;
            //xml.ReadXml(out work);
            //States = work;

            //

        }

        private void FileLoad()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".xml"; // Default file extension
            dialog.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                var xml = new XmlFile(filename);

                ObservableCollection<State> work;
                xml.ReadXml(out work);
                States = work;
                SettingShapes();
            }

        }

        private void FileSave()
        {
            // Configure save file dialog box
            var dialog = new Microsoft.Win32.SaveFileDialog();
            //dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".xml"; // Default file extension
            dialog.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            bool? result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dialog.FileName;
                var xml = new XmlFile(filename);
                xml.WriteXml(States);
            }

        }

        private void SettingShapes()
        {
            Shapes.Clear();
            NodeViewModels.Clear();
            foreach (var State in States)
            {
                var newNodeViewModel = new NodeViewModel(State);
                //Shapes.Add(newNodeViewModel);
                newNodeViewModel.TransitionViewModels = new ObservableCollection<TransitionViewModel>();
                NodeViewModels.Add(newNodeViewModel);
            }

            uint MaxIDNo = 0;
            foreach (var nodeViewModel in NodeViewModels)
            {    

                if (nodeViewModel.NodeState.IDNo > MaxIDNo) 
                {
                    MaxIDNo = nodeViewModel.NodeState.IDNo;
                }

                foreach (var transition in nodeViewModel.NodeState.Transitions)
                {
                    var target_stateID = transition.ToStateID;
                    NodeViewModel toNodeViewModel = null;
                    foreach (var nodeViewModel2 in NodeViewModels)
                    {
                        if (target_stateID == nodeViewModel2.NodeState.IDNo)
                        {
                            toNodeViewModel = nodeViewModel2;
                        }
                    }

                    var newTransitionViewModel = new TransitionViewModel(nodeViewModel, toNodeViewModel, transition);
                    Shapes.Add(newTransitionViewModel);
                    nodeViewModel.TransitionViewModels.Add(newTransitionViewModel);
                    if (toNodeViewModel != null)//[todo]上のtoNodeViewModelがnullになる
                    {
                        toNodeViewModel.TransitionViewModels.Add(newTransitionViewModel);
                    }
                }
                Shapes.Add(nodeViewModel);
            }

            State.idCounter = MaxIDNo + 1;
        }

        private void SettingTestState()
        {
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

        private void ItemMouseDown(object parameter)
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
            else if (NowMode == ControlMode.PropertyMode) 
            {
                if (parameter is NodeViewModel Node)
                {
                    SelectedNode = Node;
                }
            }

        }

        private void WindowMouseDown(object parameter) 
        {
            if (NowMode == ControlMode.NewMode)
            {
                if (parameter is Canvas canvas)
                {
                    startPoint = Mouse.GetPosition(canvas);
                }
                else 
                {
                    startPoint = Mouse.GetPosition(null);
                }
            }
        }


        bool flag_free = false;

        private void ItemMouseMove(object parameter)
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

        private void WindowMouseMove(object parameter)
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

        private void ItemMouseUp(object parameter)
        {
            if (NowMode == ControlMode.ClickMode)
            {
                isDragging = false;
                SelectedNode = null;
            }
            else if (NowMode == ControlMode.LineMode)
            {
                if (SelectedNode != null && SelectedNode2 != null)
                {
                    AddTransition(SelectedNode, SelectedNode2);
                }
                isDragging = false;
                SelectedNode = null;
                SelectedNode2 = null;
            }
            else if (NowMode == ControlMode.PropertyMode) 
            {
                if (parameter is NodeViewModel Node)
                {
                    PropertyWindow window = new PropertyWindow(Node);
                    window.ShowDialog();
                    SettingShapes();
                }
                SelectedNode = null;
            }
            else if (NowMode == ControlMode.DeleteMode)
            {
                if (parameter is NodeViewModel Node)
                {
                    DeleteNode(Node);
                }
            }

        }

        private void WindowMouseUp(object parameter)
        {
            if (NowMode == ControlMode.NewMode)
            {
                AddNode(startPoint);
            }

        }

        private void DeleteNode(NodeViewModel node)
        {
            
            List<TransitionViewModel> DeleteList = new List<TransitionViewModel>();
            foreach (var transition in node.TransitionViewModels)
            {
                var ToNode = transition.ToNodeViewModel;
                foreach (var toTransition in ToNode.TransitionViewModels)
                {
                    if (toTransition.FromNodeViewModel == node)
                    {
                        DeleteList.Add(toTransition);
                        //ToNode.TransitionViewModels.Remove(toTransition);
                        continue;
                    }
                }
            }

            for (int i = 0; i < node.TransitionViewModels.Count; i++)
            {
                var ToNode = node.TransitionViewModels[i].ToNodeViewModel;
                foreach (var target in DeleteList)
                {
                    ToNode.TransitionViewModels.Remove(target);
                    //Shapes.Remove(target);
                }

                foreach (var target in DeleteList)
                {
                    //ToNode.TransitionViewModels.Remove(target);
                    Shapes.Remove(target);
                }


                Shapes.Remove(node.TransitionViewModels[i]);
            }

            Shapes.Remove(node);
            States.Remove(node.NodeState);
            NodeViewModels.Remove(node);
        }

        private void AddNode(Point startPoint)
        {
            var node = new State("",startPoint);
            States.Add(node);
            var nodeViewModel = new NodeViewModel(node);
            NodeViewModels.Add(nodeViewModel);
            Shapes.Add(nodeViewModel);
            nodeViewModel.TransitionViewModels = new ObservableCollection<TransitionViewModel>();
        }

        private void AddTransition(NodeViewModel selectedNode, NodeViewModel selectedNode2)
        {
            var transition = new StateTransition(selectedNode2.NodeState);
            //transition.Comment = "test";
            selectedNode.NodeState.Transitions.Add(transition);
            var newTransitionViewModel = new TransitionViewModel(selectedNode,selectedNode2,transition);
            selectedNode.TransitionViewModels.Add(newTransitionViewModel);
            selectedNode2.TransitionViewModels.Add(newTransitionViewModel);
            Shapes.Add(newTransitionViewModel);
        }

        public bool IsNewRadioButtonSelected
        {
            get { return _isNewRadioButtonSelected; }
            set
            {
                _isNewRadioButtonSelected = value;
                if (value) {
                    NowMode = ControlMode.NewMode;
                }
                OnPropertyChanged(nameof(IsNewRadioButtonSelected));
            }
        }
        private bool _isNewRadioButtonSelected;

        public bool IsClickRadioButtonSelected
        {
            get { return _isClickRadioButtonSelected; }
            set
            {
                _isClickRadioButtonSelected = value;
                if (value)
                {
                    NowMode = ControlMode.ClickMode;
                }
                OnPropertyChanged(nameof(IsClickRadioButtonSelected));
            }
        }
        private bool _isClickRadioButtonSelected;

        public bool IsLineRadioButtonSelected
        {
            get { return _isLineRadioButtonSelected; }
            set
            {
                _isLineRadioButtonSelected = value;
                if (value)
                {
                    NowMode = ControlMode.LineMode;
                }
                OnPropertyChanged(nameof(IsLineRadioButtonSelected));
            }
        }
        private bool _isLineRadioButtonSelected;

        public bool IsPropertyRadioButtonSelected
        {
            get { return _isPropertyRadioButtonSelected; }
            set
            {
                _isPropertyRadioButtonSelected = value;
                if (value)
                {
                    NowMode = ControlMode.PropertyMode;
                }
                OnPropertyChanged(nameof(IsPropertyRadioButtonSelected));
            }
        }
        private bool _isPropertyRadioButtonSelected;

        public bool IsDeleteRadioButtonSelected
        {
            get { return _isDeleteRadioButtonSelected; }
            set
            {
                _isDeleteRadioButtonSelected = value;
                if (value)
                {
                    NowMode = ControlMode.DeleteMode;
                }
                OnPropertyChanged(nameof(IsDeleteRadioButtonSelected));
            }
        }
        private bool _isDeleteRadioButtonSelected;
    }
}
