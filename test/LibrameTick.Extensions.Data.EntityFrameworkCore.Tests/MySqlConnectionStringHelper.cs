﻿using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;

namespace Librame.Extensions.Data
{
    static class MySqlConnectionStringHelper
    {

        public static string Validate(string connectionString, out ServerVersion version)
        {
            var csb = new MySqlConnectionStringBuilder(connectionString);
            if (csb.AllowUserVariables != true || csb.UseAffectedRows)
            {
                try
                {
                    csb.AllowUserVariables = true;
                    csb.UseAffectedRows = false;
                }
                catch (MySqlException e)
                {
                    throw new InvalidOperationException("The MySql Connection string used with Pomelo.EntityFrameworkCore.MySql " +
                        "must contain \"AllowUserVariables=true;UseAffectedRows=false\"", e);
                }
            }

            version = ServerVersion.AutoDetect(connectionString);
            return csb.ConnectionString;
        }

    }
}
