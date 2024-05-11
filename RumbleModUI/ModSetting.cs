using RumbleModUI;
using System;
using System.CodeDom;

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
        public StringValidation StringValidation = new StringValidation();
        #endregion

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
            set => BG_TempVariable = (T)value;
        }
        public override object SavedValue
        {
            get => BG_SaveVariable;
            set => BG_SaveVariable = (T)value;
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

}
