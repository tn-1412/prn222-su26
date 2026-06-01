using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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
    }
}
