
using Barber.Helper;
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barber.ViewModels
{
    class NotificationViewModel : BaseViewModel
    {
        private PopupPage popupPage = new LoadPopupPage();
        private List<Notification> _notifications;
        public List<Notification> AllNotifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }
        public NotificationViewModel()
        {
            drawNotifications();
        }

        public async void drawNotifications()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            NotificationRequest res = await ApiService.GetOneWithGet<NotificationRequest>("notification");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if(res == null)
            {
                return;
            }

            if(res.Success)
            {
                foreach(Notification item in res.Notifications)
                {
                    item.sender.fullname = item.sender.firstname + " " + item.sender.lastname;
                    item.sender.avatar = ApiService.SERVER_HOST + item.sender.avatar;
                    item.isOther = true;
                    item.isReview = false;
                    item.created_at = DateTimeHelper.getRelativeTime(DateTimeOffset.Parse(item.created_at));
                    if (item.show == 1)
                        item.IsNew = false;
                    else
                        item.IsNew = true;

                    if (item.type == 0)//book
                    {
                        item.book.client.fullname = item.book.client.firstname + " " + item.book.client.lastname;
                        item.book.client.avatar = ApiService.SERVER_HOST + item.book.client.avatar;
                        item.book.barber.fullname = item.book.barber.firstname + " " + item.book.barber.lastname;
                        item.book.barber.avatar = ApiService.SERVER_HOST + item.book.barber.avatar;

                        item.content += item.sender.firstname + " " + item.sender.lastname;
                        if (item.currentstate == 0) 
                        {
                            item.content += " requested";
                            item.title = "Appointment Requested";
                        }
                        else if(item.currentstate == 1)
                        {
                            item.content += " cancelled";
                            item.title = "Appointment Cancelled";
                        }
                        else if (item.currentstate == 2)
                        {
                            item.content += " confirmed";
                            item.title = "Appointment Confirmed";
                        }
                        else if (item.currentstate == 3)
                        {
                            item.content += " completed";
                            item.title = "Appointment Completed";
                        }
                        else if (item.currentstate == 4)
                        {
                            item.content += " completed";
                            item.title = "Appointment Declined";
                        }

                        if (SharedService.GetUser().User.barber)
                            item.content += " an appointment for ";
                        else
                            item.content += " your appointment for ";

                        DateTime formatDate = DateTime.Parse(item.book.date);
                        item.book.date = formatDate.ToString("MM/dd/yyyy");

                        item.content += item.book.date + " at " + item.book.time + ".";
                    }
                    else if(item.type == 1) //review
                    {
                        item.title = "New Review";
                        item.content = "[ " + item.sender.fullname + " ]";
                        item.RateImages = getRatingImage(item.currentstate);

                        item.isOther = false;
                        item.isReview = true;
                    }
                    else if(item.type == 2)
                    {
                        item.title = "New Announcement";
                        item.content = "[ " + item.sender.fullname + " ] " + item.announcement.content;
                    }
                    else if (item.type == 3)
                    {
                        item.title = "New Appointment Comment";
                        if (item.bookcomment.comment.Length > 150)
                        {
                            item.bookcomment.comment = item.bookcomment.comment.Substring(150);
                            item.bookcomment.comment += "...";
                        }
                        item.content = "[ " + item.sender.fullname + " ] " + item.bookcomment.comment;

                        item.book.client.fullname = item.book.client.firstname + " " + item.book.client.lastname;
                        item.book.client.avatar = ApiService.SERVER_HOST + item.book.client.avatar;
                        item.book.barber.fullname = item.book.barber.firstname + " " + item.book.barber.lastname;
                        item.book.barber.avatar = ApiService.SERVER_HOST + item.book.barber.avatar;
                    }
                }

                AllNotifications = res.Notifications;
            }
        }

        private List<RateImage> getRatingImage(int iCount)
        {
            List<RateImage> lstRates = new List<RateImage>();

            for (int i = 0; i < 5; i++)
            {
                RateImage newRate = new RateImage();
                newRate.count = i + 1;
                if (i < iCount)
                    newRate.image = "rating_star_on.png";
                else
                    continue;
                lstRates.Add(newRate);
            }
            return lstRates;
        }
    }
}
