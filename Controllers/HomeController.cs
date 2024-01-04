using GoogleAIStudio.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace GoogleAIStudio.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ImageClassificationModel model)
        {
            try
            {
                var imageData = "";
                if (model.Image != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        model.Image!.CopyTo(stream);
                        byte[] imageBytes = stream.ToArray();
                        imageData = Convert.ToBase64String(imageBytes);
                    }
                }

                string apiKey = "AIzaSyCWv__fD6OtTpiS07DiCe2ia93fG0uYj1k";
                string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent?key=" + apiKey;

                using (HttpClient httpClient = new HttpClient())
                {

                    string jsonPayload = $@"
            {{
                ""contents"": [
                    {{
                        ""parts"": [
                            {{
                                ""text"": ""{model.Description}""
                            }},
                            {{
                                ""text"": ""Image: ""
                            }},
                            {{
                                ""inlineData"": {{
                                    ""mimeType"": ""image/jpeg"",
                                    ""data"": ""{imageData}""
                                }}
                            }}
                        ]
                    }}
                ],
                ""generationConfig"": {{
                    ""temperature"": 0.9,
                    ""topK"": 40,
                    ""topP"": 0.95,
                    ""maxOutputTokens"": 1024,
                    ""stopSequences"": []
                }},
                ""safetySettings"": [
                    {{
                        ""category"": ""HARM_CATEGORY_HARASSMENT"",
                        ""threshold"": ""BLOCK_MEDIUM_AND_ABOVE""
                    }},
                    {{
                        ""category"": ""HARM_CATEGORY_HATE_SPEECH"",
                        ""threshold"": ""BLOCK_MEDIUM_AND_ABOVE""
                    }},
                    {{
                        ""category"": ""HARM_CATEGORY_SEXUALLY_EXPLICIT"",
                        ""threshold"": ""BLOCK_MEDIUM_AND_ABOVE""
                    }},
                    {{
                        ""category"": ""HARM_CATEGORY_DANGEROUS_CONTENT"",
                        ""threshold"": ""BLOCK_MEDIUM_AND_ABOVE""
                    }}
                ]
            }}";

                    // Create the HTTP request
                    StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                    // Handle the response
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic responseObject = JObject.Parse(responseBody);

                        string text = responseObject?.candidates[0]?.content?.parts[0]?.text ?? "";
                        if (!string.IsNullOrEmpty(text))
                        {
                            return Json(new { success = true, message = text });
                        }
                        else
                        {
                            string message = "Sorry, We Could not find the response.";
                            return Json(new { success = true, message = message });


                        }
                    }
                    else
                    {
                        string message = "API Request failed.";
                        return Json(new { success = true, message = message });
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
    }
}
