using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Models
{
    public static class EntityExtensions
    {
        public static T As<T>(this IEntity entity, IMapper mapper)
        {
            return mapper.Map<T>(entity);
        }

        public static Task<T> AsAsync<T>(this IEntity entity, IMapper mapper)
        {
            return Task.Run(() => mapper.Map<T>(entity));
        }
    }
}
