using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using Newtonsoft.Json;
using RMIDispenseSequenceEditor.Models;
using RMIDispenseSequenceEditor.ViewModels;

namespace RMIDispenseSequenceEditor.Views
{
    public partial class DispenseProtocolEditorControl : UserControl
    {
        private Point _dragStartPoint;
        private object _draggedItem;

        public DispenseProtocolEditorControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DispenseProtocolEditorViewModel vm)
            {
                vm.PreviewResultCompleted += async (_, __) =>
                {
                    //await IngredientsTable.PlayStackingAnimationAsync(500);
                    await IngredientsTable.PlayBottomUpByColumnAsync(500);
                };
            }
        }


        // ========== STEP LIST (vertical drag-drop) ==========
        private void StepList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _draggedItem = GetListBoxItemContent(e.OriginalSource);
        }

        private void StepList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null)
            {
                var position = e.GetPosition(null);
                var diff = _dragStartPoint - position;
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DragDrop.DoDragDrop((DependencyObject)sender, _draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void StepList_Drop(object sender, DragEventArgs e)
        {
            var listBox = sender as ListBox;
            var items = listBox?.ItemsSource as ObservableCollection<Sequence>;
            if (items == null) return;

            var droppedData = e.Data.GetData(typeof(Sequence)) as Sequence;
            if (droppedData == null) return;

            var target = GetDataFromPoint(listBox, e.GetPosition(listBox)) as Sequence;
            if (target == null || droppedData == target) return;

            int removeIndex = items.IndexOf(droppedData);
            int insertIndex = items.IndexOf(target);
            if (removeIndex < 0 || insertIndex < 0) return;

            items.Move(removeIndex, insertIndex);
        }

        private object GetListBoxItemContent(object originalSource)
        {
            var item = FindAncestor<ListBoxItem>(originalSource as DependencyObject);
            return item?.DataContext;
        }
        
      // ========== PARALLEL ITEMS (horizontal drag-drop) ==========
        private void ParallelItemsControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            var container = FindAncestor<ContentPresenter>((DependencyObject)e.OriginalSource);
            _draggedItem = container?.Content;
        }

        private void ParallelItemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _draggedItem == null) return;
            var position = e.GetPosition(null);
            var diff = _dragStartPoint - position;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                DragDrop.DoDragDrop((DependencyObject)sender, _draggedItem, DragDropEffects.Move);
            }
        }

        private void ParallelItemsControl_Drop(object sender, DragEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            var items = itemsControl?.ItemsSource as ObservableCollection<string>;
            if (items == null) return;

            var droppedData = e.Data.GetData(typeof(string)) as string;
            if (droppedData == null) return;

            var target = GetParallelDataFromPoint(itemsControl, e.GetPosition(itemsControl));
            if (target == null || droppedData == target) return;

            int removeIndex = items.IndexOf(droppedData);
            int insertIndex = items.IndexOf(target);
            if (removeIndex < 0 || insertIndex < 0) return;

            items.Move(removeIndex, insertIndex);
        }

        private static string GetParallelDataFromPoint(ItemsControl itemsControl, Point point)
        {
            var element = itemsControl.InputHitTest(point) as DependencyObject;
            while (element != null)
            {
                if (element is ContentPresenter presenter && presenter.Content is string s)
                    return s;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        // Navigate selection

        // ========== Utility Helpers ==========
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T target)
                    return target;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private static object GetDataFromPoint(ItemsControl itemsControl, Point point)
        {
            var element = itemsControl.InputHitTest(point) as DependencyObject;
            while (element != null)
            {
                if (element is ContentPresenter presenter)
                    return presenter.Content;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }
        // Save/Load handlers
        private void SaveSteps_Click(object sender, RoutedEventArgs e)
        {
           
            var vm = DataContext as DispenseProtocolEditorViewModel;
            if (vm == null) return;

            var dialog = new SaveFileDialog
            {
                Title = "Save Dispense Sequence",
                Filter = "JSON Files (*.json)|*.json",
                FileName = "DispenseSequence.json"
            };
            if (dialog.ShowDialog() == true)
            {
                var json = JsonConvert.SerializeObject(vm.Sequences, Formatting.Indented);
                File.WriteAllText(dialog.FileName, json);
                MessageBox.Show("Sequence saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadSteps_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DispenseProtocolEditorViewModel;
            if (vm == null) return;
            
            var dialog = new OpenFileDialog
            {
                Title = "Load Dispense Sequence",
                Filter = "JSON Files (*.json)|*.json"
            };
            if (dialog.ShowDialog() == true)
            {
                var json = File.ReadAllText(dialog.FileName);
                var loaded = JsonConvert.DeserializeObject<ObservableCollection<Sequence>>(json);
                if (loaded != null)
                {
                    vm.Sequences.Clear();
                    foreach (var s in loaded)
                        vm.Sequences.Add(s);
                }
            }
        }
    }
}
