![InfoLog](https://img.shields.io/badge/InfoLog-1.0.2-orange)
![MailKit](https://img.shields.io/badge/MailKit-3.1.1-yellow)
![Newtonsoft.Json](https://img.shields.io/badge/Newtonsoft.Json-13.0.1-orange)
![Microsoft.EntityFrameworkCore](https://img.shields.io/badge/Microsoft.EntityFrameworkCore-6.0.0-informational)

# AuthenticationForm

## Description

Web application for sending "confirmation" emails. 
Surely on different sites you are faced with a request to confirm registration by e-mail or when you need to reset your password.
This application-service just provides such an opportunity.

## Connection

All functions use three endpoints. One is to verify that the site accessing this web application is trustworthy. 
Authentication is based on JWTToken. The second endpoint is needed to generate the email. It is enough 
to indicate in the header the email address to which you want to send the letter and the address to 
which information about confirmation or non-confirmation will be sent.

## Practical use

The main feature is the independence of this application from who sends the request and who receives it. 
It is possible to use this service for a variety of purposes, you just need to change the form of the sent email.
If you need to fine-tune the application to your needs, just write new 
methods in the controller and add more email design options (or send this design in the request body).
