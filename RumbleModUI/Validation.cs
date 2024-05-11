using MelonLoader;

namespace RumbleModUI
{
    public abstract class ValidationParameters
    {
        public abstract bool DoValidation(string Input);
    }
    public class ValidationTemplate : ValidationParameters
    {
        public override bool DoValidation(string Input)
        {
            //Logic for Validation
            //return true when validation successful
            //return false when validation error
            MelonLogger.Error("String Validation not implemented.");
            return true;
        }
    }
}
