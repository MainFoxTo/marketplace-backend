﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public class ReserveRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
