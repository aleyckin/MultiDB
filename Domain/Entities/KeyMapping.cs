using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class KeyMapping
    {
        public Guid UserId { get; set; }
        public string Key { get; set; }
    }
}
