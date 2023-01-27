using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Classes;
using RobotEditor.Enums;

namespace RobotEditor.ViewModel
{
    public class ValueBoxViewModel : ObservableObject
    {
        private readonly CartesianItems _selectionitems = new CartesianItems();
        private Visibility _boxVisibility = Visibility.Visible;
        private string _header = string.Empty;
        private bool _isReadOnly;
        private CartesianEnum _selectedItem = CartesianEnum.ABB_Quaternion;
        private double _v1;
        private double _v2;
        private double _v3;
        private double _v4;

        public double V1
        {
            get => _v1;
            set
            {
                SetProperty(ref _v1, value);
                RaiseItemsChanged();
            }
        }

        public double V2
        {
            get => _v2;
            set
            {
                SetProperty(ref _v2, value);
                RaiseItemsChanged();
            }
        }

        public double V3
        {
            get => _v3;
            set
            {
                SetProperty(ref _v3, value);
                RaiseItemsChanged();
            }
        }

        public double V4
        {
            get => _v4;
            set
            {
                SetProperty(ref _v4, value);
                RaiseItemsChanged();
            }
        }

        public bool IsReadOnly { get =>_isReadOnly; set=>SetProperty(ref _isReadOnly,value); }

        public string Header { get =>_header; set=>SetProperty(ref _header,value); }

        public Visibility BoxVisibility { get =>_boxVisibility; set=>SetProperty(ref _boxVisibility,value); }

        public CartesianItems SelectionItems => _selectionitems;

        public CartesianEnum SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                CheckVisibility();
                OnPropertyChanged(nameof(SelectedItem));
                RaiseItemsChanged();
            }
        }

        public event ItemsChangedEventHandler ItemsChanged;

        private void RaiseItemsChanged() => ItemsChanged?.Invoke(this, (ItemsChangedEventArgs)new EventArgs());

        private void CheckVisibility()
        {
            switch (_selectedItem)
            {
                case CartesianEnum.ABB_Quaternion:
                case CartesianEnum.Axis_Angle:
                    BoxVisibility = Visibility.Visible;
                    return;
            }
            BoxVisibility = Visibility.Collapsed;
        }
    }
}