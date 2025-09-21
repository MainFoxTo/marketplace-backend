using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public class ReleaseRequestDto
    {
        public Guid? ReservationId { get; set; }
        public Guid? CorrelationId { get; set; }
    }
}
