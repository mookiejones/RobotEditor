using CommunityToolkit.Mvvm.DependencyInjection;
using RobotEditor.Interfaces;
using RobotEditor.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RobotEditor.Controls;

/// <summary>
///     Interaction logic for VisualVariableItem.xaml
/// </summary>
public partial class VisualVariableItem : DataGrid
{
    public VisualVariableItem()
    {
        InitializeComponent();
    }

    private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        DependencyObject child = (DependencyObject)e.OriginalSource;
        DataGridRow dataGridRow = TryFindParent<DataGridRow>(child);
        if (dataGridRow != null)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid == null || dataGrid.CurrentCell.IsValid)
            {
                if (dataGrid != null)
                {
                    MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
                    if (dataGrid.CurrentCell.Item is IVariable variable && File.Exists(variable.Path))
                    {
                        instance.OpenFile(variable);
                    }
                }
                e.Handled = true;
            }
        }
    }

    public T TryFindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = GetParentObject(child);
        T result;
        if (parentObject == null)
        {
            result = default(T);
        }
        else
        {
            T t = parentObject as T;
            T arg_3E_0;
            if ((arg_3E_0 = t) == null)
            {
                arg_3E_0 = TryFindParent<T>(parentObject);
            }
            result = arg_3E_0;
        }
        return result;
    }

    public DependencyObject GetParentObject(DependencyObject child)
    {
        DependencyObject result;
        if (child == null)
        {
            result = null;
        }
        else
        {
            if (child is ContentElement contentElement)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                result = parent != null
                    ? parent
                    : (contentElement is FrameworkContentElement frameworkContentElement) ? frameworkContentElement.Parent : null;
            }
            else
            {
                if (child is FrameworkElement frameworkElement)
                {
                    DependencyObject parent = frameworkElement.Parent;
                    if (parent != null)
                    {
                        result = parent;
                        return result;
                    }
                }
                result = VisualTreeHelper.GetParent(child);
            }
        }
        return result;
    }

    private void ToolTip_Opening(object sender, ToolTipEventArgs e) => throw new NotImplementedException();
}