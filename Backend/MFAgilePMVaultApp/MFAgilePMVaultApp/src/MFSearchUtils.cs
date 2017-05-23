using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFilesAPI;

namespace MFAgilePMVaultApp
{
    class MFSearchUtils
    {

        public static ObjectSearchResults SearchForObjectsByConditions(Vault v, SearchConditions scs)
        {
            return v.ObjectSearchOperations.SearchForObjectsByConditionsEx(
                    scs, MFSearchFlags.MFSearchFlagNone, false, 500, 60);
        }

        /// <summary>
        /// Gets the specified property value as a string (DisplayValue), or null if not found or value is empty
        /// </summary>
        /// <param name="props">PropertyValues collection to search in</param>
        /// <param name="propDef">The property definition to look for</param>
        /// <returns>DisplayValue of the requested property, or null if not found or value is empty</returns>
        public static string getPropertyDisplayValue(PropertyValues props, int propDef)
        {
            int propIndex = props.IndexOf(propDef);
            if (propIndex != -1)
            {
                string val = props[propIndex].TypedValue.DisplayValue;
                if (val != "")
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the specified property value as int (Lookup), or null if not found or value is empty
        /// </summary>
        /// <param name="props">PropertyValues collection to search in</param>
        /// <param name="propDef">The property definition to look for</param>
        /// <returns>DisplayValue of the requested property, or -1 if not found or value is empty</returns>
        public static int getLookupPropertyAsInt(PropertyValues props, int propDef)
        {
            int propIndex = props.IndexOf(propDef);
            if (propIndex != -1)
            {
                return props[propIndex].Value.GetLookupID();
            }
            else
            {
                return -1;
            }
        }

        public static Lookups getMultiSelectLookupPropertyAsIntList(PropertyValues props, int propDef)
        {
            int propIndex = props.IndexOf(propDef);
            if (propIndex != -1)
            {
                return props[propIndex].Value.GetValueAsLookups();
            }
            else
            {
                // retrun empty list
                return new Lookups();
            }
        }

        /// <summary>
        /// search condition of whether the given text property starts with the specified value
        /// </summary>
        /// <param name="propertyID">property to look for</param>
        /// <param name="propertyValue">value of property</param>
        /// <returns>search condition</returns>
        public static SearchCondition startsWith(int propertyID, string propertyValue)
        {
            SearchCondition hasProp = new SearchCondition();
            hasProp.Expression.SetPropertyValueExpression(propertyID, MFParentChildBehavior.MFParentChildBehaviorNone);
            hasProp.ConditionType = MFConditionType.MFConditionTypeStartsWith;
            hasProp.TypedValue.SetValue(MFDataType.MFDatatypeText, propertyValue);
            return hasProp;
        }


        /// <summary>
        /// returns search condition checking to see if the object is deleted
        /// search conditions are used by vault.ObjectSearchOperations etc
        /// checks to see if deleted based on bool passed in 
        /// </summary>
        /// <param name="deleted">true: looking for deleted objects, false: looking for existing objects</param>
        /// <returns>search condition</returns>
        public static SearchCondition isDeletedSearchCondition(bool deleted)
        {
            SearchCondition isDeleted = new SearchCondition();
            isDeleted.Expression.SetStatusValueExpression(MFStatusType.MFStatusTypeDeleted);
            isDeleted.ConditionType = MFConditionType.MFConditionTypeEqual;
            isDeleted.TypedValue.SetValue(MFDataType.MFDatatypeBoolean, deleted);
            return isDeleted;
        }

        /// <summary>
        /// returns search condition checking to see if the object type is the same as the one passed in
        /// search conditions are used by vault.ObjectSearchOperations etc
        /// </summary>
        /// <param name="objectType">object type to check for, generally an int found on the server</param>
        /// <returns>search condition checking for object type</returns>
        public static SearchCondition isObjectType(object objectType)
        {
            SearchCondition isObject = new SearchCondition();
            isObject.Expression.SetStatusValueExpression(MFStatusType.MFStatusTypeObjectTypeID);
            isObject.ConditionType = MFConditionType.MFConditionTypeEqual;
            isObject.TypedValue.SetValue(MFDataType.MFDatatypeLookup, objectType);
            return isObject;
        }

        public static SearchCondition isObjectClass(object objectClass)
        {
            SearchCondition isObject = new SearchCondition();
            isObject.Expression.DataPropertyValuePropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass;
            isObject.ConditionType = MFConditionType.MFConditionTypeEqual;
            isObject.TypedValue.SetValue(MFDataType.MFDatatypeLookup, objectClass);
            return isObject;
        }

        public static SearchCondition isOwner(int ownerPropertyDef, object ownerId)
        {
            SearchCondition isOwner = new SearchCondition();
            isOwner.Expression.DataPropertyValuePropertyDef = ownerPropertyDef;
            isOwner.ConditionType = MFConditionType.MFConditionTypeEqual;
            isOwner.TypedValue.SetValue(MFDataType.MFDatatypeLookup, ownerId);
            return isOwner;
        }

        public static SearchCondition searchConditionID(int id)
        {
            SearchCondition isID = new SearchCondition();
            isID.Expression.SetStatusValueExpression(MFStatusType.MFStatusTypeObjectID);
            isID.ConditionType = MFConditionType.MFConditionTypeEqual;
            isID.TypedValue.SetValue(MFDataType.MFDatatypeInteger, id);
            return isID;
        }

        public static SearchCondition isClass(int classNum)
        {
            SearchCondition isClassNum = new SearchCondition();
            isClassNum.Expression.DataPropertyValuePropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass;
            isClassNum.ConditionType = MFConditionType.MFConditionTypeEqual;
            isClassNum.TypedValue.SetValue(MFDataType.MFDatatypeLookup, classNum);
            return isClassNum;
        }

        public static SearchCondition propertyNotEmpty(int propertyID)
        {
            SearchCondition hasProperty = new SearchCondition();
            hasProperty.Expression.DataPropertyValuePropertyDef = propertyID;
            hasProperty.ConditionType = MFConditionType.MFConditionTypeNotEqual;
            hasProperty.TypedValue.SetValue(MFDataType.MFDatatypeLookup, "");
            return hasProperty;
        }

        public static SearchCondition propertyNotEmpty(int propertyID, MFDataType datatype)
        {
            SearchCondition hasProperty = new SearchCondition();
            hasProperty.Expression.DataPropertyValuePropertyDef = propertyID;
            hasProperty.ConditionType = MFConditionType.MFConditionTypeNotEqual;
            hasProperty.TypedValue.SetValue(datatype, "");
            return hasProperty;
        }

        public static SearchCondition propertyIsEmpty(int propertyID, MFDataType datatype)
        {
            SearchCondition hasProperty = new SearchCondition();
            hasProperty.Expression.DataPropertyValuePropertyDef = propertyID;
            hasProperty.ConditionType = MFConditionType.MFConditionTypeEqual;
            hasProperty.TypedValue.SetValue(datatype, "");
            return hasProperty;
        }

        /// <summary>
        /// search condition of whether the property has the specified value
        /// </summary>
        /// <param name="propertyID">property to look for</param>
        /// <param name="propertyValue">value of property</param>
        /// <returns>search condition</returns>
        public static SearchCondition hasProperty(int propertyID, int propertyValue)
        {
            SearchCondition hasProp = new SearchCondition();
            hasProp.Expression.SetPropertyValueExpression(propertyID, MFParentChildBehavior.MFParentChildBehaviorNone);
            hasProp.ConditionType = MFConditionType.MFConditionTypeEqual;
            hasProp.TypedValue.SetValue(MFDataType.MFDatatypeLookup, propertyValue);
            return hasProp;
        }

        /// <summary>
        /// search condition of whether the property has the specified value
        /// </summary>
        /// <param name="propertyID">property to look for</param>
        /// <param name="propertyValue">value of property</param>
        /// <returns>search condition</returns>
        public static SearchCondition hasPropertyMultiSelect(int propertyID, int propertyValue)
        {
            SearchCondition hasProp = new SearchCondition();
            hasProp.Expression.SetPropertyValueExpression(propertyID, MFParentChildBehavior.MFParentChildBehaviorNone);
            hasProp.ConditionType = MFConditionType.MFConditionTypeEqual;
            hasProp.TypedValue.SetValue(MFDataType.MFDatatypeMultiSelectLookup, propertyValue);
            return hasProp;
        }


        /// <summary>
        /// search condition of whether the property has the specified value
        /// </summary>
        /// <param name="propertyID">property to look for</param>
        /// <param name="propertyValue">value of property</param>
        /// <returns>search condition</returns>
        public static SearchCondition hasProperty(int propertyID, string propertyValue, MFDataType type)
        {
            SearchCondition hasProp = new SearchCondition();
            hasProp.Expression.SetPropertyValueExpression(propertyID, MFParentChildBehavior.MFParentChildBehaviorNone);
            hasProp.ConditionType = MFConditionType.MFConditionTypeEqual;
            hasProp.TypedValue.SetValue(type, propertyValue);
            return hasProp;
        }

        /// <summary>
        /// search condition of whether the property has the specified value
        /// </summary>
        /// <param name="propertyID">property to look for</param>
        /// <param name="propertyValue">value of property</param>
        /// <returns>search condition</returns>
        public static SearchCondition hasProperty(int propertyID, bool propertyValue)
        {
            SearchCondition hasProp = new SearchCondition();
            hasProp.Expression.SetPropertyValueExpression(propertyID, MFParentChildBehavior.MFParentChildBehaviorNone);
            hasProp.ConditionType = MFConditionType.MFConditionTypeEqual;
            hasProp.TypedValue.SetValue(MFDataType.MFDatatypeBoolean, propertyValue);
            return hasProp;
        }

        /// <summary>
        /// search condition of whether the property has the specified value
        /// </summary>
        /// <param name="propertyID">property to look for</param>
        /// <param name="propertyValue">value of property</param>
        /// <returns>search condition</returns>
        public static SearchCondition hasProperty(int propertyID, string propertyValue)
        {
            SearchCondition hasProp = new SearchCondition();
            hasProp.Expression.SetPropertyValueExpression(propertyID, MFParentChildBehavior.MFParentChildBehaviorNone);
            hasProp.ConditionType = MFConditionType.MFConditionTypeEqual;
            hasProp.TypedValue.SetValue(MFDataType.MFDatatypeText, propertyValue);
            return hasProp;
        }

        public static SearchCondition SearchConditionMinObjID(Int32 Segment, Int32 Range)
        {
            SearchCondition oSearchCondition = new SearchCondition();
            oSearchCondition.ConditionType = MFConditionType.MFConditionTypeGreaterThanOrEqual;
            oSearchCondition.Expression.SetStatusValueExpression(MFStatusType.MFStatusTypeObjectID, null);
            oSearchCondition.TypedValue.SetValue(MFDataType.MFDatatypeInteger, Range * Segment);
            return oSearchCondition;
        }

        public static SearchCondition SearchConditionSegment(Int32 Segment, Int32 Range)
        {
            SearchCondition oSearchCondition = new SearchCondition();
            oSearchCondition.ConditionType = MFConditionType.MFConditionTypeEqual;
            oSearchCondition.Expression.SetObjectIDSegmentExpression(Range);
            oSearchCondition.TypedValue.SetValue(MFDataType.MFDatatypeInteger, Segment);
            return oSearchCondition;
        }

        /// <summary>
        /// Returns search condition based on whether the item is checked out or not
        /// </summary>
        /// <param name="checkedOut">true: looking for checked out items, false: looking for checked in items</param>
        /// <returns></returns>
        public static SearchCondition SearchConditionCheckedOut(bool checkedOut)
        {
            SearchCondition oSearchCondition = new SearchCondition();
            oSearchCondition.ConditionType = MFConditionType.MFConditionTypeEqual;
            oSearchCondition.Expression.SetStatusValueExpression(MFStatusType.MFStatusTypeCheckedOut);
            oSearchCondition.TypedValue.SetValue(MFDataType.MFDatatypeBoolean, checkedOut);
            return oSearchCondition;
        }

        public static SearchCondition SearchConditionExtID(string extid)
        {
            SearchCondition oSearchCondition = new SearchCondition();
            oSearchCondition.ConditionType = MFConditionType.MFConditionTypeEqual;
            oSearchCondition.Expression.SetStatusValueExpression(MFStatusType.MFStatusTypeExtID);
            oSearchCondition.TypedValue.SetValue(MFDataType.MFDatatypeText, extid);
            return oSearchCondition;
        }

    }
}
