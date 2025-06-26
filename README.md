<<<<<<< HEAD
# Installation Guide

* Stap 1. Clone de repository
* Stap 2. Navigeer naar "cnd/cnd/"
* Stap 3. Execute command "docker-compose up"


# CND-architectuur en ontwerpaantekeningen
# Bibliotheken:

* AutoMapper: vereenvoudigt het mappen tussen DTO's en entiteiten.

* BCrypt.Net-Next: biedt veilige wachtwoord-hashing.

* Microsoft.AspNet.Identity.Core en Microsoft.Extensions.Identity.Core: basis voor ASP.NET Identity-functionaliteiten.

* Microsoft.AspNetCore.Authentication.JwtBearer: ondersteunt JWT-gebaseerde authenticatie.

* Microsoft.EntityFrameworkCore met Npgsql provider en Design tools: maakt database-toegang mogelijk via ORM, ondersteunt migraties en PostgreSQL.

* Microsoft.EntityFrameworkCore.InMemory: wordt gebruikt voor unittests zonder externe database.

* Swashbuckle.AspNetCore: genereert Swagger/OpenAPI-documentatie voor eenvoudigere ontdekking van de API.

* System.Text.Json: ingebouwde, hoog-performante JSON-serialisatie.

# Ontwerpprincipes
* Dependency Injection (Afhankelijkheidsinjectie): services worden geregistreerd en geïnjecteerd via de ingebouwde container voor losse koppeling en testbaarheid.

* Middleware: cross-cutting concerns zoals foutafhandeling worden geïsoleerd in aangepaste middleware.

* Service Layer (Servicelaag): businesslogica is georganiseerd in serviceklassen (bijv. AuthService, JournalEntryService) achter interfaces.

* DTO’s (Data Transfer Objects): modellen die worden gebruikt om data tussen de API en clients te verzenden, waarbij de persistente modellen verborgen blijven.
=======

*CND-architectuur en ontwerpaantekeningen*
*Bibliotheken*
*AutoMapper:* vereenvoudigt het mappen tussen DTO's en entiteiten.

*BCrypt.Net-Next:* biedt veilige wachtwoord-hashing.

*Microsoft.AspNet.Identity.Core en Microsoft.Extensions.Identity.Core:* basis voor ASP.NET Identity-functionaliteiten.

*Microsoft.AspNetCore.Authentication.JwtBearer:* ondersteunt JWT-gebaseerde authenticatie.

*Microsoft.EntityFrameworkCore met Npgsql provider en Design tools:* maakt database-toegang mogelijk via ORM, ondersteunt migraties en PostgreSQL.

*Microsoft.EntityFrameworkCore.InMemory:* wordt gebruikt voor unittests zonder externe database.

*Swashbuckle.AspNetCore:* genereert Swagger/OpenAPI-documentatie voor eenvoudigere ontdekking van de API.

*System.Text.Json:* ingebouwde, hoog-performante JSON-serialisatie.

*Ontwerpprincipes*
*Dependency Injection (Afhankelijkheidsinjectie):* services worden geregistreerd en geïnjecteerd via de ingebouwde container voor losse koppeling en testbaarheid.

*Middleware:* cross-cutting concerns zoals foutafhandeling worden geïsoleerd in aangepaste middleware.

*Service Layer (Servicelaag):* businesslogica is georganiseerd in serviceklassen (bijv. AuthService, JournalEntryService) achter interfaces.

*Controllers:* http request handler voor het ontvangen en terug geven van informatie aan de gebruiker, zonder hierbij businesslogica uit te voeren.

*DTO’s (Data Transfer Objects):* modellen die worden gebruikt om data tussen de API en clients te verzenden, waarbij de persistente modellen verborgen blijven.
>>>>>>> origin/main
