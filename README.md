# Fenris-repo

This project is a WPF application developed in .NET 6 as part of an interview assignment for Fenris.

## NuGet Packages

The project uses the following NuGet packages:

- **LiveCharts.Wpf** Version="0.9.7"
- **MaterialDesignColors** Version="3.1.0"
- **MaterialDesignThemes** Version="5.1.0"
- **MaterialDesignThemes.Wpf** Version="1.0.1"
- **Microsoft.ML** Version="3.0.1"
- **Microsoft.ML.ImageAnalytics** Version="3.0.1"
- **Microsoft.ML.OnnxRuntime** Version="1.18.0"
- **Microsoft.ML.OnnxTransformer** Version="3.0.1"
- **System.Drawing.Common** Version="8.0.6"

## Task Descriptions

### Task 1: Dynamic Chart Creation

**Assumptions:**
- The focus is on creating the functionality of  Add/Remove buttons. A basic frontend is created in XAML without any style code.
- Since no error logging or exception handling flow is provided, try-catch blocks are omitted.
- `LiveCharts` NuGet package is assumed to be installed.

**Approach:**
- The constructor initializes a bar chart with 3 bars, each assigned a unique color.
- The chart is created using the `CartesianChart` control from the LiveCharts package, allowing dynamic manipulation.
- Event handlers are added for Add and Remove buttons.
- The `Fill` property is used to assign random colors to each new bar added.

**Challenges:**
- Ensuring that each new bar is assigned a unique random color was addressed by assigning a unique value to each bar and using the `Fill` property to generate a random color.

### Task 2: UI Design with Material Design

**Assumptions:**
- The top panel with the logo is omitted as no logo was provided.
- Controls are designed using only XAML and base WPF tools, without custom dropdowns or date pickers.
- This is a design task; thus, no functionality is coded for the added elements.

**Approach:**
- The design is created using the `MaterialDesignThemes` package for WPF and custom styles added in `Resources/Styles.xaml`.
- Combobox items are added dynamically from the backend during window initialization.

**Challenges:**
- Due to the lack of detailed design specifications (fonts, colors, sizes, icons, etc.), controls are styled as closely as possible to the provided screenshot.

### Task 3: YOLOv5 Object Detection

**Assumptions:**
- Focus is on creating functionality with a basic frontend in XAML without style code.
- Object detection targets only the zebra class using a pre-trained YOLOv5s ONNX model. The class index can be changed in code to detect other objects.
- A red bounding box is drawn around detected zebras.
- Since no error logging or exception handling flow is provided, try-catch blocks are omitted.

**Approach:**
- Clone the YOLOv5 repository from [ultralytics](https://github.com/ultralytics/yolov5) and export the weights of the pre-trained YOLOv5s model to ONNX format using the command: `python export.py --weights yolov5s.pt --include onnx`.
- Create a basic XAML design with a button and image control.
- Add the ONNX model file to the application folder and write code to access this file. Use the `Microsoft.ML` package to initialize the prediction model.
- Instantiate `MLContext` and create a model pipeline that transforms the input image data to the format required by the ONNX model.
- Implement the open file button click functionality to allow users to select an image for object detection.
- Convert the uploaded image to a `Bitmap` and then to a float array of size `1*3*640*640` (input tensor size for YOLOv5).
- Fit the model pipeline to the input image and use it to create a prediction engine that outputs the predicted labels. Each prediction includes a confidence score and a class ID (80 classes in the COCO dataset).
- Use the prediction data to draw bounding box rectangles on the image where the confidence score is above 60% for class ID 22 (zebra).
- An example image (`ObjectDetection.jpg`) is included in the application folder for testing.

**Challenges:**
- To export the model weights in onnx format, a new environment had to be created in conda with fresh install for all dependencies. There were compatibility issues with old versions of python and libraries like numpy that were previously available in my machine.
- Another challenge was understanding the input and output vectors of the YOLOv5 model. Then using this information to transform the input dataview into the required tensordata used in the prediction engine.

## How to Run the Project

1. Clone the repository to your local machine.
2. Ensure all required NuGet packages are installed.
3. Place the `yolov5s.onnx` model file in the project's root directory (or adjust the path in the code accordingly).
4. Build and run the application.
