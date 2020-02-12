﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class DeveloperController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;

        public DeveloperController(IMapper mapper, IUserProcessor userProcessor, IApplicationProcessor applicationProcessor)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Index()
        {
            List<ApplicationModel> apps = _mapper.Map<List<ApplicationModel>>(await _applicationProcessor.GetApplicationsByUser(User.Identity.Name));

            return View(apps);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationFormModel appForm)
        {
            if (ModelState.IsValid)
            {
                if (await _applicationProcessor.GetApplicationByUserAndName(User.Identity.Name, appForm.Name) == null) // Check if application already exists
                {
                    bool secret = appForm.Type == ApplicationFormModel.ApplicationType.WebServer; // Only generate secret for webserver applications

                    if (string.IsNullOrWhiteSpace(appForm.RedirectUri))
                    {
                        appForm.RedirectUri = "localhost:6969";
                    }

                    var app = new ApplicationModel
                    {
                        Author = _mapper.Map<UserModel>(await _userProcessor.GetUserById(User.Identity.Name)),
                        Name = appForm.Name,
                        RedirectUri = appForm.RedirectUri
                    };

                    await _applicationProcessor.CreateApplication(_mapper.Map<DataAccessLibrary.Models.ApplicationModel>(app), secret);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", "You have already created an application with this name.");

                    return View(appForm);
                }
            }
            else
            {
                return View(appForm);
            }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [Route("/Edit/{appid}")]
        public async Task<IActionResult> Edit(string appid)
        {
            ApplicationModel app = _mapper.Map<ApplicationModel>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ApplicationFormModel viewModel = new ApplicationFormModel
                {
                    Name = app.Name,
                    RedirectUri = app.RedirectUri
                };

                return View(viewModel);
            }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Edit/{appid}")]
        public async Task<IActionResult> Edit(string appid, ApplicationFormModel appForm)
        {
            if (ModelState.IsValid)
            {
                if (await _applicationProcessor.GetApplicationByUserAndName(User.Identity.Name, appForm.Name) == null)
                {
                    if (string.IsNullOrWhiteSpace(appForm.RedirectUri))
                    {
                        appForm.RedirectUri = "localhost:6969";
                    }

                    var app = new ApplicationModel
                    {
                        AppId = appid,
                        Author = _mapper.Map<UserModel>(await _userProcessor.GetUserById(User.Identity.Name)),
                        Name = appForm.Name,
                        RedirectUri = appForm.RedirectUri
                    };

                    await _applicationProcessor.UpdateApplication(_mapper.Map<DataAccessLibrary.Models.ApplicationModel>(app));

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", "You have already created an application with this name.");

                    return View(appForm);
                }
            }
            else
            {
                return View(appForm);
            }
        }


    }
}