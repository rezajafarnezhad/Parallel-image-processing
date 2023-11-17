
using System.Drawing.Imaging;
using System.Drawing;

namespace Parallel_image_processing;

internal class Program
{
    public static async Task ConvertAsync(string path)
    {
        await Task.Delay(200);
        using var originalImage = new Bitmap(path);

        // Create a new Bitmap with the same dimensions as the original image
        using var blackAndWhiteImage = new Bitmap(originalImage.Width, originalImage.Height);

        Console.WriteLine("Processing Image:");

        // Loop through each pixel in the original image
        for (int x = 0; x < originalImage.Width; x++)
        {

            for (int y = 0; y < originalImage.Height; y++)
            {


                // Get the color of the current pixel in the original image
                Color pixelColor = originalImage.GetPixel(x, y);

                // Calculate the grayscale value (average of red, green, and blue)
                int grayValue = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);

                // Create a grayscale color
                Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);

                // Set the pixel in the black and white image to the grayscale color
                blackAndWhiteImage.SetPixel(x, y, grayColor);
            }
        }

        Console.WriteLine($"{path} Processing Complete");

        // Save the black and white image to a file
        using var memoryStream = new MemoryStream();

        // Save the black and white image to the MemoryStream
        blackAndWhiteImage.Save(memoryStream, ImageFormat.Jpeg);

        // Do something with the MemoryStream (e.g., send it over the network or save to another location)
        // memoryStream.ToArray() contains the image data as bytes

        var fileInfo = new FileInfo(path);

        string outputPath = Path.Combine(fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "-output" + fileInfo.Extension);

        // Save the MemoryStream to disk asynchronously

        await SaveMemoryStreamToFileAsync(memoryStream, outputPath);
    }

    static async Task SaveMemoryStreamToFileAsync(MemoryStream memoryStream, string outputPath)
    {
        using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(fileStream);

    }

    /// <summary>
    /// Parallel processing
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {

        var images = Directory.GetFiles(@"E:\Images");
        var taskList = new List<Task>();
        foreach (var image in images)

        {
            var task = ConvertAsync(image);
            taskList.Add(task);
        }

        Task.Run(async () =>
        {
            await Task.WhenAll(taskList);
        });


        Console.ReadKey();
    }

    /// <summary>
    /// Sync processing
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    //static async Task Main(string[] args)
    //{
    //    var images = Directory.GetFiles(@"E:\Images");
    //    foreach (var image in images)
    //    {
    //        await ConvertAsync(image);

    //    }

    //    Console.ReadKey();
    //}
}