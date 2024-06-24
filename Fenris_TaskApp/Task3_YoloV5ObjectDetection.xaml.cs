using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing.Imaging;

namespace Fenris_TaskApp
{
    public partial class Task3_YoloV5ObjectDetection : Window
    {
        private string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "yolov5s.onnx");
        private MLContext mlContext;
        private PredictionEngine<YoloV5Input, YoloV5Output> predictionEngine;
        private const int ZebraClassIndex = 22;

        public Task3_YoloV5ObjectDetection()
        {
            InitializeComponent();
            InitializeModel();
        }

        private void InitializeModel()
        {
            mlContext = new MLContext();

            var pipeline = mlContext.Transforms.ApplyOnnxModel(
                modelFile: modelPath,
                outputColumnNames: new[] { "output0" },
                inputColumnNames: new[] { "images" });

            var emptyData = mlContext.Data.LoadFromEnumerable(new List<YoloV5Input>());

            var model = pipeline.Fit(emptyData);

            predictionEngine = mlContext.Model.CreatePredictionEngine<YoloV5Input, YoloV5Output>(model);
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var imagePath = openFileDialog.FileName;
                var bitmap = new Bitmap(imagePath);
                imgDisplay.Source = LoadBitmap(bitmap);

                var detections = DetectObjects(bitmap);
                DrawDetections(bitmap, detections);
                imgDisplay.Source = LoadBitmap(bitmap);
            }
        }

        private YoloV5Output DetectObjects(Bitmap bitmap)
        {
            // Resize and preprocess the image
            var resizedImage = new Bitmap(bitmap, new System.Drawing.Size(640, 640));
            var imageData = ConvertImageToFloatArray(resizedImage);

            // Inference
            var input = new YoloV5Input { Image = imageData };
            var output = predictionEngine.Predict(input);

            return output;
        }
        private float[] ConvertImageToFloatArray(Bitmap bitmap)
        {
            var imageData = new float[1 * 3 * 640 * 640]; // batch size * channels * height * width

            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int height = bitmap.Height;
            int width = bitmap.Width;
            int stride = bmpData.Stride;

            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int pixelIndex = y * stride + x * bytesPerPixel;


                        float blue = ptr[pixelIndex] / 255.0f;
                        float green = ptr[pixelIndex + 1] / 255.0f;
                        float red = ptr[pixelIndex + 2] / 255.0f;

                        imageData[(0 * 3 * 640 * 640) + (0 * 640 * 640) + (y * 640) + x] = red;
                        imageData[(0 * 3 * 640 * 640) + (1 * 640 * 640) + (y * 640) + x] = green;
                        imageData[(0 * 3 * 640 * 640) + (2 * 640 * 640) + (y * 640) + x] = blue;
                    }
                }
            }

            bitmap.UnlockBits(bmpData);

            return imageData;
        }
        private void DrawDetections(Bitmap bitmap, YoloV5Output output)
        {
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (var detection in ParseDetections(output.PredictedLabels, originalWidth, originalHeight))
                {
                    var pen = new Pen(Color.Red, 2);
                    g.DrawRectangle(pen, detection);
                }
            }
        }

        private IEnumerable<Rectangle> ParseDetections(float[] predictions, int originalWidth, int originalHeight)
        {
            var results = new List<Rectangle>();

            int numDetections = predictions.Length / 85; // Each detection has 85 values for COCO (4 + 1 + 80)

            for (int i = 0; i < numDetections; i++)
            {
                int offset = i * 85;

                float x_center = predictions[offset];
                float y_center = predictions[offset + 1];
                float width = predictions[offset + 2];
                float height = predictions[offset + 3];
                float confidence = predictions[offset + 4];

                float maxClassScore = float.MinValue;
                int classId = -1;
                for (int j = 5; j < 85; j++)
                {
                    if (predictions[offset + j] > maxClassScore)
                    {
                        maxClassScore = predictions[offset + j];
                        classId = j - 5; // Subtract 5 to get the class index
                    }
                }

                if (confidence > 0.6 && classId == ZebraClassIndex)
                {
                    float x_min = x_center - width / 2;
                    float y_min = y_center - height / 2;

                    x_min = x_min / 640 * originalWidth;
                    y_min = y_min / 640 * originalHeight;
                    width = width / 640 * originalWidth;
                    height = height / 640 * originalHeight;

                    results.Add(new Rectangle((int)x_min, (int)y_min, (int)width, (int)height));
                }
            }

            return results;
        }

        private BitmapImage LoadBitmap(Bitmap source)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                source.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
    public class YoloV5Input
    {
        [ColumnName("images")]
        [VectorType(1, 3, 640, 640)] 
        public float[] Image { get; set; }
    }

    public class YoloV5Output
    {
        [ColumnName("output0")]
        public float[] PredictedLabels { get; set; }
    }
}
