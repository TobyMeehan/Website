using DataAccessLibrary.Models;
using DataAccessLibrary.Security;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class UserProcessor : ProcessorBase, IUserProcessor
    {
        private readonly IUserTable _userTable;
        private readonly IRoleTable _roleTable;
        private readonly IUserRoleTable _userRoleTable;
        private readonly IDownloadTable _downloadTable;
        private readonly IDownloadAuthorTable _downloadAuthorTable;
        private readonly ITransactionTable _transactionTable;
        private readonly IScoreboardTable _scoreboardTable;

        public UserProcessor(IUserTable userTable, IRoleTable roleTable, IUserRoleTable userRoleTable, IDownloadTable downloadTable, IDownloadAuthorTable downloadAuthorTable, ITransactionTable transactionTable, IScoreboardTable scoreboardTable)
        {
            _userTable = userTable;
            _roleTable = roleTable;
            _userRoleTable = userRoleTable;
            _downloadTable = downloadTable;
            _downloadAuthorTable = downloadAuthorTable;
            _transactionTable = transactionTable;
            _scoreboardTable = scoreboardTable;
        }

        private async Task<User> Populate(User user)
        {
            user.Roles = new List<Role>();

            List<UserRoleModel> userRoles = await _userRoleTable.SelectByUser(user.Id);

            foreach (UserRoleModel userRole in userRoles)
            {
                if (ValidateQuery(await _roleTable.SelectById(userRole.RoleId), out Role role))
                {
                    user.Roles.Add(role);
                }
            }

            user.Roles.Sort(RoleProcessor.CompareRoles);

            user.Transactions = await _transactionTable.SelectByUser(user.Id) ?? new List<Transaction>();

            return user;

            // TODO: repeat for alerts and anything else which needs to be added.
            // TODO: Replace any manual population with this method.
        }

        public async Task<User> GetUserById(string userid)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out User user)) // Get user with specified id, check if user exists
            {
                return await Populate(user);
            }
            else
            {
                throw new ArgumentException("Provided user ID could not be found.");
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            if (ValidateQuery(await _userTable.SelectByUsername(username), out User user)) // Get user with specified username, check if user exists
            {
                return await Populate(user);
            }

            throw new ArgumentException("Provided username could not be found.");
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> users = await _userTable.Select();

            foreach (User user in users)
            {
                await Populate(user);
            }

            return users;
        }

        public async Task<List<User>> GetUsersByRole(string rolename)
        {
            if (ValidateQuery(await _roleTable.SelectByName(rolename), out Role role))
            {
                List<UserRoleModel> userRoles = await _userRoleTable.SelectByRole(role.Id);
                List<User> users = new List<User>();

                foreach (UserRoleModel item in userRoles)
                {
                    if (ValidateQuery(await _userTable.SelectById(item.UserId), out User user))
                    {
                        users.Add(await Populate(user));
                    }
                }

                return users;
            }
            else
            {
                throw new ArgumentException("No role found with specified name.");
            }
        }

        public async Task<bool> Authenticate(string username, string password)
        {
            if (ValidateQuery(await _userTable.SelectPassword(username), out PasswordModel item))
            {
                return BCrypt.CheckPassword(password, item.HashedPassword);
            }
            else
            {
                return false;
            }
        }

        public async Task<User> CreateUser(User user, string password)
        {
            if (!(await _userTable.SelectByUsername(user.Username)).Any()) // Check if username already exists
            {
                string hashedPassword = BCrypt.HashPassword(password, BCrypt.GenerateSalt()); // Hash password

                await _userTable.Insert(user, hashedPassword); // Insert user into database

                user = await GetUserByUsername(user.Username);

                foreach (Role role in user.Roles)
                {
                    if ((await _roleTable.SelectById(role.Id)).Any()) // Check if role exists
                    {
                        await _userRoleTable.Insert(new UserRoleModel { UserId = user.Id, RoleId = role.Id }); // Insert new user role relation
                    }
                    else
                    {
                        throw new ArgumentException("Provided role does not exist.");
                        // TODO: Could fail gracefully and simply not add role
                    }
                }

                return await GetUserById(user.Id);
            }
            else
            {
                throw new ArgumentException("Provided username is already in use.");
            }
        }

        public async Task UpdateUsername(string userid, string username)
        {
            if (!(await _userTable.SelectByUsername(username)).Any())
            {
                if ((await _userTable.SelectById(userid)).Any())
                {
                    await _userTable.UpdateUsername(userid, username);
                }
                else
                {
                    throw new ArgumentException("Provided user id could not be found.");
                }
            }
            else
            {
                throw new ArgumentException("Provided username is already in use.");
            }
        }

        public async Task AddRole(string userid, Role role)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out User user)) // Get user from provided ID
            {
                user.Roles = new List<Role>();

                List<UserRoleModel> userRoles = await _userRoleTable.SelectByUser(userid); // Get user role relations for the user

                foreach (UserRoleModel userRole in userRoles)
                {
                    if (ValidateQuery(await _roleTable.SelectById(userRole.RoleId), out Role r)) // Get role associated with role id
                    {
                        user.Roles.Add(r); // Add role to user's list of roles
                    }
                }

                if (!user.Roles.Any(r => r.Name == role.Name)) // Check that the user does not already have the provided role
                {
                    if ((await _roleTable.SelectById(role.Id)).Any()) // Check that the role exists
                    {
                        await _userRoleTable.Insert(new UserRoleModel { UserId = userid, RoleId = role.Id }); // Insert new user role relation
                    }
                }
            }
        }

        public async Task AddRoles(string userid, List<Role> roles)
        {
            foreach (Role role in roles)
            {
                await AddRole(userid, role);
            }
        }

        public async Task RemoveRole(string userid, Role role)
        {
            if ((await _userTable.SelectById(userid)).Any())
            {
                UserRoleModel userRole = new UserRoleModel { UserId = userid, RoleId = role.Id };
                await _userRoleTable.Delete(userRole);
            }
        }

        public async Task<bool> TrySendTransaction(Transaction transaction)
        {
            if (!ValidateQuery(await _userTable.SelectById(transaction.UserId), out User user))
            {
                return false;
            }

            if (user.Balance + transaction.Amount < 0)
            {
                return false;
            }

            await _userTable.UpdateBalance(transaction.UserId, transaction.Amount);
            await _transactionTable.Insert(transaction);

            return true;
        }

        public async Task<bool> UserExists(string username)
        {
            return (await _userTable.SelectByUsername(username)).Any();
        }

        public async Task DeleteUser(string userid)
        {
            await _downloadTable.DeleteByUser(userid);
            await _downloadAuthorTable.DeleteByUser(userid);
            await _userRoleTable.DeleteByUser(userid);
            await _transactionTable.DeleteByUser(userid);
            await _scoreboardTable.DeleteByUser(userid);
            await _userTable.Delete(userid);
        }

        public async Task UpdatePassword(string userid, string password)
        {
            if ((await _userTable.SelectById(userid)).Any())
            {
                string hashedPassword = BCrypt.HashPassword(password, BCrypt.GenerateSalt());

                await _userTable.UpdatePassword(userid, hashedPassword);
            }
            else
            {
                throw new ArgumentException("Provided user id could not be found.");
            }
        }
    }
}
