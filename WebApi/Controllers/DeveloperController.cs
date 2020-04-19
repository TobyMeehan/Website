using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DeveloperController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserProcessor _userProcessor;
        private readonly IApplicationProcessor _applicationProcessor;
        private readonly IScoreboardProcessor _scoreboardProcessor;

        public DeveloperController(IMapper mapper, IAuthorizationService authorizationService, IUserProcessor userProcessor, IApplicationProcessor applicationProcessor, IScoreboardProcessor scoreboardProcessor)
        {
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userProcessor = userProcessor;
            _applicationProcessor = applicationProcessor;
            _scoreboardProcessor = scoreboardProcessor;
        }

        public string UserId => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        public async Task<IActionResult> Index()
        {
            List<Application> apps = _mapper.Map<List<Application>>(await _applicationProcessor.GetApplicationsByUser(UserId));

            return View(apps);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationFormModel appForm)
        {
            if (ModelState.IsValid)
            {
                if (await _applicationProcessor.GetApplicationByUserAndName(UserId, appForm.Name) == null) // Check if application already exists
                {
                    bool secret = appForm.Type == ApplicationFormModel.ApplicationType.WebServer; // Only generate secret for webserver applications

                    if (string.IsNullOrWhiteSpace(appForm.RedirectUri))
                    {
                        appForm.RedirectUri = "localhost:6969";
                    }

                    var app = new Application
                    {
                        Author = _mapper.Map<User>(await _userProcessor.GetUserById(UserId)),
                        Name = appForm.Name,
                        RedirectUri = appForm.RedirectUri
                    };

                    await _applicationProcessor.CreateApplication(_mapper.Map<DataAccessLibrary.Models.Application>(app), secret);

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

        [Route("/Developer/{appid}")]
        public async Task<IActionResult> Details(string appid)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _authorizationService.AuthorizeAsync(User, app, "ApplicationPolicy")).Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(app);
        }

        [Route("/Developer/Edit/{appid}")]
        public async Task<IActionResult> Edit(string appid)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, app, "ApplicationPolicy");

                if (authorizationResult.Succeeded)
                {
                    ApplicationFormModel viewModel = new ApplicationFormModel
                    {
                        Id = appid,
                        Name = app.Name,
                        RedirectUri = app.RedirectUri
                    };

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Developer/Edit/{appid}")]
        public async Task<IActionResult> Edit(string appid, ApplicationFormModel appForm)
        {
            if (!ModelState.IsValid)
            {
                return View(appForm);
            }

            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _authorizationService.AuthorizeAsync(User, app, "ApplicationPolicy")).Succeeded)
            {
                return RedirectToAction("Index");
            }

            app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationByUserAndName(UserId, appForm.Name));

            if (app != null && appid != app?.Id)
            {
                ModelState.AddModelError("Name", "You have already created an application with this name.");
                return View(appForm);
            }

            if (string.IsNullOrWhiteSpace(appForm.RedirectUri))
            {
                appForm.RedirectUri = "localhost:6969";
            }

            app = new Application
            {
                Id = appid,
                Author = _mapper.Map<User>(await _userProcessor.GetUserById(UserId)),
                Name = appForm.Name,
                RedirectUri = appForm.RedirectUri
            };

            await _applicationProcessor.UpdateApplication(_mapper.Map<DataAccessLibrary.Models.Application>(app));

            return RedirectToAction("Details", new { appid });
        }

        [Route("/Developer/Scoreboard/{appid}")]
        public async Task<IActionResult> Scoreboard(string appid)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _authorizationService.AuthorizeAsync(User, app, "ApplicationPolicy")).Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(app);
        }

        [HttpPost]
        [Route("/Developer/Objective/{appid}")]
        public async Task<IActionResult> AddObjective(string appid, [FromForm] string name)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _authorizationService.AuthorizeAsync(User, app, "ApplicationPolicy")).Succeeded)
            {
                return RedirectToAction("Index");
            }

            await _scoreboardProcessor.CreateObjective(new DataAccessLibrary.Models.Objective { AppId = app.Id, Name = name });

            return RedirectToAction("Scoreboard", new { appid });
        }

        [HttpGet]
        [Route("/Developer/Objective/{appid}/{objective}")]
        public async Task<IActionResult> DeleteObjective(string appid, string objective)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _authorizationService.AuthorizeAsync(User, app, "ApplicationPolicy")).Succeeded)
            {
                return RedirectToAction("Index");
            }

            await _scoreboardProcessor.DeleteObjective(objective);

            return RedirectToAction("Scoreboard", new { appid });
        }

        [Route("/Developer/Delete/{appid}")]
        public async Task<IActionResult> Delete(string appid)
        {
            Application app = _mapper.Map<Application>(await _applicationProcessor.GetApplicationById(appid));

            if (app == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(app);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Developer/Delete/{appid}")]
        public async Task<IActionResult> ConfirmDelete(string appid)
        {
            await _applicationProcessor.DeleteApplication(appid);

            return RedirectToAction("Index");
        }
    }
}