using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.Models
{
    public abstract class EntityBase
    {
        public string Id { get; set; }

        public T As<T>(IMapper mapper)
        {
            return mapper.Map<T>(this);
        }

        public Task<T> AsAsync<T>(IMapper mapper)
        {
            return Task.Run(() => mapper.Map<T>(this));
        }
    }
}
