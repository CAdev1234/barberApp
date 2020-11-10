
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupportFeedbackPage : ContentPage
    {
        public static int Feedback = 1;
        public static int Support = 2;
        public int m_type;
        private PopupPage popupPage = new LoadPopupPage();
        public SupportFeedbackPage(int type)
        {
            InitializeComponent();
            m_type = type;
            if (m_type == Support)
                FeedBack.Placeholder = "DESCRIPTION";
            Email.Text = SharedService.GetUser().User.email;
        }

        private async void send(object sender, EventArgs e)
        {
            if(Email.Text == null || Email.Text == "")
            {
                HandleToast.Toast("Invalid Email.");
                return;
            }
            if(FeedBack.Text == null || FeedBack.Text == "")
            {
                if(m_type == Feedback)
                    HandleToast.Toast("Invalid Feedback.");
                else
                    HandleToast.Toast("Invalid Description.");
                return;
            }
            SupportFeedback feedback = new SupportFeedback();
            feedback.type = m_type;
            feedback.email = Email.Text;
            feedback.description = FeedBack.Text;
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, SupportFeedback>(feedback, "suportfeedback");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if(res.Success)
            {
                HandleToast.Toast("Succesfully send!");
            }
        }
    }
}