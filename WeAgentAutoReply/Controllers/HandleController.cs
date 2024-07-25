using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using RestSharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeAgentAutoReply.Controllers
{
    [Route("api/")]
    [ApiController]
    public class HandleController : ControllerBase
    {
        private readonly OfficialAccount oa;
        public HandleController(OfficialAccount oa)
        {
            this.oa = oa;
        }
        [HttpGet("OfficialAccount")]
        public string GetOfficialAccount()
        {
            Request.Query.TryGetValue("signature", out StringValues signature);
            Request.Query.TryGetValue("timestamp", out StringValues timestamp);
            Request.Query.TryGetValue("nonce", out StringValues nonce);
            Request.Query.TryGetValue("echostr", out StringValues echostr);
            if (oa.CheckSignature(timestamp, nonce, signature)) return echostr;
            return "hello, this is handle view";
        }
        [HttpPost("OfficialAccount")]
        public string PostOfficialAccount()
        {
            StreamReader stream = new StreamReader(Request.Body);
            string messageStr = stream.ReadToEndAsync().GetAwaiter().GetResult();
            var message = OfficialAccount.ReceivingStandardMessages.Parse(messageStr);
            if (message is OfficialAccount.ReceivingStandardMessages.TextMessage)
            {
                var textMessage = (OfficialAccount.ReceivingStandardMessages.TextMessage)message;
                Console.WriteLine(textMessage.Content);
                var content = textMessage.Content;
                var replyMessage = new OfficialAccount.PassiveUserReplyMessage.TextMessage
                {
                    Content = "你好，很高兴认识你~",
                    CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    ToUserName = message.FromUserName,
                    FromUserName = message.ToUserName,
                    MsgType = message.MsgType,
                };
                if (content != null)
                {
                    AskData askData=   SetAskData(content);
                    if (askData != null)
                    {
                        string baseurl = "https://yuanqi.tencent.com/openapi/v1/agent/chat/completions";
                        string bearerToken = "uHut6fq9nNa0a2cFjviRyj1ED10ZXVsf";
                
                        var client = new RestClient(baseurl);
                        var request = new RestRequest();
                        request.Method = RestSharp.Method.Post;
                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("Authorization", $"Bearer {bearerToken}");
                        request.AddJsonBody(askData);

                        var response =  client.Execute<ChatData>(request);

                        if (response.IsSuccessful)
                        {
                            Console.WriteLine("Success!");
                            ChatData chatData = JsonConvert.DeserializeObject<ChatData>(response.Content);
                            var json = JsonConvert.SerializeObject(chatData);
                            // Access the content of the first ReplyMessage
                            if (chatData?.choices.Count > 0 && chatData.choices[0].message.steps.Count > 0)
                            {
                                string firstReplyMessageContent = chatData.choices[0].message.content;
                                replyMessage.Content = firstReplyMessageContent;
                                Console.WriteLine("First ReplyMessage content: " + firstReplyMessageContent);
                            }
                            else
                            {
                                Console.WriteLine("No ReplyMessage found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: " + response.ErrorMessage);
                        }

                    }
                }
                return OfficialAccount.PassiveUserReplyMessage.Parse(replyMessage);
            }
            return "hello, this is handle view";
        }

        private AskData SetAskData(string content)
        {
            AskData askData = new AskData()
            {
                assistant_id = "hfr0hjaEDPYL",
                user_id = "rodneyxiong",
                stream = false,
                messages = new List<AgentMessage>()
                {
                    new AgentMessage()
                    {
                        role = "user",
                        content = new List<Content>()
                        {
                            new Content()
                            {
                                type = "text",
                                text =content
                            }
                        }
                    }
                }
            };

            return askData;
        }
    }
}

