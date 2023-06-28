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
      string id = "kimetsu-no-yaiba-katanakaji-no-sato-hen";
      string episode = "1";

      string fileName = $"{id}-episode-{episode}.mp4";
      string filePath = "D:\\Kanime-Mobile\\" + fileName;

      using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
      {
        response.EnsureSuccessStatusCode();

        using (var contentStream = await response.Content.ReadAsStreamAsync())
        {
          var totalBytes = response.Content.Headers.ContentLength;
          var downloadedBytes = 0L;
          var bufferSize = 65536;
          var buffer = new byte[bufferSize];
          var isMoreToRead = true;

          using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
          {
            var tasks = new List<Task>();
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
                var task = fileStream.WriteAsync(buffer, 0, bytesRead);
                tasks.Add(task);
                downloadedBytes += bytesRead;

                if (totalBytes.HasValue)
                {
                  PrintProgressBar(downloadedBytes, totalBytes.Value);
                }
              }

              if (tasks.Count >= Environment.ProcessorCount * 2)
              {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);
              }
            } while (isMoreToRead);

            await Task.WhenAll(tasks);
          }
        }
      }

      Console.WriteLine($"File downloaded: {filePath}");
    }
  }

  private static void PrintProgressBar(long downloaded, long total)
  {
    const int ProgressBarWidth = 30;

    double progressPercentage = (double)downloaded / total;
    int completedWidth = (int)(progressPercentage * ProgressBarWidth);
    int remainingWidth = ProgressBarWidth - completedWidth;

    Console.Write("\u2588");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write(new string('\u2588', completedWidth));
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write(new string('\u2591', remainingWidth));
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("\u2588");

    Console.Write(" ");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write($"{progressPercentage:P0}");
    Console.ResetColor();

    Console.Write("  ");

    string downloadedSize = FormatSize(downloaded);
    string totalSize = FormatSize(total);
    Console.Write($"{downloadedSize} / {totalSize}");

    Console.CursorLeft = 0;
  }

  private static string FormatSize(long bytes)
  {
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    int sizeIndex = 0;
    double size = bytes;

    while (size >= 1024 && sizeIndex < sizes.Length - 1)
    {
      size /= 1024;
      sizeIndex++;
    }

    return $"{size:0.##} {sizes[sizeIndex]}";
  }

}
