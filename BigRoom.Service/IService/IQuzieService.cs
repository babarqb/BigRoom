﻿using BigRoom.Repository.Entities;
using BigRoom.Service.Common.Models;
using BigRoom.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BigRoom.Service.IService
{
    public interface IQuzieService : IServiceAsync<Quize, QuizeDto>
    {
        IEnumerable<QuizeDto> GetQuziesByGroup(int groupId);
    }
}