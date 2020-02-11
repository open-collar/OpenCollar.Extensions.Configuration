using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>A class used to build implementations of <see cref="IConfigurationObject"/> interfaces.</summary>
    internal class ConfigurationObjectTypeBuilder
    {
        /// <summary>The type of the <see cref="ConfigurationObjectBase{T}"/> class.</summary>
        private readonly Type _baseClassType;

        /// <summary>The type of the interface to implement.</summary>
        private readonly Type _interfaceType;

        /// <summary>The definitions of the interface's properties.</summary>
        private readonly PropertyDef[] _propertyDefs;

        /// <summary>Initializes a new instance of the <see cref="ConfigurationObjectTypeBuilder"/> class.</summary>
        /// <param name="interfaceType"> The type of the interface. </param>
        /// <param name="propertyDefs"> The definitions of the interface's properties.. </param>
        public ConfigurationObjectTypeBuilder(Type interfaceType, IEnumerable<PropertyDef> propertyDefs)
        {
            // TODO: Make type builders cacheable.

            _interfaceType = interfaceType;
            _propertyDefs = propertyDefs.ToArray();
            _baseClassType = typeof(ConfigurationObjectBase<>).MakeGenericType(_interfaceType);
        }

        /// <summary>Generates this instance.</summary>
        /// <returns> </returns>
        public Type Generate()
        {
            var indexerDef = _baseClassType.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Debug.Assert(!ReferenceEquals(indexerDef, null));

            var builder = GetTypeBuilder();

            builder.AddInterfaceImplementation(_interfaceType);

            AddConstructor(builder);

            foreach(var propertyDef in _propertyDefs)
            {
                AddProperty(builder, propertyDef, indexerDef);
            }

            var objectTypeInfo = builder.CreateTypeInfo();

            return objectTypeInfo;
        }

        /// <summary>Adds the constructor.</summary>
        /// <param name="builder"> The builder. </param>
        private void AddConstructor(TypeBuilder builder)
        {
            var constructorArgumentTypes = new[] { typeof(IConfigurationRoot), typeof(IConfigurationParent) };

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
            generator.Emit(OpCodes.Call, baseConstructor);
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>Adds the property.</summary>
        /// <param name="builder"> The builder. </param>
        /// <param name="propertyDef"> The property definition. </param>
        /// <param name="indexerDef"> The indexer definition. </param>
        private void AddProperty(TypeBuilder builder, PropertyDef propertyDef, PropertyInfo indexerDef)
        {
            var propertyBuilder = builder.DefineProperty(propertyDef.PropertyName, PropertyAttributes.HasDefault, propertyDef.Type, null);
            var getMethodBuilder = builder.DefineMethod("get_" + propertyDef.PropertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, propertyDef.Type, Type.EmptyTypes);
            var getIl = getMethodBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldstr, propertyDef.PropertyName);
            getIl.EmitCall(OpCodes.Call, indexerDef.GetMethod, new[] { typeof(string) });
            if(propertyDef.Type != typeof(object))
            {
                getIl.Emit(propertyDef.Type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, propertyDef.Type);
            }

            getIl.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            if(!propertyDef.IsReadOnly)
            {
                var setMethodBuilder = builder.DefineMethod("set_" + propertyDef.PropertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new[] { propertyDef.Type });
                var setIl = setMethodBuilder.GetILGenerator();

                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldstr, propertyDef.PropertyName);
                setIl.Emit(OpCodes.Ldarg_1);
                if(propertyDef.Type.IsValueType)
                {
                    setIl.Emit(OpCodes.Box, propertyDef.Type);
                }

                setIl.EmitCall(OpCodes.Call, indexerDef.SetMethod, new[] { typeof(string) });
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }

        /// <summary>Gets the type builder.</summary>
        /// <returns> </returns>
        private TypeBuilder GetTypeBuilder()
        {
            var name = _interfaceType.Name;
            if(name.StartsWith("I", StringComparison.Ordinal))
            {
                name = name.Substring(1);
            }

            var typeName = name + "Impl";
            var an = new AssemblyName(typeName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            return moduleBuilder.DefineType(typeName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout, _baseClassType);
        }
    }
}