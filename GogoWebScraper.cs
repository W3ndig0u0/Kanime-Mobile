using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Threading;

class GogoWebScraper
{
  public List<string> StreamingUrlFinder()
  {
    List<string> videoUrls = new List<string>();

    string baseUrl = "https://gogoanime.hu/";
    string id = "kimetsu-no-yaiba-katanakaji-no-sato-hen";
    string episode = "1";

    string episodeUrl = $"{baseUrl}{id}-episode-{episode}";

    try
    {
      using (IWebDriver driver = new ChromeDriver())
      {
        driver.Navigate().GoToUrl(episodeUrl);
        Console.WriteLine(episodeUrl);

        IWebElement videoNode = driver.FindElement(By.XPath("//li[@class='anime']/a"));

        if (videoNode == null)
        {
          Console.WriteLine("Video not found");
          return videoUrls;
        }

        string videoUrl = videoNode.GetAttribute("data-video");

        string streamingUrl = $"{videoUrl.Replace("streaming.php", "download")}";
        driver.Navigate().GoToUrl(streamingUrl);

        IList<IWebElement> downloadDivs = WaitForElements(driver, By.ClassName("dowload"), TimeSpan.FromSeconds(2000));

        IWebElement fileSizeNode = driver.FindElement(By.Id("filesize"));
        IWebElement durationNode = driver.FindElement(By.Id("duration"));

        if (downloadDivs.Count == 0)
        {
          Console.WriteLine("Download Links not found");
          return videoUrls;
        }

        if (fileSizeNode == null || durationNode == null)
        {
          Console.WriteLine("filesize or duration not found");
          return videoUrls;
        }

        string fileSize = fileSizeNode.Text;
        string duration = durationNode.Text;

        foreach (IWebElement divElement in downloadDivs)
        {
          IReadOnlyCollection<IWebElement> linkNodes = divElement.FindElements(By.XPath(".//a"));

          if (linkNodes != null)
          {
            int counter = 1;
            foreach (IWebElement linkNode in linkNodes)
            {
              string link = linkNode.GetAttribute("href");
              string downloadText = linkNode.Text;

              if (downloadText == "DOWNLOAD FOR AD")
              {
                continue;
              }

              videoUrls.Add(link);

              Console.ForegroundColor = ConsoleColor.Yellow;
              Console.Write($"{counter}. ");
              Console.ForegroundColor = ConsoleColor.Cyan;
              Console.Write(downloadText);
              Console.ForegroundColor = ConsoleColor.White;
              Console.Write(" : ");
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine(link);
              counter++;
            }
          }

        }

        Console.WriteLine("File Size: " + fileSize);
        Console.WriteLine("Duration: " + duration);
      }
    }
    catch (Exception e)
    {
      Console.WriteLine($"Failed to retrieve video information: {e.Message}");
    }

    return videoUrls;
  }

  private IList<IWebElement> WaitForElements(IWebDriver driver, By by, TimeSpan timeout)
  {
    WebDriverWait wait = new WebDriverWait(driver, timeout);
    IList<IWebElement> elements;

    DateTime startTime = DateTime.Now;
    TimeSpan elapsed = TimeSpan.Zero;

    do
    {
      try
      {
        elements = driver.FindElements(by);

        if (elements.Count > 0)
        {
          return elements;
        }
      }
      catch (NoSuchElementException)
      {
      }

      elapsed = DateTime.Now - startTime;

      Thread.Sleep(500); // Adjust the interval as needed
    } while (elapsed < timeout);

    return new List<IWebElement>();
  }
}
