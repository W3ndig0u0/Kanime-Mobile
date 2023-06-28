class Program
{
  static void Main(string[] args)
  {

    GogoWebScraper scraper = new GogoWebScraper();
    scraper.Downloader();

    // foreach (var videoUrl in videoUrls)
    // {
    //   await VideoDownloader.DownloadVideoAsync(videoUrl);
    // }

    // Wait for user input before exiting (optional)
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }
}
