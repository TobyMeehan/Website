﻿using DataAccessLibrary.Models;
using DataAccessLibrary.Security;
using DataAccessLibrary.Storage;
using System;
using System.Collections.Generic;
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

        public async Task<ApplicationModel> GetApplicationById(string appid)
        {
            if (ValidateQuery(await _applicationTable.SelectById(appid), out ApplicationModel app)) // If the application exists
            {
                if (ValidateQuery(await _userTable.SelectById(app.UserId), out UserModel author)) // If the application's author exists
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

        public async Task<ApplicationModel> GetApplicationByUserAndName(string userid, string name)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out UserModel author)) // Check user exists
            {
                if (ValidateQuery(await _applicationTable.SelectByUserAndName(userid, name), out ApplicationModel app))
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

        public async Task<List<ApplicationModel>> GetApplicationsByName(string name)
        {
            var apps = await _applicationTable.SelectByName(name);

            if (ValidateQuery(apps)) // Check at least one application has provided name
            {
                foreach (ApplicationModel app in apps)
                {
                    if (ValidateQuery(await _userTable.SelectById(app.UserId), out UserModel author)) // Check author of each application with provided name
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

        public async Task<List<ApplicationModel>> GetApplicationsByUser(string userid)
        {
            if (ValidateQuery(await _userTable.SelectById(userid), out UserModel user)) // Check user id exists and get user it represents
            {
                var apps = await _applicationTable.SelectByUser(userid);

                if (ValidateQuery(apps)) // If user has created at least one application
                {
                    apps.ForEach(app => app.Author = user);

                    return apps;
                }
                else
                {
                    return new List<ApplicationModel>(); // Return empty list
                }
            }
            else
            {
                return new List<ApplicationModel>();
            }
        }

        /// <summary>
        /// Adds a new application to the database, and generates a client secret, if required.
        /// </summary>
        /// <param name="app">Application model to add.</param>
        /// <param name="secret">Whether to generate a client secret.</param>
        /// <returns></returns>
        public async Task<ApplicationModel> CreateApplication(ApplicationModel app, bool secret)
        {
            if (ValidateQuery(await _userTable.SelectById(app.Author.UserId))) // If provided app author exists
            {
                app.UserId = app.Author.UserId;

                if (ValidateQuery(await _applicationTable.SelectByUserAndName(app.UserId, app.Name))) // If an app with the same name has already been created by the author
                {
                    if (secret)
                    {
                        app.Secret = ClientSecret.Generate();
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
    }
}