using System;
using Ink.Runtime;

namespace CaptainPinkTurd.InkDialogue
{
    public class InkExternalFunctions
    {
        public void Bind(Story story, string functionName, Action function)
        {
            story.BindExternalFunction(functionName, function);
        }
        public void Unbind(Story story, string functionName)
        {
            story.UnbindExternalFunction(functionName);
        }
    }
}