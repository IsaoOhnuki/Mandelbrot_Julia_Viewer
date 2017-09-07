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
        public ObservableCollection<GradationDrawer.ColPos> Parette { get; set; }

        public GradationDrawer.ColPos[] ColorParette
        {
            get { return Parette.ToArray(); }
            set
            {
                Parette.Clear();
                foreach (var val in value)
                    Parette.Add(new GradationDrawer.ColPos { Color = val.Color, Position = (int)(val.Position ) });
            }
        }

        private void Parette_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                int insertIndex = e.NewStartingIndex;
                foreach (var parette in e.NewItems)
                {
                    StackLayout.Children.Insert(insertIndex++, NewItem(parette));
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

        private View NewItem(object parette)
        {
            var layout = new Grid();
            layout.BindingContext = parette;
            layout.ColumnDefinitions = new ColumnDefinitionCollection();
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
            layout.BackgroundColor = Color.DarkKhaki;
            layout.HorizontalOptions = LayoutOptions.FillAndExpand;
            var slider = new Slider();
            slider.Maximum = 100;
            slider.SetBinding(Slider.ValueProperty, new Binding("Position", BindingMode.TwoWay));
            slider.HorizontalOptions = LayoutOptions.FillAndExpand;
            slider.ValueChanged += Parette_ValueChanged;
            slider.SetValue(Grid.ColumnProperty, 0);
            layout.Children.Add(slider);
            var colsel = new ColorPicker();
            colsel.SetBinding(ColorPicker.SelectedColorProperty, new Binding("Color", BindingMode.TwoWay));
            colsel.ColorChanged += ColorChanged;
            colsel.SetValue(Grid.ColumnProperty, 1);
            layout.Children.Add(colsel);
            var addButton = new Button();
            addButton.Text = "＋";
            addButton.Clicked += AddButton_Clicked;
            addButton.SetValue(Grid.ColumnProperty, 2);
            layout.Children.Add(addButton);
            var subButton = new Button();
            subButton.Text = "－";
            subButton.Clicked += SubButton_Clicked;
            subButton.SetValue(Grid.ColumnProperty, 3);
            layout.Children.Add(subButton);
            return layout;
        }

        private void ColorChanged(object sender, ColorChangedEventArgs e)
        {
            ((MJViewModel)BindingContext).ColorParette = ColorParette.Select(x => new ColorResolutionStruct { Color = x.Color, Position = (double)x.Position / 100 }).ToArray();
        }

        private void Parette_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            ((MJViewModel)BindingContext).ColorParette = ColorParette.Select(x => new ColorResolutionStruct { Color = x.Color, Position = (double)x.Position / 100 }).ToArray();
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            int index = StackLayout.Children.IndexOf(((View)sender).Parent as View);
            Parette.Insert(index, new GradationDrawer.ColPos { Color = Parette[index].Color, Position = Parette[index].Position });
            ((MJViewModel)BindingContext).ColorParette = ColorParette.Select(x => new ColorResolutionStruct { Color = x.Color, Position = (double)x.Position / 100 }).ToArray();
        }

        private void SubButton_Clicked(object sender, EventArgs e)
        {
            int index = StackLayout.Children.IndexOf(((View)sender).Parent as View);
            Parette.RemoveAt(index);
            ((MJViewModel)BindingContext).ColorParette = ColorParette.Select(x => new ColorResolutionStruct { Color = x.Color, Position = (double)x.Position / 100 }).ToArray();
        }

        public ColorParettePage()
        {
            Parette = new ObservableCollection<GradationDrawer.ColPos>();
            Parette.CollectionChanged += Parette_CollectionChanged;

            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem("<", "", () => { }, ToolbarItemOrder.Default));
        }

        public ColorParettePage(MJViewModel viewModel)
        {
            Parette = new ObservableCollection<GradationDrawer.ColPos>();
            Parette.CollectionChanged += Parette_CollectionChanged;

            InitializeComponent();

            BindingContext = viewModel;
            GradationDrawer.BindingContext = this;
            ColorParette = viewModel.ColorParette.Select(x => new GradationDrawer.ColPos { Color = x.Color, Position = (int)(x.Position * 100) }).ToArray();
            ToolbarItems.Add(new ToolbarItem("←", "", () => { ((MasterDetailPage)Parent).Detail = new DrawPage(viewModel); }, ToolbarItemOrder.Default));
        }
    }
}