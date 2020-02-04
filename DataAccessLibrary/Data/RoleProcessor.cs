using DataAccessLibrary.Models;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
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

        public async Task<RoleModel> GetRoleById(int roleid)
        {
            if (ValidateQuery(await _roleTable.SelectById(roleid), out RoleModel role))
            {
                return role;
            }
            else
            {
                throw new ArgumentException("Provided role ID could not be found.");
            }
        }

        public async Task<RoleModel> GetRoleByName(string name)
        {
            if (ValidateQuery(await _roleTable.SelectByName(name), out RoleModel role))
            {
                return role;
            }
            else
            {
                throw new ArgumentException("Provided role name could not be found.");
            }
        }

        public async Task CreateRole(RoleModel role)
        {
            if (!ValidateQuery(await _roleTable.SelectByName(role.Name))) // Check if role name already exists
            {
                await _roleTable.Insert(role.Name);
            }
            else
            {
                throw new ArgumentException("Provided role name already exists.");
            }
        }

        public async Task DeleteRole(RoleModel role)
        {
            if (ValidateQuery(await _roleTable.SelectById(role.RoleId))) // Check that role exists
            {
                await _roleTable.Delete(role.RoleId);
            }
            else if (ValidateQuery(await _roleTable.SelectByName(role.Name), out role)) // Check that provided name exists
            {
                await _roleTable.Delete(role.RoleId);
            }
            else
            {
                throw new ArgumentException("No such role exists.");
            }
        }
    }
}
