using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Barber.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool barber { get; set; }
    }
    public class RegisterRequest
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string password_confirmation { get; set; }
        public int rate { get; set; }
        public bool barber { get; set; }
        public string device_token { get; set; }
        public string iphone_device_token { get; set; }
    }
    public class User
    {
        public int id { get; set; }
        public string avatar { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public bool barber { get; set; }
        public float rate { get; set; }
        public string StrRate { get; set; }
        public int star { get; set; }
        public string device_token { get; set; }
        public List<Review> reviews { get; set; }
        public int iReview { get; set; }
        public int favBarber { get; set; }
        public string StrReview { get; set; }
        public LocationInfo location { get; set; }
        public List<RateImage> rateImages { get; set; }
        public List<Gallery> gallerys { get; set; }
        public BookSetting booksetting { get; set; }
    }

    public class ChangePwd
    {
        public string currentPwd { get; set; }
        public string newPwd { get; set; }
    }

    public class ImageUploadRequest
    {
        public string ImageName { get; set; }
        public bool Success { get; set; }
    }
    public class RateImage
    {
        public string image { get; set; }
        public int count { get; set; }
    }
    public class Review
    {
        public User Client { get; set; }
        public string content { get; set; }
        public int rate { get; set; }
        public List<RateImage> RateImages { get; set; }
        public string created_at { get; set; }
    }
    public class ReviewBarber
    {
        public int barberid { get; set; }
        public string content { get; set; }
        public int rate { get; set; }
    }
    public class LocationInfo
    {
        public int barberid { get; set; }
        public string locationType { get; set; }
        public string shopName { get; set; }
        public string streetAddress { get; set; }
        public string floor { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string country { get; set; }
        public string sunday { get; set; }
        public string monday { get; set; }
        public string tuesday { get; set; }
        public string wednesday { get; set; }
        public string thursday { get; set; }
        public string friday { get; set; }
        public string saturday { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        //unset field when save
        public string wholeAddress { get; set; }
        public double distance { get; set; }
        public string strDistance { get; set; }
    }
    public class Week
    {
        public string day { get; set; }
        public string hour { get; set; }
    }
    public class Hour
    {
        public string hour { get; set; }
    }
    public class Service
    {
        public int id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string strprice { get; set; }
        public string duration { get; set; }
        public string hour { get; set; }
        public string min { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string check { get; set; }
    }
    public class Book
    {
        public int id { get; set; }
        public User client { get; set; }
        public User barber { get; set; }
        public List<Service> services { get; set; }
        public string allservicename { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string week { get; set; }
        public string month { get; set; }
        public int payment { get; set; }
        public int state { get; set; }
        public string stateColor { get; set; }
        public string statename { get; set; }
        public List<BookComment> bookcomments { get; set; }
    }
    public class BookStateRequest
    {
        public int id { get; set; }
        public bool Success { get; set; }
    }
    public class BookViewRequest
    {
        public List<Book> Books { get; set; }
        public bool Success { get; set; }
    }
    public class ServiceRequest
    {
        public List<Service> Services { get; set; }
        public bool Success { get; set; }
    }
    public class FavouriteRequest
    {
        public int Result { get; set; }
        public bool Success { get; set; }
    }
    public class ReviewRequest
    {
        public List<Review> Reviews { get; set; }
        public bool Success { get; set; }
    }
    public class BarberInfo
    {
        public int barberid { get; set; }
        public string bio { get; set; }
        public string phone { get; set; }
    }
    public class BarberInfoRequest
    {
        public LocationInfo Location { get; set; }
        public BarberInfo Info { get; set; }
        public List<Gallery> Gallerys { get; set; }
        public bool Success { get; set; }
    }
    public class BarberBookInfo
    {
        public LocationInfo Location { get; set; }
        public string[] exceptTimes { get; set; }
        public bool Success { get; set; }

    }
    public class BarberBookTime
    {
        public int id { get; set; }
        public string date { get; set; }
    }

    public class BookDateTime
    {
        public string date { get; set; }
        public string time { get; set; }
    }

    public class Cut
    {
        public int id { get; set; }
        public int n_id_user { get; set; }
        public string sz_cuts { get; set; }
    }

    public class CutRequest
    {
        public Cut Cut { get; set; }
        public bool Success { get; set; }
    }
    public class LoginResponse
    {
        public User User { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
    }
    public class ResponseModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }
    public class BarberModel
    {
        public List<User> Users { get; set; }
        public List<Cut> Cuts { get; set; }
        public bool Success { get; set; }
    }

    public class BookRequest
    {
        public int BookID { get; set; }
        public bool Success { get; set; }
    }

    public class SearchRequest
    {
        public List<User> searchBarbers { get; set; }
        public bool Success { get; set; }
    }
    public class SearchData
    {
        public string location { get; set; }
        public string name { get; set; }
    }

    public class SuccessRequest 
    { 
        public bool Success { get; set; }
    }

    public class GalleryRequest
    { 
        public List<Gallery> Gallerys { get; set; }
        public bool Success { get; set; }
    }

    public class Gallery
    {
        public int id { get; set; }
        public string image { get; set; }
    }
    public class OneWeek
    {
        public string weekname { get; set; }
        public int day { get; set; }
        public bool isActive { get; set; }
        public bool isNormal { get; set; }
    }

    public class BarberBook
    {
        public string day { get; set; }
    }

    public class Filter
    {
        public string request { get; set; }
        public string cancel { get; set; }
        public string confirm { get; set; }
        public string complete { get; set; }
        public string noshow { get; set; }
    }
    public class SupportFeedback
    {
        public int type { get; set; }
        public string email { get; set; }
        public string description { get; set; }
    }
    public class Notification
    {
        public int id { get; set; }
        public User sender { get; set; }
        public int typeid { get; set; }
        public int type { get; set; } //book:0, review: 1, announcement: 2, bookcomment: 3
        public int currentstate { get; set; }
        public int show { get; set; }
        public bool IsNew { get; set; }
        public string created_at { get; set; }
        public string title { get; set; }
        public string content { get; set; }

        //type data
        public Book book { get; set; }
        public List<RateImage> RateImages { get; set; }
        public Announcement announcement { get; set; }
        public BookComment bookcomment { get; set; }

        //isShow
        public bool isOther { get; set; }
        public bool isReview { get; set; }
    }
    public class BookComment
    {
        public int bookid { get; set; }
        public User sender { get; set; }
        public int senderid { get; set; }
        public bool isSend { get; set; }
        public bool isReceive { get; set; }
        public string comment { get; set; }
        public string created_at { get; set; }
    }
    public class Announcement 
    { 
        public List<User> Users { get; set; }
        public string content { get; set; }
    }
    public class NotificationRequest
    {
        public List<Notification> Notifications { get; set; }
        public bool Success { get; set; }
    }
    public class ClientRequest
    {
        public List<User> Users { get; set; }
        public bool Success { get; set; }
    }
    public class ClientDetailRequest
    {
        public List<Book> Books { get; set; }
        public FavouriteBarber FavBarber { get; set; }
        public bool Success { get; set; }
    }
    public class FavouriteBarber
    {
        public User client { get; set; }
        public User barber { get; set; }
        public int Block { get; set; }
    }
    public class BookSetting
    {
        public int barberid { get; set; }
        public int auto_confirm { get; set; }
        public int multi_service { get; set; }
        public int require_phone { get; set; }
        public int bookinterval { get; set; }
        public string last_limit_hour { get; set; }
        public string last_limit_min { get; set; }
        public string future_limit { get; set; }
        public string requrring_limit { get; set; }
        public string auto_comment { get; set; }
    }

    public class BookSettingRequest
    {
        public BookSetting BookSetting { get; set; }
        public bool Success { get; set; }
    }

    public class ClientBlockRequest
    {
        public int id { get; set; }
        public int block { get; set; }
    }
    public class NewAlarmRequest
    {
        public int NewAlarm { get; set; }
        public bool Success { get; set; }
    }
}