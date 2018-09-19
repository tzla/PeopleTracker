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
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Newtonsoft.Json;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PeopleTracker
{
    /// <summary>
    /// This is main page 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //UI constructors
        public static Thickness Border3 = new Thickness { Left = 3, Right = 3, Top = 3, Bottom = 3 };
        public static Thickness Border1 = new Thickness { Left = 1, Right = 1, Top = 1, Bottom = 1 };
        public static Thickness Padding0 = new Thickness { Left = 0, Right = 0, Top = 0, Bottom = 0 };
        public static CornerRadius Radius5 = new CornerRadius { BottomLeft = 5, BottomRight = 5, TopLeft = 5, TopRight = 5 };
        public static Thickness Margins = new Thickness { Left = 10, Right = 10 };
        public static Thickness Margins2 = new Thickness { Left = 5, Right = 5 };
        public static Thickness RightMargin = new Thickness { Right = 5, Left = 5 };
        public static Thickness VerticalMargin = new Thickness { Top = 5, Bottom = 5 };
        public static SolidColorBrush SB = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
        public static SolidColorBrush LSB = new SolidColorBrush(Windows.UI.Colors.LightSteelBlue);
        public static SolidColorBrush red = new SolidColorBrush(Windows.UI.Colors.Red);
        public static SolidColorBrush SLB = new SolidColorBrush(Windows.UI.Colors.SlateBlue);

        //UI LISTS
        List<Grid> myPressGrids = new List<Grid>();//A list of each press grids for UI control
        List<ListView> pressLists = new List<ListView>();// list of diplaylists for each press grid for binds and UI conttrol
        List<ListView> setterLists = new List<ListView>();//die setter UI lists
        List<Button> resetLists = new List<Button>();//whole press grid reset list for UI control
        List<Border> borderList = new List<Border>();//list of borders to control UI for grids
        List<List<Button>> singleResetList = new List<List<Button>>();//list of X remove buttons for each press
        List<Button> dieSetterResetList = new List<Button>();//x remove buttons for diesetters
        List<List<TextBlock>> pressLabels = new List<List<TextBlock>>();//labels for each press
        List<TextBlock> lineTitleBlocks = new List<TextBlock>(); //list for grid titles UI
        
        //Master Lists
        public List<People> PeopleList = new List<People>();//master source list
        public List<List<People>> PeopleOnPress = new List<List<People>>();//master press lists

        //source lists
        ObservableCollection<string> lineNames = new ObservableCollection<string>();//source list grid titles
        ObservableCollection<int> lineSizes = new ObservableCollection<int>();//source list for size of each line
        List<ObservableCollection<string>> dieSetterPlacer = new List<ObservableCollection<string>>();//source grid list for dieSetters
        List<ObservableCollection<string>> peoplePlacer = new List<ObservableCollection<string>>();//source grid list for operators 
        ObservableCollection<string> operatorSource = new ObservableCollection<string>(); //source for operator roster list
        ObservableCollection<string> setterSource = new ObservableCollection<string>();//source for diesetter roster list


        //int gridNumber = 0;
        bool first = true; //after loading, set to false 
        bool sorter1 = true; bool sorter2 = true; //sorting state of the two source lists
        bool Load = false; //prevents loading multiple times

        int lastIndex;//tracks the most recent edited list
        int dragItemLineTracker;
        int dragItemPressTracker;

        //these are list initiators(temporary)
        ObservableCollection<string> ass = new ObservableCollection<string>();
        ObservableCollection<string> ass2 = new ObservableCollection<string>();
        ObservableCollection<string> ass3 = new ObservableCollection<string>();
        ObservableCollection<string> ass4 = new ObservableCollection<string>();
        ObservableCollection<string> ass5 = new ObservableCollection<string>();
        ObservableCollection<string> anus = new ObservableCollection<string> { "Dolby" };
        ObservableCollection<string> anus2 = new ObservableCollection<string> { "Fink" };
        ObservableCollection<string> anus3 = new ObservableCollection<string> { "Phrank" };

        Style gridStyle = new Style();//saves the style for grid objects
        Grid newGrid;//currently focused grid loads here for easy manipulation

        /// <summary>
        /// Packages the info on line sizes and names for saving and loading
        /// </summary>
        public class SaveData
        {
            public ObservableCollection<string> MyLineNames { get; set; }
            public ObservableCollection<int> MyLineSizes { get; set; }
        }

        /// <summary>
        /// Package for tracking daily press operation. 
        /// Each Presser instance represents one day on one press for a single part
        /// </summary>
        public class Presser
        {
            DateTime RunDate { get; set; }//day
            string LineNumber { get; set; }//Line
            string PressNumber { get; set; }//press number
            string PartNo { get; set; }//what parts were made
            int PartCount { get; set; }//how many were made
        }

        /// <summary>
        /// Package for individual press operators. 
        /// This allows for roster display, as well as performance tracking. 
        /// </summary>
        public class People
        {
            public string DisplayName { get; set; }//preferred display name
            public string FirstName { get; set; }//first name
            public string LastName { get; set; }//last name
            public bool? IsOperator { get; set; }//are they operator? maybe irrevelant 
            public List<Presser> PastRun { get; set; }//list of past press runs
            public DateTimeOffset Hired { get; set; }//when were they hired
        }

        /// <summary>
        /// Package for data operations in child pages
        /// </summary>
        public class ReturnList
        {
            public List<People> PeopleList  { get; set; }//master source list
            public List<List<People>> PeopleOnPress { get; set; }//master press lists 
            public ObservableCollection<string> LineNameList { get; set; }
        }

        /// <summary>
        /// Main Initialization Method
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();//loads UI
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;//caches page when program moves away
            LoadFromFile();//loads data from saved files

            //name,length,column,row,index,source,DSsource
            //temporary?? setup
           

            this.UpdateLayout();//updates UI after building grids
            //stops startup state

            Sources.ItemsSource = operatorSource;//binds the source lists 
            DSources.ItemsSource = setterSource;
        }//initialilzer

        /// <summary>
        /// Retrieves data from child pages when returning to main page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Load)
            {
                base.OnNavigatedTo(e);
                ReturnList sendMeBack = e.Parameter as MainPage.ReturnList;
                PeopleOnPress = sendMeBack.PeopleOnPress;
                PeopleList = sendMeBack.PeopleList;
                UpdateLists();
            }
        }

        private void SendToSetup()
        {
            UpdateLists();
            Load = true;
            GridSetup(0, 0, 0, ass3, anus);
            GridSetup(1, 0, 1, ass, anus2);
            GridSetup(2, 0, 2, ass3, anus3);
            GridSetup(3, 0, 3, ass4, anus3);
            GridSetup(4, 0, 4, ass5, anus2);
            GridSetup(0, 1, 5, ass5, anus2);
            GridSetup(1, 1, 6, ass5, anus2);
            GridSetup(2, 1, 7, ass5, anus2);
            GridSetup(3, 1, 8, ass5, anus2);
            GridSetup(4, 1, 9, ass5, anus2);
            GridSetup(0, 2, 10, ass5, anus2);
            GridSetup(1, 2, 11, ass5, anus2);
            GridSetup(2, 2, 12, ass5, anus2);
            GridSetup(3, 2, 13, ass5, anus2);
            GridSetup(4, 2, 14, ass5, anus2);
            first = false;
        }

        private async void LoadFromFile()
        {
           try
            {
                if (!Load)
                {
                    String JsonFile = "PeopleList.json";
                    StorageFolder localFolder = KnownFolders.MusicLibrary;
                    StorageFile localFile = await localFolder.GetFileAsync(JsonFile);
                    String JsonString = await FileIO.ReadTextAsync(localFile);
                    List<People> cereal = JsonConvert.DeserializeObject(JsonString, typeof(List<People>)) as List<People>; //loads the saved data file
                    try
                    {
                        JsonFile = "PeopleOnPress.json";
                        localFile = await localFolder.GetFileAsync(JsonFile);
                        String peopler = await FileIO.ReadTextAsync(localFile);
                        PeopleOnPress = JsonConvert.DeserializeObject(peopler, typeof(List<List<People>>)) as List<List<People>>;
                    }
                    catch { PeopleOnPress = new List<List<People>>(); }
                    //JsonConvert.d
                    try
                    {
                        for (int y = 0; y < cereal.Count; y++) //decodes the saved file
                        {
                            People Persons = cereal[y];
                            /*Persons.DisplayName = cereal[y, 0]; Persons.FirstName = cereal[y, 1];
                            Persons.LastName = cereal[y, 2]; Persons.isOperator = Convert.ToBoolean(cereal[y, 3]);
                            Persons.hired = (Convert.ToDateTime(cereal[y, 4]));*/
                            PeopleList.Add(Persons);
                            //operatorSource.Add(Persons.DisplayName);
                        }
                    }
                    catch { }
                    JsonFile = "LineData.json";
                    localFile = await localFolder.GetFileAsync(JsonFile);
                    JsonString = await FileIO.ReadTextAsync(localFile);
                    SaveData loadData = JsonConvert.DeserializeObject(JsonString, typeof(SaveData)) as SaveData;//loads the saved data file
                    lineNames = loadData.MyLineNames;
                    lineSizes = loadData.MyLineSizes;
                    
                    
                }
            }
            catch { }
            if (PeopleOnPress == null)
            {
                PeopleOnPress = new List<List<People>>();
                for (int i = 0; i < lineSizes.Count; i++)
                {
                    List<People> newList = new List<People>();
                    PeopleOnPress.Add(newList);
                }
            }
            SendToSetup();
                
        }

        private async Task Saver()
        {
            string json = JsonConvert.SerializeObject(PeopleOnPress); 
            StorageFolder storageFolder = KnownFolders.MusicLibrary;
            StorageFile newFile = await storageFolder.CreateFileAsync("PeopleOnPress.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(newFile, json);

            SaveData thisData = new SaveData
            {
                MyLineNames = lineNames,
                MyLineSizes = lineSizes
            };
            json = JsonConvert.SerializeObject(thisData);
            newFile = await storageFolder.CreateFileAsync("LineData.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(newFile, json);

            json = JsonConvert.SerializeObject(PeopleList);
            newFile = await storageFolder.CreateFileAsync("PeopleList.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(newFile, json);

        }

        private Border InspecterMaker(int gridNumber)
        {
            Border inspectBorder = new Border
            {
                Name = pressLists.Count.ToString() + "_inspect",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                BorderBrush = SB,
                Background = LSB,
                BorderThickness = Border3,
                CornerRadius = Radius5,
                Width = 25,
                Height = 25
            };

            if (first) { borderList.Add(inspectBorder); }
            else { borderList[gridNumber] = inspectBorder; }

            string ImagesPath = "ms-appx:///Assets/mager.png";
            Uri uri = new Uri(ImagesPath, UriKind.RelativeOrAbsolute);
            BitmapImage bitmap = new BitmapImage(uri);
            Image iconic = new Image
            {
                Name = gridNumber.ToString() + "_Icon",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Fill,
                Width = 18,
                Height = 18,
                Source = bitmap
            };
            iconic.Tapped += ViewPress;
            iconic.PointerEntered += IconEnter;
            iconic.PointerExited += IconExit;
            inspectBorder.Child = iconic;
            return inspectBorder;

        }//makes magnifying glass buttons

        private void IconEnter(object sender, PointerRoutedEventArgs e)
        {
            Image senderButton = sender as Image;
            String buttonName = senderButton.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[0]);
            borderList[pressIndex].Background = SB;
            borderList[pressIndex].BorderBrush = SLB;
        }//highlights the focused icon

        private void IconExit(object sender, PointerRoutedEventArgs e)
        {
            Image senderButton = sender as Image;
            String buttonName = senderButton.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[0]);
            borderList[pressIndex].Background = LSB;
            borderList[pressIndex].BorderBrush = SB;
        }//unhighlights the focused icon

        private void TitleRowSetup(int gridNumber)
        {
            RowDefinition titleRow = new RowDefinition { Height = new GridLength(40.0, GridUnitType.Pixel) };
            newGrid.RowDefinitions.Add(titleRow);
            TextBlock titleBlock = new TextBlock
            {
                Text = newGrid.Name,
                Height = 40,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(titleBlock, 0);
            newGrid.Children.Add(titleBlock);
            Border thisInspecter = InspecterMaker(gridNumber);
            Grid.SetColumn(thisInspecter, 1);
            newGrid.Children.Add(thisInspecter);
            lineTitleBlocks.Add(titleBlock);
        }//sets up title row, add view button

        private void DefineColumns()
        {
            ColumnDefinition col1 = new ColumnDefinition { Width = new GridLength(65.0, GridUnitType.Pixel) };
            newGrid.ColumnDefinitions.Add(col1);
            ColumnDefinition col2 = new ColumnDefinition
            {
                Width = new GridLength(125.0, GridUnitType.Pixel),
                MinWidth = 100
            };
            newGrid.ColumnDefinitions.Add(col2);
            ColumnDefinition col3 = new ColumnDefinition { Width = new GridLength(25, GridUnitType.Pixel) };
            newGrid.ColumnDefinitions.Add(col3);
        }//defines standard columns for each line object

        private void DefineRows(int pressSize)
        {
            RowDefinition newRow = new RowDefinition();
            for (int i = 0; i < pressSize; i++)
            {
                newRow = new RowDefinition {Height = new GridLength(25.0, GridUnitType.Pixel)}; //press rows
                newGrid.RowDefinitions.Add(newRow);
            }

            newRow = new RowDefinition { Height = new GridLength(30.0, GridUnitType.Pixel) }; //die setter row
            newGrid.RowDefinitions.Add(newRow);

            newRow = new RowDefinition {Height = new GridLength(35.0, GridUnitType.Pixel)}; //reset button row
            newGrid.RowDefinitions.Add(newRow);
        }//defines standard rows for each line object

        private void SetupLabelColumn(int pressSize)
        {
            List<TextBlock> thisLabelList = new List<TextBlock>();
            for (int i = 0; i < pressSize + 1; i++)
            {
                if (i <= pressSize - 1)
                {
                    TextBlock NewBlock = new TextBlock
                    {
                        Text = "Press " + (i + 1).ToString(),
                        Height = 25,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetRow(NewBlock, i + 1);
                    Grid.SetColumn(NewBlock, 0);
                    newGrid.Children.Add(NewBlock);
                    thisLabelList.Add(NewBlock);
                }
                else if (i == pressSize)
                {
                    TextBlock NewBlock = new TextBlock { Text = "Die Setr.", Height = 25, VerticalAlignment = VerticalAlignment.Center };
                    Grid.SetRow(NewBlock, i + 1);
                    Grid.SetColumn(NewBlock, 0);
                    newGrid.Children.Add(NewBlock);
                }
            }
            pressLabels.Add(thisLabelList);
        }//places labels for the lists

        private Button SetupResetButton(int pressSize,int gridNumber)
        {
            Button resetButton = new Button
            {
                Content = "Reset",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Name = gridNumber.ToString() + "_Button",
                Height = 30,
                MinHeight = 30,
                MaxHeight = 30,
                FontSize = 10,
            };
            Canvas.SetZIndex(resetButton, 2);
            resetButton.Click += ResetClick;
            Grid.SetRow(resetButton, pressSize + 2);
            Grid.SetColumn(resetButton, 0);
            Grid.SetColumnSpan(resetButton, 3);
            return resetButton;
        }//generates each reset button for lists

        private ListView[] ListMaker(int gridNumber, int pressSize)
        {
            ListView newList = new ListView
            {
                Name = "Targets_" + gridNumber.ToString(),
                VerticalAlignment = VerticalAlignment.Top,
                Background = LSB,
                AllowDrop = true,
                CanReorderItems = true,
                CanDragItems = true,
                MinHeight = 25 * pressSize
            };
            
            ListView dieSetterList = new ListView
            {
                Name = "Targets_" + gridNumber.ToString() + "_DS",
                VerticalAlignment = VerticalAlignment.Center,
                Background = LSB,
                AllowDrop = true,
                CanReorderItems = true,
                CanDragItems = true,
                MinHeight = 25,
                Margin = VerticalMargin

            };
            newList.DragOver += TargetDragOver;
            newList.DragLeave += TargetDragLeave;
            newList.Drop += TargetDrop;
            newList.PointerExited += ResetSelection;
            newList.DragItemsCompleted += DragTargetDone;
            newList.DragItemsStarting += DragTargetStart;
            
            Style gridStyle = new Style();
            Setter thisSet = new Setter { Property = HeightProperty, Value = 25 };
            Setter thisSet2 = new Setter { Property = MinHeightProperty, Value = 25 };
            Setter thisSet3 = new Setter { Property = VerticalAlignmentProperty, Value = VerticalAlignment.Top };
            gridStyle.Setters.Add(thisSet);
            gridStyle.Setters.Add(thisSet2);
            gridStyle.Setters.Add(thisSet3);
            gridStyle.TargetType = typeof(ListViewItem);
            newList.ItemContainerStyle = gridStyle;
            dieSetterList.Style = newList.Style;
            dieSetterList.ItemContainerStyle = gridStyle;

            ListView[] newLists = { newList, dieSetterList };
            return newLists;
        }//generates the list for each press

        private void Loader(int gridNumber)
        {
            if (first)
            {
                try
                {
                    List<People> ThisPressList = new List<People>();
                    PeopleOnPress.Add(ThisPressList);
                    peoplePlacer.Add(new ObservableCollection<string> { });
                    dieSetterPlacer.Add(new ObservableCollection<string> { });
                }
                catch { }
            }
            else
            {
                peoplePlacer[gridNumber] = new ObservableCollection<string> { };
                dieSetterPlacer[gridNumber] = new ObservableCollection<string> { };
            }
        }//initializes lists for grid setup

        private async void UpdateLists()
        {
            if(!first)//temporary measure to trim excess off list until cause is fixed
            {
                try { PeopleOnPress.RemoveRange(15, PeopleOnPress.Count - 15); } catch { }
            }


            ObservableCollection<string> updateOpList = new ObservableCollection<string>();
            List<ObservableCollection<string>> updatePressList = new List<ObservableCollection<string>>();
            foreach (People thisPerson in PeopleList)
            {
                updateOpList.Add(thisPerson.DisplayName);
            }
            operatorSource = updateOpList;
            int track = 0;
            try
            {
                foreach (List<People> pressOpList in PeopleOnPress)
                {
                    ObservableCollection<string> singleUpdateList = new ObservableCollection<string>();
                    if (!Load) { peoplePlacer.Add(new ObservableCollection<string>()); }
                    foreach (People thisOp in pressOpList)
                    {
                        singleUpdateList.Add(thisOp.DisplayName);
                    }
                    peoplePlacer[track] = singleUpdateList;
                    track++;
                }
            }
            catch { }
            Sources.ItemsSource = operatorSource;
            track = 0;
            foreach(ListView thisView in pressLists)
            {
                thisView.ItemsSource = peoplePlacer[track];
                track++;
            }
            if (!first)
            {
                Saver();
            }
            

        }//updates the lists

        private List<Button> LittleButtonMaker(int gridNumber, int pressSize, int thisLineCount)
        {
            List<Button> thisButtonLine = new List<Button>();
            for (int i = 0; i < pressSize; i++)
            {

                Button thisResetButton = new Button
                {
                    Name = gridNumber.ToString() + "_" + i.ToString(),
                    Height = 20,
                    Content = "X",
                    FontSize = 10,
                    Padding = Padding0,
                    Margin = RightMargin,
                    Foreground = red
                };
                thisResetButton.Click += SingleReset;
                Grid.SetRow(thisResetButton, i + 1);
                Grid.SetColumn(thisResetButton, 2);
                newGrid.Children.Add(thisResetButton);
                thisButtonLine.Add(thisResetButton);
                if (i > thisLineCount - 1)
                {
                    thisResetButton.IsEnabled = false;
                }
            }
            return thisButtonLine;
        } //makes individual reset x's

        public void GridSetup(int rowPos, int colPos,int gridNumber, ObservableCollection<string> items, ObservableCollection<string> dieSetter)
        {
            string gridName = lineNames[gridNumber];
            int pressSize = lineSizes[gridNumber];
            ObservableCollection<string> thisLine = new ObservableCollection<string>();
            try
            {
                items = peoplePlacer[gridNumber];
            }
            catch{ }
            newGrid = new Grid
            {
                Name = gridName,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                BorderThickness = Border3,
                CornerRadius = Radius5,
                BorderBrush = SB,
                MaxHeight = 250,
                //Background = LSB,
                Margin = Margins2
            };
            
            TitleRowSetup(gridNumber);
            DefineColumns();
            DefineRows(pressSize);
            SetupLabelColumn(pressSize);
            Loader(gridNumber);
            ListView[] newLists =  ListMaker(gridNumber,pressSize);
            ListView newList = newLists[0];
            ListView dieSetterList = newLists[1];
            try
            {
                dieSetterPlacer[gridNumber] = dieSetter;
                dieSetterList.ItemsSource = dieSetterPlacer[gridNumber];
            }
            catch { }

            Grid.SetColumn(dieSetterList, 1);
            Grid.SetRow(dieSetterList, pressSize + 1);

            if (first){setterLists.Add(dieSetterList);}
            else{setterLists[gridNumber] = dieSetterList;}
            newGrid.Children.Add(dieSetterList);

            Grid.SetColumn(newList, 1);
            Grid.SetRow(newList, 1);
            if (pressSize > 0) { Grid.SetRowSpan(newList, pressSize); }

            for (int i = 0; i < pressSize; i++)
            {
                try { thisLine.Add(items[i]); }
                catch { }
            }
            //items!!!!!!!!!!!!!!!!!!!!!!!!!!!!;
            
            if (first)
            { 
                pressLists.Add(newList);
            }
            else
            {
                pressLists[gridNumber] = newList;
            }
            try
            {
                peoplePlacer[gridNumber] = thisLine;
           
            newList.ItemsSource = peoplePlacer[gridNumber]; // good item source
            }
            catch { }


            if (pressSize > 0) { newGrid.Children.Add(newList); }
            Button resetButton = SetupResetButton(pressSize, gridNumber);

            if (first) { resetLists.Add(resetButton); }
            else { resetLists[gridNumber] = resetButton; }
             newGrid.Children.Add(resetButton);

            List<Button> thisButtonLine = LittleButtonMaker(gridNumber, pressSize, peoplePlacer[gridNumber].Count);


            if (first) { singleResetList.Add(thisButtonLine); }
            else { singleResetList[gridNumber] = thisButtonLine; }
            
            Button thisResetButton2 = new Button
            {
                Name = gridNumber.ToString() + "_" + pressSize.ToString() + "DS",
                Height = 20,
                Content = "X",
                FontSize = 10,
                Padding = Padding0,
                Margin = RightMargin,
                Foreground = red
            };
            thisResetButton2.Click += SingleResetDS;

            Grid.SetRow(thisResetButton2, pressSize + 1);
            Grid.SetColumn(thisResetButton2, 2);
            newGrid.Children.Add(thisResetButton2);

            if (first) { dieSetterResetList.Add(thisResetButton2); }
            else { dieSetterResetList[gridNumber] = thisResetButton2; }

            ColumnDefinition col = new ColumnDefinition
            {
                MinWidth = 200,
                Width = new GridLength(230, GridUnitType.Pixel)
            };
            if (first)
            {
                for (int x = 0; x < 3; x++)
                {
                    RowDefinition roe = new RowDefinition
                    {
                        Height = new GridLength(250, GridUnitType.Pixel),
                        MinHeight = 250
                    };
                    piss.RowDefinitions.Add(roe);
                }
            }
            piss.ColumnDefinitions.Add(col);
            Grid.SetColumn(newGrid, rowPos);
            Grid.SetRow(newGrid, colPos);
            if (first) { myPressGrids.Add(newGrid); }
            else { myPressGrids[gridNumber] = newGrid;}
            piss.Children.Add(newGrid);
            //Saver();
            //gridNumber++;
        }//constructs grids for each press

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            String buttonName = senderButton.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[0]);
            List<People> thisGridRoster = PeopleOnPress[pressIndex];
            foreach (People gridPeople in thisGridRoster)
            {
                PeopleList.Add(gridPeople);
               //operatorSource.Add(thisperson);
            }
            ObservableCollection<string> resetDieSetterList = setterLists[pressIndex].ItemsSource as ObservableCollection<string>;
            try { setterSource.Add(resetDieSetterList[0]); } catch { }
            foreach (Button thisButton in singleResetList[pressIndex])
            {
                thisButton.IsEnabled = false;
            }
            dieSetterResetList[pressIndex].IsEnabled = false;
            resetDieSetterList.Clear();
            thisGridRoster.Clear();
            PeopleOnPress[pressIndex] = thisGridRoster;
            lastIndex = pressIndex;
            UpdateLists();
            //add mover to orginal list here

        }//big reset button

        private void SingleReset(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            String buttonName = senderButton.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[0]);
            int specificPress = Convert.ToInt16(nameArray[1]);
            List<People> thisLinePeople = PeopleOnPress[pressIndex];
            People removedPerson = thisLinePeople[specificPress];
            PeopleList.Add(removedPerson);
            thisLinePeople.Remove(removedPerson);
            PeopleOnPress[pressIndex] = thisLinePeople;
            List<Button> thisButtonList = singleResetList[pressIndex];
            thisButtonList[peoplePlacer[pressIndex].Count-1].IsEnabled = false;
            lastIndex = pressIndex;
            UpdateLists();
        }//individual x's

        private void SingleResetDS(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            String buttonName = senderButton.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[0]);
            ObservableCollection<string> resetList = setterLists[pressIndex].ItemsSource as ObservableCollection<string>;
            setterSource.Add(resetList[0]);
            resetList[0] = null;
            //setterLists[pressIndex] = null;

            //resetList.RemoveAt(specificPress);

            dieSetterResetList[pressIndex].IsEnabled = false;

        }//individual x's for die setters

        private async void ViewPress(object sender, TappedRoutedEventArgs e)
        {
            Image senderButton = sender as Image;
            String buttonName = senderButton.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[0]);
            cocker.Text = nameArray[0] + " : " + nameArray[1];
            PressDialog PressDialog = new PressDialog {};
            PressDialog.SetCurrentSize(lineSizes[pressIndex]);
            PressDialog.PressBox(lineNames[pressIndex]);
            PressDialog.Width = 800;
            ContentDialogResult result = await PressDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                lineNames[pressIndex] = PressDialog.GetPressBox();
                int newSize = PressDialog.GetNewSize();
                lineTitleBlocks[pressIndex].Text = lineNames[pressIndex];
                AdjustGrid(pressIndex, newSize);
                lineSizes[pressIndex] = newSize;
            }
            ObservableCollection<string> thisList = peoplePlacer[pressIndex];
            borderList[pressIndex].Background = LSB;
            borderList[pressIndex].BorderBrush = SB;
            //Saver();
        }//mag glass button

        private void DragStart(object sender, DragItemsStartingEventArgs e)
        {
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
            Sources.Background = SB;
            foreach (ListView thisList in setterLists)
            {
                thisList.AllowDrop = false;
                thisList.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
            }
            //mainSource = true;
        }//drag from source

        private void DragStop(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            Sources.Background = LSB;
            foreach (ListView thisList in setterLists)
            {
                thisList.AllowDrop = true;
                thisList.Background = LSB;
            }
        }//stop drag from source

        private void Sorter1(object sender, RoutedEventArgs e)
        {
            if (sorter1)
            {
                SortButton1.Content = "↑↓";
                sorter1 = false;
                PeopleList = PeopleList.OrderBy(x => x.DisplayName).ToList();
                
            }
            else
            {
                SortButton1.Content = "↓↑";
                sorter1 = true;
                PeopleList = PeopleList.OrderByDescending(x => x.DisplayName).ToList();
            }
            UpdateLists();
        } //sort source

        private void Sorter2(object sender, RoutedEventArgs e)
        {
            if (sorter2)
            {
                SortButton2.Content = "↑↓";
                sorter2 = false;
                ObservableCollection<string> _referenceNew2 = new ObservableCollection<string>(from i in setterSource orderby i select i);
                setterSource = _referenceNew2;
                DSources.ItemsSource = setterSource;
            }
            else
            {
                SortButton2.Content = "↓↑";
                sorter2 = true;
                ObservableCollection<string> _referenceNew2 = new ObservableCollection<string>(from i in setterSource orderby i descending select i);
                setterSource = _referenceNew2;
                DSources.ItemsSource = setterSource;
            }
        } //sort diesetter list

        private void AdjustGrid(int pressIndex, int newSize)
        {
            Grid adjustGrid = myPressGrids[pressIndex];
            List<TextBlock> theseLabels = pressLabels[pressIndex];
            List<Button> thisButtonList = singleResetList[pressIndex];
            ObservableCollection<string> thisLine = peoplePlacer[pressIndex];
            int rowPlace = Grid.GetRow(adjustGrid);
            int columnPlace = Grid.GetColumn(adjustGrid);
            adjustGrid.Visibility = Visibility.Collapsed;
            adjustGrid = null;
            ObservableCollection<string> thisList = pressLists[pressIndex].ItemsSource as ObservableCollection<string>;
            List<People> thisPeopleList = PeopleOnPress[pressIndex];
            for (int i = thisList.Count-1 ;i>=newSize;i--)
            {
                //operatorSource.Add(thisList[i]);
                PeopleList.Add(thisPeopleList[i]);
                thisPeopleList.Remove(thisPeopleList[i]);
            }
            PeopleOnPress[pressIndex] = thisPeopleList;
            lineSizes[pressIndex] = newSize;
            lastIndex = pressIndex;
            UpdateLists();
            GridSetup(columnPlace, rowPlace, pressIndex, thisList, anus);
        } //takes size and name adjustments for grids

        private void DragStartDS(object sender, DragItemsStartingEventArgs e)
        {
            var items = new StringBuilder();
            foreach (var item in e.Items)
            {
                if (items.Length > 0) items.AppendLine();
                items.Append(item as string);
            }
            // Set the content of the DataPackage
            e.Data.SetText(items.ToString());
            // As we want our Reference list to say intact, we only allow Copy
            e.Data.RequestedOperation = DataPackageOperation.Copy;
            DSources.Background = SB;
            foreach (ListView thisList in pressLists)
            {
                thisList.AllowDrop = false;
                thisList.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
            }
        }//die setter list movement

        private void DragStopDS(ListViewBase sender, DragItemsCompletedEventArgs args)//die setter list movement
        {
            DSources.Background = LSB;
            foreach (ListView thisList in pressLists)
            {
                thisList.AllowDrop = true;
                thisList.Background = LSB;
            }
        }

        private void TargetDragOver(object sender, DragEventArgs e)
        {
            // Our list only accepts text
            ListView DragList = sender as ListView;
            String buttonName = DragList.Name;
            String[] nameArray = buttonName.Split('_');
            int pressIndex = Convert.ToInt16(nameArray[1]);
            ObservableCollection<string> targetList = DragList.ItemsSource as ObservableCollection<string>;
            if (targetList.Count < lineSizes[pressIndex])
            {
                e.AcceptedOperation = (e.DataView.Contains(StandardDataFormats.Text)) ? DataPackageOperation.Move : DataPackageOperation.None;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
            DragList.Background = SB;
        }//dragged over target list event

        private void TargetDragLeave(object sender, DragEventArgs e)
        {
            ListView DragList = sender as ListView;
            DragList.Background = LSB;
        }//leave over list(reset color)

        private void EditRoster(object sender, RoutedEventArgs e)
        {
            ReturnList SendData = new ReturnList
            {
                LineNameList = lineNames,
                PeopleList = PeopleList,
                PeopleOnPress = PeopleOnPress
            };
            this.Background = this.Background;
            this.Frame.Navigate(typeof(PeopleDetail),SendData);
        }//edit souce list method

        private async void EditRoster2(object sender, RoutedEventArgs e)
        {
            ListDialog DieSetter = new ListDialog();
            DieSetter.GetRoster(setterSource);
            ContentDialogResult result = await DieSetter.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {

            }
        }

        private void ResetSelection(object sender, PointerRoutedEventArgs e)
        {
            ListView senderList = sender as ListView;
            senderList.SelectedIndex = -1;
            //UpdateLists;
            //this.Background = red;
        }//resets selected item in lists
        
        private async void  DragTargetStart(object sender, DragItemsStartingEventArgs e)
        {
            string item = e.Items.First().ToString();
            
            ListView targetList = sender as ListView;
            String ListName = targetList.Name;
            String[] GetName = ListName.Split('_');
            dragItemLineTracker = Convert.ToInt16(GetName[1]);
            dragItemPressTracker = peoplePlacer[dragItemLineTracker].IndexOf(item);
            cocker.Text = dragItemPressTracker.ToString() + " : " + dragItemLineTracker.ToString();
            //peoplePlacer[dragItemLineTracker].IndexOf



        }




        private async void DragTargetDone(object sender, DragItemsCompletedEventArgs e)
        {
            string thisItem = e.Items.First().ToString();
            ListView thisLast = sender as ListView;
            ListView targetList = sender as ListView;
            String ListName = targetList.Name;
            String[] GetName = ListName.Split('_');
            int targetLine = Convert.ToInt16(GetName[1]);
            int newPosition = peoplePlacer[targetLine].IndexOf(thisItem);
            if(targetLine == dragItemLineTracker)
            {
                List<People> targetedList = PeopleOnPress[targetLine];
                People movedPerson = targetedList[dragItemPressTracker];
                targetedList.Remove(movedPerson);
                targetedList.Insert(newPosition, movedPerson);
                PeopleOnPress[targetLine] = targetedList;
            }
            UpdateLists();
        }

        private async void TargetDrop(object sender, DragEventArgs e)
        {
            // This test is in theory not needed as we returned DataPackageOperation.None if
            // the DataPackage did not contained text. However, it is always better if each
            // method is robust by itself
            ListView targetList = sender as ListView;
            String ListName = targetList.Name;
            String[] GetName = ListName.Split('_');
            int pressIndex = Convert.ToInt16(GetName[1]);
            targetList.Background = LSB;

            /*if (PeopleList[ListNo].Count < 4)
            {*/
                if (e.DataView.Contains(StandardDataFormats.Text))
                {
                    // We need to take a Deferral as we won't be able to confirm the end
                    // of the operation synchronously
                    var def = e.GetDeferral();
                    var s = await e.DataView.GetTextAsync();
                    var items = s.Split('\n');
                    foreach (var item in items)
                    {
                        peoplePlacer[pressIndex].Add(item);
                        int newIndex = operatorSource.IndexOf(item);
                        //cocker.Text = newIndex.ToString();
                        People newPeople = PeopleList[newIndex];

                        PeopleOnPress[pressIndex].Add(newPeople);

                        PeopleList.Remove(newPeople);
                        //operatorSource.Remove(item);
                        List<Button> thisButtonList = singleResetList[pressIndex];
                        thisButtonList[peoplePlacer[pressIndex].Count-1].IsEnabled = true;
                        lastIndex = pressIndex;
                        UpdateLists();
                    }
                e.AcceptedOperation = DataPackageOperation.Move;
                targetList.SelectedIndex = -1;
                def.Complete();
                Saver();
                }
            /*}*/
        }//item mover


    }
}
