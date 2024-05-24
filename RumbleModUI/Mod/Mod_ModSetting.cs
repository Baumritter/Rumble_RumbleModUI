using System;

namespace RumbleModUI
{
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
        public DescriptionTags Tags = new DescriptionTags(false, false);

        public virtual event System.Action CurrentValueChanged;
        public event System.Action SavedValueChanged;
        #endregion

        protected void OnCurrentValueChange() { CurrentValueChanged?.Invoke(); }
        protected void OnSavedValueChange() { SavedValueChanged?.Invoke(); }

        public abstract string GetValueAsString();
        public abstract object Value { get; set; }
        public abstract object SavedValue { get; set; }

    }
    public class ModSetting<T> : ModSetting
    {
        private T BG_SaveVariable;
        private T BG_TempVariable;
        public override object Value
        {
            get => BG_TempVariable;
            set 
            {
                if (!value.Equals(BG_TempVariable))
                {
                    OnCurrentValueChange();
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
                    OnSavedValueChange();
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
    public class DescriptionTags
    {
        public DescriptionTags() { }
        public DescriptionTags(bool isSummary, bool isEmpty)
        {
            IsSummary = isSummary;
            IsEmpty = isEmpty;
        }

        public bool IsSummary { get; set; }
        public bool IsEmpty { get; set; }
    }
}
