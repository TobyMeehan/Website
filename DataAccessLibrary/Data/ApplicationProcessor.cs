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

        public ApplicationProcessor(IApplicationTable applicationTable, IUserTable userTable, IUserProcessor userProcessor)
        {
            _applicationTable = applicationTable;
            _userTable = userTable;
            _userProcessor = userProcessor;
        }

        public async Task<Application> GetApplicationById(string appid)
        {
            if (ValidateQuery(await _applicationTable.SelectById(appid), out Application app)) // If the application exists
            {
                if (ValidateQuery(await _userTable.SelectById(app.UserId), out User author)) // If the application's author exists
                {
                    app.Author = author;

                    return app;
                }
                else
                {
                    throw new Exception("Author of provided application could not be found.");
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<Application> GetApplicationByUserAndName(string userid, string name)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out User author)) // Check user exists
            {
                if (ValidateQuery(await _applicationTable.SelectByUserAndName(userid, name), out Application app))
                {
                    app.Author = author;

                    return app;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new ArgumentException("Provided user ID could not be found.");
            }

        }

        public async Task<List<Application>> GetApplicationsByName(string name)
        {
            var apps = await _applicationTable.SelectByName(name);

            if (apps.Any()) // Check at least one application has provided name
            {
                foreach (Application app in apps)
                {
                    if (ValidateQuery(await _userTable.SelectById(app.UserId), out User author)) // Check author of each application with provided name
                    {
                        app.Author = author;
                    }
                    else
                    {
                        throw new Exception("Author of provided application could not be found.");
                    }
                }

                return apps;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Application>> GetApplicationsByUser(string userid)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out User user)) // Check user id exists and get user it represents
            {
                var apps = await _applicationTable.SelectByUser(userid);

                if (apps.Any()) // If user has created at least one application
                {
                    apps.ForEach(app => app.Author = user);

                    return apps;
                }
                else
                {
                    return new List<Application>(); // Return empty list
                }
            }
            else
            {
                return new List<Application>();
            }
        }

        /// <summary>
        /// Adds a new application to the database, and generates a client secret, if required.
        /// </summary>
        /// <param name="app">Application model to add.</param>
        /// <param name="secret">Whether to generate a client secret.</param>
        /// <returns></returns>
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
            // TODO: Could add additional validation, I don't think it's necessary
        }
    }
}
