﻿using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using VisualQueryApplication.Controls.GraphBuilder;

namespace VisualQueryApplication.ViewModels
{
    public class VisualNodeViewModel : VisualGraphComponentViewModel
    {
        public Type NodeType { get; private set; }

        public string NodeTitle
        {
            get
            {
                Attribute titleAttribute = NodeType.GetCustomAttribute(typeof(NodeName));

                if (((NodeName)titleAttribute).IsHidden)
                    return "";
                else
                    return ((NodeName)titleAttribute).Name;
            }
        }

        public ObservableCollection<NodePinViewModel> Inputs
        {
            get
            {
                return inputs;
            }
            set
            {
                inputs = value;
                OnPropertyChanged(nameof(Inputs));
            }
        }

        private ObservableCollection<NodePinViewModel> inputs = new ObservableCollection<NodePinViewModel>();

        public ObservableCollection<NodePinViewModel> Outputs
        {
            get
            {
                return outputs;
            }
            set
            {
                outputs = value;
                OnPropertyChanged(nameof(Outputs));
            }
        }

        private ObservableCollection<NodePinViewModel> outputs = new ObservableCollection<NodePinViewModel>();

        public ObservableCollection<NodePinViewModel> ExecutionInputs
        {
            get { return executionInputs; }
            set
            {
                executionInputs = value;
                OnPropertyChanged(nameof(ExecutionInputs));
            }
        } 

        private ObservableCollection<NodePinViewModel> executionInputs = new ObservableCollection<NodePinViewModel>(); 

        public ObservableCollection<NodePinViewModel> ExecutionOutputs
        {
            get { return executionOutputs; }
            set
            {
                executionOutputs = value;
                OnPropertyChanged(nameof(ExecutionOutputs));
            }
        }

        private ObservableCollection<NodePinViewModel> executionOutputs = new ObservableCollection<NodePinViewModel>();

        public VisualNodeViewModel(Type nodeType)
        {
            this.NodeType = nodeType;

            // If the node is executable
            if (this.NodeType.IsSubclassOf(typeof(ExecutableNode)))
            {
                // Set an execution-in pin
                executionInputs.Add(new NodePinViewModel("In", null, false, true, 0));

                List<Attribute> outputsList = nodeType.GetCustomAttributes(typeof(ExecutionOutDescription)).ToList();

                for (int i = 0; i < outputsList.Count; i++)
                {
                    ExecutionOutputs.Add(new NodePinViewModel(
                        ((ExecutionOutDescription)outputsList[i]).Label, null, true, true, i));
                }
            }

            int inputIndex = 0;
            int outputIndex = 0;

            // Import inputs and outputs
            foreach (FieldInfo field in this.NodeType.GetFields())
            {
                foreach (Attribute attribute in field.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof (ExposedInput))
                    {
                        switch ((attribute as ExposedInput).LabelDisplay)
                        {
                            case LabelDisplay.Field:
                                inputs.Add(new NodePinViewModel(field.Name, field.FieldType, false, false, inputIndex));
                                break;
                            case LabelDisplay.Custom:
                                inputs.Add(new NodePinViewModel((attribute as ExposedInput).Label, field.FieldType, false, false, inputIndex));
                                break;
                            case LabelDisplay.Hidden:
                                inputs.Add(new NodePinViewModel("", field.FieldType, false, false, inputIndex));
                                break;
                        }

                        inputIndex++;
                    }
                    else if (attribute.GetType() == typeof (ExposedOutput))
                    {
                        switch ((attribute as ExposedOutput).LabelDisplay)
                        {
                            case LabelDisplay.Field:
                                outputs.Add(new NodePinViewModel(field.Name, field.FieldType, true, false, inputIndex));
                                break;
                            case LabelDisplay.Custom:
                                outputs.Add(new NodePinViewModel((attribute as ExposedInput).Label, field.FieldType, true, false, inputIndex));
                                break;
                            case LabelDisplay.Hidden:
                                outputs.Add(new NodePinViewModel("", field.FieldType, true, false, inputIndex));
                                break;
                        }

                        outputIndex++;
                    }
                }
            }
        }

        public override void RemoveConnections()
        {
            GraphEditorViewModel editor = (GraphEditorViewModel)((MainWindow)(App.Current.MainWindow)).VisualEditor.DataContext;
            List<NodePin> pins = new List<NodePin>();

            pins.AddRange(Inputs.Select(pin => pin.Pin));
            pins.AddRange(Outputs.Select(pin => pin.Pin));
            pins.AddRange(ExecutionInputs.Select(pin => pin.Pin));
            pins.AddRange(ExecutionOutputs.Select(pin => pin.Pin));

            List<ConnectionViewModel> connectionsToRemove = new List<ConnectionViewModel>();

            foreach (var connection in editor.Connections)
            {
                if (pins.Contains(connection.InputPinControl) || (pins.Contains(connection.OutputPinControl)))
                {
                    connectionsToRemove.Add(connection);
                }
            }

            foreach (var connection in connectionsToRemove)
            {
                editor.Connections.Remove(connection);
            }
        }
    }
}
