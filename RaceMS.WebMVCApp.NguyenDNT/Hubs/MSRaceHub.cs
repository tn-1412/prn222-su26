using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS.WebMVCApp.NguyenDNT.Models;
using RaceMS_Services.NguyenDNT;

namespace RaceMS.WebMVCApp.NguyenDNT.Hubs
{
    [Authorize]
    public class MSRaceHub : Hub
    {
        private readonly IJockeyNguyenDntService _jockeyNguyenDntService;
        private readonly IRegistrationNguyenDntService _registrationNguyenDntService;

        public MSRaceHub(IJockeyNguyenDntService jockeyNguyenDntService, IRegistrationNguyenDntService registrationNguyenDntService)
        {
            _jockeyNguyenDntService = jockeyNguyenDntService;
            _registrationNguyenDntService = registrationNguyenDntService;
        }

        [Authorize(Roles = RoleNames.CanDelete)]
        public async Task HubDelete_JockeyNguyenDnt(int jockeyId)
        {
            try
            {
                await _jockeyNguyenDntService.DeleteAsync(jockeyId);
                await Clients.All.SendAsync("Receiver_DeleteJockeyNguyenDnt", jockeyId);
            }
            catch (InvalidOperationException ex)
            {
                await Clients.Caller.SendAsync("DeleteError", ex.Message);
            }
        }

        [Authorize(Roles = RoleNames.CanWrite)]
        public async Task HubCreate_JockeyNguyenDnt(JockeyNguyenDnt jockey)
        {
            try
            {
                await _jockeyNguyenDntService.CreateAsync(jockey);
                await Clients.All.SendAsync("Receiver_CreateJockeyNguyenDnt", jockey);
            }
            catch (InvalidOperationException ex)
            {
                await Clients.Caller.SendAsync("CreateError", ex.Message);
            }
        }

        [Authorize(Roles = RoleNames.CanWrite)]
        public async Task HubEdit_JockeyNguyenDnt(JockeyNguyenDnt jockey)
        {
            try
            {
                await _jockeyNguyenDntService.UpdateAsync(jockey);
                await Clients.All.SendAsync("Receiver_EditJockeyNguyenDnt", jockey);
            }
            catch (InvalidOperationException ex)
            {
                await Clients.Caller.SendAsync("EditError", ex.Message);
            }
        }

        [Authorize(Roles = RoleNames.CanDelete)]
        public async Task HubDelete_RegistrationNguyenDnt(int registrationId)
        {
            try
            {
                await _registrationNguyenDntService.DeleteAsync(registrationId);
                await Clients.All.SendAsync("Receiver_DeleteRegistrationNguyenDnt", registrationId);
            }
            catch (InvalidOperationException ex)
            {
                await Clients.Caller.SendAsync("DeleteError", ex.Message);
            }
        }

        [Authorize(Roles = RoleNames.CanWrite)]
        public async Task HubCreate_RegistrationNguyenDnt(RegistrationNguyenDnt registration)
        {
            try
            {
                await _registrationNguyenDntService.CreateAsync(registration);
                await Clients.All.SendAsync("Receiver_CreateRegistrationNguyenDnt", registration);
            }
            catch (InvalidOperationException ex)
            {
                await Clients.Caller.SendAsync("CreateError", ex.Message);
            }
        }

        [Authorize(Roles = RoleNames.CanWrite)]
        public async Task HubEdit_RegistrationNguyenDnt(RegistrationNguyenDnt registration)
        {
            try
            {
                await _registrationNguyenDntService.UpdateAsync(registration);
                await Clients.All.SendAsync("Receiver_EditRegistrationNguyenDnt", registration);
            }
            catch (InvalidOperationException ex)
            {
                await Clients.Caller.SendAsync("EditError", ex.Message);
            }
        }
    }
}
