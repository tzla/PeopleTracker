using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PeopleTracker
{
    public sealed partial class ListDialog : ContentDialog
    {
        public ObservableCollection<string> Roster = new ObservableCollection<string> { "butt" };
        public ObservableCollection<string> holdMe = new ObservableCollection<string>();

        public ListDialog()
        {
            this.InitializeComponent();
            HaHa.ItemsSource = Roster;
        }
        
        public void GetRoster(ObservableCollection<string> getRoster)
        {
            Roster = getRoster;
        }
        
        private void DragStart(object sender, DragItemsStartingEventArgs e)
        {
            holdMe = new ObservableCollection<string>();
            var items = new StringBuilder();
            foreach (var item in e.Items)
            {
                if (items.Length > 0) items.AppendLine();
                items.Append(item as string);
            }
            // Set the content of the DataPackage
            e.Data.SetText(items.ToString());
           
            // As we want our Reference list to say intact, we only allow Copy
            e.Data.RequestedOperation = DataPackageOperation.Move;
            
        }

        private async void DropDelete(object sender, DragEventArgs e)
        {
            Trash.Background = new SolidColorBrush(Windows.UI.Colors.LightSteelBlue);
           // Roster.Add("Poop");
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                // We need to take a Deferral as we won't be able to confirm the end
                // of the operation synchronously
                var def = e.GetDeferral();
                var s = await e.DataView.GetTextAsync();
                var items = s.Split('\n');
                foreach (var item in items)
                {
                    Roster = HaHa.ItemsSource as ObservableCollection<string>;
                    Roster.Remove(item);
                }
                e.AcceptedOperation = DataPackageOperation.Move;
                def.Complete();
            }
        }

        private void DropOver(object sender, DragEventArgs e)
        {
            Trash.Background = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            Trash.BorderBrush = new SolidColorBrush(Windows.UI.Colors.SlateBlue);
            e.AcceptedOperation = (e.DataView.Contains(StandardDataFormats.Text) ? DataPackageOperation.Move : DataPackageOperation.None);
            e.DragUIOverride.IsGlyphVisible = false;
            String person = holdMe.ToString();
            e.DragUIOverride.Caption = "Remove " + person ;
        }

        private new void DragLeave (object sender, DragEventArgs e)
        {
            Trash.Background = new SolidColorBrush(Windows.UI.Colors.LightSteelBlue);
            Trash.BorderBrush = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
        }

        private void DropLeave(object sender, DragEventArgs e)
        {
            Trash.Background = new SolidColorBrush(Windows.UI.Colors.LightSteelBlue);
            Trash.BorderBrush = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
        }

        private async void AddPerson(object sender, RoutedEventArgs e)
        {
            this.Hide();
            
            string text = await InputTextDialogAsync("Add Person");
            await this.ShowAsync();
        }

        private async Task<string> InputTextDialogAsync(string title)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 38;
            inputTextBox.Background = new SolidColorBrush(Windows.UI.Colors.LightSteelBlue);
            inputTextBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            inputTextBox.BorderThickness = new Thickness(3, 3, 3, 3);
            inputTextBox.PlaceholderText = "Enter Name Here";
            inputTextBox.PlaceholderForeground = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                string text = inputTextBox.Text;
                if (text != "" && text != null)
                {
                    Roster = HaHa.ItemsSource as ObservableCollection<string>;
                    Roster.Add(text);
                }
                return text;
            }
            else
                return "";
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            holdMe = Roster;
            this.Hide();
        }
    }
}
