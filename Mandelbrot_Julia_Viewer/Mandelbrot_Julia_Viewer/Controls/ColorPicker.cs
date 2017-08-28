using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Controls
{
    public class ColorPicker : ContentView
    {
        public ObservableCollection<ColorStruct> ListItems { get; set; }
        //private ColorStruct[] listItems;
        //public ColorStruct[] ListItems
        //{
        //    get { return listItems; }
        //    set
        //    {
        //        listItems = null;
        //        if (value != null)
        //        {
        //            listItems = new ColorStruct[value.GetLength(0)];
        //            value.CopyTo(listItems, 0);
        //        }
        //    }
        //}

        public ColorPicker()
        {
            ListItems = new ObservableCollection<ColorStruct>(ColorStruct.Colors);

            ListView listView = new ListView();
            listView.BackgroundColor = Color.CornflowerBlue;
            listView.ItemTemplate = new DataTemplate(typeof(ListCell));
            listView.ItemsSource = ListItems;
            Content = listView;
        }

        // http://furuya02.hatenablog.com/entry/2014/08/08/003036 Xamarin.Forms ListViewでTwitter風のレイアウトを作成してみました（機種依存コードなし）
        [Preserve(Conditional = true)]
        private class ListCell : ViewCell
        {
            public ListCell()
            {
                //BoxView boxView = new BoxView();
                //boxView.SetBinding(BoxView.ColorProperty, "Color");
                Label label = new Label();
                label.HorizontalOptions = LayoutOptions.FillAndExpand;
                label.SetBinding(Label.TextProperty, "Name");
                label.SetBinding(Label.TextColorProperty, "Foreground");
                label.SetBinding(Label.BackgroundColorProperty, "Color");
                View = label;
            }
        }

        class PreserveAttribute : System.Attribute
        {
            public bool Conditional { get; set; }
        }
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

        public static ColorStruct[] Colors
        {
            get
            {
                return new ColorStruct[] {
                    new ColorStruct {Color = Color.AliceBlue, Name = "AliceBlue"},
                    new ColorStruct {Color = Color.MintCream, Name = "MintCream"},
                    new ColorStruct {Color = Color.MistyRose, Name = "MistyRose"},
                    new ColorStruct {Color = Color.Moccasin, Name = "Moccasin"},
                    new ColorStruct {Color = Color.NavajoWhite, Name = "NavajoWhite"},
                    new ColorStruct {Color = Color.Navy, Name = "Navy"},
                    new ColorStruct {Color = Color.OldLace, Name = "OldLace"},
                    new ColorStruct {Color = Color.MidnightBlue, Name = "MidnightBlue"},
                    new ColorStruct {Color = Color.Olive, Name = "Olive"},
                    new ColorStruct {Color = Color.Orange, Name = "Orange"},
                    new ColorStruct {Color = Color.OrangeRed, Name = "OrangeRed"},
                    new ColorStruct {Color = Color.Orchid, Name = "Orchid"},
                    new ColorStruct {Color = Color.PaleGoldenrod, Name = "PaleGoldenrod"},
                    new ColorStruct {Color = Color.PaleGreen, Name = "PaleGreen"},
                    new ColorStruct {Color = Color.PaleTurquoise, Name = "PaleTurquoise"},
                    new ColorStruct {Color = Color.OliveDrab, Name = "OliveDrab"},
                    new ColorStruct {Color = Color.PaleVioletRed, Name = "PaleVioletRed"},
                    new ColorStruct {Color = Color.MediumVioletRed, Name = "MediumVioletRed"},
                    new ColorStruct {Color = Color.MediumSpringGreen, Name = "MediumSpringGreen"},
                    new ColorStruct {Color = Color.LightSkyBlue, Name = "LightSkyBlue"},
                    new ColorStruct {Color = Color.LightSlateGray, Name = "LightSlateGray"},
                    new ColorStruct {Color = Color.LightSteelBlue, Name = "LightSteelBlue"},
                    new ColorStruct {Color = Color.LightYellow, Name = "LightYellow"},
                    new ColorStruct {Color = Color.Lime, Name = "Lime"},
                    new ColorStruct {Color = Color.LimeGreen, Name = "LimeGreen"},
                    new ColorStruct {Color = Color.MediumTurquoise, Name = "MediumTurquoise"},
                    new ColorStruct {Color = Color.Linen, Name = "Linen"},
                    new ColorStruct {Color = Color.Maroon, Name = "Maroon"},
                    new ColorStruct {Color = Color.MediumAquamarine, Name = "MediumAquamarine"},
                    new ColorStruct {Color = Color.MediumBlue, Name = "MediumBlue"},
                    new ColorStruct {Color = Color.MediumOrchid, Name = "MediumOrchid"},
                    new ColorStruct {Color = Color.MediumPurple, Name = "MediumPurple"},
                    new ColorStruct {Color = Color.MediumSlateBlue, Name = "MediumSlateBlue"},
                    new ColorStruct {Color = Color.Magenta, Name = "Magenta"},
                    new ColorStruct {Color = Color.LightSeaGreen, Name = "LightSeaGreen"},
                    new ColorStruct {Color = Color.PapayaWhip, Name = "PapayaWhip"},
                    new ColorStruct {Color = Color.Peru, Name = "Peru"},
                    new ColorStruct {Color = Color.SpringGreen, Name = "SpringGreen"},
                    new ColorStruct {Color = Color.SteelBlue, Name = "SteelBlue"},
                    new ColorStruct {Color = Color.Tan, Name = "Tan"},
                    new ColorStruct {Color = Color.Teal, Name = "Teal"},
                    new ColorStruct {Color = Color.Thistle, Name = "Thistle"},
                    new ColorStruct {Color = Color.Tomato, Name = "Tomato"},
                    new ColorStruct {Color = Color.Snow, Name = "Snow"},
                    new ColorStruct {Color = Color.Transparent, Name = "Transparent"},
                    new ColorStruct {Color = Color.Violet, Name = "Violet"},
                    new ColorStruct {Color = Color.Wheat, Name = "Wheat"},
                    new ColorStruct {Color = Color.White, Name = "White"},
                    new ColorStruct {Color = Color.WhiteSmoke, Name = "WhiteSmoke"},
                    new ColorStruct {Color = Color.Yellow, Name = "Yellow"},
                    new ColorStruct {Color = Color.YellowGreen, Name = "YellowGreen"},
                    new ColorStruct {Color = Color.Turquoise, Name = "Turquoise"},
                    new ColorStruct {Color = Color.PeachPuff, Name = "PeachPuff"},
                    new ColorStruct {Color = Color.SlateGray, Name = "SlateGray"},
                    new ColorStruct {Color = Color.SkyBlue, Name = "SkyBlue"},
                    new ColorStruct {Color = Color.Pink, Name = "Pink"},
                    new ColorStruct {Color = Color.Plum, Name = "Plum"},
                    new ColorStruct {Color = Color.PowderBlue, Name = "PowderBlue"},
                    new ColorStruct {Color = Color.Purple, Name = "Purple"},
                    new ColorStruct {Color = Color.Red, Name = "Red"},
                    new ColorStruct {Color = Color.RosyBrown, Name = "RosyBrown"},
                    new ColorStruct {Color = Color.SlateBlue, Name = "SlateBlue"},
                    new ColorStruct {Color = Color.RoyalBlue, Name = "RoyalBlue"},
                    new ColorStruct {Color = Color.Salmon, Name = "Salmon"},
                    new ColorStruct {Color = Color.SandyBrown, Name = "SandyBrown"},
                    new ColorStruct {Color = Color.SeaGreen, Name = "SeaGreen"},
                    new ColorStruct {Color = Color.SeaShell, Name = "SeaShell"},
                    new ColorStruct {Color = Color.Sienna, Name = "Sienna"},
                    new ColorStruct {Color = Color.Silver, Name = "Silver"},
                    new ColorStruct {Color = Color.SaddleBrown, Name = "SaddleBrown"},
                    new ColorStruct {Color = Color.LightSalmon, Name = "LightSalmon"},
                    new ColorStruct {Color = Color.MediumSeaGreen, Name = "MediumSeaGreen"},
                    new ColorStruct {Color = Color.LightGreen, Name = "LightGreen"},
                    new ColorStruct {Color = Color.Crimson, Name = "Crimson"},
                    new ColorStruct {Color = Color.LightPink, Name = "LightPink"},
                    new ColorStruct {Color = Color.DarkBlue, Name = "DarkBlue"},
                    new ColorStruct {Color = Color.DarkCyan, Name = "DarkCyan"},
                    new ColorStruct {Color = Color.DarkGoldenrod, Name = "DarkGoldenrod"},
                    new ColorStruct {Color = Color.DarkGray, Name = "DarkGray"},
                    new ColorStruct {Color = Color.Cornsilk, Name = "Cornsilk"},
                    new ColorStruct {Color = Color.DarkGreen, Name = "DarkGreen"},
                    new ColorStruct {Color = Color.DarkMagenta, Name = "DarkMagenta"},
                    new ColorStruct {Color = Color.DarkOliveGreen, Name = "DarkOliveGreen"},
                    new ColorStruct {Color = Color.DarkOrange, Name = "DarkOrange"},
                    new ColorStruct {Color = Color.DarkOrchid, Name = "DarkOrchid"},
                    new ColorStruct {Color = Color.DarkRed, Name = "DarkRed"},
                    new ColorStruct {Color = Color.DarkSalmon, Name = "DarkSalmon"},
                    new ColorStruct {Color = Color.DarkKhaki, Name = "DarkKhaki"},
                    new ColorStruct {Color = Color.DarkSeaGreen, Name = "DarkSeaGreen"},
                    new ColorStruct {Color = Color.CornflowerBlue, Name = "CornflowerBlue"},
                    new ColorStruct {Color = Color.Chocolate, Name = "Chocolate"},
                    new ColorStruct {Color = Color.AntiqueWhite, Name = "AntiqueWhite"},
                    new ColorStruct {Color = Color.Aqua, Name = "Aqua"},
                    new ColorStruct {Color = Color.Aquamarine, Name = "Aquamarine"},
                    new ColorStruct {Color = Color.Azure, Name = "Azure"},
                    new ColorStruct {Color = Color.Beige, Name = "Beige"},
                    new ColorStruct {Color = Color.Bisque, Name = "Bisque"},
                    new ColorStruct {Color = Color.Coral, Name = "Coral"},
                    new ColorStruct {Color = Color.Black, Name = "Black"},
                    new ColorStruct {Color = Color.Blue, Name = "Blue"},
                    new ColorStruct {Color = Color.BlueViolet, Name = "BlueViolet"},
                    new ColorStruct {Color = Color.Brown, Name = "Brown"},
                    new ColorStruct {Color = Color.BurlyWood, Name = "BurlyWood"},
                    new ColorStruct {Color = Color.CadetBlue, Name = "CadetBlue"},
                    new ColorStruct {Color = Color.Chartreuse, Name = "Chartreuse"},
                    new ColorStruct {Color = Color.BlanchedAlmond, Name = "BlanchedAlmond"},
                    new ColorStruct {Color = Color.DarkSlateBlue, Name = "DarkSlateBlue"},
                    new ColorStruct {Color = Color.Cyan, Name = "Cyan"},
                    new ColorStruct {Color = Color.DarkTurquoise, Name = "DarkTurquoise"},
                    new ColorStruct {Color = Color.HotPink, Name = "HotPink"},
                    new ColorStruct {Color = Color.IndianRed, Name = "IndianRed"},
                    new ColorStruct {Color = Color.Indigo, Name = "Indigo"},
                    new ColorStruct {Color = Color.Ivory, Name = "Ivory"},
                    new ColorStruct {Color = Color.Khaki, Name = "Khaki"},
                    new ColorStruct {Color = Color.Lavender, Name = "Lavender"},
                    new ColorStruct {Color = Color.Honeydew, Name = "Honeydew"},
                    new ColorStruct {Color = Color.LavenderBlush, Name = "LavenderBlush"},
                    new ColorStruct {Color = Color.LemonChiffon, Name = "LemonChiffon"},
                    new ColorStruct {Color = Color.LightBlue, Name = "LightBlue"},
                    new ColorStruct {Color = Color.LightCoral, Name = "LightCoral"},
                    new ColorStruct {Color = Color.LightCyan, Name = "LightCyan"},
                    new ColorStruct {Color = Color.DarkSlateGray, Name = "DarkSlateGray"},
                    new ColorStruct {Color = Color.LightGray, Name = "LightGray"},
                    new ColorStruct {Color = Color.LawnGreen, Name = "LawnGreen"},
                    new ColorStruct {Color = Color.GreenYellow, Name = "GreenYellow"},
                    new ColorStruct {Color = Color.LightGoldenrodYellow, Name = "LightGoldenrodYellow"},
                    new ColorStruct {Color = Color.Gray, Name = "Gray"},
                    new ColorStruct {Color = Color.Green, Name = "Green"},
                    new ColorStruct {Color = Color.DarkViolet, Name = "DarkViolet"},
                    new ColorStruct {Color = Color.DeepPink, Name = "DeepPink"},
                    new ColorStruct {Color = Color.DimGray, Name = "DimGray"},
                    new ColorStruct {Color = Color.DodgerBlue, Name = "DodgerBlue"},
                    new ColorStruct {Color = Color.Firebrick, Name = "Firebrick"},
                    new ColorStruct {Color = Color.FloralWhite, Name = "FloralWhite"},
                    new ColorStruct {Color = Color.DeepSkyBlue, Name = "DeepSkyBlue"},
                    new ColorStruct {Color = Color.Fuchsia, Name = "Fuchsia"},
                    new ColorStruct {Color = Color.Gainsboro, Name = "Gainsboro"},
                    new ColorStruct {Color = Color.GhostWhite, Name = "GhostWhite"},
                    new ColorStruct {Color = Color.Goldenrod, Name = "Goldenrod"},
                    new ColorStruct {Color = Color.Gold, Name = "Gold"},
                    new ColorStruct {Color = Color.ForestGreen, Name = "ForestGreen" }
                };
            }
        }
    }
}