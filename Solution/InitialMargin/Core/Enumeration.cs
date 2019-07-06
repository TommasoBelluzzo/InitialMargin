#region Using Directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
#endregion

namespace InitialMargin.Core
{
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
        public Int32 Index => m_Index;

        public String Description => m_Description;

        public String Name => m_Name;
        #endregion

        #region Properties (Static)
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
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as Enumeration<T>);
        }

        public Boolean Equals(Enumeration<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return (m_Name == other.Name); 
        }

        public override Int32 GetHashCode()
        {
            return m_Name.GetHashCode();
        } 

        public override String ToString()
        {
            return m_Name;
        }
        #endregion

        #region Methods (Operators)
        public static Boolean operator ==(Enumeration<T> left, Enumeration<T> right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static Boolean operator !=(Enumeration<T> left, Enumeration<T> right)
        {
            return !(left == right);
        }
        #endregion

        #region Methods (Static)
        public static Boolean IsDefined(String name)
        {
            return IsDefined(name, false);
        }

        public static Boolean IsDefined(String name, Boolean ignoreCase)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name specified.", nameof(name));

            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            
            return Values.Any(x => String.Compare(name, x.Name, comparison) == 0);
        }

        public static Boolean TryParse(String name, out T value)
        {
            return TryParse(name, false, out value);
        }

        public static Boolean TryParse(String name, Boolean ignoreCase, out T value)
        {
            try
            {
                value = Parse(name, ignoreCase);
                return true;
            }
            catch (ArgumentException)
            {
                value = null;
                return false;
            }
        }

        public static T Parse(String name)
        {
            return Parse(name, false);
        }

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