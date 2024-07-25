using System;
using System.Collections.Generic;

public class AgentMessage
{
    public string role { get; set; }
    public List<Content> content { get; set; }
}

public class Content
{
    public string type { get; set; }
    public string text { get; set; }
}

public class AskData
{
    public string assistant_id { get; set; }
    public string user_id { get; set; }
    public bool stream { get; set; }
    public List<AgentMessage> messages { get; set; }
}


public class ReplyUsage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class ReplyToolCall
{
    public string id { get; set; }
    public string type { get; set; }
    public ReplyFunction function { get; set; }
}

public class ReplyFunction
{
    public string name { get; set; }
    public string desc { get; set; }
    public string type { get; set; }
    public string arguments { get; set; }
}

public class ReplyStep
{
    public string role { get; set; }
    public string content { get; set; }
    public ReplyUsage usage { get; set; }
    public int time_cost { get; set; }
    public List<ReplyToolCall> tool_calls { get; set; }
}

public class ReplyMessage
{
    public string role { get; set; }
    public string content { get; set; }
    public List<ReplyStep> steps { get; set; }
}

public class ReplyChoice
{
    public string finish_reason { get; set; }
    public ReplyMessage message { get; set; }
}

public class ChatData
{
    public string id { get; set; }
    public int created { get; set; }
    public List<ReplyChoice> choices { get; set; }
    public string assistant_id { get; set; }
    public ReplyUsage usage { get; set; }
}