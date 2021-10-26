using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Essentials;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Globalization;
using SQLite;
using System.Threading.Tasks;

// *****connect to Bluestacks*****
// *****adb connect 127.0.0.1:5555*****

// TODO 
// get the next day low tide for the 3 time problem [ done ]
// get the a method to get the value from the database
// away to make sure that the database is never empty

// < Button Text = "Save Data" HorizontalOptions = "FillAndExpand" BackgroundColor = "#1690F4" TextColor = "White" Margin = "0,20" Clicked = "saveBtn" />


namespace App5
{
    public partial class MainPage : ContentPage
    {
        readonly SQLiteAsyncConnection _database;
        public MainPage()
        {
            InitializeComponent();

            // if connnectivity is available updated the database - make this a yes or no thing
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                appendToSql();
            }
        }


        async void deleteBtn(object sender, EventArgs e)
        {
            await App.Database.DeleteTideAsync<TideInformation>();
            
        }

        async void showBtn(object sender, EventArgs e)
        {
            TideInformationListView.ItemsSource = await App.Database.GetTideAsync();
        }

        async void appendToSql()
        {
            // appends the next 5 days of data to database
            for (int i = 1; i < 6; i++)
            {
                string iter = i.ToString();
                var myResult = getTideData(iter);

                // check if the date already exist
                bool myReturnValue = App.Database.checkDate(myResult.Item1);

                if(myReturnValue == false)

                    await App.Database.SaveTideAsync(new TideInformation
                    {
                        date = myResult.Item1,
                        firstLowTide = myResult.Item2,
                        firstHighTide = myResult.Item3,
                        secondLowTide = myResult.Item4,
                        secondHighTide = myResult.Item5
                    });
           }
        }



        async void findBtn(object sender, EventArgs e)
        {
            // need to somewhat use my list in another 

            List<string> outputList = await App.Database.GetListAsync();
            await DisplayAlert("Alert", "The first low tide is at: " + outputList[0], "Ok");
        }




        // This needs to be refactored!!!!! Apply OOP 
        public static Tuple<string, string, string, string, string> getTideData(string todaysDate) // make this so you can set the day here
        {
            // 1 = today, 2 = tomorrow

            string date, firstLowTide, firstHighTide, secondLowTide, secondHighTide;
            date = firstLowTide = firstHighTide = secondLowTide = secondHighTide = string.Empty;

            var url = @"https://tides.willyweather.com.au/qld/sunshine-coast/ocean-beach.html";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            // 5:16 am1.1m10:53 am0.35m5:36 pm1.64m -> regex pattern -> \d+\W\\d+\d+
            // Output string -> "Today 5 Oct1:25 am0.09m7:35 am1.51m1:19 pm0.07m7:48 pm1.83m" -> Parse out the date then convert to datetime

            List<string> tideResults = new List<string>();
            var items = htmlDoc.DocumentNode.SelectNodes($"//div/ul/li[{todaysDate}]"); // $"//div/ul/li[{todaysDate}]/ul")
            foreach (var item in items)
            {
                tideResults.Add(item.InnerText);
            }

            // Get *tomorrows* 1st tide time
            int intTomorrowsDate = Int32.Parse(todaysDate) + 1;
            string strTomorrowsDate = intTomorrowsDate.ToString();
            List<string> tomorrowsTideResults = new List<string>();
            var tomorrowItems = htmlDoc.DocumentNode.SelectNodes($"//div/ul/li[{strTomorrowsDate}]"); // $"//div/ul/li[{todaysDate}]/ul")
            foreach (var item in tomorrowItems)
            {
                tomorrowsTideResults.Add(item.InnerText);
            }

            string strTomorrow = tomorrowsTideResults[2];
            string[] tomorrowTimeInterval =
              Regex.Matches(strTomorrow, @"((1[0-2]|0?[1-9]):([0-5][0-9]) \s?([AaPp][Mm]))").Cast<Match>().Select(m => m.Value).ToArray();


            // get the fourth time interval
            string str = tideResults[2];
            string[] timeInterval =
              Regex.Matches(str, @"((1[0-2]|0?[1-9]):([0-5][0-9]) \s?([AaPp][Mm]))").Cast<Match>().Select(m => m.Value).ToArray();

            // get the date 
            string[] getDate =
                Regex.Matches(str, @"\d+\s[A-Za-z][A-Za-z][A-Za-z]").Cast<Match>().Select(m => m.Value).ToArray();

            // converts the date to datetime then converts to string - something is wrong here
            var mydate = DateTime.ParseExact(getDate[0], "d MMM", CultureInfo.InvariantCulture);
            string myNewDate = mydate.ToString();
            string onlyDate = myNewDate.Substring(0, 10); // this use to be 9 characters works when 9/10/2021 breaks when 19/10/2021

            //
            if(timeInterval.Length == 4)
            {
                foreach (var myParsedDate in timeInterval)
                {
                    date = onlyDate;
                    firstLowTide = timeInterval[0];
                    firstHighTide = timeInterval[1];
                    secondLowTide = timeInterval[2];
                    secondHighTide = timeInterval[3];
                }
            }
            else
            {
                date = onlyDate;
                firstLowTide = timeInterval[0];
                firstHighTide = timeInterval[1];
                secondLowTide = timeInterval[2];
                secondHighTide = tomorrowTimeInterval[0];
            }
            return new Tuple<string, string, string, string, string>(date, firstLowTide, firstHighTide
                , secondLowTide, secondHighTide);
        }
    }
}
