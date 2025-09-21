﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public class ReleaseResultDto
    {
        public Guid ReservationId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = default!;
        public DateTime ReleasedAt { get; set; }
    }
}
