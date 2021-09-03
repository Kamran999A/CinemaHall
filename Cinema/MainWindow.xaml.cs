using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Cinema
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string ImagePath { get; set; }
        public string Minute { get; set; }
        public string Description { get; set; }
        public dynamic Data { get; set; }
        public dynamic SingleData { get; set; }
        private int i = 0;
        private string _youtubeVideo;
        HttpClient httpClient = new HttpClient();


        public MainWindow()
        {

            DataContext = this;
            InitializeComponent();
            //Calendar cal = new Calendar();
            //CalendarLabel.SetBinding(ComboBox.ItemsSourceProperty, new Binding()
            //{ Source = Enum.GetValues(typeof(Days)) });
            //CalendarLabel.SetBinding(ComboBox.SelectedValueProperty, new Binding("CurrentDay")

            //{ Source = cal });
            SetFirst();


        }

        private async void SetFirst() 
        {
            var name = "X-Men: Apocalypse";
            HttpResponseMessage response = new HttpResponseMessage();
            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=bdba957b&s={name}&plot=full").Result;
            var str = response.Content.ReadAsStringAsync().Result;

            Data = JsonConvert.DeserializeObject(str);
            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=bdba957b&t={Data.Search[0].Title}&plot=full").Result;

            str = response.Content.ReadAsStringAsync().Result;

            SingleData = JsonConvert.DeserializeObject(str);
            movieNameLabel.Content = SingleData.Title;
            MovieImage.Source = SingleData.Poster;
            MovieImage2.Source = SingleData.Poster;
            Minute = SingleData.Runtime;
            Description = SingleData.Genre;
            movieDescriptionLabel.Content = SingleData.Genre;
            movieCountryLabel.Content = SingleData.Country;
            movieRuntimeLabel.Content = SingleData.Runtime;
            movieReleasedLabel.Content = SingleData.Released;
            movieImdbLabel.Content = SingleData.imdbRating + " /";

        }

        private async void Search_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
          
            var name = MovieTextBox.Text;
            HttpResponseMessage response = new HttpResponseMessage();
            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=bdba957b&s={name}&plot=full").Result;
            var str = response.Content.ReadAsStringAsync().Result;

            Data = JsonConvert.DeserializeObject(str);
            response = httpClient.GetAsync($@"http://www.omdbapi.com/?apikey=bdba957b&t={Data.Search[0].Title}&plot=full").Result;

            str = response.Content.ReadAsStringAsync().Result;

            SingleData = JsonConvert.DeserializeObject(str);
            MovieImage.Source = SingleData.Poster;
            MovieImage2.Source = SingleData.Poster;
            movieDescriptionLabel.Content = SingleData.Genre; 
            movieCountryLabel.Content = SingleData.Country;
            Minute = SingleData.Runtime;
            //movieLabel.Content = Minute + "  " + Description;
            //Nextbtn.Visibility = Visibility.Visible;
            //nextbor.Visibility = Visibility.Visible;

            //var keyword = String.Format("{0} {1} trailer", SingleData.Title, SingleData.Year);
            //await Search(keyword);
            //LoadTrailer();
        }

        private void Search_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MovieTextBox.Visibility = Visibility.Visible;
            BorderSearch.Visibility = Visibility.Visible;
        }
    }
}
