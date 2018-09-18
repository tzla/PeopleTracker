using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class PressDialog : ContentDialog
    {
        List<int> sizeChoices = new List<int> { 0, 1, 2, 3, 4, 5 };
        public PressDialog()
        {
            this.InitializeComponent();
            NumberBox.ItemsSource = sizeChoices;
        }
        public void SetCurrentSize(int size)
        {
            NumberBox.SelectedIndex = size;
        }
        public void PressBox(string LineName)
        {
            NameBox.Text = LineName;
        }
        public string GetPressBox()
        {
            return NameBox.Text;
        }
        public int getNewSize()
        {
            return NumberBox.SelectedIndex;
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
