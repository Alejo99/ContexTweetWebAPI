using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using ContexTweet.Data;
using Accord.MachineLearning;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Accord.Math.Distances;
using Accord.Statistics;

namespace ContexTweet.Clustering
{
    class Program
    {
        /// <summary>
        /// Number of centroids for the k-medoids algorithm
        /// </summary>
        private const int KMEDOIDS_NCENTROIDS = 10;
        /// <summary>
        /// Number of max iterations for the k-medoids algorithm
        /// </summary>
        private const int KMEDOIDS_MAXITERATIONS = 150;

        /// <summary>
        /// Main program that iterates over all Urls in the database and applys k-medoids clustering.
        /// The results are inserted in an index table that is used to retrieve tweets by the browser extension.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Config builder
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            // Get repository
            var tweetsRepo = GetRepository(configuration);

            // Get all distinct URLs
            var urls = tweetsRepo.Urls
                .Select(urltw => urltw.Url)
                .Distinct()
                .AsEnumerable();
            Console.WriteLine(string.Format("* {0} urls", urls.Count()));

            // Iterate over the URLs
            foreach(string url in urls)
            {
                // Search tweets by URL
                var tweets = tweetsRepo.Urls
                    .Where(urltw => urltw.Url.Equals(url))
                    .Select(urltw => urltw.Tweet)
                    .AsEnumerable();
                var numberOfTweets = tweets.Count();
                Console.WriteLine(string.Format("** {0} tweets for url {1}", numberOfTweets, url));

                // If there are at least 11 tweets...
                // ...cluster them to reduce duplicates or near duplicates
                if(numberOfTweets > 10)
                {
                    // Get the TF-IDF weighted bags of words (one row for each tweet)
                    double[][] bows = GetTFIDFBagOfWords(tweets.Select(t => t.Text).ToArray());

                    // Add user id, sentiment score and tweet id, standardise thie data and 
                    // get a list of features. An index of tweets is needed because tweet Ids 
                    // are too large for a double precision variable.
                    Dictionary<double, string> tweetIds = new Dictionary<double, string>();
                    double[][] features = AddTweetInfo(bows, tweets, ref tweetIds);

                    //Get adjusted weights for each feature.
                    double[] weights = GetWeights(features[0].Length);

                    // Create k-medoids to cluster tweets.
                    // Use square euclidean distance using the weights obtained previously.
                    var kmedoids = new KMedoids<double>(KMEDOIDS_NCENTROIDS, 
                        new WeightedSquareEuclidean(weights));
                    kmedoids.MaxIterations = KMEDOIDS_MAXITERATIONS;

                    // Compute and retrieve the cluster and centroids
                    var clusters = kmedoids.Learn(features);
                    var centroids = clusters.Centroids;

                    // TODO: clean index table and insert url -> centroid (tweet)

                    var medoids = new List<Models.Tweet>();
                    foreach(var centroid in centroids)
                    {
                        var tweet = tweets
                            .Where(tw => tw.Id.Equals(tweetIds[centroid[0]]))
                            .FirstOrDefault();
                        medoids.Add(tweet);
                    }

                    Console.WriteLine("yey");
                }
            }
        }

        /// <summary>
        /// Builds an ITweetRepository based on the configuration served as a parameter.
        /// </summary>
        /// <param name="configuration">The configuration object</param>
        /// <returns>An ITweetRepository</returns>
        private static ITweetRepository GetRepository(IConfigurationRoot configuration)
        {
            // Connection string and DbContext
            var connString = configuration.GetConnectionString("ContexTweetDb");
            var dbOptions = new DbContextOptionsBuilder<ContexTweetDbContext>();
            dbOptions.UseSqlServer(connString);
            var dbContext = new ContexTweetDbContext(dbOptions.Options);

            // Repository
            return new EFTweetRepository(dbContext);
        }

        /// <summary>
        /// Processes the tweets to generate an TF-IDF output vector.
        /// </summary>
        /// <param name="tweets">The tweets to process</param>
        /// <returns>A TF-IDF bag of words</returns>
        private static double[][] GetTFIDFBagOfWords(string[] tweets)
        {
            // Tokenize the tweets
            string[][] words = tweets.Tokenize();

            // TF-IDF settings
            TFIDF codebook = new TFIDF()
            {
                Tf = TermFrequency.Log,
                Idf = InverseDocumentFrequency.Default
            };
            // Process all the words from the tweets 
            codebook.Learn(words);

            // Get the weighted bags of words (one row for each tweet)
            return codebook.Transform(words);
        }

        /// <summary>
        /// <para>
        /// Adds tweet id, user id and sentiment score to the bag of words.
        /// </para>
        /// <para>
        /// This method uses the dictionary as a reference to keep track of the tweets. 
        /// It also uses zscores to standardise the data.
        /// </para>
        /// </summary>
        /// <param name="bagOfWords">A TF-IDF bag of words</param>
        /// <param name="tweets">The tweets to process</param>
        /// <param name="tweetIds">The index dictionary to fill</param>
        /// <returns></returns>
        private static double[][] AddTweetInfo(double[][] bagOfWords,
            IEnumerable<Models.Tweet> tweets, 
            ref Dictionary<double, string> tweetIds)
        {
            // Add user id, sentiment score
            for(int i = 0; i < bagOfWords.Length; i++)
            {
                var wordFeatures = bagOfWords[i];
                var tweet = tweets.ElementAt(i);
                var tmp = new double[wordFeatures.Length + 2];
                tmp[0] = double.Parse(tweet.UserId); //user id
                tmp[1] = tweet.SentimentScore; //sentiment score
                Array.Copy(wordFeatures, 0, tmp, 2, wordFeatures.Length);
                bagOfWords[i] = tmp;
            }
            // Standardise using zscores
            bagOfWords = bagOfWords.ZScores();

            // Add tweet id. Since user id is out of precision for a double, 
            // use a dict as an index to retrieve the tweets after.
            var index = Enumerable.Range(0, bagOfWords.Length).Select(n => Convert.ToDouble(n)).ToArray();
            for (int j = 0; j < bagOfWords.Length; j++)
            {
                var wordFeatures = bagOfWords[j];
                var tweet = tweets.ElementAt(j);
                var tmp = new double[wordFeatures.Length + 1];
                tmp[0] = index[j]; //index id
                tweetIds[index[j]] = tweet.Id; //(double)index id --> (string)tweet id
                Array.Copy(wordFeatures, 0, tmp, 1, wordFeatures.Length);
                bagOfWords[j] = tmp;
            }

            return bagOfWords;
        }

        /// <summary>
        /// Arranges the weights to be used in an Weighted Euclidian Distance object.
        /// </summary>
        /// <param name="numberOfFeatures">Number of features</param>
        /// <returns>The arranged weights that ignore the first element (tweet id)</returns>
        private static double[] GetWeights(int numberOfFeatures)
        {
            double[] weights = new double[numberOfFeatures];
            weights.Populate(1);
            weights[0] = 0; //tweet id
            weights[1] = 0.25; //user id
            weights[2] = 1.25; //sentiment score

            return weights;
        }
    }

    static class Utils
    {
        /// <summary>
        /// Populates an array with a constant value.
        /// Taken from https://stackoverflow.com/questions/1014005/how-to-populate-instantiate-a-c-sharp-array-with-a-single-value
        /// </summary>
        /// <typeparam name="T">Type of the array to populate</typeparam>
        /// <param name="arr">Array to populate</param>
        /// <param name="value">Constant value to populate the array</param>
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }
    }
}
