/*
 * This file is part of OpenCollar.Extensions.Configuration.
 *
 * OpenCollar.Extensions.Configuration is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Configuration is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Configuration.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright � 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ApiDocs
{
    /// <summary>
    ///     The main application class.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Creates the host builder.
        /// </summary>
        /// <param name="args">
        ///     The command line arguments.
        /// </param>
        /// <returns>
        ///     A fully initialized host builder object.
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">
        ///     The command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}