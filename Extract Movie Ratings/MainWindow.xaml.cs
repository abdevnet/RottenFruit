using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Extract_Movie_Ratings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string GOOGLE_CUSTOM_SEARCH_KEY = "AIzaSyBJtj6wOOLTu3S9DoPQj6ioyDbpspgB0Xg";
        const string GOOGLE_CUSTOM_CX_RM = "007678121076466613582:bh1mg8vcfku";
        const string GOOGLE_CUSTOM_CX_IMDB = "007678121076466613582:k2qbehv29ns";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            string[] movies = txtMovies.Text.Split('\n');

            for (int i = 0; i < movies.Count(); i++)
            {

                int rtTomatoMeter = 0;
                int rtPublicRating = 0;
                int rtNumberOfVotes = 0;

                double imdbRating = 0.0;
                int imdbRatingCount = 0;

                //Extracting ratings from Rotten Tomatoes

                string query = String.Format("https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}", GOOGLE_CUSTOM_SEARCH_KEY, GOOGLE_CUSTOM_CX_RM, movies[i]);
                JObject response = JObject.Parse(new System.Net.WebClient().DownloadString(query));

                string rtLink = response.SelectToken("items[0].link").ToString();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(rtLink);

                HtmlNode[] nodes = document.DocumentNode.SelectNodes("//small[contains(@class, 'mop-ratings-wrap__text--small')]").ToArray();
                rtTomatoMeter = Convert.ToInt32(nodes[0].InnerHtml.ToString());

                nodes = document.DocumentNode.SelectNodes("//span[contains(@class, 'mop-ratings-wrap__percentage')]").ToArray();
                rtPublicRating = Convert.ToInt32(nodes[1].InnerHtml.ToString().Trim().Replace("%", string.Empty));

                nodes = document.DocumentNode.SelectNodes("//strong[contains(@class, 'mop-ratings-wrap__text--small')]").ToArray();
                rtNumberOfVotes = Convert.ToInt32(nodes[1].InnerHtml.ToString().Replace("User Ratings: ", string.Empty).Replace(",", string.Empty));


                //Extracting ratings from IMDb
 
                query = String.Format("https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}", GOOGLE_CUSTOM_SEARCH_KEY, GOOGLE_CUSTOM_CX_IMDB, movies[i]);
                response = JObject.Parse(new System.Net.WebClient().DownloadString(query));

                string imdbLink = response.SelectToken("items[0].link").ToString();
                web = new HtmlWeb();
                document = web.Load(imdbLink);

                nodes = document.DocumentNode.SelectNodes("//span[contains(@itemprop, 'ratingValue')]").ToArray();
                imdbRating = Convert.ToDouble(nodes[0].InnerHtml.ToString());

                nodes = document.DocumentNode.SelectNodes("//span[contains(@itemprop, 'ratingCount')]").ToArray();
                imdbRatingCount = Convert.ToInt32(nodes[0].InnerHtml.ToString().Replace(",", string.Empty));

                //Placing the result in the rating text box
                txtMovieRatings.Text += movies[i] + "\n\tRotten Tomatoes\n\t\tTomato meter: " 
                                        + rtTomatoMeter + "\n\t\tPublic rating: " 
                                        + rtPublicRating + "%\n\t\tNumber of votes: " 
                                        + rtNumberOfVotes + "\n\tIMDb\n\t\tRating: " 
                                        + imdbRating + "\n\t\tRating count: " 
                                        + imdbRatingCount + "\n";
            }
        }
    }
}
