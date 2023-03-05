using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Eportfolio.Models;

public class UserController : Controller
{
    private readonly IConfiguration _config;

    public UserController(IConfiguration config)
    {
        _config = config;
    }
}
