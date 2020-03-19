using DataAccessLibrary.Models;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class RoleProcessor : ProcessorBase, IRoleProcessor
    {
        private readonly IRoleTable _roleTable;

        public RoleProcessor(IRoleTable roleTable)
        {
            _roleTable = roleTable;
        }

        /// <summary>
        /// Compares roles in order to rank the most significant. Allows the highest ranked role to be first in list.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareRoles(Role x, Role y)
        {
            if (x == null) // If x is null
            {
                if (y == null) // and y is null
                {
                    return 0; // they're equal
                }
                else // and y is not null
                {
                    return -1; // y is greater
                }
            }
            else // If x is not null
            {
                if (y == null) // and y is null
                {
                    return 1; // x is greater
                }
                else // Compare names
                {
                    if (x.Name == y.Name) return 0; // Make sure there are no issues from duplicate names

                    // Sort roles in order of rank
                    if (x.Name == UserRoles.Admin) return 1; // y cannot be admin as the two names are not equal
                    if (y.Name == UserRoles.Admin) return -1; // As above, for x

                    if (x.Name == UserRoles.Verified) return 1; // y is neither verified nor admin as we checked against both above
                    if (y.Name == UserRoles.Verified) return -1; // As above, for x

                    return x.Name.CompareTo(y.Name); // We have checked all our special cases so we can simply use default comparison
                }
            }
        }

        public async Task<Role> GetRoleById(string roleid)
        {
            if (ValidateQuery(await _roleTable.SelectById(roleid), out Role role))
            {
                return role;
            }
            else
            {
                throw new ArgumentException("Provided role ID could not be found.");
            }
        }

        public async Task<Role> GetRoleByName(string name)
        {
            if (ValidateQuery(await _roleTable.SelectByName(name), out Role role))
            {
                return role;
            }
            else
            {
                throw new ArgumentException("Provided role name could not be found.");
            }
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _roleTable.Select();
        }

        public async Task CreateRole(string name)
        {
            if (!(await _roleTable.SelectByName(name)).Any()) // Check if role name already exists
            {
                await _roleTable.Insert(name);
            }
            else
            {
                throw new ArgumentException("Provided role name already exists.");
            }
        }

        public async Task DeleteRole(Role role)
        {
            if ((await _roleTable.SelectById(role.Id)).Any()) // Check that role exists
            {
                await _roleTable.Delete(role.Id);
            }
            else if (ValidateQuery(await _roleTable.SelectByName(role.Name), out role)) // Check that provided name exists
            {
                await _roleTable.Delete(role.Id);
            }
            else
            {
                throw new ArgumentException("No such role exists.");
            }
        }
    }
}
