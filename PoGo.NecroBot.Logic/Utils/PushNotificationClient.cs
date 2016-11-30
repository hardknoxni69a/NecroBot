﻿using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Utils
{
    //somehow, we can add PushBulletSharp in project, that is a simple client to just for send note message,
    public class PushNotificationClient
    {
        private static void HandleEvent(ErrorEvent errorEvent, ISession session)
        {
            //SendPushNotificationV2("Error occured", errorEvent.Message);
        }

        private static StreamContent AddContent(Stream stream, string filename)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = "\"" + filename + "\""
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return fileContent;
        }
        private static StringContent addContent(string name, string content)
        {
            var fileContent = new StringContent(content);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"" + name + "\""
            };
            return fileContent;
        }
        
        public static void SendMailNotification(string title, string body)
        {
            var fromAddress = new MailAddress("nhattruong84@gmail.com", "Truong Nguyen");
            var toAddress = new MailAddress("samuraitruong@hotmail.com", "Truong Nguyen");

            const string fromPassword = "@Abc123$";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = title,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }
        public static async Task<bool> SendPushNotificationV2(string title, string body)
        {
            bool isSusccess = false;

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("o.tsGyIPRalTONqukSFgqQH4eRI9OVjJLl", "")
            };

            // string name = Path.GetFileName(pathFile);
            using (var wc = new HttpClient(handler))
            {
                using (var multiPartCont = new MultipartFormDataContent())
                {
                    multiPartCont.Add(addContent("type", "note"));
                    multiPartCont.Add(addContent("title", title));
                    multiPartCont.Add(addContent("body", body));
                    //multiPartCont.Add(AddContent(new FileStream(pathFile, FileMode.Open), name));

                    try
                    {
                        var resp = wc.PostAsync("https://api.pushbullet.com/v2/pushes", multiPartCont);
                        var result = await resp.Result.Content.ReadAsStringAsync();
                        SendMailNotification(title, body);
                       //need check return message to confirm.
                       isSusccess = true;
                    }
                    catch (Exception ex)
                    {
                        isSusccess = false;
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return isSusccess;
        }
    }
}