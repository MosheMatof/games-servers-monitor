using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Messaging
{
    class GameMessage
    {
        public object Sender { get; }
        public string PropertyName { get;}
        public object Value { get; }

        public GameMessage(object sender, string propertyName, object value = null)
        {
            Sender = sender;
            PropertyName = propertyName;
            Value = value;
        }
    }
}
