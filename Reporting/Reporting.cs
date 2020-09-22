using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Reporting
{
    public static class Reporting
    {
        private static string APP_NAME = Environment.GetEnvironmentVariable("APP_NAME");
        private static string CLIENT_ID = Environment.GetEnvironmentVariable("CLIENT_ID");
        private static string CLIENT_SECRET = Environment.GetEnvironmentVariable("CLIENT_SECRET");
        private static string TO_ADDRESS = Environment.GetEnvironmentVariable("TO_ADDRESS");
        private static string SENDGRID_API_KEY = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        private static string BASE_URL = Environment.GetEnvironmentVariable("BASE_URL");

        [FunctionName("Reporting")]
        public static async Task RunAsync([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log) // "0 0 0 * * SAT" for each saturday, "0 * * * * *" for each minute (testing)
        {
            log.LogInformation($"Bike-Finder Timer trigger function executed at: {DateTime.Now}");

            // Get data
            string json = await GetHistoryAsync();

            // Analyze data and generate a report
            Report report = generateReport(json);

            // Output
            log.LogInformation($"Anzahl benutzer Fahrräder: {report.totalBikes}");
            log.LogInformation($"Gefahrene Gesamtstrecke: {report.totalDistance.ToString("#.##")} KM");
            log.LogInformation($"Anzahl Fahrten: {report.totalRides}");
            log.LogInformation($"Einzelne Fahrten: {report.resultsDebug}");

            // Send report via mail
            Response res = await SendMail(report);
            if (res.StatusCode.ToString().Equals("Accepted"))
            {
                log.LogInformation($"Mail has been sent successfully at: {DateTime.Now}");
            }
            else
            {
                log.LogInformation($"Mail sending failed at: {DateTime.Now}");
            }
        }

        private static Report generateReport(string data)
        {
            // Deserialize data
            var history = JsonConvert.DeserializeObject<Rootobject>(data);

            Console.WriteLine("Analyzing data...");

            // Create unique list of bikes
            List<History> historyUnique = new List<History>();

            foreach (Item element in history.items)
            {
                var match = historyUnique.FirstOrDefault(x => x.bike == element.data.bike.iv);

                if (match != null)
                {
                    // Add distance to matching element
                    match.distance += element.data.distance.iv;
                    match.count++;
                }
                else
                {
                    // Create and add to list
                    History newElement = new History()
                    {
                        startDate = element.data.startDate.iv,
                        endDate = element.data.endDate.iv,
                        bike = element.data.bike.iv,
                        distance = element.data.distance.iv,
                        image = element.data.image.iv[0],
                        count = 1
                    };
                    historyUnique.Add(newElement);
                }
            }

            // Calculate totals
            float totalDistance = 0f;
            int totalTours = 0;
            string bikeResultHTML = string.Empty;
            string bikeResultDebug = string.Empty;

            foreach (History element in historyUnique)
            {
                totalDistance += element.distance;
                totalTours += element.count;
                // HTML list for report
                bikeResultHTML = bikeResultHTML + "<li>Bike " + element.bike + ": " + element.distance.ToString("0.00") + " KM in " + element.count + " Fahrten</li>";
                bikeResultDebug = bikeResultDebug + Environment.NewLine + "Bike " + element.bike + ": " + element.distance.ToString("0.00") + " KM in " + element.count + " Fahrten";
            }

            // Return report
            return new Report
            {
                executedDate = DateTime.Now,
                totalDistance = totalDistance,
                totalBikes = historyUnique.Count,
                totalRides = history.total,
                resultsHTML = bikeResultHTML,
                resultsDebug = bikeResultDebug
            };
        }

        private static async Task<string> GetHistoryAsync()
        {

            // Get a token
            RestClient client = new RestClient(BASE_URL);
            RestRequest request = new RestRequest("identity-server/connect/token");

            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", CLIENT_ID);
            request.AddParameter("client_secret", CLIENT_SECRET);
            request.AddParameter("scope", "squidex-api");

            var response = client.Post(request);
            string token = JsonConvert.DeserializeObject<Token>(response.Content).AccessToken;

            // Get history data from cms
            request = new RestRequest("api/content/"+APP_NAME+"/history", Method.GET);
            request.AddParameter("Authorization",
            string.Format("Bearer " + token),
                        ParameterType.HttpHeader);
            response = client.Execute(request);

            // Return data
            return response.Content;
        }

        private static async Task<Response> SendMail(Report report)
        {
            var client = new SendGridClient(SENDGRID_API_KEY);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("reporting@bike-finder.com", "Bike-Finder Reporting"),

                Subject = "Ein neuer Bike-Finder Report steht zur Verfügung",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Es wurde ein neuer Report generiert.</strong></br></br>"
                + "Allgemeine Zusammenfassung:</br>"
                + "<ul>"
                + "<li>Datum der Report-generierung: " + report.executedDate + " Uhr </li>"
                + "<li>Anzahl benutzer Fahrräder: " + report.totalBikes + "</li>"
                + "<li>Gefahrene Gesamtstrecke:  " + report.totalDistance.ToString("#.##") + " KM</li>"
                + "<li>Anzahl Fahrten: " + report.totalRides + "</li>"
                + "</ul>"
                + "</br>"
                + "Details pro Fahrrad:</br>"
                + "<ul>"
                + report.resultsHTML
                + "</ul>"
                + "</br>"
                + "Sie haben diese Mail erhalten, weil Sie als Verantwortlicher des Bike-Finder Services hinterlegt sind. Diese ist eine automatisch versendete Systemnachricht."
            };
            msg.AddTo(new EmailAddress(TO_ADDRESS, "Owner"));
            var response = await client.SendEmailAsync(msg);

            return response;
        }
    }
}
