using System;
using System.Net.Http;
using System.Threading.Tasks;

class VideoDownloader
{
  public static async Task DownloadVideoAsync(string videoUrl)
  {
    using (HttpClient client = new HttpClient())
    {
      // Generate a unique file name for the downloaded video
      string fileName = $"video_{DateTime.Now:yyyyMMddHHmmss}.m3u8";

      try
      {
        HttpResponseMessage response = await client.GetAsync(videoUrl);
        response.EnsureSuccessStatusCode();

        using (var fileStream = System.IO.File.Create(fileName))
        {
          await response.Content.CopyToAsync(fileStream);
        }

        Console.WriteLine($"Video downloaded successfully: {fileName}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed to download video: {ex.Message}");
      }
    }
  }
}
