using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Configuration;

namespace OpenCollar.Extensions.Configuration
{
    internal class ConfigurationObjectTypeBuilder
    {
        private readonly Type _baseClassType;

        private readonly Type _interfaceType;

        private readonly PropertyDef[] _propertyDefs;

        public ConfigurationObjectTypeBuilder(Type interfaceType, IEnumerable<PropertyDef> propertyDefs)
        {
            // TODO: Make type builders cacheable.

            _interfaceType = interfaceType;
            _propertyDefs = propertyDefs.ToArray();
            _baseClassType = typeof(ConfigurationObjectBase<>).MakeGenericType(new[] { _interfaceType });
        }

        public Type Generate()
        {
            var indexerDef = _baseClassType.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            System.Diagnostics.Debug.Assert(!ReferenceEquals(indexerDef, null));

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

        private void AddConstructor(TypeBuilder builder)
        {
            var constructorArgumentTypes = new Type[] { typeof(IConfigurationRoot) };

            var baseConstructor = _baseClassType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, constructorArgumentTypes, null);

            System.Diagnostics.Debug.Assert(!ReferenceEquals(baseConstructor, null));

            var constructorBuilder = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, constructorArgumentTypes);

            var generator = constructorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg, 0);
            generator.Emit(OpCodes.Ldarg, 1);
            generator.Emit(OpCodes.Call, baseConstructor);
            generator.Emit(OpCodes.Ret);
        }

        private void AddProperty(TypeBuilder builder, PropertyDef propertyDef, PropertyInfo indexerDef)
        {
            var propertyBuilder = builder.DefineProperty(propertyDef.PropertyName, PropertyAttributes.HasDefault, propertyDef.Type, null);
            var getMethodBuilder = builder.DefineMethod("get_" + propertyDef.PropertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, propertyDef.Type, Type.EmptyTypes);
            var getIl = getMethodBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldstr, propertyDef.PropertyName);
            getIl.EmitCall(OpCodes.Call, indexerDef.GetMethod, new Type[] { typeof(string) });
            if(propertyDef.Type != typeof(object))
            {
                if(propertyDef.Type.IsValueType)
                {
                    getIl.Emit(OpCodes.Unbox_Any, propertyDef.Type);
                }
                else
                {
                    getIl.Emit(OpCodes.Castclass, propertyDef.Type);
                }
            }
            getIl.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            if(!propertyDef.IsReadOnly)
            {
                var setMethodBuilder =
                    builder.DefineMethod("set_" + propertyDef.PropertyName,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.Virtual,
                      null, new[] { propertyDef.Type });
                var setIl = setMethodBuilder.GetILGenerator();

                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldstr, propertyDef.PropertyName);
                setIl.Emit(OpCodes.Ldarg_1);
                if(propertyDef.Type.IsValueType)
                {
                    setIl.Emit(OpCodes.Box, propertyDef.Type);
                }
                setIl.EmitCall(OpCodes.Call, indexerDef.SetMethod, new Type[] { typeof(string) });
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }

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
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    _baseClassType);
        }
    }
}