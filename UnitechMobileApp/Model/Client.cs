﻿using System;
using System.IO;
using System.Net;

namespace UnitechMobileApp.Model
{
    public static class Client
    {
        static string domain = "https://ies.unitech-mo.ru/api?token=e78a4a9c0b16dd06b0ebc4748345a144";
        static CookieContainer cookies = null;
        public static IUser User { get; private set; }

        /// <summary>
        /// метод выполняет request и возвращает строку полученную из него
        /// </summary>
        static string FillRequest(HttpWebRequest request)
        {
            string result = "";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Метод логинит в API unitech-mo и запоминает нужные Cookies
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns> Json строку при удачной аутентификации и "null" при неудаче</returns>
        static public string Auth(string login, string password, out bool authResult)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"{domain}&query=AUTH&login={login}&password={password}");
            request.CookieContainer = new CookieContainer();
            string result = "";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                cookies = new CookieContainer();
                cookies.Add(response.Cookies);

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            authResult = result != "null";

            // TODO: пересмотреть механизм определения типа юзера
            if (authResult)
            {
                // Студент
                if (result.Contains("\"user_type\":\"2\""))
                    User = new StudentBehavior();
                // Преподователь
                else
                    User = new TeacherBehavior();
            }

            return result;
        }

        /// <summary>
        /// Метод берет новости с API unitech-mo
        /// </summary>
        /// <param name="offset">Смещение относительно последней новости</param>
        /// <param name="limit">Количестов новостей которые нужно взять (0 - все)</param>
        /// <returns>Html с текстом новостей </returns>
        static public string News(int offset = 0, int limit = 1)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"{domain}&query=NEWS&offset={offset}&limit={limit}");
            request.CookieContainer = cookies;

            return FillRequest(request);

        }

        /// <summary>
        /// Метод берет расписание этой недели залогиненного пользователя с API unitech-mo
        /// </summary>
        /// <returns>Json с расписанием</returns>
        static public string Schedule()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"{domain}&query=SCHEDULE");
            request.CookieContainer = cookies;

            return FillRequest(request);
        }

        /// <summary>
        /// Метод берет расписание залогиненного пользователя с выбранной недели через API unitech-mo
        /// </summary>
        /// <returns>Json с расписанием</returns>
        static public string Schedule(DateTime begdate, DateTime enddate, out bool sucses)
        {
            sucses = false;
            string result = null;

            if ((begdate.DayOfWeek == DayOfWeek.Monday && enddate.DayOfWeek == DayOfWeek.Sunday) 
                && ((enddate - begdate).TotalDays == 6))
            {
                sucses = true;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"{domain}&query=SCHEDULE&d={begdate.ToShortDateString()}-{enddate.ToShortDateString()}");
                request.CookieContainer = cookies;
                
                result = FillRequest(request);
            }

            return result;
        }

        /// <summary>
        /// Метод берет учебный план залогиненного пользователя с API unitech-mo
        /// </summary>
        /// <returns>Json с учебным планом</returns>
        static public string StudentPlan()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"{domain}&query=STUDENT_PLAN");
            request.CookieContainer = cookies;

            return FillRequest(request);
        }
    }
}