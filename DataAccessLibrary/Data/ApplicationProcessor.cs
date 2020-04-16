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
    public class ApplicationProcessor : ProcessorBase, IApplicationProcessor
    {
        private readonly IApplicationTable _applicationTable;
        private readonly IUserTable _userTable;
        private readonly IUserProcessor _userProcessor;
        private readonly IScoreboardProcessor _scoreboardProcessor;

        public ApplicationProcessor(IApplicationTable applicationTable, IUserTable userTable, IUserProcessor userProcessor, IScoreboardProcessor scoreboardProcessor)
        {
            _applicationTable = applicationTable;
            _userTable = userTable;
            _userProcessor = userProcessor;
            _scoreboardProcessor = scoreboardProcessor;
        }

        private async Task<Application> Populate(Application application)
        {
            if (!(await _userTable.SelectById(application.UserId)).Any())
            {
                throw new Exception("Author of provided application could not be found.");
            }

            application.Author = await _userProcessor.GetUserById(application.UserId);
            application.Scoreboard = await _scoreboardProcessor.GetScoreboardByApplication(application.Id);

            return application;
        }

        public async Task<Application> GetApplicationById(string appid)
        {
            if (!ValidateQuery(await _applicationTable.SelectById(appid), out Application app)) // If the application exists
            {
                return null;
            }

            return await Populate(app);
        }

        public async Task<Application> GetApplicationByUserAndName(string userid, string name)
        {
            if (!ValidateQuery(await _applicationTable.SelectByUserAndName(userid, name), out Application app))
            {
                return null;
            }

            return await Populate(app);
        }

        public async Task<List<Application>> GetApplicationsByName(string name)
        {
            var apps = await _applicationTable.SelectByName(name);

            if (!apps.Any())
            {
                return new List<Application>();
            }

            foreach (Application app in apps)
            {
                await Populate(app);
            }

            return apps;
        }

        public async Task<List<Application>> GetApplicationsByUser(string userid)
        {
            var apps = await _applicationTable.SelectByUser(userid);

            if (!apps.Any())
            {
                return new List<Application>();
            }

            foreach (Application app in apps)
            {
                await Populate(app);
            }

            return apps;
        }

        public async Task<Application> CreateApplication(Application app, bool secret)
        {
            if ((await _userTable.SelectById(app.Author.Id)).Any()) // If provided app author exists
            {
                app.UserId = app.Author.Id;

                if (!(await _applicationTable.SelectByUserAndName(app.UserId, app.Name)).Any()) // If an app with the same name has not already been created by the author
                {
                    if (secret)
                    {
                        app.Secret = RandomString.GenerateCrypto();
                    }

                    await _applicationTable.Insert(app);

                    return await GetApplicationByUserAndName(app.UserId, app.Name);
                }
                else
                {
                    throw new ArgumentException("Application with the same name has already been created by the user.");
                }
            }
            else
            {
                throw new ArgumentException("Provided user could not be found.");
            }
        }

        public async Task UpdateApplication(Application app)
        {
            if ((await _userTable.SelectById(app.Author.Id)).Any()) // If provided app author exists
            {
                if ((await _applicationTable.SelectById(app.Id)).Any()) // If provided app ID exists
                {
                    if (ValidateQuery(await _applicationTable.SelectByUserAndName(app.UserId, app.Name), out Application match) && match.Id == app.Id) // Check if an application with the same name has already been created. Check that the ID of the two applications match
                    {
                        await _applicationTable.Update(app);
                    }
                    else
                    {
                        throw new ArgumentException("Application with the same name has already been created by the user.");
                    }
                }
                else
                {
                    throw new ArgumentException("Provided application ID could not be found.");
                }
            }
            else
            {
                throw new ArgumentException("Provided user could not be found.");
            }
        }

        public async Task DeleteApplication(string appid)
        {
            await _applicationTable.Delete(appid);
        }

        public async Task<bool> ValidateApplication(string clientId, string clientSecret, string redirectUri, bool ignoreSecret)
        {
            Application app = await GetApplicationById(clientId);

            if (app == null)
                return false;

            bool valid = true;

            valid = (clientSecret != null || ignoreSecret) && valid;
            valid = (app.Secret == clientSecret || ignoreSecret) && valid;

            valid = app.RedirectUri == redirectUri && valid;

            return valid;
        }
    }
}
