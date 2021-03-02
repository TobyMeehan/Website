using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;

namespace TobyMeehan.Com.Data.Repositories
{
    public class SqlButtonRepository : SqlRepository<ButtonPress>, IButtonRepository
    {
        private readonly ISqlTable<ButtonPress> _table;
        private readonly IRoleRepository _roles;
        private readonly IUserRepository _users;
        private readonly TheButtonOptions _buttonOptions;

        public SqlButtonRepository(ISqlTable<ButtonPress> table, IRoleRepository roles, IUserRepository users, IOptions<TheButtonOptions> buttonOptions) : base(table)
        {
            _table = table;
            _roles = roles;
            _users = users;
            _buttonOptions = buttonOptions.Value;
        }

        public async Task AddAsync(string userId, TimeSpan buttonTimeSpan)
        {
            string id = Guid.NewGuid().ToString();

            string buttonRoleName = GetButtonRole((int)buttonTimeSpan.TotalSeconds);

            if (buttonRoleName == null) return;

            await _table.InsertAsync(new
            {
                Id = id,
                UserId = userId,
                TimePressed = DateTime.Now,
                ButtonSeconds = buttonTimeSpan.Seconds
            });

            EntityCollection<Role> roles = (await _roles.GetAsync()).ToEntityCollection();

            foreach (var role in roles.Where(r => UserRoles.ButtonRoles.Contains(r.Name)))
            {
                await _users.RemoveRoleAsync(userId, role.Id);
            }

            await _users.AddRoleAsync(userId, roles.Single(r => r.Name == buttonRoleName).Id);
        }

        public int GetButtonPercentage(int buttonSeconds)
        {
            double totalSeconds = _buttonOptions.TimeSpan.TotalSeconds;

            return (int)(((double)buttonSeconds / totalSeconds) * 100d);
        }

        private string GetButtonRole(int buttonSeconds)
        {
            switch (GetButtonPercentage(buttonSeconds))
            {
                case int i when i > 100: // the button is dead, ignore
                    return null;
                case int i when i > 93: // red
                    return UserRoles.Red;
                case int i when i > 80: // orange
                    return UserRoles.Orange;
                case int i when i > 60: // yellow
                    return UserRoles.Yellow;
                case int i when i > 40: // green
                    return UserRoles.Green;
                case int i when i > 20: // blue
                    return UserRoles.Blue;
                default: // purple
                    return UserRoles.Purple;
            }
        }

        public async Task<IList<ButtonPress>> GetByUserAsync(string userId)
        {
            return (await _table.SelectByAsync(b => b.UserId == userId)).ToList();
        }
    }
}
