using Controls;
using Mandelbrot_Julia_Viewer.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mandelbrot_Julia_Viewer.Views
{
    // http://itblogdsi.blog.fc2.com/blog-entry-194.html PickerにHorizontalTextAlignmentを実装する方法 | Xamarin.Forms

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorParettePage : ContentPage
    {
        public ObservableCollection<Mandelbrot_Julia.ColorResolutionStruct> Parette { get; set; }
        public Mandelbrot_Julia.ColorResolutionStruct[] ColorParette
        {
            get { return Parette.ToArray(); }
            set
            {
                Parette.Clear();
                foreach (var val in value)
                    Parette.Add(val);
            }
        }

        private void Parette_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                int insertIndex = e.NewStartingIndex;
                foreach (var parette in e.NewItems)
                {
                    var layout = new Grid();
                    layout.BindingContext = parette;
                    layout.ColumnDefinitions = new ColumnDefinitionCollection();
                    layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
                    layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    layout.BackgroundColor = Color.DarkKhaki;
                    layout.HorizontalOptions = LayoutOptions.FillAndExpand;
                    var slider = new Slider();
                    slider.SetBinding(Slider.ValueProperty, new Binding("Posision", BindingMode.TwoWay));
                    slider.HorizontalOptions = LayoutOptions.FillAndExpand;
                    slider.SetValue(Grid.ColumnProperty, 0);
                    layout.Children.Add(slider);
                    var colsel = new ColorPicker();
                    colsel.SetBinding(ColorPicker.SelectedColorProperty, new Binding("Color", BindingMode.TwoWay));
                    colsel.SetValue(Grid.ColumnProperty, 1);
                    layout.Children.Add(colsel);
                    StackLayout.Children.Insert(insertIndex ++, layout);
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                for (int l = 0; l < e.OldItems.Count; ++l)
                {
                    StackLayout.Children.RemoveAt(e.OldStartingIndex + l);
                }
            }
        }

        public ColorParettePage()
        {
            Parette = new ObservableCollection<Mandelbrot_Julia.ColorResolutionStruct>();
            Parette.CollectionChanged += Parette_CollectionChanged;

            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem("<", "", () => { }, ToolbarItemOrder.Default));
        }

        public ColorParettePage(MJViewModel viewModel)
        {
            Parette = new ObservableCollection<Mandelbrot_Julia.ColorResolutionStruct>();
            Parette.CollectionChanged += Parette_CollectionChanged;

            InitializeComponent();

            ColorParette = viewModel.ColorParette;
            ToolbarItems.Add(new ToolbarItem("←", "", () => { ((MasterDetailPage)Parent).Detail = new DrawPage(viewModel); }, ToolbarItemOrder.Default));
        }
    }
}