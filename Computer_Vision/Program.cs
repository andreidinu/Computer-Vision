using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace Computer_Vision
{
    class Program
    {
        static void Main(string[] args)
        {
            const string subscriptionKey = "57bf5badf73a4df99a0d6b58b7a02fb8";
            const string server = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0";
            string path = @"E:\Programare\C#\Computer_Vision\Computer_Vision\bin\Debug\Images";
            string[] filesPath = Directory.GetFiles(path, "*.jpg");
            string fileName, tagName, dirName, dir;
            double maxConfidence;
            int i, j;

            for(i = 0; i < filesPath.Length; ++i)
            {
                AnalysisResult computerVision = null;
                Task.Run(async () =>
                {
                    computerVision = await UploadAndAnalyzeImage(filesPath[i], subscriptionKey, server);
                }).GetAwaiter().GetResult();

                if(computerVision.Tags.Length > 0)
                {
                    tagName = computerVision.Tags[0].Name;
                    maxConfidence = computerVision.Tags[0].Confidence;

                    for(j = 0; j < computerVision.Tags.Length; ++j)
                    {
                        if(maxConfidence < computerVision.Tags[j].Confidence)
                        {
                            maxConfidence = computerVision.Tags[j].Confidence;
                            tagName = computerVision.Tags[j].Name;
                        }
                    }
                    dirName = tagName;
                }
                else
                {
                    dirName = "Tagless";
                };

                dir = Path.Combine(path + "\\" + dirName);
                Directory.CreateDirectory(dir);

                fileName = Path.GetFileName(filesPath[i]);

                File.Move(filesPath[i], path + "\\" + dirName + "\\" + fileName);
            }   
        }

        public static async Task<AnalysisResult> UploadAndAnalyzeImage(string imageFilePath, string subscriptionKey, string server)
        {
            await Task.Delay(1000);
            VisionServiceClient VisionServiceClient = new VisionServiceClient(subscriptionKey, server);

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };
                AnalysisResult analysisResult = await VisionServiceClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                return analysisResult;
            }
        }
    }
}