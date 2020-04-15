using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Storage
{
    public interface IPkceTable
    {
        Task Insert(Pkce pkce);
        Task<List<Pkce>> Select(string clientid);
    }
}