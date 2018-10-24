using KRF.Core.Entities.ValueList;
using System;
using System.Collections.Generic;

namespace KRF.Core.FunctionalContracts
{
    public interface IValueListManagement<TValueList> where TValueList : ValueList
    {
        /// <summary>
        /// Create Value list based on ValueType. ValueList can be any of AddressType, Category, Color etc..
        /// All Sub-class under ValueList can be created via this.
        /// </summary>
        /// <param name="valueList">Valuelist details</param>
        /// <param name="valueType">Type of value </param>
        /// <returns>return the id of the newly created one.</returns>
        int Create(TValueList valueList, ValueType valueType);

        /// <summary>
        /// Edit Value list details. ValueList can be any of AddressType, Category, Color etc..
        /// All Sub-class under ValueList can be edited via this.
        /// </summary>
        /// <param name="valueList">Valuelist details</param>
        /// <param name="valueType">Type of value </param>
        /// <returns>Updated value list.</returns>
        TValueList Edit(TValueList valueList, ValueType valueType);

        /// <summary>
        /// Delete Value list. ValueList can be any of AddressType, Category, Color etc..
        /// All Sub-class under ValueList can be deleted via this.
        /// </summary>
        /// <param name="valueListId">Valuelist Id</param>
        /// <param name="valueType">Type of value </param>
        /// <returns>return True - if success else False.</returns>
        bool Delete(int valueListId, ValueType valueType);

        /// <summary>
        /// Reorder value list collection.
        /// </summary>
        /// <param name="valueListCollection">Collection of Value list</param>
        /// <param name="valueType">Type of value</param>
        /// <returns>return True - if success else False.</returns>
        bool Reorder(IList<TValueList> valueListCollection, ValueType valueType);

        /// <summary>
        /// Make a value list default selection for the value type.
        /// </summary>
        /// <param name="valueListId">Valuelist Id</param>
        /// <param name="valueType">Type of value </param>
        /// <returns>return True - if success else False.</returns>
        bool MakeDefault(int valueListId, ValueType valueType);

        /// <summary>
        /// Select a value list based on ValueType.
        /// </summary>
        /// <param name="valueListId">Valuelist Id</param>
        /// <param name="valueType">Type of value</param>
        /// <returns>return True - if success else False.</returns>
        TValueList Select(int valueListId, ValueType valueType);

        /// <summary>
        /// Select all value list for a particular ValueType.
        /// </summary>
        /// <param name="valueType">Type of value</param>
        /// <returns>return True - if success else False.</returns>
        IList<TValueList> SelectAll(ValueType valueType);
    }
}
