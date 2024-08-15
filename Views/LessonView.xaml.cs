using MAUI_IOT.ViewModels;
using System.Diagnostics;

namespace MAUI_IOT.Views;

public partial class LessonView : ContentPage
{
    public LessonView(LessonViewModel lessonViewModel)
    {
        InitializeComponent();
        BindingContext = lessonViewModel;
    }
}