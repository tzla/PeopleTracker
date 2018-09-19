using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PeopleTracker
{
    /// <summary>
    /// This page shows detailed information on the press room employees.
    /// Edits, Additions, and Removals from the master list are done here. 
    /// </summary>
    public sealed partial class PeopleDetail : Page
    {
        List<List<MainPage.People>> PeopleOnPress = new List<List<MainPage.People>>();
        List<MainPage.People> PeopleList = new List<MainPage.People>();
        List<MainPage.People> LittleList = new List<MainPage.People>();
        MainPage.ReturnList sendMeBack = new MainPage.ReturnList();

        public ObservableCollection<string> FirstNamer = new ObservableCollection<string>();
        public ObservableCollection<string> LastNamer = new ObservableCollection<string>();
        public ObservableCollection<string> DisplayNamer = new ObservableCollection<string>();
        public ObservableCollection<string> HireDater = new ObservableCollection<string>();
        bool sort1 = true; bool sort2 = true; bool sort3 = true; bool sort4 = true; bool sorted = false;
        ListView thisLister = new ListView();
        List<string> Presses = new List<string>();


        /// <summary>
        /// This will initialize the page
        /// </summary>
        public PeopleDetail()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            Date_Hired.MinYear = DateTimeOffset.Parse("Jan 1, 1970");
            Date_Hired.MaxYear = DateTimeOffset.Now;
            Boxes.ItemsSource = Presses;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            sendMeBack = e.Parameter as MainPage.ReturnList;
            PeopleOnPress = sendMeBack.PeopleOnPress;
            LittleList = sendMeBack.PeopleList;
            /*foreach(MainPage.People persons in LittleList)
            {
                PeopleList.Add(persons);
            }
            foreach(List<MainPage.People> personList in PeopleOnPress)
            {
                foreach(MainPage.People persons in personList)
                {
                    PeopleList.Add(persons);
                }
            }*/
            Presses = sendMeBack.LineNameList.ToList();
            Boxes.ItemsSource = Presses;
            this.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
            Updater();
            
        }


        /// <summary>
        /// This takes the entered information and saves it to the master list
        /// </summary>
        private void SaveEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                PeopleList[thisLister.SelectedIndex].DisplayName = DisplayName1.Text;
                PeopleList[thisLister.SelectedIndex].FirstName = First_Name1.Text;
                PeopleList[thisLister.SelectedIndex].LastName = Last_Name1.Text;
                PeopleList[thisLister.SelectedIndex].IsOperator = OpCheck1.IsChecked;
                PeopleList[thisLister.SelectedIndex].Hired = (Date_Hired.Date);
            }
            catch { }
            PeopleList.Clear();
            Updater();
        }

        private void BackToMain(object sender, RoutedEventArgs e)
        {
            sendMeBack.PeopleList = LittleList;
            sendMeBack.PeopleOnPress = PeopleOnPress;
            this.Frame.Navigate(typeof(MainPage),sendMeBack);
        }

        private async void SavePeople()
        {
            string json = JsonConvert.SerializeObject(LittleList);
            StorageFolder storageFolder = KnownFolders.MusicLibrary;
            StorageFile newFile = await storageFolder.CreateFileAsync("PeopleList.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(newFile, json);
            json = JsonConvert.SerializeObject(PeopleOnPress);
            newFile = await storageFolder.CreateFileAsync("PeopleOnPress.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(newFile, json);
        }


        /// <summary>
        /// This parses the master data list into child lists for display and manipulation
        /// </summary>
        private void Updater()
        {
            if (!sorted)
            {
                foreach (MainPage.People persons in LittleList)
                {
                    PeopleList.Add(persons);
                }
                foreach (List<MainPage.People> personList in PeopleOnPress)
                {
                    foreach (MainPage.People persons in personList)
                    {
                        PeopleList.Add(persons);
                    }
                }
            }
            HireDater.Clear(); FirstNamer.Clear(); LastNamer.Clear(); DisplayNamer.Clear();
            foreach (MainPage.People people in PeopleList)
            {
                try { HireDater.Add(people.Hired.ToString("MM-dd-yyyy")); } catch { }
                try { FirstNamer.Add(people.FirstName); } catch { }
                try { LastNamer.Add(people.LastName); } catch { }
                try { DisplayNamer.Add(people.DisplayName); } catch { }
            }
            BigFirstNameList.ItemsSource = FirstNamer;
            BigLastNameList.ItemsSource = LastNamer;
            BigDisplayNameList.ItemsSource = DisplayNamer;
            BigHireDateList.ItemsSource = HireDater;
            B1.Height = B2.Height = B3.Height = B4.Height = PeopleList.Count * 45;
            this.UpdateLayout();
            SavePeople();
        }

        private void DisplaySorter(object sender, RoutedEventArgs e)
        {
            sorted = true;
            if (sort1)
            {
                PeopleList = PeopleList.OrderByDescending(x => x.DisplayName).ToList();
                sort1 = false;
                DisplayNameSort.Content = "↑↓";
            }
            else
            {
                PeopleList = PeopleList.OrderBy(x => x.DisplayName).ToList();
                sort1 = true;
                DisplayNameSort.Content = "↓↑";
            }
            Updater();
            sorted = false;
        }
        private void FirstNameSorter(object sender, RoutedEventArgs e)
        {
            sorted = true;
            if (sort2)
            {
                PeopleList = PeopleList.OrderByDescending(x => x.FirstName).ToList();
                sort2 = false;
                FirstNameSort.Content = "↑↓";
            }
            else
            {
                PeopleList = PeopleList.OrderBy(x => x.FirstName).ToList();
                sort2 = true;
                FirstNameSort.Content = "↓↑";
            }
            Updater();
            sorted = false;
        }
        private void LastNameSorter(object sender, RoutedEventArgs e)
        {
            sorted = true;
            if (sort3)
            {
                PeopleList = PeopleList.OrderByDescending(x => x.LastName).ToList();
                sort3 = false;
                LastNameSort.Content = "↑↓";
            }
            else
            {
                PeopleList = PeopleList.OrderBy(x => x.LastName).ToList();
                sort3 = true;
                LastNameSort.Content = "↓↑";
            }
            Updater();
            sorted = false;
        }
        private void DateSorter(object sender, RoutedEventArgs e)
        {
            sorted = true;
            if (sort4)
            {
                PeopleList = PeopleList.OrderByDescending(x => x.Hired).ToList();
                sort4 = false;
                HireDateSort.Content = "↑↓";
            }
            else
            {
                PeopleList = PeopleList.OrderBy(x => x.Hired).ToList();
                sort4 = true;
                HireDateSort.Content = "↓↑";
            }
            Updater();
            sorted = false;
        }

        private void SelectedIndexing(object sender, SelectionChangedEventArgs e)
        {
            thisLister = sender as ListView;
            BigDisplayNameList.SelectedIndex = BigFirstNameList.SelectedIndex = BigHireDateList.SelectedIndex = BigLastNameList.SelectedIndex = thisLister.SelectedIndex;
            try
            {
                DisplayName1.Text = PeopleList[thisLister.SelectedIndex].DisplayName;
                First_Name1.Text = PeopleList[thisLister.SelectedIndex].FirstName;
                Last_Name1.Text = PeopleList[thisLister.SelectedIndex].LastName;
                OpCheck1.IsChecked = PeopleList[thisLister.SelectedIndex].IsOperator;
                Date_Hired.Date = PeopleList[thisLister.SelectedIndex].Hired;
            }
            catch { }
        }

        private void Boxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<MainPage.People> ThisLineList = new List<MainPage.People>();
            ThisLineList = PeopleOnPress[Boxes.SelectedIndex];
            List<string> ThisLineListDisplay = new List<string>();
            foreach(MainPage.People thisPerson in ThisLineList)
            {
                ThisLineListDisplay.Add(thisPerson.DisplayName);
            }
            try { SelectedPressList.ItemsSource = ThisLineListDisplay; } catch { }
        }

        private async void AddPerson(object sender, RoutedEventArgs e)
        {
            AddDialog addPerson = new AddDialog();
            ContentDialogResult result = await addPerson.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                MainPage.People newPerson = addPerson.GetNewPerson();
                LittleList.Add(newPerson);
                PeopleList.Clear();
                Updater();
            }
        }
    }
}
