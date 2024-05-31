using System;

namespace RumbleModUI
{
    /// <summary>
    /// See GoogleDoc for explanation.
    /// </summary>
    public abstract class ModSetting
    {
        #region Constructor
        public ModSetting() { }
        #endregion

        #region Enums
        public enum AvailableTypes
        {
            Description,
            String,
            Integer,
            Float,
            Double,
            Boolean
        }
        #endregion

        #region Variables
        public string Name = "";
        public AvailableTypes ValueType = AvailableTypes.String;
        public string Description = "";
        public int LinkGroup = 0;

        public ValidationParameters ValidationParameters = new ValidationTemplate();
        public Tags Tags = new Tags();

        public virtual event EventHandler<EventArgs> CurrentValueChanged;
        public virtual event EventHandler<EventArgs> SavedValueChanged;
        #endregion


        public abstract string GetValueAsString();
        public abstract object Value { get; set; }
        public abstract object SavedValue { get; set; }

    }

    /// <summary>
    /// EventArgs override.
    /// </summary>
    public class ValueChange<T> : EventArgs
    {
        public ValueChange(T value) { Value = value; }
        public T Value { get; set; }
    }

    /// <summary>
    /// See GoogleDoc for explanation.
    /// </summary>
    public class ModSetting<T> : ModSetting
    {

        private T BG_SaveVariable;
        private T BG_TempVariable;
        public override event EventHandler<EventArgs> CurrentValueChanged;
        public override event EventHandler<EventArgs> SavedValueChanged;
        public override object Value
        {
            get => BG_TempVariable;
            set 
            {
                if (!value.Equals(BG_TempVariable))
                {
                    CurrentValueChanged?.Invoke(this, new ValueChange<T>((T)value));
                    BG_TempVariable = (T)value; 
                }
            }
        }
        public override object SavedValue
        {
            get => BG_SaveVariable;
            set
            {
                if (!value.Equals(BG_SaveVariable))
                {
                    SavedValueChanged?.Invoke(this, new ValueChange<T>((T)value));
                    BG_SaveVariable = (T)value;
                }
            }
        }


        public override string GetValueAsString()
        {
            if (ValueType == AvailableTypes.Boolean) 
            {
                return BG_TempVariable.ToString().ToLower();
            }
            return BG_TempVariable.ToString();
        }

    }
    public class Tags
    {
        public Tags() 
        { 
            IsSummary = false;
            IsEmpty = false;
            IsCustom = false;
            CustomString = ""; 
            IsPassword = false;
            DoNotSave = false;
        }

        public bool IsSummary { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsCustom { get; set; }
        public string CustomString { get; set; }
        public bool IsPassword { get; set; }
        public bool DoNotSave { get; set; }
    }
}
