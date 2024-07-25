using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCoreWeChatOAMessage.Controllers
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
                    if (content.ToUpper().StartsWith("BMI"))
                    {
                        var bmiArr = content.Split(" ");
                        if (bmiArr.Length == 3 && double.TryParse(bmiArr[1], out double height) && double.TryParse(bmiArr[2], out double weight))
                        {
                            var bmi = (weight / ((height / 100) * (height / 100)));
                            var result = $"您的身体质量指数(BMI)为{bmi:#.0}，";
                            if (bmi < 18.5) result += "可有点偏瘦啦，多吃点啦🧋~";
                            else if (bmi < 23.9) result += "在正常范围内，请继续保持🎉 ~";
                            else if (bmi < 26.9) result += "有点偏胖喽，可以去运动运动啦 🏃‍ ~";
                            else if (bmi < 29.9) result += "处于肥胖阶段，要去运动一下啦 🏋 ~";
                            else result += "很危险，去医院看看吧 ⛑ !";
                            replyMessage.Content = result;
                        }
                    }
                }
                return OfficialAccount.PassiveUserReplyMessage.Parse(replyMessage);
            }
            return "hello, this is handle view";
        }

    }
}

