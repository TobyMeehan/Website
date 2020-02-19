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

        public UserProcessor(IUserTable userTable, IRoleTable roleTable, IUserRoleTable userRoleTable)
        {
            _userTable = userTable;
            _roleTable = roleTable;
            _userRoleTable = userRoleTable;
        }

        public async Task<UserModel> GetUserById(string userid)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out UserModel user)) // Get user with specified id, check if user exists
            {
                List<UserRoleModel> userRoles = await _userRoleTable.SelectByUser(userid); // Get user role relations for the user

                foreach (UserRoleModel userRole in userRoles)
                {
                    if (ValidateQuery(await _roleTable.SelectById(userRole.RoleId), out RoleModel role)) // Get role associated with role id
                    {
                        user.Roles.Add(role); // Add role to user's list of roles
                    }
                }

                // TODO: repeat for alerts and anything else which needs to be added.

                return user;
            }
            else
            {
                throw new ArgumentException("Provided user ID could not be found.");
            }
        }

        public async Task<UserModel> GetUserByUsername(string username)
        {
            if (ValidateQuery(await _userTable.SelectByUsername(username), out UserModel user)) // Get user with specified username, check if user exists
            {
                List<UserRoleModel> userRoles = await _userRoleTable.SelectByUser(user.UserId); // Get user role relations for the user

                foreach (UserRoleModel userRole in userRoles)
                {
                    if (ValidateQuery(await _roleTable.SelectById(userRole.RoleId), out RoleModel role)) // Get role associated with role id
                    {
                        user.Roles.Add(role); // Add role to user's list of roles
                    }
                }

                return user;
            }

            throw new ArgumentException("Provided username could not be found.");
        }

        public async Task<bool> Authenticate(string username, string password)
        {
            if (ValidateQuery(await _userTable.SelectPassword(username), out PasswordModel item))
            {
                return BCrypt.CheckPassword(password, item.HashedPassword);
            }
            else
            {
                throw new ArgumentException("Provided username could not be found.");
            }
        }

        public async Task<UserModel> CreateUser(UserModel user, string password, List<RoleModel> roles)
        {
            if (!ValidateQuery(await _userTable.SelectByUsername(user.Username))) // Check if username already exists
            {
                string hashedPassword = BCrypt.HashPassword(password, BCrypt.GenerateSalt()); // Hash password

                await _userTable.Insert(user, hashedPassword); // Insert user into database

                user = await GetUserByUsername(user.Username);

                foreach (RoleModel role in roles)
                {
                    if (ValidateQuery(await _roleTable.SelectById(role.RoleId))) // Check if role exists
                    {
                        await _userRoleTable.Insert(new UserRoleModel { UserId = user.UserId, RoleId = role.RoleId }); // Insert new user role relation
                    }
                    else
                    {
                        throw new ArgumentException("Provided role does not exist.");
                        // TODO: Could fail gracefully and simply not add role
                    }
                }

                return await GetUserById(user.UserId);
            }
            else
            {
                throw new ArgumentException("Provided username is already in use.");
            }
        }

        public async Task AddRole(string userid, RoleModel role)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out UserModel user)) // Get user from provided ID
            {
                if (!user.Roles.Any(r => r.Name == role.Name)) // Check that the user does not already have the provided role
                {
                    if (ValidateQuery(await _roleTable.SelectById(role.RoleId))) // Check that the role exists
                    {
                        await _userRoleTable.Insert(new UserRoleModel { UserId = userid, RoleId = role.RoleId }); // Insert new user role relation
                    }
                }
            }
        }

        public async Task AddRoles(string userid, List<RoleModel> roles)
        {
            foreach (RoleModel role in roles)
            {
                await AddRole(userid, role);
            }
        }

        public async Task<bool> UserExists(string username)
        {
            return ValidateQuery(await _userTable.SelectByUsername(username));
        }

        public async Task DeleteUser(string userid)
        {
            await _userTable.Delete(userid);
        }
    }
}
