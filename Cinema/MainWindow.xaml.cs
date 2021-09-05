using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
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
using CefSharp;
using CefSharp.Wpf;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
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

        List<Movie> movies = new List<Movie>();

        Dictionary<string, bool> seats = new Dictionary<string, bool>();


      

        private int i = 0;

        private string _youtubeVideo;


        ChromiumWebBrowser chrome;
        private readonly YouTubeService _youtubeService;
        private ApiConfig _config;


        HttpClient httpClient = new HttpClient();



        public RelayCommand MessageCommand { get; set; }
        public RelayCommand SendCommand { get; set; }




        public MainWindow()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";
            Cef.Initialize(settings, true, browserProcessHandler: null);

            InitializeComponent();
            DataContext = this;
            MessageCommand = new RelayCommand(Display);

            SendCommand = new RelayCommand(Send);

            seats = FileHelper.ReadJSON_Users();
            PrepareApi();
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _config.Token,
                ApplicationName = this.GetType().ToString()
                
            });
            SetFirst();
        }


        private void PrepareApi()
        {
            var str = string.Empty;

            var fileName = "ApiConfig.json";

            if (!File.Exists(fileName))
                return;

            using (var fs = File.OpenRead("ApiConfig.json"))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
                str = sr.ReadToEnd();

            _config = JsonConvert.DeserializeObject<ApiConfig>(str);
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
            dynamic SingleData = JsonConvert.DeserializeObject(str);
            movies.Add(new Movie
            {
                Name = SingleData.Title,
                Genre = SingleData.Genre,
                Poster = SingleData.Poster,
                Country = SingleData.Country,
                Director = SingleData.Director,
                Writers = SingleData.Writer,
                MovieImage = SingleData.Poster,
                MovieImage2 = SingleData.Poster,
                Minute = SingleData.Runtime,
                Imdb = SingleData.imdbRating,
                Year = SingleData.Released,
                Plot = SingleData.Plot

            });
            foreach (var movie in movies)
            {

                movieNameLabel.Content = movie.Name;
               // MovieImage.Source = new BitmapImage(new Uri(movie.MovieImage));
                MovieImage2.Source = new BitmapImage(new Uri(movie.MovieImage2));

                string Genre = movie.Genre;
                string[] Genres = Genre.Split(',');
                movieDescriptionLabel.Content = Genres[0] + " |" + Genres[1] + " |" + Genres[2];
                movieCountryLabel.Content = movie.Country;
                movieRuntimeLabel.Content = movie.Minute;
                movieReleasedLabel.Content = movie.Year.ToString("dd MMMM yyyy");
                movieImdbLabel.Content = movie.Imdb + " /";
                PlotText.Text = movie.Plot;
                Writers.Content = movie.Writers;
                DirectorLabel.Content = movie.Director;



            }

            var keyword = String.Format("{0} {1} trailer", SingleData.Title, SingleData.Year);
            await Search2(keyword);
            LoadTrailer();

        }
        private void LoadTrailer2()
        {
            var windowAK = new Window();


            // ChromiumBrowser.Address = $@"https://www.youtube.com/watch?v{_youtubeVideo}&autoplay=1&modestbranding=1";
            ChromiumWebBrowser browser = new ChromiumWebBrowser();
            browser.Address = $@"https://www.youtube.com/embed/{_youtubeVideo}?autoplay=1";

            //var handler = browser.Resurse
            windowAK.Content = browser;
            ChromiumBrowser.StopCommand.Execute(true);
            // Application.Current.Run(windowAK);
            windowAK.ShowDialog();

        }
        private async void  Button_Click_1(object sender, RoutedEventArgs e)
        {
            ChromiumBrowser.Address = $@"https://www.youtube.com/embed/{_youtubeVideo}?autoplay=1&mute=1";
            var keyword = String.Format("{0} {1} trailer", movieNameLabel.Content, movieReleasedLabel.Content);
            await Search2(keyword);
            LoadTrailer2();
            ChromiumBrowser.Address = $@"https://www.youtube.com/embed/{_youtubeVideo}?autoplay=1";
        }
        private void LoadTrailer()
        {
            ChromiumBrowser.Address = $@"https://www.youtube.com/embed/{_youtubeVideo}?autoplay=1&controls=0";
        }

        private async Task Search2(string keyword)
        {
            var searchListRequest = _youtubeService.Search.List("snippet");
            searchListRequest.Q = keyword;
            searchListRequest.MaxResults = 1;
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(searchResult.Id.VideoId);
                        break;
                    default:
                        break;
                }
            }
            _youtubeVideo = videos[0];
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




            movies.Add(new Movie
            {
                Name = SingleData.Title,
                Genre = SingleData.Genre,
                Poster = SingleData.Poster,
                Country = SingleData.Country,
                Director = SingleData.Director,
                Writers = SingleData.Writer,
                MovieImage = SingleData.Poster,
                MovieImage2 = SingleData.Poster,
                Minute = SingleData.Runtime,
                Imdb = SingleData.imdbRating,
                Year = SingleData.Released,
                Plot = SingleData.Plot
            });

            foreach (var movie in movies)
            {

                movieNameLabel.Content = movie.Name;
               // MovieImage.Source = new BitmapImage(new Uri(movie.MovieImage));
                MovieImage2.Source = new BitmapImage(new Uri(movie.MovieImage2));

                string Genre = movie.Genre;
                string[] Genres = Genre.Split(',');
                movieDescriptionLabel.Content = Genres[0] + " |" + Genres[1] + " |" + Genres[2];
                movieCountryLabel.Content = movie.Country;
                movieRuntimeLabel.Content = movie.Minute;
                movieReleasedLabel.Content = movie.Year.ToString("dd MMMM yyyy");
                movieImdbLabel.Content = movie.Imdb + " /";
                // FileHelper.FileHelper.WriteToJsonFile(seats, $"{movie.Name}.json");


            }
            var keyword = String.Format("{0} {1} trailer", SingleData.Title, SingleData.Year);
            await Search2(keyword);
            LoadTrailer();
        }

        private void Search_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MovieTextBox.Visibility = Visibility.Visible;
            BorderSearch.Visibility = Visibility.Visible;
        }




        public void Display(object text = null)
        {

            //seats.Add(A2.Name, A2.IsChecked.Value);
            //seats.Add(A3.Name, A3.IsChecked.Value);
            //seats.Add(A4.Name, A4.IsChecked.Value);
            //seats.Add(A5.Name, A5.IsChecked.Value);
            //seats.Add(A6.Name, A6.IsChecked.Value);
            //seats.Add(A7.Name, A7.IsChecked.Value);

            //seats.Add(B2.Name, B2.IsChecked.Value);
            //seats.Add(B3.Name, B3.IsChecked.Value);
            //seats.Add(B4.Name, B4.IsChecked.Value);
            //seats.Add(B5.Name, B5.IsChecked.Value);
            //seats.Add(B6.Name, B6.IsChecked.Value);
            //seats.Add(B7.Name, B7.IsChecked.Value);


            //seats.Add(C2.Name, C2.IsChecked.Value);
            //seats.Add(C3.Name, C3.IsChecked.Value);
            //seats.Add(C4.Name, C4.IsChecked.Value);
            //seats.Add(C5.Name, C5.IsChecked.Value);
            //seats.Add(C6.Name, C6.IsChecked.Value);
            //seats.Add(C7.Name, C7.IsChecked.Value);


            //seats.Add(D2.Name, D2.IsChecked.Value);
            //seats.Add(D3.Name, D3.IsChecked.Value);
            //seats.Add(D4.Name, D4.IsChecked.Value);
            //seats.Add(D5.Name, D5.IsChecked.Value);
            //seats.Add(D6.Name, D6.IsChecked.Value);
            //seats.Add(D7.Name, D7.IsChecked.Value);

            //seats.Add(E2.Name, E2.IsChecked.Value);
            //seats.Add(E3.Name, E3.IsChecked.Value);
            //seats.Add(E4.Name, E4.IsChecked.Value);
            //seats.Add(E5.Name, E5.IsChecked.Value);
            //seats.Add(E6.Name, E6.IsChecked.Value);
            //seats.Add(E7.Name, E7.IsChecked.Value);

            //seats.Add(F2.Name, F2.IsChecked.Value);
            //seats.Add(F3.Name, F3.IsChecked.Value);
            //seats.Add(F4.Name, F4.IsChecked.Value);
            //seats.Add(F5.Name, F5.IsChecked.Value);
            //seats.Add(F6.Name, F6.IsChecked.Value);
            //seats.Add(F7.Name, F7.IsChecked.Value);



            //seats.Add(G2.Name, G2.IsChecked.Value);
            //seats.Add(G3.Name, G3.IsChecked.Value);
            //seats.Add(G4.Name, G4.IsChecked.Value);
            //seats.Add(G5.Name, G5.IsChecked.Value);
            //seats.Add(G6.Name, G6.IsChecked.Value);
            //seats.Add(G7.Name, G7.IsChecked.Value);


            //seats.Add(H2.Name, H2.IsChecked.Value);
            //seats.Add(H3.Name, H3.IsChecked.Value);
            //seats.Add(H4.Name, H4.IsChecked.Value);
            //seats.Add(H5.Name, H5.IsChecked.Value);
            //seats.Add(H6.Name, H6.IsChecked.Value);
            //seats.Add(H7.Name, H7.IsChecked.Value);





            //seats.Add(I2.Name, I2.IsChecked.Value);
            //seats.Add(I3.Name, I3.IsChecked.Value);
            //seats.Add(I4.Name, I4.IsChecked.Value);
            //seats.Add(I5.Name, I5.IsChecked.Value);
            //seats.Add(I6.Name, I6.IsChecked.Value);
            //seats.Add(I7.Name, I7.IsChecked.Value);




            //seats.Add(J2.Name, J2.IsChecked.Value);
            //seats.Add(J3.Name, J3.IsChecked.Value);
            //seats.Add(J4.Name, J4.IsChecked.Value);
            //seats.Add(J5.Name, J5.IsChecked.Value);
            //seats.Add(J6.Name, J6.IsChecked.Value);
            //seats.Add(J7.Name, J7.IsChecked.Value);


            //seats.Add(K2.Name, K2.IsChecked.Value);
            //seats.Add(K3.Name, K3.IsChecked.Value);
            //seats.Add(K4.Name, K4.IsChecked.Value);
            //seats.Add(K5.Name, K5.IsChecked.Value);
            //seats.Add(K6.Name, K6.IsChecked.Value);
            //seats.Add(K7.Name, K7.IsChecked.Value);

            //foreach (var movie in movies)
            //{
            //    FileHelper.FileHelper.WriteToJsonFile(seats, $"{movie.Name}.json");
            //}


        }
        public void Send(object text = null)
        {
            MessageBox.Show("Message was sent");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveToFiles()
        {
            //var directoryName = $@"{movie.Name}";



            //if (!Directory.Exists(directoryName))
            //    Directory.CreateDirectory(directoryName);



            //var fileName = FileHelper.FileHelper.CreateNewFileName();
            //save to json file

        }
        private void A_Checked(object sender, RoutedEventArgs e)
        {


            //if (sender is CheckBox)
            //{
            //    seats = null;
            //    ;

            //}
            //String encoded = JsonConvert.SerializeObject(seats);

            //foreach (var movie in movies)
            //{

            //    //FileHelper.FileHelper.WriteToJsonFile(seats, $"{movie.Name}.json");
            //}

        }

    }
}
