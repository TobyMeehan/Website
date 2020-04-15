using DataAccessLibrary.Models;
using DataAccessLibrary.Security;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class ConnectionProcessor : ProcessorBase, IConnectionProcessor
    {
        private readonly IConnectionTable _connectionTable;
        private readonly IUserTable _userTable;
        private readonly IApplicationTable _applicationTable;
        private readonly IAuthorizationCodeTable _authorizationCodeTable;
        private readonly IPkceTable _pkceTable;

        public ConnectionProcessor(IConnectionTable connectionTable, IUserTable userTable, IApplicationTable applicationTable, IAuthorizationCodeTable authorizationCodeTable, IPkceTable pkceTable)
        {
            _connectionTable = connectionTable;
            _userTable = userTable;
            _applicationTable = applicationTable;
            _authorizationCodeTable = authorizationCodeTable;
            _pkceTable = pkceTable;
        }

        private DateTime DefaultExpiry => DateTime.Now.AddMinutes(30);

        private AuthorizationCode GenerateAuthCode(Connection connection, DateTime expiry, int length = 32)
        {
            return new AuthorizationCode
            {
                ConnectionId = connection.Id,
                Expiry = expiry,
                Code = RandomString.GenerateCrypto(length)
            };
        }

        private async Task<Connection> Populate(Connection connection)
        {
            connection.Application = (await _applicationTable.SelectById(connection.AppId)).SingleOrDefault();
            connection.User = (await _userTable.SelectById(connection.UserId)).SingleOrDefault();

            return connection;
        }

        public async Task<Connection> GetConnectionById(string connectionid)
        {
            if (ValidateQuery(await _connectionTable.SelectById(connectionid), out Connection connection))
            {
                return await Populate(connection);
            }

            return null;
        }

        public async Task<Connection> GetConnectionByUserAndApplication(string userid, string appid)
        {
            if (ValidateQuery(await _connectionTable.SelectByUserAndApplication(userid, appid), out Connection connection))
            {
                return await Populate(connection);
            }

            return null;
        }

        public async Task<List<Connection>> GetConnectionsByUser(string userid)
        {
            List<Connection> connections = await _connectionTable.SelectByUser(userid) ?? new List<Connection>();

            foreach (Connection connection in connections)
            {
                await Populate(connection);
            }

            return connections;
        }

        public async Task<AuthorizationCode> GetAuthorizationCode(string code)
        {
            if (ValidateQuery(await _authorizationCodeTable.SelectByCode(code), out AuthorizationCode authCode))
            {
                if (authCode.Expiry >= DateTime.Now)
                {
                    if (ValidateQuery(await _connectionTable.SelectById(authCode.ConnectionId), out Connection connection))
                    {
                        authCode.Connection = await Populate(connection);
                        return authCode;
                    }
                }
            }

            return null;
        }

        public async Task<Pkce> GetPkce(string clientid)
        {
            if (ValidateQuery(await _pkceTable.Select(clientid), out Pkce pkce))
            {
                return pkce;
            }
            else
            {
                return null;
            }
        }

        public async Task<AuthorizationCode> CreateAuthorizationCode(string userid, string appid)
        {
            // If there isn't already a connection, create one
            if (!ValidateQuery(await _connectionTable.SelectByUserAndApplication(userid, appid), out Connection connection))
            {
                await _connectionTable.Insert(new Connection { UserId = userid, AppId = appid });
                connection = (await _connectionTable.SelectByUserAndApplication(userid, appid)).Single();
            }

            AuthorizationCode authCode = GenerateAuthCode(connection, DefaultExpiry);
            await _authorizationCodeTable.Insert(authCode);

            return await GetAuthorizationCode(authCode.Code);
        }

        public async Task CreatePkce(Pkce pkce)
        {
            await _pkceTable.Insert(pkce);
        }

        public async Task<bool> ValidatePkce(Pkce pkce)
        {
            pkce.CodeChallenge = Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(pkce.CodeVerifier)));

            List<Pkce> codes = await _pkceTable.Select(pkce.ClientId);
            return codes.Any(c => c.CodeChallenge == pkce.CodeChallenge);
        }

        public async Task DeleteConnection(string connectionid)
        {
            await _connectionTable.Delete(connectionid);
        }
    }
}
