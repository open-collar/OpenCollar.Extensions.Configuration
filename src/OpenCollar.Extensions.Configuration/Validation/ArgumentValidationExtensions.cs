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
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

using JetBrains.Annotations;

using OpenCollar.Extensions.Configuration.Resources;

namespace OpenCollar.Extensions.Configuration.Validation
{
    /// <summary>
    ///     Extension methods used in the validation of arguments.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/Validation/ArgumentValidationExtensions/ArgumentValidationExtensions.svg" />
    /// </remarks>
    internal static class ArgumentValidationExtensions
    {
        /// <summary>
        ///     Validates the specified argument value is a member of the enumeration type specified.
        /// </summary>
        /// <typeparam name="TEnum"> The type of the enumeration to which the argument must belong. </typeparam>
        /// <param name="argumentValue"> The value of the argument to validate. </param>
        /// <param name="argumentName"> The name of the argument to validate. </param>
        /// <param name="validation"> The type of validation to perform. </param>
        /// <exception cref="ArgumentException"> <paramref name="argumentValue" /> must be an of an enum type </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The value provided in <paramref name="validation" /> was not a valid member of the <see cref="EnumIs" /> enum.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The value provided in <paramref name="argumentValue" /> was not a valid member of the enum.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The value provided in <paramref name="argumentValue" /> was zero.
        /// </exception>
        [AssertionMethod]
        [ContractArgumentValidator]
        public static void Validate<TEnum>(this TEnum argumentValue, [NotNull] [InvokerParameterName] string argumentName, EnumIs validation)
        where TEnum : struct
        {
            var type = typeof(TEnum);

            if(!type.IsEnum)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_EnumValueNotEnum, nameof(argumentValue)),
                nameof(argumentValue));
            }

            if(validation == EnumIs.None)
            {
                return;
            }

            if(!Enum.IsDefined(typeof(EnumIs), validation))
            {
                throw new ArgumentOutOfRangeException(nameof(validation), validation,
                string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ValidationKindWrong, nameof(validation), nameof(EnumIs)));
            }

            if(validation.HasFlag(EnumIs.ValidMember) && !Enum.IsDefined(type, argumentValue))
            {
                throw new ArgumentOutOfRangeException(nameof(validation), validation,
                string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_EnumValueNotMember, argumentName, type.Namespace, type.Name));
            }

            if(validation.HasFlag(EnumIs.NonZero))
            {
                var value = Convert.ToInt32(argumentValue, CultureInfo.InvariantCulture);

                if(value == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(validation), validation,
                    string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ValueZero, argumentName));
                }
            }

            Contract.EndContractBlock();
        }

        /// <summary>
        ///     Validates the value of the specified argument according to the rules defined in
        ///     <paramref name="validation" /> .
        /// </summary>
        /// <param name="argumentValue"> The value of the argument to validate. </param>
        /// <param name="argumentName"> The name of the argument to validate (used in error messages). </param>
        /// <param name="validation"> The type of validation to perform. </param>
        /// <exception cref="ArgumentNullException"> The argument is <see langword="null" />. </exception>
        /// <exception cref="ArgumentException"> The argument is zero-length. </exception>
        /// <exception cref="ArgumentException"> The argument contains only white space characters. </exception>
        [AssertionMethod]
        [ContractArgumentValidator]
        [ContractAbbreviator]
        public static void Validate([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [ValidatedNotNull]
        this string argumentValue, [NotNull] [InvokerParameterName] string argumentName, StringIs validation)
        {
            /*
             * We should test "validation", but that would be an expensive thing to do for something that could be
             * called very frequently.
             */
            var isNull = ReferenceEquals(argumentValue, null);
            if(validation.HasFlag(StringIs.NotNull) && isNull)
            {
                var normalizedArgumentName = string.IsNullOrWhiteSpace(argumentName) ? Exceptions.Fragment_UnknownArgument : argumentName;
                throw new ArgumentNullException(normalizedArgumentName,
                string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ArgumentIsNull, normalizedArgumentName));
            }

            if(validation.HasFlag(StringIs.NotEmpty) && !isNull && argumentValue.Length <= 0)
            {
                var normalizedArgumentName = string.IsNullOrWhiteSpace(argumentName) ? Exceptions.Fragment_UnknownArgument : argumentName;
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ArgumentIsZeroLength, normalizedArgumentName),
                normalizedArgumentName);
            }

#pragma warning disable S2583 // Conditionally executed blocks should be reachable
            if(validation.HasFlag(StringIs.NotWhiteSpace) && !isNull && argumentValue.Length > 0 && string.IsNullOrWhiteSpace(argumentValue))
#pragma warning restore S2583 // Conditionally executed blocks should be reachable
            {
                var normalizedArgumentName = string.IsNullOrWhiteSpace(argumentName) ? Exceptions.Fragment_UnknownArgument : argumentName;
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ArgumentIsWhiteSpaceOnly, normalizedArgumentName),
                normalizedArgumentName);
            }

            Contract.EndContractBlock();
        }

        /// <summary>
        ///     Validates the value of the specified argument according to the rules defined in
        ///     <paramref name="validation" /> .
        /// </summary>
        /// <typeparam name="T"> The type of the value. </typeparam>
        /// <param name="argumentValue"> The value of the argument to validate. </param>
        /// <param name="argumentName"> The name of the argument to validate (used in error messages). </param>
        /// <param name="validation"> The type of validation to perform. </param>
        /// <exception cref="ArgumentNullException"> The argument is <see langword="null" />. </exception>
        [AssertionMethod]
        [ContractArgumentValidator]
        [ContractAbbreviator]
        public static void Validate<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NoEnumeration] [ValidatedNotNull]
        this T argumentValue, [NotNull] [InvokerParameterName] string argumentName, ObjectIs validation) where T : class
        {
            /*
             * We should test "validation", but that would be an expensive thing to do for something that could be
             * called very frequently.
             */
            var isNull = ReferenceEquals(argumentValue, null);
            if(validation.HasFlag(ObjectIs.NotNull) && isNull)
            {
                var normalizedArgumentName = string.IsNullOrWhiteSpace(argumentName) ? Exceptions.Fragment_UnknownArgument : argumentName;
                throw new ArgumentNullException(normalizedArgumentName,
                string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ArgumentIsNull, normalizedArgumentName));
            }

            Contract.EndContractBlock();
        }

        /// <summary>
        ///     Validates the value of the specified enumerable argument according to the rules defined in
        ///     <paramref name="argumentValue" /> and <paramref name="elementValidation" /> and returns containing the
        ///     validated argument as an array.
        /// </summary>
        /// <typeparam name="T"> The type of the elements in the argument. </typeparam>
        /// <param name="argumentValue"> The value of the argument to validate. </param>
        /// <param name="argumentName"> The name of the argument to validate (used in error messages). </param>
        /// <param name="argumentValidation"> The type of validation to perform on the argument. </param>
        /// <param name="elementValidation">
        ///     The type of validation to perform on the elements contained in the argument sequence.
        /// </param>
        /// <returns> The contents of the argument, in an array. </returns>
        /// <exception cref="ArgumentNullException"> The argument is <see langword="null" />. </exception>
        /// <exception cref="ArgumentException"> An element in the argument is <see langword="null" />. </exception>
        [AssertionMethod]
        [ContractArgumentValidator]
        [ContractAbbreviator]
        public static T[] Validate<T>([ItemCanBeNull] [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [ValidatedNotNull]
        this IEnumerable<T> argumentValue, [NotNull] [InvokerParameterName] string argumentName, ObjectIs argumentValidation, ObjectIs elementValidation)
        where T : class
        {
            /*
             * We should test "validation", but that would be an expensive thing to do for something that could be
             * called very frequently.
             */
            var isNull = ReferenceEquals(argumentValue, null);
            if(isNull)
            {
                if(argumentValidation.HasFlag(ObjectIs.NotNull))
                {
                    var normalizedArgumentName = string.IsNullOrWhiteSpace(argumentName) ? Exceptions.Fragment_UnknownArgument : argumentName;
                    throw new ArgumentNullException(normalizedArgumentName,
                    string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ArgumentIsNull, normalizedArgumentName));
                }

                return Array.Empty<T>();
            }

            var checkElements = elementValidation.HasFlag(ObjectIs.NotNull);
            var index = 0;
            var elements = new List<T>();

            foreach(var element in argumentValue)
            {
                if(checkElements && ReferenceEquals(element, null))
                {
                    var normalizedArgumentName = string.Concat(string.IsNullOrWhiteSpace(argumentName) ? Exceptions.Fragment_UnknownArgument : argumentName,
                    "[", index.ToString(@"D", CultureInfo.InvariantCulture), @"]");
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Exceptions.Validate_ArgumentIsNull, normalizedArgumentName),
                    normalizedArgumentName);
                }

                ++index;

                elements.Add(element);
            }

            Contract.EndContractBlock();

            return elements.ToArray();
        }
    }
}