#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
#endregion

namespace InitialMargin.Core
{
    /// <summary>Represents the base class from which all the enumerations must derive. This class is abstract.</summary>
    public abstract class Enumeration<T> : IEquatable<Enumeration<T>> where T : Enumeration<T>
    {
        #region Members
        private readonly Int32 m_Index;
        private readonly String m_Description;
        private readonly String m_Name;
        #endregion

        #region Members (Static)
        // ReSharper disable once StaticMemberInGenericType
        private static Int32 s_Offset;
        // ReSharper disable once StaticMemberInGenericType
        private static ReadOnlyCollection<String> s_Names;
        // ReSharper disable once StaticMemberInGenericType
        private static ReadOnlyCollection<T> s_Values;
        #endregion

        #region Properties
        /// <summary>Gets the index of the enumeration member.</summary>
        /// <value>An <see cref="T:System.Int32"/> value.</value>
        public Int32 Index => m_Index;

        /// <summary>Gets the description of the enumeration member.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Description => m_Description;

        /// <summary>Gets the name of the enumeration member.</summary>
        /// <value>A <see cref="T:System.String"/>.</value>
        public String Name => m_Name;
        #endregion

        #region Properties (Static)
        /// <summary>Gets the list of the enumeration member names.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> of names.</value>
        public static ReadOnlyCollection<String> Names
        {
            get
            {
                if (s_Names == null)
                {
                    List<String> names = Values.Select(x => x.Name).ToList();
                    s_Names = names.AsReadOnly();
                }

                return s_Names;
            }
        }

        /// <summary>Gets the list of the enumeration member values.</summary>
        /// <value>A <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/> of values.</value>
        public static ReadOnlyCollection<T> Values
        {
            get
            {
                if (s_Values == null)
                {
                    FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
                    Int32 fieldInfosCount = fieldInfos.Length;
            
                    List<T> values = new List<T>(fieldInfosCount);

                    for (Int32 i = 0; i < fieldInfos.Length; ++i)
                    {
                        FieldInfo fieldInfo = fieldInfos[i];
                        Object fieldValue = fieldInfo.GetValue(null);

                        if (fieldValue is T value)
                            values.Add(value);
                    }

                    s_Values = values.AsReadOnly();
                }

                return s_Values;
            }
        }
        #endregion

        #region Constructors
        /// <summary>Represents the base constructor used by derived classes.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <param name="description">The <see cref="T:System.String"/> representing the description of the enumeration member.</param>
        protected Enumeration(String name, String description)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name specified.", nameof(name));

            if (String.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Invalid description specified.", nameof(description));

            m_Index = s_Offset++;
            m_Description = description.Trim();
            m_Name = name.Trim();
        }
        #endregion

        #region Methods
        /// <summary>Indicates whether the current instance is equal to the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current instance.</param>
        /// <returns><c>true</c> if <paramref name="obj">obj</paramref> is an instance of <see cref="InitialMargin.Core.Enumeration{T}"/> and is equal to the current instance; otherwise, <c>false</c>.</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as Enumeration<T>);
        }

        /// <summary>Indicates whether the current instance is equal to the specified object of the same type.</summary>
        /// <param name="other">The <see cref="InitialMargin.Core.Enumeration{T}"/> to compare with the current object.</param>
        /// <returns><c>true</c> if <paramref name="other">other</paramref> is equal to the current instance; otherwise, <c>false</c>.</returns>
        public Boolean Equals(Enumeration<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return (m_Name == other.Name); 
        }

        /// <summary>Returns the hash code of the current instance.</summary>
        /// <returns>An <see cref="T:System.Int32"/> representing the hash code of the current instance.</returns>
        public override Int32 GetHashCode()
        {
            return m_Name.GetHashCode();
        } 

        /// <summary>Returns the text representation of the current instance.</summary>
        /// <returns>A <see cref="T:System.String"/> representing the current instance.</returns>
        public override String ToString()
        {
            return m_Name;
        }
        #endregion

        #region Methods (Operators)
        /// <summary>Returns a value indicating whether two enumerations are equal.</summary>
        /// <param name="left">The first <see cref="InitialMargin.Core.Enumeration{T}"/> object to compare.</param>
        /// <param name="right">The second <see cref="InitialMargin.Core.Enumeration{T}"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator ==(Enumeration<T> left, Enumeration<T> right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        /// <summary>Returns a value indicating whether two enumerations are not equal.</summary>
        /// <param name="left">The first <see cref="InitialMargin.Core.Enumeration{T}"/> object to compare.</param>
        /// <param name="right">The second <see cref="InitialMargin.Core.Enumeration{T}"/> object to compare.</param>
        /// <returns><c>true</c> if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are not equal; otherwise, <c>false</c>.</returns>
        public static Boolean operator !=(Enumeration<T> left, Enumeration<T> right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods (Static)
        /// <summary>Returns a value indicating whether the specified name corresponds to an existing enumeration member.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <returns><c>true</c> if <paramref name="name">name</paramref> corresponds to an existing enumeration member; otherwise, <c>false</c>.</returns>
        public static Boolean IsDefined(String name)
        {
            return IsDefined(name, false);
        }

        /// <summary>Returns a value indicating whether the specified name corresponds to an existing enumeration member.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive matching; otherwise, <c>false</c>.</param>
        /// <returns><c>true</c> if <paramref name="name">name</paramref> corresponds to an existing enumeration member; otherwise, <c>false</c>.</returns>
        public static Boolean IsDefined(String name, Boolean ignoreCase)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name specified.", nameof(name));

            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            
            return Values.Any(x => String.Compare(name, x.Name, comparison) == 0);
        }

        /// <summary>Tries to convert the name of an enumeration member to the equivalent instance.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <param name="value">The enumeration member corresponding to the <paramref name="name">name</paramref> parameter or <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="name">name</paramref> was successfully converted; otherwise, <c>false</c>.</returns>
        public static Boolean TryParse(String name, out T value)
        {
            return TryParse(name, false, out value);
        }

        /// <summary>Tries to convert the name of an enumeration member to the equivalent instance.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive parsing; otherwise, <c>false</c>.</param>
        /// <param name="value">The enumeration member corresponding to the <paramref name="name">name</paramref> parameter or <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="name">name</paramref> was successfully converted; otherwise, <c>false</c>.</returns>
        public static Boolean TryParse(String name, Boolean ignoreCase, out T value)
        {
            try
            {
                value = Parse(name, ignoreCase);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        /// <summary>Converts the name of an enumeration member to the equivalent instance.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <returns>The enumeration member corresponding to the <paramref name="name">name</paramref> parameter.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="name">name</paramref> is invalid.</exception>
        public static T Parse(String name)
        {
            return Parse(name, false);
        }

        /// <summary>Converts the name of an enumeration member to the equivalent instance.</summary>
        /// <param name="name">The <see cref="T:System.String"/> representing the name of the enumeration member.</param>
        /// <param name="ignoreCase"><c>true</c> to perform a case-insensitive parsing; otherwise, <c>false</c>.</param>
        /// <returns>The enumeration member corresponding to the <paramref name="name">name</paramref> parameter.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <paramref name="name">name</paramref> is invalid.</exception>
        public static T Parse(String name, Boolean ignoreCase)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name specified.", nameof(name));

            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            T value = Values.FirstOrDefault(x => String.Equals(name, x.Name, comparison));

            if (value == null)
                throw new ArgumentException("The specified name is not associated with a value.", nameof(name));

            return value;
        }
        #endregion
    }
}