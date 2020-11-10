using Barber.Models;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barber.Services
{
    class SharedService
    {
        public static void SetUser(LoginResponse res)
        {
            Preferences.Set("logged", res.Success);
            Preferences.Set("token", res.Token);
            Preferences.Set("id", res.User.id);
            Preferences.Set("firstname", res.User.firstname);
            Preferences.Set("lastname", res.User.lastname);
            Preferences.Set("fullname", res.User.firstname + " " + res.User.lastname);
            Preferences.Set("email", res.User.email);
            Preferences.Set("barber", res.User.barber);
            Preferences.Set("avatar", ApiService.SERVER_HOST + res.User.avatar);
        }
        public static void SetValue(string key, string value)
        {
            Preferences.Remove(key);
            Preferences.Set(key, value);
        }

        public static string GetValue(string key)
        {
            return Preferences.Get(key, "");
        }
        public static LoginResponse GetUser()
        {
            try
            {
                return new LoginResponse()
                {
                    Success=Preferences.Get("logged", false),
                    Token=Preferences.Get("token", ""),
                    User = new User()
                    {
                        id = Preferences.Get("id", 0),
                        firstname = Preferences.Get("firstname", ""),
                        lastname = Preferences.Get("lastname", ""),
                        fullname = Preferences.Get("fullname", ""),
                        email = Preferences.Get("email", ""),
                        barber = Preferences.Get("barber", true),
                        avatar = Preferences.Get("avatar", "")
                    }
                };
            }
            catch (Exception e)
            {

            }
            return default(LoginResponse);
        }
        public static void SetFilter(Filter filter)
        {
            Preferences.Set("request", filter.request);
            Preferences.Set("cancel", filter.cancel);
            Preferences.Set("confirm", filter.confirm);
            Preferences.Set("complete", filter.complete);
            Preferences.Set("noshow", filter.noshow);
        }

        public static Filter getFilter()
        {
            try
            {
                return new Filter()
                {
                    request = Preferences.Get("request", ""),
                    cancel = Preferences.Get("cancel", ""),
                    confirm = Preferences.Get("confirm", ""),
                    complete = Preferences.Get("complete", ""),
                    noshow = Preferences.Get("noshow", "")
                };
            }
            catch (Exception e)
            {
                string message = e.ToString();
            }
            return default(Filter);
        }
    }

    
}
