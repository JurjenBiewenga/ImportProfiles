﻿using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityScript.Lang;
using Object = UnityEngine.Object;

public static class SerializedPropertyExtensionMethod
{
    public static object GetValue(this SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue;
            case SerializedPropertyType.Boolean:
                return property.boolValue;
            case SerializedPropertyType.Float:
                return property.floatValue;
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return property.colorValue;
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return property.intValue;
            case SerializedPropertyType.Enum:
                return property.enumNames[property.enumValueIndex];
            case SerializedPropertyType.Vector2:
                return property.vector2Value;
            case SerializedPropertyType.Vector3:
                return property.vector3Value;
            case SerializedPropertyType.Vector4:
                return property.vector4Value;
            case SerializedPropertyType.Rect:
                return property.rectValue;
            case SerializedPropertyType.ArraySize:
                return property.arraySize;
            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return property.boundsValue;
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue;
            case SerializedPropertyType.ExposedReference:
                return property.exposedReferenceValue;
            case SerializedPropertyType.FixedBufferSize:
                return property.fixedBufferSize;
            case SerializedPropertyType.Vector2Int:
                return property.vector2IntValue;
            case SerializedPropertyType.Vector3Int:
                return property.vector3IntValue;
            case SerializedPropertyType.RectInt:
                return property.rectIntValue;
            case SerializedPropertyType.BoundsInt:
                return property.boundsIntValue;
            case SerializedPropertyType.Generic:
                return null;
            case SerializedPropertyType.Character:
                return null;
            case SerializedPropertyType.Gradient:
                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void SetValue(this SerializedProperty property, object value)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                property.intValue = (int) value;
                break;
            case SerializedPropertyType.Boolean:
                property.boolValue = (bool) value;
                break;
            case SerializedPropertyType.Float:
                property.floatValue = (float) value;
                break;
            case SerializedPropertyType.String:
                property.stringValue = (string) value;
                break;
            case SerializedPropertyType.Color:
                property.colorValue = (Color) value;
                break;
            case SerializedPropertyType.ObjectReference:
                property.objectReferenceValue = (Object) value;
                break;
            case SerializedPropertyType.LayerMask:
                property.intValue = (int) value;
                break;
            case SerializedPropertyType.Enum:
                property.enumValueIndex = System.Array.IndexOf(property.enumNames, value);
                break;
            case SerializedPropertyType.Vector2:
                property.vector2Value = (Vector2) value;
                break;
            case SerializedPropertyType.Vector3:
                property.vector3Value = (Vector3) value;
                break;
            case SerializedPropertyType.Vector4:
                property.vector4Value = (Vector4) value;
                break;
            case SerializedPropertyType.Rect:
                property.rectValue = (Rect) value;
                break;
            case SerializedPropertyType.ArraySize:
                property.arraySize = (int) value;
                break;
            case SerializedPropertyType.AnimationCurve:
                property.animationCurveValue = (AnimationCurve) value;
                break;
            case SerializedPropertyType.Bounds:
                property.boundsValue = (Bounds) value;
                break;
            case SerializedPropertyType.Quaternion:
                property.quaternionValue = (Quaternion) value;
                break;
            case SerializedPropertyType.ExposedReference:
                property.exposedReferenceValue = (Object) value;
                break;
            case SerializedPropertyType.Vector2Int:
                property.vector2IntValue = (Vector2Int) value;
                break;
            case SerializedPropertyType.Vector3Int:
                property.vector3IntValue = (Vector3Int) value;
                break;
            case SerializedPropertyType.RectInt:
                property.rectIntValue = (RectInt) value;
                break;
            case SerializedPropertyType.BoundsInt:
                property.boundsIntValue = (BoundsInt) value;
                break;
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.FixedBufferSize:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static bool AddItem(this SerializedProperty property, object item)
    {
        if (property.isArray)
        {
            property.arraySize++;
            var arrayElement = property.GetArrayElementAtIndex(property.arraySize - 1);
            arrayElement.SetValue(item);
            return true;
        }

        return false;
    }
}