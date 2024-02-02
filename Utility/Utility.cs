﻿namespace nIS
{
    #region References

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PuppeteerSharp;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mail;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Websym.Core.ConfigurationManager;


    #endregion

    public class Utility : IUtility
    {
        private static int CreateAndWriteToZipFileCount = 0;
        /// <summary>
        /// This method will helps to get description of perticular entity of class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name
        /// </param>
        /// <param name="entityType">
        /// The type of entity.
        /// </param>
        /// <returns>
        /// It returns a string.
        /// </returns>
        public string GetDescription(string propertyName, Type entityType)
        {
            var property = entityType.GetProperty(propertyName);
            var attribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true)[0];
            var description = (DescriptionAttribute)attribute;
            return description.Description;
        }

        public static string FormatCurrency(string input)
        {
            if (input.Contains("R"))
                return input;

            decimal amount = decimal.Parse(input, System.Globalization.CultureInfo.InvariantCulture); // Parse the string to a decimal

            string formattedAmount = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-ZA"));
            return ((amount < 0 ? "-" : "") + formattedAmount.Replace(",", ".")).Replace("--", "-");
        }
        public static string FormatDecimaltoStringValue(string input)
        {
            // Convert the string to a decimal
            if (decimal.TryParse(input, out decimal number))
            {
                // Format the decimal without trailing .00
                string formattedNumber = number.ToString("0.##");
                // Display the result
                return formattedNumber;
            }
            else
            {
                return "Invalid input";
            }

        }
public static string FormatCurrency(double amount)
        {
            string formattedAmount = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-ZA"));
            return ((amount < 0 ? "-" : "") + formattedAmount.Replace(",", ".")).Replace("--", "-");
        }

        /// <summary>
        /// This method will helps to get enum key valuee pair of perticular entity of class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name
        /// </param>
        /// <param name="entityType">
        /// The type of entity.
        /// </param>
        /// <returns>
        /// It returns a string.
        /// </returns>
        public List<KeyValuePair<string, int>> GetEnumKeyValue(Enum entityType)
        {
            IList<KeyValuePair<string, int>> keyValuePairEnum = new List<KeyValuePair<string, int>>();
            foreach (var value in Enum.GetValues(entityType.GetType()))
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                    keyValuePairEnum.Add(new KeyValuePair<string, int>(attributes[0].Description, (int)value));
                else
                    keyValuePairEnum.Add(new KeyValuePair<string, int>(value.ToString(), (int)value));
            }
            return keyValuePairEnum.ToList();
        }

        /// <summary>
        /// This method executes the web request using the specified parameters.
        /// </summary>
        /// <param name="instanceURL">The instance URL.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="objectData">The object data.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="toBeSerailzied">This property should be set to be true if passing object data as primitive data type.</param>
        /// <returns>
        /// Returns the response object
        /// </returns>
        public string ExecuteWebRequest(string instanceURL, string controller, string action, string objectData, string tenantKey, string tenantCode, bool toBeSerailzied = false)
        {
            string responseFromServer = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(instanceURL + "/" + controller + "/" + action);
                HttpWebResponse response = null;
                request.Headers.Add(tenantKey, tenantCode);
                request.Method = "POST";
                string postData = toBeSerailzied ? JsonConvert.SerializeObject(objectData) : objectData;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    dataStream = response.GetResponseStream();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (dataStream != null)
                        {
                            StreamReader reader = new StreamReader(dataStream);
                            responseFromServer = reader.ReadToEnd();
                            reader.Close();
                            dataStream.Close();
                        }
                    }
                }
                catch (WebException webException)
                {
                    response = (HttpWebResponse)webException.Response;
                    dataStream = response.GetResponseStream();
                    if (dataStream != null)
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        dataStream.Close();

                        JObject jObject = JsonConvert.DeserializeObject<JObject>(responseFromServer);
                        throw new Exception(jObject["Error"]["Message"].ToString());
                    }
                }

                return responseFromServer;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method gets the configuration values from configuration manager component.
        /// </summary>
        /// <param name="configurationSearchParameter">The configuration search parameter.</param>
        /// <param name="configurationBaseURLKey">The configuration base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the list of configuration section
        /// </returns>
        public IList<Websym.Core.ConfigurationManager.ConfigurationSection> GetConfigurationValues(ConfigurationSearchParameter configurationSearchParameter, string configurationBaseURLKey, string tenantKey, string tenantCode)
        {

            IList<Websym.Core.ConfigurationManager.ConfigurationSection> configurationSectionList = null;
            try
            {

                return configurationSectionList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method gets the connection string from configuration manager as per the specified key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationBaseURLKey">The configuration base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the connection string for the specified configuration key(s).
        /// </returns>
        public string GetConnectionString(string section, string configurationKey, string configurationBaseURLKey, string tenantKey, string tenantCode)
        {
            string sqlConnectionString = string.Empty;
            try
            {
                ConfigurationSearchParameter configurationSearchParameter = new ConfigurationSearchParameter();
                configurationSearchParameter.SectionName = section;
                configurationSearchParameter.ConfigurationKey = configurationKey;

                IList<Websym.Core.ConfigurationManager.ConfigurationSection> configurationSectionList = this.GetConfigurationValues(configurationSearchParameter, configurationBaseURLKey, tenantKey, tenantCode);
                if (configurationSectionList != null && configurationSectionList.Count > 0)
                {
                    if (configurationSectionList[0].ConfigurationItems != null && configurationSectionList[0].ConfigurationItems.Count > 0)
                    {
                        sqlConnectionString = configurationSectionList[0].ConfigurationItems[0].Value;
                    }
                }

                sqlConnectionString = sqlConnectionString.EndsWith(";") ? sqlConnectionString : sqlConnectionString + ";";
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return sqlConnectionString;
        }

        /// <summary>
        /// This method gets this list of localized resources for cshtml as per the specified search parameter
        /// </summary>
        /// <param name="culture">The culture name</param>
        /// <param name="sectionName">The UI section name</param>
        /// <param name="resourceBaseUrl">The base url</param>
        /// <param name="tenantKey">The Tenant key</param>
        /// <param name="defaultTenant">The default tenant code.</param>
        /// <returns></returns>
        public Dictionary<string, string> GetResourcesForUI(string culture, string sectionName, string resourceBaseUrl, string tenantKey, string defaultTenant)
        {
            Dictionary<string, string> resourceItems = new Dictionary<string, string>();
            return resourceItems;
        }

        /// <summary>
        /// This method helps to send mail.
        /// </summary>
        /// <param name="mail">MailMessage object</param>
        /// <param name="applicationSMTPClientHost">Application smtp client host</param>
        /// <param name="applicationSMTPClientPort">Application smtp client port</param>
        /// <param name="applicationEmailPassword">Application email password</param>
        /// <param name="tenantCode">The tenant code</param>
        public void SendMail(MailMessage mail, string applicationSMTPClientHost, int applicationSMTPClientPort, string applicationEmailPassword, string tenantCode)
        {
            try
            {
                var emailFromAddress = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"];
                var displayName = System.Configuration.ConfigurationManager.AppSettings["MailDisplayName"];
                var password = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddressPassword"];
                bool enableSSL = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSSL"]);
                var smtpAddress = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
                var portNumber = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);
                mail.From = new MailAddress(emailFromAddress, displayName);
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.EnableSsl = enableSSL;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #region Platform 

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPostRequest(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                string responseString = response.Content.ReadAsStringAsync().Result;

                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPostRequestEncodedContent(string baseURL, string actionPath, string parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                using (StringContent content = new StringContent(parameters, Encoding.Default, "application/x-www-form-urlencoded"))
                {
                    response = client.PostAsync(baseURL + actionPath, content).Result;
                    string responseString = response.Content.ReadAsStringAsync().Result;
                }
                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPutRequest(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPostRequestByUrlEncodedContent(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        pairs.Add(new KeyValuePair<string, string>(header.Key, header.Value));
                    }
                }
                FormUrlEncodedContent content = new FormUrlEncodedContent(headersDictionary);
                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                response = client.PostAsync(baseURL + actionPath, content).Result;

                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP get request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpGetRequest(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = client.GetAsync(baseURL + actionPath + ((parameters != null) && (parameters is string) && (!string.IsNullOrWhiteSpace((string)parameters)) ? (string)parameters : "")).Result;
                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// This method help to write html string to actual file
        /// </summary>
        /// <param name="Message"> the message string </param>
        /// <param name="fileName"> the file name </param>
        /// <param name="batchId"> the batch identifier </param>
        /// <param name="customerId"> the customer identifier </param>
        public string WriteToFile(string Message, string fileName, string scheduleName, string batchName, long customerId, string baseURL, string outputLocation,bool printPdf = false, string headerHtml = "", string fspName = "", string fspTradingName = "", string footerHtml = "", string segment = "", string language ="")
        {
            string fspNameHeader = fspName;
            string fspTradingNameHeader = fspTradingName;
            
    string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
            string statementDestPath = outputLocation + "\\Statements" + "\\" + scheduleName + "\\" + batchName;
            string statementPath = baseURL + "\\Statements" + "\\" + scheduleName + "\\" + batchName + "\\" + customerId + "\\" + fileName;
            if (!Directory.Exists(statementDestPath))
            {
                Directory.CreateDirectory(statementDestPath);
            }
            string path = statementDestPath + "\\" + customerId + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = Path.Combine(path, fileName);
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }

            //To move js, css and other assets contents which are common to each statment file
            if (!Directory.Exists(statementDestPath + "\\common"))
            {
                DirectoryCopy(resourceFilePath, (statementDestPath + "\\common"), true);
            }

            //Printing PDF
            if (printPdf)
            {
                var deleteHtmlAfterPdfGenerate = false;
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["DeleteHtmlAfterPdfGenerate"], out deleteHtmlAfterPdfGenerate);
                var outputPdfPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileName) + ".pdf");

                Task.Run(async () =>
                {

                    var result = await GeneratePdf(filepath, outputPdfPath,headerHtml,language, fspNameHeader, fspTradingNameHeader);
                    if (string.IsNullOrEmpty(result))
                    {
                        if (deleteHtmlAfterPdfGenerate)
                        {
                            File.Delete(filepath);
                        }
                    };
                }).Wait();
            }

            return filepath;
        }

        /// <summary>
        /// This method help to write json stringin to actual file
        /// </summary>
        /// <param name="Message"> the message string </param>
        /// <param name="fileName"> the file name </param>
        /// <param name="batchId"> the batch identifier </param>
        /// <param name="customerId"> the customer identifier </param>
        public void WriteToJsonFile(string Message, string fileName, long batchId, long customerId, string baseURL)
        {
            string jsonFileDestPath = baseURL + "\\Statements" + "\\" + batchId;
            if (!Directory.Exists(jsonFileDestPath))
            {
                Directory.CreateDirectory(jsonFileDestPath);
            }
            string path = jsonFileDestPath + "\\" + customerId + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + fileName;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(Message);
            }
        }

        /// <summary>
        /// This method help to copy files from one directory to another directory
        /// </summary>
        /// <param name="sourceDirName"> the path of source directory </param>
        /// <param name="destDirName"> the path of destinaation diretory </param>
        /// <param name="copySubDirs"> the bool value of is want to copy sub directory of source directory </param>
        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (!File.Exists(temppath))
                {
                    file.CopyTo(temppath, false);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (subdir.Name.ToLower() != "sampledata")
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
        }

        /// <summary>
        /// This method help to create zip file of common html with js, css, and image files
        /// </summary>
        /// <param name="htmlstr"> the html string </param>
        /// <param name="fileName"> the filename </param>
        /// <param name="batchId"> the batch id </param>
        // public string CreateAndWriteToZipFile(string htmlstr, string fileName, long batchId, string baseURL, string outputLocation, IDictionary<string, string> filesDictionary = null)
        public string CreateAndWriteToZipFile(string htmlstr, string fileName, string scheduleName, string batchName, string baseURL, string outputLocation, IDictionary<string, string> filesDictionary = null)
        {
            CreateAndWriteToZipFileCount += 1;
            var callerName = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name;
            string zipPath = string.Empty;
            try
            {
                //create folder to store the html statement files for current batch customers

                if (!Directory.Exists(Path.GetPathRoot(outputLocation)))
                {
                    return string.Empty;
                }

                string path = outputLocation + "\\Statements" + "\\" + scheduleName + "\\" + batchName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //create media folder for common images and videos files of asset library
                string mediaPath = path + "\\common\\media\\";
                if (!Directory.Exists(mediaPath))
                {
                    Directory.CreateDirectory(mediaPath);
                }

                //common resource files path 
                string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";

                //asset (images and videos) files as well as json files
                if (filesDictionary != null && filesDictionary?.Count > 0)
                {
                    //WebClient webClient = new WebClient();
                    foreach (KeyValuePair<string, string> file in filesDictionary)
                    {
                        if (File.Exists(file.Value))
                        {
                            if (file.Key.Contains(".json"))
                            {
                                //File.Copy(file.Value, Path.Combine(spath, file.Key));
                            }
                            else
                            {
                                if (File.Exists(Path.Combine(mediaPath, file.Key)))
                                {
                                    // If it exists, delete the destination file before copying the new one
                                    File.Delete(Path.Combine(mediaPath, file.Key));
                                }
                                File.Copy(file.Value, Path.Combine(mediaPath, file.Key));
                            }
                        }
                    }
                }

                //copy all common resource file to current batch statement folder as well as at common statement folder
                DirectoryCopy(resourceFilePath, (path + "\\common"), true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return zipPath;
        }

        /// <summary>
        /// This method help to delete unwantedly added json files if html generation failed for customer
        /// </summary>
        /// <param name="batchId"> the batch identifier </param>
        /// <param name="customerId"> the customer identifier </param>
        /// <returns>true if deleted successfully, otherwise false.</returns>
        public bool DeleteUnwantedDirectory(long batchId, long? customerId, string baseURL)
        {
            string deleteDirPath = baseURL + "\\Statements" + "\\" + batchId + "\\" + customerId;
            DirectoryInfo directoryInfo = new DirectoryInfo(deleteDirPath);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
            return true;
        }

        /// <summary>
        /// This method help to get string value of month
        /// </summary>
        /// <param name="m"> the numeric value of month </param>
        /// <returns>string value of month</returns>
        public string getMonth(int m)
        {
            string res;
            switch (m)
            {
                case 1:
                    res = "Jan";
                    break;
                case 2:
                    res = "Feb";
                    break;
                case 3:
                    res = "Mar";
                    break;
                case 4:
                    res = "Apr";
                    break;
                case 5:
                    res = "May";
                    break;
                case 6:
                    res = "Jun";
                    break;
                case 7:
                    res = "Jul";
                    break;
                case 8:
                    res = "Aug";
                    break;
                case 9:
                    res = "Sep";
                    break;
                case 10:
                    res = "Oct";
                    break;
                case 11:
                    res = "Nov";
                    break;
                case 12:
                    res = "Dec";
                    break;
                default:
                    res = "Jan";
                    break;
            }
            return res;
        }

        /// <summary>
        /// This method help to get month difference in between 2 dates
        /// </summary>
        /// <param name="endDate"> the end date value </param>
        /// <param name="startDate"> the start date value </param>
        /// <returns>difference between 2 dates in numeric</returns>
        public int MonthDifference(DateTime endDate, DateTime startDate)
        {
            return (endDate.Month - startDate.Month) + 12 * (endDate.Year - startDate.Year);
        }

        /// <summary>
        /// This method help to get day difference in between 2 dates
        /// </summary>
        /// <param name="endDate"> the end date value </param>
        /// <param name="startDate"> the start date value </param>
        /// <returns>difference between 2 dates in numeric</returns>
        public int DayDifference(DateTime endDate, DateTime startDate)
        {
            return Convert.ToInt32((endDate.Date - startDate.Date).TotalDays);
        }

        /// <summary>
        /// This method help to get year difference in between 2 dates
        /// </summary>
        /// <param name="endDate"> the end date value </param>
        /// <param name="startDate"> the start date value </param>
        /// <returns>difference between 2 dates in numeric</returns>
        public int YearDifference(DateTime startDate, DateTime endDate)
        {
            //Excel documentation says "COMPLETE calendar years in between dates"
            int years = endDate.Year - startDate.Year;

            if (startDate.Month == endDate.Month &&// if the start month and the end month are the same
                endDate.Day < startDate.Day// AND the end day is less than the start day
                || endDate.Month < startDate.Month)// OR if the end month is less than the start month
            {
                years--;
            }

            return years;
        }

        /// <summary>
        /// This method help to get numeric value of month
        /// </summary>
        /// <param name="m"> the string value of month </param>
        /// <returns>numeric value of month</returns>
        public int getNumericMonth(string m)
        {
            int res;
            if (m.ToLower() == "january" || m.ToLower() == "jan")
            {
                res = 1;
            }
            else if (m.ToLower() == "february" || m.ToLower() == "feb")
            {
                res = 2;
            }
            else if (m.ToLower() == "march" || m.ToLower() == "mar")
            {
                res = 3;
            }
            else if (m.ToLower() == "april" || m.ToLower() == "apr")
            {
                res = 4;
            }
            else if (m.ToLower() == "may")
            {
                res = 5;
            }
            else if (m.ToLower() == "june" || m.ToLower() == "jun")
            {
                res = 6;
            }
            else if (m.ToLower() == "july" || m.ToLower() == "jul")
            {
                res = 7;
            }
            else if (m.ToLower() == "august" || m.ToLower() == "aug")
            {
                res = 8;
            }
            else if (m.ToLower() == "september" || m.ToLower() == "sep")
            {
                res = 9;
            }
            else if (m.ToLower() == "october" || m.ToLower() == "oct")
            {
                res = 10;
            }
            else if (m.ToLower() == "november" || m.ToLower() == "nov")
            {
                res = 11;
            }
            else if (m.ToLower() == "december" || m.ToLower() == "dec")
            {
                res = 12;
            }
            else
            {
                res = 1;
            }
            return res;
        }

        /// <summary>
        /// This method help to save image file from url
        /// </summary>
        /// <param name="filePath"> the file path value </param>
        /// <param name="format"> the image format </param>
        /// <param name="imageUrl"> the image url </param>
        /// <returns>return true if download successfully.</returns>
        public bool SaveImage(string filePath, ImageFormat format, string imageUrl)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                Bitmap bitmap;
                bitmap = new Bitmap(stream);

                if (bitmap != null)
                {
                    bitmap.Save(filePath, format);
                }

                stream.Flush();
                stream.Close();
                client.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// This method executes the web request using the specified parameters.
        /// </summary>
        /// <param name="instanceURL">The instance URL.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="objectData">The object data.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="toBeSerailzied">This property should be set to be true if passing object data as primitive data type.</param>
        /// <returns>
        /// Returns the response object
        /// </returns>
        public string ExecuteWebTenantRequest(string instanceURL, string controller, string action, string objectData, string tenantKey, string tenantCode, bool isThirdPartyEnabled = false, bool toBeSerailzied = false)
        {
            string responseFromServer = string.Empty;
            try
            {

                WebRequest request = WebRequest.Create(instanceURL + "/" + controller + "/" + action + "?isThirdPartyEnabled=" + isThirdPartyEnabled);
                HttpWebResponse response = null;
                request.Headers.Add(tenantKey, tenantCode);
                request.Method = "POST";
                string postData = toBeSerailzied ? JsonConvert.SerializeObject(objectData) : objectData;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    dataStream = response.GetResponseStream();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (dataStream != null)
                        {
                            StreamReader reader = new StreamReader(dataStream);
                            responseFromServer = reader.ReadToEnd();
                            reader.Close();
                            dataStream.Close();
                        }
                    }
                }
                catch (WebException webException)
                {
                    response = (HttpWebResponse)webException.Response;
                    dataStream = response.GetResponseStream();
                    if (dataStream != null)
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        dataStream.Close();

                        JObject jObject = JsonConvert.DeserializeObject<JObject>(responseFromServer);
                        throw new Exception(jObject["Error"]["Message"].ToString());
                    }
                }

                return responseFromServer;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method executes pdf crowd tool web request to convert HTML file to PDF.
        /// </summary>
        /// <param name="htmlStatementPath">The statement statement path.</param>
        /// <param name="outPdfPath">The output pdf path.</param>
        /// <param name="password">The password to protect PDF.</param>
        /// <returns>
        /// Returns the true if pdf generated successfully, otherwise false
        /// </returns>
        public bool HtmlStatementToPdf(string htmlStatementPath, string outPdfPath, string password)
        {
            var isPdfSuccess = false;
            try
            {
                var userName = System.Configuration.ConfigurationManager.AppSettings["PdfCrowdUserName"];
                var apiKey = System.Configuration.ConfigurationManager.AppSettings["PdfCrowdApiKey"];
                var client = new pdfcrowd.HtmlToPdfClient(userName, apiKey);

                //Set the output page width. The safe maximum is 200in otherwise some PDF viewers may be unable to open the PDF.
                client.setPageWidth("12in");

                //Set the output page height. Use -1 for a single page PDF. The safe maximum is 200in otherwise some PDF viewers may be unable to open the PDF.
                client.setPageHeight("10in");

                //Set the output page top margin.
                client.setMarginTop("0.4in");

                //Set the output page right margin.
                client.setMarginRight("0.2in");

                //Set the output page bottom margin.
                client.setMarginBottom("0.4in");

                //Set the output page left margin.
                client.setMarginLeft("0.2in");

                //Set the output page header height.
                client.setHeaderHeight("0.4in");

                //Use the specified HTML as the output page footer. 
                client.setFooterHtml("Page <span class='pdfcrowd-page-number'></span> of <span class='pdfcrowd-page-count'></span> pages");

                //Set the output page footer height.
                client.setFooterHeight("0.4in");

                //The viewport width affects the @media min-width and max-width CSS properties. 
                //This mode can be used to choose a particular version (mobile, desktop, ..) of a responsive page
                client.setRenderingMode("viewport");

                //The HTML contents width fits the print area width.
                client.setSmartScalingMode("content-fit");

                //Set the quality of embedded JPEG images. A lower quality results in a smaller PDF file but can lead to compression artifacts.
                client.setJpegQuality(80);

                //Specify which image types will be converted to JPEG. 
                //Converting lossless compression image formats (PNG, GIF, ...) to JPEG may result in a smaller PDF file.
                client.setConvertImagesToJpeg("all");

                //Set the DPI of images in PDF. A lower DPI may result in a smaller PDF file.
                client.setImageDpi(300);

                //Set the title of the PDF.
                client.setTitle("PDF statement");

                //Set the author of the PDF.
                client.setAuthor("PPS");

                if (password != string.Empty)
                {
                    //Encrypt the PDF. This prevents search engines from indexing the contents.
                    client.setEncrypt(true);

                    //Protect the PDF with a user password. 
                    //When a PDF has a user password, it must be supplied in order to view the document and to perform operations allowed by the access permissions.
                    client.setUserPassword(password);
                }

                client.convertFileToFile(htmlStatementPath, outPdfPath);
                isPdfSuccess = true;
            }
            catch (pdfcrowd.Error why)
            {
                throw why;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isPdfSuccess;
        }

        /// <summary>
        /// This method helps to format currency as per provided country currency details amount value
        /// </summary>
        /// <param name="CountryCultureInfoCode">The country currency cultureInfo code value.</param>
        /// <param name="CurrencyDecimalSeparator">Defines the string that separates integral and decimal digits.</param>
        /// <param name="currencyFormat">The currency format value.</param>
        /// <param name="amount">The amount value.</param>
        /// <returns>
        /// Returns the the formatted amount value in string
        /// </returns>
        public string CurrencyFormatting(string CountryCultureInfoCode, string CurrencyDecimalSeparator, string currencyFormat, decimal amount)
        {
            try
            {
                NumberFormatInfo myNumberFormatInfo = new CultureInfo(CountryCultureInfoCode, false).NumberFormat;
                myNumberFormatInfo.CurrencyDecimalSeparator = CurrencyDecimalSeparator;
                return amount.ToString(currencyFormat, myNumberFormatInfo);
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public async Task<string> GeneratePdf(string htmlPath, string outPdfPath, string segment, string language ,string fspName,string fspTradingName)
        {
            // Declare and initialize a variable for potential PDF generation errors
            string pdfGenerationError = null;

        // Retrieve the font folder path for headers and footers from the configuration settings
        string headerFooterFontFolderPath = System.Configuration.ConfigurationManager.AppSettings["HeaderFooterFontFolderPath"];

            // Check if the font folder path is missing or empty
            if (string.IsNullOrWhiteSpace(headerFooterFontFolderPath))
            {
                // Set an error message if the font folder path is missing
                pdfGenerationError = "HeaderFooterFontFolderPath appSetting key is missing in web.config.";
                // Return the error message and exit the method
                return pdfGenerationError;
            }

           
            // Read the content of the header and footer HTML files based on the 'segment'
         string  headerPdfContent = File.ReadAllText($@"{headerFooterFontFolderPath}\HeaderFooters\" + segment + "_header.html", Encoding.UTF8);
         string footerPdfContent = File.ReadAllText($@"{headerFooterFontFolderPath}\HeaderFooters\" + segment + "_footer.html", Encoding.UTF8);

            switch (segment)
            {
                case "PPS":
                  string  ppsNameWithValue = string.IsNullOrEmpty(fspName) ? "hard coded" : fspName;
                    headerPdfContent = headerPdfContent.Replace("{{fspNameHeader}}", ppsNameWithValue);
                    // Perform actions for PPS
                    break;
                case "FSP":
                    string fspNameWithValue = string.IsNullOrEmpty(fspName) ? "hard coded" : fspName;
                    string fspTradingNameWithValue = string.IsNullOrEmpty(fspTradingName) ? "hard coded" : fspName;
                    string fspNameHeader = fspNameWithValue + " T/A " + fspTradingNameWithValue;
                    headerPdfContent = headerPdfContent.Replace("{{fspNameHeader}}", fspNameHeader);
                    // Perform actions for FSP
                    break;
                default:
                    Console.WriteLine("Unknown Page Type");
                    break;
            }

            // Get the directory path of the output PDF file
            string directoryPath = Path.GetDirectoryName(outPdfPath);
            // Get the parent directory path
            string parentDirectoryPath = Directory.GetParent(directoryPath).FullName;

            // Construct the full path for the logo image file and read its content as bytes
            var logoImgPath = parentDirectoryPath + @"\common\images\logo3.jpg";
            var logoImgPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgPath));

            // Construct the full path for the Facebook logo image file and read its content as bytes
            var logoFbPath = parentDirectoryPath + @"\common\images\fb_foot.png";
            var logoFbPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoFbPath));

            // Construct the full path for the Instagram logo image file and read its content as bytes
            var logoImgInstaPath = parentDirectoryPath + @"\common\images\insta_foot.png";
            var logoImgInstaPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgInstaPath));

            // Construct the full path for the Twitter logo image file and read its content as bytes
            var logoImgTwitterPath = parentDirectoryPath + @"\common\images\twitter_foot.png";
            var logoImgTwitterPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgTwitterPath));

            // Construct the full path for the LinkedIn logo image file and read its content as bytes
            var logoImgInPath = parentDirectoryPath + @"\common\images\in_foot.png";
            var logoImgInBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgInPath));

            // Construct the full path for the YouTube logo image file and read its content as bytes
            var logoImgYouPath = parentDirectoryPath + @"\common\images\you_foot.png";
            var logoImgYouPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgYouPath));

            // Construct the full path for the TikTok logo image file and read its content as bytes
            var logoImgTiktokPath = parentDirectoryPath + @"\common\images\ticktok_foot.png";
            var logoImgTiktokPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgTiktokPath));

            // Replace placeholders in the header and footer HTML content with base64-encoded image strings
            headerPdfContent = headerPdfContent.Replace("{{fspLogoImgPath}}", logoImgPathBase64);
            footerPdfContent = footerPdfContent.Replace("{{logoImgFbPath}}", logoFbPathBase64);
            footerPdfContent = footerPdfContent.Replace("{{logoImgInstaPath}}", logoImgInstaPathBase64);
            footerPdfContent = footerPdfContent.Replace("{{logoImgTwitterPath}}", logoImgTwitterPathBase64);
            footerPdfContent = footerPdfContent.Replace("{{logoImgInPath}}", logoImgInBase64);
            footerPdfContent = footerPdfContent.Replace("{{logoImgYouPath}}", logoImgYouPathBase64);
            footerPdfContent = footerPdfContent.Replace("{{logoImgTiktokPath}}", logoImgTiktokPathBase64);
            footerPdfContent = footerPdfContent.Replace("{{PageNumber}}", "<span class='pageNumber'></span>/<span class='totalPages'></span>");

      
            var pdfOptions = new PuppeteerSharp.PdfOptions();
                    pdfOptions.PrintBackground = true;
                    pdfOptions.DisplayHeaderFooter = true;
                    pdfOptions.HeaderTemplate = headerPdfContent;
                    pdfOptions.Landscape = false;
                    pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "4cm", Left = "1cm", Right = "1cm", Top = "4cm" };
                    pdfOptions.Scale = 1m;
                    pdfOptions.FooterTemplate = footerPdfContent;
                    pdfOptions.PreferCSSPageSize = true;
                    pdfOptions.Format = PuppeteerSharp.Media.PaperFormat.A4;

            var browserFetcher = new BrowserFetcher();
            // Download the necessary browser binaries
            await browserFetcher.DownloadAsync();

            // Launch Puppeteer with specific options
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            // Create a new page
            var page = await browser.NewPageAsync();
            // Navigate to the specified HTML path
            await page.GoToAsync(htmlPath);
            //Add script tags to the page
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\jquery.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\popper.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\bootstrap.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\highcharts.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\jquery.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\series-label.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\exporting.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\export-data.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\accessibility.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\script.js" });
            // Generate PDF from the page with specified options
            // Add style tags to the page
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\bootstrap.min.css" });
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\font-awesome.min.css" });
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\site.css" });
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\ltr.css" });
            await page.PdfAsync(outPdfPath, pdfOptions);
            return pdfGenerationError;
        }

    }
    #endregion

    //  }
}