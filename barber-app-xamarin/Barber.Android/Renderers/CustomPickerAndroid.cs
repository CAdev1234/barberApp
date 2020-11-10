using Android.Content;
using Android.Graphics.Drawables;
using Barber.Droid.Renderers;
using Barber.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerAndroid))]
namespace Barber.Droid.Renderers
{
    public class CustomPickerAndroid : PickerRenderer
    {
        public CustomPickerAndroid(Context context) : base(context)
        {
        }
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                var gradientDrawable = new GradientDrawable();
                gradientDrawable.SetCornerRadius(15);
                //gradientDrawable.SetStroke(5, Android.Graphics.Color.Rgb(247, 148, 29));
                gradientDrawable.SetColor(Android.Graphics.Color.Rgb(34, 34, 34));
                Control.SetBackground(gradientDrawable);
                Control.SetPadding(5, 15, 0, 0);
            }
        }
    }
}