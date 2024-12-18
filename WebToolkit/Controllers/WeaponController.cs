﻿using MediawikiTranslator.Generators;
using Microsoft.AspNetCore.Mvc;

namespace WebToolkit.Controllers
{
    public class WeaponController : Controller
    {
        [HttpPost("GenerateWeapon")]
        public string GenerateWeapon(string json)
        {
            try
            {
                return Weapon.GenerateFromJson(json);
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.StatusCode = 500;
                Response.WriteAsync(ex.Message);
                return string.Empty;
            }
        }
        [HttpPost("MassGenerateWeapons")]
        public async Task<string> MassGenerateWeapons(string data, string game)
        {
            try
            {
                return await Weapon.GenerateFromXlsx(data, game);
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.StatusCode = 500;
                Response.WriteAsync(ex.Message).RunSynchronously();
                return string.Empty;
            }
        }
    }
}