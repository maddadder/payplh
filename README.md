# payplh

Blazor Server Application with JWT Token Authorization

This repository contains a Blazor Server application that includes how to access a protected API endpoint using JWT token-based authorization. The application showcases a scenario where a protected endpoint is accessed within the same application by generating JWT tokens on-the-fly.

Protected Endpoint: The application includes a protected API endpoint (/api/stripe/get-payment-history) that requires authentication using the [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] attribute.

JWT Token Generation: this application demonstrates a custom approach to generate JWT tokens on-the-fly within the application. This is useful for scenarios where immediate token generation is required for accessing protected endpoints within the same application.