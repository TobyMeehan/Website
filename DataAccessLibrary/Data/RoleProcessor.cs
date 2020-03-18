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
