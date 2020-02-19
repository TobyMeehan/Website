using DataAccessLibrary.Models;
using DataAccessLibrary.Security;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class ConnectionProcessor : ProcessorBase, IConnectionProcessor
    {
        private readonly IConnectionTable _connectionTable;
        private readonly IUserTable _userTable;
        private readonly IApplicationTable _applicationTable;

        public ConnectionProcessor(IConnectionTable connectionTable, IUserTable userTable, IApplicationTable applicationTable)
        {
            _connectionTable = connectionTable;
            _userTable = userTable;
            _applicationTable = applicationTable;
        }

        public string GenerateAuthCode(int length = 32)
        {
            return $"{SecureSecret.Generate(length)}{DateTime.Now.ToBinary()}";
        }

        public async Task<ConnectionModel> CreateConnection(ConnectionModel connection)
        {
            if (ValidateQuery(await _userTable.SelectById(connection.User.UserId))) // Check that provided user exists
            {
                if (ValidateQuery(await _applicationTable.SelectById(connection.Application.AppId))) // Check that provided application exists
                {
                    connection.UserId = connection.User.UserId;
                    connection.AppId = connection.Application.AppId;

                    connection.AuthorizationCode = GenerateAuthCode();

                    if (ValidateQuery(await _connectionTable.SelectByUserAndApplication(connection.User.UserId, connection.Application.AppId))) // If a connection between the provided user and application already exists
                    {
                        await _connectionTable.Update(connection);
                    }
                    else
                    {
                        await _connectionTable.Insert(connection);
                    }

                    if (ValidateQuery(await _connectionTable.SelectByUserAndApplication(connection.UserId, connection.AppId), out ConnectionModel output))
                    {
                        return output;
                    }
                    else
                    {
                        throw new Exception("Error occurred getting new connection.");
                    }
                }
                else
                {
                    throw new ArgumentException("Provided application ID could not be found.");
                }
            }
            else
            {
                throw new ArgumentException("Provided user ID could not be found.");
            }
        }

        public async Task<ConnectionModel> GetConnectionByAuthCode(string authorizationCode)
        {
            if (ValidateQuery(await _connectionTable.SelectByAuthCode(authorizationCode), out ConnectionModel connection))
            {
                if (ValidateQuery(await _userTable.SelectById(connection.UserId), out UserModel user))
                {
                    if (ValidateQuery(await _applicationTable.SelectById(connection.AppId), out ApplicationModel app))
                    {
                        connection.User = user;
                        connection.Application = app;

                        return connection;
                    }
                    else
                    {
                        throw new ArgumentException("Provided application ID could not be found.");
                    }
                }
                else
                {
                    throw new ArgumentException("Provided user ID could not be found.");
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ValidateAuthCode(string authorizationCode)
        {
            string date = authorizationCode.Remove(0, 32);
            
            if (long.TryParse(date, out long binDate))
            {
                DateTime validDate = DateTime.FromBinary(binDate).AddHours(2);

                if (DateTime.Now < validDate)
                {
                    if (ValidateQuery(await _connectionTable.SelectByAuthCode(authorizationCode)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
