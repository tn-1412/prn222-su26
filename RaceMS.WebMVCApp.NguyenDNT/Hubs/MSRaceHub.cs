using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RaceMS.Entities.NguyenDNT.Models;
using RaceMS_Services.NguyenDNT;

namespace RaceMS.WebMVCApp.NguyenDNT.Hubs
{
    [Authorize]
    public class MSRaceHub : Hub
    {
        private readonly IJockeyNguyenDntService _jockeyNguyenDntService;

        public MSRaceHub(IJockeyNguyenDntService jockeyNguyenDntService)
        {
            _jockeyNguyenDntService = jockeyNguyenDntService;
        }

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
    }
}
