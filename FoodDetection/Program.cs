using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CsvHelper;
using CsvHelper.Configuration;
using System.Linq;

public class Annotation
{
    public string label_name { get; set; }
    public int bbox_x { get; set; }
    public int bbox_y { get; set; }
    public int bbox_width { get; set; }
    public int bbox_height { get; set; }
    public string image_name { get; set; }
    public int image_width { get; set; }
    public int image_height { get; set; }
}

public class VoTTJson
{
    public Asset asset { get; set; }
    public List<Region> regions { get; set; }
    public string version { get; set; }
}

public class Asset
{
    public string format { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string path { get; set; }
    public Size size { get; set; }
    public int state { get; set; }
    public int type { get; set; }
}

public class Size
{
    public int width { get; set; }
    public int height { get; set; }
}

public class Region
{
    public string id { get; set; }
    public string type { get; set; }
    public List<string> tags { get; set; }
    public BoundingBox boundingBox { get; set; }
    public List<Point> points { get; set; }
}

public class BoundingBox
{
    public float left { get; set; }
    public float top { get; set; }
    public float width { get; set; }
    public float height { get; set; }
}

public class Point
{
    public float x { get; set; }
    public float y { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // Path to your CSV file
        var csvPath = "C:/Users/DENI/Downloads/labels_my-project-dd_2025-04-26-02-11-52.csv";
        // Path where you want to save the VoTT JSON file
        var outputJsonPath = "output_vott.json";
        var annotations = new List<Annotation>();

        // Directory where your images are stored
        var imageDirectory = "C:/Users/DENI/Downloads/ObjectDetection/avocado-salmon-toast/"; // Adjust this based on your folder structure

        // Read CSV file using CsvHelper
        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
        {
            annotations = new List<Annotation>(csv.GetRecords<Annotation>());
        }

        var vottJsonList = new List<VoTTJson>();

        var tagIndexMap = new Dictionary<string, int>();
        var imagePaths = new HashSet<string>();

        foreach (var annotation in annotations)
        {
            var assetName = imageDirectory + annotation.image_name;
            if (!assetName.EndsWith(".jpg") && !assetName.EndsWith(".png"))
            {
                continue; // Modify this line if you want to prefer .png or another format
            }

            // Ensure image path is added
            if (!imagePaths.Contains(assetName))
            {
                imagePaths.Add(assetName);

                // Create asset entry
                var asset = new Asset
                {
                    format = "jpg",  // You can change to PNG if needed
                    id = Guid.NewGuid().ToString(),
                    name = assetName,
                    path = "file:" + Path.Combine(Directory.GetCurrentDirectory(), assetName),
                    size = new Size
                    {
                        width = annotation.image_width,
                        height = annotation.image_height
                    },
                    state = 2,
                    type = 1
                };

                // Create VoTT JSON for each image
                var vottJson = new VoTTJson
                {
                    asset = asset,
                    regions = new List<Region>(),
                    version = "2.1.0"
                };

                // Add regions (bounding boxes) for each annotation related to the image
                foreach (var anno in annotations.Where(a => a.image_name == annotation.image_name))
                {
                    var region = new Region
                    {
                        id = Guid.NewGuid().ToString(),
                        type = "RECTANGLE",
                        tags = new List<string> { anno.label_name },
                        boundingBox = new BoundingBox
                        {
                            left = (float)anno.bbox_x / anno.image_width,
                            top = (float)anno.bbox_y / anno.image_height,
                            width = (float)anno.bbox_width / anno.image_width,
                            height = (float)anno.bbox_height / anno.image_height
                        },
                        points = new List<Point>
                        {
                            new Point { x = (float)anno.bbox_x / anno.image_width, y = (float)anno.bbox_y / anno.image_height },
                            new Point { x = (float)(anno.bbox_x + anno.bbox_width) / anno.image_width, y = (float)anno.bbox_y / anno.image_height },
                            new Point { x = (float)(anno.bbox_x + anno.bbox_width) / anno.image_width, y = (float)(anno.bbox_y + anno.bbox_height) / anno.image_height },
                            new Point { x = (float)anno.bbox_x / anno.image_width, y = (float)(anno.bbox_y + anno.bbox_height) / anno.image_height }
                        }
                    };

                    // Add the region to the VoTT JSON for this image
                    vottJson.regions.Add(region);
                }

                // Add this image's VoTT JSON to the list
                vottJsonList.Add(vottJson);
            }
        }

        // Serialize the list of VoTT JSON objects and save them to a file
        var json = JsonConvert.SerializeObject(vottJsonList, Formatting.Indented);
        File.WriteAllText(outputJsonPath, json);

        Console.WriteLine("VoTT JSON file saved to: " + outputJsonPath);
    }
}
