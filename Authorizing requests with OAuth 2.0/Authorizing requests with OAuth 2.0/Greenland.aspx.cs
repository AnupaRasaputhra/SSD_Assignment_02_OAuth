using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Authorizing_requests_with_OAuth_2._0
{
    public partial class Greenland : System.Web.UI.Page
    {
        private WebRequest request;
        private Stream dataStream;
        private Hashtable hTRespose;
        private String[] arrResp;
        private string status;

        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            imgProfilePic.Visible = false;
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Params["code"]))
            {
                /// <summary>
                /// Obtaining the Authorization Code
                /// </summary>
                string authorization_code = HttpContext.Current.Request.Params["code"] + "#_=_";

                /// <summary>
                /// Set parameters to obtain the token id
                /// </summary>

                string postData = "grant_type=authorization_code" +
                                    "&client_id=" + ConfigurationManager.AppSettings["ClientID"] +
                                    "&redirect_uri=" + ConfigurationManager.AppSettings["RedirectURI"] +
                                    "&code=" + authorization_code;

                /// <summary>
                /// Send HTTP Post Request
                /// </summary>
                HTTP_POST(ConfigurationManager.AppSettings["TokenEndpoint"], postData);

                /// <summary>
                /// Get Response
                /// </summary>
                string res = GetResponse();

                res = res.Replace("\"", "");
                res = res.Replace("{", "");
                res = res.Replace("}", "");
                res = res.Replace(",", ":");
                arrResp = res.Split(':');

                RetriveUserAlbums(arrResp);
            }
        }


        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// Redirected to the authorize endpoint of Facebook
            /// </summary>
            Response.Redirect(ConfigurationManager.AppSettings["AuthorizeEndpoint"] +
                "response_type=code&client_id=" + ConfigurationManager.AppSettings["ClientID"] +
                "&redirect_uri=" + ConfigurationManager.AppSettings["RedirectURI"] +
                "&scope=" + ConfigurationManager.AppSettings["Scope"]);
        }


        public void HTTP_POST(string Url, string Data)
        {
            string credentials = EncodeTo64(ConfigurationManager.AppSettings["ClientID"] + ":" + ConfigurationManager.AppSettings["AppSecret"]);
            string Out = String.Empty;

            /// <summary>
            /// send a HTTP POST request to the Token Endpoint of facebook
            /// </summary>
            request = System.Net.WebRequest.Create(Url);
            try
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] byteArray = Encoding.UTF8.GetBytes(Data);
                request.ContentLength = byteArray.Length;
                request.Headers["Authorization"] = "Basic " + credentials;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetResponse()
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        /// <summary>
        /// encoded in Base64
        /// </summary>
        static public string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        public void RetriveUserAlbums(string[] arrResp)
        {
            /// <summary>
            /// Retrieving user’s profile image
            /// </summary>
            string url = "https://graph.facebook.com/v2.8/me/albums";
            Uri sWebAddress = new Uri(url);

            HttpWebRequest request = WebRequest.Create(sWebAddress) as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 5000;
            request.Headers["Authorization"] = "Bearer" + " " + arrResp[1];

            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    var jsonResponse = sr.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResponse);

                    JArray data = (JArray)jObject.SelectToken("data");
                    foreach (JToken pData_ in data)
                    {
                        if ((string)pData_.SelectToken("name") == "Profile Pictures")
                        {
                            string albumId = (string)pData_.SelectToken("id");
                            RetrivePhotosOfAlbum(albumId);
                        }
                    }
                }
            }
        }

        public void RetrivePhotosOfAlbum(string pId)
        {

            /// <summary>
            /// Retrieving user’s profile image
            /// </summary>
            string url = "https://graph.facebook.com/v2.8/" + pId + "/photos";
            Uri sWebAddress = new Uri(url);

            HttpWebRequest request = WebRequest.Create(sWebAddress) as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 5000;
            request.Headers["Authorization"] = "Bearer" + " " + arrResp[1];

            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    var jsonResponse = sr.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResponse);

                    JArray data = (JArray)jObject.SelectToken("data");
                    foreach (JToken pData_ in data)
                    {
                        if ((string)pData_.SelectToken("name") == "PC: Mahie 😘")
                        {
                            string photoId = (string)pData_.SelectToken("id");
                            RetrivePhoto(photoId);
                        }
                    }
                }
            }
        }

        public void RetrivePhoto(string pId)
        {

            /// <summary>
            /// Retrieving user’s profile image
            /// </summary>
            string url = "https://graph.facebook.com/" + pId + "/picture";
            Uri sWebAddress = new Uri(url);

            HttpWebRequest request = WebRequest.Create(sWebAddress) as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 5000;
            request.Headers["Authorization"] = "Bearer" + " " + arrResp[1];

            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
            {
                var img = Bitmap.FromStream(s);

                byte[] bytes;
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    bytes = stream.ToArray();

                    btnSignIn.Visible = false;
                    imgProfilePic.Visible = true;

                    imgProfilePic.ImageUrl = "data:image;base64," + Convert.ToBase64String(bytes);
                }
            }
        }
    }
}