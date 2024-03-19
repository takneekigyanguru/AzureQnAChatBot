using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Completions;
using OpenAI_API;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace azureqnachatbot.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatBotController : ControllerBase
    {
        private string ocpkey { get; set; }

        public ChatBotController(IConfiguration configuration)
        {
            this.ocpkey = configuration.GetValue<string>("ocpkey");
        }

        [HttpPost]
        public async Task<IActionResult> ChatBot([FromBody] string userInput)
        {

            
            try
            {
                string answer = string.Empty;
               
                string apiEndpoint = "langqnaservice.cognitiveservices.azure.com";
                string queryParam = "/language/:query-knowledgebases?projectName=qnaproject&api-version=2021-10-01&deploymentName=production";
                var httpClient = new HttpClient();

                var responseContent = "";
                var requestUri = $"https://{apiEndpoint}" + queryParam;

                var requestBodyNotJson = new
                {
                    top = 3,
                    question = userInput,
                    includeUnstructuredSources = true,
                    confidenceScoreThreshold = .95,
                    answerSpanRequest = new
                    {
                        enable = true,
                        topAnswersWithSpan = 1                        
                    }
                };
                string requestBody = JsonConvert.SerializeObject(requestBodyNotJson);
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.ocpkey);
                //request.Headers.Add("Content-Type", "application/json");

                var response = await httpClient.SendAsync(request);
                responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response from Model:");
                Console.WriteLine(responseContent);

                var responsejson = JsonConvert.DeserializeObject<AnswerResponse>(responseContent);
                answer = responsejson.Answers[0].answer;

                return Ok(new { Answer = answer });                

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "An error occurred while processing the request.- " + ex.Message });
            }
        }

    }

    public class AnswerResponse
    {
        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {
        public List<string> Questions { get; set; }
        public string answer { get; set; }
        public double ConfidenceScore { get; set; }
        public int Id { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

}
