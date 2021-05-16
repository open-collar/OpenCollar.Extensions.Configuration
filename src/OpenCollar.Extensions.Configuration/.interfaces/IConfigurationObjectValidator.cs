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
 * Copyright © 2019-2021 Jonathan Evans (jevans@open-collar.org.uk).
 */

namespace OpenCollar.Extensions.Configuration
{
    public interface IConfigurationObjectValidator
    {
    }

    /// <summary>
    ///     The public interface of a class used to validate the values loaded into a configuration object.
    /// </summary>
    /// <typeparam name="TConfigurationObject">
    ///     The type of the configuration object validated by the implemented class.
    /// </typeparam>
    /// <remarks>
    ///     Implementations must be registered with the
    ///     <see cref="Microsoft.Extensions.DependencyInjection.ServiceProvider" /> using the
    /// </remarks>
    public interface IConfigurationObjectValidator<TConfigurationObject> : IConfigurationObjectValidator where TConfigurationObject : IConfigurationObject
    {
        /// <summary>
        ///     Validates the specified configuration object.
        /// </summary>
        /// <param name="configurationObject">
        ///     The configuration object to be validated.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Implementations must be registered with the
        ///         <see cref="Microsoft.Extensions.DependencyInjection.ServiceProvider" /> or have a default constructor.
        ///     </para>
        ///     <para>
        ///         The <see cref="Validate" /> method is called whenever a batch of changes has been completed. Any
        ///         problems should be indicated by throwing exceptions.
        ///     </para>
        ///     "/&gt;
        ///     <para>
        ///         If a nested configuration object has a validator that will be called before the parent object's validator.
        ///     </para>
        /// </remarks>
        public void Validate(TConfigurationObject configurationObject);
    }
}