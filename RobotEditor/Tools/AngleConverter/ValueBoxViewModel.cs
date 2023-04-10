using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Enums;
using RobotEditor.Languages.Data;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace RobotEditor.Tools.AngleConverter;

public partial class ValueBoxViewModel : ObservableObject
{
    private Visibility _boxVisibility = Visibility.Visible;

    [ObservableProperty]
    private string _header = string.Empty;
    [ObservableProperty]
    private bool isReadOnly;

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
            _ = SetProperty(ref _v1, value);
            RaiseItemsChanged();
        }
    }

    public double V2
    {
        get => _v2;
        set
        {
            _ = SetProperty(ref _v2, value);
            RaiseItemsChanged();
        }
    }

    public double V3
    {
        get => _v3;
        set
        {
            _ = SetProperty(ref _v3, value);
            RaiseItemsChanged();
        }
    }

    public double V4
    {
        get => _v4;
        set
        {
            _ = SetProperty(ref _v4, value);
            RaiseItemsChanged();
        }
    }



    public Visibility BoxVisibility { get => _boxVisibility; set => SetProperty(ref _boxVisibility, value); }

    public CartesianItems SelectionItems { get; } = new CartesianItems();

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

    private void RaiseItemsChanged()
    {
        var args = new EventArgs();
        var passedArgs = args as ItemsChangedEventArgs;

        ItemsChanged?.Invoke(this, passedArgs);

    }
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