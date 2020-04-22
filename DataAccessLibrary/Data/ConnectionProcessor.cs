using DataAccessLibrary.Models;
using DataAccessLibrary.Security;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public ConnectionProcessor(IConnectionTable connectionTable, IUserTable userTable, IApplicationTable applicationTable, IAuthorizationCodeTable authorizationCodeTable)
        {
            _connectionTable = connectionTable;
            _userTable = userTable;
            _applicationTable = applicationTable;
            _authorizationCodeTable = authorizationCodeTable;
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
            if (!ValidateQuery(await _authorizationCodeTable.SelectByCode(code), out AuthorizationCode authCode))
            {
                return null;
            }

            if (authCode.Expiry < DateTime.Now)
            {
                await _authorizationCodeTable.DeleteById(authCode.Id);
                return null;
            }

            if (!ValidateQuery(await _connectionTable.SelectById(authCode.ConnectionId), out Connection connection))
            {
                await _authorizationCodeTable.DeleteByConnection(authCode.ConnectionId);
                return null;
            }

            authCode.Connection = await Populate(connection);
            return authCode;
        }

        public async Task<AuthorizationCode> CreateAuthorizationCode(string userid, string appid, string codeChallenge = null)
        {
            // If there isn't already a connection, create one
            if (!ValidateQuery(await _connectionTable.SelectByUserAndApplication(userid, appid), out Connection connection))
            {
                await _connectionTable.Insert(new Connection { UserId = userid, AppId = appid });
                connection = (await _connectionTable.SelectByUserAndApplication(userid, appid)).Single();
            }

            AuthorizationCode authCode = GenerateAuthCode(connection, DefaultExpiry);

            authCode.CodeChallenge = codeChallenge;

            await _authorizationCodeTable.Insert(authCode);

            return await GetAuthorizationCode(authCode.Code);
        }

        public bool CheckPkce(string codeChallenge, string codeVerifier)
        {
            if (codeChallenge == null && codeVerifier == null)
            {
                return true;
            }

            string hash = Pkce.ChallengeFromVerifier(codeVerifier);

            return codeChallenge == hash || codeChallenge == WebUtility.UrlEncode(hash); // also accept url encoded version as this is allowed
        }

        public async Task DeleteConnection(string connectionid)
        {
            await _connectionTable.Delete(connectionid);
        }

        public async Task DeleteInvalidAuthCodes()
        {
            foreach (var authCode in await _authorizationCodeTable.Select())
            {
                if (authCode.Expiry < DateTime.Now)
                {
                    await _authorizationCodeTable.DeleteById(authCode.Id);
                }
            }
        }
    }
}
