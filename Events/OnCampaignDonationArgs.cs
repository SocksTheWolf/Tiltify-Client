using System;
using Tiltify.Models;

namespace Tiltify.Events
{
    public class OnCampaignDonationArgs : EventArgs
    {
        public WebSocketDonationInformation Donation;
    }
}