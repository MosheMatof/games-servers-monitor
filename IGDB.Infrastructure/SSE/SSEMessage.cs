using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDB.Infrastructure.SSE
{
    public class SSEMessage
    {
        public string Event { get; set; }
        public string Id { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Event))
            {
                builder.AppendLine($"event: {Event}");
            }
            if (!string.IsNullOrEmpty(Id))
            {
                builder.AppendLine($"id: {Id}");
            }
            builder.AppendLine($"data: {Data}");
            builder.AppendLine();
            return builder.ToString();
        }
    }

}
