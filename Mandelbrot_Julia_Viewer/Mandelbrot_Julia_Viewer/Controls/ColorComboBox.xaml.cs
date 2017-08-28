using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorComboBox : ContentView
    {
        // http://www.losttechnology.jp/WebDesign/colorlist.html さまざまな色一覧
        // http://matatabi-ux.hateblo.jp/entry/2014/09/26/120000 Xamarin.Forms の View にバインド可能プロパティを追加する
        // http://qiita.com/sukobuto/items/c8a12070324e713828ea Xamarin.Forms で独自コントロールを定義する
        // https://forums.xamarin.com/discussion/64654/dialog-color-picker ColorPicker
        public ObservableCollection<ColorPlus> ColorEnum { get; set; }

        private ColorPlus selectedColorPlus;
        public ColorPlus SelectedColorPlus
        {
            get { return selectedColorPlus; }
            set
            {
                if (selectedColorPlus != value)
                {
                    selectedColorPlus = value;
                    SelectedColor = selectedColorPlus.Color;
                }
            }
        }

        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(Value), // プロパティ名
            typeof(TimeSpan), // プロパティの型
            typeof(ColorComboBox), // プロパティを持つ View の型
            TimeSpan.FromMinutes(1), // 初期値
            BindingMode.TwoWay, // バインド方向
            null, // バリデーションメソッド
            (BindableObject bindable, object oldValue, object newValue) => { }, // 変更後イベントハンドラ
            null, // 変更時イベントハンドラ
            null); // BindableProperty.CoerceValueDelegate Xamarin 公式にも説明なしなので用途不明


        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ColorComboBox),
            Color.White,
            BindingMode.TwoWay,
            null,
            (BindableObject d, object oldValue, object newValue) =>
            {
                if (newValue == oldValue)
                    return;
                ColorComboBox obj = d as ColorComboBox;
                obj.SelectedColorPlus = obj.ColorEnum.Where(x => x.Color == (Color)newValue).FirstOrDefault();
                obj.OnPropertyChanged(nameof(SelectedColor));
            });

        public ColorComboBox()
        {
            var colList = new List<ColorStruct>(ColorList.Colors.AsEnumerable().Distinct());

            ColorEnum = new ObservableCollection<ColorPlus>(colList
                .Select(x => new ColorPlus
                {
                    Color = x.Color,
                    TextColor = ColorToValue(x.Color) < 0.44 ? Color.White : Color.Black,
                    Name = x.Name,
                    Hue = ColorToHue(x.Color),
                    //Brightness = ColorToBrightness(x.Color),
                    //Lightness = ColorToLightness(x.Color),
                    //Saturation = ColorToSaturation(x.Color),
                    Lightness = ColorToLightness(x.Color) == 0 ? 0 : ColorToLightness(x.Color) < 0.25 ? 0.25 : ColorToLightness(x.Color) < 0.5 ? 0.49 : ColorToLightness(x.Color) == 0.5 ? 0.5 : ColorToLightness(x.Color) < 0.75 ? 0.75 : 1,
                    Brightness = ColorToBrightness(x.Color) == 0 ? 0 : ColorToBrightness(x.Color) < 0.2 ? 0.2 : ColorToBrightness(x.Color) < 0.4 ? 0.4 : ColorToBrightness(x.Color) < 0.6 ? 0.6 : ColorToBrightness(x.Color) < 0.8 ? 0.8 : 1,
                    Saturation = ColorToSaturation(x.Color) == 0 ? 0 : ColorToSaturation(x.Color) < 0.2 ? 0.2 : ColorToSaturation(x.Color) < 0.4 ? 0.4 : ColorToSaturation(x.Color) < 0.6 ? 0.6 : ColorToSaturation(x.Color) < 0.8 ? 0.8 : 1,
                })
                .OrderByDescending(x => ColorToGray(x.Color))
                .ThenByDescending(x => x.Color.R == 0 && x.Color.G == 0 && x.Color.B == 0 ? 1 : x.Lightness)
                .ThenBy(x => x.Color.R == 0 && x.Color.G == 0 && x.Color.B == 0 ? 0 : x.Saturation)
                .ThenBy(x => x.Hue)
                );

            SelectedColorPlus = ColorEnum.Where(x => x.Color == Color.White).FirstOrDefault();

            InitializeComponent();

            BindingContext = this;
        }

        public double ColorToGray(Color col)
        {
            if (col.R == col.G && col.R == col.B)
                return (double)col.R / 255;
            else
                return 0;
        }

        public static double ColorToValue(Color col)
        {
            return (
                //((double)col.A / 255)
                //+
                ((double)col.R / 255) * 1.6
                +
                ((double)col.G / 255) * 2.4
                +
                ((double)col.B / 255)
                ) / 5.0;
        }

        public static double ColorToStrength(Color col)
        {
            return ((((int)col.A) << 24) + (((int)col.R) << 16) + (((int)col.G) << 8) + ((int)col.B));
        }

        public static double ColorToHue(Color col)
        {
            double max = 0;
            double min = 255;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            if (min > col.R)
                min = col.R;
            if (min > col.G)
                min = col.G;
            if (min > col.B)
                min = col.B;
            double ret = 0;
            if (col.B <= col.G && col.B <= col.R)
            {
                ret = 60.0 * ((col.G - col.R) / (max - min)) + 60;
            }
            if (col.R <= col.G && col.R <= col.B)
            {
                ret = 60.0 * ((col.B - col.G) / (max - min)) + 180;
            }
            if (col.G <= col.R && col.G <= col.B)
            {
                ret = 60.0 * ((col.R - col.B) / (max - min)) + 300;
            }
            return ret;
        }

        public static double ColorToSaturation(Color col)
        {
            double max = 0;
            double min = 255;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            if (min > col.R)
                min = col.R;
            if (min > col.G)
                min = col.G;
            if (min > col.B)
                min = col.B;
            return (max - min) / max;
        }

        public static double ColorToLightness(Color col)
        {
            double max = 0;
            double min = 255;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            if (min > col.R)
                min = col.R;
            if (min > col.G)
                min = col.G;
            if (min > col.B)
                min = col.B;
            return (double)(max + min) / 255 / 2;
        }

        public static double ColorToBrightness(Color col)
        {
            double max = 0;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            return max / 255;
        }

        public class ColorStruct
        {
            public Color Color { get; set; }
            public string Name { get; set; }
            public override bool Equals(object obj)
            {
                ColorStruct v = obj as ColorStruct;
                return v != null && Color.Equals(v.Color);
            }
            public override int GetHashCode()
            {
                return Color.GetHashCode();
            }
        }

        //[TypeConverter(typeof(ColorPlusColorTypeConverter))]
        public class ColorPlus : BindableObject
        {
            public ColorPlus()
            {
                Color = Color.White;
                TextColor = Color.Black;
                Name = "White";
                Hue = ColorToHue(Color.White);
                Saturation = ColorToSaturation(Color.White);
                Brightness = ColorToBrightness(Color.White);
                Lightness = ColorToLightness(Color.White);
            }
            private Color color;
            public Color Color
            {
                get { return color; }
                set { color = value; OnPropertyChanged(); }
            }
            private Color textColor;
            public Color TextColor
            {
                get { return textColor; }
                set { textColor = value; OnPropertyChanged(); }
            }
            public override bool Equals(object obj)
            {
                ColorPlus v = obj as ColorPlus;
                return v != null && Color.Equals(v.Color);
            }
            public override int GetHashCode()
            {
                return Color.GetHashCode();
            }
            private string name;
            public string Name
            {
                get { return name; }
                set { name = value; OnPropertyChanged(); }
            }
            private double hue;
            public double Hue
            {
                get { return hue; }
                set { hue = value; OnPropertyChanged(); }
            }
            private double saturation;
            public double Saturation
            {
                get { return saturation; }
                set { saturation = value; OnPropertyChanged(); }
            }
            private double brightness;
            public double Brightness
            {
                get { return brightness; }
                set { brightness = value; OnPropertyChanged(); }
            }
            private double lightness;
            public double Lightness
            {
                get { return lightness; }
                set { lightness = value; OnPropertyChanged(); }
            }
            public static implicit operator ColorPlus(Color value)
            {
                var colList = ColorList.Colors.Where(x => value == x.Color).FirstOrDefault();
                return new ColorPlus
                {
                    Color = value,
                    TextColor = ColorToValue(value) < 0.6 ? Color.White : Color.Black,
                    Name = colList?.Name ?? "NoName",
                    Hue = ColorToHue(value),
                    Saturation = ColorToSaturation(value),
                    Brightness = ColorToBrightness(value),
                    Lightness = ColorToLightness(value)
                };
            }
            public static implicit operator Color(ColorPlus value)
            {
                return value.Color;
            }
        }
    }

    //public class ColorPlusColorTypeConverter : TypeConverter
    //{
    //    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    //    {
    //        return destinationType == typeof(ColorComboBox.ColorPlus);
    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    //    {
    //        var val = (ColorComboBox.ColorPlus)value;
    //        return (Color)val;
    //    }

    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return sourceType == typeof(Color);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    //    {
    //        var val = (Color)value;
    //        ColorComboBox.ColorPlus ret = val;
    //        return ret;
    //    }
    //}

    //public class ColorPlusColorConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var val = (Color)value;
    //        ColorComboBox.ColorPlus ret = val;
    //        return ret;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var val = (ColorComboBox.ColorPlus)value;
    //        return (Color)val;
    //    }
    //}

    //public class ColorPlusBrushConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var val = (Color)value;
    //        Brush ret = new SolidColorBrush(val);
    //        return ret;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var val = ((SolidColorBrush)value).Color;
    //        return (Color)val;
    //    }
    //}

    public class ColorList
    {
        public static ColorComboBox.ColorStruct[] Colors
        {
            get
            {
                return new ColorComboBox.ColorStruct[] {
                    new ColorComboBox.ColorStruct {Color = Color.AliceBlue, Name = "AliceBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.MintCream, Name = "MintCream"},
                    new ColorComboBox.ColorStruct {Color = Color.MistyRose, Name = "MistyRose"},
                    new ColorComboBox.ColorStruct {Color = Color.Moccasin, Name = "Moccasin"},
                    new ColorComboBox.ColorStruct {Color = Color.NavajoWhite, Name = "NavajoWhite"},
                    new ColorComboBox.ColorStruct {Color = Color.Navy, Name = "Navy"},
                    new ColorComboBox.ColorStruct {Color = Color.OldLace, Name = "OldLace"},
                    new ColorComboBox.ColorStruct {Color = Color.MidnightBlue, Name = "MidnightBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Olive, Name = "Olive"},
                    new ColorComboBox.ColorStruct {Color = Color.Orange, Name = "Orange"},
                    new ColorComboBox.ColorStruct {Color = Color.OrangeRed, Name = "OrangeRed"},
                    new ColorComboBox.ColorStruct {Color = Color.Orchid, Name = "Orchid"},
                    new ColorComboBox.ColorStruct {Color = Color.PaleGoldenrod, Name = "PaleGoldenrod"},
                    new ColorComboBox.ColorStruct {Color = Color.PaleGreen, Name = "PaleGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.PaleTurquoise, Name = "PaleTurquoise"},
                    new ColorComboBox.ColorStruct {Color = Color.OliveDrab, Name = "OliveDrab"},
                    new ColorComboBox.ColorStruct {Color = Color.PaleVioletRed, Name = "PaleVioletRed"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumVioletRed, Name = "MediumVioletRed"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumSpringGreen, Name = "MediumSpringGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.LightSkyBlue, Name = "LightSkyBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.LightSlateGray, Name = "LightSlateGray"},
                    new ColorComboBox.ColorStruct {Color = Color.LightSteelBlue, Name = "LightSteelBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.LightYellow, Name = "LightYellow"},
                    new ColorComboBox.ColorStruct {Color = Color.Lime, Name = "Lime"},
                    new ColorComboBox.ColorStruct {Color = Color.LimeGreen, Name = "LimeGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumTurquoise, Name = "MediumTurquoise"},
                    new ColorComboBox.ColorStruct {Color = Color.Linen, Name = "Linen"},
                    new ColorComboBox.ColorStruct {Color = Color.Maroon, Name = "Maroon"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumAquamarine, Name = "MediumAquamarine"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumBlue, Name = "MediumBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumOrchid, Name = "MediumOrchid"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumPurple, Name = "MediumPurple"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumSlateBlue, Name = "MediumSlateBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Magenta, Name = "Magenta"},
                    new ColorComboBox.ColorStruct {Color = Color.LightSeaGreen, Name = "LightSeaGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.PapayaWhip, Name = "PapayaWhip"},
                    new ColorComboBox.ColorStruct {Color = Color.Peru, Name = "Peru"},
                    new ColorComboBox.ColorStruct {Color = Color.SpringGreen, Name = "SpringGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.SteelBlue, Name = "SteelBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Tan, Name = "Tan"},
                    new ColorComboBox.ColorStruct {Color = Color.Teal, Name = "Teal"},
                    new ColorComboBox.ColorStruct {Color = Color.Thistle, Name = "Thistle"},
                    new ColorComboBox.ColorStruct {Color = Color.Tomato, Name = "Tomato"},
                    new ColorComboBox.ColorStruct {Color = Color.Snow, Name = "Snow"},
                    new ColorComboBox.ColorStruct {Color = Color.Transparent, Name = "Transparent"},
                    new ColorComboBox.ColorStruct {Color = Color.Violet, Name = "Violet"},
                    new ColorComboBox.ColorStruct {Color = Color.Wheat, Name = "Wheat"},
                    new ColorComboBox.ColorStruct {Color = Color.White, Name = "White"},
                    new ColorComboBox.ColorStruct {Color = Color.WhiteSmoke, Name = "WhiteSmoke"},
                    new ColorComboBox.ColorStruct {Color = Color.Yellow, Name = "Yellow"},
                    new ColorComboBox.ColorStruct {Color = Color.YellowGreen, Name = "YellowGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.Turquoise, Name = "Turquoise"},
                    new ColorComboBox.ColorStruct {Color = Color.PeachPuff, Name = "PeachPuff"},
                    new ColorComboBox.ColorStruct {Color = Color.SlateGray, Name = "SlateGray"},
                    new ColorComboBox.ColorStruct {Color = Color.SkyBlue, Name = "SkyBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Pink, Name = "Pink"},
                    new ColorComboBox.ColorStruct {Color = Color.Plum, Name = "Plum"},
                    new ColorComboBox.ColorStruct {Color = Color.PowderBlue, Name = "PowderBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Purple, Name = "Purple"},
                    new ColorComboBox.ColorStruct {Color = Color.Red, Name = "Red"},
                    new ColorComboBox.ColorStruct {Color = Color.RosyBrown, Name = "RosyBrown"},
                    new ColorComboBox.ColorStruct {Color = Color.SlateBlue, Name = "SlateBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.RoyalBlue, Name = "RoyalBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Salmon, Name = "Salmon"},
                    new ColorComboBox.ColorStruct {Color = Color.SandyBrown, Name = "SandyBrown"},
                    new ColorComboBox.ColorStruct {Color = Color.SeaGreen, Name = "SeaGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.SeaShell, Name = "SeaShell"},
                    new ColorComboBox.ColorStruct {Color = Color.Sienna, Name = "Sienna"},
                    new ColorComboBox.ColorStruct {Color = Color.Silver, Name = "Silver"},
                    new ColorComboBox.ColorStruct {Color = Color.SaddleBrown, Name = "SaddleBrown"},
                    new ColorComboBox.ColorStruct {Color = Color.LightSalmon, Name = "LightSalmon"},
                    new ColorComboBox.ColorStruct {Color = Color.MediumSeaGreen, Name = "MediumSeaGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.LightGreen, Name = "LightGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.Crimson, Name = "Crimson"},
                    new ColorComboBox.ColorStruct {Color = Color.LightPink, Name = "LightPink"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkBlue, Name = "DarkBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkCyan, Name = "DarkCyan"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkGoldenrod, Name = "DarkGoldenrod"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkGray, Name = "DarkGray"},
                    new ColorComboBox.ColorStruct {Color = Color.Cornsilk, Name = "Cornsilk"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkGreen, Name = "DarkGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkMagenta, Name = "DarkMagenta"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkOliveGreen, Name = "DarkOliveGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkOrange, Name = "DarkOrange"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkOrchid, Name = "DarkOrchid"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkRed, Name = "DarkRed"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkSalmon, Name = "DarkSalmon"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkKhaki, Name = "DarkKhaki"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkSeaGreen, Name = "DarkSeaGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.CornflowerBlue, Name = "CornflowerBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Chocolate, Name = "Chocolate"},
                    new ColorComboBox.ColorStruct {Color = Color.AntiqueWhite, Name = "AntiqueWhite"},
                    new ColorComboBox.ColorStruct {Color = Color.Aqua, Name = "Aqua"},
                    new ColorComboBox.ColorStruct {Color = Color.Aquamarine, Name = "Aquamarine"},
                    new ColorComboBox.ColorStruct {Color = Color.Azure, Name = "Azure"},
                    new ColorComboBox.ColorStruct {Color = Color.Beige, Name = "Beige"},
                    new ColorComboBox.ColorStruct {Color = Color.Bisque, Name = "Bisque"},
                    new ColorComboBox.ColorStruct {Color = Color.Coral, Name = "Coral"},
                    new ColorComboBox.ColorStruct {Color = Color.Black, Name = "Black"},
                    new ColorComboBox.ColorStruct {Color = Color.Blue, Name = "Blue"},
                    new ColorComboBox.ColorStruct {Color = Color.BlueViolet, Name = "BlueViolet"},
                    new ColorComboBox.ColorStruct {Color = Color.Brown, Name = "Brown"},
                    new ColorComboBox.ColorStruct {Color = Color.BurlyWood, Name = "BurlyWood"},
                    new ColorComboBox.ColorStruct {Color = Color.CadetBlue, Name = "CadetBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Chartreuse, Name = "Chartreuse"},
                    new ColorComboBox.ColorStruct {Color = Color.BlanchedAlmond, Name = "BlanchedAlmond"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkSlateBlue, Name = "DarkSlateBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Cyan, Name = "Cyan"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkTurquoise, Name = "DarkTurquoise"},
                    new ColorComboBox.ColorStruct {Color = Color.HotPink, Name = "HotPink"},
                    new ColorComboBox.ColorStruct {Color = Color.IndianRed, Name = "IndianRed"},
                    new ColorComboBox.ColorStruct {Color = Color.Indigo, Name = "Indigo"},
                    new ColorComboBox.ColorStruct {Color = Color.Ivory, Name = "Ivory"},
                    new ColorComboBox.ColorStruct {Color = Color.Khaki, Name = "Khaki"},
                    new ColorComboBox.ColorStruct {Color = Color.Lavender, Name = "Lavender"},
                    new ColorComboBox.ColorStruct {Color = Color.Honeydew, Name = "Honeydew"},
                    new ColorComboBox.ColorStruct {Color = Color.LavenderBlush, Name = "LavenderBlush"},
                    new ColorComboBox.ColorStruct {Color = Color.LemonChiffon, Name = "LemonChiffon"},
                    new ColorComboBox.ColorStruct {Color = Color.LightBlue, Name = "LightBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.LightCoral, Name = "LightCoral"},
                    new ColorComboBox.ColorStruct {Color = Color.LightCyan, Name = "LightCyan"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkSlateGray, Name = "DarkSlateGray"},
                    new ColorComboBox.ColorStruct {Color = Color.LightGray, Name = "LightGray"},
                    new ColorComboBox.ColorStruct {Color = Color.LawnGreen, Name = "LawnGreen"},
                    new ColorComboBox.ColorStruct {Color = Color.GreenYellow, Name = "GreenYellow"},
                    new ColorComboBox.ColorStruct {Color = Color.LightGoldenrodYellow, Name = "LightGoldenrodYellow"},
                    new ColorComboBox.ColorStruct {Color = Color.Gray, Name = "Gray"},
                    new ColorComboBox.ColorStruct {Color = Color.Green, Name = "Green"},
                    new ColorComboBox.ColorStruct {Color = Color.DarkViolet, Name = "DarkViolet"},
                    new ColorComboBox.ColorStruct {Color = Color.DeepPink, Name = "DeepPink"},
                    new ColorComboBox.ColorStruct {Color = Color.DimGray, Name = "DimGray"},
                    new ColorComboBox.ColorStruct {Color = Color.DodgerBlue, Name = "DodgerBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Firebrick, Name = "Firebrick"},
                    new ColorComboBox.ColorStruct {Color = Color.FloralWhite, Name = "FloralWhite"},
                    new ColorComboBox.ColorStruct {Color = Color.DeepSkyBlue, Name = "DeepSkyBlue"},
                    new ColorComboBox.ColorStruct {Color = Color.Fuchsia, Name = "Fuchsia"},
                    new ColorComboBox.ColorStruct {Color = Color.Gainsboro, Name = "Gainsboro"},
                    new ColorComboBox.ColorStruct {Color = Color.GhostWhite, Name = "GhostWhite"},
                    new ColorComboBox.ColorStruct {Color = Color.Goldenrod, Name = "Goldenrod"},
                    new ColorComboBox.ColorStruct {Color = Color.Gold, Name = "Gold"},
                    new ColorComboBox.ColorStruct {Color = Color.ForestGreen, Name = "ForestGreen" }
                };
            }
        }
    }
}
