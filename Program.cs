﻿using System;

class Program
{
  static void Main(string[] args)
  {
    string id = "kimetsu-no-yaiba-katanakaji-no-sato-hen";
    string episode = "1";

    GogoWebScraper scraper = new GogoWebScraper();
    var videoUrls = scraper.StreamingUrlFinder(id, episode);
    if (videoUrls == null)
    {
      return;
    }

    int selectedOption;

    do
    {
      Console.Write("Enter the number of the video URL to download (0 to exit): ");
      string? input = Console.ReadLine();

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

    VideoDownloader.Download(selectedVideoUrl, id).Wait();
  }
}
