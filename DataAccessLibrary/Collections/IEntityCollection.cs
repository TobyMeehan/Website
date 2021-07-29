using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Collections
{
    public interface IEntityCollection<T> : IEnumerable<T> where T : EntityBase
    {
        T this[string id] { get; }
    }
}
