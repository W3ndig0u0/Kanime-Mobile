using System;

class Program
{
  static void Main(string[] args)
  {
    GogoWebScraper scraper = new GogoWebScraper();
    var videoUrls = scraper.StreamingUrlFinder();

    if (videoUrls.Count == 0)
    {
      Console.WriteLine("No video URLs found.");
    }
    else
    {
      Console.WriteLine("Choose a video URL to download:");

      for (int i = 0; i < videoUrls.Count; i++)
      {
        Console.WriteLine($"{i + 1}. {videoUrls[i]}");
      }

      int selectedOption;
      do
      {
        Console.Write("Enter the number of the video URL to download (0 to exit): ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out selectedOption))
        {
          if (selectedOption >= 1 && selectedOption <= videoUrls.Count)
          {
            break;
          }
          else if (selectedOption == 0)
          {
            Console.WriteLine("Exiting...");
            return;
          }
        }

        Console.WriteLine("Invalid input. Please try again.");
      } while (true);

      string selectedVideoUrl = videoUrls[selectedOption - 1];

      VideoDownloader.Download(selectedVideoUrl).Wait();
    }

    // Wait for user input before exiting (optional)
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }
}
