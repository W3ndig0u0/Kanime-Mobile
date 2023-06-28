using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class VideoDownloader
{
  public static async Task Download(string url)
  {
    using (HttpClient client = new HttpClient())
    {
      string fileName = GetFileNameFromUrl(url);
      string filePath = "D:\\Kanime-Mobile\\" + fileName;

      using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
      {
        response.EnsureSuccessStatusCode();

        using (var contentStream = await response.Content.ReadAsStreamAsync())
        {
          var totalBytes = response.Content.Headers.ContentLength;
          var downloadedBytes = 0L;
          var buffer = new byte[8192];
          var isMoreToRead = true;

          using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
          {
            do
            {
              var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
              if (bytesRead == 0)
              {
                isMoreToRead = false;
                await Task.Delay(100);
              }
              else
              {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloadedBytes += bytesRead;

                if (totalBytes.HasValue)
                {
                  PrintProgressBar(downloadedBytes, totalBytes.Value);
                }
              }
            } while (isMoreToRead);
          }
        }
      }

      Console.WriteLine($"File downloaded: {filePath}");
    }
  }

  private static void PrintProgressBar(long completed, long total)
  {
    const int ProgressBarWidth = 50;
    double progressPercentage = (double)completed / total;
    int completedWidth = (int)(progressPercentage * ProgressBarWidth);
    int remainingWidth = ProgressBarWidth - completedWidth;

    Console.Write("[");

    Console.Write(new string('=', completedWidth));
    Console.Write(new string(' ', remainingWidth));

    Console.Write("] ");
    Console.Write($"{progressPercentage:P0}");

    Console.CursorLeft = 0; // Move the cursor back to the start of the line
  }

  private static string GetFileNameFromUrl(string url)
  {
    int lastSlashIndex = url.LastIndexOf('/');
    string fileName = url.Substring(lastSlashIndex + 1);
    return fileName;
  }
}
