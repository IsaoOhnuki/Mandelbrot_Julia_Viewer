using Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;
using Microsoft.Graphics.Canvas;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Xamarin.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

[assembly: ExportRenderer(typeof(DrawPanel), typeof(Mandelbrot_Julia_Viewer.UWP.DrawPanelRenderer))]
namespace Mandelbrot_Julia_Viewer.UWP
{
    class DrawPanelRenderer : ViewRenderer<DrawPanel, CanvasControl>
    {
        Windows.UI.Input.GestureRecognizer recognizer;

        public CanvasBitmap Image { get; set; }
        public Size ImageSize { get; set; }
        public Windows.Foundation.Rect DrawRect { get; set; }

        private Point origin;
        public Point Origin
        {
            get { return origin; }
            set
            {
                origin = value;
            }
        }
        private double scale = 1;
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                if (scale < 0.01)
                    scale = 0.01;
            }
        }

        public bool UpdateImage()
        {
            if (Element.ImageDataValid)
            {
                double drawWidth = Element.ImageWidth * Scale;
                double drawHeight = Element.ImageHeight * Scale;

                DrawRect = new Windows.Foundation.Rect(
                    Origin.X - drawWidth / 2 + Control.ActualWidth / 2,
                    Origin.Y - drawHeight / 2 + Control.ActualHeight / 2,
                    drawWidth,
                    drawHeight);
                return true;
            }
            return false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DrawPanel> e)
        {
            if (Control == null && e.NewElement != null)
            {
                var ctrl = new CanvasControl();
                ctrl.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All;
                SetNativeControl(ctrl);

                recognizer = new Windows.UI.Input.GestureRecognizer();
                recognizer.GestureSettings = GestureSettings.ManipulationTranslateX
                    | GestureSettings.ManipulationTranslateY
                    | GestureSettings.ManipulationRotate
                    | GestureSettings.ManipulationTranslateInertia
                    | GestureSettings.ManipulationRotateInertia;
            }
            if (Control != null && e.OldElement != null)
            {
                Control.Draw -= Control_Draw;
                Control.SizeChanged -= Control_SizeChanged;
                Control.ManipulationDelta -= Control_ManipulationDelta;
                Control.PointerWheelChanged -= Control_PointerWheelChanged;
            }
            if (Control != null && e.NewElement != null)
            {
                Control.Draw += Control_Draw;
                Control.SizeChanged += Control_SizeChanged;
                Control.ManipulationDelta += Control_ManipulationDelta;
                Control.PointerWheelChanged += Control_PointerWheelChanged;

                Control.PointerPressed += OnPointerPressed;
                Control.PointerMoved += OnPointerMoved;
                Control.PointerReleased += OnPointerReleased;
                Control.PointerCanceled += OnPointerCanceled;

                recognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting;
                recognizer.ManipulationStarted += OnManipulationStarted;
                recognizer.ManipulationUpdated += OnManipulationUpdated;
                recognizer.ManipulationCompleted += OnManipulationCompleted;
            }
            base.OnElementChanged(e);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            Control.CapturePointer(args.Pointer);
            recognizer.ProcessDownEvent(args.GetCurrentPoint(Control));
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            recognizer.ProcessMoveEvents(args.GetIntermediatePoints(Control));
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            recognizer.ProcessUpEvent(args.GetCurrentPoint(Control));
            Control.ReleasePointerCapture(args.Pointer);
        }

        void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            recognizer.CompleteGesture();
            Control.ReleasePointerCapture(args.Pointer);
        }

        void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
        }

        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            //previousTransform.Matrix = cumulativeTransform.Value;

            //// Get the center point of the manipulation for rotation
            //Point center = new Point(e.Position.X, e.Position.Y);
            //deltaTransform.CenterX = center.X;
            //deltaTransform.CenterY = center.Y;

            //// Look at the Delta property of the ManipulationDeltaRoutedEventArgs to retrieve
            //// the rotation, X, and Y changes
            //deltaTransform.Rotation = e.Delta.Rotation;
            //deltaTransform.TranslateX = e.Delta.Translation.X;
            //deltaTransform.TranslateY = e.Delta.Translation.Y;
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == DrawPanel.ImageDataProperty.PropertyName
                || e.PropertyName == DrawPanel.ImagePixelOfByteSizeProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageOriginProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageWidthProperty.PropertyName
                || e.PropertyName == DrawPanel.ImageHeightProperty.PropertyName)
            {
                Image = await Task<CanvasBitmap>.Run(() => {
                    var dvc = CanvasDevice.GetSharedDevice();
                    return CanvasBitmap.CreateFromBytes(dvc, Element.ImageData, Element.ImageWidth, Element.ImageHeight, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, 96);
                });
                if (UpdateImage())
                    Control.Invalidate();
            }
        }

        private void Control_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (UpdateImage())
                Control.Invalidate();
        }

        private void Control_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(Control).Properties.MouseWheelDelta;
            double oldScale = Scale;
            if (delta > 0)
            {
                Scale *= 0.2 * delta / 120 + 1;
            }
            else
            {
                Scale += 0.2 * delta / 120 * Scale;
            }
            if (UpdateImage())
                Control.Invalidate();
        }

        private void Control_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            Scale *= e.Delta.Scale;
            if (UpdateImage())
                Control.Invalidate();
        }

        private void Control_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Image != null)
            {
                args.DrawingSession.DrawImage(Image, DrawRect);
            }
        }
    }


    // https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/BasicInput Basic input sample
    public sealed partial class Scenario5 : Windows.UI.Xaml.Controls.Page
    {
        Windows.UI.Input.GestureRecognizer recognizer;
        ManipulationInputProcessor manipulationProcessor;

        public Scenario5()
        {
            //this.InitializeComponent();

            InitOptions();

            // Create a GestureRecognizer which will be used to process the manipulations
            // done on the rectangle
            recognizer = new Windows.UI.Input.GestureRecognizer();

            // Create a ManipulationInputProcessor which will listen for events on the
            // rectangle, process them, and update the rectangle's position, size, and rotation
            ////manipulationProcessor = new ManipulationInputProcessor(recognizer, manipulateMe, mainCanvas);
            //< Canvas x: Name = "mainCanvas" ManipulationMode = "None" Margin = "0,12,0,0" MinHeight = "400" >
            //    < Border x: Name = "manipulateMe" Background = "LightGray" Height = "200" Width = "200"  ManipulationMode = "All" />
            //</ Canvas >
        }

        private void InitOptions()
        {
            ////movementAxis.SelectedIndex = 0;
            ////InertiaSwitch.IsOn = true;
            //< ComboBox Name = "movementAxis" Header = "Movement Axis" SelectionChanged = "movementAxis_Changed" VerticalAlignment = "Center" Margin = "0,12,0,0" >
            //    < ComboBoxItem Content = "X and Y" />
            //    < ComboBoxItem Content = "X only" />
            //    < ComboBoxItem Content = "Y only" />
            //</ ComboBox >
            //< ToggleSwitch x: Name = "InertiaSwitch" Header = "Inertia" Margin = "0,12,0,0" Toggled = "InertiaSwitch_Toggled" />
        }

        private void movementAxis_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (manipulationProcessor == null)
            {
                return;
            }

            ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            switch (selectedItem.Content.ToString())
            {
                case "X only":
                    manipulationProcessor.LockToXAxis();
                    break;
                case "Y only":
                    manipulationProcessor.LockToYAxis();
                    break;
                default:
                    manipulationProcessor.MoveOnXAndYAxes();
                    break;
            }
        }

        private void InertiaSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (manipulationProcessor == null)
            {
                return;
            }

            ////manipulationProcessor.UseInertia(InertiaSwitch.IsOn);
        }

        void resetButton_Pressed(object sender, RoutedEventArgs e)
        {
            InitOptions();
            manipulationProcessor.Reset();
        }
    }

    class ManipulationInputProcessor
    {
        Windows.UI.Input.GestureRecognizer recognizer;
        Windows.UI.Xaml.UIElement element;
        Windows.UI.Xaml.UIElement reference;
        TransformGroup cumulativeTransform;
        MatrixTransform previousTransform;
        CompositeTransform deltaTransform;

        public ManipulationInputProcessor(Windows.UI.Input.GestureRecognizer gestureRecognizer, UIElement target, UIElement referenceFrame)
        {
            recognizer = gestureRecognizer;
            element = target;
            reference = referenceFrame;

            // Initialize the transforms that will be used to manipulate the shape
            InitializeTransforms();

            // The GestureSettings property dictates what manipulation events the
            // Gesture Recognizer will listen to.  This will set it to a limited
            // subset of these events.
            recognizer.GestureSettings = GenerateDefaultSettings();

            // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.
            element.PointerPressed += OnPointerPressed;
            element.PointerMoved += OnPointerMoved;
            element.PointerReleased += OnPointerReleased;
            element.PointerCanceled += OnPointerCanceled;

            // Set up event handlers to respond to gesture recognizer output
            recognizer.ManipulationStarted += OnManipulationStarted;
            recognizer.ManipulationUpdated += OnManipulationUpdated;
            recognizer.ManipulationCompleted += OnManipulationCompleted;
            recognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting;
        }

        public void InitializeTransforms()
        {
            cumulativeTransform = new TransformGroup();
            deltaTransform = new CompositeTransform();
            previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };

            cumulativeTransform.Children.Add(previousTransform);
            cumulativeTransform.Children.Add(deltaTransform);

            element.RenderTransform = cumulativeTransform;
        }

        // Return the default GestureSettings for this sample
        GestureSettings GenerateDefaultSettings()
        {
            return GestureSettings.ManipulationTranslateX |
                GestureSettings.ManipulationTranslateY |
                GestureSettings.ManipulationRotate |
                GestureSettings.ManipulationTranslateInertia |
                GestureSettings.ManipulationRotateInertia;
        }

        // Route the pointer pressed event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            // Set the pointer capture to the element being interacted with so that only it
            // will fire pointer-related events
            element.CapturePointer(args.Pointer);

            // Feed the current point into the gesture recognizer as a down event
            recognizer.ProcessDownEvent(args.GetCurrentPoint(reference));
        }

        // Route the pointer moved event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            // Feed the set of points into the gesture recognizer as a move event
            recognizer.ProcessMoveEvents(args.GetIntermediatePoints(reference));
        }

        // Route the pointer released event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            // Feed the current point into the gesture recognizer as an up event
            recognizer.ProcessUpEvent(args.GetCurrentPoint(reference));

            // Release the pointer
            element.ReleasePointerCapture(args.Pointer);
        }

        // Route the pointer canceled event to the gesture recognizer.
        // The points are in the reference frame of the canvas that contains the rectangle element.
        void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            recognizer.CompleteGesture();
            element.ReleasePointerCapture(args.Pointer);
        }

        // When a manipulation begins, change the color of the object to reflect
        // that a manipulation is in progress
        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Border b = element as Border;
            b.Background = new SolidColorBrush(Windows.UI.Colors.DeepSkyBlue);
        }

        // Process the change resulting from a manipulation
        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            previousTransform.Matrix = cumulativeTransform.Value;

            // Get the center point of the manipulation for rotation
            Point center = new Point(e.Position.X, e.Position.Y);
            deltaTransform.CenterX = center.X;
            deltaTransform.CenterY = center.Y;

            // Look at the Delta property of the ManipulationDeltaRoutedEventArgs to retrieve
            // the rotation, X, and Y changes
            deltaTransform.Rotation = e.Delta.Rotation;
            deltaTransform.TranslateX = e.Delta.Translation.X;
            deltaTransform.TranslateY = e.Delta.Translation.Y;
        }

        // When a manipulation that's a result of inertia begins, change the color of the
        // the object to reflect that inertia has taken over
        void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            Border b = element as Border;
            b.Background = new SolidColorBrush(Windows.UI.Colors.RoyalBlue);
        }

        // When a manipulation has finished, reset the color of the object
        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Border b = element as Border;
            b.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
        }

        // Modify the GestureSettings property to only allow movement on the X axis
        public void LockToXAxis()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
            recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateY;
        }

        // Modify the GestureSettings property to only allow movement on the Y axis
        public void LockToYAxis()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
            recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateX;
        }

        // Modify the GestureSettings property to allow movement on both the the X and Y axes
        public void MoveOnXAndYAxes()
        {
            recognizer.CompleteGesture();
            recognizer.GestureSettings |= GestureSettings.ManipulationTranslateX | GestureSettings.ManipulationTranslateY;
        }

        // Modify the GestureSettings property to enable or disable inertia based on the passed-in value
        public void UseInertia(bool inertia)
        {
            if (!inertia)
            {
                recognizer.CompleteGesture();
                recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
            }
            else
            {
                recognizer.GestureSettings |= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
            }
        }

        public void Reset()
        {
            element.RenderTransform = null;
            recognizer.CompleteGesture();
            InitializeTransforms();
            recognizer.GestureSettings = GenerateDefaultSettings();
        }
    }
}
