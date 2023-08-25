using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using BlazorHosted;
using Lib;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly AppSecrets _appSecrets;
    public StripeController(AppSecrets appSecrets)
    {
        _appSecrets = appSecrets;
        StripeConfiguration.ApiKey = _appSecrets.StripeSecretKey;
    }

    [HttpGet("create-checkout-session")]
    public ActionResult Create()
    {
        var domain = _appSecrets.BaseAddress;
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    // Provide the exact Price ID (for example, price_123) of the product you want to sell
                    Price = _appSecrets.PriceId,
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = domain + "/success.html",
            CancelUrl = domain + "/cancel.html",
        };
        var service = new SessionService();
        Session session = service.Create(options);

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }
}