using Capstone4.Models;
using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Capstone4
{
    public class Timezone
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public DateTime GetDateTime(ServiceRequest serviceRequest)
        {

            LatLong mine = GetLatLong(serviceRequest);
            double lat = mine.latitude;
            double longitude = mine.longitude;
            //double stamp = DateTime.UtcNow.ToOADate();
            double myStamp = Getstamp();
            //double mystamp2 = myStamp.ToOADate();
            string source = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\time.txt");
            string url = "https://maps.googleapis.com/maps/api/timezone/json?location=" + lat + "," + longitude + "&timestamp=" + myStamp + "&key=" + source;
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            TimeObject result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<TimeObject>(responseFromServer);
            reader.Close();
            response.Close();
            db.SaveChanges();
            DateTime timeUtc = DateTime.UtcNow;
            try
            {
                TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(result.timeZoneName);
                DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
                serviceRequest.Timezone = result.timeZoneName;
                return Time;
            }

            catch (TimeZoneNotFoundException)
            {

                if (result.timeZoneName == "Hawaii-Aleutian Standard Time")
                {
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
                    DateTime TimeBackup = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, ZoneBackup);
                    serviceRequest.Timezone = "Hawaiian Standard Time";
                    return TimeBackup;
                }
                if (result.timeZoneName == "Alaska Standard Time")
                {
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
                    DateTime TimeBackup = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, ZoneBackup);
                    serviceRequest.Timezone = "Alaskan Standard Time";
                    return TimeBackup;
                }

                DateTime backup = catchTime(result.rawOffset + result.dstOffset);
                serviceRequest.Timezone = result.timeZoneName;
                return backup;

                //Console.WriteLine("The registry does not define the " + result.timeZoneName + "Time zone.");
            }
            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("Registry data on the " + result.timeZoneName + "Time zone has been corrupted.");
            }

            return DateTime.Now;

        }

        public Checker checkDate(Checker checker)
        {

            LatLong mine = GetLatLongCheck(checker);
            if(mine.Found == false)
            {
                checker.Found = false;
                return checker;
            }
            checker.Found = true;
            double lat = mine.latitude;
            double longitude = mine.longitude;
            double myStamp = Getstamp();
            //double myStamp = 1496341456;
            string source = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\time.txt");
            string url = "https://maps.googleapis.com/maps/api/timezone/json?location=" + lat + "," + longitude + "&timestamp=" + myStamp + "&key=" + source;
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            TimeObject result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<TimeObject>(responseFromServer);
            reader.Close();
            response.Close();
            db.SaveChanges();
            DateTime now = DateTime.UtcNow;
            try
            {
                if (checker.CompletionDeadline.HasValue)
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(result.timeZoneName);
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, Zone);
                    try
                    {
                        DateTime Time = TimeZoneInfo.ConvertTime(dt, Zone);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = result.timeZoneName;
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
            }

            catch (TimeZoneNotFoundException)
            {

                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Hawaii-Aleutian Standard Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Hawaiian Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Alaska Standard Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Alaskan Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Alaska Daylight Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Alaskan Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Mountain Daylight Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Mountain Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Pacific Daylight Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Pacific Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Eastern Daylight Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Eastern Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }
                if (checker.CompletionDeadline.HasValue && result.timeZoneName == "Central Daylight Time")
                {
                    var dt = checker.CompletionDeadline.Value;
                    TimeZoneInfo ZoneBackup = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    DateTime convertNow = TimeZoneInfo.ConvertTime(now, ZoneBackup);
                    try
                    {
                        DateTime TimeBackup = TimeZoneInfo.ConvertTime(dt, ZoneBackup);
                    }
                    catch
                    {
                        checker.Invalid = true;
                        return checker;
                    }
                    checker.Timezone = "Central Standard Time";
                    if (checker.CompletionDeadline > convertNow)
                    {
                        checker.OK = true;
                    }
                    if (checker.CompletionDeadline < convertNow)
                    {
                        checker.OK = false;
                    }
                    return checker;
                }

            }
            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("Registry data on the " + result.timeZoneName + "Time zone has been corrupted.");
            }

            return checker;

        }

        public LatLong GetLatLong(ServiceRequest servicerequest)
        {
            AddressData[] addresses = new AddressData[]
           {
            new AddressData
            {
                Address = servicerequest.Address.Street,
                City = servicerequest.Address.City,
                State = servicerequest.Address.State
            }
           };
            foreach (var address in addresses)
            {
                var gls = new GoogleLocationService();
                try
                {
                    LatLong MyLatLong = new LatLong();
                    var latlong = gls.GetLatLongFromAddress(address);
                    var Latitude = latlong.Latitude;
                    var Longitude = latlong.Longitude;
                    MyLatLong.latitude = Latitude;
                    MyLatLong.longitude = Longitude;
                    return MyLatLong;
                }
                catch (System.Net.WebException ex)
                {
                    System.Console.WriteLine("Google Maps API Error {0}", ex.Message);
                  
                }

            }
            return null;
        }

        public LatLong GetLatLongCheck(Checker checker)
        {
            AddressData[] addresses = new AddressData[]
           {
            new AddressData
            {
                Address = checker.Street,
                City = checker.City,
                State = checker.State
            }
           };
            foreach (var addressToCheck in addresses)
            {
                var gls = new GoogleLocationService();
                try
                {
                    LatLong MyLatLong = new LatLong();
                    var latlong = gls.GetLatLongFromAddress(addressToCheck);
                    if(latlong == null)
                    {
                        MyLatLong.Found = false;
                        return MyLatLong;
                    }
                    var Latitude = latlong.Latitude;
                    var Longitude = latlong.Longitude;
                    MyLatLong.latitude = Latitude;
                    MyLatLong.longitude = Longitude;
                    MyLatLong.OK = true;
                    MyLatLong.Found = true;
                    return MyLatLong;
                }
                catch (System.Net.WebException ex)
                {
                    System.Console.WriteLine("Google Maps API Error {0}", ex.Message);

                }

            }
            return null;
        }

        public double Getstamp()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            double secondsSinceEpoch = (double)t.TotalSeconds;
            return secondsSinceEpoch;
        }
        public DateTime catchTime(int offset)
        {
            DateTime timeUtc = DateTime.UtcNow;
            return timeUtc.AddSeconds(offset);
        }
    }

}
