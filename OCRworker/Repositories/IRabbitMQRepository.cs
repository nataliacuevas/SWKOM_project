using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRworker.Repositories
{
    public delegate Task ProcessDelegate(string message);
    internal interface IRabbitMQRepository
    {
        void Subscribe(string queue, EventHandler<BasicDeliverEventArgs> subscription);
        void Send(string queue, string message);

        void SimpleSubscribe(string queue, ProcessDelegate subscription);
    }
}
