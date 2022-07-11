using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 
using Unity.RenderStreaming; 

namespace Gorani.Classroom {
    enum ActionType
    {
        ChangeLabel = 0,
    }

    [Serializable]
    class Message
    {
        public ActionType type;
        public string argument;
    }

    [Serializable]
    class ChangeLabelEvent : UnityEvent<string> {};

    class MultiplayChannel : DataChannelBase
    {
        public ChangeLabelEvent OnChangeLabel; 

        protected override void OnMessage(byte[] bytes)
        {
            string str = System.Text.Encoding.UTF8.GetString(bytes);
            var message = JsonUtility.FromJson<Message>(str);
            switch(message.type)
            {
                case ActionType.ChangeLabel:
                    OnChangeLabel?.Invoke(message.argument);
                    return;
            }
        }

        public void ChangeLabel(string text)
        {
            var msg = new Message
            {
                type = ActionType.ChangeLabel,
                argument = text
            };
            Send(JsonUtility.ToJson(msg));
        }
    }
}