using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net.Http;

/*
iterate l
*/

namespace App5
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void deleteBtn(object sender, EventArgs e)
        {
            await App.Database.DeleteTideAsync<TideInformation>();
            await DisplayAlert("Alert","All Items have been deleted","Ok");
        }

        void saveBtn(object sender, EventArgs e)
        {
           appendToSql();
           DisplayAlert("Alert", "All Items have been Added", "Ok");
        }

        async void showBtn(object sender, EventArgs e)
        {
            TideInformationListView.ItemsSource = await App.Database.GetTideAsync();
        }

        async void appendToSql()
        {
            // appends the next 5 days of data to database
            for (int i = 1; i < 5; i++)
            {
                string iter = i.ToString();
                var myResult = getTideData(iter);

                await App.Database.SaveTideAsync(new TideInformation
                {
                    Date = "4/10/2021", 
                    // check if the date already exist

                    firstLowTide = myResult.Item1,
                    firstHighTide = myResult.Item2,
                    secondLowTide = myResult.Item3,
                    secondHighTide = myResult.Item4
                });

            }
        }

        public static Tuple<string, string, string, string> getTideData(string todaysDate) // make this so you can set the day here
        {
            // get the current date and its associated tide times []
            // get the data for the next 5 days and store in sqlite []
            // Todays date
            // 1 = today, 2 = tomorrow
            string firstLowTide, firstHighTide, secondLowTide, secondHighTide;
            firstLowTide = firstHighTide = secondLowTide = secondHighTide = string.Empty;

            var url = @"https://tides.willyweather.com.au/qld/sunshine-coast/ocean-beach.html";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            //5:16 am1.1m10:53 am0.35m5:36 pm1.64m -> string result
            //\d+\W\\d+\d+
            List<string> tideResults = new List<string>();
            //var items = htmlDoc.DocumentNode.SelectNodes("//div/ul/li[1]/ul");
            var items = htmlDoc.DocumentNode.SelectNodes($"//div/ul/li[{todaysDate}]/ul"); //
            foreach (var item in items)
            {
                tideResults.Add(item.InnerText);
            }
            // Part 2: call Regex.Match.
            string str = tideResults[0];
            string[] result =
              Regex.Matches(str, @"((1[0-2]|0?[1-9]):([0-5][0-9]) \s?([AaPp][Mm]))").Cast<Match>().Select(m => m.Value).ToArray();

            foreach (var myParsedDate in result)
            {
                firstLowTide = result[0];
                firstHighTide = result[1];
                secondLowTide = result[2];
                secondHighTide = result[3]; // if this is blank use tomorrows hightide date instead
            }

            return new Tuple<string, string, string, string>(firstLowTide, firstHighTide
                , secondLowTide, secondHighTide);
        }
    }
}
