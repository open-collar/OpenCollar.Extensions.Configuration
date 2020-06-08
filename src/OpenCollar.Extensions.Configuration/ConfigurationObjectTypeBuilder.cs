using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     A class used to build implementations of <see cref="IConfigurationObject" /> interfaces.
    /// </summary>
    /// <remarks>
    ///     The following UML has been generated directly from the source code using
    ///     <a href="https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml"> Jebbs PlantUML </a>. <img src="../images/uml-diagrams/ConfigurationObjectTypeBuilder/ConfigurationObjectTypeBuilder.svg" />
    /// </remarks>
    internal class ConfigurationObjectTypeBuilder
    {
        /// <summary>
        ///     A cache of generated collection converter types, keyed on the element type.
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type> _collectionConverters = new System.Collections.Concurrent.ConcurrentDictionary<Type, Type>();

        /// <summary>
        ///     A cache of generated dictionary converter types, keyed on the element type.
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type> _dictionaryConverters = new System.Collections.Concurrent.ConcurrentDictionary<Type, Type>();

        /// <summary>
        ///     The type of the <see cref="ConfigurationObjectBase{T}" /> class.
        /// </summary>
        private readonly Type _baseClassType;

        /// <summary>
        ///     The type of the interface to implement.
        /// </summary>
        private readonly Type _interfaceType;

        /// <summary>
        ///     The definitions of the interface's properties.
        /// </summary>
        private readonly PropertyDef[] _propertyDefs;

        /// <summary>
        ///     The settings used to control how configuration objects are created and the features they support.
        /// </summary>
        private readonly ConfigurationObjectSettings _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectTypeBuilder" /> class.
        /// </summary>
        /// <param name="interfaceType">
        ///     The type of the interface.
        /// </param>
        /// <param name="propertyDefs">
        ///     The definitions of the interface's properties..
        /// </param>
        /// <param name="settings">
        ///     Optional settings used to control how configuration objects are created and the features they support.
        /// </param>
        public ConfigurationObjectTypeBuilder(Type interfaceType, IEnumerable<PropertyDef> propertyDefs, ConfigurationObjectSettings settings)
        {
            // TODO: Make type builders cacheable.

            _interfaceType = interfaceType;
            _propertyDefs = propertyDefs.ToArray();
            _baseClassType = typeof(ConfigurationObjectBase<>).MakeGenericType(_interfaceType);
            _settings = settings;
            settings.ConfigureEnvironment();
        }

        /// <summary>
        ///     Generates this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        public Type Generate()
        {
            var indexerDef = _baseClassType.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Debug.Assert(!ReferenceEquals(indexerDef, null));

            var builder = GetTypeBuilder();

            builder.AddInterfaceImplementation(_interfaceType);

            // Debugger display attribute to aid debugging.
            var debuggerDisplayString = "\\{" + _interfaceType.Name + "\\}: {DisplayPath}";
            var debuggerDisplayAttributeConstructor = typeof(DebuggerDisplayAttribute).GetConstructor(new[] { typeof(string) });
            var debuggerDisplayAttributeBuilder = new CustomAttributeBuilder(debuggerDisplayAttributeConstructor, new object[] { debuggerDisplayString });
            builder.SetCustomAttribute(debuggerDisplayAttributeBuilder);

            // Debugger step through to make the class behave more like a POCO
            var debuggerStepThroughAttributeConstructor = typeof(DebuggerStepThroughAttribute).GetConstructor(Array.Empty<Type>());
            var debuggerStepThroughAttributeBuilder = new CustomAttributeBuilder(debuggerStepThroughAttributeConstructor, Array.Empty<object>());
            builder.SetCustomAttribute(debuggerStepThroughAttributeBuilder);

            AddConstructor(builder);

            foreach(var propertyDef in _propertyDefs)
            {
                AddProperty(builder, propertyDef, indexerDef);
            }

            var objectTypeInfo = builder.CreateTypeInfo();

            return objectTypeInfo;
        }

        /// <summary>
        ///     Writes the code implementing a property getter.
        /// </summary>
        /// <param name="propertyDef">
        ///     The definition of the property for which to generate code.
        /// </param>
        /// <param name="indexerDef">
        ///     The definition of the indexer in the base class that represents the property.
        /// </param>
        /// <param name="propertyBuilder">
        ///     The property builder with which to generate code.
        /// </param>
        /// <param name="getMethodBuilder">
        ///     The get method builder.
        /// </param>
        private static void AddPropertyGet(PropertyDef propertyDef, PropertyInfo indexerDef, PropertyBuilder propertyBuilder, MethodBuilder getMethodBuilder)
        {
            var ilGenerator = getMethodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldstr, propertyDef.PropertyName);
            ilGenerator.EmitCall(OpCodes.Call, indexerDef.GetMethod, new[] { typeof(string) });
            if(propertyDef.Type != typeof(object))
            {
                ilGenerator.Emit(propertyDef.Type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, propertyDef.Type);
            }

            ilGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);
        }

        private static Type GetCollectionConverter(Type type)
        {
            return _collectionConverters.GetOrAdd(type, t => typeof(OpenCollar.Extensions.Configuration.Converters.Newtonsoft.Json.ConfigurationCollectionConverter<>).MakeGenericType(new Type[] { t }));
        }

        private static Type GetDictionaryConverter(Type type)
        {
            return _dictionaryConverters.GetOrAdd(type, t => typeof(OpenCollar.Extensions.Configuration.Converters.Newtonsoft.Json.ConfigurationDictionaryConverter<>).MakeGenericType(new Type[] { t }));
        }

        /// <summary>
        ///     Writes the code implementing a property setter.
        /// </summary>
        /// <param name="builder">
        ///     The type builder into which to add generated code.
        /// </param>
        /// <param name="propertyDef">
        ///     The definition of the property for which to generate code.
        /// </param>
        /// <param name="indexerDef">
        ///     The definition of the indexer in the base class that represents the property.
        /// </param>
        /// <param name="propertyBuilder">
        ///     The property builder with which to generate code.
        /// </param>
        private static void WritePropertySet(TypeBuilder builder, PropertyDef propertyDef, PropertyInfo indexerDef, PropertyBuilder propertyBuilder)
        {
            var setMethodBuilder = builder.DefineMethod("set_" + propertyDef.PropertyName,
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new[] { propertyDef.Type });

            var ilGenerator = setMethodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldstr, propertyDef.PropertyName);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            if(propertyDef.Type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, propertyDef.Type);
            }

            ilGenerator.EmitCall(OpCodes.Call, indexerDef.SetMethod, new[] { typeof(string) });
            ilGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        /// <summary>
        ///     Adds the constructor for the generated class.
        /// </summary>
        /// <param name="builder">
        ///     The type builder into which to add the generated code.
        /// </param>
        private void AddConstructor(TypeBuilder builder)
        {
            var constructorArgumentTypes = new[] { typeof(IPropertyDef), typeof(IConfigurationRoot), typeof(IConfigurationParent), typeof(ConfigurationObjectSettings) };

            var baseConstructor = _baseClassType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
            constructorArgumentTypes, null);

            Debug.Assert(!ReferenceEquals(baseConstructor, null));

            var constructorBuilder =
            builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
            CallingConventions.Standard, constructorArgumentTypes);

            var generator = constructorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg, 0);
            generator.Emit(OpCodes.Ldarg, 1);
            generator.Emit(OpCodes.Ldarg, 2);
            generator.Emit(OpCodes.Ldarg, 3);
            generator.Emit(OpCodes.Ldarg, 4);
            generator.Emit(OpCodes.Call, baseConstructor);
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>
        ///     Adds the property.
        /// </summary>
        /// <param name="builder">
        ///     The type builder into which to add generated code.
        /// </param>
        /// <param name="propertyDef">
        ///     The definition of the property for which to generate code.
        /// </param>
        /// <param name="indexerDef">
        ///     The definition of the indexer in the base class that represents the property.
        /// </param>
        private void AddProperty(TypeBuilder builder, PropertyDef propertyDef, PropertyInfo indexerDef)
        {
            var propertyBuilder = builder.DefineProperty(propertyDef.PropertyName, PropertyAttributes.HasDefault, propertyDef.Type, null);

            // PropertyName attribute to aid JSON serialization. [System.Text.Json.Serialization.JsonPropertyNameAttribute(typeof(Converters.ConfigurationDictionaryConverter<TElement>))]
            var propertyNameAttributeConstructor = typeof(System.Text.Json.Serialization.JsonPropertyNameAttribute).GetConstructor(new[] { typeof(string) });
            var propertyNameAttributeBuilder = new CustomAttributeBuilder(propertyNameAttributeConstructor, new object[] { propertyDef.PathSection });
            propertyBuilder.SetCustomAttribute(propertyNameAttributeBuilder);

            if(_settings.EnableNewtonSoftJsonSupport)
            {
                // Newtonsoft JsonObject attribute (used to identify properties to be serialized by the Newtonsoft.Json
                // serializer when that is being used). NB. We are not directly referencing the assembly, but instead
                // loading it dynamically, to avoid creating a dependency for code that does not use the Newtonsoft.Json assemblies.

                // [Newtonsoft.Json.Serialization.JsonPropertyAttribute("<property-name>")]
                var newtonSoftJsonPropertyAttributeType = ConfigurationObjectSettings.NewtonSoftJsonPropertyAttributeType;
                var newtonSoftJsonPropertyAttributeConstructor = newtonSoftJsonPropertyAttributeType.GetConstructor(new[] { typeof(string) });
                var newtonSoftJsonPropertyAttributeBuilder = new CustomAttributeBuilder(newtonSoftJsonPropertyAttributeConstructor, new object[] { propertyDef.PathSection });
                propertyBuilder.SetCustomAttribute(newtonSoftJsonPropertyAttributeBuilder);
            }

            System.Reflection.ConstructorInfo converterAttributeConstructor;
            Type converterType;
            CustomAttributeBuilder converterAttributeBuilder;
            Type newtonSoftConverterAttribute;

            switch(propertyDef.Implementation.ImplementationKind)
            {
                case ImplementationKind.ConfigurationCollection:

                    if(_settings.EnableNewtonSoftJsonSupport)
                    {
                        newtonSoftConverterAttribute = ConfigurationObjectSettings.NewtonSoftJsonConverterAttributeType;
                        converterAttributeConstructor = newtonSoftConverterAttribute.GetConstructor(new[] { typeof(Type) });
                        converterType = GetCollectionConverter(propertyDef.Implementation.Type);
                        converterAttributeBuilder = new CustomAttributeBuilder(converterAttributeConstructor, new object[] { converterType });
                        propertyBuilder.SetCustomAttribute(converterAttributeBuilder);
                    }
                    break;

                case ImplementationKind.ConfigurationDictionary:

                    // [System.Text.Json.Serialization.JsonConverterAttribute(typeof(Converters.ConfigurationDictionaryConverter<TElement>))]
                    // Converter attribute to aid JSON serialization.
                    converterAttributeConstructor = typeof(System.Text.Json.Serialization.JsonConverterAttribute).GetConstructor(new[] { typeof(Type) });
                    converterType = typeof(Converters.Text.Json.ConfigurationDictionaryConverter<>).MakeGenericType(new[] { propertyDef.Implementation.Type });
                    converterAttributeBuilder = new CustomAttributeBuilder(converterAttributeConstructor, new object[] { converterType });
                    propertyBuilder.SetCustomAttribute(converterAttributeBuilder);

                    if(_settings.EnableNewtonSoftJsonSupport)
                    {
                        newtonSoftConverterAttribute = ConfigurationObjectSettings.NewtonSoftJsonConverterAttributeType;
                        converterAttributeConstructor = newtonSoftConverterAttribute.GetConstructor(new[] { typeof(Type) });
                        converterType = GetDictionaryConverter(propertyDef.Implementation.Type);
                        converterAttributeBuilder = new CustomAttributeBuilder(converterAttributeConstructor, new object[] { converterType });
                        propertyBuilder.SetCustomAttribute(converterAttributeBuilder);
                    }
                    break;
            }

            var getMethodBuilder = builder.DefineMethod("get_" + propertyDef.PropertyName,
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, propertyDef.Type, Type.EmptyTypes);

            AddPropertyGet(propertyDef, indexerDef, propertyBuilder, getMethodBuilder);

            if(!propertyDef.IsReadOnly)
            {
                WritePropertySet(builder, propertyDef, indexerDef, propertyBuilder);
            }
        }

        /// <summary>
        ///     Gets the type builder.
        /// </summary>
        /// <returns>
        /// </returns>
        private TypeBuilder GetTypeBuilder()
        {
            var name = _interfaceType.Name;
            if(name.StartsWith("I", StringComparison.Ordinal))
            {
                name = name.Substring(1);
            }

            var typeName = name + "Impl";
            var an = new AssemblyName("OpenCollar.Extensions.Configuration.Implementations." + name);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            return moduleBuilder.DefineType(typeName,
            TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout, _baseClassType);
        }
    }
}