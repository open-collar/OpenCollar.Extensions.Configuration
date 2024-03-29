﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenCollar.Extensions.Configuration.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Exceptions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Exceptions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OpenCollar.Extensions.Configuration.Resources.Exceptions", typeof(Exceptions).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This collection is read-only..
        /// </summary>
        internal static string CollectionIsReadOnly {
            get {
                return ResourceManager.GetString("CollectionIsReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An item with the same key has already been added: {0}..
        /// </summary>
        internal static string DuplicateKey {
            get {
                return ResourceManager.GetString("DuplicateKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more change event handlers threw an exception..
        /// </summary>
        internal static string EventHandlerThrewException {
            get {
                return ResourceManager.GetString("EventHandlerThrewException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Unknown].
        /// </summary>
        internal static string Fragment_UnknownArgument {
            get {
                return ResourceManager.GetString("Fragment_UnknownArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; did not identify a valid element.  Value: {1}..
        /// </summary>
        internal static string KeyNotFound {
            get {
                return ResourceManager.GetString("KeyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to assign value to property &apos;{0}&apos;.  See inner exception for details..
        /// </summary>
        internal static string UnableToAssignValue {
            get {
                return ResourceManager.GetString("UnableToAssignValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is null..
        /// </summary>
        internal static string Validate_ArgumentIsNull {
            get {
                return ResourceManager.GetString("Validate_ArgumentIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; contains only white space characters..
        /// </summary>
        internal static string Validate_ArgumentIsWhiteSpaceOnly {
            get {
                return ResourceManager.GetString("Validate_ArgumentIsWhiteSpaceOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is zero-length..
        /// </summary>
        internal static string Validate_ArgumentIsZeroLength {
            get {
                return ResourceManager.GetString("Validate_ArgumentIsZeroLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not large enough to hold the contents of this collection (if data is copied to the location specified by &apos;{1}&apos;..
        /// </summary>
        internal static string Validate_ArrayTooSmall {
            get {
                return ResourceManager.GetString("Validate_ArrayTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;0&apos; does not contain a valid value..
        /// </summary>
        internal static string Validate_EnumDoesNotContainValidValue {
            get {
                return ResourceManager.GetString("Validate_EnumDoesNotContainValidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must be an of an enum type..
        /// </summary>
        internal static string Validate_EnumValueNotEnum {
            get {
                return ResourceManager.GetString("Validate_EnumValueNotEnum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value provided in &apos;{0}&apos; was not a valid member of the &apos;{1}.{2}&apos; enum..
        /// </summary>
        internal static string Validate_EnumValueNotMember {
            get {
                return ResourceManager.GetString("Validate_EnumValueNotMember", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must be less than or equal to the number of items in the collection..
        /// </summary>
        internal static string Validate_MustBeLessThanOrEqualToCount {
            get {
                return ResourceManager.GetString("Validate_MustBeLessThanOrEqualToCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must be greater than or equal to zero..
        /// </summary>
        internal static string Validate_NumberMustBeGreaterThanEqualZero {
            get {
                return ResourceManager.GetString("Validate_NumberMustBeGreaterThanEqualZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must be at least zero..
        /// </summary>
        internal static string Validate_NumberTooSmall {
            get {
                return ResourceManager.GetString("Validate_NumberTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must contain a valid path or fragment of a path..
        /// </summary>
        internal static string Validate_Path {
            get {
                return ResourceManager.GetString("Validate_Path", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value provided in &apos;{0}&apos; was not a valid member of the &apos;{1}&apos; enum..
        /// </summary>
        internal static string Validate_ValidationKindWrong {
            get {
                return ResourceManager.GetString("Validate_ValidationKindWrong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value provided in &apos;{0}&apos; was zero..
        /// </summary>
        internal static string Validate_ValueZero {
            get {
                return ResourceManager.GetString("Validate_ValueZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This value is read-only..
        /// </summary>
        internal static string ValueIsReadOnly {
            get {
                return ResourceManager.GetString("ValueIsReadOnly", resourceCulture);
            }
        }
    }
}
