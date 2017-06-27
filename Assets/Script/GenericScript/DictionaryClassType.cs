using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

public class DictionaryClassType
{
    public DictionaryClassType(string name, Dictionary<string, object> dic)
    {
        this.name = name;
        BuildDynamicTypeWithProperties(dic);
    }

    /// <summary>
    /// 作成した型への値の格納
    /// </summary>
    /// <param name="obj">格納対象</param>
    /// <param name="data">格納するデータ</param>
    public void SetDictionary(object obj, Dictionary<string, object> data)
    {
        Type objectType = obj.GetType();
        if (RetType != objectType)
        {
            throw new Exception("対象外の型のインスタンスが指定されています");
        }

        foreach (PropertyInfo pInfo in RetType.GetProperties())
        {
            string propertyName = pInfo.Name;
            if (data.ContainsKey(propertyName))
            {
                if (data[propertyName].GetType() != pInfo.PropertyType)
                {
                    throw new Exception("次のプロパティの型が異なります：" + propertyName);
                }
                RetType.InvokeMember(propertyName, BindingFlags.SetProperty,
                                     null, obj, new object[] { data[propertyName] });
            }
        }
    }

    /// <summary>
    /// 作成した型からの値の取り出し
    /// </summary>
    /// <param name="obj">取得対象</param>
    /// <returns>取得結果</returns>
    public Dictionary<string, object> GetDictionary(object obj)
    {
        Type objectType = obj.GetType();
        if (RetType != objectType)
        {
            throw new Exception("対象外の型のインスタンスが指定されています");
        }
        Dictionary<string, object> retVal = new Dictionary<string, object>();
        foreach (PropertyInfo pInfo in RetType.GetProperties())
        {
            string propertyName = pInfo.Name;
            object value = RetType.InvokeMember(propertyName, BindingFlags.GetProperty,
                                                    null, obj, new object[] { });
            retVal.Add(propertyName, value);
        }
        return retVal;
    }

    /// <summary>
    /// 作成するクラス名
    /// </summary>
    private string name;
    public string Name
    {
        get { return name; }
    }

    /// <summary>
    /// 作成した型
    /// </summary>
    private Type retType;
    public Type RetType
    {
        get { return retType; }
    }

    /// <summary>
    /// インスタンス生成
    /// </summary>
    /// <returns>インスタンス</returns>
    public object CreateInstance()
    {
        return Activator.CreateInstance(RetType);
    }

    /// <summary>
    /// 型の作成
    /// </summary>
    /// <remarks>
    /// <para>Dictionaryの持つKeyをPropertyとした型を作成する</para>
    /// </remarks>
    /// <param name="dic">基とするDictionary</param>
    private void BuildDynamicTypeWithProperties(Dictionary<string, object> dic)
    {
        // まず
        AppDomain myDomain = Thread.GetDomain();
        AssemblyName myAsmName = new AssemblyName();
        myAsmName.Name = this.Name;

        AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
                                                                       AssemblyBuilderAccess.RunAndSave);
        ModuleBuilder myModBuilder = myAsmBuilder.DefineDynamicModule(myAsmName.Name);

        TypeBuilder myTypeBuilder = myModBuilder.DefineType(Name,
                                                            TypeAttributes.Public);

        foreach (string key in dic.Keys)
        {
            // フィールド作成
            Type valueType = dic[key].GetType();
            FieldBuilder customerNameBldr = myTypeBuilder.DefineField(key + "_Value",
                                            valueType,
                                            FieldAttributes.Private);

            // プロパティ作成
            PropertyBuilder custNamePropBldr = myTypeBuilder.DefineProperty(key,
                                                             PropertyAttributes.HasDefault,
                                                             valueType,
                                                             null);

            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // getter作成
            MethodBuilder custNameGetPropMthdBldr =
                myTypeBuilder.DefineMethod("get_" + key,
                                           getSetAttr,
                                           valueType,
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIL.Emit(OpCodes.Ret);

            // setter作成
            MethodBuilder custNameSetPropMthdBldr =
                myTypeBuilder.DefineMethod("set_" + key,
                                           getSetAttr,
                                           null,
                                           new Type[] { valueType });

            ILGenerator custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIL.Emit(OpCodes.Ret);

            // getter,setterとプロパティとの紐付け
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
        }
        this.retType = myTypeBuilder.CreateType();
    }
}