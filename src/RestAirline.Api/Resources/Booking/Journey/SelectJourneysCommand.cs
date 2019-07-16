﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RestAirline.Api.Controllers;
using RestAirline.Api.Hypermedia;

namespace RestAirline.Api.Resources.Booking.Journey
{
    public class SelectJourneysCommand : HypermediaCommand<JourneysSelectedResource>
    {
        [Obsolete("For serialization")]
        public SelectJourneysCommand()
        {
            JourneyIds = new List<string>();
        }

        public SelectJourneysCommand(IUrlHelper urlHelper) : base(urlHelper.Link((BookingController c) => c.SelectJourneys(null)))
        {
        }
        
        public List<string> JourneyIds { get; set; }
    }
}