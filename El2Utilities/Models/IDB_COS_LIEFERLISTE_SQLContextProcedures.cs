﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using El2Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace El2Core.Models
{
    public partial interface IDB_COS_LIEFERLISTE_SQLContextProcedures
    {
        Task<List<MachPlanVorgangsProcResult>> MachPlanVorgangsProcAsync(string accountID, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    }
}
