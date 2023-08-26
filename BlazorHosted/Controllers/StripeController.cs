using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using BlazorHosted;
using BlazorHosted.Data;
using Lib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [HttpPost("get-payment-history")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult GetPaymentHistory([FromBody] BlazorCustomer customerData)
    {
        var domain = _appSecrets.BaseAddress;
        var customerService = new CustomerService();

        // Define the search options with the email
        var listOptions = new CustomerListOptions
        {
            Email = customerData.email,
            Limit = 1, // You can limit the number of results, if needed
        };
        string customerId;

        // Retrieve customers matching the email
        StripeList<Customer> customers = customerService.List(listOptions);
        // If a matching customer is found, retrieve the customer ID
        if (customers.Data.Count > 0)
        {
            customerId = customers.Data[0].Id;
            var chargeService = new ChargeService();
            var chargeListOptions = new ChargeListOptions
            {
                Customer = customerId,
                Limit = 100, // You can adjust the limit as needed
            };

            StripeList<Charge> charges = chargeService.List(chargeListOptions);

            long totalAmountPaid = charges.Sum(charge => charge.Amount);

            BlazorCustomer result = new BlazorCustomer{
                email = customerData.email,
                total = totalAmountPaid,
            };
            return Ok(result);
        }
        else
        {
            return BadRequest("No Customer Found");
        }
        
    }

    [HttpPost("create-payment-session")]
    public ActionResult Create([FromBody] BlazorCustomer customerData)
    {
        var domain = _appSecrets.BaseAddress;
        var customerService = new CustomerService();

        // Define the search options with the email
        var listOptions = new CustomerListOptions
        {
            Email = customerData.email,
            Limit = 1, // You can limit the number of results, if needed
        };
        string customerId;

        // Retrieve customers matching the email
        StripeList<Customer> customers = customerService.List(listOptions);
        // If a matching customer is found, retrieve the customer ID
        if (customers.Data.Count > 0)
        {
            customerId = customers.Data[0].Id;
            // Use the customerId for further operations
        }
        else
        {
            // Create a new customer with the targetEmail
            var customerOptions = new CustomerCreateOptions
            {
                Email = customerData.email,
            };
            customerId = customerService.Create(customerOptions).Id;
        }
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "us_bank_account" },
            PaymentMethodOptions = new SessionPaymentMethodOptionsOptions
            {
                UsBankAccount = new SessionPaymentMethodOptionsUsBankAccountOptions
                {
                    FinancialConnections = new SessionPaymentMethodOptionsUsBankAccountFinancialConnectionsOptions
                    {
                        Permissions = new List<string> { "payment_method" },
                    },
                },
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    // Provide the exact Price ID (for example, price_123) of the product you want to sell
                    Price = _appSecrets.PriceId,
                    Quantity = 1,
                },
            },
            SuccessUrl = "https://plhhoa.link/posts/payments", //domain + "/success",
            CancelUrl = "https://plhhoa.link/posts/payments", //domain + "/cancel",
            CustomFields = new List<SessionCustomFieldOptions>
            {
                new SessionCustomFieldOptions
                {
                    Key = "housenumber",
                    Label = new SessionCustomFieldLabelOptions
                    {
                        Type = "custom",
                        Custom = "House Number",
                    },
                    Type = "numeric",
                    Numeric = new SessionCustomFieldNumericOptions{
                        MinimumLength = 4,
                        MaximumLength = 5
                    }
                },
            },
        };
        var service = new SessionService();
        Session session = service.Create(options);

        // Return the redirect URL as JSON response
        var response = new { RedirectUrl = session.Url };
        return Ok(response);
    }
}